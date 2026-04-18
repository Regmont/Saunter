using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovementVertical : MonoBehaviour
{
    [SerializeField]
    private float mouseSensitivityVertical = 30f;
    private float yRotation = 0f;
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

        float mouseY = mouseDelta.y * mouseSensitivityVertical * Time.deltaTime;

        yRotation -= mouseY;

        yRotation = Mathf.Clamp(yRotation, -80f, 80f);

        transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);
    }
}
