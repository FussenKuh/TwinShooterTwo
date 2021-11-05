using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimescaleAdjustDemo : MonoBehaviour
{
    public float timeScale = 0.5f;
    public float duration = 1f;

    public float currentDuration = 0f;

    public Text timescaleLabel;
    public Text durationLabel;
    public GameObject spawnedObjectPrefab;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnObject());
    }

    IEnumerator SpawnObject()
    {
        while (true)
        {
            GameObject tmp = Instantiate(spawnedObjectPrefab);
            tmp.transform.position = transform.position;
            tmp.transform.Rotate(new Vector3(0, 0, 45));
            Destroy(tmp, 4f);
            yield return new WaitForSeconds(2f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timescaleLabel.text = "TimeScale: " + Time.timeScale.ToString("N2");

        if (currentDuration > 0)
        {
            currentDuration -= Time.unscaledDeltaTime;
            durationLabel.text = "Duration: " + currentDuration.ToString("N2");
        }
        else
        {
            durationLabel.text = "Duration: 0.00";
        }

        // Left click to adjust timescale for a period of time
        if (Input.GetMouseButtonUp(0))
        {
            FKS.Utils.Time.AdjustTimeScale(timeScale, duration);
            currentDuration = duration;
        }
        // Right click to cancel the timescale adjustment
        if (Input.GetMouseButtonUp(1))
        {
            FKS.Utils.Time.RestoreTimeScale();
            currentDuration = 0;
        }

    }
}
