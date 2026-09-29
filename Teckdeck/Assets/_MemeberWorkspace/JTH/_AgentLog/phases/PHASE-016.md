# PHASE-016 — 축별 월드/로컬 회전

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-23 14:37 (KST)`
- 사용자 승인: `z축으로는 world 기준으로 돌리고 x축으로는 로컬로 돌릴 수 있게 bool도 나눠줘`

## 요청과 목표

- 원래 요청: 축마다 월드/로컬을 따로 고를 수 있게 bool 분리
- 이 Phase의 목표: `useWorldSpace`를 X/Y/Z bool로 나누고 축별로 `Rotate`
- 완료 조건: 컴파일, 인스펙터에 축별 토글 노출

## 승인된 구현 범위

- 변경 예정 파일: `Test/Scripts/TestAxisRotator.cs`, `PROGRESS.md`, `PHASE-016.md`
- 구현 방법: 축마다 `transform.Rotate(axis, angle, Space)`
- 검증 방법: Unity 컴파일, 콘솔 에러 없음
- 명시적으로 제외한 항목: 가속도 bool 분리, Player/Board, 씬, `_Shared`

## 구현 결과

- 수행한 작업: `useWorldSpace`를 `useWorldSpaceX/Y/Z`로 분리. Update에서 X→Y→Z 순으로 각각 회전.
- 생성·수정한 파일:
  - `Assets/_MemeberWorkspace/JTH/Test/Scripts/TestAxisRotator.cs`
  - `Assets/_MemeberWorkspace/JTH/_AgentLog/PROGRESS.md`
  - `Assets/_MemeberWorkspace/JTH/_AgentLog/phases/PHASE-016.md`
- 계획과 달라진 점 및 이유: Y도 같이 분리. 한 축만 나누면 나머지 축은 다시 묶이게 됨.

## 학습 노트

- 전체 실행 흐름: 이번 각속도를 구한 뒤 X, Y, Z를 따로 `Rotate`
- 주요 클래스/메서드의 역할:
  - `useWorldSpaceX/Y/Z`: 해당 축을 월드로 돌릴지, 로컬로 돌릴지
  - `RotateAxis`: 한 축만 `Space.World` 또는 `Space.Self`로 회전
- UniTask 사용 위치와 이유: 사용 안 함
- DOTween 사용 위치와 이유: 사용 안 함
- 중요한 구현 원리: 축마다 Space가 다르면 한 번의 `Rotate(Vector3)`로는 불가능. `Vector3.right` + `Space.Self`는 로컬 X, `Vector3.forward` + `Space.World`는 월드 Z.
- 예외 상황과 대응: 해당 축 각도가 0이면 생략. 예전 `useWorldSpace` 직렬화 값은 사라짐.

## 검증

- 자동 검증: `validate_script` 에러 0, 콘솔 에러 0
- Unity Editor 수동 확인 절차: X 로컬 + Z 월드로 Play. `Use World Space X` 끄고 `Use World Space Z` 켜기.
- 결과: 컴파일 통과. Play Mode는 사용자 확인
- 검증하지 못한 항목: Play Mode에서 혼합 회전감

## 다음 단계

- 남은 문제: 없음
- 제안하는 다음 Phase: 없음
- 추가 승인이 필요한 사항: 없음
