using UnityEngine;

[CreateAssetMenu(menuName = "New Character/New Character")]
public class PlayerData : ScriptableObject
{
    [Header("Basic Info")]
    public string characterID;
    public string characterName;
    [TextArea] public string characterDescription;

    [Header("Weapon Info")]
    public BulletData bulletData;

    [Header("Visuals")]
    public Sprite characterIcon;
    public Sprite characterHead;
    public Sprite characterBody;
    public Sprite characterLegs;
    public Sprite characterWeapon;

    [Header("Sound Effects")]
    public AudioClip attackClip;
    public AudioClip dieClip;
    public AudioClip hitClip;            // Saat mati
    public AudioClip[] idleClips;          // Gumaman saat idle

    [Header("Animation Settings")]
    public AnimationClip idleAnimation;
    public AnimationClip runAnimation;
    public AnimationClip jumpAnimation;
    public AnimationClip hurtAnimation;   


    [Header("Stats")]
    public float health;
    public float speed;
    public float jumpForce;
    public float dashForce;

    [Header("VFX")]
    public AnimationClip hitAnimation;

    
}
