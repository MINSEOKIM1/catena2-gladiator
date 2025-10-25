using UnityEngine;

[CreateAssetMenu(fileName = "BattleSoundReference", menuName = "ScriptableObjects/Sound/BattleSoundReference")]
public class BattleSoundReference : ScriptableObject
{
    [Header("SFX")]
    public AudioClip crowdCheer;
    public AudioClip crowdBoo;
    
    public AudioClip swordCollision;
    public AudioClip dodge;
    public AudioClip heavyHit;
}
