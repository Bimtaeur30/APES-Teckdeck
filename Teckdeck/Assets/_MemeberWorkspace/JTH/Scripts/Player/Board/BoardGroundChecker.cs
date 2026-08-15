using System;
using System.Collections.Generic;
using ModuleSystem;
using UnityEngine;

namespace JTH.Player.Board
{
    public class BoardGroundChecker : MonoBehaviour, IModule, IGroundChecker
    {
        [SerializeField] private float checkInterval = 0.02f;
        [SerializeField] private List<Vector3> localRayOrigins = new List<Vector3>();
        [SerializeField] private float rayLength = 0.2f;
        [SerializeField] private LayerMask groundLayer;

        private float _checkTimer;

        public bool IsGrounded { get; private set; }
        public event Action OnGrounded;

        public void Initialize(ModuleOwner owner)
        {
            Debug.Assert(groundLayer != 0, "Ground 레이어가 없습니다.");
            Debug.Assert(localRayOrigins is { Count: > 0 }, "레이 시작 위치가 없습니다.");
        }

        private void FixedUpdate()
        {
            _checkTimer += Time.fixedDeltaTime;
            if (_checkTimer < checkInterval)
                return;

            _checkTimer = 0f;
            bool prevGrounded = IsGrounded;
            CheckGround();
            if (prevGrounded == false && IsGrounded)
                OnGrounded?.Invoke();
        }

        private void CheckGround()
        {
            if (localRayOrigins == null || localRayOrigins.Count == 0)
            {
                IsGrounded = false;
                return;
            }

            Vector3 down = -transform.up;
            for (int i = 0; i < localRayOrigins.Count; i++)
            {
                Vector3 origin = transform.TransformPoint(localRayOrigins[i]);
                if (Physics.Raycast(origin, down, rayLength, groundLayer, QueryTriggerInteraction.Ignore) == false)
                {
                    IsGrounded = false;
                    return;
                }
            }

            IsGrounded = true;
        }

        private void OnDrawGizmosSelected()
        {
            if (localRayOrigins == null)
                return;

            Vector3 down = -transform.up;
            for (int i = 0; i < localRayOrigins.Count; i++)
            {
                Vector3 origin = transform.TransformPoint(localRayOrigins[i]);
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(origin, 0.02f);
                Gizmos.DrawLine(origin, origin + down * rayLength);
            }
        }
    }
}
