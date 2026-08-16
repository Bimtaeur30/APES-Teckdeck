# PHASE-001 — Movement 분리와 Push 속도 구간 전이

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-13 17:35 (KST)`
- 사용자 승인: `Movement 폴더 만들어서 옮기고 SO도 빼고 Enum 추가해서 switch 적용시켜줘`

## 요청과 목표

- 원래 요청: Movement 관련을 폴더로 옮기고, SO로 수치를 빼며, 속도 구간 enum으로 Push에서 Ride/Tuck/Idle switch를 적용한다.
- 이 Phase의 목표: 이동 데이터와 속도 구간 판정을 Movement 모듈에 두고, Push 상태가 손 뗌 시 그 구간으로 전이한다.
- 완료 조건: `IControlMovement.SpeedBand`로 Push가 Ride/Tuck/Idle 중 하나로 바뀌고, 튜닝 수치는 SO에 있다.

## 승인된 구현 범위

- 변경 예정 파일:
  - `Scripts/Player/Movement/` (신규)
  - `GameModules/Player/Movement/PlayerMovementSO.asset` (신규)
  - `PlayerPushState.cs`, `AbstractPlayerState.cs`, `PlayerController.cs`
- 구현 방법: 기존 Movement 스크립트 GUID를 유지한 채 폴더 이동, 수치를 PlayerMovementSO로 이전, BoardSpeedBand를 Rigidbody 수평 속도로 계산, Push에서 전진 입력 해제 시 switch
- 검증 방법: Unity 컴파일, 콘솔 스크립트 에러 없음
- 명시적으로 제외한 항목: 씬에 SO 할당, 다른 상태 전이, UniTask, Shared 수정

## 구현 결과

- 수행한 작업: Movement 스크립트를 `Scripts/Player/Movement/`로 옮기고, 튜닝 수치와 속도 구간 임계값을 `PlayerMovementSO`로 분리했다. Push는 전진 입력을 놓으면 `SpeedBand`로 IDLE/RIDE/TUCK을 고른다.
- 생성·수정한 파일:
  - 생성: `Scripts/Player/Movement/BoardSpeedBand.cs`, `PlayerMovementSO.cs`, `IControlMovement.cs`, `PlayerMovement.cs`
  - 생성: `GameModules/Player/Movement/PlayerMovementSO.asset`
  - 수정: `AbstractPlayerState.cs`, `PlayerController.cs`, `PlayerPushState.cs`
  - 삭제: 기존 `Scripts/Player/PlayerMovement.cs`, `IControlMovement.cs`
- 계획과 달라진 점 및 이유: 씬에 PlayerMovement 컴포넌트가 없어 SO 슬롯을 할당하지 않았다.

## 학습 노트

- 전체 실행 흐름: Idle에서 전진 입력 → Push. Push에서 전진을 놓으면 Movement가 Rigidbody 수평 속도로 Stopped/Ride/Tuck 구간을 계산하고, PushState가 그 값으로 ChangeState 한다.
- 주요 클래스/메서드의 역할:
  - `PlayerMovementSO`: 속력·감쇠·Stopped/Tuck 임계값
  - `PlayerMovement.SpeedBand`: 현재 속도가 어느 구간인지 계산
  - `PlayerPushState.HandleMovementChange`: 입력 전달 + 손 뗌 시 전이
- UniTask 사용 위치와 이유: 사용하지 않음. 입력 한 번에 끝나는 전이라 대기가 없다.
- DOTween 사용 위치와 이유: 사용하지 않음.
- 중요한 구현 원리: 임계값과 현재 속도는 Movement가 들고, 어디로 갈지는 상태가 정한다. `ChangeState`는 Movement가 호출하지 않는다.
- 예외 상황과 대응: SO나 Rigidbody가 없으면 SpeedBand는 Stopped를 반환하고 Initialize에서 Assert 한다. 씬에 SO를 안 꽂으면 손 뗌 시 항상 IDLE로 간다.

## 검증

- 자동 검증: Unity `EditorUtility.scriptCompilationFailed == false`. GUID가 새 Movement 경로를 가리킨다.
- Unity Editor 수동 확인 절차:
  1. `GameModules/Player/Movement/PlayerMovementSO.asset`을 PlayerMovement의 Movement Data에 할당
  2. Play 후 Idle에서 전진 → Push, 손을 떼면 속도에 따라 Ride/Tuck/Idle
  3. SO의 Tuck Speed / Stopped Speed를 바꿔 구간이 바뀌는지 확인
- 결과: 컴파일 성공. Play Mode 전이는 씬에 컴포넌트/SO 할당이 없어 미확인.
- 검증하지 못한 항목: Play Mode에서 실제 보드 속도 구간 전이

## 다음 단계

- 남은 문제: PlayerMovement에 SO 할당, Ride/Tuck/Jump 전이 없음
- 제안하는 다음 Phase: Ride에서 감속 시 Idle, Tuck 진입/이탈
- 추가 승인이 필요한 사항: 씬에 SO를 할당하는 공용/씬 수정
