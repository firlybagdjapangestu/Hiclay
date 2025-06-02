using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ShootingController : MonoBehaviour
{
    [Header("Player Status")]
    [SerializeField] private PlayerStatus playerStatus;

    [Header("Input System")]
    private InputSystem_Actions inputActions;
    private Vector2 lookDirection;

    [Header("Visual and Rotation")]
    [SerializeField] private Transform visual;
    [SerializeField] private float returnSpeed;

    [Header("Player Controller")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rigidbody2D rb;
    private float jumpForce;
    [SerializeField] private float defaultAngleRight = 0f;
    [SerializeField] private float defaultAngleLeft = 180f;

    [Header("Object Pooling")]
    private ObjectPoolManager objectPoolManager;

    [Header("Shooting")]
    [SerializeField] private Transform startBulletTransform;
    [SerializeField] private GameObject bulletPrefab;
    private BulletData currentBulletData;

    private bool canShoot = true;

    private void Awake()
    {
        objectPoolManager = Object.FindFirstObjectByType<ObjectPoolManager>();
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }

    private void Start()
    {
        currentBulletData = playerStatus.playerData.bulletData;
        rb = playerController.rb;
        jumpForce = playerStatus.jumpForce;
    }

    private void OnEnable()
    {
        inputActions.Player.Look.performed += OnLookPerformed;
        inputActions.Player.Look.canceled += OnLookCanceled;
    }

    private void OnDisable()
    {
        inputActions.Player.Look.performed -= OnLookPerformed;
        inputActions.Player.Look.canceled -= OnLookCanceled;
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        lookDirection = context.ReadValue<Vector2>();
    }

    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        lookDirection = Vector2.zero;
    }

    private void Update()
    {
        RotateTowardsDirection();
        if (lookDirection.sqrMagnitude > 0.01f && canShoot)
        {
            StartCoroutine(ShootWithDelay());
        }
    }

    private void RotateTowardsDirection()
    {
        if (lookDirection.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
            transform.localRotation = Quaternion.Euler(0f, 0f, angle);
            HandleFlip(angle); // kalau kamu masih pakai ini untuk keperluan tambahan
        }
        else
        {
            // Flip senjata secara vertikal saat idle
            Vector3 scale = transform.localScale;
            scale.y = playerController.IsFacingRight ? 1f : -1f;
            transform.localScale = scale;

            // Arahkan rotasi ke sudut default idle
            float targetAngle = playerController.IsFacingRight ? defaultAngleRight : defaultAngleLeft;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * returnSpeed);
        }
    }


    private void HandleFlip(float angle)
    {
        if (angle > 90f || angle < -90f)
        {
            visual.localScale = new Vector3(-Mathf.Abs(visual.localScale.x), visual.localScale.y, visual.localScale.z);
        }
        else
        {
            visual.localScale = new Vector3(Mathf.Abs(visual.localScale.x), visual.localScale.y, visual.localScale.z);
        }
    }

    private IEnumerator ShootWithDelay()
    {
        canShoot = false;
        Shoot();
        playerStatus.playerSFXChannel.RaiseEvent(playerStatus.attackClip);
        yield return new WaitForSeconds(currentBulletData != null ? currentBulletData.fireRate : 0.3f);
        canShoot = true;
    }

    private void Shoot()
    {
        GameObject bulletGO = objectPoolManager.ActiveObject(bulletPrefab, startBulletTransform.position, Quaternion.identity);
        bulletGO.layer = LayerMask.NameToLayer("Player");

        BulletController bullet = bulletGO.GetComponent<BulletController>();
        if (bullet != null)
        {
            Vector2 shootDir = lookDirection.normalized;
            bullet.ApplyBulletData(currentBulletData);
            bullet.SetDirection(shootDir);
            bullet.Activate();

            // Coba loncat jika nembak ke bawah
            TryJumpFromShootDirection(shootDir);
        }
        else
        {
            Debug.LogWarning("BulletController tidak ditemukan pada prefab bullet!");
        }
    }


    private void TryJumpFromShootDirection(Vector2 shootDir)
    {
        if (!playerController.isGrounded) return;
        // Cek apakah tembakan ke arah bawah  
        if (shootDir.y < -0.7f)
        {
            // Reset kecepatan vertikal biar lompatnya konsisten  
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            // Tambahkan gaya ke atas  
 
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }


    public void SetBulletData(BulletData newBulletData)
    {
        currentBulletData = newBulletData;
    }
}
