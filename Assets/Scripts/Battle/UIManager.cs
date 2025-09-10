using UnityEngine;

namespace Battle
{
    public class UIManager : MonoBehaviour
    {
        [Header("Attack Timer")]
        [SerializeField] private GameObject attackTimingBarObject;
        [SerializeField] private RectTransform attackTimingCorrectRange;
        [SerializeField] private RectTransform attackTimingHandle;

        public void ToggleAttackTimingBar(bool value)
        {
            attackTimingBarObject.SetActive(value);
        }

        public void SetAttackCorrectTimingRange(float startRatio, float endRatio)
        {
            var anchorMin = attackTimingCorrectRange.anchorMin;
            var anchorMax = attackTimingCorrectRange.anchorMax;

            anchorMin.x = startRatio;
            anchorMax.x = endRatio;
            
            attackTimingCorrectRange.anchorMin = anchorMin;
            attackTimingCorrectRange.anchorMax = anchorMax;
        }
        
        public void UpdateAttackTimingBar(float value)
        {
            var anchorMin = attackTimingHandle.anchorMin;
            var anchorMax = attackTimingHandle.anchorMax;

            anchorMin.x = value;
            anchorMax.x = value;
            
            attackTimingHandle.anchorMin = anchorMin;
            attackTimingHandle.anchorMax = anchorMax;
        }
    }
}
