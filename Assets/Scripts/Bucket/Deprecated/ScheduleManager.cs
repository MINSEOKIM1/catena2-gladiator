using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/*
namespace Bucket.Manager
{
    public class ScheduleManager : MonoBehaviour, IListener
    {
        public static ScheduleManager Instance { get { return _instance; } }
        private static ScheduleManager _instance = null;
        
        public int Date
        {
            get => date;
        }
        private int date = 0;
        
        [Space(10)] [Header("Schedule UI")]
        [SerializeField] private GameObject calenderUI;
        [SerializeField] private GameObject scheduleScrollViewContent;
        
        [Space(10)] [Header("ETC")]
        private int spareDate = 3;
        [SerializeField] private TextMeshProUGUI dateText;
        [SerializeField] private GameObject schedulePrefab;
        
        [SerializeField] private List<ScheduleBlock> scheduleList = new List<ScheduleBlock>();

        private bool needPlayerFightCaculate = false;
        
        void Awake()
        {
            //관리 페이즈에만 존재하는 싱글톤 ㅎㅎ
            if(_instance == null)
            {
                _instance = this;
                //DontDestroyOnLoad(gameObject);
                //return;
            }
            //DestroyImmediate(gameObject);
        }

        void Start()
        {
            EventManager.Instance.AddListener(EVENT_TYPE.eChallengeMail, this);
            
            InitializeSchdule();
            
            DatePass();
            
            RefreshScheduleUI();
        }
        
        public void OnEvent(EVENT_TYPE EventType, Component Sender, object Param1 = null, object Param2 = null)
        {
            switch (EventType)
            {
                case  EVENT_TYPE.eChallengeMail:
                    scheduleList.Add(GenerateScheduleBlock(date + spareDate, ManagementPhaseManager.Instance.fighters[0]));
                    break;
            }
        }
            
        private void DatePass()
        {
            date++;
            
            dateText.text = $"{date} 일차";

            if (needPlayerFightCaculate)
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
            ManagementPhaseManager.Instance.OpenDailyNews(date.ToString() + " 일차 소식", newsText);
        }

        private void RefreshCalendarUI()
        {
            
        }

        //기본적으로 스케쥴을 다 만들어 놓음
        //도전장 날라오면 그걸로 스케쥴 만듬
        //도전장 거절 시 삭선
        private void InitializeSchdule()
        {
            /*for (int i = 0; i < date + spareDate; i++)
            {
                bool flag = false;
                //도전장 등으로 이미 해당 날짜 블럭 있으면 삭제
                foreach (ScheduleBlock block in scheduleList)
                {
                    if (block.date == i + 1)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag) continue;
                scheduleList.Insert(i, GenerateScheduleBlock(i + 1));
            }#1#

            for (int i = 0; i < spareDate; i++)
            {
                scheduleList.Add(GenerateScheduleBlock(i + 1));   
            }
        }
        
        private void RefreshScheduleUI()
        {
            foreach (Transform child in scheduleScrollViewContent.transform)
            {
                Destroy(child.gameObject);
            }

            //도전장 고려
            if (scheduleList.Count < date + spareDate) scheduleList.Add(GenerateScheduleBlock(date + spareDate));

            foreach (ScheduleBlock block in scheduleList)
            {
                MakeScheduleUIObject(block);
            }
        }

        private void MakeScheduleUIObject(ScheduleBlock block)
        {
            GameObject g = Instantiate(schedulePrefab, scheduleScrollViewContent.transform);

            g.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = block.date == date ? $"<color=\"yellow\">{block.date} 일차</color>" : $"{block.date} 일차";
            
            Transform table1 = g.transform.GetChild(1);
            Transform table2 = g.transform.GetChild(2);

            table1.GetChild(0).GetComponent<TextMeshProUGUI>().text = block.table1.fighter1.FighterName;
            table1.GetChild(1).GetComponent<TextMeshProUGUI>().text = block.table1.fighter2.FighterName;
                
            table2.GetChild(0).GetComponent<TextMeshProUGUI>().text = block.table2.fighter1.FighterName;
            table2.GetChild(1).GetComponent<TextMeshProUGUI>().text = block.table2.fighter2.FighterName;

            
            if (block.date < date) g.GetComponent<Image>().color = Color.grey;
            
            if (block.date <= date)
            {
                FighterTable table = block.GetTableByIndex(1); 
                if (table.winloss != null)
                {
                    if ((bool)table.winloss)
                    {
                        table1.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
                        table1.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 0.5f);
                    }
                    else
                    {
                        table1.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 0.5f);
                        table1.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.red;
                    }
                }
                
                table = block.GetTableByIndex(2); 
                if (table.winloss != null)
                {
                    if ((bool)table.winloss)
                    {
                        table2.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
                        table2.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 0.5f);
                    }
                    else
                    {
                        table2.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 0.5f);
                        table2.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.red;
                    }
                }
            }
        }
        
        private void RefreshScheduleBlockWinLoss(ScheduleBlock block, int index, bool winloss)
        {
            FighterTable table = block.GetTableByIndex(index);
            table.winloss = table.winloss ?? winloss;
        }

        //player 는 table1의 왼쪽 고정임
        public void SchdulePlayerWin()
        {
            scheduleList[date - 1].table1.winloss = true;
        }

        private FighterTable GenerateFighterTable(Fighter fighter1 = null, Fighter fighter2 = null)
        {
            fighter1 = fighter1 ?? ManagementPhaseManager.Instance.GetRandomFighter();

            /*int count = 0;
            while (fighter2 == null || fighter1 == fighter2)
            {
                fighter2 = fighter2 ?? ManagementPhaseManager.Instance.GetRandomFighter();
                count++;
                if (count == 1000) Debug.LogError("While Loop");
            }#1#
            
            fighter2 = fighter2 ?? ManagementPhaseManager.Instance.GetRandomFighter();
            return new FighterTable(fighter1, fighter2);
        }

        private ScheduleBlock GenerateScheduleBlock(int fightDate, Fighter fighter1 = null, Fighter fighter2 = null, Fighter fighter3 = null, Fighter fighter4 = null)
        {
            FighterTable table1 = GenerateFighterTable(fighter1, fighter2);
            FighterTable table2 = GenerateFighterTable(fighter3, fighter4);
            
            /*int count = 0;
            while (table1.fighter1 == table2.fighter1 || table1.fighter1 == table2.fighter2 || table1.fighter2 == table2.fighter1 || table1.fighter2 == table2.fighter2)
            {
                table2 = GenerateFighterTable();
                count++;
                if (count == 1000) Debug.LogError("While Loop");
            }#1#
            return new ScheduleBlock(fightDate, GenerateFighterTable(fighter1, fighter2), GenerateFighterTable(fighter3, fighter4));
        }

        private void CaculateWinner(ScheduleBlock block, int index)
        {
            FighterTable table = block.GetTableByIndex(index);
            Fighter fighter1 = table.GetFighterByIndex(1);
            Fighter fighter2 = table.GetFighterByIndex(2);

            if (!needPlayerFightCaculate && fighter1 == ManagementPhaseManager.Instance.fighters[0] ||
                fighter2 == ManagementPhaseManager.Instance.fighters[0])
            {
                needPlayerFightCaculate = true;
                return;
            }
                
            if (Fighter.CaculateWinRate(fighter1, fighter2) > Random.Range(0, 1f))
            {
                //fighter1 win
                RefreshScheduleBlockWinLoss(block, index, true);
                fighter1.WinMatch(fighter2);
                fighter2.LoseMatch(fighter1);
            }
            else
            {
                RefreshScheduleBlockWinLoss(block, index, false);
                fighter1.LoseMatch(fighter2);
                fighter2.WinMatch(fighter1);
            }
        }
    }

    [Serializable]
    public class FighterTable
    {
        public Fighter fighter1;
        public Fighter fighter2;
        public bool? winloss = null;

        public FighterTable(Fighter fighter1, Fighter fighter2)
        {
            this.fighter1 = fighter1;
            this.fighter2 = fighter2;
        }

        public Fighter GetFighterByIndex(int index)
        {
            if (index == 1) return fighter1;
            return fighter2;
        }

        //return winner if true
        //loser if false
        public Fighter GetWinner(bool isWinner)
        {
            if (winloss != null)
            {
                return isWinner ? ((bool)winloss ? fighter1 : fighter2) : ((bool)winloss ? fighter2 : fighter1);
            }

            return null;
        }
    }

    [Serializable]
    public class ScheduleBlock
    {
        public int date;
        public FighterTable table1;
        public FighterTable table2;

        public ScheduleBlock(int date, FighterTable table1, FighterTable table2)
        {
            this.date = date;
            this.table1 = table1;
            this.table2 = table2;
        }

        public FighterTable GetTableByIndex(int index)
        {
            if (index == 1) return table1;
            return table2;
        }
    }
}*/
