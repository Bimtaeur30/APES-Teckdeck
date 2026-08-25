# PHASE-020 — Board 상태 AbstractVehicleState 이전

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-25 19:45 (KST)`
- 사용자 승인: `State 에러 고쳐줘. BoardState 지워서 필드 새로 만들고 GetModule으로 받아주면 됨`

## 요청과 목표

- 원래 요청: `AbstractBoardState` 삭제 이후 컴파일 에러를, 베이스를 다시 만들지 않고 필드로 받도록 고친다
- 이 Phase의 목표: 보드 상태 클래스가 `AbstractVehicleState`를 상속하고, `BoardController`/`IBrakable`을 생성자에서 캐시한다
- 완료 조건: `AbstractBoardState` CS0246 해소, 콘솔 에러 0

## 승인된 구현 범위

- 변경 예정 파일: `Board/FSM/States/*.cs`, `PROGRESS.md`, `PHASE-020.md`
- 구현 방법: `AbstractBoardState` 대신 `AbstractVehicleState`. `Board`는 owner 캐스트, `Brakable`은 `GetModule<IBrakable>()`
- 검증 방법: `validate_script`, Unity 콘솔 에러
- 명시적으로 제외한 항목: 상태 전환 로직 변경, 씬/프리팹, `_Shared`, `AbstractVehicleState`에 보드 전용 필드 추가

## 구현 결과

- 수행한 작업: 6개 보드 상태를 `AbstractVehicleState`로 옮기고, 쓰던 `Board`/`Brakable`만 필드로 복구
- 생성·수정한 파일:
  - `Assets/_MemeberWorkspace/JTH/Scripts/Vehicles/Board/FSM/States/BoardIdleState.cs`
  - `Assets/_MemeberWorkspace/JTH/Scripts/Vehicles/Board/FSM/States/BoardRideState.cs`
  - `Assets/_MemeberWorkspace/JTH/Scripts/Vehicles/Board/FSM/States/BoardTuckState.cs`
  - `Assets/_MemeberWorkspace/JTH/Scripts/Vehicles/Board/FSM/States/BoardJumpState.cs`
  - `Assets/_MemeberWorkspace/JTH/Scripts/Vehicles/Board/FSM/States/BoardPushState.cs`
  - `Assets/_MemeberWorkspace/JTH/Scripts/Vehicles/Board/FSM/States/BoardBrakeState.cs`
  - `Assets/_MemeberWorkspace/JTH/_AgentLog/PROGRESS.md`
  - `Assets/_MemeberWorkspace/JTH/_AgentLog/phases/PHASE-020.md`
- 계획과 달라진 점 및 이유: `BoardController`는 `IModule`이 아니라서 `GetModule` 대신 owner 캐스트. `IBrakable`만 `GetModule`

## 학습 노트

- 전체 실행 흐름: 상태 생성자에서 owner/모듈을 한 번 받고, Enter/Update/Exit는 기존과 같이 그 필드를 쓴다
- 주요 클래스/메서드의 역할:
  - `AbstractVehicleState`: 모든 탈것 상태가 공유하는 입력·이동
  - `BoardController Board`: 보드 전용 `ChangeState(BoardState, ...)` 호출용
  - `IBrakable Brakable`: 브레이크 가능 여부와 Brake/EndBrake
- UniTask 사용 위치와 이유: 사용 안 함. 컴파일 에러 수정만 함
- DOTween 사용 위치와 이유: 사용 안 함
- 중요한 구현 원리: 베이스 클래스가 없어도, 생성자에서 필요한 의존성만 필드로 두면 상태 로직은 그대로다. 모듈은 `GetModule<T>()`, 오너 자신은 캐스트
- 예외 상황과 대응: owner가 `BoardController`가 아니면 캐스트가 실패한다. 이 상태들은 보드 FSM에서만 생성된다

## 검증

- 자동 검증: 6개 상태 `validate_script` 에러 0, Unity 컴파일 후 `AbstractBoardState` CS0246 0건
- Unity Editor 수동 확인 절차: Play 후 Idle→Push→Ride 전환이 이전과 같은지 확인
- 결과: 컴파일 통과
- 검증하지 못한 항목: Play Mode 체감

## 다음 단계

- 남은 문제: 없음
- 제안하는 다음 Phase: 없음
- 추가 승인이 필요한 사항: 없음
