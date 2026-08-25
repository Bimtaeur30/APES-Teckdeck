using UnityEngine;

namespace JTH.Test.Scripts
{
    [ExecuteInEditMode] // 에디터 모드에서도 실시간으로 확인 가능하도록 설정
    public class TestAxisRotator : MonoBehaviour
    {
        [Header("기즈모 설정")]
        public float axisLength = 3.0f; // 그리고자 하는 축의 길이
        public Color gizmoColor = Color.yellow; // 축의 색상

        private void OnDrawGizmos()
        {
            // 1. 현재 오브젝트의 회전 상태(Quaternion)를 가져옵니다.
            Quaternion currentRotation = transform.rotation;

            // 2. 만약 이미 identity(회전 없음) 상태라면 축을 계산할 필요가 없습니다.
            if (Quaternion.Angle(currentRotation, Quaternion.identity) < 0.1f)
            {
                return;
            }

            // 3. 원래 상태(Identity)로 돌아가기 위한 역회전(Inverse)을 구합니다.
            Quaternion inverseRotation = Quaternion.Inverse(currentRotation);

            // 4. 역회전 Quaternion에서 '단 하나의 회전축(Vector3)'과 '회전각(float)'을 추출합니다.
            // 이 때 ToAngleAxis가 추출해내는 axis가 바로 오일러 회전 정리에서 말하는 유일한 축입니다.
            inverseRotation.ToAngleAxis(out _, out Vector3 recoveryAxisWorld);

            // 5. 계산된 축을 기즈모로 화면에 그려줍니다.
            Gizmos.color = gizmoColor;
        
            // 오브젝트의 중심점(Pivot)을 기준으로, 계산된 세계 좌표계(World) 기준의 복구 축을 선으로 그립니다.
            Vector3 startPoint = transform.position - (recoveryAxisWorld * axisLength * 0.5f);
            Vector3 endPoint = transform.position + (recoveryAxisWorld * axisLength * 0.5f);
        
            Gizmos.DrawLine(startPoint, endPoint);

            // 축의 방향을 시각적으로 알 수 있게 양 끝에 작은 구체를 그려줍니다.
            Gizmos.DrawSphere(startPoint, 0.05f);
            Gizmos.DrawSphere(endPoint, 0.05f);
        }
    }
}