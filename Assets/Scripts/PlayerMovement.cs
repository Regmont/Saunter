using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 3;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private GameManager gameManager;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void Update()
    {
        if (!gameManager.GamePause)
        {
            GetInput();
        }
    }

    public void FixedUpdate()
    {
        if (!gameManager.GamePause)
        {
            Move();
        }
    }

    private void GetInput()
    {
        moveDirection = Vector3.zero;

        if (Keyboard.current.aKey.isPressed)
        {
            moveDirection -= transform.right;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            moveDirection += transform.right;
        }

        if (Keyboard.current.wKey.isPressed)
        {
            moveDirection += transform.forward;
        }

        if (Keyboard.current.sKey.isPressed)
        {
            moveDirection -= transform.forward;
        }

        moveDirection = moveDirection.normalized;
    }

    private void Move()
    {
        Vector3 targetPosition = rb.position + moveDirection * speed * Time.fixedDeltaTime;
        rb.MovePosition(targetPosition);
    }
}
