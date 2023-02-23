using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Made by Marcel

public class PickUps : MonoBehaviour
{
    [SerializeField]
    private float flipSpeed = 5f;

    private void Update()
    {
        transform.Rotate(0, 0, 90 * Time.deltaTime * flipSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PhotonView photonView = other.GetComponent<PhotonView>();

            //Checking for Single- or Multiplay
            if (!GameManager.Instance.IsSingleplay.IsSingleplay)
            {
                if (photonView.IsMine)
                {
                    if (this.gameObject.tag == "GoldPickUp")
                    {
                        PhotonNetwork.LocalPlayer.AddScore(5);
                    }
                    else if (this.gameObject.tag == "VioletPickUp")
                    {
                        PhotonNetwork.LocalPlayer.AddScore(1);
                    }
                    Destroy(gameObject);
                }
            }
            else if (GameManager.Instance.IsSingleplay.IsSingleplay)
            {
                if (this.gameObject.tag == "GoldPickUp")
                {
                    other.GetComponent<ThirdPersonController>().Points += 5;
                }
                else if (this.gameObject.tag == "VioletPickUp")
                {
                    other.GetComponent<ThirdPersonController>().Points++;
                }
                Destroy(gameObject);
            }


        }
    }

}
