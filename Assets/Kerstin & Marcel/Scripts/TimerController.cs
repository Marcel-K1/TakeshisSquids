using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Made by Marcel

public class TimerController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI timeCounter;

    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private float roundTime = 180f;

    private bool timerGoing;

    private TimeSpan timeLeft;

    public TimeSpan TimeLeft { get => timeLeft; set => timeLeft = value; }
    public float RoundTime { get => roundTime; set => roundTime = value; }
    public bool TimerGoing { get => timerGoing; set => timerGoing = value; }


    public void BeginTimer()
    {
        TimerGoing = true;
        StartCoroutine(UpdateTimer());
    }

    //Set up for the timer of the game
    private IEnumerator UpdateTimer()
    {
        while (TimerGoing)
        {
            roundTime -= Time.deltaTime;
            TimeLeft = TimeSpan.FromSeconds(RoundTime);
            string timeLeftStr = TimeLeft.ToString("mm':'ss");
            timeCounter.text = $"Time: {timeLeftStr}";
            yield return null;
        }
    }
}
