using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

//Made by Marcel ©Photon

[RequireComponent(typeof(PhotonView))]
public class ShowInfoOfPlayer : MonoBehaviourPunCallbacks
{
    private GameObject textGo;
    public TextMesh tm;
    public float CharacterSize = 0;

    public Font font;
    public bool DisableOnOwnObjects;

    void Start()
    {
        if (font == null)
        {
#if UNITY_3_5
            font = (Font)FindObjectsOfTypeIncludingAssets(typeof(Font))[0];
#else
            font = (Font)Resources.FindObjectsOfTypeAll(typeof(Font))[0];
#endif
            Debug.LogWarning("No font defined. Found font: " + font);
        }

        if (tm == null)
        {
            textGo = new GameObject("3d text");
            //textGo.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            textGo.transform.parent = this.gameObject.transform;
            textGo.transform.localPosition = Vector3.zero;

            MeshRenderer mr = textGo.AddComponent<MeshRenderer>();
            mr.material = font.material;
            tm = textGo.AddComponent<TextMesh>();
            tm.font = font;
            tm.anchor = TextAnchor.MiddleCenter;
            if (this.CharacterSize > 0)
            {
                tm.characterSize = this.CharacterSize;
            }
        }
    }

    void Update()
    {
        bool showInfo = !this.DisableOnOwnObjects || this.photonView.IsMine;

        if (textGo != null)
        {
            textGo.SetActive(showInfo);
        }
        if (!showInfo)
        {
            return;
        }


        Player owner = this.photonView.Owner;

        if (!GameManager.Instance.IsSingleplay.IsSingleplay)
        {
            if (owner != null)
            {
                tm.text = (string.IsNullOrEmpty(owner.NickName)) ? "player" + owner.UserId : owner.NickName;
            }
            else if (this.photonView.IsRoomView)
            {
                tm.text = "Default";
            }
            else
            {
                tm.text = "Default";
            }
        }
        else if (GameManager.Instance.IsSingleplay.IsSingleplay)
        {
            tm.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0,0,0));
            tm.text = PlayerPrefs.GetString("PlayerName", "Default");
        }
    }
}
