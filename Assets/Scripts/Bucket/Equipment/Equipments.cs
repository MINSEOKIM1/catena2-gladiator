using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bucket
{
    [Serializable]
    public class Equipments
    {
        [SerializeField]
        public Helmet _helmet;
        [SerializeField]
        public Chestplate _chestplate;
        [SerializeField]
        public Leggings _leggings;
        [SerializeField]
        public Boots _boots;
        [SerializeField]
        public Weapon _weapon;

        public Equipments(Helmet helmet, Chestplate chestplate, Leggings leggings, Boots boots, Weapon weapon)
        {
            _helmet = helmet;
            _chestplate = chestplate;
            _leggings = leggings;
            _boots = boots;
            _weapon = weapon;
        }

        /// <summary>
        /// 교체하는 장비, 교체 부위
        /// 두 개 타입이 일치하지 않으면 안바꿔줌 --> 그대로 반환 아무 일도 없음
        /// 타입이 일치하면 기존 장비를 뺴고 새 장비 끼기
        /// 기존에 끼고 있던 장비 반환
        ///
        /// 더블클릭을 통해 장비 교체시 targetType = newEquipment.EQUIPMENT_TYPE
        /// 드래그 앤 드롭으로 장비 교체시 targetType 은 클릭한 부분의 장비 타입 넣기
        ///
        /// 근데 일단 UI 없고 귀찮으니까 기능만 만들기 ㅎㅎ
        /// 나중에 UI 생기면 그 때 어떻게 할지 정하죠 하하
        /// </summary>
        /// <param name="newEquipment">새로 끼는 장비</param>
        /// <param name="targetType">교체하고자 하는 부위 (새 장비의 부위가 아님)</param>
        public void EquipNewEquipment(Equipment newEquipment, EQUIPMENT_TYPE? type)
        {
            switch (type)
            { 
                case EQUIPMENT_TYPE.HELMET:
                    //_helmet = (Helmet)newEquipment;
                    if (newEquipment == null) _helmet = new Equipment() as Helmet;
                    else
                    {
                        _helmet = new Helmet(newEquipment.itemName, newEquipment.uiIcon, newEquipment.gameIcon);
                        _helmet.itemInfo = newEquipment.itemInfo;
                        _helmet.attributes = newEquipment.attributes;
                    }
                    break;
                case EQUIPMENT_TYPE.CHESTPLATE:
                    //_chestplate = (Chestplate)newEquipment;
                    if (newEquipment == null) _chestplate = new Equipment() as Chestplate;
                    else
                    {
                        _chestplate = new Chestplate(newEquipment.itemName, newEquipment.uiIcon, newEquipment.gameIcon);
                        _chestplate.itemInfo = newEquipment.itemInfo;
                        _chestplate.attributes = newEquipment.attributes;
                    }
                    break;
                case EQUIPMENT_TYPE.LEGGINGS:
                    //_leggings = (Leggings)newEquipment;
                    if (newEquipment == null) _leggings = new Equipment() as Leggings;
                    else
                    {
                        _leggings = new Leggings(newEquipment.itemName, newEquipment.uiIcon, newEquipment.gameIcon);
                        _leggings.itemInfo = newEquipment.itemInfo;
                        _leggings.attributes = newEquipment.attributes;
                    }
                    break;
                case EQUIPMENT_TYPE.BOOTS:
                    //_boots = (Boots)newEquipment;
                    if (newEquipment == null) _boots = new Equipment() as Boots;
                    else
                    {
                        _boots = new Boots(newEquipment.itemName, newEquipment.uiIcon, newEquipment.gameIcon);
                        _boots.itemInfo = newEquipment.itemInfo;
                        _boots.attributes = newEquipment.attributes;
                    }
                    break;
                case EQUIPMENT_TYPE.WEAPON:
                    //_weapon = (Weapon)newEquipment;
                    if (newEquipment == null) _weapon = new Equipment() as Weapon;
                    else
                    {
                        _weapon = new Weapon(newEquipment.itemName, newEquipment.uiIcon, newEquipment.gameIcon);
                        _weapon.itemInfo = newEquipment.itemInfo;
                        _weapon.attributes = newEquipment.attributes;
                    }
                    break;
                case null:
                    break;
                default:
                    Debug.LogError("장비 타입이 어느 것과도 일치하지 않음 에러");
                    break;
            }

        }
        
        public Equipment GetEquipmentByIndex(int i)
        {
            return i switch
            {
                0 => _helmet,
                1 => _chestplate,
                2 => _leggings,
                3 => _boots,
                4 => _weapon,
                _ => null
            };
        }
        
        public List<Equipment> GetParts()
        {
            return new List<Equipment>{_helmet, _chestplate, _leggings,  _boots, _weapon};
        }

        /// <summary>
        /// 드래곤볼 스카우터
        /// </summary>
        /// <returns></returns>
        public int GetAttackPower()
        {
            int power = 0;
            foreach (Equipment e in GetParts())
            {
                Debug.Log(e);
                if (e != null && !string.IsNullOrEmpty(e.itemName)) power += e.GetAttributeValue(EQUIPMENT_ATTRIBUTE.ATTACK_POWER);
                //what the fuck
                //monobehaviour 상속하면 이 함수가 작동한다는 사실
                //if (e != null) power += e.GetAttributeValue(EQUIPMENT_ATTRIBUTE.ATTACK_POWER);
            }
            
            return power;
        }

        public int GetDefensePower()
        {
            int power = 0;
            foreach (Equipment e in GetParts())
            {
                if (e != null && !string.IsNullOrEmpty(e.itemName)) power += e.GetAttributeValue(EQUIPMENT_ATTRIBUTE.DEFENSE);
            }
            
            return power;
        }

        public int GetTotalEquipmentPower()
        {
            return GetAttackPower() + GetDefensePower();
        }
    }
}
