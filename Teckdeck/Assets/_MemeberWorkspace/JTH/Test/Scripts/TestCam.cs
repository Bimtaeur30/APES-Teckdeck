using UnityEngine;

namespace JTH.Test.Scripts
{
    public class TestCam : MonoBehaviour
    {
        [SerializeField] private float distance = 15f;
        [SerializeField] private float[] decel = { 4f, 10f };
        [SerializeField] private float angle;
        [SerializeField] private float yawSwitchAngle = 45f;
        [SerializeField] private float rollPerSideVel;
        [SerializeField] private Transform playerTrm;

        private Rigidbody _playerRb;
        private float _yaw;
        private bool _hasYaw;

        private void Awake()
        {
            Debug.Assert(playerTrm != null, "TestCam: playerTrm이 없습니다.");
            Debug.Assert(decel != null && decel.Length > 0, "TestCam: decel 배열이 비어 있습니다.");

            if (playerTrm != null)
                _playerRb = playerTrm.GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (playerTrm == null || decel == null || decel.Length == 0)
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

            _yaw = Mathf.LerpAngle(
                _yaw,
                targetYaw,
                1f - Mathf.Exp(decay * Time.deltaTime)
            );

            float roll = 0f;
            if (_playerRb != null)
            {
                float sideVel = Vector3.Dot(_playerRb.linearVelocity, playerTrm.right);
                roll = sideVel * rollPerSideVel;
            }

            Quaternion posRot = Quaternion.Euler(angle, _yaw, 0f);
            transform.SetPositionAndRotation(
                playerTrm.position + posRot * (Vector3.back * distance),
                Quaternion.Euler(angle, _yaw, roll)
            );
        }
    }
}
