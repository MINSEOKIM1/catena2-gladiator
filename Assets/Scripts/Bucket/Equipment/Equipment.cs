using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bucket
{
    public enum EQUIPMENT_TYPE
    {
        NULL = -1,
        HELMET,
        CHESTPLATE,
        LEGGINGS,
        BOOTS,
        WEAPON
    }

    [Serializable]
    public enum EQUIPMENT_ATTRIBUTE
    {
        ATTACK_POWER,   //공격력
        DAMAGE,         //데미지
        DEFENSE,        //방어력
        CRITICAL,       //크리티컬
        IGNORE_DEFENSE, //방어력 무시/관통
    }

    public static class EQUIPMENT_KOTRANSLATE
    {
        public static string ToString(EQUIPMENT_ATTRIBUTE attribute)
        {
            switch (attribute)
            {
                case EQUIPMENT_ATTRIBUTE.ATTACK_POWER:
                    return "공격력";
                case EQUIPMENT_ATTRIBUTE.DAMAGE:
                    return "데미지";
                case EQUIPMENT_ATTRIBUTE.DEFENSE:
                    return "방어력";
                case EQUIPMENT_ATTRIBUTE.CRITICAL:
                    return "크리티컬";
                case EQUIPMENT_ATTRIBUTE.IGNORE_DEFENSE:
                    return "방어력 무시";
                default:
                    return "미식별 속성";
            }
        }

        public static string ToString(EQUIPMENT_TYPE type)
        {
            switch (type)
            {
                case EQUIPMENT_TYPE.HELMET:
                    return "헬멧";
                case EQUIPMENT_TYPE.CHESTPLATE:
                    return "상의";
                case EQUIPMENT_TYPE.LEGGINGS:
                    return "하의";
                case EQUIPMENT_TYPE.BOOTS:
                    return "신발";
                case EQUIPMENT_TYPE.WEAPON:
                    return "무기";
                default:
                    return "미식별 장비 타입";
            }
        }
    }
    
    [Serializable]
    public class Equipment
    {
        //반드시 세팅될 속성들!
        public EQUIPMENT_TYPE type;
        [SerializeField]
        public string itemName = "장비";
        /*[FormerlySerializedAs("Damage")] [SerializeField]
        public int damage = 5;
        [FormerlySerializedAs("Armor")] [SerializeField]
        public int defense = 5;*/
        public Sprite uiIcon;
        public Sprite gameIcon;

        //부가 속성들!
        public string itemInfo = "Item Info";
        
        public List<AttributeSet> attributes = new();

        public Equipment()
        {
            type = EQUIPMENT_TYPE.NULL;
            itemName = "";
            this.uiIcon = null;
            this.gameIcon = null;
        }
        
        public Equipment(EQUIPMENT_TYPE equipmentType, string name, Sprite uiIcon, Sprite gameIcon)
        {
            type = equipmentType;
            itemName = name;
            /*this.damage = damage;
            this.defense = defense;*/
            this.uiIcon = uiIcon;
            this.gameIcon =  gameIcon;
        }

        public void AddAttribute(EQUIPMENT_ATTRIBUTE attribute, int value)
        {
            foreach (AttributeSet set in attributes)
            {
                if (set.attribute == attribute)
                {
                    set.value = value;
                    return;
                }
            }
            
            attributes.Add(new AttributeSet(attribute, value));
        }

        public void RemoveAttribute(EQUIPMENT_ATTRIBUTE attribute)
        {
            foreach (AttributeSet set in attributes)
            {
                if (set.attribute == attribute)
                {
                    attributes.Remove(set);
                    return;
                }
            }
        }

        public void SetInfo(string s)
        {
            itemInfo = s;
        }

        public int GetAttributeValue(EQUIPMENT_ATTRIBUTE attribute)
        {
            if (attributes.Count == 0)
            {
                return 0;
            }
            foreach (AttributeSet set in attributes)
            {
                if (set.attribute.Equals(attribute))
                {
                    return set.value;
                }
            }

            return 0;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(itemName)) return "Null Item";
            string s = $"{itemName}\n{EQUIPMENT_KOTRANSLATE.ToString(type)}\n";

            /*
            foreach (AttributeSet set in attributes)
            {
                s += $"\n{EQUIPMENT_ATTRIBUTE_KOTRANSLATE.ToString(set.attribute)} : {set.value}";
            }
            */

            
            //설정된 attribute 순서로 반드시 반환하기
            //공격력 --> 방어력 이 순서
            //딱히 이유는 없긴 함
            foreach (EQUIPMENT_ATTRIBUTE a in Enum.GetValues(typeof(EQUIPMENT_ATTRIBUTE)))
            {
                foreach (AttributeSet set in attributes)
                {
                    if (set.attribute.Equals(a))
                    {
                        s += $"\n{EQUIPMENT_KOTRANSLATE.ToString(set.attribute)} : {set.value}";
                        break;
                    }
                }
            }
            
            s += "\n\n" + itemInfo + "\n";
            
            return s;
        }
    }

    [Serializable]
    public class Helmet : Equipment
    {
        public Helmet(string name, Sprite icon1, Sprite icon2) : base(EQUIPMENT_TYPE.HELMET, name, icon1, icon2)
        {

        }
    
    }
    [Serializable]
    public class Chestplate : Equipment
    {
        public Chestplate(string name, Sprite icon1, Sprite icon2) : base(EQUIPMENT_TYPE.CHESTPLATE, name, icon1, icon2)
        {
        
        }
    }
    [Serializable]
    public class Leggings : Equipment
    {
        public Leggings(string name, Sprite icon1, Sprite icon2) : base(EQUIPMENT_TYPE.LEGGINGS, name, icon1, icon2)
        {
        
        }
    }
    [Serializable]
    public class Boots : Equipment
    {
        public Boots(string name, Sprite icon1, Sprite icon2) : base(EQUIPMENT_TYPE.BOOTS, name, icon1, icon2)
        {
        
        }
    }
    [Serializable]
    public class Weapon : Equipment
    {
        public Weapon(string name, Sprite icon1, Sprite icon2) : base(EQUIPMENT_TYPE.WEAPON, name, icon1, icon2)
        {
        
        }
    }
    
    [Serializable]
    public class AttributeSet
    {
        public EQUIPMENT_ATTRIBUTE attribute;
        public int value;

        public AttributeSet(EQUIPMENT_ATTRIBUTE attribute, int value)
        {
            this.attribute = attribute;
            this.value = value;
        }
    }
}