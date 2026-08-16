# PHASE-008 — 지면 기본 저항 (n)

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-16 03:03 (KST)`
- 사용자 승인: `기본적으로 -n하는 저항도 추가해야할듯. 저항을 추가해줘`

## 요청과 목표

- 원래 요청: linearDamping 대신(추가로) 면 위 속도를 초당 n만큼 줄이는 기본 저항.
- 이 Phase의 목표: 점프가 아닐 때 평면 속도를 `MoveTowards(0, BaseDecel * dt)`로 줄인다.
- 완료 조건: SO `BaseDecel`로 튜닝 가능. 점프/수직 속도는 안 건드림.

## 승인된 구현 범위

- 변경 파일: `BoardMovement.cs`, `BoardMovementSO.cs`, `BoardMovementSO.asset`
- 제외: `_Shared`, 씬 `linearDamping` 값 변경, 공기저항 Exp

## 구현 결과

- `ApplyBaseResistance`: 사이드 그립 다음, 지면에서만. 진행 방향 전체(앞+옆)를 n만큼 줄임.
- 기본값 `BaseDecel = 2`. 튜닝은 사용자.

## 학습 노트

- `linearDamping`은 속도에 비례(Exp). `BaseDecel`은 일정량(쿨롱). 둘 다 켜면 저항이 겹친다.
- UniTask/DOTween: 없음

## 검증

- Play Mode에서 푸시 없이 평지 활주가 일정하게 느려지는지. 점프 중 낙하가 안 느려지는지.
- 씬 Rigidbody `linearDamping`은 지금 0.5라서, 기본 저항만 보려면 0으로 낮추면 된다.
