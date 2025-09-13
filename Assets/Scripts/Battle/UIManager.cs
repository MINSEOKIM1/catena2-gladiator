using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    public class UIManager : MonoBehaviour
    {
        [Header("Attack Timer")]
        [SerializeField] private GameObject attackTimingBarObject;
        [SerializeField] private RectTransform attackTimingCorrectRange;
        [SerializeField] private RectTransform attackTimingHandle;

        [Header("Counter Popup")]
        [SerializeField] private GameObject counterPopup;
        [SerializeField] private Image counterPopupIconImage;
        [SerializeField] private TMP_Text counterPopupText;

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
        
        public void SetAttackTimingBar(float value)
        {
            var anchorMin = attackTimingHandle.anchorMin;
            var anchorMax = attackTimingHandle.anchorMax;

            anchorMin.x = value;
            anchorMax.x = value;
            
            attackTimingHandle.anchorMin = anchorMin;
            attackTimingHandle.anchorMax = anchorMax;
        }

        public void ToggleCounterPopup(bool value)
        {
            counterPopup.SetActive(value);
        }

        public void SetCounterPopupIconFill(float value)
        {
            counterPopupIconImage.fillAmount = value;
        }

        public void SetCounterPopupText(string value)
        {
            counterPopupText.text = value;
        }
    }
}
