# PHASE-016 — EnemyBT Null 타입 연결

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-05 22:42 (UTC+9)`
- 사용자 승인: `어떻게 수정하는지 모르니 수정해 달라는 요청`

## 요청과 목표

- 원래 요청: EnemyBT의 Null 타입을 고친다
- 완료 조건: 그래프가 현재 클래스 이름을 가리킨다

## 구현 결과

- `Enemy.BT.EnemyCommands, AssemblyStageDefinition`를 `Enemy.BT.StateCommands, Assembly-CSharp`로 바꿨다.
- `EnemyCommandChange` 타입을 `CommandChange`로 바꿨다.
- `AbstractEnemy` 타입을 `Enemy.AbstractEnemy`로 바꿨다.
- `BtVar.StateChannel`은 블랙보드 변수 이름 `EnemyCommandChange`를 가리킨다.

## 학습 노트

- 그래프는 클래스 이름과 어셈블리 이름을 문자열로 저장한다. 이름을 바꾸면 그 문자열이 Null이 된다.
- 블랙보드 변수 이름 `EnemyCommands`, `EnemyCommandChange`는 타입 이름과 별개라 그대로 뒀다.

## 검증

- 에셋에서 예전 타입 문자열이 없는 것을 확인했다.
- Unity 그래프 창은 직접 열지 못했다.
