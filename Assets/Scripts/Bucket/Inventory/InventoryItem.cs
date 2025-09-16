using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bucket.Inventory
{
    public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public bool isClickable = true;
        public Equipment item;
    
        
        private Image image;

        public Transform parentAfterDrag;

        /*public void Awake()
        {
            image = GetComponent<Image>();
            parentAfterDrag = transform.parent;
        }

        public void Start()
        {
            if (item != null) ShowUIItem(item);
        }*/

        public void OnEnable()
        {
            image = GetComponent<Image>();
            parentAfterDrag = transform.parent;
            if (item != null) ShowUIItem(item);
        }

        public void ShowUIItem(Equipment item)
        {
            if (item == null) return;
            this.item = item;
            image.sprite = item.uiIcon;
            transform.SetParent(parentAfterDrag);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!isClickable) return;
            //Debug.Log("begin drag");
            image.raycastTarget = false;
            parentAfterDrag = transform.parent;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isClickable) return;
            //Debug.Log("dragging");
            transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isClickable) return;
            //Debug.Log("end drag");
            image.raycastTarget = true;
            transform.SetParent(parentAfterDrag);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //Debug.Log(item.ToString());
            ManagementPhaseManager.Instance.ShowItemInfoOnHover(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ManagementPhaseManager.Instance.HideItemInfoOnHover();
        }

        private const float interval = 0.25f;
        private float lastClickedTime = -1.0f;

        public void OnPointerClick(PointerEventData eData)
        {
            if (!isClickable) return;
            if ((Time.time - lastClickedTime) < interval)
            {
                //더블 클릭
                lastClickedTime = -1.0f;

                transform.parent.GetComponent<InventorySlot>().DoubleClickItem(this);
            }
            else
            {
                //그냥 클릭
                lastClickedTime = Time.time;
            }
        }
    }
}
