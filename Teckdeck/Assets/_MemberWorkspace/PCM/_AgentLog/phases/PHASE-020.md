# PHASE-020 — EnemyBT 블랙보드 null 변수 복구

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-06 22:21 (UTC+9)`
- 사용자 승인: `BehaviorGraphAgentEditor NullReferenceException을 수정해 달라는 요청`

## 구현 결과

- 블랙보드 변수 목록에 들어 있던 빈 참조 `rid: -2`를 실제 변수로 바꿨다.
- 타입 어셈블리를 `Assembly-CSharp`에서 `PCM_assembly`로 바꿨다. 대상은 `StateCommands`, `CommandChange`, `Enemy.AbstractEnemy`다.

## 학습 노트

- 인스펙터는 변수 목록을 돌며 `variable.GUID`를 읽는다. 목록 칸이 null이면 그 줄에서 예외가 난다.
- 스크립트를 `PCM_assembly`로 옮긴 뒤에도 그래프가 예전 어셈블리 이름을 들고 있으면 Unity가 그 변수를 빈 칸으로 저장한다.

## 검증

- 변수 목록 세 곳에 `rid: -2`가 없는 것을 확인했다.
- Unity 인스펙터는 직접 열지 못했다.
