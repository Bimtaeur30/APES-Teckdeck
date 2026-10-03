# Phase 015: 가까운 앞 벽에서 점프 이동 정지

## 목표와 승인

- 사용자 `KTJ`가 2026-10-03에 이동 모듈 수정 계획을 승인했다.
- 출발 벽과 앞 벽이 가까워 `isWall`이 계속 참이어도 앞 벽에서 이동을 멈춘다.

## 변경 파일

- `Assets/_MemeberWorkspace/KTJ/02_Script/Player/Movement/PlayerMovementModule.cs`
- 작업 기록: `_AgentLog/PROGRESS.md`, `_AgentLog/phases/PHASE-015.md`

## 구현

- `leftStartWall` 상태 전환을 점프 정지 조건에서 제거했다.
- 매 `Update`의 이동 전에 실제 플레이어 SphereCollider의 월드 반경과 중심을 구한다.
- 중심 Raycast로 시작 시 이미 플레이어 구가 닿는 가까운 앞 벽을 확인한다.
- SphereCast로 그 밖의 이동 경로와 가장자리 충돌을 확인한다.
- 감지된 거리 중 가까운 곳에서 플레이어 Collider가 벽에 닿도록 멈춘 뒤 `isJumping`을 해제한다.
- Rigidbody 이동, 조준선, 씬·프리팹, 공용 파일은 변경하지 않았다.

## 검증

- `dotnet build KTJ.csproj --no-restore -nologo -v:q`: 성공, 오류 0개. 생성 프로젝트의 기존 참조 버전 충돌 경고 8개.
- `git diff --check`: 통과.
- Unity Play Mode에서 가까운 두 벽 사이 이동은 이 환경에서 실행 검증하지 못했다.

## 남은 확인 사항

- `PlayerTestScene`에서 출발 벽과 앞 벽의 간격이 짧아도 앞 벽에서 멈추는지 확인한다.
- Player의 `OnCollisionStay`와 조준 각도 허용 여부는 별개 흐름이며 이번 Phase에서 수정하지 않았다.
- 플레이어 중심이 이미 벽 Collider 내부에 놓인 경우에는 Raycast와 SphereCast가 모두 해당 벽을 놓칠 수 있다.

## 같은 Phase의 후속 수정 (2026-10-03)

- 사용자 확인 결과 첫 이동 후 다음 이동이 시작되지 않았다.
- 원인: 초기 정지 간격 `0.02`가 씬의 `overlapSphereRadius 0.51`과 플레이어 반경 `0.5`의 차이 `0.01`보다 커서, 도착 후 `isWall`이 거짓이 되었다. 벽 접점 노말도 갱신되지 않을 수 있었다.
- 정지 간격을 제거하고 실제 Collider가 벽에 닿는 위치에서 멈추도록 변경했다.
- 후속 수정 뒤 `dotnet build KTJ.csproj --no-restore -nologo -clp:ErrorsOnly -v:q` 성공: 오류 0개, 기존 참조 충돌 경고 8개.
