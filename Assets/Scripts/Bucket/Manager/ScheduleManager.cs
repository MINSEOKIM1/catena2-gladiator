using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        REST,
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
        [SerializeField] private int date = 0;
        
        public int Time
        {
            get => time;
        }
        [SerializeField] private int time = 0;
        
        [SerializeField] private CalendarUIType currentCalendarUIType = CalendarUIType.Day;
        [SerializeField] private GameObject currentCalendarUITypeObject;

        [Space(10)] [Header("Schedule UI")]
        [SerializeField] private GameObject calendarContentBox;

        [Space(10)] [Header("UI")] 
        [SerializeField] private GameObject PlayEventButton;
        
        [Space(10)] [Header("ETC")]
        public SerializableDictionary<int, DailySchedules> ScheduleList = new SerializableDictionary<int, DailySchedules>();

        public int CurrentDisplayDate = 0;
        [SerializeField] private TextMeshProUGUI dateText;
        [SerializeField] private GameObject dayDataPrefab;
        [SerializeField] private GameObject weekDataPrefab;

        delegate void MyFunc();

        void Awake()
        {
            //관리 페이즈에만 존재하는 싱글톤 ㅎㅎ
            if (_instance == null)
            {
                _instance = this;
                // DontDestroyOnLoad(gameObject);
                // return;
            }
            // DestroyImmediate(gameObject);
            
            currentCalendarUITypeObject = calendarContentBox.transform.GetChild((int)currentCalendarUIType).gameObject;

            if (DataManager.Instance.scheduleList != null)
            {
                date = DataManager.Instance.date;
                time = DataManager.Instance.time;
                ScheduleList = DataManager.Instance.scheduleList;
            }
        }

        void Start()
        {
            EventManager.Instance.AddListener(EVENT_TYPE.eChangeOnSchedule, this);
            
            if (!DataManager.Instance.firstDatePass)
            {
                DatePass();
                DataManager.Instance.firstDatePass = true;
            }

            //TODO 데이터매니저에서 스케쥴 빼오기
            /*
            ScheduleList = LoadScheduleData()
            EventManager.Instance.PostNotification(EVENT_TYPE.eChangeOnSchedule, this);
             */

            //테스트용으로 스케쥴 만들기임
            // AddScheduleToList(new Schedule(1, 0, 1, CalendarUIDataType.Event, "Special Event!"));
            // AddScheduleToList(new Schedule(1, 1, 3,  CalendarUIDataType.Fight));
            // AddScheduleToList(new Schedule(2, 1, 3,  CalendarUIDataType.Fight));
            // AddScheduleToList(new Schedule(3, 0, 2, CalendarUIDataType.Excercise));
            // AddScheduleToList(new Schedule(7, 1, 2, CalendarUIDataType.REST));
            // AddScheduleToList(new Schedule(8, 1, 2, CalendarUIDataType.REST));
            // AddScheduleToList(new Schedule(15, 1, 2, CalendarUIDataType.Excercise));
            // AddScheduleToList(new Schedule(18, 1, 2, CalendarUIDataType.Excercise));
        }

        private void Update()
        {
            RefreshCalendarUI();
        }

        public void OnEvent(EVENT_TYPE eventType, Component sender, object param1 = null, object param2 = null)
        {
            switch (eventType)
            {
                case EVENT_TYPE.eChangeOnSchedule:
                    RefreshCalendarUI();
                    RefreshPlayEventButton();
                    break;
            }
        }

        public Image fadeoutPanel;
        IEnumerator SceneFadeOut()
        {
            fadeoutPanel.gameObject.SetActive(true);
            var color = Color.black;

            while (color.a > 0)
            {
                color.a -= UnityEngine.Time.deltaTime;
                fadeoutPanel.color = color;
                yield return null;
            }
            fadeoutPanel.gameObject.SetActive(false);
        }
        public void DatePass()
        {
            StartCoroutine(SceneFadeOut());
            if (ScheduleList.ContainsKey(date))
            {
                var schedules = ScheduleList[date].schedules;

                foreach (var s in schedules)
                {
                    if (s.startTime >= time)
                    {
                        s.noshow = true;
                        DataManager.Instance.popularity -= 15;
                    }
                }
            }

            DataManager.Instance.hp += (100 - DataManager.Instance.hp) / 2; 
            
            date++;
            CurrentDisplayDate = date;
            time = 0;

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
            
            RefreshCalendarUI();
            RefreshPlayEventButton();
            
            
            
            //TODO 데일리 뉴스 띄우기
            
            EventManager.Instance.PostNotification(EVENT_TYPE.eDatePass, this, date);
            
            DataManager.Instance.SaveDatas();
        }
        
        
        public void AddScheduleToList(Schedule schedule)
        {
            int date = schedule.scheduleDate;
            foreach (KeyValuePair<int, DailySchedules> s in ScheduleList)
            {
                if (s.Key == date)
                {
                    //겹치는 스케쥴을 추가하면 원래거 삭제하기
                    foreach (Schedule sc in s.Value.schedules.ToList())
                    {
                        if (sc.endTime > schedule.startTime || sc.endTime > schedule.startTime)
                        {
                            s.Value.schedules.Remove(sc);
                        }
                    }
                    
                    s.Value.AddSchedule(schedule);
                    
                    EventManager.Instance.PostNotification(EVENT_TYPE.eChangeOnSchedule, this);
                    DataManager.Instance.SaveDatas();
                    return;
                }
            }
                
            ScheduleList.Add(date, new DailySchedules());
            ScheduleList[date].AddSchedule(schedule);
            
            EventManager.Instance.PostNotification(EVENT_TYPE.eChangeOnSchedule, this);
            DataManager.Instance.SaveDatas();
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
                        
                        EventManager.Instance.PostNotification(EVENT_TYPE.eChangeOnSchedule, this);
                        DataManager.Instance.SaveDatas();
                    }
                    return;
                }
            }
        }
