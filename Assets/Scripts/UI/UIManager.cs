using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField]
    private GameObject promptImageObject;

    [SerializeField]
    private TMP_Text promptText;

    //Unity Functions
    //====================================================================================================================//


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //====================================================================================================================//

    public void ShowPromptWindow(in bool showWindow, in string displayText)
    {
        promptImageObject.SetActive(showWindow);

        promptText.text = displayText;
    }
    
}
