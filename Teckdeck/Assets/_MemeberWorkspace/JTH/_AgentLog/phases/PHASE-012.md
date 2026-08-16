# PHASE-012 — Brake를 IBrakable/BoardBrakeMove로 분리

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-16 06:45 (KST)`
- 사용자 승인: `Brake도 걍 IBrakable로 빼버리고 네가 Push처럼 또 분리해줘`

## 요청과 목표

- Push와 같이 브레이크 힘을 별도 모듈로 분리. Idle 제동(리뷰 5번)은 제외.

## 구현 결과

- 생성: `IBrakable`, `BoardBrakeMove`
- 제거: `IControlMovement.DoBrake` / `IsBraking`, `BoardMovement` 브레이크 감속
- `AbstractBoardState`가 `IBrakable`을 들고 `ShouldBrake`로 Push/Brake 분기
- 씬 `Movement/BrakeMove`
- Push에서 Brake로 갈 때 `return` 추가 (클립이 이어서 재생되던 구멍)

## 검증

- Inspector에서 BrakeMove에 SO/RB. Play Mode 제동 감속은 애니 이벤트 이후.
