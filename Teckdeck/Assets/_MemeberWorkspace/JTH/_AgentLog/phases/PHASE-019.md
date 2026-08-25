# PHASE-019 — TestCam 보드 뒤 Exp 추적

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-25 18:05 (KST)`
- 사용자 승인: `진행해`

## 요청과 목표

- 원래 요청: Update에서 특정 각도로 보드 뒤를 보고, 딱딱하지 않게 Exp. Pitch는 Angle 고정, Roll은 SideVelocity, Yaw는 Exp 추적. Yaw 오차가 크면 Decel 배열 2번째. 전부 묶지 않고 Decel만 배열
- 이 Phase의 목표: `TestCam`을 보드 뒤 추적 카메라로 구현
- 완료 조건: 스크립트 컴파일, 인스펙터에 distance / decel[] / angle / yawSwitchAngle / rollPerSideVel / playerTrm 노출

## 승인된 구현 범위

- 변경 예정 파일: `Test/Scripts/TestCam.cs`, `PROGRESS.md`, `PHASE-019.md`
- 구현 방법: Pitch+Yaw로 뒤 위치, Yaw는 `LerpAngle` + `1 - Exp(-decel * dt)`, Decel은 yaw 오차로 `[0]`/`[1]` 전환, Roll은 사이드 속도 * 배율
- 검증 방법: Unity 컴파일, 콘솔 에러 없음
- 명시적으로 제외한 항목: 씬/프리팹, Cinemachine, Board/Player 이동, `_Shared`

## 구현 결과

- 수행한 작업: `TestCam.Update`에서 보드 뒤 위치/회전을 매 프레임 적용. Pitch 고정, Yaw Exp, Roll은 사이드 속도, Decel만 배열.
- 생성·수정한 파일:
  - `Assets/_MemeberWorkspace/JTH/Test/Scripts/TestCam.cs`
  - `Assets/_MemeberWorkspace/JTH/_AgentLog/PROGRESS.md`
  - `Assets/_MemeberWorkspace/JTH/_AgentLog/phases/PHASE-019.md`
- 계획과 달라진 점 및 이유: 없음. Roll은 Exp 없이 사이드 속도에 바로 붙임.

## 학습 노트

- 전체 실행 흐름: Play → `Awake`에서 Rigidbody 캐시 → 매 프레임 `Update`에서 목표 Yaw를 구하고, 오차에 따라 Decel을 고른 뒤 Exp로 `_yaw`를 붙인 다음 위치/회전을 씀
- 주요 클래스/메서드의 역할:
  - `angle`: Pitch. 값이 그대로 카메라 X 각도가 되고 시간에 따라 안 변함
  - `decel[]`: `[0]` 평소, `[1]`은 `|현재 yaw - 보드 yaw|`가 `yawSwitchAngle`보다 클 때
  - `rollPerSideVel`: `Dot(속도, 보드.right)`에 곱해서 Roll(Z)을 만듦. 부호를 음수로 두면 기울기 방향이 반대
  - `_hasYaw`: 첫 프레임에 현재 보드 yaw로 스냅해서 0도에서 크게 도는 걸 막음
- UniTask 사용 위치와 이유: 사용 안 함. 매 프레임 추적은 Update가 맞음
- DOTween 사용 위치와 이유: 사용 안 함. Exp 감쇠는 Lerp 한 줄이면 됨
- 중요한 구현 원리:
  - `1 - Exp(-decel * dt)`는 프레임레이트와 무관하게 목표로 붙는 비율. `BoardMovement`의 turn과 같은 식
  - `LerpAngle`은 359→1처럼 0/360 경계를 짧은 쪽으로 돈다
  - 위치는 `Euler(pitch, yaw, 0) * back * distance`라서 Roll이 카메라를 옆으로 밀지 않음. Roll은 회전만
- 예외 상황과 대응:
  - `playerTrm` 없거나 `decel`이 비면 Update를 건너뜀
  - `decel` 길이가 1이면 오차가 커도 `[0]`만 씀
  - Rigidbody가 `playerTrm`에 없으면 Roll은 0

## 검증

- 자동 검증: `validate_script` 에러 0. 콘솔 에러 0. 에디터 idle, compiling 아님
- Unity Editor 수동 확인 절차:
  1. TestCam 인스펙터에서 `Distance`를 0이 아니게 (카메라는 이미 대략 거리 10, pitch 40에 있음)
  2. `Angle`을 원하는 pitch로 (예: 40)
  3. `Decel` Size 2. `[0]`은 천천히, `[1]`은 더 빠르게 잡아채게
  4. `Yaw Switch Angle` 넘기며 급회전
  5. `Roll Per Side Vel`을 작은 값으로 넣고 드리프트 시 기울기 확인
- 결과: 컴파일 통과. Play Mode는 사용자 확인
- 검증하지 못한 항목: Play Mode 추적감. 씬 값은 승인 범위 밖이라 수정하지 않음

## 다음 단계

- 남은 문제: 씬 TestCam의 `distance`/`angle`이 0. Play 전에 인스펙터에서 넣어야 뒤가 보임
- 제안하는 다음 Phase: 없음
- 추가 승인이 필요한 사항: 없음
