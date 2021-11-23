using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    [SerializeField]
    CameraSystem cs;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.Initialize();

        PlayerManager.Instance.SetPlayerJoin(true);

        cs = GameObject.Find("Main Camera System").GetComponent<CameraSystem>();
        cs.Reset();

        UpdateStatsOverlay();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateStatsOverlay()
    {
        string bottomText = "Press <color=yellow>-space bar-</color> or <color=yellow>-a-</color> on your gamepad to join<size=80%>";
        string middleText = "";
        string upperRightText =
            "<size=80%> The world is gray...You're a <color=orange>glitch</color>, a <color=red>bug</color>.\n" +
            "You're <color=blue>colorful</color> and it scares them. <color=yellow>you</color>...</size>\n" +
            "<size=130%> must be stopped.</size>";
        string upperLeftText = "<size=70%>---Records---\nLevel <color=yellow>" 
            + GameManager.Instance.highScore.ToString("D5") 
            + "</color>\nDamage <color=yellow>"
            + GameManager.Instance.highDamage.ToString("D5") + "</color></size>";


        StatsOverlay.Instance.UpdateBottomText(bottomText);
        StatsOverlay.Instance.UpdateMiddleText(middleText);
        StatsOverlay.Instance.UpdateUpperLeftText(upperLeftText);
        StatsOverlay.Instance.UpdateUpperRightText(upperRightText);
    }
}
