using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Playroom;

public class LobbyManager : MonoBehaviour
{
    public PlayroomKit _playroomKit;

    public void Awake()
    {
        _playroomKit = new PlayroomKit();

    }

    public void Start()
    {
        Debug.Log("Staring Lobby");
        _playroomKit.InsertCoin(new InitOptions()
        {
            maxPlayersPerRoom = 2,
            matchmaking = false,
            defaultPlayerStates = new() {
                {"health", 100},
            },
        }, () => {
            // Game launch logic here
        },() => { Debug.Log("OnDisconnect callback"); });
    }
}
