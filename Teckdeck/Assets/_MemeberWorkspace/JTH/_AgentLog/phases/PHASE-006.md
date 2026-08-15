# PHASE-006 — Push 재생을 HashDataSO로 교체

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-15 18:22 (KST)`
- 사용자 승인: `실수로 클립을 넣어버렸네; HashData로 바꿔서 다시 해줘`

## 요청과 목표

- 원래 요청: IPushable의 Front/Back을 AnimationClip이 아니라 HashData로 바꾼다.
- 이 Phase의 목표: `PlayClip`에 Animator 상태 해시(`HashDataSO.HashValue`)를 넘기도록 연결한다.
- 완료 조건: Front/Back 슬롯이 `HashDataSO`이고, PushState가 `HashValue`로 재생한다.

## 승인된 구현 범위

- 변경 예정 파일: `IPushable.cs`, `BoardMovement.cs`, `BoardPushState.cs`, `PlayerMoveScene.unity`
- 구현 방법: 클립 필드 → `FrontPushHash`/`BackPushHash`. 씬에 기존 `FRONT_PUSH`/`BACK_PUSH` param 에셋을 연결한다.
- 검증 방법: Unity 컴파일, Inspector 슬롯 확인
- 명시적으로 제외한 항목: `_Shared`, HashData 에셋 내용 변경, Animator Controller 수정

## 구현 결과

- 수행한 작업: 클립 참조를 HashDataSO로 교체. `PlayClip(hash.HashValue, 0f, 0.1f)`.
- 생성·수정한 파일: `IPushable.cs`, `BoardMovement.cs`, `BoardPushState.cs`, `PlayerMoveScene.unity`
- 계획과 달라진 점 및 이유: 없음

## 학습 노트

- 전체 실행 흐름: Push 방향이 바뀌면 `HashDataSO.HashValue`로 `CrossFadeInFixedTime`한다. 클립 파일 이름이 아니라 Animator 상태 이름 해시가 필요하다.
- 주요 클래스/메서드의 역할: `HashDataSO`는 `HashName`을 `Animator.StringToHash`한 값을 보관한다. `IPushable`은 Front/Back 상태 해시를 노출한다.
- UniTask 사용 위치와 이유: 없음
- DOTween 사용 위치와 이유: 없음
- 중요한 구현 원리: `PlayClip`은 클립 에셋 해시가 아니라 Animator 상태 해시를 받는다. 그래서 클립을 넣으면 상태가 안 찾아진다.
- 예외 상황과 대응: HashData 슬롯이 비면 Assert. HashName이 컨트롤러 상태 이름과 달라야 전환이 실패한다.

## 검증

- 자동 검증: Unity 컴파일
- Unity Editor 수동 확인 절차: BoardMovement에 FRONT_PUSH/BACK_PUSH param이 들어있는지, Play Mode에서 앞/뒤 푸시 전환
- 결과: 코드/씬 연결 완료
- 검증하지 못한 항목: Play Mode 실기 확인

## 다음 단계

- 남은 문제: Play Mode
- 제안하는 다음 Phase: 없음
- 추가 승인이 필요한 사항: 없음
