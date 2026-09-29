# PHASE-015 — 축 회전 테스트 컴포넌트

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-23 14:35 (KST)`
- 사용자 승인: `만들어`

## 요청과 목표

- 원래 요청: 인스펙터에서 Vector3 축 속도, 월드/로컬, 가속도/등속도를 바꿔가며 회전을 확인하는 MonoBehaviour
- 이 Phase의 목표: `JTH/Test/Scripts`에 테스트용 회전체 컴포넌트 추가
- 완료 조건: 스크립트 컴파일, 인스펙터 필드 노출, Play Mode에서 회전 확인 가능

## 승인된 구현 범위

- 변경 예정 파일: `Test/Scripts/TestAxisRotator.cs`, `PROGRESS.md`, `PHASE-015.md`
- 구현 방법: `Update`에서 `transform.Rotate`, 가속도 시 내부 각속도 누적
- 검증 방법: Unity 컴파일, 콘솔 에러 없음
- 명시적으로 제외한 항목: Player/Board, 씬, Rigidbody 토크, `_Shared`

## 구현 결과

- 수행한 작업: `TestAxisRotator` 추가. 등속도는 `angularRates` 그대로, 가속도는 `_currentAngularVelocity`에 누적.
- 생성·수정한 파일:
  - `Assets/_MemeberWorkspace/JTH/Test/Scripts/TestAxisRotator.cs`
  - `Assets/_MemeberWorkspace/JTH/_AgentLog/PROGRESS.md`
  - `Assets/_MemeberWorkspace/JTH/_AgentLog/phases/PHASE-015.md`
- 계획과 달라진 점 및 이유: 없음. 가속도 해제 시 `OnValidate`에서 누적 속도를 0으로 되돌림.

## 학습 노트

- 전체 실행 흐름: Play → `OnEnable`에서 누적 속도 0 → 매 프레임 `Update`에서 이번 각속도 결정 → `transform.Rotate`
- 주요 클래스/메서드의 역할:
  - `angularRates`: 등속도면 deg/s, 가속도면 deg/s²
  - `useWorldSpace`: `Space.World` / `Space.Self`
  - `AccumulateAngularVelocity`: 각속도를 시간에 따라 쌓음
- UniTask 사용 위치와 이유: 사용 안 함. 매 프레임 회전은 Update가 맞음
- DOTween 사용 위치와 이유: 사용 안 함. 지속 회전 확인용이라 트윈이 필요 없음
- 중요한 구현 원리: `Rotate(eulerAngles * dt)`는 한 프레임 동안의 각변위. 가속도면 v += a*dt 후 그 v로 돈다.
- 예외 상황과 대응: 값이 0이면 회전 생략. 컴포넌트 끄거나 등속도로 바꾸면 누적 속도 리셋.

## 검증

- 자동 검증: `validate_script` 에러 0, 콘솔 에러 0
- Unity Editor 수동 확인 절차: 큐브 등에 붙이고 Play. `(0, 90, 0)` 등속도/월드, 로컬, 가속도를 바꿔 본다.
- 결과: 컴파일 통과. Play Mode는 사용자 확인
- 검증하지 못한 항목: Play Mode에서 실제 회전감

## 다음 단계

- 남은 문제: 없음
- 제안하는 다음 Phase: 없음
- 추가 승인이 필요한 사항: 없음
