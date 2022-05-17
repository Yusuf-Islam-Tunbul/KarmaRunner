using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAreaScript : MonoBehaviour
{
    [SerializeField] float speed;

    GameManager game_manager;    

    // Start is called before the first frame update
    void Start()
    {
        game_manager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (game_manager.game_state == GameState.Ending)
        {
            game_manager.MoveTowardsPlayer(gameObject, speed);
        }
    }
}
