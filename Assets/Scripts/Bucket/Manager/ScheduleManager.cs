using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Bucket.Manager
{
    public enum CalendarUIType
    {
        Day,
        Week,
        Month,
        Year
    }

    public enum CalendarUIDataType
    {
        Nothing,
        Fight,
        Excercise,
        Event,
    }
    
    public class ScheduleManager : MonoBehaviour, IListener
    {
        public static ScheduleManager Instance
        {
            get { return _instance; }
        }

        private static ScheduleManager _instance = null;

        public int Date
        {
            get => date;
        }

        private int date = 0;
        [SerializeField] private CalendarUIType currentCalendarUIType = CalendarUIType.Day;
        [SerializeField] private GameObject currentCalendarUITypeObject;

        [Space(10)] [Header("Schedule UI")]
        [SerializeField] private GameObject calendarContentBox;
        
        [Space(10)] [Header("ETC")]
        public SerializableDictionary<int, DailySchedules> ScheduleList = new SerializableDictionary<int, DailySchedules>();

        public int CurrentDisplayDate = 0;
        [SerializeField] private TextMeshProUGUI dateText;
        [SerializeField] private GameObject dayDataPrefab;
        [SerializeField] private GameObject weekDataPrefab;

        void Awake()
        {
            //관리 페이즈에만 존재하는 싱글톤 ㅎㅎ
            if (_instance == null)
            {
                _instance = this;
                //DontDestroyOnLoad(gameObject);
                //return;
            }
            //DestroyImmediate(gameObject);
            
            AddScheduleToList(new Schedule(1, 0, 1, CalendarUIDataType.Event, "Special Event!"));
            AddScheduleToList(new Schedule(1, 1, 3,  CalendarUIDataType.Fight));
            AddScheduleToList(new Schedule(3, 0, 2, CalendarUIDataType.Excercise));
            AddScheduleToList(new Schedule(8, 1, 2, CalendarUIDataType.Excercise));
            AddScheduleToList(new Schedule(15, 1, 2, CalendarUIDataType.Excercise));
            AddScheduleToList(new Schedule(18, 1, 2, CalendarUIDataType.Excercise));
        }

        void Start()
        {
            currentCalendarUITypeObject = calendarContentBox.transform.GetChild((int)currentCalendarUIType).gameObject;
            
            EventManager.Instance.AddListener(EVENT_TYPE.eChallengeMail, this);

            DatePass();

        }

        public void OnEvent(EVENT_TYPE EventType, Component Sender, object Param1 = null, object Param2 = null)
        {
            switch (EventType)
            {
                case EVENT_TYPE.eChallengeMail:
                    //스케쥴에 결투 일정 표시
                    break;
            }
        }

        private void DatePass()
        {
            date++;
            CurrentDisplayDate = date;

            dateText.text = $"{date} 일차";

            /*if (needPlayerFightCaculate)
            {
                //mmr 계산 해야함
                /*scheduleList[date - 2].table1.winloss = false;
                needPlayerFightCaculate = false;#1#

                CaculateWinner(scheduleList[date - 2], 1);
                needPlayerFightCaculate = false;
            }

            CaculateWinner(scheduleList[date - 1], 1);
            CaculateWinner(scheduleList[date - 1], 2);

            EventManager.Instance.PostNotification(EVENT_TYPE.eDatePass, this, date);

            RefreshScheduleUI();

            string newsText = "";
            if (scheduleList[date - 1].table1.winloss != null) newsText += scheduleList[date - 1].table1.GetWinner(true)?.FighterName + " wins " +
                                                           scheduleList[date - 1].table1.GetWinner(false)?.FighterName + "\n";
            newsText += scheduleList[date - 1].table2.GetWinner(true)?.FighterName + " wins " +
                        scheduleList[date - 1].table2.GetWinner(false)?.FighterName + "\n";
            ManagementPhaseManager.Instance.OpenDailyNews(date.ToString() + " 일차 소식", newsText);*/

            //TODO
            //스케쥴 최신화
            RefreshCalendarUI();
            
            //데일리 뉴스 띄우기
        }

        public void RefreshCalendarUI()
        {
            CancelInvoke();
            switch (currentCalendarUIType)
            {
                case CalendarUIType.Day:
                    calendarContentBox.GetComponent<VerticalLayoutGroup>().childControlHeight = false;

                    MakeDayDataFormat();
                    Invoke(nameof(MakeDayDataDetail), 0);
                    
                    break;
                case CalendarUIType.Week:
                    calendarContentBox.GetComponent<VerticalLayoutGroup>().childControlHeight = true;
                    
                    MakeWeekDataFormat();
                    Invoke(nameof(MakeWeekDataDetail), 0);
                    
                    break;
                case CalendarUIType.Month:
                    calendarContentBox.GetComponent<VerticalLayoutGroup>().childControlHeight = true;
                    
                    MakeMonthDataFormat();
                    Invoke(nameof(MakeMonthDataDetail), 0);
                    
                    break;
            }
        }
        
        public void ChangeCurrentDisplayDate(int amount)
        {
            if (currentCalendarUIType == CalendarUIType.Day)
            {
                CurrentDisplayDate = CurrentDisplayDate + amount > 0 ?  CurrentDisplayDate + amount : CurrentDisplayDate;
            }
            else CurrentDisplayDate = CurrentDisplayDate + amount * 7 > 0 ?  CurrentDisplayDate + amount * 7 : CurrentDisplayDate;
            
            RefreshCalendarUI();
        }

        public void SwitchCalendarUI(CalendarUIType calendarUIType)
        {
            CurrentDisplayDate = date;
            
            currentCalendarUIType = calendarUIType;
            currentCalendarUITypeObject.SetActive(false);
            currentCalendarUITypeObject = calendarContentBox.transform.GetChild((int)currentCalendarUIType).gameObject;
            currentCalendarUITypeObject.SetActive(true);
            
            RefreshCalendarUI();
        }

        public void SwitchCalendarUI(string type)
        {
            switch (type)
            {
                case "Day":
                    SwitchCalendarUI(CalendarUIType.Day);
                    break;
                case "Week":
                    SwitchCalendarUI(CalendarUIType.Week);
                    break;
                case "Month":
                    SwitchCalendarUI(CalendarUIType.Month);
                    break;
            }
        }

        private void MakeDayDataObject(int start, int end, Color color) //0~24
        {
            GameObject dayObj = Instantiate(dayDataPrefab, currentCalendarUITypeObject.transform);
            Image image = dayObj.GetComponent<Image>();

            dayObj.transform.rotation = Quaternion.Euler(0, 0, -start * 15);
            image.fillAmount = (end - start) / 24f;
            image.color = color;
        }
        
        private void MakeDayDataFormat()
        {
            foreach (Transform child in currentCalendarUITypeObject.transform)
            {
                Destroy(child.gameObject);
            }
            
            int activePoint = ManagementPhaseManager.Instance.dailyActivePoint;
            if (activePoint == 3)
            {
                MakeDayDataObject(22, 31, Color.black);                
                MakeDayDataObject(7, 12, Color.red);
                MakeDayDataObject(12, 17, Color.blue);
                MakeDayDataObject(17, 22, Color.green);
            }
            else if (activePoint == 4)
            {
                MakeDayDataObject(22, 30, Color.black);                
                MakeDayDataObject(6, 10, Color.red);
                MakeDayDataObject(10, 14, Color.blue);
                MakeDayDataObject(14, 18, Color.green);
                MakeDayDataObject(18, 22, Color.yellow);
            }
        }

        private int indexToRealTime(int i)
        {
            if (ManagementPhaseManager.Instance.dailyActivePoint == 3)
            {
                return 7 + i * 5;
            }
            else if (ManagementPhaseManager.Instance.dailyActivePoint == 4)
            {
                return 6 + i * 4;
            }

            //나중에 행동력이 너무 많이 증가할 수 있다면 그냥 자는 시간 고정하고 분할하기 14 / 8 이런 식으로
            Debug.LogError("Need to Newly Caculate Time");
            return 0;
        }

        private void MakeDayDataDetail()
        {
            if (!ScheduleList.TryGetValue(CurrentDisplayDate, out var value)) return;
            List<Schedule> sl = value.schedules;
            
            foreach (Schedule s in sl)
            {
                int x = s.startTime;
                int y = s.endTime;

                Transform target = currentCalendarUITypeObject.transform.GetChild(y);

                Image i = target.GetComponent<Image>();
                i.transform.rotation = Quaternion.Euler(0, 0, -indexToRealTime(x) * 15);
                i.fillAmount *= (y - x);
            }
        }

        private void MakeWeekDataFormat()
        {
            GameObject dataObj = currentCalendarUITypeObject.transform.GetChild(1).gameObject;

            foreach (Transform child in dataObj.transform)
            {
                Destroy(child.gameObject);
            }

            int activePoint = ManagementPhaseManager.Instance.dailyActivePoint;
            for (int i = 0; i < activePoint * 7; i++)
            {
                Instantiate(weekDataPrefab, dataObj.transform);
            }
        }

        private void MakeWeekDataDetail()
        {
            GameObject dataObj = currentCalendarUITypeObject.transform.GetChild(1).gameObject;
            
            //showing start to end
            int startDay = CurrentDisplayDate - CurrentDisplayDate % 7 + 1;
            int endDay = startDay + 6;

            for (int i = startDay; i <= endDay; i++)
            {
                if (!ScheduleList.TryGetValue(i, out var value)) continue;
                List<Schedule> sl = value.schedules;
                
                // a th day x - y --> a + 7x - 1 ~ a + 7y - 1
                foreach (Schedule s in sl)
                {
                    int x = s.startTime;
                    int y = s.endTime;

                    Transform target = dataObj.transform.GetChild(i % 7 + 7 * (y - 1) - 1);
                    
                    target.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(100, 105 * (y - x) - 5);
                    target.GetChild(0).GetComponent<Image>().color = Color.yellow;
                    target.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = s.ToString();
                }
            }
        }
        
        private void MakeMonthDataFormat()
        {
            GameObject dataObj = currentCalendarUITypeObject.transform.GetChild(1).gameObject;

            foreach (Transform child in dataObj.transform)
            {
                Destroy(child.gameObject);
            }
            
            for (int i = 0; i < 5 * 7; i++)
            {
                Instantiate(weekDataPrefab, dataObj.transform);
            }
        }
        
        private void MakeMonthDataDetail()
        {
            GameObject dataObj = currentCalendarUITypeObject.transform.GetChild(1).gameObject;
            
            //showing start to end
            int startWeek = (CurrentDisplayDate - 1) / 7 + 1;
            int endWeek = startWeek + 4;

            for (int i = startWeek; i <= endWeek; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    int targetDay = (i - 1) * 7 + j + 1;
                    if (!ScheduleList.TryGetValue(targetDay, out var value)) continue;
                    List<Schedule> sl = value.schedules;

                    string s = "";
                    foreach (Schedule sc in sl)
                    {
                        s += sc + "\n";
                    }

                    s.TrimEnd();
                    
                    dataObj.transform.GetChild(targetDay - 1 - (startWeek - 1) * 7).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = s;
                }
            }
        }
        
        public void AddScheduleToList(Schedule schedule)
        {
            int date = schedule.scheduleDate;
            foreach (KeyValuePair<int, DailySchedules> s in ScheduleList)
            {
                if (s.Key == date)
                {
                    s.Value.AddSchedule(schedule);
                    return;
                }
            }
            ScheduleList.Add(date, new DailySchedules());
            ScheduleList[date].AddSchedule(schedule);
        }
        
        public void RemoveScheduleFromList(Schedule schedule)
        {
            int date = schedule.scheduleDate;
            foreach (KeyValuePair<int, DailySchedules> s in ScheduleList)
            {
                if (s.Key == date)
                {
                    s.Value.RemoveSchedule(schedule);
                    if (s.Value.schedules.Count == 0)
                    {
                        ScheduleList.Remove(date);
                    }
                    return;
                }
            }
        }
    }

    //시간을 0 - 1 - 2 - 3으로 분할 (0-1이 아침, 1-2가 점심, 2-3이 저녁, 0-2면 아침 점심 다 하는거)
    //행동력이 4면 4까지 확장
    [Serializable]
    public class Schedule
    {
        public int scheduleDate;
        public int startTime;
        public int endTime;
        public CalendarUIDataType scheduleType;

        //type : fight
        public Fighter fighter1 = null;
        public Fighter fighter2 = null;
        
        //type : event
        public string eventName = "";

        public Schedule(int date, int startTime, int endTime, CalendarUIDataType scheduleType)
        {
            scheduleDate = date;
            this.startTime = startTime;
            this.endTime = endTime;
            this.scheduleType = scheduleType;
        }
        
        public Schedule(int date, int startTime, int endTime, CalendarUIDataType scheduleType, Fighter fighter1, Fighter fighter2)
        {
            scheduleDate = date;
            this.startTime = startTime;
            this.endTime = endTime;
            this.scheduleType = scheduleType;
            
            this.fighter1 = fighter1;
            this.fighter2 = fighter2;
        }

        public Schedule(int date, int startTime, int endTime, CalendarUIDataType scheduleType, string eventName)
        {
            scheduleDate = date;
            this.startTime = startTime;
            this.endTime = endTime;
            this.scheduleType = scheduleType;
            
            this.eventName = eventName;
        }

        public override string ToString()
        {
            switch (scheduleType)
            {
                case CalendarUIDataType.Fight:
                    if (fighter1 == null || fighter2 == null) return "??? vs ???";
                    return fighter1.ToString() + " vs " + fighter2.ToString();
                case CalendarUIDataType.Excercise:
                    return "훈련!";
                case CalendarUIDataType.Event:
                    return eventName;
                default:
                    Debug.LogError("Need to name the type of schedule to be string");
                    return "???";
            }
        }
    }

    [Serializable]
    public class DailySchedules
    {
        public List<Schedule> schedules = new  List<Schedule>();

        public void AddSchedule(Schedule schedule)
        {
            if (!schedules.Contains(schedule)) schedules.Add(schedule);
        }

        public void RemoveSchedule(Schedule schedule)
        {
            if (schedules.Contains(schedule))
            {
                schedules.Remove(schedule);
            }
        }
    }

    [Serializable]
    public class SerializableKeyValue<K, V>
    {
        public K key;
        public V value;
    }
    
    [Serializable]
    public class SerializableDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<SerializableKeyValue<K, V>> _keyValueList;

        public void OnBeforeSerialize() 
        {
            if (this.Count < _keyValueList.Count)
            {
                return;
            }

            _keyValueList.Clear();
        
            foreach (var kv in this)
            {
                _keyValueList.Add(new SerializableKeyValue<K, V>()
                {
                    key = kv.Key,
                    value = kv.Value
                });
            }        
        }

        public void OnAfterDeserialize() 
        {
            this.Clear();
            foreach (var kv in _keyValueList)
            {
                if(!this.TryAdd(kv.key, kv.value))
                {
                    Debug.LogError($"List has duplicate Key");
                }
            }
        }
    }
}
