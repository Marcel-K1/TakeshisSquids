using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System.Linq;

//Made by Marcel

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    public enum EWallColor
    {
        Red,
        Green
    }

    [SerializeField]
    private Transform startPos = null;

    [SerializeField]
    private GameObject[] wallArray = new GameObject[4];

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private TimerController timerController;

    [SerializeField]
    private SO_GameMode isSingleplay;
    public SO_GameMode IsSingleplay { get => isSingleplay; set => isSingleplay = value; }

    private Dictionary<string, int> playerScores = new Dictionary<string, int>();

    private float nextSpawnTime = 0f;

    private EWallColor currentWallColor = EWallColor.Green;
    public EWallColor CurrentWallColor { get => currentWallColor; set => currentWallColor = value; }

    private string winner;
    public string Winner { get => winner; set => winner = value; }
    
    private bool isPaused = false;
    public bool IsPaused { get => isPaused; set => isPaused = value; }

    public static bool IsInputEnabled = true;


    private static event System.Action changeColorEvent = () => { };

    public event System.Action ChangeColorEvent { add { changeColorEvent += value; } remove { changeColorEvent -= value; } }
    
    
    private static event System.Action timeUpEvent = () => { };

    public event System.Action TimeUpEvent { add { timeUpEvent += value; } remove { timeUpEvent -= value; } }
    
    
    private static event System.Action reloadEvent = () => { };

    public event System.Action ReloadEvent { add { reloadEvent += value; } remove { reloadEvent -= value; } }

   
    private static event System.Action playerIsDeadEvent = () => { };

    public event System.Action PlayerIsDeadEvent { add { playerIsDeadEvent += value; } remove { playerIsDeadEvent -= value; } }


    public void OnEnable()
    {
        changeColorEvent += OnChangeColor;
        timeUpEvent += OnTimeUp;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

    }

    private void Start()
    {

        //Checking current game mode
        int randomX = Random.Range(-5, 5);
        int randomZ = Random.Range(-3, 3);
        if (IsSingleplay.IsSingleplay)
        {
            Instantiate(player, startPos.position, Quaternion.identity);
        }
        else if (!IsSingleplay.IsSingleplay)
        {
            PhotonNetwork.Instantiate("Player2", new Vector3(startPos.position.x + randomX, startPos.position.y, startPos.position.z + randomZ), Quaternion.identity, 0);
        }


        //Setting wall color on begin of game
        foreach (GameObject wall in wallArray)
        {
            wall.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        }

        //Starting timer
        timerController.BeginTimer();

        //Setting up the wall change interval
        nextSpawnTime = Time.time + 15.0f;
    }

    private void Update()
    {
        //Quit Condition
        if (Input.GetKeyDown(KeyCode.Escape))
        {
//#if UNITY_EDITOR
//            UnityEditor.EditorApplication.isPlaying = false;
//#else
//            Application.Quit();
//#endif
            ReloadAndResetToMainMenu();
            //SceneManager.LoadScene(0);
        }

        //Wall Change Check
        if (Time.time > nextSpawnTime)
        {
            changeColorEvent.Invoke();
            //Increment nextSpawnTime for interval
            switch (CurrentWallColor)
            {
                case EWallColor.Red:
                    nextSpawnTime += 5.0f;
                    break;
                case EWallColor.Green:
                    nextSpawnTime += 15.0f;
                    break;
                default:
                    break;
            }

        }

        //Invoke timeUpEvent
        if (timerController.RoundTime <= 0f)
        {
            isPaused = true;
            Pause(isPaused);
            timeUpEvent.Invoke();
        }
    }

    public void OnDisable()
    {
        changeColorEvent -= OnChangeColor;
        timeUpEvent -= OnTimeUp;
    }

    //Events
    private void OnTimeUp()
    {
        if (!IsSingleplay.IsSingleplay)
        {
            InitDictionary();
            Winner = SortDictionary(playerScores);
        }
        timerController.TimerGoing = false;
        StartCoroutine(WaitForSeconds());
    }
    private void OnChangeColor()
    {
        switch (CurrentWallColor)
        {
            case EWallColor.Red:
                foreach (GameObject wall in wallArray)
                {
                    if (wall.gameObject.GetComponent<MeshRenderer>().material.color == Color.red)
                    {
                        wall.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
                        CurrentWallColor = EWallColor.Green;
                        IsInputEnabled = true;
                    }
                }
                break;
            case EWallColor.Green:
                foreach (GameObject wall in wallArray)
                {
                    if (wall.gameObject.GetComponent<MeshRenderer>().material.color == Color.green)
                    {
                        wall.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
                        CurrentWallColor = EWallColor.Red;
                        IsInputEnabled = false;
                    }
                }
                break;
            default:
                break;
        }
    }

    //Methods
    private void InitDictionary()
    {
        //Populating dictionary with players and their scores at the beginning of the game
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerScores.Add(player.NickName, player.GetScore());
        }
    }
    private string SortDictionary(Dictionary<string, int> playerScore)
    {
        var maxValue = playerScore.Values.Max();
        var keyToMaxValue = "";

        foreach (var player in playerScore)
        {
            if (player.Value == maxValue)
            {
                keyToMaxValue = player.Key;
                return keyToMaxValue;
            }
            else
                return "No Winner";
            
        }
        return keyToMaxValue;

    }
    public void Pause(bool _paused)
    {
        if (_paused)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }
    private IEnumerator WaitForSeconds()
    {
        yield return new WaitForSecondsRealtime(5);
        ReloadAndResetToMainMenu();
    }
    public void ReloadAndResetToMainMenu()
    {
        reloadEvent.Invoke();

        SceneManager.LoadScene(0);

        isPaused = false;
        Pause(isPaused);
        
        DestroyThyself();
    }
    public void DestroyThyself() 
    { 
        Destroy(gameObject); 
        instance = null; 
    }
}
