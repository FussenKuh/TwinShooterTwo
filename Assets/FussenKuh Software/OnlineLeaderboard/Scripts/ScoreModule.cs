using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FKS;

public class ScoreModule : MonoBehaviour
{
    public List<Score> scores = new List<Score>();

    [Tooltip("The scoreboard to interact with")]
    public OnlineScoreBoard scoreboard;

    [Tooltip("The Game ID to submit/retrieve")]
    public int gameId = 1;


    [Space(25)]
    [Header("Retrieve Score Info")]
    [Tooltip("Set to 'True' to retrieve scores. It will reset to 'False' when the retrieve scores request is sent")]
    public bool retrieveScores = false;

    [Header("Post Score Info")]
    [Tooltip("The Score to submit")]
    public int score  = 0;
    [Tooltip("The Name to submit")]
    public string scoreName = "Chris";
    [Tooltip("Optional json data to send with the high score entry")]
    public string scoreData;
    [Tooltip("Set to 'True' to submit a score. It will reset to 'False' when the score is submitted")]
    public bool submitScore = false;

    [Space(25)]
    [Tooltip("Enable debug console printouts")]
    public bool DEBUG = false;

    // Start is called before the first frame update
    void Start()
    {
        if (scoreboard != null)
        {
            scoreboard.OnRetrieveScoresEvent += OnRetrieveScoresEvent;
            scoreboard.RetrieveScores(gameId);
        }
    }

    private void OnDestroy()
    {
        if (scoreboard != null)
        {
            scoreboard.OnRetrieveScoresEvent -= OnRetrieveScoresEvent;
        }
    }

    private void OnRetrieveScoresEvent(object sender, OnlineScoreBoard.RetrieveScoresEventArgs e)
    {
        if (e.Error)
        {

            Debug.LogWarning("There was a problem retrieving scores. Errors: " + e.ErrorString);
        }
        else
        {
            if (DEBUG)
            {
                scores = e.Scores;
                foreach (Score score in e.Scores)
                {
                    Debug.Log(score.ToString());
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (scoreboard != null)
        {

            if (submitScore)
            {
                submitScore = false;
                scoreboard.PostScore(scoreName, score, gameId, scoreData);
            }
            if (retrieveScores)
            {
                retrieveScores = false;
                scoreboard.RetrieveScores(gameId);
            }
        }
    }
}
