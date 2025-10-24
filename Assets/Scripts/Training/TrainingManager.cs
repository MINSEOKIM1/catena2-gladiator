using System.Collections;
using Bucket.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Training
{
    public class TrainingManager : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Button attackTrainingButton;
        [SerializeField] private Button defenseTrainingButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private TMP_Text statusText;

        [Header("Character")]
        [SerializeField] private Animator characterAnimator;

        private bool attack;

        private void SetButtonsInteractable(bool value)
        {
            attackTrainingButton.interactable = value;
            defenseTrainingButton.interactable = value;
            quitButton.interactable = value;
        }

        private IEnumerator PerformTraining()
        {
            const float trainingDuration = 3f;
            const float trainingStatusDisplayDuration = 3f;
            
            // TODO: Add character training animation
            
            yield return new WaitForSeconds(trainingDuration);

            characterAnimator.SetBool("Attack", false);
            characterAnimator.SetBool("Defense", false);
            SetButtonsInteractable(true);
            statusText.gameObject.SetActive(true);

            if (attack)
            {
                ManagementPhaseManager.Instance.fighters[0].AddBasicDamage(2);
                statusText.text = $"공격력이 2 증가했습니다!\n시간이 지났습니다.";
            }
            else
            {
                ManagementPhaseManager.Instance.fighters[0].AddBasicDefense(2);
                statusText.text = $"방어력이 2 증가했습니다!\n시간이 지났습니다.";
            }

            if (DataManager.Instance.TimePassNoShow())
            {
                statusText.text += "\n예정된 경기를 진행하지 않아 인기도가 하락하였습니다...";
            }

            DataManager.Instance.hp -= 15;

            yield return new WaitForSeconds(trainingStatusDisplayDuration);
            
            statusText.gameObject.SetActive(false);
        }

        private IEnumerator FailTraining()
        {
            if (DataManager.Instance.hp <= 15)
            {
                statusText.text = $"체력이 너무 없습니다...";                
            }
            else
            {
                statusText.text = $"밤에는 잠을 자야합니다. 돌아갑시다...";
            }
            statusText.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(1f);
            
            statusText.gameObject.SetActive(false);
        }
        
        
        public void TrainAttack()
        {
            if (DataManager.Instance.time == 3 || DataManager.Instance.hp < 15)
            {
                StartCoroutine(FailTraining());
                return;
            }
            
            SetButtonsInteractable(false);
            
            // TODO: Determine training outcome value
            const int atkIncrease = 2;
            // TODO: Apply training outcome

            
            characterAnimator.SetBool("Attack", true);

            attack = true;
            
            StartCoroutine(PerformTraining());
        }
        
        public void TrainDefense()
        {
            if (DataManager.Instance.time == 3 || DataManager.Instance.hp < 15)
            {
                StartCoroutine(FailTraining());
                return;
            }
            
            SetButtonsInteractable(false);
            
            // TODO: Determine training outcome value
            const int defIncrease = 2;
            // TODO: Apply training outcome

            statusText.text = $"방어력이 {defIncrease} 증가했습니다!";
            characterAnimator.SetBool("Defense", true);
            
            attack = false;
            
            StartCoroutine(PerformTraining());
        }

        public void QuitTraining()
        {
            SceneManager.LoadScene("Scenes/Map");
        }
    }
}
