using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechaController : MonoBehaviour
{
    [SerializeField] private MechaStatus mechaStatus;

    [Header("References")]
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] private Transform visual;

    [Header("Movement Settings")]
    private float moveSpeed = 5f;

    [Header("Animation Settings")]
    private Animator animator;
    private AnimationClip idleAnimation;
    private AnimationClip runAnimation;
    private AnimationClip hurtAnimation;




    private InputSystem_Actions inputActions;
    private Vector2 moveInput;
    public enum MechaState { Idle, Move, Hurt, Die }
    public MechaState currentState;
    private MechaState previousState;

    public bool IsFacingRight => visual.localScale.x > 0;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }

    private void Start()
    {
        animator = mechaStatus.animator;

        idleAnimation = mechaStatus.hitAnimation; // fallback kalau idle animasi nggak ada, bisa custom nanti
        runAnimation = mechaStatus.hitAnimation;
        hurtAnimation = mechaStatus.hitAnimation;

        moveSpeed = mechaStatus.speed;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;
        inputActions.Player.Disable();
    }

    private void Update()
    {
        UpdateState();
    }

    private void FixedUpdate()
    {
        // Horizontal velocity langsung
        rb.linearVelocityX = moveInput.x * moveSpeed;

        // Cuma apply gaya ke atas, dan bukan gaya horizontal
        if (moveInput.y > 0.1f)
        {
            float upwardForce = moveInput.y * moveSpeed;

            // Bisa kamu tweak biar ngangkat sesuai mass & gravityScale
            rb.AddForce(Vector2.up * 20, ForceMode2D.Force);
            Debug.Log($"Applying upward force: {upwardForce}");
        }
    }


    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        HandleFlip(moveInput.x);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void HandleFlip(float xInput)
    {
        if (xInput > 0.1f)
            visual.localScale = new Vector3(Mathf.Abs(visual.localScale.x), visual.localScale.y, visual.localScale.z);
        else if (xInput < -0.1f)
            visual.localScale = new Vector3(-Mathf.Abs(visual.localScale.x), visual.localScale.y, visual.localScale.z);
    }

    private void UpdateState()
    {
        if (mechaStatus.health <= 0)
        {
            currentState = MechaState.Die;
        }
        else if (mechaStatus.isHurting)
        {
            currentState = MechaState.Hurt;
        }
        else if (moveInput.sqrMagnitude > 0.1f)
        {
            currentState = MechaState.Move;
        }
        else
        {
            currentState = MechaState.Idle;
        }
    }

    
}
