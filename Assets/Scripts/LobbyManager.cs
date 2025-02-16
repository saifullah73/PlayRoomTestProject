using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Playroom;
using System;

public class LobbyManager : MonoBehaviour
{
    public PlayroomKit _playroomKit;

    public Transform[] spanwPoints;

    public PlayerManager[] playerPrefab;

    public UIController uiController;

    protected int totalPlayersJoined = 0;
    protected List<PlayerManager> playerManagers;

    protected List<int> availableSpawnIdx;
    public CameraManger cameraManger;
    protected bool raceStarted = false;
    public void Awake()
    {
        _playroomKit = new PlayroomKit();
    }

    protected virtual void ResetAndStartRace(string data, string senderId){
        string[] statesToExclude = {};
        _playroomKit.ResetStates(statesToExclude, () =>
        {
        });
        foreach (PlayerManager p in playerManagers)
        {

            p.Init(p.playroomplayer,p.playroomId, p.playerIdx, cameraManger, p.playroomId == _playroomKit.MyPlayer().id);
            p.ResetAnims();
            p.transform.position = spanwPoints[p.playerIdx].position;
            p.transform.rotation = spanwPoints[p.playerIdx].rotation;
        }

        if (totalPlayersJoined == 2 && !raceStarted){
            StartCoroutine(CountdownRoutine());
        }

    }



    protected virtual void Reset(){
        _playroomKit.RpcCall("reset", "", PlayroomKit.RpcMode.ALL);
    }

    public virtual void Start()
    {
        Debug.Log("[Unity Log] Staring Lobby");
        totalPlayersJoined = 0;
        playerManagers = new List<PlayerManager>();
        availableSpawnIdx = new List<int>();
        uiController.resetButton.onClick.AddListener(() => Reset());
        for (int i = 0; i < spanwPoints.Length; i++)
        {
            availableSpawnIdx.Add(i);
        }
        _playroomKit.InsertCoin(new InitOptions()
        {
            maxPlayersPerRoom = 2,
            matchmaking = false,
            gameId = "6JbtdgOCgyNd5eXKKsr0",
        }, () => {
            _playroomKit.OnPlayerJoin(SpawnPlayers);
            
            _playroomKit.RpcRegister("reset", ResetAndStartRace);
            _playroomKit.RpcRegister("end", EndRaceRPC);



        },() => { Debug.Log("OnDisconnect callback"); });
    }



    protected virtual void SpawnPlayers(PlayroomKit.Player player){
        int playeridx = availableSpawnIdx.Count > 0 ? availableSpawnIdx[0] : 0;
        PlayerManager playerManager = Instantiate<PlayerManager>(playerPrefab[playeridx], spanwPoints[playeridx].position, spanwPoints[playeridx].rotation);
        bool isMyPlayer = _playroomKit.MyPlayer().id == player.id;
        playerManager.Init(player,player.id, playeridx,cameraManger,isMyPlayer);
        playerManagers.Add(playerManager);
        availableSpawnIdx.Remove(playeridx);
        totalPlayersJoined += 1;
        player.OnQuit(RemovePlayer);

        if (totalPlayersJoined == 2 && !raceStarted){
            StartCoroutine(CountdownRoutine());
        }
    }

    protected IEnumerator CountdownRoutine()
    {

        uiController.ShowButton(false);
        uiController.Show(true);
        uiController.ShowText("3");
        yield return new WaitForSeconds(1f);
        uiController.ShowText("2");
        yield return new WaitForSeconds(1f);
        uiController.ShowText("1");
        yield return new WaitForSeconds(1f);
        uiController.ShowText("START");
        yield return new WaitForSeconds(1f);
        uiController.Show(false);

        StartRace();

         
    }

    protected virtual void StartRace(){
        foreach (PlayerManager player in playerManagers)
        {
            player.StartRace();
        }
    }

    private void RemovePlayer(string playerID)
    {
        PlayerManager player = playerManagers.Find(x => x.playroomId == playerID);

        if (player != null)
        {
            availableSpawnIdx.Add(player.playerIdx);
            playerManagers.Remove(player);
            totalPlayersJoined -= 1;
            Destroy(player.gameObject);
        }
        else
        {
            Debug.LogWarning("Player not found in list");
        }
    }

    protected virtual void EndRaceRPC(string data, string senderId){

        string playerId = _playroomKit.GetState<string>("winner");
        raceStarted = false;
        PlayerManager player = playerManagers.Find(x => x.playroomId == playerId);
        Debug.Log("[Unity Log] WinnerID Recieved " + playerId);
        string winner = player.playerIdx == 0 ? "Blue" : "Pink";
        string color = winner == "Blue"? "#71ABB2" : "#BC8782";
        uiController.ShowText($"FINISH\n<color={color}> {winner}</color> Wins");
        uiController.Show(true);
        if (_playroomKit.IsHost()){
            uiController.ShowButton(true);
        }
        foreach (PlayerManager p in playerManagers)
        {
            p.EndRace(p.playroomId == playerId);
        }
    }

    public virtual void EndGame(string playerId){
        Debug.Log("[Unity Log] WinnerID initiate " + playerId);
        _playroomKit.SetState("winner", playerId,true);
        _playroomKit.RpcCall("end", "", PlayroomKit.RpcMode.ALL);
    }
}
