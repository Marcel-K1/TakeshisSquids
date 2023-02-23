using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO", menuName = "ScriptableObjects/SO_GameMode", order = 1)]
public class SO_GameMode : ScriptableObject
{
    [SerializeField]
    private bool isSingleplay;
    public bool IsSingleplay { get => isSingleplay; set => isSingleplay = value; }

}
