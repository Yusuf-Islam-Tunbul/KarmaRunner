using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayButtonScript : MonoBehaviour
{
    [SerializeField] GameObject player;

    GameManager game_manager;

    // Start is called before the first frame update
    void Start()
    {
        game_manager = FindObjectOfType<GameManager>();
        GetComponent<Button>().onClick.AddListener(ButtonMethod);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ButtonMethod()
    {
        if (game_manager.game_state == GameState.Menu)
        {
            game_manager.game_state = GameState.Waiting;
            player.GetComponent<Rigidbody>().useGravity = true;
            GetComponentInParent<Animator>().SetBool("playing", true);
        }

        else
        {
            game_manager.game_state = GameState.Menu;
            SceneManager.LoadScene(0);
        }
    }
}
