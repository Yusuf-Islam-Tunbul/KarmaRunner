using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System.Runtime.Serialization.Formatters.Binary;

public enum GameState
{
    Menu, Waiting, Running, Ending, Won, Lost, Draw
}

public enum PlayerState
{
    FreeMovement, EndingMovement, Stopped
}

public enum ActMovement
{
    Idle, PingPong, Rotate
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameState game_state;
    
    [SerializeField] GameObject player_cylinder;
    [SerializeField] GameObject player_fire;
    [SerializeField] GameObject act_prefab;
    [SerializeField] GameObject karma_object_prefab;
    [SerializeField] GameObject karma_points_text;
    [SerializeField] GameObject best_points_text;
    [SerializeField] GameObject end_area;
    [SerializeField] GameObject end_screen;
    [SerializeField] GameObject won_text;
    [SerializeField] GameObject lost_text;
    [SerializeField] GameObject draw_text;
    [SerializeField] float targeted_game_time;
    [SerializeField] float road_width;

    BinaryFormatter converter;
    string score_directory;
    int number_of_platforms;
    float[] camera_sizes;
    int karma_points;
    float game_time;
    bool enlarging;

    GameObject created_act;
    Vector3 current_act_scale;

    private void Awake()
    {
        AssignInstance();
    }

    // Start is called before the first frame update
    void Start()
    {
        converter = new BinaryFormatter();
        score_directory = Application.persistentDataPath + @"\best.bin";

        best_points_text.GetComponent<TextMeshProUGUI>().text = "BEST: " + LoadScore().ToString();
        number_of_platforms = GameObject.FindGameObjectsWithTag("Platform").Length;
        karma_points = 0;
        UpdateKarmaPointsText();
        game_time = 0;

        SetCameraSizes(ref camera_sizes);
        //DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        CheckTime();
        Enlarge(player_cylinder.transform, 1.2f, 1.2f);
    }

    private void FixedUpdate()
    {
        
    }

    void AssignInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MoveTowardsPlayer(GameObject game_object, float speed)
    {
        game_object.transform.position += Vector3.back * speed * Time.deltaTime;
    }

    public void TeleportPlatform(GameObject platform, float position, float size)
    {
        if (position <= -size)
        {
            platform.transform.position += Vector3.forward * size * number_of_platforms;
        }
    }

    public void CreateAct(GameObject generator,float scale, float offset, float speed, string name_, int value, Material color, int movement)
    {
        created_act = Instantiate(act_prefab);

        current_act_scale = created_act.transform.localScale;
        current_act_scale.x = scale;
        created_act.transform.localScale = current_act_scale;


        created_act.transform.position = generator.transform.position + Vector3.right* offset;

        created_act.transform.rotation = Quaternion.Euler(new Vector3(25, 0, 0));

        created_act.GetComponent<ActScript>().speed = speed;
        created_act.GetComponent<ActScript>().name_ = name_;
        created_act.GetComponent<ActScript>().value = value;
        created_act.GetComponent<ActScript>().movement = (ActMovement)movement;
        

        created_act.GetComponent<Renderer>().material=color;
    }

    public void FillTexts(GameObject parent, params string[] texts)
    {
        TextMeshProUGUI[] text_objects = parent.GetComponentsInChildren<TextMeshProUGUI>();

        int index = 0;

        if (texts.Length > text_objects.Length)
        {
            foreach(TextMeshProUGUI text_object in text_objects)
            {
                text_object.text = texts[index];
                index++;
            }
        }

        else
        {
            foreach (string text in texts)
            {
                text_objects[index].text = text;
                index++;
            }
        }
    }

    public void StartRunning(Animator animator)
    {
        if (!animator.GetBool("falling") && animator.GetCurrentAnimatorStateInfo(0).IsName("Running"))
        {
            game_state = GameState.Running;
        }
    }

    public void MoveWithTouch(Transform object_transform, float[] boundaries, float speed)
    {
        if(Input.touchCount>0 && object_transform.position.x>=boundaries[0] && object_transform.position.x <= boundaries[1])
        {
            object_transform.position += Vector3.right * (Input.touches[0].deltaPosition.x * camera_sizes[0] / Screen.width) * speed;
        }
    }

    void SetCameraSizes(ref float[] camera_sizes)
    {
        float aspect_ratio = Screen.height / Screen.width;

        camera_sizes = new float[2];
        camera_sizes[1] = Camera.main.orthographicSize*2;
        camera_sizes[0] = camera_sizes[1] / aspect_ratio;
    }

    public void KeepInsideBoundaries(Transform object_transform, float[] boundaries)
    {
        if (object_transform.position.x < boundaries[0])
        {
            object_transform.position = new Vector3(boundaries[0], object_transform.position.y, object_transform.position.z);
        }

        else if(object_transform.position.x > boundaries[1])
        {
            object_transform.position = new Vector3(boundaries[1], object_transform.position.y, object_transform.position.z);
        }
    }

