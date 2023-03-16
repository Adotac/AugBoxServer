using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace MultiP2P
{
    public sealed class Pawn : NetworkBehaviour
    {
        [SyncVar]
        public Player controllingPlayer;

        [SyncVar]
        public float health;
    }
}