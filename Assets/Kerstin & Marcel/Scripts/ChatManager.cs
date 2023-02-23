using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;
using System;
using Photon.Realtime;

//Made by Kerstin

public class ChatManager : MonoBehaviour, IChatClientListener
{

    [SerializeField] ChatClient chatClient;

    [SerializeField] ScrollRect scrollRect;

    [SerializeField] Text messagePanel;

    [SerializeField] InputField messageInput;

    string channelName = "ChatChannel";

    public Text MessagePanel { get => messagePanel; set => messagePanel = value; }

    private void Awake()
    {
        chatClient = new ChatClient(this);
    }

    private void OnEnable()
    {
        chatClient.ChatRegion = "EU";
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, 
                           PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues(PhotonNetwork.NickName));
        //channelName = PhotonNetwork.CurrentRoom.Name;
    }

    private void OnDisable()
    {
        //messagePanel.text = "";
        //chatClient.Unsubscribe(new string[] { channelName });
    }



    private void Update()
    {
        chatClient?.Service();
    }

    public void SendMessageToChat()
    {
        chatClient.PublishMessage(channelName, messageInput.text);
        messageInput.text = string.Empty;
    }

    public void OnConnected()
    {
        chatClient?.Subscribe(new string[] { channelName });
    }

    public void OnGetMessages(string channelname, string[] senders, object[] messages)
    {
        string message = "";
        
        for (int i = 0; i < senders.Length; i++)
        {
            message = string.Format($"{senders[i]}: {messages[i]}");
        }
        MessagePanel.text += string.Format(message + "\n");

        scrollRect.normalizedPosition = new Vector2(0, 0);
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        chatClient.PublishMessage(channelName, $"<color=#FF0000><b><i>{PhotonNetwork.NickName}</i></b> has joined</color>");
    }

    public void OnUnsubscribed(string[] channels)
    {
        chatClient.PublishMessage(channelName, $"<color=#FF0000><b><i>{PhotonNetwork.NickName}</i></b> has left</color>");
    }

    #region Unused Methods
    public void DebugReturn(DebugLevel level, string message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnChatStateChange(ChatState state)
    {
        //throw new System.NotImplementedException();
    }


    public void OnDisconnected()
    {
        //throw new System.NotImplementedException();
    }


    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        //throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        //throw new System.NotImplementedException();
    }



    public void OnUserSubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }
    #endregion
}
