using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Bucket
{
    public enum EQUIPMENT_TYPE
    {
        HELMET,
        CHESTPLATE,
        LEGGINGS,
        BOOTS,
        WEAPON
    }

    [Serializable]
    public class Equipment
    {
        public EQUIPMENT_TYPE type;
        [FormerlySerializedAs("Name")] [SerializeField]
        public string itemName = "장비";
        [FormerlySerializedAs("Damage")] [SerializeField]
        public int damage = 5;
        [FormerlySerializedAs("Armor")] [SerializeField]
        public int armor = 5;

        public Sprite uiIcon;
        public Sprite gameIcon;

        public string itemInfo = "fill in blank";
        
        public Equipment(EQUIPMENT_TYPE equipmentType, string name, int damage, int armor, Sprite uiIcon, Sprite gameIcon)
        {
            type = equipmentType;
            itemName = name;
            this.damage = damage;
            this.armor = armor;
            this.uiIcon = uiIcon;
            this.gameIcon =  gameIcon;
        }
    }

    [Serializable]
    public class Helmet : Equipment
    {
        public Helmet(string name, int armor, Sprite icon1, Sprite icon2) : base(EQUIPMENT_TYPE.HELMET, name, 0, armor, icon1, icon2)
        {

        }
    
    }
    [Serializable]
    public class Chestplate : Equipment
    {
        public Chestplate(string name, int armor, Sprite icon1, Sprite icon2) : base(EQUIPMENT_TYPE.CHESTPLATE, name, 0, armor, icon1, icon2)
        {
        
        }
    }
    [Serializable]
    public class Leggings : Equipment
    {
        public Leggings(string name, int armor, Sprite icon1, Sprite icon2) : base(EQUIPMENT_TYPE.LEGGINGS, name, 0, armor, icon1, icon2)
        {
        
        }
    }
    [Serializable]
    public class Boots : Equipment
    {
        public Boots(string name, int armor, Sprite icon1, Sprite icon2) : base(EQUIPMENT_TYPE.BOOTS, name, 0, armor, icon1, icon2)
        {
        
        }
    }
    [Serializable]
    public class Weapon : Equipment
    {
        public Weapon(string name, int damage, Sprite icon1, Sprite icon2) : base(EQUIPMENT_TYPE.WEAPON, name, damage, 0, icon1, icon2)
        {
        
        }
    }
}