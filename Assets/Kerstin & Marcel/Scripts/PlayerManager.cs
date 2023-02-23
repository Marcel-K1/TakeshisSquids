using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Made by Marcel

public class PlayerManager : MonoBehaviourPun
{
    private GameObject searchResult;
    
    private PhotonView pv;
    public PhotonView Pv { get => pv; set => pv = value; }

    private ThirdPersonController thirdPersonController;

    private static event System.Action playerIsDeadEvent = () => { };

    public event System.Action PlayerIsDeadEvent { add { playerIsDeadEvent += value; } remove { playerIsDeadEvent -= value; } }

    private void OnEnable()
    {
        GameManager.Instance.ReloadEvent += OnReload;
        playerIsDeadEvent += OnPlayerDeath;
    }

    void Start()
    {
        Pv = photonView;
        thirdPersonController = GetComponent<ThirdPersonController>();

        //Setting up camera reference
        if (Pv.IsMine && !GameManager.Instance.IsSingleplay.IsSingleplay)
        {
            CameraReference.Instance.SetFollow(transform.GetChild(0));       
        }
        else if (GameManager.Instance.IsSingleplay.IsSingleplay)
        {
            CameraReference.Instance.SetFollow(transform.GetChild(0));
            searchResult = GameObject.FindGameObjectWithTag("Chat");
            searchResult.SetActive(false);
        }
    }

    private void Update()
    {
        //Check for LocalPlayer death
        if (Pv.IsMine && !GameManager.Instance.IsSingleplay.IsSingleplay && thirdPersonController.CurrentHealth == 0)
        {
            playerIsDeadEvent.Invoke();
        }
        else if (GameManager.Instance.IsSingleplay.IsSingleplay && thirdPersonController.CurrentHealth == 0)
        {
            playerIsDeadEvent.Invoke();
        }
    }
    
    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReloadEvent -= OnReload;
        }
        playerIsDeadEvent -= OnPlayerDeath;
    }

    //Events
    private void OnPlayerDeath()
    {
        if (Pv.IsMine && !GameManager.Instance.IsSingleplay.IsSingleplay)
        {
            GameManager.Instance.ReloadAndResetToMainMenu();
            if (this.gameObject != null)
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.LeaveLobby();
                PhotonNetwork.Destroy(this.gameObject);
                Destroy(this.gameObject);
            }
        }
        else if (GameManager.Instance.IsSingleplay.IsSingleplay)
        {
            GameManager.Instance.ReloadAndResetToMainMenu();
        }
    }
    private void OnReload()
    {
        if (Pv.IsMine && !GameManager.Instance.IsSingleplay.IsSingleplay)
        {
            if (this.gameObject != null)
            {
                PhotonNetwork.Destroy(this.gameObject);
                Destroy(this.gameObject);
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.LeaveLobby();
            }
        }
        else
        {
            return;
        }

    }

}
