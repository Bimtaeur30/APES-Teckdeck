# PHASE-017 — 이동 SO 모듈별 분리

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-24 17:20 (KST)`
- 사용자 승인: `나눠줘`

## 요청과 목표

- 원래 요청: 모듈이 `BoardMovementSO`를 같이 참조하는 걸 나누고, SO 스크립트는 모든 탈것이 쓰게, 보드는 `BoardDriftSO`처럼 에셋 이름으로 구분
- 이 Phase의 목표: 모듈마다 자기 SO만 참조. 공용 클래스 + 보드 에셋 이름
- 완료 조건: 예전 `BoardMovementSO` 타입 없음. 각 모듈이 해당 SO를 참조. 씬 슬롯이 보드 에셋을 가리킴. 컴파일 에러 없음

## 승인된 구현 범위

- 변경 예정 파일: `Scripts/Player/Movement/SO/`, 모듈 스크립트, `GameModules/Player/Board/Movement/` 에셋, `PlayerMoveScene`, `_AgentLog`
- 구현 방법: 공용 SO 클래스 5개. 보드 에셋은 `BoardXxxSO`. 예전 `BoardMovementSO.cs` 제거
- 검증 방법: Unity `validate_script`, 타입 검색, 콘솔 에러, 씬 GUID
- 명시적으로 제외한 항목: DriftMove 씬 부착, CanDrift/DoDrift 물리 버그, `_Shared`

## 구현 결과

- 수행한 작업: 공용 SO와 보드 에셋이 이미 나뉘어 있어 참조·컴파일을 확인하고 예전 타입 부재를 검증함
- 생성·수정한 파일:
  - `Scripts/Player/Movement/SO/MovementSO.cs`
  - `Scripts/Player/Movement/SO/PushMoveSO.cs`
  - `Scripts/Player/Movement/SO/BrakeMoveSO.cs`
  - `Scripts/Player/Movement/SO/JumpMoveSO.cs`
  - `Scripts/Player/Movement/SO/DriftMoveSO.cs`
  - `GameModules/Player/Board/Movement/BoardMovementSO.asset` (클래스 `MovementSO`로 축소)
  - `GameModules/Player/Board/Movement/BoardPushSO.asset`
  - `GameModules/Player/Board/Movement/BoardBrakeSO.asset`
  - `GameModules/Player/Board/Movement/BoardJumpSO.asset`
  - `GameModules/Player/Board/Movement/BoardDriftSO.asset`
  - `PushMove` / `BrakeMove` / `JumpMove` / `DriftMove` / `BoardMovement`
  - `Scenes/PlayerMoveScene.unity` (모듈별 에셋 슬롯)
  - `_AgentLog/PROGRESS.md`, `phases/PHASE-017.md`
- 계획과 달라진 점 및 이유: 없음. 드리프트 회전 배율은 계획대로 `MovementSO`에 유지

## 학습 노트

- 전체 실행 흐름: 각 모듈이 Awake `Initialize` 때 자기 SO만 읽고, 물리 값은 그 에셋에서 온다
- 주요 클래스/메서드의 역할:
  - `PushMoveSO` 등: 탈것 공용 데이터 모양
  - `BoardPushSO.asset` 등: 보드 전용 수치. 다른 탈것은 같은 클래스로 `BikePushSO`처럼 만들면 됨
  - `MovementSO.DriftMaxTurnSpeed` / `DriftDecay`: 드리프트 중 회전만. 슬립 물리는 `DriftMoveSO`
- UniTask 사용 위치와 이유: 사용 안 함
- DOTween 사용 위치와 이유: 사용 안 함
- 중요한 구현 원리: 스크립트 이름은 기능, 에셋 이름은 탈것. `StoppedSpeed`는 브레이크 판정과 속도 구간이 둘 다 써서 `BrakeMoveSO`와 `MovementSO`에 각각 둔다
- 예외 상황과 대응: `DriftMove`는 씬에 아직 없음. `BoardDriftSO`는 만들어 뒀으니 붙일 때 그 에셋을 넣으면 된다

## 검증

- 자동 검증: 관련 스크립트 `validate_script` 에러 0. 프로젝트에 `BoardMovementSO` 타입 0개, `DriftMoveSO` 1개. 콘솔 에러 0
- Unity Editor 수동 확인 절차: Player 자식 Jump/Push/Brake/BoardMovement Inspector에서 각각 BoardJumpSO / BoardPushSO / BoardBrakeSO / BoardMovementSO가 들어갔는지 확인
- 결과: 컴파일 통과. 씬 Jump/Push/Brake/Movement 슬롯은 새 에셋 GUID
- 검증하지 못한 항목: Play Mode 체감. DriftMove 씬 부착

## 다음 단계

- 남은 문제: DriftMove 오브젝트가 씬에 없음. CanDrift가 항상 true, DoDrift가 시작 조건에만 쓰임
- 제안하는 다음 Phase: DriftMove 씬 부착 + BoardDriftSO 할당
- 추가 승인이 필요한 사항: 없음
