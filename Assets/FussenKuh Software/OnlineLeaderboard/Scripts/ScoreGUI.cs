using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FKS;

public class ScoreGUI : MonoBehaviour
{
    [SerializeField]
    OnlineScoreBoard scoreboard;

    [SerializeField]
    GameObject scoreEntryPrefab;

    [SerializeField]
    Transform scrollviewContents;


    private void OnRetrieveScoresEvent(object sender, OnlineScoreBoard.RetrieveScoresEventArgs e)
    {
        if (e.Error)
        {
            Debug.LogWarning(gameObject.name + ": There was a problem retrieving scores. Errors: " + e.ErrorString);
        }
        else
        {
            RefreshScores(e.Scores);
        }
    }


    public void RefreshScores(List<Score> scores)
    {
        // Delete all of the items currently in the UI
        for (int i=0; i< scrollviewContents.childCount; i++)
        {
            Destroy(scrollviewContents.GetChild(i).gameObject);
        }

        if (scoreEntryPrefab != null)
        {
            ScoreEntry infoEntry = Instantiate(scoreEntryPrefab, scrollviewContents).GetComponent<ScoreEntry>();
            infoEntry.Rank = "Rank";
            infoEntry.Name = "Name";
            infoEntry.Score = "Level";
            infoEntry.name = "Score " + infoEntry.Name + " " + infoEntry.Score;

            int rank = 1;
            foreach (Score score in scores)
            {
                if (rank > 10) { return; } // Only show the top 10 results
                ScoreEntry tmp = Instantiate(scoreEntryPrefab, scrollviewContents).GetComponent<ScoreEntry>();
                tmp.Rank = rank.ToString("N0");
                tmp.Name = score.data;
                tmp.Score = score.score.ToString("N0");
                tmp.name = "Score " + score.data + " " + tmp.Score + " " + tmp.Name;
                rank++;
            }
        }
        else
        {
            Debug.LogWarning(gameObject.name + ": Missing scoreEntryPrefab reference. No scores will be displayed.");
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (scoreboard != null)
        {
            scoreboard.OnRetrieveScoresEvent += OnRetrieveScoresEvent;
        }
        else
        {
            Debug.LogWarning(gameObject.name + ": Missing OnlineScoreBoard reference. No scores will be retrieved.");
            scoreboard = null;
        }

        if (scrollviewContents == null)
        {
            Debug.LogWarning(gameObject.name + ": Missing scrollviewContents reference. Did you foolishly delete this reference?");
            scrollviewContents = null;
        }

        if (scoreEntryPrefab == null){
            Debug.LogWarning(gameObject.name + ": Missing scoreEntryPrefab reference. Did you foolishly delete this reference?");
            scoreEntryPrefab = null;
        }
    }

    private void OnDestroy()
    {
        if (scoreboard != null)
        {
            scoreboard.OnRetrieveScoresEvent -= OnRetrieveScoresEvent;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
