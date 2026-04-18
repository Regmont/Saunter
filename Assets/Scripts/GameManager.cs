using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool GamePause => gamePaused;

    private bool gamePaused;
    private TextMeshProUGUI debugText;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        gamePaused = false;
    }

    public void Start()
    {
        debugText = GameObject.Find("Debug Text").GetComponent<TextMeshProUGUI>();
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

            if (gamePaused)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    private void DrawUI()
    {
        if (debugText == null) return;

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
