using UnityEngine;

namespace Map
{
    public class MapNode : MonoBehaviour
    {
        [Header("Assets")]
        [SerializeField] private Sprite enabledSprite;
        [SerializeField] private Sprite disabledSprite;
        
        [Header("Settings")]
        public int requiredProgress;

        public string sceneName;
        public string placeName;
        [Multiline] public string description;

        private bool _enabled;

        public void SetNodeState(int progress)
        {
            if (progress >= requiredProgress)
            {
                _enabled = true;
                GetComponent<Collider2D>().enabled = true;
                GetComponent<SpriteRenderer>().sprite = enabledSprite;

                // TODO: Update visual state to enabled
            }
            else
            {
                _enabled = false;
                GetComponent<Collider2D>().enabled = false;
                GetComponent<SpriteRenderer>().sprite = disabledSprite;
                
                // TODO: Update visual state to disabled
            }
        }

        private void OnMouseDown()
        {
            if (!_enabled) return;
            
            Debug.Log("OnMouseDown");
            
            MapManager.Instance.FocusOnNode(this);
        }
    }
}
