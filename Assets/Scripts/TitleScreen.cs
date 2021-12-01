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
        //cs.Reset();
        StartCoroutine(ResetCamera());

        UpdateStatsOverlay();
    }

    IEnumerator ResetCamera()
    {
        yield return new WaitForSeconds(0.1f);
        cs.Reset();
    }


    // Update is called once per frame
    void Update()
    {

    }

    void UpdateStatsOverlay()
    {
        int level = Mathf.Max(0, (GameManager.Instance.highScore - 1));
        string byLineText = "GitHub GameOff 2021 - Chris Kraszewski v" + Application.version;
        string bottomText = "Press <color=yellow>-space bar-</color> or <color=yellow>-a-</color> on your gamepad to join<size=80%>";
        string middleText = "";
        string upperRightText = 
            "Name <color=yellow>" + GameManager.Instance.uniqueName +
            "</color>\nLevel <color=yellow>"
            + level.ToString("D3")
            + "</color>\n<size=45%>highest level cleared</size>";


        StatsOverlay.Instance.UpdateBottomText(bottomText);
        StatsOverlay.Instance.UpdateMiddleText(middleText);
        StatsOverlay.Instance.UpdateUpperRightText(upperRightText);
        StatsOverlay.Instance.UpdateByLineText(byLineText);
    }
}
