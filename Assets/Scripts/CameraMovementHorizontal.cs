using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovementHorizontal : MonoBehaviour
{
    [SerializeField]
    private float mouseSensitivityHorizontal = 30f;
    private float xRotation = 0f;
    private GameManager gameManager;

    public void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void Update()
    {
        if (!gameManager.GamePause)
        {
            Rotate();
        }
    }

    private void Rotate()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivityHorizontal * Time.deltaTime;

        xRotation += mouseX;

        transform.localRotation = Quaternion.Euler(0f, xRotation, 0f);
    }
}
