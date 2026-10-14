# PHASE-011 — TestStage 스폰 / 타이머 / 클리어(SO)

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-17 06:00 (UTC+9)`
- 사용자 승인: `승인` (클리어 저장은 SO True만)

## 요청과 목표

- 스폰포인트: 위치, 통과 시 저장, 사망 시 해당 스폰 부활
- 시간 UI 표시
- 클리어 시 영구 저장 대신 `StageTypeSO.isUnlocked = true`만 설정

## 승인된 구현 범위

- `Scripts/StageRun/Runtime/*` 신규
- 빈 `StageChoose/Runtime/StageSpawnPoint.cs` 제거(중복 방지)
- `_AgentLog` 갱신
- 제외: PlayerPrefs 등 저장 시스템, JTH 플레이어 수정, TestStage.unity 강제 배치

## 구현 결과

| 클래스 | 역할 |
|---|---|
| `StageSpawnPoint` | Trigger 통과 시 체크포인트 등록 |
| `StageSpawnManager` | 현재 스폰 기억, Respawn, 낙하/R키 테스트 사망 |
| `StageKillZone` | 즉사 트리거 → Respawn |
| `StageTimer` | 경과 시간 TMP 표시(Canvas에 배치한 TMP를 Inspector 연결) |
| `StageClearZone` | 골 트리거 → ClearStage |
| `StageRunController` | 타이머 시작, 클리어 시 SO.isUnlocked=true |

## 학습 노트

- 체크포인트는 `Order`가 낮아지면 덮어쓰지 않음(뒤로 가도 이전 스폰 유지)
- SO 변경은 Play 중 메모리에만 반영되고, Play 종료 시 에셋 원본으로 돌아갈 수 있음(저장 방식 미정)
- 플레이어는 Tag `Player` 또는 Inspector 할당

## 검증

- 코드 작성 완료
- Unity Play: TestStage에 오브젝트 배치 후 수동 확인 필요

## TestStage 배치 가이드

1. 빈 오브젝트 + `StageSpawnManager`, `StageRunController`, `StageTimer`
2. Canvas에 TMP `TimeText` 배치 → `StageTimer.timeText`에 연결 (코드로 UI 생성하지 않음)
3. `StageRunController.stageData`에 Test1/Test2 SO 연결
4. SpawnPoint들: Collider(IsTrigger), Tag 통과용 Player
5. ClearZone / KillZone: Trigger Collider
6. 플레이어 오브젝트 Tag = `Player`
