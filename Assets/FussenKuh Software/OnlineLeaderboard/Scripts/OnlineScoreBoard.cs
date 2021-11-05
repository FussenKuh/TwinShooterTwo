using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;

namespace FKS
{

    public class OnlineScoreBoard : MonoBehaviour
    {
        [SerializeField]
        private string highscoreURL = "https://ldjam.fussenkuh.com/highscore.php";


        public event EventHandler<RetrieveScoresEventArgs> OnRetrieveScoresEvent;

        public class RetrieveScoresEventArgs : EventArgs
        {
            public List<Score> Scores { get; set; }
            public string ErrorString { get; set; }
            public bool Error { get; set; }
        }

        /// <summary>
        /// Retrieve scores from the online leaderboard. The OnRetrieveSocresEvent will contain the retrieved scores
        /// </summary>
        /// <param name="gameid">The leaderboard ID</param>
        public void RetrieveScores(int gameid)
        {
            StartCoroutine(DoRetrieveScores(gameid));
        }

        /// <summary>
        /// Post a score to the online leaderboard
        /// </summary>
        /// <param name="name">The player's name</param>
        /// <param name="score">The player's score</param>
        /// <param name="gameid">The leaderboard ID</param>
        public void PostScore(string name, int score, int gameid, string data="")
        {
            StartCoroutine(DoPostScores(name, score, gameid, data));
        }

        public void PostScore(Score score, int gameid)
        {
            PostScore(score.name, score.score, gameid, score.data);
        }

        IEnumerator DoRetrieveScores(int gameid)
        {
            RetrieveScoresEventArgs retVal = new RetrieveScoresEventArgs();

            retVal.Error = false;
            retVal.ErrorString = "";

            WWWForm form = new WWWForm();
            form.AddField("retrieve_leaderboard", "true");
            form.AddField("gameid", gameid);

            using (UnityWebRequest www = UnityWebRequest.Post(highscoreURL, form))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(www.error);
                    retVal.Error = true;
                    retVal.ErrorString = www.error;
                }
                else
                {
                    Debug.Log("Retrieved score data");
                    string contents = www.downloadHandler.text;

                    // If you've gotten here, then 'contents' should be a
                    // json array of Score (as defined by the struct at the bottom of this file).

                    string jsonResult = "";

                    // Read all the JSON into a string
                    using (StringReader reader = new StringReader(contents))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            jsonResult += line;
                        }
                    }

                    // Uncomment if you want to see the raw json that was retrieved
                    // Debug.Log(jsonResult);

                    jsonResult = JsonHelper.FixJson(jsonResult);

                    retVal.Scores = new List<Score>(JsonHelper.FromJson<Score>(jsonResult));

                    if (retVal.Scores.Count == 0)
                    {
                        Debug.Log("There is no score data for gameID " + gameid + ". Returning an empty List<Score>");
                    }
                }
            }

            OnRetrieveScoresEvent?.Invoke(this, retVal);
        }

        IEnumerator DoPostScores(string name, int score, int gameid, string data)
        {
            WWWForm form = new WWWForm();
            form.AddField("post_leaderboard", "true");
            form.AddField("name", name);
            form.AddField("score", score);
            form.AddField("gameid", gameid);
            form.AddField("data", data);

            using (UnityWebRequest www = UnityWebRequest.Post(highscoreURL, form))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Debug.Log("Successfully posted score!");
                }
            }
        }
    }

    [System.Serializable]
    public struct Score
    {
        public string name;
        public int score;
        public string data;

        public override string ToString()
        {
            return name + " : " + score.ToString();
        }

        public string ToStringWithData()
        {
            return name + " : " + score.ToString() + " -- " + data;
        }
    }
}


