# PHASE-003 — Turn yaw 적용과 Push 가속도 이벤트

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-14 19:55 (KST)`
- 사용자 승인: `구현하면 됨` — Turn/Push 분리, 속도 회전+yaw, Push 이벤트 가속도, Jump 공중 잠금. 이름 변경은 제외.

## 요청과 목표

- 원래 요청: `_velocity` 없이 `linearVelocity`를 돌리고 owner는 yaw만 추종. Push/Turn은 float. Push는 애니 이벤트 구간 동안 SO 가속도. 점프 중 조향·보드 회전 없음. 입력은 Player 유지.
- 이 Phase의 목표: Movement 적용 방식을 바꾸고, BoardTrigger+PushState로 킥 수명을 상태가 관리하게 한다.
- 완료 조건: `SetMovementDirection` 제거, `Turn`/`Push`/`EndPush`로 동작, Visual에 BoardTrigger, 컴파일 에러 없음.

## 승인된 구현 범위

- 변경 예정 파일: Movement 인터페이스/구현, 상태들, PlayerInputSO, BoardTrigger, PlayerMoveScene
- 구현 방법: FixedUpdate에서 `owner.up` 기준 속도 회전 + yaw 정렬. 점프 중 둘 다 스킵. Push는 시작~끝 이벤트 동안 `AddForce(Acceleration)`.
- 검증 방법: Unity 컴파일, 콘솔 에러 없음. Play Mode는 사용자가 클립 이벤트 연결 후 확인.
- 명시적으로 제외한 항목: `Player`→`Board` 이름 변경, `_Shared` 수정, 클립에 이벤트 키프레임 삽입

## 구현 결과

- 수행한 작업: Vector2 입력을 Turn(x)와 FSM의 y로 분리. 턴 Lerp는 유지하고 적용만 속도 회전으로 변경. Push는 커브 없이 이벤트 구간 가속도. Jump Enter에서 Turn(0), 공중에서는 조향·yaw 없음. Visual에 BoardTrigger 추가.
- 생성·수정한 파일:
  - 생성: `Scripts/Player/BoardTrigger.cs`, `IBoardTrigger.cs`, `Movement/IPushable.cs`
  - 수정: `PlayerMovement.cs`, `IControlMovement.cs`, `PlayerMovementSO.cs`, `PlayerInputSO.cs`, Idle/Ride/Tuck/Push/Brake/Jump 상태
  - 수정: `Scenes/PlayerMoveScene.unity` (Visual에 BoardTrigger)
- 계획과 달라진 점 및 이유: 수평 속도 클램프는 기존 MaxSpeed 의도를 유지하며 깨져 있던 `normalized * magnitude`를 고쳤다.

## 학습 노트

- 전체 실행 흐름: 상태가 `Turn(x)` → Update에서 `_turnSpeed` Lerp → FixedUpdate에서 점프가 아니면 속도를 `owner.up`으로 돌리고 보드 yaw를 평면 속도에 맞춤. Push 클립의 `AnimPushStart`/`AnimPushEnd`가 BoardTrigger 이벤트 → PushState가 `Push`/`EndPush` 후 속도 구간으로 전이. Exit 때 아직 밀고 있으면 EndPush.
- 주요 클래스/메서드의 역할:
  - `PlayerMovement.Turn`: 조향 입력만 저장
  - `ApplyMovement`: 속도 회전, yaw, 푸시 힘
  - `BoardTrigger`: Animator와 같은 오브젝트에서 애니 이벤트를 모듈 이벤트로 변환
  - `PlayerPushState`: 방향 ±1, 블렌드 파라미터, 푸시 수명
- UniTask 사용 위치와 이유: Jump 커브 루프만 기존처럼 `WaitForFixedUpdate`. Push는 이벤트+FixedUpdate라 UniTask 없음.
- DOTween 사용 위치와 이유: 사용하지 않음.
- 중요한 구현 원리: `AngleAxis(turn, up) * velocity`는 up 성분을 보존한다. yaw는 `SignedAngle`로 up 축만 돌려 피치/롤을 물리 엔진에 맡긴다. `AddForce(Acceleration)`는 초당 속도 변화라 틱 설정에 덜 묶인다.
- 예외 상황과 대응: 평면 속도가 Stopped 이하면 시선 스킵. 점프 중 회전 스킵. 푸시 중 점프로 나가면 Exit가 EndPush.

## 검증

- 자동 검증: Unity 컴파일, 콘솔 에러 0
- Unity Editor 수동 확인 절차:
  1. Push 클립에 `AnimPushStart` / `AnimPushEnd` 이벤트 넣기
  2. Animator에 Float `PushDirection` 파라미터 추가
  3. Play에서 전진 → Push, 조향 시 속도가 옆으로 돌고 보드는 yaw만 따라감
  4. 점프 중에는 보드가 이륙 자세를 유지
- 결과: 컴파일 통과. Play Mode 미실시
- 검증하지 못한 항목: 실제 푸시 클립 이벤트, 경사 yaw, 점프 착지 감각

## 다음 단계

- 남은 문제: 애니 이벤트·블렌드 파라미터는 에셋 작업. PushPower 튜닝.
- 제안하는 다음 Phase: `Player`→`Board` 이름 변경 (입력 SO/맵은 Player 유지)
- 추가 승인이 필요한 사항: 이름 변경 Phase
