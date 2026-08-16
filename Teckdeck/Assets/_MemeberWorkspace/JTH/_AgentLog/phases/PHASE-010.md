# PHASE-010 — Break에 속도 임계 추가

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-16 03:38 (KST)`
- 사용자 승인: `BreakAngle 넘어감 && Speed 임계값도 넘어감까지 해야 -n으로`

## 요청과 목표

- `-n`(KineticDecel)은 슬립각이 BreakAngle 이상 **이고** 면 위 속도가 BreakSpeed 초과일 때만.
- 각만 크고 느리면 Exp.

## 구현 결과

- SO `BreakSpeed` 기본 8.
- Snap 미만은 기존 스냅. 그 외 `isBroken`일 때만 n, 아니면 Exp.

## 검증

- 옆착지여도 느리면 Exp로 붙는지. 빠르고 각이 크면 n으로 미끄러지는지.
