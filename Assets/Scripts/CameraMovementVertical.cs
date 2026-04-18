using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovementVertical : MonoBehaviour
{
    [SerializeField]
    private float mouseSensitivityVertical = 30f;
    private float yRotation = 0f;

    public void Start()
    {

    }

    public void Update()
    {
        Rotate();
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
