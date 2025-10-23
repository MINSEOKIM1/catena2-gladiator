using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bucket.Inventory;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Bucket.Manager
{
    [Serializable]
    public enum MANAGEMENT_PHASE_STATE
    {
        Home = -1,
        Regroup,
        Statistic,
        Excersice,
        Shop
    }
    
    public class ManagementPhaseManager : MonoBehaviour, IListener
    {
        public static ManagementPhaseManager Instance { get { return _instance; } }
        private static ManagementPhaseManager _instance = null;
    
        [SerializeField] [Header("**Fighters**")]
        public List<Fighter> fighters = new List<Fighter>();

        private int fighterIndex = 0;

        [Space(10)] [Header("Prefab")] 
        [SerializeField] private GameObject inventoryItemPrefab;
        [SerializeField] private GameObject RankingPrefab;
        
        [Space(10)] [Header("Scriptables")]
        [SerializeField] private EquipmentScriptable helmetScriptable;
        [SerializeField] private EquipmentScriptable chestplateScriptable;
        [SerializeField] private EquipmentScriptable leggingsScriptable;
        [SerializeField] private EquipmentScriptable bootsScriptable;
        [SerializeField] private EquipmentScriptable weaponScriptable;

        [FormerlySerializedAs("EquippedinventorySlots")]
        [Space(10)] [Header("UI")]
    
        [Space(10)] [Header("Inventory UI")]
        [SerializeField] public InventorySlot[] equippedinventorySlots;
        [SerializeField] public InventorySlot[] unEquippedinventorySlots;
        /*[SerializeField] private TextMeshProUGUI helmetText;
        [SerializeField] private TextMeshProUGUI chestText;
        [SerializeField] private TextMeshProUGUI leggingsText;
        [SerializeField] private TextMeshProUGUI bootsText;
        [SerializeField] private TextMeshProUGUI weaponText;*/

        [Space(10)] [Header("Screen UI")] 
        [SerializeField] private GameObject[] screenUIs;
        
        [Space(10)] [Header("Game Render")]
        [SerializeField] private SpriteRenderer[] HomeGameRenderers;

        [Space(10)] [Header("ETC UI")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private TextMeshProUGUI activePointText;
        [SerializeField] private TextMeshProUGUI tirednessText;
        [SerializeField] private TextMeshProUGUI popularityText;
        [SerializeField] private TextMeshProUGUI statisticStatInfo;
        [SerializeField] private GameObject popUpItemInfoContentBox;
        [SerializeField] private GameObject scrollViewContentBox;
        [SerializeField] private GameObject NewsBack;
        [SerializeField] private GameObject ChallengeMail;

        private Coroutine showItemInfoOnHoverCoroutine = null;
        
        [Space(10)] [Header("ETC")]
        public List<EventData> dailyEvents = new List<EventData>();
        public int dailyActivePoint = 3; //행동력
        private int challengeMailDate;
        private int challengeMailTime;
        private Fighter ChallengeMailFighter;

#region Basic

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
            
            LoadDailyEvent();
            
            InitializeFighters(8);
            
            RefreshRankingUI();
        }

        void Start()
        {
            EventManager.Instance.AddListener(EVENT_TYPE.eDatePass, this);
            EventManager.Instance.AddListener(EVENT_TYPE.eChallengeMail, this);
            
            DataManager.Instance.SaveDatas();
        }

        void Update()
        {
            /*//메인케릭터 장비 랜덤 교체는 스페이스
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Equipments newEquipments = MakeRandomEquips();
                Debug.Log($"changed helmet from {fighters[0].Equipments.EquipNewEquipment(newEquipments._helmet, EQUIPMENT_TYPE.HELMET).itemName} to {fighters[0].Equipments._helmet.itemName}");
                Debug.Log($"changed chestplate from {fighters[0].Equipments.EquipNewEquipment(newEquipments._chestplate, EQUIPMENT_TYPE.CHESTPLATE).itemName} to {fighters[0].Equipments._chestplate.itemName}");
                Debug.Log($"changed leggings from {fighters[0].Equipments.EquipNewEquipment(newEquipments._leggings, EQUIPMENT_TYPE.LEGGINGS).itemName} to {fighters[0].Equipments._leggings.itemName}");
                Debug.Log($"changed boots from {fighters[0].Equipments.EquipNewEquipment(newEquipments._boots, EQUIPMENT_TYPE.BOOTS).itemName} to {fighters[0].Equipments._boots.itemName}");
                Debug.Log($"changed weapon from {fighters[0].Equipments.EquipNewEquipment(newEquipments._weapon, EQUIPMENT_TYPE.WEAPON).itemName} to {fighters[0].Equipments._weapon.itemName}");
                RefreshStatUI();
            }*/
        
            //현재 보는 케릭터 변경은 탭
            /*if (Input.GetKeyDown(KeyCode.Tab))
            {
                fighterIndex = (fighterIndex + 1) % fighters.Count;
                InitializeStatUI();
            }*/

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EditorApplication.isPaused = true;
            }
        }

        public void OnEvent(EVENT_TYPE eventType, Component sender, object param1 = null, object param2 = null)
        {
            switch (eventType)
            {
                case EVENT_TYPE.eDatePass:
                    RefreshFighterRanking();
                    RefreshRankingUI();
                    ExpectDailyEvent();
                    break;
                case EVENT_TYPE.eChallengeMail:
                    DisplayChallengeMail((int) param1, (Fighter) param2);
                    break;
            }
        }

#endregion
        
#region UI
        
        private void DisplayDailyNews(string newsDateText, string newsText)
        {
            NewsBack.SetActive(true);
            NewsBack.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = newsDateText;
            NewsBack.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = newsText;
        }

        /// <summary>
        /// 싸움 신청 왔을 때 메일 띄우기
        /// </summary>
        /// <param name="date">몇 일 후인지</param>
        /// <param name="enemy">싸움 신청을 한 적</param>
        private void DisplayChallengeMail(int num, Fighter enemy)
        {
            ChallengeMail.SetActive(true);
            
            challengeMailDate = num / 10;
            challengeMailTime = num % 10;
            string s = "";
            
            //TODO
            //행동력 증가하면 수정해야함
            if (challengeMailTime == 0)
            {
                s = "오전";
            }
            else if (challengeMailTime == 1)
            {
                s = "오후";
            }
            
            ChallengeMailFighter = enemy;
            
            ChallengeMail.SetActive(true);
            ChallengeMail.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = 
                $"{enemy.FighterName} 으로부터 결투 신청이 왔습니다\n" +
                $"결투 예정일 : {challengeMailDate}일 후 {s}\n" +
                "필요 행동력 : 2\n" +
                "수락하시겠습니까?\n" +
                "<color=red>수락하시면 겹치는 스케쥴은 사라집니다.</color>";
        }

        public void AcceptChallengeMail()
        {
            ScheduleManager.Instance.AddScheduleToList(new Schedule(ScheduleManager.Instance.Date + challengeMailDate, challengeMailTime, challengeMailTime + 2, CalendarUIDataType.Fight, fighters[0], ChallengeMailFighter));
            challengeMailDate = 0;
            challengeMailTime = 0;
            ChallengeMailFighter = null;
            
            ChallengeMail.SetActive(false);
        }

        public void RejectChallengeMail()
        {
            challengeMailDate = Mathf.Max(1, challengeMailDate - 1);
            
            ChallengeMail.SetActive(false);
        }

        public void OpenPhaseScreen(string state)
        {
            switch (state)
            {
                case "Home":
                    Debug.Log("Open Home");
                    foreach (GameObject ui in screenUIs)
                    {
                        ui.SetActive(false);
                    }
                    break;
                case "Regroup":
                    Debug.Log("Open Regroup");
                    screenUIs[(int)MANAGEMENT_PHASE_STATE.Regroup].SetActive(true);
                    break;
                case "Statistic":
                    Debug.Log("Open Statistic");
                    screenUIs[(int)MANAGEMENT_PHASE_STATE.Statistic].SetActive(true);
                    InitializeStatUI();
                    break;
                case "Excersice":
                    Debug.Log("Open Excersice");
                    screenUIs[(int)MANAGEMENT_PHASE_STATE.Excersice].SetActive(true);
                    break;
                case "Shop":
                    Debug.Log("Open Shop");
                    screenUIs[(int)MANAGEMENT_PHASE_STATE.Shop].SetActive(true);
                    break;
            }
        }
        
        public void InitializeStatUI()
        {
            Debug.Log("Initialize stat UI");
            Equipments equipments = fighters[fighterIndex].Equipments;
        
            //title.text = $"*{fighters[fighterIndex].FighterName}* (Tab to Change)";
            RefreshStatInfo();
        
            /*helmetText.text =  equipments._helmet != null ? $"{equipments._helmet.itemName}\narmor : {equipments._helmet.armor}" : "Nothing";
            chestText.text = equipments._chestplate != null ? $"{equipments._chestplate.itemName}\narmor : {equipments._chestplate.armor}" : "Nothing";
            leggingsText.text = equipments._leggings != null ? $"{equipments._leggings.itemName}\narmor : {equipments._leggings.armor}" : "Nothing";
            bootsText.text = equipments._boots != null ? $"{equipments._boots.itemName}\narmor : {equipments._boots.armor}" : "Nothing";
            weaponText.text = equipments._weapon != null ? $"{equipments._weapon.itemName}\ndamage : {equipments._weapon.damage}" : "Nothing";*/
        
            /*
            helmetUIImage.sprite = equipments._helmet.uiIcon;
            chestUIImage.sprite = equipments._chestplate.uiIcon;
            leggingsUIImage.sprite = equipments._leggings.uiIcon;
            bootsUIImage.sprite = equipments._boots.uiIcon;
            weaponUIImage.sprite = equipments._weapon.uiIcon;*/
            
            for (int i = 0; i < 5; i++)
            {
                EraseEquippedInventory((EQUIPMENT_TYPE)i);
                //Debug.Log(equippedinventorySlots[i].GetComponentInChildren<InventoryItem>()?.item.itemName);
                SpawnInventoryItem(equippedinventorySlots[i], equipments.GetEquipmentByIndex(i));
                //Debug.Log(equippedinventorySlots[i].GetComponentInChildren<InventoryItem>()?.item.itemName);

                if (fighterIndex != 0)
                {
                    if (equippedinventorySlots[i].transform.childCount != 0)
                    {
                        equippedinventorySlots[i].transform.GetChild(equippedinventorySlots[i].transform.childCount - 1).GetComponent<InventoryItem>().isClickable = false;
                    }
                }
                else
                {
                    if (equippedinventorySlots[i].transform.childCount != 0)
                    {
                        equippedinventorySlots[i].transform.GetChild(equippedinventorySlots[i].transform.childCount - 1).GetComponent<InventoryItem>().isClickable = true;
                    }
                }
                
                HomeGameRenderers[i].sprite = equipments.GetEquipmentByIndex(i)?.gameIcon;
            }
        
            if (fighterIndex != 0)
            {
                foreach (InventorySlot slot in unEquippedinventorySlots)
                {
                    slot.gameObject.SetActive(false);
                }
            }
            else
            {
                foreach (InventorySlot slot in unEquippedinventorySlots)
                {
                    slot.gameObject.SetActive(true);
                }
            }
        }

        public void RefreshCharacterImageOnBackground(EQUIPMENT_TYPE type)
        {
            HomeGameRenderers[(int)type].sprite = fighters[fighterIndex].Equipments.GetEquipmentByIndex((int)type)?.gameIcon;
        }

        public void RefreshStatInfo()
        {
            statisticStatInfo.text = $"패러미터\n전투력 : {fighters[0].GetTotalPower()}\n공격력 : {fighters[0].Equipments.GetAttackPower()}\n방어력 : {fighters[0].Equipments.GetDefensePower()}\n피로도 : {fighters[0].Tiredness}";
        }

        /*public void RefreshStatUI(EQUIPMENT_TYPE type)
        {
            switch (type)
            {
                case EQUIPMENT_TYPE.HELMET:
                    EraseEquippedInventory(EQUIPMENT_TYPE.HELMET);
                    SpawnInventoryItem(equippedinventorySlots[0], fighters[fighterIndex].Equipments._helmet);
                    break;
                case EQUIPMENT_TYPE.CHESTPLATE:
                    EraseEquippedInventory(EQUIPMENT_TYPE.CHESTPLATE);
                    SpawnInventoryItem(equippedinventorySlots[1], fighters[fighterIndex].Equipments._chestplate);
                    break;
                case EQUIPMENT_TYPE.LEGGINGS:
                    EraseEquippedInventory(EQUIPMENT_TYPE.LEGGINGS);
                    SpawnInventoryItem(equippedinventorySlots[2], fighters[fighterIndex].Equipments._leggings);
                    break;
                case EQUIPMENT_TYPE.BOOTS:
                    EraseEquippedInventory(EQUIPMENT_TYPE.BOOTS);
                    SpawnInventoryItem(equippedinventorySlots[3], fighters[fighterIndex].Equipments._boots);
                    break;
                case EQUIPMENT_TYPE.WEAPON:
                    EraseEquippedInventory(EQUIPMENT_TYPE.WEAPON);
                    SpawnInventoryItem(equippedinventorySlots[4], fighters[fighterIndex].Equipments._weapon);
                    break;
                default:
                    Debug.LogError("No Equipment Type Found");
                    break;
            }
        }*/

        public void RefreshRankingUI()
        {
            foreach (Transform child in scrollViewContentBox.transform)
            {
                Destroy(child.gameObject);
            }

            List<Fighter> fightersSortByPower = fighters.ToList();
            fightersSortByPower.Sort(new FighterSortComparer());
            fightersSortByPower.Reverse();

            foreach (Fighter fighter in fightersSortByPower)
            {
                GameObject gb = Instantiate(RankingPrefab, scrollViewContentBox.transform);

                gb.transform.GetChild(1).GetComponent<Image>().sprite = fighter.fighterSprite;
                gb.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = fighter.FighterName == "Player" ? $"<color=\"red\">Player</color>" : fighter.FighterName;
                gb.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = $"전투력 : {fighter.GetTotalPower()}";
                gb.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = fighter.CurrentRank == 1 ? $"<color=\"red\">{fighter.CurrentRank} 등</color>" : $"{fighter.CurrentRank} 등";
            }
        }
        
        private void EraseEquippedInventory(EQUIPMENT_TYPE type)
        {
            InventorySlot slot = equippedinventorySlots[(int)type];
            if (slot.transform.childCount != 0)
            {
                Destroy(slot.transform.GetChild(0).gameObject);
            }
        }

        private void SpawnInventoryItem(InventorySlot slot, Equipment item)
        {
            if (item == null) return;
            GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
            InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
            inventoryItem.ShowUIItem(item);
        }

        public void ShowItemInfoOnHover(InventoryItem item)
        {
            popUpItemInfoContentBox.SetActive(true);
            
            popUpItemInfoContentBox.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = item.item.ToString();
            
            /*popUpItemInfoContentBox.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;
            Debug.Log("Mouse : " + Input.mousePosition);
            Debug.Log("앵커 포지션 : " + popUpItemInfoContentBox.GetComponent<RectTransform>().anchoredPosition);
            Debug.Log("오프셋 : " + popUpItemInfoContentBox.GetComponent<RectTransform>().offsetMax);
            
            //20f 는 오프셋임 (라임 지렷고)
            Vector2 offset
                = new Vector2(
                    Mathf.Max(popUpItemInfoContentBox.GetComponent<RectTransform>().offsetMax.x + 20f, 0),
                    Mathf.Max(popUpItemInfoContentBox.GetComponent<RectTransform>().offsetMax.y + 20f, 0));
            popUpItemInfoContentBox.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition - (Vector3)offset;*/
            
            if (showItemInfoOnHoverCoroutine != null) StopCoroutine(showItemInfoOnHoverCoroutine);
            showItemInfoOnHoverCoroutine = StartCoroutine(ShowItemInfoOnHoverCo());
        }

        private IEnumerator ShowItemInfoOnHoverCo()
        {
            yield return new WaitForEndOfFrame();
            Vector2 targetPos = Input.mousePosition;
            /*Rect rect = canvas.GetComponent<RectTransform>().rect;
            Vector2 boxSize = 2 * new Vector2(rect.xMax, rect.yMax) + popUpItemInfoContentBox.GetComponent<RectTransform>().sizeDelta;
            Debug.Log(boxSize);*/
            
            Vector2 offset = targetPos + popUpItemInfoContentBox.GetComponent<RectTransform>().sizeDelta;
            /*Debug.Log(offset);*/
            //위 오른쪽으로 20f는 띄어놓기
            offset = new Vector2(MathF.Max(offset.x + 20f, 0), MathF.Max(offset.y + 20f, 0));

            targetPos -= offset;
            popUpItemInfoContentBox.GetComponent<RectTransform>().anchoredPosition = targetPos;
        }

        public void HideItemInfoOnHover()
        {
            if (showItemInfoOnHoverCoroutine != null) StopCoroutine(showItemInfoOnHoverCoroutine);
            popUpItemInfoContentBox.SetActive(false);
        }

#endregion

#region Functionality

        private void LoadDailyEvent()
        {
            TextAsset eventData = Resources.Load<TextAsset>("EventData");
        
            string[] data = eventData.text.Split(new char[] { '\n' });
        
            //제목 뺴고
            for (int i = 1; i < data.Length - 1; i++)
            {
                string[] row = data[i].Split(new char[] { ',' });

                if (row[1] != "")
                {
                    EventData e = new EventData();

                    e.eventName = row[0];
                    int.TryParse(row[1], out e.startDate);
                    int.TryParse(row[2], out e.endDate);
                    float.TryParse(row[3], out e.eventPossibility);
                    int.TryParse(row[4], out e.actionPowerAmount);
                
                    dailyEvents.Add(e);
                }
            }

            /*foreach (EventData e in events)
            {
                Debug.Log(e.eventName + "/" + e.startDate + "/" + e.endDate + "/" + e.eventPossibility + "/" + e.actionPowerAmount);
            }*/
        }
        
        [Serializable]
        public class EventData
        {
            public string eventName;
            public int startDate;
            public int endDate;
            public float eventPossibility;
            public int actionPowerAmount;

            public override string ToString()
            {
                return eventName + " / " +  startDate + " / " + endDate + " / " + eventPossibility + " / " + actionPowerAmount;
            }
        }
        
        private void ExpectDailyEvent()
        {
            foreach (EventData e in dailyEvents)
            {
                if (e.startDate <= ScheduleManager.Instance.Date && ScheduleManager.Instance.Date <= e.endDate && Random.Range(0f, 1f) < e.eventPossibility)
                {
                    switch (e.eventName.TrimEnd())
                    {
                        case "Challenge_Mail":
                            if (challengeMailDate != 0)
                            {
                                EventManager.Instance.PostNotification(EVENT_TYPE.eChallengeMail, this,challengeMailDate * 10 + challengeMailTime, ChallengeMailFighter);
                                break;
                            }
                            EventManager.Instance.PostNotification(EVENT_TYPE.eChallengeMail, this,Random.Range(1, 5) * 10 + Random.Range(0, 2) , GetRandomFighter());
                            break;
                        case "Sign_Show":
                            break;
                    }
                }
            }
        }

        private void SendChallengeMail()
        {
            //TODO
            //도전장 UI 띄우기
            //UI에서 ok 누르면 싸우는 스케쥴 생성하도록
            EventManager.Instance.PostNotification(EVENT_TYPE.eChallengeMail, this, GetRandomFighter(), true);
        }

        private void InitializeFighters(int numOfFighters = 1)
        {
            numOfFighters = Mathf.Max(1, numOfFighters);
            //0번째가 플레이어
            for (int i = 0; i < numOfFighters; i++)
            {

                if (i == 0)
                {
                    Player player = new Player("Player", Random.Range(0, 50), MakeRandomEquips());
                    fighters.Add(player);
                    continue;
                }
                fighters.Add(new Fighter(MakeRandomName(), Random.Range(0, 50), MakeRandomEquips()));
            }

            List<Fighter> fightersSortByPower = fighters.ToList();
            fightersSortByPower.Sort(new FighterSortComparer());
            for (int i = 0; i < fightersSortByPower.Count; i++)
            {
                fightersSortByPower[i].CurrentRank = fightersSortByPower.Count - i;
            }
        }
        
        private Equipments MakeRandomEquips()
        {
            /*Helmet helmet = new Helmet($"{modifiers1[Random.Range(0, modifiers1.Length)]} {modifiers2[Random.Range(0, modifiers2.Length)]}의 헬멧", Random.Range(5, 10), Random.Range(0, 10));
        Chestplate chestplate = new Chestplate($"{modifiers1[Random.Range(0, modifiers1.Length)]} {modifiers2[Random.Range(0, modifiers2.Length)]}의 상의", Random.Range(5, 10), Random.Range(0, 10));
        Leggings leggings = new Leggings($"{modifiers1[Random.Range(0, modifiers1.Length)]} {modifiers2[Random.Range(0, modifiers2.Length)]}의 하의", Random.Range(5, 10), Random.Range(0, 10));
        Boots boots = new Boots($"{modifiers1[Random.Range(0, modifiers1.Length)]} {modifiers2[Random.Range(0, modifiers2.Length)]}의 신발", Random.Range(5, 10), Random.Range(0, 10));
        Weapon weapon = new Weapon($"{modifiers1[Random.Range(0, modifiers1.Length)]} {modifiers2[Random.Range(0, modifiers2.Length)]}의 {weaponType[Random.Range(0, weaponType.Length)]}", Random.Range(5, 10), Random.Range(0, 10));
        */

            int idx = 0;
            
            idx = Random.Range(0, helmetScriptable.uiIcons.Length);
            Helmet helmet = new Helmet($"{modifiers1[Random.Range(0, modifiers1.Length)]} {modifiers2[Random.Range(0, modifiers2.Length)]}'s helmet", helmetScriptable.uiIcons[idx], helmetScriptable.gameIcons[idx]);
            helmet.AddAttribute(EQUIPMENT_ATTRIBUTE.DEFENSE, Random.Range(1, 10));
            helmet.SetInfo("기본 투구입니다.");
            
            idx = Random.Range(0, chestplateScriptable.uiIcons.Length);
            Chestplate chestplate = new Chestplate($"{modifiers1[Random.Range(0, modifiers1.Length)]} {modifiers2[Random.Range(0, modifiers2.Length)]}'s chestplate", chestplateScriptable.uiIcons[idx], chestplateScriptable.gameIcons[idx]);
            chestplate.AddAttribute(EQUIPMENT_ATTRIBUTE.DEFENSE, Random.Range(1, 10));
            chestplate.SetInfo("기본 상의입니다.");
            
            idx = Random.Range(0, leggingsScriptable.uiIcons.Length);
            Leggings leggings = new Leggings($"{modifiers1[Random.Range(0, modifiers1.Length)]} {modifiers2[Random.Range(0, modifiers2.Length)]}'s leggings", leggingsScriptable.uiIcons[idx], leggingsScriptable.gameIcons[idx]);
            leggings.AddAttribute(EQUIPMENT_ATTRIBUTE.DEFENSE, Random.Range(1, 10));
            leggings.SetInfo("기본 하의입니다.");
            
            idx = Random.Range(0, bootsScriptable.uiIcons.Length);
            Boots boots = new Boots($"{modifiers1[Random.Range(0, modifiers1.Length)]} {modifiers2[Random.Range(0, modifiers2.Length)]}'s boots", bootsScriptable.uiIcons[idx], bootsScriptable.gameIcons[idx]);
            boots.AddAttribute(EQUIPMENT_ATTRIBUTE.DEFENSE, Random.Range(1, 10));
            boots.SetInfo("기본 신발입니다.");
            
            idx = Random.Range(0, weaponScriptable.uiIcons.Length);
            Weapon weapon = new Weapon($"{modifiers1[Random.Range(0, modifiers1.Length)]} {modifiers2[Random.Range(0, modifiers2.Length)]}'s {weaponType[Random.Range(0, weaponType.Length)]}", weaponScriptable.uiIcons[idx], weaponScriptable.gameIcons[idx]);
            weapon.AddAttribute(EQUIPMENT_ATTRIBUTE.ATTACK_POWER, Random.Range(1, 10));
            weapon.SetInfo("기본 무기입니다.");
            
            return new Equipments(helmet, chestplate, leggings, boots, weapon);
        }

        private string[] modifiers1 = new[] { "Weird", "Powerful", "Great", "Awesome", "Shining", "Precious", "Fallen", "Dark" };
        private string[] modifiers2 = new[] { "Dog", "cat", "Panda", "Goat", "Mouse", "Cow", "Snake", "Tiger" };
        private string[] weaponType = new[] { "Greatsword", "Dagger", "Long Sword", "Tree Branch", "Stick" };
    
        private string MakeRandomName()
        {
            return
                $"{nameModifiers1[Random.Range(0, nameModifiers1.Length)]}{nameModifiers2[Random.Range(0, nameModifiers2.Length)]}{nameModifiers3[Random.Range(0, nameModifiers3.Length)]}";
        }
    
        private string[] nameModifiers1 = new[] { "Yang", "Jeon", "Gim", "Son", "Jeong" };
        private string[] nameModifiers2 = new[] { "Dong", "Min", "Se", "Ho", "Seo" };
        private string[] nameModifiers3 = new[] { "Hwan", "Seo", "Hyun", "Jae", "Yun" };

        /// <summary>
        /// without player
        /// </summary>
        /// <returns></returns>
        public Fighter GetRandomFighter()
        {
            return fighters[Random.Range(1, fighters.Count)];
        }

        private void RefreshFighterRanking()
        {
            List<Fighter> fightersSortByPower = fighters.ToList();
            fightersSortByPower.Sort(new FighterSortComparer());
            for (int i = 0; i < fightersSortByPower.Count; i++)
            {
                fightersSortByPower[i].CurrentRank = fightersSortByPower.Count - i;
            }
        }

#endregion
    }
}
