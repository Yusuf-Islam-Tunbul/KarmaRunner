using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    [SerializeField] float speed;
    GameManager game_manager;

    float size;

    // Start is called before the first frame update
    void Start()
    {
        game_manager = FindObjectOfType<GameManager>();
        size = GetComponent<BoxCollider>().bounds.size.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (game_manager.game_state == GameState.Running)
        {
            game_manager.MoveTowardsPlayer(gameObject, speed);
            game_manager.TeleportPlatform(gameObject, transform.position.z, size);
        }

        else if(game_manager.game_state == GameState.Ending)
        {
            game_manager.MoveTowardsPlayer(gameObject, speed);
        }
    }
}
