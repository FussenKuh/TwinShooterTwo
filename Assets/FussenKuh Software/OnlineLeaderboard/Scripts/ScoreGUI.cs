using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FKS;
using System.Linq;

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

        scores.Sort(delegate (Score x, Score y)
        {
            GameManager.OnlineScoreData xData = JsonUtility.FromJson<GameManager.OnlineScoreData>(x.data);
            GameManager.OnlineScoreData yData = JsonUtility.FromJson<GameManager.OnlineScoreData>(y.data);

            if (x.score > y.score)
            {
                return -1;
            }
            else if (x.score == y.score)
            {
                if (xData.elapsedTime <= yData.elapsedTime)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }

            }
            else
            {
                return 1;
            }

        });

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
            infoEntry.ElapsedTime = "Time";
            infoEntry.name = "Score " + infoEntry.Name + " " + infoEntry.Score;

            List<ScoreEntry> scoreEntries = new List<ScoreEntry>();

            // This shows calling the Sort(Comparison(T) overload using
            // an anonymous method for the Comparison delegate.
            // This method treats null as the lesser of two values.
            //parts.Sort(delegate (Part x, Part y)
            //{
            //    if (x.PartName == null && y.PartName == null) return 0;
            //    else if (x.PartName == null) return -1;
            //    else if (y.PartName == null) return 1;
            //    else return x.PartName.CompareTo(y.PartName);
            //});


            int rank = 1;
            foreach (Score score in scores)
            {
                if (rank > 10) { return; } // Only show the top 10 results

                GameManager.OnlineScoreData osd = JsonUtility.FromJson<GameManager.OnlineScoreData>(score.data);

                ScoreEntry tmp = Instantiate(scoreEntryPrefab, scrollviewContents).GetComponent<ScoreEntry>();
                tmp.Rank = rank.ToString("N0");
                tmp.Name = osd.name;
                tmp.Score = (score.score - 1).ToString("N0");
                tmp.ElapsedTime = osd.elapsedTime.ToString("N2");
                tmp.name = "Score " + osd.name + " " + tmp.Score + " " + tmp.Name;
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
