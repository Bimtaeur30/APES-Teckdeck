# Phase 021: 공중 감속 비행과 JumpState 복귀

## 목표와 승인

- KTJ가 2026-10-15에 수정된 Phase 021 계획을 `진행`으로 승인했다.
- 점프 중 R키로 GrapplePullState에 진입해 마우스 방향으로 최대 3초간 감속 비행한다.
- 비행 중 벽 접촉 시 즉시 JumpState로 복귀하고, 벽에 닿지 않으면 3초 후 복귀한다.

## 변경 파일

- `02_Script/Player/Player.cs`
- `02_Script/Player/FSM/GrapplePullState.cs`
- `02_Script/Player/FSM/JumpState.cs`
- `02_Script/Player/Movement/IPlayerMovementModule.cs`
- `02_Script/Player/Movement/PlayerMovementModule.cs`
- `_AgentLog/PROGRESS.md`, `_AgentLog/phases/PHASE-021.md`

## 구현

- Player가 점프 중 R키를 누르면 이미 등록된 GrapplePullState로 전환한다.
- GrapplePullState는 진입 시 기존 점프 이동을 일시정지하고 경과 시간을 초기화한다.
- State.Update에서 마우스의 수평 방향을 매 프레임 계산하고 지수 감쇠 속도로 한 프레임의 이동 거리를 구한다.
- 이동 모듈은 기존 점프와 비행의 Raycast, SphereCast, 도착 벽 검사를 같은 메서드로 처리한다.
- 비행에서 벽을 감지하면 접촉 사실을 보관하고 즉시 JumpState로 복귀한다. 복귀한 JumpState는 이동을 재개하고, 모듈의 다음 Update가 도착 콜백으로 IdleState에 전환한다.
- 3초 경과 시 공중에 있으면 JumpState가 저장된 기존 점프 방향과 차징 배율을 재사용한다. JumpStart의 벽 조건은 다시 검사하지 않는다.
- 기존 GrappleEnterContainer 계약과 State 목록 에셋은 유지했다.

## 검증

- Unity Editor 스크립트 재컴파일: 성공, 오류 0개.
- `dotnet build KTJ.csproj --no-restore -nologo -clp:ErrorsOnly -v:q`: 오류 메시지 없이 종료 코드 1. CLI 빌드 성공 여부는 확인하지 못했다.
- Unity Play Mode 동작은 미검증이다. 열린 씬 조회 도구가 HTTP 400을 반환했다.
- 수동 확인: PlayerTestScene에서 벽 점프 후 R키를 눌러 마우스 방향 추종과 감속을 확인한다. 벽에 닿으면 즉시 JumpState로 복귀한 뒤 IdleState로 도착 처리되는지, 벽에 닿지 않으면 약 3초 뒤 JumpState의 기존 점프가 재개되는지 확인한다.

## 범위 밖

- Rigidbody, 공용 FSM, 씬·프리팹, 패키지 변경 없음.
