# 작업 진행 현황

## 사용자

- 이름/이니셜: `PCM`
- 작업폴더: `Assets/_MemeberWorkspace/PCM/`
- 마지막 갱신: `2026-08-11 14:55 (UTC+9)`

## 현재 요청

- 요청 요약: 고장 난 MapUiSG Shader Graph 복구
- 승인된 범위: Phase 004 완료
- 범위 밖 항목: 텍스트/챌린지 UI

## Phase 현황

| Phase | 상태 | 목표 | 기록 |
|---|---|---|---|
| 002 | 완료 | 반투명 패널 | `phases/PHASE-002.md` |
| 003 | 완료 | 우측 끝 페이드(당시 .shader) | `phases/PHASE-003.md` |
| 004 | 완료 | MapUiSG Shader Graph 복구 | `phases/PHASE-004.md` |

## 현재 재개 지점

- 마지막 완료 작업: `MapUiSG.shadergraph` 복구 + MapUiMat 재연결
- 다음에 할 작업: Unity에서 Graph/씬 시각 확인
- 관련 파일: `MapUiSG.shadergraph`, `MapUiMat.mat`, `StageChoose Scene.unity`

## 검증 요약

- 수행한 검증: 파일 복구·참조 GUID 유지
- 통과 여부: 부분 (Editor Import/시각 미확인)
- 아직 검증하지 못한 항목: Shader Graph 창 오픈, Game 뷰 페이드
