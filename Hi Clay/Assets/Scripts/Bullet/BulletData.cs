using UnityEngine;


public enum BulletType
{
    Straight,
    Homing
    // Tambah lagi: Zigzag, Exploding, dkk
}

[CreateAssetMenu(menuName = "Bullet/Bullet Data")]
public class BulletData : ScriptableObject
{
    public string bulletName;
    public float health;
    public float speed = 10f;
    public float damage = 5f;
    public float lifetime = 5f;
    public float fireRate = 0.2f;
    public float areaOfEffect = 0f;
    public bool isPiercing = false;
    public float knockbackForce = 0f;

    public GameObject hitEffect;
    public AnimationClip hitAnimation;
    public Sprite bulletSprite;
    public GameObject bulletGameObject;
    public BulletType bulletType;
}

