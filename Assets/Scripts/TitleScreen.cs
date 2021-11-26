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
        string bottomText = "Press <color=yellow>-space bar-</color> or <color=yellow>-a-</color> on your gamepad to join<size=80%>";
        string middleText = "";
        string upperRightText = "Level <color=yellow>" 
            + GameManager.Instance.highScore.ToString("D5") 
            + "</color>\nDamage <color=yellow>"
            + GameManager.Instance.highDamage.ToString("D5") + "</color>";


        StatsOverlay.Instance.UpdateBottomText(bottomText);
        StatsOverlay.Instance.UpdateMiddleText(middleText);
        StatsOverlay.Instance.UpdateUpperRightText(upperRightText);
    }
}
