using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FKS;

public class DamagePopupTest : MonoBehaviour
{

    public float spawnRate = 2f;
    public float timer = 0;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = spawnRate;
            
            MessagePopup.Create(new Vector3(Random.Range(-5,5), Random.Range(-5,5), 0), Random.Range(5,10).ToString(), FKS.Utils.UtilsClass.TestChance(30));
        }
    }
}
