using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.AddressableAssets;

public sealed class Player : NetworkBehaviour
{
    public static Player Instance { get; private set; }
    [SyncVar] public string uname;
    [SyncVar] public bool isReady;
    [SyncVar] public Pawn controlledPawn;

    //override fishnet callbacks
    public override void OnStartServer()
    {
        base.OnStartServer();
        
        GameManager.instance.players.Add(this);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        GameManager.instance.players.Remove(this);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!IsOwner) return;

        Instance = this;

        //UIManager.Instance.Initialize();

        //UIManager.Instance.Show<LobbyView>();
    }

    private void Update()
    {
        if (IsOwner) return; //check if this is the owner of the object in the client or host side

        if (Input.GetKeyDown(KeyCode.R)) {
            ServerSetIsReady(!isReady);
        }

    }

    [ServerRpc(RequireOwnership = false)]//this is a field that allows to execute the code on the serverside
    private void ServerSetIsReady(bool val) {
        isReady = val;   
    }
}
