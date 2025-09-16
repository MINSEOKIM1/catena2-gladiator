using UnityEngine;

namespace Bucket
{
    public class EquipmentScriptable : ScriptableObject
    {
        public Sprite[] uiIcons;
        public Sprite[] gameIcons;
    }

    [CreateAssetMenu(menuName = "Equipment Scriptable/Helmet")]
    public class HelmetScriptable : EquipmentScriptable
    {}

    [CreateAssetMenu(menuName = "Equipment Scriptable/Chestplate")]
    public class ChestplateScriptable : EquipmentScriptable
    {}

    [CreateAssetMenu(menuName = "Equipment Scriptable/Leggings")]
    public class LeggingsScriptable : EquipmentScriptable
    {}

    [CreateAssetMenu(menuName = "Equipment Scriptable/Boots")]
    public class BootsScriptable : EquipmentScriptable
    {}

    [CreateAssetMenu(menuName = "Equipment Scriptable/Weapon")]
    public class WeaponScriptable : EquipmentScriptable
    {}
}