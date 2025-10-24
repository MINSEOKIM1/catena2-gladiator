using System;
using System.Collections.Generic;
using Bucket.Inventory;
using UnityEngine;

namespace Bucket.Manager
{
    //TODO
    //일단은 DataManager가 데이터를 가지고 있겠지만 추후에는 Application.persistantDataPath 에 인코딩해서 적는게 좋아보임
    public class DataManager : MonoBehaviour
    {
        public SerializableDictionary<int, DailySchedules> scheduleList =  new SerializableDictionary<int, DailySchedules>();
        public List<Fighter> fighters = new List<Fighter>();
        public List<ManagementPhaseManager.EventData> events;
        public InventorySlot[] equippedinventorySlots;
        public InventorySlot[] unEquippedinventorySlots;
        public int date;
        public int time;
        public int activePoint;
        public int popularity = 30;
        public int money = 50;
        
        public bool firstSetFighters = false;
        public bool loadData = false;
        public bool firstDatePass = false;

        public int hp = 100;
        
        public static DataManager Instance
        {
            get { return _instance; }
        }

        private static DataManager _instance = null;
        
        private void Awake()
        {
            //모든 씬에 존재하는 데이터 매니저
            if(_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                
                return;
            }
            
            DestroyImmediate(gameObject);
        }

        public bool TimePassNoShow()
        {
            bool noshow = false;
            
            if (scheduleList.ContainsKey(date))
            {
                var schedules = scheduleList[date].schedules;

                foreach (var s in schedules)
                {
                    if (s.startTime == time)
                    {
                        noshow = true;
                        s.noshow = true;
                        popularity -= 15;
                    }
                }
            }

            time += 1;

            return noshow;
        }
        
        public void SaveDatas()
        {
            date = ScheduleManager.Instance.Date;
            scheduleList = ScheduleManager.Instance.ScheduleList;
            fighters = ManagementPhaseManager.Instance.fighters;
            equippedinventorySlots = ManagementPhaseManager.Instance.equippedinventorySlots;
            unEquippedinventorySlots = ManagementPhaseManager.Instance.unEquippedinventorySlots;
            date = ScheduleManager.Instance.Date;
            activePoint = ManagementPhaseManager.Instance.dailyActivePoint;
            time = ScheduleManager.Instance.Time;
            
            events = ManagementPhaseManager.Instance.dailyEvents;
        }
    }
}
