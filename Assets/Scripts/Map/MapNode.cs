using UnityEngine;
using UnityEngine.EventSystems;

namespace Map
{
    public class MapNode : MonoBehaviour, IPointerDownHandler
    {
        [Header("Assets")]
        [SerializeField] private Sprite enabledSprite;
        [SerializeField] private Sprite disabledSprite;
        
        [Header("Settings")]
        public int requiredProgress;

        public string sceneName;
        public string placeName;
        [Multiline] public string description;

        [Header("Values")]
        public bool nodeEnabled;

        public void SetNodeState(int progress)
        {
            if (progress >= requiredProgress)
            {
                nodeEnabled = true;
                GetComponent<Collider2D>().enabled = true;
                GetComponent<SpriteRenderer>().sprite = enabledSprite;
            }
            else
            {
                nodeEnabled = false;
                GetComponent<Collider2D>().enabled = false;
                GetComponent<SpriteRenderer>().sprite = disabledSprite;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!nodeEnabled) return;
            
            MapManager.Instance.FocusOnNode(this);
        }
    }
}
