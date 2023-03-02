using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Linq;
using UnityEngine;

public sealed class GameManager : NetworkBehaviour
{
    public static GameManager instance { get; set; }

    [SyncObject]
    public readonly SyncList<Player> players = new SyncList<Player>();

    [SyncVar]
    public bool conStart; //for the other scripts in the server

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (!IsServer) return; //if not in the server don't execute update

        conStart = players.All(player => player.isReady);
    }

    //[Server]
    //public void StartGame()
    //{
    //    if (!conStart) return;

    //    for (int i = 0; i < players.Count; i++)
    //    {
    //        players[i].StartGame();
    //    }
    //}

    //[Server]
    //public void StopGame()
    //{
    //    for (int i = 0; i < players.Count; i++)
    //    {
    //        players[i].StopGame();
    //    }
    //}
}
