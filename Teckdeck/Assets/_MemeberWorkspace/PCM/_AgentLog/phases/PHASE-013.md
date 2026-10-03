# PHASE-013 — Player/참조 Inspector 연결

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-17 20:20 (UTC+9)`
- 사용자 승인: `승인`

## 요청과 목표

- Tag / Find로 Player·매니저를 찾지 않고 Inspector 연결만 사용

## 구현 결과

- `StageSpawnManager`: Tag 폴백·FindObjects 제거, `player`/`initialSpawn` 필수 연결
- `IsPlayer(Collider)`로 자식 콜라이더까지 판별
- `StageSpawnPoint` / `StageKillZone` / `StageClearZone`: Tag 비교 제거
- `StageRunController` / `StageClearZone`: FindFirstObjectByType 제거

## 씬에서 연결할 것

1. `StageSpawnManager.player` → Player Transform
2. `StageSpawnManager.initialSpawn` → 시작 SpawnPoint
3. `StageRunController` → timer / spawnManager / stageData (이미 TestStage에 일부 연결됨)
4. `StageClearZone.runController` → StageRunController

## 검증

- 코드 반영 완료
- Unity에서 Player·Spawn 슬롯 연결 후 Play 확인 필요
