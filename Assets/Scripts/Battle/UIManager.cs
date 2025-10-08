using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    public class UIManager : MonoBehaviour
    {
        [Header("Gladiator Status")]
        [SerializeField] private Slider gladiatorHealthSlider;
        
        [Header("Enemy Status")]
        [SerializeField] private Slider enemyHealthSlider;
        
        [Header("Attack Timer")]
        [SerializeField] private GameObject attackTimingBarObject;
        [SerializeField] private RectTransform attackTimingCorrectRange;
        [SerializeField] private RectTransform attackTimingHandle;

        [Header("Counter Popup")]
        [SerializeField] private GameObject counterPopup;
        [SerializeField] private Slider counterTimingSlider;
        [SerializeField] private Transform counterCommandsParent;

        [Space(10)]
        [SerializeField] private GameObject counterCommandPrefab;

        private GameObject[] currentCommandObjects;

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

        public void SetCounterTimingBar(float value)
        {
            counterTimingSlider.value = value;
        }

        public void SetCounterCommands(CommandType[] commands)
        {
            currentCommandObjects = new GameObject[commands.Length];
            
            for (var i = 0; i < commands.Length; i++)
            {
                var command = commands[i];
                var commandObject = Instantiate(counterCommandPrefab, counterCommandsParent);
                
                var rotationAngle = command switch
                {
                    CommandType.Up => 0f,
                    CommandType.Left => 90f,
                    CommandType.Down => 180f,
                    CommandType.Right => 270f,
                    _ => 0f
                };
                commandObject.transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);

                currentCommandObjects[i] = commandObject;
            }
        }

        public void SetCounterCommandActive(int index)
        {
            currentCommandObjects[index].GetComponent<Image>().color = Color.yellow;
            // TODO: fix this later
        }
        
        public void ClearCounterCommands()
        {
            if (currentCommandObjects == null) return;
            
            foreach (var commandObject in currentCommandObjects)
            {
                Destroy(commandObject);
            }
            currentCommandObjects = null;
        }
        
        public void SetGladiatorHealth(float value)
        {
            gladiatorHealthSlider.value = value;
        }
        
        public void SetEnemyHealth(float value)
        {
            enemyHealthSlider.value = value;
        }
    }
}
