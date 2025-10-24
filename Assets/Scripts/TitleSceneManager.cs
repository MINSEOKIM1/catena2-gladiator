using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    public Image fadeImage;
    public float timeToFadeIn = 5f;

    public void OnStart()
    {
        StartCoroutine(SceneChange());
    }

    private IEnumerator SceneChange()
    {
        fadeImage.gameObject.SetActive(true);

        while (fadeImage.color.a < 0.95f)
        {
            fadeImage.color = Color.Lerp(fadeImage.color, Color.black, timeToFadeIn * Time.deltaTime);
            yield return null;
        }
        
        SceneManager.LoadScene("BucketScene");
    }
}