    public void PerformAct(GameObject act)
    {
        karma_points += act.GetComponent<ActScript>().value;
        UpdateKarmaPointsText();

        GameObject karma_object = Instantiate(karma_object_prefab, act.transform.position, Camera.main.transform.rotation);
        karma_object.GetComponentsInChildren<TextMeshProUGUI>()[0].text = act.GetComponent<ActScript>().value.ToString("+#;-#;0");
        karma_object.GetComponentsInChildren<TextMeshProUGUI>()[0].color = act.GetComponent<Renderer>().material.color;

        Destroy(act);
    }

    public void MoveUpwards(Transform object_transform, float speed)
    {
        object_transform.position += Vector3.up * Time.deltaTime * speed;
    }

    void UpdateKarmaPointsText()
    {
        karma_points_text.GetComponent<TextMeshProUGUI>().text = "KARMA: " + karma_points;

        switch (karma_points)
        {
            case var value when karma_points < 0:
                karma_points_text.GetComponent<TextMeshProUGUI>().color = Color.red;
                break;

            case var value when karma_points == 0:
                karma_points_text.GetComponent<TextMeshProUGUI>().color = Color.white;
                break;

            case var value when karma_points > 0:
                karma_points_text.GetComponent<TextMeshProUGUI>().color = Color.green;
                break;

            default:
                break;
        }
    }

    void CheckTime()
    {
        if (game_state == GameState.Running)
        {
            game_time += Time.deltaTime;

            if (game_time >= targeted_game_time)
            {
                game_state = GameState.Ending;
            }
        }
    }

    public void MoveTowardsCenter(GameObject game_object)
    {
        if (Mathf.Abs(game_object.transform.position.x) > 0.1)
        {
            game_object.transform.position = new Vector3(Mathf.Lerp(game_object.transform.position.x, 0, 0.02f), game_object.transform.position.y, game_object.transform.position.z);
        }

        //else
        //{
        //    game_object.GetComponent<PlayerScript>().player_state = PlayerState.Stopped;
        //    //Judgement();
        //}
    }

    public int CheckArrival(GameObject game_object, GameObject destination, float error_margin)
    {

        if (Mathf.Abs(game_object.transform.position.z-destination.transform.position.z)<error_margin)
        {
            game_state = Judgement();

            switch (game_state)
            {
                case GameState.Lost:
                    return -1;
                case GameState.Draw:
                    return 0;
                case GameState.Won:
                    return 1;
                default:
                    return 0;
            }
        }

        return -2;
    }

    GameState Judgement()
    {
        //if (System.Convert.ToInt32(File.ReadAllText(Directory.GetCurrentDirectory() + @"\best.txt")) < karma_points)
        //{
        //    File.WriteAllText(Directory.GetCurrentDirectory() + @"\best.txt", karma_points.ToString());
        //    best_points_text.GetComponent<TextMeshProUGUI>().text = "BEST: " + karma_points.ToString();
        //}

        if (LoadScore() < karma_points)
        {
            SaveScore(karma_points);
        }

        end_screen.SetActive(true);

        switch (karma_points)
        {
            case var value when karma_points < 0:
                HellEvent();
                return GameState.Lost;

            case var value when karma_points == 0:
                LimboEvent();
                return GameState.Draw;

            case var value when karma_points > 0:
                HeavenEvent();
                return GameState.Won;

            default:
                return game_state;
        }
    }

    void HeavenEvent()
    {
        won_text.SetActive(true);
        player_cylinder.SetActive(true);
        enlarging = true;
    }

    void LimboEvent()
    {
        draw_text.SetActive(true);
        end_area.AddComponent<Rigidbody>();
        end_area.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        end_area.GetComponent<Rigidbody>().mass = 1000;
    }

    void HellEvent()
    {
        lost_text.SetActive(true);
        player_fire.SetActive(true);
    }

    void Enlarge(Transform game_object, float increment, float target_size)
    {
        if (enlarging)
        {
            game_object.localScale += new Vector3(increment, 0, increment)*Time.deltaTime;

            if(game_object.localScale.x > target_size)
            {
                enlarging = false;
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit(0);
    }

    public void ActMovement_(GameObject act,ActMovement movement)
    {
        if (movement == ActMovement.PingPong)
        {
            PingPongMovement(act.transform);
        }

        else if(movement == ActMovement.Rotate)
        {
            RotateMovement(act.transform);
        }
    }
    
    void PingPongMovement(Transform act)
    {
        act.position = new Vector3(Mathf.PingPong(Time.time, road_width)-road_width/2,act.position.y, act.position.z);
    }

    void RotateMovement(Transform act)
    {
        act.rotation = Quaternion.Euler(act.rotation.eulerAngles.x, act.rotation.eulerAngles.y + 1, act.rotation.eulerAngles.z);
    }

    int LoadScore()
    {
        int value;

        try
        {
            FileStream file_stream = new FileStream(score_directory, FileMode.Open);
            value = (int)converter.Deserialize(file_stream);
            file_stream.Close();
        }

        catch
        {
            value= 0;
        }

        return value;
        
    }

    void SaveScore(int score)
    {
        FileStream file_stream = new FileStream(score_directory, FileMode.OpenOrCreate);
        converter.Serialize(file_stream, score);
        file_stream.Close();
    }
}
