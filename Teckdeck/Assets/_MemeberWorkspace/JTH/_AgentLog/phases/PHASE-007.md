# PHASE-007 — 슬립각 사이드 그립

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-16 02:11 (KST)`
- 사용자 승인: `진행해, 짜피 값 튜닝은 내가 할꺼야`

## 요청과 목표

- 원래 요청: 코를 조향하고 옆 속도만 깎는다. 일반 라이딩은 드리프트 없음. 옆착지·스킬에서만 각이 크게 벌어지면 Exp/n.
- 이 Phase의 목표: 속도 벡터 회전과 `AlignHeadingToVelocity`를 제거하고, 슬립각 3구간 그립을 넣는다.
- 완료 조건: 점프가 아니면 yaw는 `MoveRotation`, 옆 속도는 Snap/Exp/n. 값은 SO에서 튜닝.

## 승인된 구현 범위

- 변경 예정 파일: `BoardMovement.cs`, `BoardMovementSO.cs`, `BoardMovementSO.asset`
- 구현 방법: 코를 `up` 기준 회전. `|각| < Snap`이면 side=0, `Snap~Break`는 Exp, `Break` 이상은 KineticDecel(n).
- 검증 방법: Unity 컴파일. Play Mode 값은 사용자가 튜닝.
- 명시적으로 제외한 항목: `_Shared`, FSM, 공기저항, 바닥/보드 재질 SO

## 구현 결과

- 수행한 작업: `ApplySideGrip` 추가. `AlignHeadingToVelocity` 삭제. SO에 SnapAngle, BreakAngle, GripDecay, KineticDecel.
- 생성·수정한 파일: `BoardMovement.cs`, `BoardMovementSO.cs`, `BoardMovementSO.asset`
- 계획과 달라진 점 및 이유: 없음. 기본값은 Snap 20, Break 45.

## 학습 노트

- 전체 실행 흐름: FixedUpdate에서 코를 돌린 뒤 슬립각을 재고 옆만 깎는다. 점프 중에는 둘 다 안 한다. 착지 때 각이 남아 있으면 그때 그립이 붙는다.
- 주요 클래스/메서드의 역할: `ApplySideGrip`이 면 위 속도를 앞/옆으로 나누고 각 구간에 따라 옆만 줄인다.
- UniTask 사용 위치와 이유: 없음
- DOTween 사용 위치와 이유: 없음
- 중요한 구현 원리: 일반 턴 각은 Snap 안에 들어가 매 틱 side=0이 된다. Break를 넘는 건 옆착지·스킬처럼 한 번에 각이 크게 열린 경우다. Break 너머 n은 운동마찰이라 Exp보다 약하게 튜닝해야 미끄러짐이 유지된다.
- 예외 상황과 대응: 면 위 속도나 forward가 거의 0이면 각이 불안정해서 return.

## 검증

- 자동 검증: Unity 컴파일
- Unity Editor 수동 확인 절차: 평지에서 턴 시 코를 따라가는지. 공중에서 보드를 돌린 뒤 착지하면 Snap 밖일 때 미끄러지는지. SO 값 조절.
- 결과: 코드 반영
- 검증하지 못한 항목: Play Mode 튜닝

## 다음 단계

- 남은 문제: Snap/Break/GripDecay/KineticDecel 사용자 튜닝
- 제안하는 다음 Phase: 없음
- 추가 승인이 필요한 사항: 없음
