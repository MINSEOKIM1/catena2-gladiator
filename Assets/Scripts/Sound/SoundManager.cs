using UnityEditor;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region fields
    public static SoundManager Instance; //singleton instance
    [SerializeField] private AudioSource crowdEffectSource;
    [SerializeField] private AudioSource battleEffectSource;
    #endregion
    #region References
    private BattleSoundReference _battleSoundReference;
    #endregion
    void Awake()
    {
        //singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        _battleSoundReference = Resources.Load("ScriptableObject" + "/Sound/BattleSoundReference") as BattleSoundReference;
        crowdEffectSource.volume = 0.8f;
        battleEffectSource.volume = 0.6f;
    }
    
    //For temporary use
    public void playSFX(string sfxName)
    {
        AudioClip clip = null;
        switch (sfxName)
        {
            case "crowdCheer":
                clip = _battleSoundReference.crowdCheer;
                crowdEffectSource.PlayOneShot(clip);
                break;
            case "crowdBoo":
                clip = _battleSoundReference.crowdBoo;
                crowdEffectSource.PlayOneShot(clip);
                break;
            case "swordCollision":
                clip = _battleSoundReference.swordCollision;
                battleEffectSource.PlayOneShot(clip);
                break;
            case "dodge":
                clip = _battleSoundReference.dodge;
                battleEffectSource.PlayOneShot(clip);
                break;
            case "heavyHit":
                clip = _battleSoundReference.heavyHit;
                battleEffectSource.PlayOneShot(clip);
                break;
            default:
                Debug.LogWarning("SFX name not found: " + sfxName);
                return;
        }
        
    }
    
    public void stopAllSFX()
    {
        crowdEffectSource.Stop();
        battleEffectSource.Stop();
    }
  
}
