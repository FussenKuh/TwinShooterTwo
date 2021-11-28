using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardTrigger : MonoBehaviour
{
    [SerializeField]
    CanvasGroup _canvasGroup;

    [SerializeField]
    FKS.OnlineScoreBoard _scoreboard;

    [SerializeField]
    int _onlineGameID = 5;

    [SerializeField]
    float _fadeTime = 0.5f;

    [SerializeField]
    int _playerCount = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) { _playerCount++; }
        if (_playerCount > 0)
        {
            StartCoroutine(FadeCanvasIn());
            _scoreboard.RetrieveScores(_onlineGameID);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) { _playerCount--; }
        if (_playerCount == 0)
        {
            StartCoroutine(FadeCanvasOut());
        }
    }



    IEnumerator FadeCanvasOut()
    {
        while (_canvasGroup.alpha > 0)
        {
            _canvasGroup.alpha = _canvasGroup.alpha - (Time.unscaledDeltaTime / _fadeTime);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FadeCanvasIn()
    {
        while (_canvasGroup.alpha < 1)
        {
            _canvasGroup.alpha = _canvasGroup.alpha + (Time.unscaledDeltaTime / _fadeTime);
            yield return new WaitForEndOfFrame();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        _onlineGameID = GameManager.Instance.onlineGameID;
        _scoreboard.RetrieveScores(_onlineGameID);

        _canvasGroup.GetComponent<Canvas>().worldCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
