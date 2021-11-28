using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsOverlay : MonoBehaviour
{
    public TextMeshProUGUI upperLeftText;
    public TextMeshProUGUI upperRightText;
    public TextMeshProUGUI middleText;
    public TextMeshProUGUI bottomText;

    public TextMeshProUGUI bylineText;

    static public StatsOverlay Instance;


    void UpdateText(string text, TextMeshProUGUI textMesh)
    {
        if (textMesh != null)
        {
            textMesh.SetText(text);
        }
    }

    public void UpdateUpperLeftText(string text)
    {
        UpdateText(text, upperLeftText);
    }
    public void UpdateUpperRightText(string text)
    {
        UpdateText(text, upperRightText);
    }
    public void UpdateMiddleText(string text)
    {
        UpdateText(text, middleText);
    }
    public void UpdateBottomText(string text)
    {
        UpdateText(text, bottomText);
    }

    public void UpdateByLineText(string text)
    {
        UpdateText(text, bylineText);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    public void Start()
    {
        Canvas c = GetComponent<Canvas>();
        c.worldCamera = Camera.main;

        //upperRightText = transform.Find("Text - Upper Right").GetComponent<TextMeshProUGUI>();
        //upperLeftText = transform.Find("Text - Upper Left").GetComponent<TextMeshProUGUI>();
        //middleText = transform.Find("Text - Middle").GetComponent<TextMeshProUGUI>();
        //bottomText = transform.Find("Text - Bottom").GetComponent<TextMeshProUGUI>();

        //DontDestroyOnLoad(gameObject);
    }

}
