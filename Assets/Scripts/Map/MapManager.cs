using TMPro;
using UnityEngine;

namespace Map
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }
        
        [Header("References")]
        [SerializeField] private GameObject nodesParent;
        [SerializeField] private GameObject leftPanel;
        [SerializeField] private TMP_Text placeNameText;
        [SerializeField] private TMP_Text descriptionText;

        [Header("Settings")]
        public int currentProgress;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Start()
        {
            // TODO: Get currentProgress
            
            InitializeNodes();
        }

        private void InitializeNodes()
        {
            var nodes = nodesParent.GetComponentsInChildren<MapNode>();

            foreach (var node in nodes)
            {
                node.SetNodeState(currentProgress);
            }
        }

        public void FocusOnNode(MapNode node)
        {
            placeNameText.text = node.placeName;
            descriptionText.text = node.description;

            leftPanel.SetActive(true);
            // TODO: Add animation
        }

        public void FocusOff()
        {
            leftPanel.SetActive(false);
            // TODO: Add animation
        }
    }
}
