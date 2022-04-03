using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private enum UISTATE
    {
        NONE,
        MENU,
        GAME,
        GAME_OVER
    }

    //Properties
    //====================================================================================================================//

    [Header("Fading"),SerializeField]
    private Image fadeImage;
    private bool _fading;
    
    //GameUI Properties
    //====================================================================================================================//

    [Header("Game UI"), SerializeField] private GameObject gameUIWindow;
    
    [SerializeField]
    private GameObject promptImageObject;

    [SerializeField]
    private TMP_Text promptText;

    //Main Menu UI Properties
    //====================================================================================================================//
    [Header("Main Menu UI"), SerializeField] 
    private GameObject menuUIWindow;
    [SerializeField]
    private CanvasGroup mainMenuCanvasGroup;
    [SerializeField]
    private Button playGameButton;
    [SerializeField]
    private Button settingsButton;
    [SerializeField]
    private Button quitButton;
    
    //Main Menu UI Properties
    //====================================================================================================================//
    [Header("Game Over UI"), SerializeField] 
    private GameObject gameOverUIWindow;
    [SerializeField]
    private CanvasGroup gameOverCanvasGroup;
    [SerializeField]
    private Button restartButton;

    //====================================================================================================================//
    

    private VolcanoController _volcanoController;
    private CameraLook _playerCameraLook;
    private PlayerController _playerController;

    //Unity Functions
    //====================================================================================================================//


    private void Awake()
    {
        Instance = this;

        _volcanoController = FindObjectOfType<VolcanoController>();
        _playerCameraLook = FindObjectOfType<CameraLook>();
        _playerController = FindObjectOfType<PlayerController>();
    }

    private void OnEnable()
    {
        VolcanoController.OnGameOver += OnGameOver;
    }

    

    // Start is called before the first frame update
    private void Start()
    {

        FadeUI(1f, 0f, 1.5f);
        
#if UNITY_WEBGL
        quitButton.gameObject.SetActive(false);
#endif

        SetupButtons();
        SetUIState(UISTATE.MENU);
        ShowPromptWindow(false, string.Empty);
        
        _playerCameraLook.gameObject.SetActive(false);
        _playerCameraLook.enabled = false;
        _volcanoController.enabled = false;
        _playerController.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            LockCursor(!_cursorLocked);
        }
    }
    
    private void OnDisable()
    {
        VolcanoController.OnGameOver -= OnGameOver;
    }

    //Setup UI
    //====================================================================================================================//

    private void SetupButtons()
    {
        playGameButton.onClick.AddListener(() =>
        {
            _playerCameraLook.gameObject.SetActive(true);
           SetUIState(UISTATE.GAME);
            this.DelayedCall(1.6f, () =>
            {
                _playerCameraLook.enabled = true;
                _playerController.enabled = true;
                _volcanoController.enabled = true;
            });
            FadeCanvasGroup(mainMenuCanvasGroup, 1f, 0f, 1f);
            AudioController.PlaySound(AudioController.SOUND.UI_Press);
        });
        settingsButton.onClick.AddListener(() =>
        {
            AudioController.PlaySound(AudioController.SOUND.UI_Press);
        });
        
        restartButton.onClick.AddListener(() =>
        {
            FadeUI(0f,1f, 1f);
            this.DelayedCall(1.1f, () =>
            {
                SceneManager.LoadScene(0);
            });
            
        });
        
        quitButton.onClick.AddListener(Application.Quit);
    }
    
    private bool _cursorLocked;
    private void LockCursor(in bool state)
    {
        _cursorLocked = state;
        
        Cursor.lockState = _cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !_cursorLocked;
    }

    //====================================================================================================================//

    public void ShowPromptWindow(in bool showWindow, in string displayText)
    {
        promptImageObject.SetActive(showWindow);

        promptText.text = displayText;
    }

    private void FadeUI(float fadeFromAlpha, float fadeToAlpha, float fadeTime)
    {
        IEnumerator FadeCoroutine()
        {
            _fading = true;
            var color = fadeImage.color;
            color.a = fadeFromAlpha;
            fadeImage.color = color;

            for (float t = 0; t <= fadeTime; t+=Time.deltaTime)
            {
                color.a = Mathf.Lerp(fadeFromAlpha, fadeToAlpha, t / fadeTime);
                fadeImage.color = color;
                yield return null;
            }
            
            color.a = fadeToAlpha;
            fadeImage.color = color;
            _fading = false;
        }

        if (_fading)
            return;
        
        StartCoroutine(FadeCoroutine());
    }

    private void FadeCanvasGroup(CanvasGroup target, float fadeFromAlpha, float fadeToAlpha, float fadeTime)
    {
        IEnumerator FadeCoroutine()
        {
            target.alpha = fadeFromAlpha;
            
            for (float t = 0; t <= fadeTime; t+=Time.deltaTime)
            {
                target.alpha = Mathf.Lerp(fadeFromAlpha, fadeToAlpha, t / fadeTime);
                yield return null;
            }
            target.alpha = fadeToAlpha;
        }

        StartCoroutine(FadeCoroutine());
    }

    //====================================================================================================================//

    private void SetUIState(in UISTATE uiState)
    {
        switch (uiState)
        {
            case UISTATE.NONE:
                menuUIWindow.gameObject.SetActive(false);
                gameUIWindow.gameObject.SetActive(false);
                gameOverUIWindow.gameObject.SetActive(false);
                LockCursor(false);
                break;
            case UISTATE.MENU:
                menuUIWindow.gameObject.SetActive(true);
                gameUIWindow.gameObject.SetActive(false);
                gameOverUIWindow.gameObject.SetActive(false);
                LockCursor(false);
                break;
            case UISTATE.GAME:
                mainMenuCanvasGroup.interactable = false;
                mainMenuCanvasGroup.blocksRaycasts = false;
                //menuUIWindow.gameObject.SetActive(false);
                gameUIWindow.gameObject.SetActive(true);
                gameOverUIWindow.gameObject.SetActive(false);
                LockCursor(true);
                break;
            case UISTATE.GAME_OVER:
                menuUIWindow.gameObject.SetActive(false);
                gameUIWindow.gameObject.SetActive(false);
                gameOverUIWindow.gameObject.SetActive(true);
                LockCursor(false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(uiState), uiState, null);
        }
        
    }

    //Callback Functions
    //====================================================================================================================//
    
    private void OnGameOver()
    {
        gameOverCanvasGroup.alpha = 0f;
        this.DelayedCall(5, () =>
        {
            FadeCanvasGroup(gameOverCanvasGroup, 0f, 1f, 1f);
            SetUIState(UISTATE.GAME_OVER);
        });
    }

    //====================================================================================================================//
    
}
