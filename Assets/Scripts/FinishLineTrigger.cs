using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLineTrigger : MonoBehaviour
{
    public LobbyManager lobbyManager;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            string id = other.GetComponent<PlayerManager>().playroomId;
            lobbyManager.EndGame(id);
        }
    }
}
