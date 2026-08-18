# PHASE-009 — 기본 감속 속도 임계

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-16 03:10 (KST)`
- 사용자 승인: `기본 감속에도 임계값을 넣어서 속도가 그 이하일 때 감속하게 해줘 그리고 테스트하게 그 이하일 때 디버그 찍어줘`

## 요청과 목표

- 면 위 속도가 `BaseDecelThreshold` 이하일 때만 `BaseDecel` 적용. 그 구간에서 콘솔 로그.

## 구현 결과

- SO `BaseDecelThreshold` 기본 5. `ApplyBaseResistance`에서 초과 시 return. 이하일 때 `Debug.Log($"BaseDecel {speed}")`.
- 테스트용 로그라 나중에 제거하면 됨.

## 검증

- Play Mode: 임계 위에서 로그 없음. 내려가면 `BaseDecel` 로그와 함께 -n 감속.
