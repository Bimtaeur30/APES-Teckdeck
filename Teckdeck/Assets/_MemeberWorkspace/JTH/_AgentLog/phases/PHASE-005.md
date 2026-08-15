# PHASE-005 — Push front/back 클립 분리 재생

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-15 18:15 (KST)`
- 사용자 승인: `PushState 업데이트... 궁금한거 물어보고 구현해`

## 요청과 목표

- 부모 `Enter` 클립 재생을 쓰지 않고, IPushable의 Front/Back 클립을 입력 y로 나눠 재생한다. 루프이므로 입력이 바뀔 때만 클립을 바꾼다.
- 완료 조건: y 데드존이면 0으로 푸시 종료, 아니면 ±1로 front/back 재생.

## 승인된 구현 범위

- 변경 파일: `BoardPushState.cs`
- 제외: `_Shared`, 클립 에셋 이름 변경

## 구현 결과

- `base.Enter` 없음. `RenderClipIfNotPlaying`으로 클립 이름 해시 재생.
- 입력 변경 시에만 방향 재계산.

## 검증

- Play Mode: 전진 유지 시 front 루프, 뒤로 바꾸면 back으로 한 번만 전환, 손 떼면 속도 구간으로 나감.
- Animator 상태 이름이 클립 이름과 같아야 함 (`FRONT_PUSH` 등).
