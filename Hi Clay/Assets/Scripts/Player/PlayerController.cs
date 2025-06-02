using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus;

    public enum PlayerState { Idle, Run, Jump, Hurt, Die }

    [Header("References")]
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] private Transform visual;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Settings")]
    private float groundCheckRadius = 0.2f;
    private float moveSpeed = 5f;


    private Animator animator;
    private AnimationClip idleAnimation;
    private AnimationClip runAnimation;
    private AnimationClip jumpAnimation;
    private AnimationClip hurtAnimation;

    public bool isGrounded;
    private InputSystem_Actions inputActions;
    private Vector2 moveInput;
    public PlayerState currentState;
    private PlayerState previousState;

    public bool IsFacingRight => visual.localScale.x > 0;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }

    private void Start()
    {
        animator = playerStatus.animator;
        idleAnimation = playerStatus.idleAnimation;
        runAnimation = playerStatus.runAnimation;
        jumpAnimation = playerStatus.jumpAnimation;
        hurtAnimation = playerStatus.hurtAnimation;

        moveSpeed = playerStatus.speed;
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
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        UpdateState();
        HandleAnimation();
    }

    private void FixedUpdate()
    {
        rb.linearVelocityX = moveInput.x * moveSpeed;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveInput = input;
        HandleFlip(input.x);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    public void HandleFlip(float xInput)
    {
        if (xInput > 0.1f)
            visual.localScale = new Vector3(Mathf.Abs(visual.localScale.x), visual.localScale.y, visual.localScale.z);
        else if (xInput < -0.1f)
            visual.localScale = new Vector3(-Mathf.Abs(visual.localScale.x), visual.localScale.y, visual.localScale.z);
    }

    private void UpdateState()
    {
        if (playerStatus.health <= 0)
        {
            currentState = PlayerState.Die;
        }
        else if (!isGrounded)
        {
            currentState = PlayerState.Jump;
        }
        else if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            currentState = PlayerState.Run;
        }
        else
        {
            currentState = PlayerState.Idle;
        }
    }

    private void HandleAnimation()
    {
        if (currentState == previousState) return;

        switch (currentState)
        {
            case PlayerState.Idle:
                animator.Play(idleAnimation.name);
                break;
            case PlayerState.Run:
                animator.Play(runAnimation.name);
                break;
            case PlayerState.Jump:
                animator.Play(jumpAnimation.name);
                break;
            case PlayerState.Hurt:
                animator.Play(hurtAnimation.name);
                break;
            case PlayerState.Die:
                animator.Play("Die"); // ganti dengan nama animasi kematian kamu
                break;
        }

        previousState = currentState;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
