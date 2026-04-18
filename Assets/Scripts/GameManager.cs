using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool GamePause => gamePaused;
    public bool IsSitting => isSitting;

    private bool gamePaused;
    private bool canSit;
    private bool isSitting;

    private TextMeshProUGUI debugText;
    private TextMeshProUGUI interactionText;

    private GameObject currentBench;
    private GameObject player;

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

        Cursor.lockState = CursorLockMode.Locked;
        gamePaused = false;
        canSit = false;
        isSitting = false;
    }

    public void Start()
    {
        debugText = GameObject.Find("Debug Text").GetComponent<TextMeshProUGUI>();
        interactionText = GameObject.Find("Interaction Text").GetComponent<TextMeshProUGUI>();
        player = GameObject.FindWithTag("Player");

        AudioClip music = Resources.Load<AudioClip>("Music/Terraria");
        AudioManager.Instance.PlayMusic(music);
    }

    public void Update()
    {
        Input();
        DrawUI();
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
