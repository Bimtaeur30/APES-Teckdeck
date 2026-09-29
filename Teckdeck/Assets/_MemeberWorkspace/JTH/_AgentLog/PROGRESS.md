# 작업 진행 현황

## 사용자

- 이름/이니셜: `JTH`
- 작업폴더: `Assets/_MemeberWorkspace/JTH/`
- 마지막 갱신: `2026-08-25 19:45 (KST)`

## 현재 요청

- 요청 요약: AbstractBoardState 삭제 후 보드 상태를 AbstractVehicleState + 필드/GetModule로 고침
- 승인된 범위: PHASE-020
- 범위 밖 항목: 상태 전환 로직, 씬/프리팹, `_Shared`

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

## 현재 재개 지점

- 마지막 완료 작업: PHASE-020
- 다음에 할 작업: 없음
- 사용자 승인이 필요한 사항: 없음
- 관련 파일: `Assets/_MemeberWorkspace/JTH/Scripts/Vehicles/Board/FSM/States/`
- 알려진 문제 또는 위험: `BoardController`는 모듈이 아니라 owner 캐스트. 보드 상태가 아닌 컨트롤러에서 생성되면 실패

## 검증 요약

- 수행한 검증: 6개 상태 `validate_script` 에러 0, Unity 컴파일 후 콘솔 `AbstractBoardState` 에러 0
- 통과 여부: 컴파일 통과
- 아직 검증하지 못한 항목: Play Mode 상태 전환 체감
