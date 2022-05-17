using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActScript : MonoBehaviour
{
    public float speed;
    public string name_;
    public int value;
    public ActMovement movement;

    GameManager game_manager;

    // Start is called before the first frame update
    void Start()
    {
        game_manager = FindObjectOfType<GameManager>();

        game_manager.FillTexts(gameObject, name_, value.ToString("+#;-#;0"));
    }

    // Update is called once per frame
    void Update()
    {
        game_manager.MoveTowardsPlayer(gameObject, speed);
        game_manager.ActMovement_(gameObject, movement);
    }
}
