using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 3.5f;
    public float runSpeed = 7f;
    public float rotationSpeed = 10f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Interacción")]
    public float interactRange = 2.5f;
    public LayerMask interactableLayer;

    [Header("Referencias")]
    public Transform cameraTransform; // arrastrar la Main Camera / vcam aquí

    private CharacterController controller;
    private Animator animator;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isRunning;

    // Nombres de parámetros del Animator (deben coincidir con el Animator Controller)
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int InteractHash = Animator.StringToHash("Interact");

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleJump();
        HandleAttack();
        HandleInteract();

        // Gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleGroundCheck()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // pequeño valor para mantenerlo pegado al suelo
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal"); // A/D
        float v = Input.GetAxisRaw("Vertical");   // W/S
        isRunning = Input.GetKey(KeyCode.LeftShift);

        Vector3 inputDir = new Vector3(h, 0f, v).normalized;

        // El personaje SIEMPRE mira hacia donde mira la cámara (evita que la
        // cámara "salte" al moverse hacia los costados o hacia atrás).
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Quaternion facingRotation = Quaternion.LookRotation(camForward, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, facingRotation,
                                               rotationSpeed * Time.deltaTime);

        if (inputDir.magnitude >= 0.1f)
        {
            // Movimiento tipo "strafe": adelante/atrás/lateral relativo a la cámara,
            // sin importar hacia dónde gire el cuerpo.
            Vector3 moveDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;
            float speed = isRunning ? runSpeed : walkSpeed;
            controller.Move(moveDir * speed * Time.deltaTime);

            animator.SetFloat(SpeedHash, isRunning ? 1f : 0.5f, 0.1f, Time.deltaTime);
        }
        else
        {
            animator.SetFloat(SpeedHash, 0f, 0.1f, Time.deltaTime);
        }
    }

    void HandleJump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger(JumpHash);
        }
    }

    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0)) // Click izquierdo
        {
            animator.SetTrigger(AttackHash);
            // Aquí podés agregar detección de colisión / daño con un Overlap o Raycast
        }
    }

    void HandleInteract()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger(InteractHash);

            Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, interactableLayer);
            if (hits.Length > 0)
            {
                // Ejemplo: cada objeto interactuable puede tener su propio script
                // con un método Interact() que se llama aquí
                hits[0].SendMessage("Interact", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}