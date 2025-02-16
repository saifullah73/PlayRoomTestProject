using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Playroom;

public class TestLobbyMock : LobbyManager
{

    public override void Start()
    {
        Debug.Log("Staring Lobby");
        totalPlayersJoined = 0;
        playerManagers = new List<PlayerManager>();
        availableSpawnIdx = new List<int>();
        uiController.resetButton.onClick.AddListener(() => ResetAndStartRace());
        for (int i = 0; i < spanwPoints.Length; i++)
        {
            availableSpawnIdx.Add(i);
        }

        SpawnPlayersTest("abc");
    }

    protected void ResetAndStartRace(){
        foreach (PlayerManager p in playerManagers)
        {

            p.Init(p.playroomplayer,p.playroomId, p.playerIdx, cameraManger, true);
            p.ResetAnims();
            p.transform.position = spanwPoints[p.playerIdx].position;
            p.transform.rotation = spanwPoints[p.playerIdx].rotation;
        }

        if (!raceStarted){
            StartCoroutine(CountdownRoutine());
        }

    }



    protected void SpawnPlayersTest(string id){
        int playeridx = availableSpawnIdx.Count > 0 ? availableSpawnIdx[0] : 0;
        PlayerManager playerManager = Instantiate<PlayerManager>(playerPrefab[playeridx], spanwPoints[playeridx].position, spanwPoints[playeridx].rotation);
        playerManager.Init(null,id, playeridx,cameraManger,true);
        playerManagers.Add(playerManager);
        availableSpawnIdx.Remove(playeridx);
        totalPlayersJoined += 1;
        if (!raceStarted){
            StartCoroutine(CountdownRoutine());
        }
    }

    protected override void StartRace(){
        foreach (PlayerManager player in playerManagers)
        {
            player.StartRace();
        }
    }

    public override void EndGame(string playerId){
        raceStarted = false;
        PlayerManager player = playerManagers.Find(x => x.playroomId == playerId);

        string winner = player.playerIdx == 0 ? "Blue" : "Pink";
        string color = winner == "Blue"? "#71ABB2" : "#BC8782";
        uiController.ShowText($"FINISH\n<color={color}> {winner}</color> Wins");
        uiController.Show(true);
        uiController.ShowButton(true);
        foreach (PlayerManager p in playerManagers)
        {
            p.EndRace(player.playroomId == playerId);
        }
    }
}
