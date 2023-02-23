using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Made by Marcel

public class Balls : MonoBehaviour
{
    [SerializeField]
    private float ballDamage = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PhotonView photonView = other.GetComponent<PhotonView>();

            if (!GameManager.Instance.IsSingleplay.IsSingleplay)
            {
                if (photonView.IsMine)
                {
                    other.GetComponent<ThirdPersonController>().CurrentHealth = other.GetComponent<ThirdPersonController>().CurrentHealth - ballDamage;
                }
            }
            else if (GameManager.Instance.IsSingleplay.IsSingleplay)
            {
                other.GetComponent<ThirdPersonController>().CurrentHealth = other.GetComponent<ThirdPersonController>().CurrentHealth - ballDamage;
            }


        }
    }

}
