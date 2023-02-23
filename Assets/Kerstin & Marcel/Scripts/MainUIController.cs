using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Made by Marcel ©UnityAssets

namespace Platformer.UI
{
    public class MainUIController : MonoBehaviour
    {
        public GameObject[] panels;

        public void SetActivePanel(int index)
        {
            for (var i = 0; i < panels.Length; i++)
            {
                var active = i == index;
                var g = panels[i];
                if (g.activeSelf != active) g.SetActive(active);
            }
        }

        void OnEnable()
        {
            SetActivePanel(0);
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            GameManager.Instance.DestroyThyself();
            NetworkManager.Instance.DestroyThyself();   
#else
            Application.Quit();
#endif
        }
    }
}