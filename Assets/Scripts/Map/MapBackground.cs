using UnityEngine;

namespace Map
{
    public class MapBackground : MonoBehaviour
    {
        private void OnMouseDown()
        {
            MapManager.Instance.FocusOff();
        }
    }
}
