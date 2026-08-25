# PHASE-013 — Space 홀드 점프/드리프트 입력 뼈대

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-20 13:00 (KST)`
- 사용자 승인: `GetModule로 필요한거 가지고 오고 CanJump/CanDrift, OnSpace/OnSpaceKeyChange, n초면 드리프트 그 전에 떼면 점프. 말한 뼈대만`

## 요청과 목표

- 원래 요청: Space 누름/뗌 이벤트로 홀드 시간에 따라 점프와 드리프트를 가른다. 조건 프로퍼티는 뼈대만.
- 이 Phase의 목표: 입력 이름 변경, Binder가 모듈을 가져와 홀드 분기만 수행.
- 완료 조건: 누름 후 n초 전에 떼면 점프, n초 넘으면 드리프트 시작, 드리프트 중 떼면 종료.

## 승인된 구현 범위

- 변경 예정 파일: `PlayerInputSO.cs`, `PlayerActionBinder.cs`(생성), `BoardJumpMove.cs`
- 구현 방법: `OnSpaceKeyChange(bool)`로 누름/뗌. Binder가 `GetModule<IJumpable>`, `GetModule<IDriftable>`. `CanJump`/`CanDrift` 프로퍼티로 호출 여부만 가름.
- 검증 방법: Unity 컴파일. 씬 부착과 Play Mode는 사용자.
- 명시적으로 제외한 항목: ActionState, `_Shared` Input Asset, 씬/프리팹, CanJump·CanDrift 상세 조건, Drift 애니

## 구현 결과

- 수행한 작업: PlayerInputSO 이벤트 변경. Binder 홀드 분기. JumpMove에서 Space 구독 제거.
- 생성·수정한 파일: `PlayerActionBinder.cs`, `PlayerInputSO.cs`, `BoardJumpMove.cs`, `_AgentLog`
- 계획과 달라진 점 및 이유: `_Shared`의 Jump 액션 이름은 공용 파일이라 그대로 두고, JTH 쪽에서 `OnSpace`로 감쌈.

## 학습 노트

- 전체 실행 흐름: Space started → 홀드 타이머. duration 전에 canceled → `CanJump`면 Jump 상태. duration 넘김 → `CanDrift`면 `DoDrift=true`. 그 뒤 canceled → `DoDrift=false`.
- 주요 클래스/메서드의 역할: `PlayerInputSO.OnSpaceKeyChange`가 누름/뗌. `PlayerActionBinder`가 시간과 프로퍼티만 보고 호출.
- UniTask 사용 위치와 이유: 없음. 홀드는 Update에서 누적.
- DOTween 사용 위치와 이유: 없음
- 중요한 구현 원리: 생성된 Input System 콜백 이름은 여전히 `OnJump`다. Binder는 그걸 `OnSpace`로 받아 누름/뗌만 밖으로 보낸다.
- 예외 상황과 대응: Binder가 Player에 없으면 점프/드리프트 입력이 없다.

## 검증

- 자동 검증: Unity 콘솔 컴파일
- Unity Editor 수동 확인 절차: Player에 `PlayerActionBinder` 추가, `driftHoldDuration` 설정, Space 탭/홀드.
- 결과: 코드 반영
- 검증하지 못한 항목: Play Mode, 씬 부착

## 다음 단계

- 남은 문제: `CanJump`/`CanDrift` 조건은 사용자 작성. Binder 씬 부착.
- 제안하는 다음 Phase: 없음
- 추가 승인이 필요한 사항: `_Shared` Jump → Space 이름 변경
