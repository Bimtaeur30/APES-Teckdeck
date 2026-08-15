# PHASE-004 — Board 전용 타입 이름 변경

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-15 17:00 (KST)`
- 사용자 승인: `진행해 일단 내가 Board 전용인건 따로 Board 폴더 만들어서 뺐음`

## 요청과 목표

- 원래 요청: 보드 본체는 Board로 이름 변경. 입력은 Player 유지. 사용자가 Board 폴더를 이미 분리함.
- 이 Phase의 목표: Board 폴더 안의 `Player*` 타입·네임스페이스를 `Board*`로 맞추고, FSM 에셋 className과 씬 identifier를 갱신한다.
- 완료 조건: 보드 상태/이동 클래스가 `Board*`이고, `PlayerController`/`PlayerInputSO`는 그대로다.

## 승인된 구현 범위

- 변경 예정 파일: `Scripts/Player/Board/**`, 이동 인터페이스, FSM 에셋, PlayerMoveScene
- 구현 방법: 파일+meta 이름 유지 GUID로 변경. 네임스페이스 `JTH.Board.*`. Player 입력/컨트롤러는 유지.
- 검증 방법: Unity 컴파일
- 명시적으로 제외한 항목: `PlayerController`, `PlayerInputSO`, Input Actions, `_Shared`, GameModules 폴더 이름

## 구현 결과

- 수행한 작업: Board 폴더의 Movement/FSM 타입을 Board*로 변경. 인터페이스 네임스페이스를 `JTH.Board.Movement`로 맞춤. 상태 에셋 className과 씬 identifier 갱신.
- 생성·수정한 파일:
  - 이름 변경: `BoardMovement`, `BoardMovementSO`, `BoardState`, `BoardIdle/Jump/Push/Ride/Tuck/BrakeState`
  - 수정: `AbstractBoardState`, `AbstractPlayerState`, `PlayerController`, `IControlMovement`, `IJumpable`, `IPushable`, `IGroundChecker`, `BoardTrigger`, `BoardGroundChecker`
  - 수정: FSM state assets, `PlayerMovementSO.asset` identifier, `PlayerMoveScene.unity`
- 계획과 달라진 점 및 이유: `PlayerState` 부분 문자열이 `AbstractPlayerState`를 깨서, `AbstractBoardState` 상속을 `AbstractPlayerState`로 복구했다.

## 학습 노트

- 전체 실행 흐름: Player는 입력과 ModuleOwner. Board 폴더는 보드 물리·상태. FSM 에셋은 문자열 클래스 이름으로 상태를 만들기 때문에 이름 변경 시 `className`도 같이 바꿔야 한다.
- 주요 클래스/메서드의 역할:
  - `PlayerController` + `PlayerInputSO`: 사람 입력
  - `BoardMovement` / `Board*State`: 보드
- UniTask/DOTween: 변경 없음.
- 중요한 구현 원리: Unity 컴포넌트 참조는 스크립트 GUID(.meta)를 따라가므로 파일 이름과 클래스 이름을 같이 바꾸고 meta GUID는 유지한다.
- 예외 상황과 대응: 일괄 치환에서 `PlayerState` ⊂ `AbstractPlayerState`. 긴 이름부터 바꾸거나 단어 경계를 써야 한다.

## 검증

- 자동 검증: csproj에 새 Board 경로 반영. Unity MCP는 리로드 중 타임아웃.
- Unity Editor 수동 확인 절차:
  1. 콘솔 에러 없는지
  2. Visual의 BoardTrigger, Movement의 BoardMovement 슬롯이 안 비었는지
  3. State list의 Idle/Jump/Push 클래스가 Board*State인지
- 결과: 에디터 재연결 전 컴파일은 사용자가 확인
- 검증하지 못한 항목: Play Mode

## 다음 단계

- 남은 문제: `GameModules/Player/Movement/PlayerMovementSO.asset` 파일 이름은 그대로. Brake enum 항목은 기존에도 없음.
- 제안하는 다음 Phase: 없음 (이름 변경 요청 범위 완료)
- 추가 승인이 필요한 사항: GameModules/Player 폴더 이름, PlayerController를 Board로 옮길지
