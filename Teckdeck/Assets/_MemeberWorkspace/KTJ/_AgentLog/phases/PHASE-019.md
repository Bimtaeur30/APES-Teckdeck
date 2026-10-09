# Phase 019: 점프 차징 배율을 이동속도에 적용

## 목표와 승인

- KTJ가 2026-10-06에 전달 구조를 확인하고 `진행`으로 승인했다.
- 점프 키를 놓는 순간의 `currentJumpCharge`를 FSM과 이동 모듈을 거쳐 점프 속도에 적용한다.

## 변경 파일

- `02_Script/Player/Movement/MovementContainer.cs`
- `02_Script/Player/FSM/IdleState.cs`
- `02_Script/Player/FSM/JumpState.cs`
- `02_Script/Player/Movement/IPlayerMovementModule.cs`
- `02_Script/Player/Movement/PlayerMovementModule.cs`
- `_AgentLog/PROGRESS.md`, `_AgentLog/phases/PHASE-019.md`

## 구현

- `MovementContainer`가 이동 방향 데이터와 점프 차징 배율을 함께 보관한다.
- `IdleState`가 컨테이너를 만들어 `JumpState`에 전달한다.
- `JumpState`가 컨테이너를 이동 모듈에 전달한다.
- 이동 모듈은 점프 시작 시 배율을 저장하고, 매 프레임 `jumpForce * 배율 * Time.deltaTime`을 방향에 곱한다.
- 기존 사용자의 `Player` 차징 코드와 입력 변경은 유지했다.

## 검증

- `dotnet build KTJ.csproj --no-restore -nologo -clp:ErrorsOnly -v:q`: 성공, 오류 0개, 경고 12개.
- Unity Play Mode에서 실제 이동속도와 벽 도착은 아직 확인하지 못했다.
