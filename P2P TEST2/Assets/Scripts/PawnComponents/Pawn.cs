using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public sealed class Pawn : NetworkBehaviour
{
    [SyncVar]
    public Player controllingPlayer;

    [SyncVar]
    public float health;
}
