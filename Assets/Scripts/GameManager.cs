using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public bool GamePause => gamePaused;

    private bool gamePaused;
    private TextMeshProUGUI debugText;

    public void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        gamePaused = false;
    }

    public void Start()
    {
        debugText = GameObject.Find("Debug Text").GetComponent<TextMeshProUGUI >();
    }

    public void Update()
    {
        Input();
        DrawUI();
    }

    private void Input()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            gamePaused = !gamePaused;
        }
    }

    private void DrawUI()
    {
        if (!gamePaused)
        {
            debugText.text = string.Empty;
        }
        else
        {
            debugText.text = "Pause";
        }
    }
}
