using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreEntry : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI rankField;
    [SerializeField]
    TextMeshProUGUI nameField;
    [SerializeField]
    TextMeshProUGUI scoreField;
    [SerializeField]
    TextMeshProUGUI elapsedTimeField;

    private void Awake()
    {
        if (rankField == null)
        {
            Debug.LogError("ScoreEntry.cs - rankField not set");
            rankField = null;
        }
        if (nameField == null)
        {
            Debug.LogError("ScoreEntry.cs - nameField not set");
            nameField = null;
        }

        if (scoreField == null)
        {
            Debug.LogError("ScoreEntry.cs - scoreField not set");
            scoreField = null;
        }

        if (elapsedTimeField == null)
        {
            Debug.LogError("ScoreEntry.cs - elapsedTimeField not set");
            elapsedTimeField = null;
        }

    }

    public string Rank
    {
        get
        {
            return rankField.text;
        }
        set
        {
            rankField.SetText(value);
        }
    }
    public string Name
    {
        get
        {
            return nameField.text;
        }
        set
        {
            nameField.SetText(value);
        }
    }

    public string Score
    {
        get
        {
            return scoreField.text;
        }
        set
        {
            scoreField.SetText(value);
        }
    }

    public string ElapsedTime
    {
        get
        {
            return elapsedTimeField.text;
        }
        set
        {
            elapsedTimeField.SetText(value);
        }
    }

}
