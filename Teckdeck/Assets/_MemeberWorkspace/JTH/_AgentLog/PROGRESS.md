# 작업 진행 현황

## 사용자

- 이름/이니셜: `JTH`
- 작업폴더: `Assets/_MemeberWorkspace/JTH/`
- 마지막 갱신: `2026-08-25 19:55 (KST)`

## 현재 요청

- 요청 요약: TestCam 에디터에서 프로퍼티 변경 즉시 반영
- 승인된 범위: PHASE-021
- 범위 밖 항목: 씬 값 수정, Play Mode Exp 동작 변경, `_Shared`

## Phase 현황

| Phase | 상태 | 목표 | 기록 |
|---|---|---|---|
| 001 | 완료 | Movement 폴더/SO/속도 구간 | `phases/PHASE-001.md` |
| 002 | 완료 | 다중 레이 지면 판정 | `phases/PHASE-002.md` |
| 003 | 완료 | Turn+yaw, Push 가속도+Trigger | `phases/PHASE-003.md` |
| 004 | 완료 | Board 전용 타입 이름 변경 | `phases/PHASE-004.md` |
| 005 | 완료 | Push front/back 분리 재생 | `phases/PHASE-005.md` |
| 006 | 완료 | Push 재생을 HashDataSO로 교체 | `phases/PHASE-006.md` |
| 007 | 완료 | 슬립각 사이드 그립 | `phases/PHASE-007.md` |
| 008 | 완료 | 지면 기본 저항 (n) | `phases/PHASE-008.md` |
| 009 | 완료 | 기본 감속 속도 임계 | `phases/PHASE-009.md` |
| 010 | 완료 | Break에 속도 임계 | `phases/PHASE-010.md` |
| 011 | 완료 | Push를 BoardPushMove로 분리 | `phases/PHASE-011.md` |
| 012 | 완료 | Brake를 BoardBrakeMove로 분리 | `phases/PHASE-012.md` |
| 013 | 완료 | Space 홀드 점프/드리프트 뼈대 | `phases/PHASE-013.md` |
| 014 | 완료 | 회전 적용 앞속도 임계 | `phases/PHASE-014.md` |
| 015 | 완료 | 축 회전 테스트 컴포넌트 | `phases/PHASE-015.md` |
| 016 | 완료 | 축별 월드/로컬 회전 | `phases/PHASE-016.md` |
| 017 | 완료 | 이동 SO 모듈별 분리 | `phases/PHASE-017.md` |
| 018 | 완료 | SoundClipSO loop 시 random blend 필드 | `phases/PHASE-018.md` |
| 019 | 완료 | TestCam 보드 뒤 Exp 추적 | `phases/PHASE-019.md` |
| 020 | 완료 | Board 상태 AbstractVehicleState 이전 | `phases/PHASE-020.md` |
| 021 | 완료 | TestCam 에디터 즉시 반영 | `phases/PHASE-021.md` |

## 현재 재개 지점

- 마지막 완료 작업: PHASE-021
- 다음에 할 작업: 없음
- 사용자 승인이 필요한 사항: 없음
- 관련 파일: `Assets/_MemeberWorkspace/JTH/Test/Scripts/TestCam.cs`
- 알려진 문제 또는 위험: 에디트 모드에서 카메라를 손으로 옮기면 Update가 다시 붙임

## 검증 요약

- 수행한 검증: `validate_script` 에러 0, 콘솔 TestCam 에러 0
- 통과 여부: 컴파일 통과. 인스펙터 드래그 체감은 사용자 확인
- 아직 검증하지 못한 항목: 에디터에서 Distance/Angle 드래그
