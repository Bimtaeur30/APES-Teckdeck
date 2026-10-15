# Phase 022: 비행 방향 회전과 마지막 방향 점프 재개

## 목표와 승인

- KTJ가 2026-10-15에 Phase 022 계획을 `진행`으로 승인했다.
- GrapplePullState의 3초 비행 동안 플레이어가 매 프레임 마우스 방향을 바라보게 한다.
- 3초 만료 시 마지막 유효 마우스 방향으로 기존 점프를 재개한다.

## 변경 파일

- `02_Script/Player/FSM/GrapplePullState.cs`
- `02_Script/Player/Movement/IPlayerMovementModule.cs`
- `02_Script/Player/Movement/PlayerMovementModule.cs`
- `_AgentLog/PROGRESS.md`, `_AgentLog/phases/PHASE-022.md`

## 구현

- GrapplePullState는 진입 때 마지막 방향 기록을 초기화하고, 유효한 마우스 방향을 읽을 때마다 최신 방향을 보관한다.
- MoveFlightStep은 해당 방향으로 몸체를 회전시킨 다음 충돌 검사와 Transform 이동을 수행한다.
- 3초 만료 시 벽에 닿지 않았다면 SetJumpDirection으로 저장된 점프 이동 벡터의 방향을 마지막 유효 마우스 방향으로 교체한다. 기존 거리 데이터와 차징 배율은 유지한다.
- 마우스 방향을 한 번도 구하지 못했다면 기존 점프 방향을 유지한다.
- 벽에 먼저 닿았다면 Phase 021처럼 즉시 JumpState로 복귀하며, 점프 방향 변경은 수행하지 않는다. 접촉 프레임에는 벽 법선 회전이 적용된다.

## 검증

- Unity Editor 스크립트 재컴파일: 성공, 오류 0개.
- 방향 저장, 3초 만료, 벽 접촉 조기 복귀 경로를 정적으로 확인했다.
- Unity Editor는 Play Mode가 일시정지된 상태였으므로 실제 움직임과 회전은 확인하지 않았다.
- 수동 확인: PlayerTestScene에서 점프 후 R키를 누르고 마우스를 움직여 플레이어 회전을 확인한다. 벽에 닿지 않은 채 3초가 지나면 마지막 마우스 방향으로 점프가 이어지는지 확인한다. 비행 중 벽에 닿으면 즉시 도착 처리되는지 확인한다.

## 범위 밖

- Rigidbody, 공용 FSM, 씬·프리팹, 패키지 변경 없음.
