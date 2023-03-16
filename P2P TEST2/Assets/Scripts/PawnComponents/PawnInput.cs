using FishNet.Object;
using UnityEngine;

namespace MultiP2P
{
    public sealed class PawnInput : NetworkBehaviour    
    {
        private Pawn _pawn;

        public float _horizontal;
        public float _vertical;

        public float _mouseX;
        public float _mouseY;

        public float _sensitivity;

        public bool _jump;

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();

            _pawn = GetComponent<Pawn>();
        }

        private void Update()
        {
            if (!IsOwner) return; //should not execute if the controlling side is not the owner

            _horizontal = Input.GetAxis("Horizontal");
            _vertical = Input.GetAxis("Vertical");

            _mouseX = Input.GetAxis("Mouse X") * _sensitivity;
            _mouseY = Input.GetAxis("Mouse Y") * _sensitivity;

            _jump = Input.GetButton("Jump");
        }
    }
}