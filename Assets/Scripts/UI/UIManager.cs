using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private enum UISTATE
    {
        NONE,
        MENU,
        GAME
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
    private Button playGameButton;
    [SerializeField]
    private Button settingsButton;
    [SerializeField]
    private Button quitButton;

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
            AudioController.PlaySound(AudioController.SOUND.UI_Press);
        });
        settingsButton.onClick.AddListener(() =>
        {
            AudioController.PlaySound(AudioController.SOUND.UI_Press);
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

    public void FadeUI(float fadeFromAlpha, float fadeToAlpha, float fadeTime)
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

    //====================================================================================================================//

    private void SetUIState(in UISTATE uiState)
    {
        switch (uiState)
        {
            case UISTATE.NONE:
                menuUIWindow.gameObject.SetActive(false);
                gameUIWindow.gameObject.SetActive(false);
                LockCursor(false);
                break;
            case UISTATE.MENU:
                menuUIWindow.gameObject.SetActive(true);
                gameUIWindow.gameObject.SetActive(false);
                LockCursor(false);
                break;
            case UISTATE.GAME:
                menuUIWindow.gameObject.SetActive(false);
                gameUIWindow.gameObject.SetActive(true);
                LockCursor(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(uiState), uiState, null);
        }
        
    }
}
