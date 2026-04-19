using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovementVertical : MonoBehaviour
{
    [SerializeField]
    private float mouseSensitivityVertical = 30f;
    private float yRotation = 0f;

    public void Update()
    {
        if (!(GameManager.Instance.GamePause || GameManager.Instance.IsSitting || GameManager.Instance.GameNotStarted))
        {
            Rotate();
        }
    }

    public void ResetYRotation()
    {
        yRotation = 0f;
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
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
