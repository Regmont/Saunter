using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool GamePause => gamePaused;
    public bool IsSitting => isSitting;
    public bool GameNotStarted => !gameStarted;
    public float MouseSensitivity => mouseSensitivity;

    private float mouseSensitivity = 30;

    private bool gamePaused;
    private bool canSit;
    private bool isSitting;
    private bool gameStarted;

    private TextMeshProUGUI interactionText;

    private GameObject currentBench;
    private GameObject player;
    private GameObject gridPause;
    private GameObject gridMainMenu;
    private GameObject settingsContainer;
    private GameObject audioManager;

    private bool isAnimating = false;

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

        gamePaused = false;
        canSit = false;
        isSitting = false;
        gameStarted = false;
    }

    public void Start()
    {
        interactionText = GameObject.Find("Interaction Text").GetComponent<TextMeshProUGUI>();
        gridPause = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(go => go.name == "GridPause" && go.scene.isLoaded);
        gridMainMenu = GameObject.Find("GridMainMenu");
        settingsContainer = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(go => go.name == "SettingsContainer" && go.scene.isLoaded);
        audioManager = GameObject.Find("AudioManager");

        player = GameObject.FindWithTag("Player");

        AudioClip music = Resources.Load<AudioClip>("Music/Terraria");
        AudioManager.Instance.PlayMusic(music);
    }

    public void Update()
    {
        if (gameStarted)
        {
            Input();
            SetPauseMenuActive(gamePaused);
        }
    }

    public void SetSitAvailable(bool available)
    {
        canSit = available;

        SetHideText(!canSit);
    }

    public void SetCurrentBench(GameObject currentBench)
    {
        this.currentBench = currentBench;
    }

    public void BackToGameButtonClicked()
    {
        SetPauseMenuActive(false);
    }

    public void MainMenuButtonClicked()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Include);

        foreach (GameObject obj in allObjects)
        {
            if (obj.scene.buildIndex == -1)
            {
                Destroy(obj);
            }
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartGameButtonClicked()
    {
        gameStarted = true;
        gridMainMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ExitButtonClicked()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    public void SettingsButtonClicked()
    {
        settingsContainer.SetActive(true);

        if (gameStarted)
        {
            gridPause.SetActive(false);
        }
        else
        {
            gridMainMenu.SetActive(false);
        }
    }

    public void BackButtonClicked()
    {
        if (gameStarted)
        {
            gridPause.SetActive(true);
        }
        else
        {
            gridMainMenu.SetActive(true);
        }
            
        settingsContainer.SetActive(false);
    }

    public void MouseSensitivityChange(float value)
    {
        mouseSensitivity = value;
    }

    public void MisicVolumeChange(float value)
    {
        AudioSource[] sources = audioManager.GetComponents<AudioSource>();
        AudioSource musicSource = sources[0];

        musicSource.volume = value;
    }

    public void SfxVolumeChange(float value)
    {
        AudioSource[] sources = audioManager.GetComponents<AudioSource>();
        AudioSource musicSource = sources[1];

        musicSource.volume = value;
    }

    private void SetPauseMenuActive(bool active)
    {
        if (gridPause == null)
        {
            return;
        }

        if (!active)
        {
            StartCoroutine(DeactivateGrid());
            gamePaused = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            gridPause.SetActive(true);
        }
    }

    private IEnumerator DeactivateGrid()
    {
        yield return null;
        gridPause.SetActive(false);
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
        else
        {
            if (!gamePaused)
            {
                if (Keyboard.current.spaceKey.wasPressedThisFrame && canSit && !isAnimating)
                {
                    if (!isSitting)
                    {
                        SetHideText(true);
                        SitOnBench();
                    }
                    else
                    {
                        SetHideText(true);
                        GetUpFromBench();
                    }
                }
            }
        }
    }

    private void SitOnBench()
    {
        Collider[] colliders = currentBench.GetComponents<Collider>();
        foreach (Collider col in colliders)
        {
            if (col.isTrigger)
            {
                col.enabled = false;
            }
        }

        isAnimating = true;
        isSitting = true;

        Vector3 sitPosition = currentBench.transform.position + currentBench.transform.forward * 1f;
        sitPosition.y = player.transform.position.y;
        player.transform.position = sitPosition;

        Vector3 directionToBench = currentBench.transform.position - player.transform.position;
        directionToBench.y = 0;
        player.transform.rotation = Quaternion.LookRotation(directionToBench);

        Camera.main.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);

        isSitting = true;
        StartCoroutine(AnimateCamera());
    }

    private void GetUpFromBench()
    {
        isAnimating = true;
        StartCoroutine(ReturnCamera());
        isSitting = false;

        player.GetComponent<CameraMovementHorizontal>().ResetXRotation();
        Camera.main.GetComponent<CameraMovementVertical>().ResetYRotation();

        SetHideText(false);

        if (currentBench != null)
        {
            Collider[] colliders = currentBench.GetComponents<Collider>();
            foreach (Collider col in colliders)
            {
                if (col.isTrigger)
                {
                    col.enabled = true;
                }
            }
        }
    }

    private IEnumerator ReturnCamera()
    {
        Transform cameraTransform = Camera.main.transform;
        Transform playerTransform = player.transform;

        Vector3 startLocalPosition = cameraTransform.localPosition;
        Vector3 targetLocalPosition = new Vector3(0, 0.5f, 0);

        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            cameraTransform.localPosition = Vector3.Lerp(startLocalPosition, targetLocalPosition, t);

            yield return null;
        }

        cameraTransform.localPosition = targetLocalPosition;
        isAnimating = false;
    }

    private IEnumerator AnimateCamera()
    {
        Transform cameraTransform = Camera.main.transform;
        Transform playerTransform = player.transform;

        float duration = 2f;
        float elapsed = 0f;

        Quaternion playerStartRotation = playerTransform.rotation;
        Quaternion playerTargetRotation = playerStartRotation * Quaternion.Euler(0, 180f, 0);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            playerTransform.rotation = Quaternion.Slerp(playerStartRotation, playerTargetRotation, t);

            yield return null;
        }

        playerTransform.rotation = playerTargetRotation;

        elapsed = 0f;
        Vector3 startLocalPosition = cameraTransform.localPosition;
        Vector3 targetLocalPosition = startLocalPosition + new Vector3(0, 0, -0.7f);

        float startLocalY = cameraTransform.localPosition.y;
        float targetLocalY = startLocalY - 0.2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Vector3 newLocalPos = Vector3.Lerp(startLocalPosition, targetLocalPosition, t);
            newLocalPos.y = Mathf.Lerp(startLocalY, targetLocalY, t);
            cameraTransform.localPosition = newLocalPos;

            yield return null;
        }

        cameraTransform.localPosition = new Vector3(targetLocalPosition.x, targetLocalY, targetLocalPosition.z);
        isAnimating = false;
    }

    private void SetHideText(bool hide)
    {
        if (hide)
        {
            interactionText.text = string.Empty;
        }
        else
        {
            interactionText.text = "Press Space to sit";
        }
    }
}
