using System;
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

    enum CalendarUIDataType
    {
        Fight,
        Excercise,
        FanEvent,
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
            switch (currentCalendarUIType)
            {
                case CalendarUIType.Day:
                    calendarContentBox.GetComponent<VerticalLayoutGroup>().childControlHeight = false;
                    foreach (Transform child in currentCalendarUITypeObject.transform)
                    {
                        Destroy(child.gameObject);
                    }
                    
                    MakeDayDataObject(0, 8, Color.red);
                    MakeDayDataObject(8, 16, Color.blue);
                    MakeDayDataObject(16, 24, Color.green);
                    
                    break;
                case CalendarUIType.Week:
                    calendarContentBox.GetComponent<VerticalLayoutGroup>().childControlHeight = true;
                    /*foreach (Transform child in currentCalendarUITypeObject.transform)
                    {
                        Destroy(child.gameObject);
                    }*/
                    
                    //make childs
                    MakeWeekDataObjects();
                    
                    break;
                case CalendarUIType.Month:
                    calendarContentBox.GetComponent<VerticalLayoutGroup>().childControlHeight = true;
                    
                    break;
            }
        }

        public void SwitchCalendarUI(CalendarUIType calendarUIType)
        {
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

        private void MakeWeekDataObjects()
        {
            GameObject weekDataObj = currentCalendarUITypeObject.transform.GetChild(1).gameObject;

            foreach (Transform child in weekDataObj.transform)
            {
                Destroy(child.gameObject);
            }

            int activePoint = ManagementPhaseManager.Instance.dailyActivePoint;
            for (int i = 0; i < activePoint * 7; i++)
            {
                Instantiate(weekDataPrefab, weekDataObj.transform);
            }
            
            //각 weekDataObj에다가 이미지 입힐 것!
        }
    }
}
