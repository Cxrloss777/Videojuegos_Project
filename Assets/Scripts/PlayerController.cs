using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float rotationSmoothTime = 0.12f;
    public float gravity = -19.62f;

    [Header("Grounding (evita que el personaje flote)")]
    public float groundedOffset = 0.1f;
    public float groundedRadius = 0.25f;
    public LayerMask groundLayers;

    [Header("Salto (opcional)")]
    public bool allowJump = true;
    public float jumpHeight = 1.2f;

    [Header("Camara (controlada por Cinemachine)")]
    [Tooltip("La Camera real de la escena (la que tiene el CinemachineBrain). Se usa solo para saber hacia donde mirar, Cinemachine la mueve solo.")]
    public Transform cameraTransform;
    public bool lockCursor = true;

    [Header("Animacion")]
    public float animationSmoothTime = 0.1f;
    public float walkAnimSpeed = 0.5f;
    public float runAnimSpeed = 1f;

    private CharacterController controller;
    private Animator animator;

    private bool isGrounded;
    private float rotationVelocity;
    private float verticalVelocity;
    private const float terminalVelocity = -53f;

    private float currentAnimSpeed;
    private float animVelocity;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        GroundedCheck();
        ApplyGravity();
        Move();
        HandleAttack();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool willLock = Cursor.lockState != CursorLockMode.Locked;
            Cursor.lockState = willLock ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !willLock;
        }
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(
            transform.position.x,
            transform.position.y + groundedOffset,
            transform.position.z);

        isGrounded = Physics.CheckSphere(
            spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);

        if (isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;
    }

    private void ApplyGravity()
    {
        if (verticalVelocity > terminalVelocity)
            verticalVelocity += gravity * Time.deltaTime;

        if (isGrounded && allowJump && Input.GetButtonDown("Jump"))
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger(JumpHash);
        }
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool sprint = Input.GetKey(KeyCode.LeftShift);

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;
        float targetSpeed = sprint ? sprintSpeed : moveSpeed;

        // Cinemachine ya esta moviendo/rotando la Camera real por su cuenta;
        // aca solo LEEMOS su yaw actual para saber hacia donde es "adelante".
        float cameraYaw = cameraTransform != null ? cameraTransform.eulerAngles.y : 0f;

        float targetAnimSpeed = 0f;

        if (inputDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg
                                 + cameraYaw;
            float smoothAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(
                moveDirection.normalized * targetSpeed * Time.deltaTime
                + new Vector3(0, verticalVelocity, 0) * Time.deltaTime);

            targetAnimSpeed = sprint ? runAnimSpeed : walkAnimSpeed;
        }
        else
        {
            controller.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
        }

        currentAnimSpeed = Mathf.SmoothDamp(currentAnimSpeed, targetAnimSpeed, ref animVelocity, animationSmoothTime);
        animator.SetFloat(SpeedHash, currentAnimSpeed);
    }

    private void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger(AttackHash);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? new Color(0, 1, 0, 0.4f) : new Color(1, 0, 0, 0.4f);
        Vector3 spherePosition = new Vector3(
            transform.position.x, transform.position.y + groundedOffset, transform.position.z);
        Gizmos.DrawSphere(spherePosition, groundedRadius);
    }
}