#region ScheduleUI
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
                    CurrentDisplayDate = date;
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
            if (!ScheduleList.TryGetValue(CurrentDisplayDate, out DailySchedules value)) return;
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
                var go = Instantiate(weekDataPrefab, dataObj.transform);
                if (i % 7 < CurrentDisplayDate % 7 - 1) go.GetComponentInChildren<Image>().color = Color.grey;
                else if (i % 7 == CurrentDisplayDate % 7 - 1 && i / 7 < time) go.GetComponentInChildren<Image>().color = Color.grey;
            }

            for (int i = 0; i < 7; i++)
            {
                currentCalendarUITypeObject.transform.GetChild(0).GetChild(i).GetComponent<Image>().color = new Color(0 , 0, 0 , 1);
            }
            
            currentCalendarUITypeObject.transform.GetChild(0).GetChild((date - 1) % 7).GetComponent<Image>().color = new Color(255 / 255f, 248 / 255f, 57 / 255f, 1);
        }

        private void MakeWeekDataDetail()
        {
            GameObject dataObj = currentCalendarUITypeObject.transform.GetChild(1).gameObject;
            
            //showing start to end
            int startDay = (CurrentDisplayDate - 1) / 7 * 7 + 1;
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

                    Transform target = dataObj.transform.GetChild((i - 1) % 7 + 7 * (y - 1));
                    
                    target.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(100, 105 * (y - x) - 5);

                    if (s.scheduleDate < date)
                    {
                        if (s.noshow)
                        {
                            target.GetChild(0).GetComponent<Image>().color = Color.red;
                        }
                        else
                        {
                            target.GetChild(0).GetComponent<Image>().color = Color.grey;
                        }
                    }
                    else if (s.scheduleDate == date)
                    {
                        if (s.startTime > time)
                        {
                            target.GetChild(0).GetComponent<Image>().color = Color.yellow;
                        }
                        else if (s.startTime == time && date == s.scheduleDate)
                        {
                            target.GetChild(0).GetComponent<Image>().color = Color.green;
                        }
                        else
                        {
                            if (s.noshow)
                            {
                                target.GetChild(0).GetComponent<Image>().color = Color.red;
                            }
                            else
                            {
                                target.GetChild(0).GetComponent<Image>().color = Color.grey;
                            }
                        }
                    }
                    else
                    {
                        target.GetChild(0).GetComponent<Image>().color = Color.yellow;
                    }

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
            
            for (int i = 0; i < 7; i++)
            {
                currentCalendarUITypeObject.transform.GetChild(0).GetChild(i).GetComponent<Image>().color = new Color(57 / 255f, 169 / 255f, 255 / 255f, 1);
            }
            
            currentCalendarUITypeObject.transform.GetChild(0).GetChild((date - 1) % 7).GetComponent<Image>().color = new Color(255 / 255f, 248 / 255f, 57 / 255f, 1);
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
#endregion UI

#region UI

        private void RefreshPlayEventButton()
        {
            if (ManagementPhaseManager.Instance.dailyActivePoint <= time)
            {
                //하루가 끝난거
                EditPlayEventButton(false, "하루 끝!", () => { });
                return;
            }
            
            if (!ScheduleList.ContainsKey(date))
            {
                //날에 스케쥴이 없을 때
                EditPlayEventButton(true, "시간 때우기", () =>
                {
                    Debug.Log("시간 + 1");
                    time += 1;
                    RefreshPlayEventButton();
                });
                return;
            }
            
            List<Schedule> dailySc = ScheduleList[date].schedules.ToList();
            int maxTime = 99;
            int nextTime = maxTime;

            MyFunc mF = null;
            
            foreach (Schedule sc in dailySc)
            {
                nextTime = time < sc.startTime ? Mathf.Min(nextTime, sc.startTime) : nextTime;
                if (sc.startTime.Equals(time))
                {
                    mF = () =>
                    {
                        time += sc.endTime - sc.startTime;
                        RefreshPlayEventButton();
                    };
                    
                    switch (sc.scheduleType)
                    {
                        //TODO 추후에 새로운 행동 추가시 추가 필요
                        case CalendarUIDataType.Fight:
                            //전투하러 가기
                            mF += () =>
                            {
                                Debug.Log("전투");
                                Debug.Log("하러");
                                Debug.Log("가기");
                            };
                            EditPlayEventButton(true, "전투!", mF);
                            return;
                        case CalendarUIDataType.Excercise:
                            //활동하러 가기
                            mF += () =>
                            {
                                Debug.Log("훈련");
                            };
                            EditPlayEventButton(true, "훈련!", mF);
                            return;
                        case CalendarUIDataType.Event:
                            //이벤트 하러 가기
                            mF += () =>
                            {
                                Debug.Log($"{sc.eventName} 하러 가기");
                            };
                            EditPlayEventButton(true, $"{sc.eventName}", mF);
                            return;
                        case CalendarUIDataType.REST:
                            mF += () =>
                            {
#if UNITYEDITOR
                                Debug.Log("휴식");
                                UnityEditor.EditorApplication.isPlaying = false;
#endif
                            };
                            EditPlayEventButton(true, "휴식!", mF);
                            return;
                        default:
                            Debug.Log("Unknown Schedule type");
                            return;
                    }
                }
            }

            //오늘 스케쥴은 다 끝남
            if (time == maxTime)
            {
                EditPlayEventButton(true, "시간 때우기", () =>
                {
                    Debug.Log("시간 + 1");
                    time += 1;
                    RefreshPlayEventButton();
                });
                return;
            }
            
            //오늘 스케쥴 더 있음
            //다음 스케쥴까지 시간 때우기
            
            //다음 스케쥴로 바로 시간 보내기
            /*EditPlayEventButton(true, "다음 스케쥴로!", () =>
            {
                Debug.Log("다음 스케쥴까지 시간 때우기");
                time = nextTime;
                RefreshPlayEventButton();
            });*/
            
            EditPlayEventButton(true, "시간 때우기", () =>
            {
                Debug.Log("시간 + 1");
                time += 1;
                RefreshPlayEventButton();
            });
        }

        private void EditPlayEventButton(bool active, string text, MyFunc myFunc)
        {
            PlayEventButton.GetComponent<Button>().interactable = active;
            PlayEventButton.GetComponent<Button>().onClick.RemoveAllListeners();
            PlayEventButton.GetComponent<Button>().onClick.AddListener(() => myFunc());
            PlayEventButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
        }

#endregion
        
    }

    //시간을 0 - 1 - 2 - 3으로 분할 (0-1이 아침, 1-2가 점심, 2-3이 저녁, 0-2면 아침 점심 다 하는거)
    //행동력이 4면 4까지 확장
    /// <summary>
    /// TODO 추가적인 스케쥴 종류가 생기면 "생성자" 추가 가능
    /// </summary>
    [Serializable]
    public class Schedule
    {
        public int scheduleDate;
        public int startTime;
        public int endTime;
        public CalendarUIDataType scheduleType;

        public bool noshow;

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
                    return fighter1.FighterName + " vs " + fighter2.FighterName;
                case CalendarUIDataType.Excercise:
                    return "훈련!";
                case CalendarUIDataType.Event:
                    return eventName;
                case CalendarUIDataType.REST:
                    return "휴식!";
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
            if (!schedules.Contains(schedule))
            {
                Debug.Log($"{schedule.scheduleDate} 일차 / {schedule.startTime} ~ {schedule.endTime} / {schedule.scheduleType} 추가");
                schedules.Add(schedule);
            }
        }

        public void RemoveSchedule(Schedule schedule)
        {
            if (schedules.Contains(schedule))
            {
                Debug.Log($"{schedule.scheduleDate} 일차 / {schedule.startTime} ~ {schedule.endTime} / {schedule.scheduleType} 삭제");
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
