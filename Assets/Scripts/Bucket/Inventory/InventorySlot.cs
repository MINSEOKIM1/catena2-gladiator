using System.Linq;
using Bucket.Manager;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bucket.Inventory
{
    public class InventorySlot : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            InventoryItem item = eventData.pointerDrag.GetComponent<InventoryItem>();
            if (!IsItemMovingAvailable(item, out var equipment, out EQUIPMENT_TYPE? type))
            {
                //Debug.Log("Cant Move Item");
                return;
            }
            //Debug.Log("Can Move Item");
            if (transform.childCount != 0)
            {
                SwapInventoryItem(item, this);
            }
            else
            {
                MoveInventoryItem(item);
            }
            
            if (type != null)
            {
                item.transform.SetParent(item.parentAfterDrag);
                ManagementPhaseManager.Instance.fighters[0].Equipments.EquipNewEquipment(equipment, type);
                ManagementPhaseManager.Instance.RefreshStatInfo();
                ManagementPhaseManager.Instance.RefreshCharacterImageOnBackground((EQUIPMENT_TYPE)type);
            }
        }

        /// <summary>
        /// slot1 에서 slot2 로 item 이동
        /// 은 아니고 slot1과 slot2에 있는 템 정보를 바꿈
        /// </summary>
        /// <param name="slot1">from</param>
        /// <param name="slot2">to</param>
        public void SwapInventoryItem(InventoryItem item1, InventorySlot slot2)
        {
            //인벤토리에서의 이동
            //Debug.Log($"Swapping {item1.parentAfterDrag.name} to {slot2.name}");
            InventoryItem item2 = slot2.GetComponentInChildren<InventoryItem>();
            
            Equipment equipment1 = item1.item;
            Equipment equipment2 = item2.item;
            
            item1.ShowUIItem(equipment2);
            item2.ShowUIItem(equipment1);
        }

        /// <summary>
        /// item의 위치 이동
        /// </summary>
        /// <param name="item"></param>
        public void MoveInventoryItem(InventoryItem item)
        {
            //Debug.Log($"Moving {item.parentAfterDrag.name} to {name}");
            item.parentAfterDrag = transform;
        }

        //slot1은 item1.parentAfterDrag임
        //item1은 무조건 있음
        //slot2도 무조건 있음 (자기 자신임)
        //item2은 있는지 없는지 모름
        //slot1과 slot2모두 equippedInven이면 false
        //둘 중 하나만 equippedInven이면 unequipped인거에 대해 type보기
        //둘 다 아니면 true
        /// <summary>
        /// item1이 이 칸에 오는 것이 가능한지 보는 함수
        /// </summary>
        /// <param name="item1"></param>
        /// <returns></returns>
        public bool IsItemMovingAvailable(InventoryItem item1, out Equipment newEquipment, out EQUIPMENT_TYPE? type)
        {
            InventorySlot slot1 = item1.parentAfterDrag.GetComponent<InventorySlot>();
            InventorySlot slot2 = this;
            if (ManagementPhaseManager.Instance.equippedinventorySlots.Contains(slot1)
                    && ManagementPhaseManager.Instance.equippedinventorySlots.Contains(slot2))
            {
                newEquipment = null;
                type = null;
                return false;
            }

            if (!ManagementPhaseManager.Instance.equippedinventorySlots.Contains(slot1)
                && !ManagementPhaseManager.Instance.equippedinventorySlots.Contains(slot2))
            {
                newEquipment = null;
                type = null;
                return true;
            }
            
            if (ManagementPhaseManager.Instance.equippedinventorySlots.Contains(slot1))
            {
                type = FindEquippedSlotType(slot1);
                //장비해제 상황
                //slot1이 equipped
                //item2가 없으면 true
                if (slot2.transform.childCount == 0)
                {
                    newEquipment = null;
                    return true;
                }

                //item2가 있는데 item1이랑 같은 type이면 true 아니면 false
                newEquipment = slot2.GetComponentInChildren<InventoryItem>().item;
                return slot2.GetComponentInChildren<InventoryItem>().item.type == item1.item.type;
            }
            else
            {
                type = FindEquippedSlotType(slot2);
                //장비착용 상황
                //slot2이 equipped
                //slot2에 item1가 올 수 있으면 true 아니면 false
                newEquipment = item1.item;
                return item1.item.type.Equals(type);
            }
        }

        private EQUIPMENT_TYPE? FindEquippedSlotType(InventorySlot slot)
        {
            int i = 0;
            foreach (InventorySlot es in ManagementPhaseManager.Instance.equippedinventorySlots)
            {
                if (es.Equals(slot))
                {
                    return (EQUIPMENT_TYPE)i;
                }

                i++;
            }
            return null;
        }

        [CanBeNull]
        private InventorySlot FindEmptyUnEquippedSlot()
        {
            foreach (InventorySlot s in ManagementPhaseManager.Instance.unEquippedinventorySlots)
            {
                if (s.transform.childCount == 0) return s;
            }
            return null;
        }

        public void DoubleClickItem(InventoryItem item)
        {
            EQUIPMENT_TYPE? slotType = FindEquippedSlotType(this);
            
            //장착된 템 해제
            if (slotType != null)
            {
                InventorySlot slot = FindEmptyUnEquippedSlot();
                if (slot != null)
                {
                    slot.MoveInventoryItem(item);
                    item.transform.SetParent(item.parentAfterDrag);
                    ManagementPhaseManager.Instance.fighters[0].Equipments.EquipNewEquipment(null, item.item.type);
                    ManagementPhaseManager.Instance.RefreshStatInfo();
                    ManagementPhaseManager.Instance.RefreshCharacterImageOnBackground(item.item.type);
                }
                return;
            }
            
            //안 장착된 템 장착
            InventorySlot target = ManagementPhaseManager.Instance.equippedinventorySlots[(int)item.item.type];
            if (target.transform.childCount == 0)
            {
                target.MoveInventoryItem(item);
            }
            else
            {
                SwapInventoryItem(item, target);
            }
            item.transform.SetParent(item.parentAfterDrag);
            ManagementPhaseManager.Instance.fighters[0].Equipments.EquipNewEquipment(target.GetComponentInChildren<InventoryItem>().item, item.item.type);
            ManagementPhaseManager.Instance.RefreshStatInfo();
            ManagementPhaseManager.Instance.RefreshCharacterImageOnBackground(item.item.type);
        }
    }
}
