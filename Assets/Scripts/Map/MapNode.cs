using UnityEngine;

namespace Map
{
    public class MapNode : MonoBehaviour
    {
        public int requiredProgress;

        public string placeName;
        [Multiline] public string description;

        private bool _enabled;

        public void SetNodeState(int progress)
        {
            if (progress >= requiredProgress)
            {
                _enabled = true;
                
                // TODO: Update visual state to enabled
            }
            else
            {
                _enabled = false;
                
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
