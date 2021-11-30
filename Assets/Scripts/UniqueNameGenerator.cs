using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueNameGenerator : MonoBehaviour
{
    public string uniqueName;

    enum Letters { A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z}

    public bool _debugTest;

    // Start is called before the first frame update
    void Start()
    {
        SetSring();
    }

    static public string GenerateString(int numberOfCharacters)
    {
        string retVal = "";

        for (int i=0; i< numberOfCharacters; i++)
        {
            Letters letter = (Letters)Random.Range(0, 26);
            retVal += letter.ToString();
        }

        return retVal;
    }

    void SetSring()
    {
        uniqueName = "";
        Letters letter = (Letters)Random.Range(0, 26);
        uniqueName += letter.ToString();
        letter = (Letters)Random.Range(0, 26);
        uniqueName += letter.ToString();
        letter = (Letters)Random.Range(0, 26);
        uniqueName += letter.ToString();
        letter = (Letters)Random.Range(0, 26);
        uniqueName += letter.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (_debugTest)
        {
            _debugTest = false;
            SetSring();
        }
    }
}
