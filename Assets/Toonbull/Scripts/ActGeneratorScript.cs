using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class ActGeneratorScript : MonoBehaviour
{
    [SerializeField] TextAsset acts_text;
    [SerializeField] float[] time_interval;
    [SerializeField] float[] scale_interval;
    [SerializeField] float[] offset_interval;
    [SerializeField] Material positive_color;
    [SerializeField] Material negative_color;
    [SerializeField] float speed;

    GameManager game_manager;

    Dictionary<string, int> acts = new Dictionary<string, int>();

    float offset;
    float scale;
    KeyValuePair<string, int> act;
    Material color;
    int movement;

    // Start is called before the first frame update
    void Start()
    {
        game_manager = FindObjectOfType<GameManager>();

        ReadActs();
        //acts.Add("murder", -1);
        //acts.Add("honesty", 1);

        StartCoroutine(GenerateAct());
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ReadActs()
    {
        foreach (string line in acts_text.ToString().Split('\n'))
        {
            acts.Add(line.Split(';')[0], System.Convert.ToInt32(line.Split(';')[1]));
        }
    }

    IEnumerator GenerateAct()
    {

        if(game_manager.game_state == GameState.Waiting || game_manager.game_state == GameState.Menu)
        {
            yield return new WaitForSeconds(1);
            StartCoroutine(GenerateAct());
        }

        else if (game_manager.game_state == GameState.Running)
        {
            SetActProperties();

            yield return new WaitForSeconds(Random.Range(time_interval[0], time_interval[1]));

            StartCoroutine(GenerateAct());
        }
    }

    void SetActProperties()
    {
        scale = Random.Range(scale_interval[0], scale_interval[1]);

        offset = Random.Range(offset_interval[0], offset_interval[1]);

        if ((Mathf.Abs(offset) + scale / 2) > Mathf.Abs(offset_interval[1]))
        {
            offset = (Mathf.Abs(offset_interval[1]) - scale / 2) * Mathf.Sign(offset);
        }

        act = acts.ElementAt(Random.Range(0, acts.Count));

        color = act.Value > 0 ? positive_color : negative_color;

        movement = Random.Range(0, 3);
        

        game_manager.CreateAct(gameObject, scale, offset, speed, act.Key, act.Value, color, movement);
    }
}
