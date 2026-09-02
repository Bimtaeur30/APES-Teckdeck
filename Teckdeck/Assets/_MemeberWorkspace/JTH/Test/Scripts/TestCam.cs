using UnityEngine;

namespace JTH.Test.Scripts
{
    [ExecuteAlways]
    public class TestCam : MonoBehaviour
    {
        [SerializeField] private float distance = 15f;
        [SerializeField] private float[] decel = { 4f, 10f };
        [SerializeField] private float angle;
        [SerializeField] private float yawSwitchAngle = 45f;
        [SerializeField] private float rollPerSideVel;
        [SerializeField] private float maxRoll;
        [SerializeField] private Transform playerTrm;
        [SerializeField] private Vector3 offset;

        private Rigidbody _playerRb;
        private float _yaw;
        private bool _hasYaw;

        private void Awake()
        {
            Debug.Assert(playerTrm != null, "TestCam: playerTrm이 없습니다.");
            CachePlayerRb();
        }

        private void OnValidate()
        {
            CachePlayerRb();

#if UNITY_EDITOR
            if (Application.isPlaying == false)
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
#endif
        }

        private void Update()
        {
            if (playerTrm == null)
                return;

            if (Application.isPlaying == false)
            {
                ApplyCamera(playerTrm.eulerAngles.y, GetRoll());
                return;
            }

            if (decel == null || decel.Length == 0)
                return;

            float targetYaw = playerTrm.eulerAngles.y;
            if (_hasYaw == false)
            {
                _yaw = targetYaw;
                _hasYaw = true;
            }

            float yawDelta = Mathf.Abs(Mathf.DeltaAngle(_yaw, targetYaw));
            int decelIndex = yawDelta > yawSwitchAngle && decel.Length > 1 ? 1 : 0;
            float decay = -decel[decelIndex];
            Debug.Log(decelIndex);
            _yaw = Mathf.LerpAngle(
                _yaw,
                targetYaw,
                1f - Mathf.Exp(decay * Time.deltaTime)
            );

            ApplyCamera(_yaw, GetRoll());
        }

        private void CachePlayerRb()
        {
            _playerRb = playerTrm != null ? playerTrm.GetComponent<Rigidbody>() : null;
        }

        private float GetRoll()
        {
            if (_playerRb == null)
                CachePlayerRb();

            if (_playerRb == null)
                return 0f;

            float sideVel = Vector3.Dot(_playerRb.linearVelocity, playerTrm.right);
            return Mathf.Min(maxRoll, Mathf.Abs(sideVel * rollPerSideVel)) * (sideVel > 0 ? 1 : -1);
        }

        private void ApplyCamera(float yaw, float roll)
        {
            Quaternion posRot = Quaternion.Euler(angle, yaw, 0f);
            transform.SetPositionAndRotation(
                playerTrm.position + posRot * (Vector3.back * distance) + offset,
                Quaternion.Euler(angle, yaw, roll)
            );
        }
    }
}
