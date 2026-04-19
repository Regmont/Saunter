using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 3;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isPaused = false;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void Update()
    {
        if (GameManager.Instance.GamePause || GameManager.Instance.IsSitting || GameManager.Instance.GameNotStarted)
        {
            if (!isPaused)
            {
                PausePhysics();
            }

            return;
        }
        else
        {
            if (isPaused)
            {
                ResumePhysics();
            }
        }

        GetInput();
    }

    public void FixedUpdate()
    {
        if (!(GameManager.Instance.GamePause || GameManager.Instance.IsSitting || GameManager.Instance.GameNotStarted))
        {
            Move();
        }
    }

    private void PausePhysics()
    {
        isPaused = true;
        rb.isKinematic = true;
    }

    private void ResumePhysics()
    {
        isPaused = false;
        rb.isKinematic = false;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bench"))
        {
            GameManager.Instance.SetSitAvailable(true);
            GameManager.Instance.SetCurrentBench(other.GameObject());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bench"))
        {
            GameManager.Instance.SetSitAvailable(false);
            GameManager.Instance.SetCurrentBench(null);
        }
    }
}
