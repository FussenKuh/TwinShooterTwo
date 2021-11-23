using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoalPoint : MonoBehaviour
{
    public TextMeshPro textMesh;

    [SerializeField]
    public float requiredDamage = 200;

    [SerializeField]
    int numPlayersAtGoal = 0;

    string useText = "A";

    private void Awake()
    {
        Messenger<float>.AddListener("EnemyDamage", OnEnemyDamage);
    }

    private void OnDestroy()
    {
        Messenger<float>.RemoveListener("EnemyDamage", OnEnemyDamage);
    }

    void OnEnemyDamage(float damage)
    {
        requiredDamage -= damage;
        requiredDamage = Mathf.Max(0, requiredDamage);

        UpdateText();
    }

    void UpdateText()
    {
        if (textMesh != null)
        {
            if (numPlayersAtGoal > 0 && requiredDamage <= 0)
            {
                textMesh.SetText("<size=50%>press <color=yellow>-" + useText + "-</color> to get to the next level</size>");
            }
            else
            {
                textMesh.SetText("<size=50%>Do <color=yellow>" + (int)requiredDamage + "</color> more damage</size>");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            numPlayersAtGoal++;

            if (collision.GetComponent<Player>().UsingKeyboardAndMouse)
            {
                useText = "Space";
            }
            else
            {
                useText = "A";
            }

            UpdateText();

            Messenger<bool, Player>.Broadcast("PlayerAtGoal", true, collision.GetComponent<Player>(), MessengerMode.DONT_REQUIRE_LISTENER);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            numPlayersAtGoal--;

            UpdateText();

            Messenger<bool, Player>.Broadcast("PlayerAtGoal", false, collision.GetComponent<Player>(), MessengerMode.DONT_REQUIRE_LISTENER);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        requiredDamage = GameManager.Instance.DamageToClearLevel;
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
