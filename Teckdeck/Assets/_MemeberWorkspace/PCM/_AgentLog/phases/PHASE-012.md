# PHASE-012 — TestStage Canvas 타이머 UI 배치

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-17 06:06 (UTC+9)`
- 사용자 승인: `승인` (UI는 Canvas 배치, 코드 생성 금지)

## 요청과 목표

- 타이머 UI를 코드로 만들지 않고 `TestStage` Canvas에 설치
- `StageTimer`에 TMP 연결

## 구현 결과

- `Scene/TestStage.unity`에 추가:
  - `Canvas` / `TimeText` (TMP)
  - `EventSystem`
  - `StageRun` (`StageSpawnManager`, `StageTimer`, `StageRunController`)
- `StageTimer.timeText` → `TimeText` 연결
- `StageRunController.stageData` → `Test1` SO

## 검증

- 씬 YAML 반영 완료
- Unity에서 씬 리로드 후 Hierarchy·Inspector 연결 확인 필요
