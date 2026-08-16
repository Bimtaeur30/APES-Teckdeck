# PHASE-011 — Push를 BoardPushMove로 분리

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-16 05:54 (KST)`
- 사용자 승인: `Movement너무 길어져서 Push는 나눠야할 것 같아서 JumpMove처럼 나눠줘`

## 요청과 목표

- Push 힘/해시를 `BoardJumpMove`와 같은 별도 모듈로 분리.

## 구현 결과

- 생성: `BoardPushMove.cs` (`IModule`, `IPushable`)
- 수정: `BoardMovement`에서 IPushable 제거. `BoardPushState`는 `GetModule<IPushable>()`.
- 씬: Movement 아래 `PushMove` 자식, 기존 Front/Back hash 슬롯 이동.

## 학습 노트

- Jump처럼 상태/트리거는 FSM, 힘 적용은 모듈 FixedUpdate.
- UniTask/DOTween: 없음

## 검증

- Inspector에서 PushMove에 hash/SO/RB가 들어있는지. Play Mode 푸시 가속.
