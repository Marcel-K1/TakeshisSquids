using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Made by Marcel

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private static NetworkManager instance;
    public static NetworkManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<NetworkManager>();
            }
            return instance;
        }
    }


    [Header("General References")]
    [SerializeField]
    private GameObject loadingScreen;
    [SerializeField]
    private TMP_Text userNameOutput;
    [SerializeField]
    private TMP_InputField userNameInput;
    [SerializeField]
    const string playerNamePrefKey = "PlayerName";
    public static string PlayerNamePrefKey => playerNamePrefKey;

    [Header("Quickplay References")]
    [SerializeField]
    private GameObject playRandomButton;
    [SerializeField]
    private TextMeshProUGUI playerRandomRoomCount;
    [SerializeField]
    private TextMeshProUGUI randomRoomNameText;

    [Header("Quickplay Room Options")]
    [SerializeField]
    private int randomRoomSize = 4;
    [SerializeField]
    private bool randomRoomIsVisible = true;
    [SerializeField]
    private bool randomRoomIsOpen = true;
    [SerializeField]
    private string randomRoomName = "Default Room";

    [Header("Play References")]
    [SerializeField]
    private GameObject playRoomButton;
    [SerializeField]
    private TextMeshProUGUI roomNameText;
    [SerializeField]
    private TextMeshProUGUI roomSizeText;
    [SerializeField]
    private TextMeshProUGUI playerRoomCount;
    [SerializeField]
    private Transform roomListContent;
    [SerializeField]
    private RoomListing roomListing;

    [Header("Play Room Options")]
    [SerializeField]
    private Slider roomSizeInput;
    [SerializeField]
    private Toggle roomIsVisibleInput;
    [SerializeField]
    private TMP_InputField roomNameInput;

    [SerializeField]
    private ChatManager chatManager;

    [SerializeField]
    private SO_GameMode isSingleplay;

    private static event System.Action<Player> newMasterEvent = (newMasterPlayer) => { };

    public event System.Action<Player> NewMasterEvent { add { newMasterEvent += value; } remove { newMasterEvent -= value; } }

    private List<RoomListing> listings = new List<RoomListing>();

    private Dictionary<string, int> playerScores = new Dictionary<string, int>();
    
    void Awake()
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

        //Setting cursor visible
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //UI Stuff
        loadingScreen.SetActive(true);
        userNameInput.text = string.Empty;
        string defaultName = string.Empty;

        //AutoConnect und SyncScene
        PhotonNetwork.AutomaticallySyncScene = true;
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else if (PhotonNetwork.IsConnected)
        {
            loadingScreen?.SetActive(false);
        }

    }

    public override void OnEnable()
    {
        base.OnEnable();
        roomSizeInput.onValueChanged.AddListener(delegate { roomSizeText.text = roomSizeInput.value.ToString(); });
        newMasterEvent += OnMasterClientSwitched;
    }

    private void Update()
    {
        //Update the player count
        if (PhotonNetwork.InRoom)
        {
            playerRandomRoomCount.text = $"Player Count: {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
            playerRoomCount.text = $"Player Count: {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
        }
        
    }

    public override void OnDisable()
    {
        base.OnDisable();
        newMasterEvent -= OnMasterClientSwitched;
    }

    #region Button Methods

    public void Singleplay()
    {
        isSingleplay.IsSingleplay = true;
        SceneManager.LoadScene(1);
    }
    public void Quickplay()
    {
        isSingleplay.IsSingleplay = false;
        Debug.Log("Try to join random room.");

        PhotonNetwork.JoinRandomRoom();

        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

    }
    public void Play()
    {
        isSingleplay.IsSingleplay = false;
        Debug.Log("Try to join default lobby.");

        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

    }
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)roomSizeInput.value;
        roomOptions.IsVisible = roomIsVisibleInput;
        roomOptions.IsOpen = true;
        
            
        Debug.Log("Trying to create new room.");
        PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions);
    }
    public void LeaveRoom()
    {
        if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
        {
            return;
        }
        
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LeaveRoom();
    }
    public void PlayRandomRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            SceneManager.LoadScene(1);
        }
    }
    public void PlayRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            SceneManager.LoadScene(1);
        }
    }
    public void SetUserName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Debug.Log("Player Name is null or empty");
            return;
        }

        PhotonNetwork.NickName = value;

        userNameOutput.text = string.Format($"Username: {PhotonNetwork.NickName}");

        PlayerPrefs.SetString(PlayerNamePrefKey, value);
    }
    public void JoinRoom()
   {
        foreach (RoomListing listing in listings)
        {
            if (listing.Toggle.isOn)
            {
                if (listing.RoomInfo.IsOpen)
                {
                    PhotonNetwork.JoinRoom(listing.RoomInfo.Name);
                }
            }
            else
            {
                Debug.Log("You haven't chosen a room! Please select a room to join");
            }
        }
        
   }

    #endregion

    #region Photon Callbacks
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Joining random room failed cause: " + message);

        RoomOptions randomRoomOptions = new RoomOptions();
        randomRoomOptions.MaxPlayers = (byte)randomRoomSize;
        randomRoomOptions.IsVisible = randomRoomIsVisible;
        randomRoomOptions.IsOpen = randomRoomIsOpen;

        Debug.Log("Trying to create new room.");
        PhotonNetwork.CreateRoom(randomRoomName, randomRoomOptions);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"Created {PhotonNetwork.CurrentRoom} room successfully");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Player has joined {PhotonNetwork.CurrentRoom.Name}.");

        playRandomButton.SetActive(PhotonNetwork.IsMasterClient);
        playRoomButton.SetActive(PhotonNetwork.IsMasterClient);
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        randomRoomNameText.text = PhotonNetwork.CurrentRoom.Name;

        playerRandomRoomCount.text = $"Player Count: {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";

        playerRoomCount.text = $"Player Count: {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Player has entered the default lobby.");
    }

    public override void OnConnected()
    {
        Debug.Log("You successfully connected to the Photon Server.");

        loadingScreen?.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} has entered the room.");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playRandomButton.SetActive(PhotonNetwork.IsMasterClient);
        playRoomButton.SetActive(PhotonNetwork.IsMasterClient); 
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {

        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                int index = listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                if (index != -1)
                {
                    Destroy(listings[index].gameObject);
                    listings.RemoveAt(index);
                }
            }
            else if (!info.IsVisible)
            {
                int index = listings.FindIndex(x => x.RoomInfo.Name == info.Name);

                Destroy(listings[index].gameObject);
                listings.RemoveAt(index);

            }
            else
            {
                if (info.PlayerCount <= info.MaxPlayers)
                {
                    RoomListing listing = Instantiate(roomListing, roomListContent);
                    if (listing != null)
                    {

                        listing.SetRoomInfo(info);
                        listings.Add(listing);
                    }
                }
            }
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        chatManager.MessagePanel.text += string.Format($"{newMasterClient.NickName} is now MasterClient. \n");
    }

    #endregion

    #region Methods

    public void DestroyThyself() 
    { 
        Destroy(gameObject); 
        instance = null; 
    }

    #endregion

}
