using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JTH.Vehicles.Board;
using JTH.Vehicles.Board.FSM;
using JTH.Vehicles.Movement;
using ModuleSystem;
using UnityEngine;

namespace JTH.Vehicles
{
    public class BoardActionBinder : MonoBehaviour, IModule
    {
        [SerializeField] private float driftHoldDuration = 0.2f;

        private BoardController _player;
        private IJumpable _jumpable;
        private IDriftable _driftable;

        private bool _jumpTimeOut;
        private CancellationTokenSource _pressCts;

        public bool CanJump => _jumpable.CanJump;
        public bool CanDrift => _player.CurrentState is BoardState.Ride or BoardState.Tuck or BoardState.Push;

        public void Initialize(ModuleOwner owner)
        {
            _player = (BoardController)owner;
            _jumpable = owner.GetModule<IJumpable>();
            _driftable = owner.GetModule<IDriftable>();

            _player.PlayerInput.OnSpaceKeyChange += HandleSpaceKeyChange;
        }

        private void OnDestroy()
        {
            if (_player != null && _player.PlayerInput != null)
                _player.PlayerInput.OnSpaceKeyChange -= HandleSpaceKeyChange;
        }
        
        private void HandleSpaceKeyChange(bool isPressed)
        {
            if (isPressed)
            {
                _pressCts = new CancellationTokenSource();
                WaitDrift(_pressCts.Token).Forget();
                return;
            }
            
            if (_pressCts != null)
            {
                _pressCts.Cancel();
                _pressCts.Dispose();
                _pressCts = null;
            }
            
            if (_jumpTimeOut == false && CanJump)
                _player.ChangeState(BoardState.Jump, 0.1f);
        }

        private async UniTaskVoid WaitDrift(CancellationToken ct)
        {
            _jumpTimeOut = false;
            
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(driftHoldDuration), cancellationToken: ct);
                _jumpTimeOut = true;
                while (true)
                {
                    _driftable.DoDrift = CanDrift;
                    await UniTask.WaitForEndOfFrame(ct);
                }
            }
            catch (OperationCanceledException)
            {
                _driftable.DoDrift = false;
            }
        }
    }
}
