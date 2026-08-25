# PHASE-014 — 회전 적용 앞속도 임계

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-20 13:47 (KST)`
- 사용자 승인: `앞으로 가지도 않는데 회전하면 이상해보임. rotationSpeed는 그대로 오르내리고 적용만 임계값`

## 요청과 목표

- `Mathf.Abs(frontVel)`이 SO 임계를 넘을 때만 yaw를 적용한다.
- `_turnSpeed` 계산은 정지 중에도 그대로 오르내린다.

## 구현 결과

- SO `RotationThreshold` 기본 1.
- `CalculationRotation`은 그대로. `MoveRotation`만 임계 뒤에 둔다.

## 검증

- 정지 상태에서 스틱을 꺾어도 보드는 안 돌고, 밀린 뒤에만 도는지. Inspector에서 임계 조절.
