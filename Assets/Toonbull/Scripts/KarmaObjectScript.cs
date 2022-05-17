using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KarmaObjectScript : MonoBehaviour
{
    [SerializeField] float speed;

    [HideInInspector] public int value;

    GameManager game_manager;

    // Start is called before the first frame update
    void Start()
    {
        game_manager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        game_manager.MoveUpwards(transform, speed);
    }
}
