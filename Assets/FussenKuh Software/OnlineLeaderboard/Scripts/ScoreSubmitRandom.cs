using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FKS;
using System.Globalization;
using TMPro;

public class ScoreSubmitRandom : MonoBehaviour
{
    [Tooltip("The scoreboard to interact with")]
    public OnlineScoreBoard scoreboard;

    [Tooltip("The Game ID to submit/retrieve")]
    public int gameId = 5;

    [Tooltip("The score information to submit to the online leaderboard")]
    public Score scoreToSubmit;

    public List<Score> scores = new List<Score>();

    public TextMeshProUGUI submitText;

    List<string> firstName = new List<string>()
    {
        "Michael",
        "Christopher",
        "Jessica",
        "Matthew",
        "Ashley",
        "Jennifer",
        "Joshua",
        "Amanda",
        "Daniel",
        "David",
        "James",
        "Robert",
        "John",
        "Joseph",
        "Andrew",
        "Ryan",
        "Brandon",
        "Jason",
        "Justin",
        "Sarah",
        "William",
        "Jonathan",
        "Stephanie",
        "Brian",
        "Nicole",
        "Nicholas",
        "Anthony",
        "Heather",
        "Eric",
        "Elizabeth",
        "Adam",
        "Megan",
        "Melissa",
        "Kevin",
        "Steven",
        "Thomas",
        "Timothy",
        "Christina",
        "Kyle",
        "Rachel",
        "Laura",
        "Lauren",
        "Amber",
        "Brittany",
        "Danielle",
        "Richard",
        "Kimberly",
        "Jeffrey",
        "Amy",
        "Crystal",
        "Michelle",
        "Tiffany",
        "Jeremy",
        "Benjamin",
        "Mark",
        "Emily",
        "Aaron",
        "Charles",
        "Rebecca",
        "Jacob"
    };

    List<string> lastNames = new List<string>()
    {
        "smith",
        "johnson",
        "williams",
        "jones",
        "brown",
        "davis",
        "miller",
        "wilson",
        "moore",
        "taylor",
        "anderson",
        "thomas",
        "jackson",
        "white",
        "harris",
        "martin",
        "thompson",
        "garcia",
        "martinez",
        "robinson",
        "clark",
        "rodriguez",
        "lewis",
        "lee",
        "walker",
        "hall",
        "allen",
        "young",
        "hernandez",
        "king",
        "wright",
        "lopez",
        "hill",
        "scott",
        "green",
        "adams",
        "baker",
        "gonzalez",
        "nelson",
        "carter",
        "mitchell",
        "perez",
        "roberts",
        "turner",
        "phillips",
        "campbell",
        "parker",
        "evans",
        "edwards",
        "collins",
        "stewart",
        "sanchez",
        "morris",
        "rogers",
        "reed",
        "cook",
        "morgan",
        "bell",
        "murphy",
        "bailey"
    };

    public void OnSubmitPressed()
    {
        if (scoreboard == null)
        {
            Debug.LogWarning(gameObject.name + ": The scoreboard object reference is not set. No score will be submitted");
            return;
        }

        // Creates a TextInfo based on the "en-US" culture.
        TextInfo myTI = new CultureInfo("en-US", false).TextInfo;

        Button btn = GetComponent<Button>();
        btn.interactable = false;

        scoreToSubmit.data = "";
        scoreToSubmit.score = Random.Range(1, 10000);
        scoreToSubmit.name = firstName[Random.Range(0, firstName.Count)] + " " + myTI.ToTitleCase(lastNames[Random.Range(0, lastNames.Count)]);
        scoreToSubmit.data = JsonUtility.ToJson(scoreToSubmit);

        scoreboard.PostScore(scoreToSubmit, gameId);

        submitText.SetText(scoreToSubmit.name + " submitted with a score of " + scoreToSubmit.score.ToString("N0"));

        StartCoroutine(DoRetrieveScore(gameId));
    }


    IEnumerator DoRetrieveScore(int gid)
    {
        yield return new WaitForSeconds(0.5f);
        scoreboard.RetrieveScores(gid);
        yield return new WaitForSeconds(0.25f);
        Button btn = GetComponent<Button>();
        btn.interactable = true;
    }


    private void OnRetrieveScoresEvent(object sender, OnlineScoreBoard.RetrieveScoresEventArgs e)
    {
        if (e.Error)
        {

            Debug.LogWarning("There was a problem retrieving scores. Errors: " + e.ErrorString);
        }
        else
        {
            scores.Clear();
            foreach (Score score in e.Scores)
            {
                scores.Add(JsonUtility.FromJson<Score>(score.data));
            }

        }
    }

    private void Awake()
    {
        if (scoreboard != null)
        {
            scoreboard.OnRetrieveScoresEvent += OnRetrieveScoresEvent;
        }
    }

    private void OnDestroy()
    {
        if (scoreboard != null)
        {
            scoreboard.OnRetrieveScoresEvent -= OnRetrieveScoresEvent;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i< lastNames.Count; i++)
        {

            lastNames[i] = lastNames[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
