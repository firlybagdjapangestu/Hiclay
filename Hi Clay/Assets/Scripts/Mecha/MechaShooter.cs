using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;

public class MechaShooter : MonoBehaviour
{
    [Header("Mecha Status")]
    [SerializeField] private MechaStatus mechaStatus;
    [SerializeField] private MechaController mechaController;
    [SerializeField] private float defaultAngleRight = 0f;
    [SerializeField] private float defaultAngleLeft = 180f;

    [Header("Input System")]
    private InputSystem_Actions inputActions;
    private Vector2 lookDirection;

    [Header("References")]
    [SerializeField] private Transform visual;
    [SerializeField] private Transform startBulletTransform;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float returnSpeed;

    private ObjectPoolManager objectPoolManager;
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
        currentBulletData = mechaStatus.bulletData;
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


    private IEnumerator ShootWithDelay()
    {
        canShoot = false;
        Shoot();
        mechaStatus.mechaSFXChannel.RaiseEvent(mechaStatus.attackClip);
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
            bullet.ApplyBulletData(currentBulletData);
            bullet.SetDirection(lookDirection.normalized);
            bullet.Activate();
        }
    }

    public void SetBulletData(BulletData newBulletData)
    {
        currentBulletData = newBulletData;
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
            scale.y = mechaController.IsFacingRight ? 1f : -1f;
            transform.localScale = scale;

            // Arahkan rotasi ke sudut default idle
            float targetAngle = mechaController.IsFacingRight ? defaultAngleRight : defaultAngleLeft;
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
}
