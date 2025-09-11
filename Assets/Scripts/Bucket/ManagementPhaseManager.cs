using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bucket.Inventory;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Bucket
{
    public class ManagementPhaseManager : MonoBehaviour, IListener
    {
        public static ManagementPhaseManager Instance { get { return _instance; } }
        private static ManagementPhaseManager _instance = null;

        [SerializeField] private int date = 0;
    
        [SerializeField] [Header("**Fighters**")]
        public List<Fighter> fighters = new List<Fighter>();

        private int fighterIndex = 0;

        [Space(10)] [Header("Prefab")] 
        [SerializeField] private GameObject inventoryItemPrefab;
        
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
    
        [Space(10)] [Header("Game Render")]
        [SerializeField] private SpriteRenderer[] GameRenderers;

        [Space(10)] [Header("ETC UI")] 
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Image letterImage;
        [SerializeField] private TextMeshProUGUI dateText;

        private Coroutine runningLetterCo = null;
    
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
            
            //initialize n fighters
            //1st one is player
            InitializeFighters(8);
            InitializeStatUI();
        }

        void Start()
        {
            EventManager.Instance.AddListener(EVENT_TYPE.eDatePass, this);
            EventManager.Instance.AddListener(EVENT_TYPE.eChallengeMail, this);
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
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                fighterIndex = (fighterIndex + 1) % fighters.Count;
                InitializeStatUI();
            }
        
            //날짜 지나가기
            //나중에 날짜 지나는게 다른 클래스에서 담당할까봐 이벤트로 만들어놓음~
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                EventManager.Instance.PostNotification(EVENT_TYPE.eDatePass, this, ManagementPhaseManager._instance.date + 1);
            }
        }

        public void OnEvent(EVENT_TYPE EventType, Component Sender, object Param1 = null, object Param2 = null)
        {
            switch (EventType)
            {
                case EVENT_TYPE.eDatePass:
                    date++;
                    dateText.text = $"Date : {date}";
                    Debug.Log($"날짜가 {date}로 바뀌었습니다.");
                    // 날짜 지나면서 다른 애들이 강해지거나, 피로도 누적 등 구현 함수 필요
                    RefreshFighters();
                    ExpectChallenge();
                    break;
                case EVENT_TYPE.eChallengeMail:
                    ReceiveChallengeLetter((Fighter)Param1);
                    break;
            }
        }

        /// <summary>
        /// expect whether challenge comes or not
        /// param for designated fighter
        /// </summary>
        /// <param name="fighter"></param>
        private void ExpectChallenge(Fighter fighter = null)
        {
            if (Random.Range(0f, 1f) > 0.3f) return; //30% to receive challenge mail
            if (fighter == null) fighter = fighters[Random.Range(1, fighters.Count)]; //random fighter except player
            EventManager.Instance.PostNotification(EVENT_TYPE.eChallengeMail, this, fighter);
        }

        private void ReceiveChallengeLetter(Fighter from)
        {
            Debug.Log($"랭킹 {from.CurrentRank}위인 {from.FighterName}로부터 도전장이 날라왔습니다.");
            if (runningLetterCo != null) StopCoroutine(runningLetterCo);
            runningLetterCo = StartCoroutine(letterCoroutine());
        }

        IEnumerator letterCoroutine()
        {
            letterImage.color = Random.ColorHSV();
            yield return new WaitForSeconds(1f);
            letterImage.color = Color.white;
        }

        private void RefreshFighters()
        {
            //figters 장비 스탯들 올려주기
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

        public void InitializeStatUI()
        {
            Debug.Log("Initialize stat UI");
            Equipments equipments = fighters[fighterIndex].Equipments;
        
            //title.text = $"*{fighters[fighterIndex].FighterName}* (Tab to Change)";
            title.text = $"*{fighters[fighterIndex].FighterName}*\npower : {equipments.GetPower() + fighters[fighterIndex].BasicDamage}\nrank : {fighters[fighterIndex].CurrentRank}";
        
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
                SpawnInventoryItem(equippedinventorySlots[i], equipments.GetEquipment(i));

                GameRenderers[i].sprite = equipments.GetEquipment(i)?.gameIcon;
            }
        
            if (fighterIndex != 0)
            {
                foreach (InventorySlot slot in equippedinventorySlots)
                {
                    if (slot.transform.childCount == 0) continue;
                    slot.transform.GetChild(0).GetComponent<InventoryItem>().isClickable = false;
                }
            }
            else
            {
                foreach (InventorySlot slot in equippedinventorySlots)
                {
                    if (slot.transform.childCount == 0) continue;
                    slot.transform.GetChild(0).GetComponent<InventoryItem>().isClickable = true;
                }
            }
        }

        public void RefreshCharacter(EQUIPMENT_TYPE type)
        {
            GameRenderers[(int)type].sprite = fighters[fighterIndex].Equipments.GetEquipment((int)type)?.gameIcon;
        }

        public void RefreshStatUI(EQUIPMENT_TYPE type)
        {
            Debug.Log($"Refresh {type} stat UI");
            
            title.text = $"*{fighters[fighterIndex].FighterName}*\npower : {fighters[fighterIndex].Equipments.GetPower() + fighters[fighterIndex].BasicDamage}\nrank : {fighters[fighterIndex].CurrentRank}";
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

        private void EraseEquippedInventory(EQUIPMENT_TYPE type)
        {
            InventorySlot slot = equippedinventorySlots[(int)type];
            if (slot.transform.childCount == 0) return;
            Destroy(equippedinventorySlots[(int)type].transform.GetChild(0).gameObject);
        }

        private void SpawnInventoryItem(InventorySlot slot, Equipment item)
        {
            if (item == null) return;
            GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
            InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
            inventoryItem.ShowUIItem(item);
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
            Helmet helmet = new Helmet($"{modifiers1[Random.Range(0, modifiers1.Length)]} {modifiers2[Random.Range(0, modifiers2.Length)]}'s helmet", Random.Range(1, 10), helmetScriptable.uiIcons[idx], helmetScriptable.gameIcons[idx]);
            idx = Random.Range(0, chestplateScriptable.uiIcons.Length);
            Chestplate chestplate = new Chestplate($"{modifiers1[Random.Range(0, modifiers1.Length)]} {modifiers2[Random.Range(0, modifiers2.Length)]}'s chestplate", Random.Range(1, 10), chestplateScriptable.uiIcons[idx], chestplateScriptable.gameIcons[idx]);
            idx = Random.Range(0, leggingsScriptable.uiIcons.Length);
            Leggings leggings = new Leggings($"{modifiers1[Random.Range(0, modifiers1.Length)]} {modifiers2[Random.Range(0, modifiers2.Length)]}'s leggings", Random.Range(1, 10), leggingsScriptable.uiIcons[idx], leggingsScriptable.gameIcons[idx]);
            idx = Random.Range(0, bootsScriptable.uiIcons.Length);
            Boots boots = new Boots($"{modifiers1[Random.Range(0, modifiers1.Length)]} {modifiers2[Random.Range(0, modifiers2.Length)]}'s boots", Random.Range(1, 10), bootsScriptable.uiIcons[idx], bootsScriptable.gameIcons[idx]);
            idx = Random.Range(0, weaponScriptable.uiIcons.Length);
            Weapon weapon = new Weapon($"{modifiers1[Random.Range(0, modifiers1.Length)]} {modifiers2[Random.Range(0, modifiers2.Length)]}'s {weaponType[Random.Range(0, weaponType.Length)]}", Random.Range(1, 10), weaponScriptable.uiIcons[idx], weaponScriptable.gameIcons[idx]);
            
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
    }
}
