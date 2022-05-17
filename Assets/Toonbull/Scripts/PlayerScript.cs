using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] GameObject end_point;
    [SerializeField] GameObject menu_button;
    [SerializeField] float[] movement_boundaries;
    [SerializeField] float speed;

    public PlayerState player_state;

    GameManager game_manager;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        game_manager = FindObjectOfType<GameManager>();
        player_state = PlayerState.FreeMovement;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (game_manager.game_state == GameState.Waiting)
        {
            game_manager.StartRunning(animator);
        }

        else if(game_manager.game_state == GameState.Running)
        {
            game_manager.MoveWithTouch(transform, movement_boundaries, speed);
            game_manager.KeepInsideBoundaries(transform, movement_boundaries);
        }

        else if (game_manager.game_state == GameState.Ending)
        {
            if (player_state == PlayerState.FreeMovement)
            {
                game_manager.MoveWithTouch(transform, movement_boundaries, speed);
                game_manager.KeepInsideBoundaries(transform, movement_boundaries);
            }

            else if (player_state == PlayerState.EndingMovement)
            {
                game_manager.MoveTowardsCenter(gameObject);
                animator.SetInteger("outcome",game_manager.CheckArrival(gameObject, end_point,0.1f));
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            animator.SetBool("falling", false);
            menu_button.SetActive(true);
        }

        if (collision.gameObject.tag == "Act")
        {
            game_manager.PerformAct(collision.gameObject);
        }

        if (collision.gameObject.tag == "EndEntrance")
        {
            player_state = PlayerState.EndingMovement;
        }
    }
}
