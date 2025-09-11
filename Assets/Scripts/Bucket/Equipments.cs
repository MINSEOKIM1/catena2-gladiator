using System;
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
                    if (newEquipment == null) _helmet = null;
                    else _helmet = new Helmet(newEquipment.itemName, newEquipment.armor, newEquipment.uiIcon, newEquipment.gameIcon);
                    return;
                case EQUIPMENT_TYPE.CHESTPLATE:
                    //_chestplate = (Chestplate)newEquipment;
                    if (newEquipment == null) _chestplate = null;
                    else _chestplate = new Chestplate(newEquipment.itemName, newEquipment.armor, newEquipment.uiIcon, newEquipment.gameIcon);
                    return;
                case EQUIPMENT_TYPE.LEGGINGS:
                    //_leggings = (Leggings)newEquipment;
                    if (newEquipment == null) _leggings = null;
                    else _leggings = new Leggings(newEquipment.itemName, newEquipment.armor, newEquipment.uiIcon, newEquipment.gameIcon);
                    return;
                case EQUIPMENT_TYPE.BOOTS:
                    //_boots = (Boots)newEquipment;
                    if (newEquipment == null) _boots = null;
                    else _boots = new Boots(newEquipment.itemName, newEquipment.armor, newEquipment.uiIcon, newEquipment.gameIcon);
                    return;
                case EQUIPMENT_TYPE.WEAPON:
                    //_weapon = (Weapon)newEquipment;
                    if (newEquipment == null) _weapon = null;
                    else _weapon = new Weapon(newEquipment.itemName, newEquipment.damage, newEquipment.uiIcon, newEquipment.gameIcon);
                    return;
                case null:
                    return;
                default:
                    Debug.LogError("장비 타입이 어느 것과도 일치하지 않음 에러");
                    return;
            }

        }
        
        public Equipment GetEquipment(int i)
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

        /// <summary>
        /// 드래곤볼 스카우터
        /// 가중치는 내 마음임
        /// </summary>
        /// <returns>전투력</returns>
        public int GetPower()
        {
            int power = 0;
            power += _helmet != null ? _helmet.armor * 1 : 0;
            power += _chestplate != null ? _chestplate.armor * 1 : 0;
            power += _leggings != null ? _leggings.armor * 1 : 0;
            power += _boots != null ? _boots.armor * 1 : 0;
            power += _weapon != null ? _weapon.damage * 1 : 0;

            return power;
        }
    }
}
