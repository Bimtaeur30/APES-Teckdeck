# PHASE-021 — TestCam 에디터 즉시 반영

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-25 19:55 (KST)`
- 사용자 승인: `에디터에서도 프로퍼티 바꾸면 바로 적용되게 해줘`

## 요청과 목표

- 원래 요청: 에디터에서 인스펙터 프로퍼티를 바꾸면 카메라에 바로 적용
- 이 Phase의 목표: Play Mode가 아니어도 distance/angle 등이 씬 카메라에 즉시 반영
- 완료 조건: 에디트 모드에서 인스펙터 값 변경 시 카메라 Transform이 갱신됨. Play Mode Exp 추적은 유지

## 승인된 구현 범위

- 변경 예정 파일: `Test/Scripts/TestCam.cs`, `PROGRESS.md`, `PHASE-021.md`
- 구현 방법: `[ExecuteAlways]`, 에디트 모드에서는 Exp 없이 즉시 포즈. `OnValidate`에서 `QueuePlayerLoopUpdate`로 인스펙터 변경 직후 Update 실행
- 검증 방법: Unity 컴파일, 콘솔 에러 없음
- 명시적으로 제외한 항목: 씬 값 수정, Play Mode Exp 동작 변경, `_Shared`

## 구현 결과

- 수행한 작업: 포즈 적용을 `ApplyCamera`로 분리. 에디트 모드는 보드 현재 yaw로 즉시 배치. 인스펙터 변경 시 플레이어 루프를 한 번 돌림.
- 생성·수정한 파일:
  - `Assets/_MemeberWorkspace/JTH/Test/Scripts/TestCam.cs`
  - `Assets/_MemeberWorkspace/JTH/_AgentLog/PROGRESS.md`
  - `Assets/_MemeberWorkspace/JTH/_AgentLog/phases/PHASE-021.md`
- 계획과 달라진 점 및 이유: 없음. 에디트 모드에서는 Exp를 쓰지 않음. 시간 없이 값을 확인해야 해서.

## 학습 노트

- 전체 실행 흐름: 인스펙터 값 변경 → `OnValidate` → 에디터 플레이어 루프 예약 → `Update` → `ApplyCamera`
- 주요 클래스/메서드의 역할:
  - `[ExecuteAlways]`: Play가 아니어도 `Update`가 돈다
  - `OnValidate`: 직렬화 필드가 바뀔 때 호출. Transform을 여기서 직접 안 바꾸고 루프만 깨움
  - 에디트 모드 `Update`: `_yaw` Exp 없이 `playerTrm.eulerAngles.y`로 바로 붙임
- UniTask 사용 위치와 이유: 사용 안 함
- DOTween 사용 위치와 이유: 사용 안 함
- 중요한 구현 원리: `OnValidate` 안에서 `SetPositionAndRotation`을 직접 하면 Unity가 경고를 낸다. 그래서 `QueuePlayerLoopUpdate`로 Update 타이밍에 적용한다. Play 중에는 기존 Exp 경로를 그대로 탄다.
- 예외 상황과 대응: 에디트 모드에서 카메라를 손으로 옮기면 다음 Update에 다시 붙는다. 씬 뷰가 안 보이면 Update가 안 돌 수 있어 OnValidate에서 루프를 깨운다.

## 검증

- 자동 검증: `validate_script` 에러 0. 콘솔 TestCam 에러 0
- Unity Editor 수동 확인 절차: Play를 끄고 TestCam의 Distance/Angle을 드래그. 카메라가 보드 뒤에서 바로 움직이는지 확인
- 결과: 컴파일 통과. 인스펙터 드래그는 사용자 확인
- 검증하지 못한 항목: 인스펙터 드래그 체감

## 다음 단계

- 남은 문제: 없음
- 제안하는 다음 Phase: 없음
- 추가 승인이 필요한 사항: 없음
