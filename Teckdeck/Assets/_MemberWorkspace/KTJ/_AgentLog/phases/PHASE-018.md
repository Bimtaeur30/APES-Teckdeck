# Phase 018: JumpState 고착 수정

## 목표와 승인

- KTJ가 2026-10-05에 Phase 018 계획을 승인했다.
- 점프 시작 실패 또는 도착 벽 감지 누락 후 JumpState에 남는 문제를 수정한다.

## 변경 파일

- `02_Script/Player/FSM/IdleState.cs`
- `02_Script/Player/FSM/JumpState.cs`
- `02_Script/Player/Movement/IPlayerMovementModule.cs`
- `02_Script/Player/Movement/PlayerMovementModule.cs`
- `_AgentLog/PROGRESS.md`, `_AgentLog/phases/PHASE-018.md`

## 구현

- `JumpStart`가 성공 여부를 반환한다. 실패하면 `JumpState`가 즉시 Idle로 복귀한다.
- 시작 시 벽 감지를 다시 수행해 `Update` 실행 순서에 따른 오래된 판정을 사용하지 않는다.
- 이동 경로의 Raycast/SphereCast가 벽을 놓친 경우 이동 후 구와 겹치는 앞쪽 비 Trigger 벽을 확인한다.
- 도착 시 `isJumping`을 먼저 해제하고 기존 종료 콜백을 호출한다.
- 벽 겹침 검사에서 Trigger를 제외해 이동 캐스트와 감지 조건을 일치시켰다.
- 기존 사용자의 벽 법선 회전 변경과 씬·프리팹 변경은 유지했다.

## 검증

- `dotnet build KTJ.csproj --no-restore -nologo -clp:ErrorsOnly -v:q`: 성공, 오류 0개, 기존 경고 12개.
- Unity Play Mode의 빠른 연속 점프와 벽 도착 동작은 아직 확인하지 못했다.
