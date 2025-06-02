using UnityEngine;

[CreateAssetMenu(menuName = "New Mecha/New Mecha")]
public class MechaData : ScriptableObject
{
    [Header("Basic Info")]
    public string mechaID;
    public string mechaName;
    [TextArea] public string mechaDescription;

    [Header("Weapon Info")]
    public BulletData bulletData;

    [Header("Visuals")]
    public Sprite mechaHead;
    public Sprite mechaBody;
    public Sprite mechaShoulder;
    public Sprite mechaArm;
    public Sprite mechaThigh;
    public Sprite mechaLegs;
    
    [Header("Sound Effects")]
    public AudioClip attackClip;
    public AudioClip dieClip;
    public AudioClip hitClip;            

    [Header("Stats")]
    public float health;
    public float speed;
    public float dashForce;

    [Header("VFX")]
    public AnimationClip hitAnimation;
}
