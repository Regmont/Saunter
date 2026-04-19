using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovementHorizontal : MonoBehaviour
{
    private float xRotation = 0f;

    public void Update()
    {
        if (!(GameManager.Instance.GamePause || GameManager.Instance.IsSitting || GameManager.Instance.GameNotStarted))
        {
            Rotate();
        }
    }

    public void ResetXRotation()
    {
        xRotation = 0f;
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private void Rotate()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * GameManager.Instance.MouseSensitivity * Time.deltaTime;

        xRotation += mouseX;

        transform.localRotation = Quaternion.Euler(0f, xRotation, 0f);
    }
}
