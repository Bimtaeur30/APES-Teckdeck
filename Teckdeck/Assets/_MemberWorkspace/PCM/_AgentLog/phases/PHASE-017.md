# PHASE-017 — Enemy 칸을 AbstractEnemy에 연결

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-05 22:55 (UTC+9)`
- 사용자 승인: `Enemy는 AbstractEnemy로 하면 된다는 요청`

## 구현 결과

- `Enemy find Target` 노드의 Enemy 칸이 블랙보드 변수 `AbstractEnemy`를 가리키게 연결했다.
- 같은 변수를 작성용 블랙보드와 런타임 블랙보드에 추가했다. 타입은 `Enemy.AbstractEnemy`다.

## 검증

- 에셋에서 링크 rid와 변수 이름이 같은지 확인했다.
- Unity 그래프 창은 직접 열지 못했다.
