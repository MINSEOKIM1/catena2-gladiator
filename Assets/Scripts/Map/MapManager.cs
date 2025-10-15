using TMPro;
using UnityEngine;

namespace Map
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }

        private const float CameraDefaultSize = 5f;
        private const float CameraZoomedSize = 5.5f;
        private readonly Vector3 cameraOriginalPosition = new(0f, 0f, -10f);
        
        [Header("References")]
        [SerializeField] private GameObject nodesParent;
        [SerializeField] private GameObject leftPanel;
        [SerializeField] private TMP_Text placeNameText;
        [SerializeField] private TMP_Text descriptionText;

        [Header("Settings")]
        public int currentProgress;

        private float _cameraZoomedOffsetX;
        private Camera _mainCamera;

        private void Awake()
        {
            Instance = this;

            _mainCamera = Camera.main;
            if (_mainCamera == null) return;

            // Calculate camera zoomed offset X
            var leftPanelWidth = leftPanel.GetComponent<RectTransform>().rect.width;
            var worldWidth = 2f * CameraZoomedSize * _mainCamera.aspect;
            _cameraZoomedOffsetX = worldWidth * (leftPanelWidth / 2f) / Screen.width;
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

            _mainCamera.orthographicSize = CameraZoomedSize;

            // Move camera to focus on the node
            var cameraTargetPos = node.transform.position;
            cameraTargetPos.z = _mainCamera.transform.position.z;
            cameraTargetPos.x -= _cameraZoomedOffsetX;
            _mainCamera.transform.position = cameraTargetPos;
        }

        public void FocusOff()
        {
            leftPanel.SetActive(false);
            // TODO: Add animation
            
            _mainCamera.orthographicSize = CameraDefaultSize;
            _mainCamera.transform.position = cameraOriginalPosition;
        }
    }
}
