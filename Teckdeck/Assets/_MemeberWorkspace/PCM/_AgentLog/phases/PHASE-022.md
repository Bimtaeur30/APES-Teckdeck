# PHASE-022 — EnemyBT를 Editor API로 적용

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-07 23:45 (UTC+9)`
- 사용자 승인: `BT에 적용된 게 없다`는 보고에 대한 수정

## 원인

- YAML로 `EnemyBT.asset`을 직접 고쳤다.
- Unity에서 Behavior 그래프 창이 열려 있으면, 창이 저장하거나 리임포트할 때 메모리上的 authoring 그래프로 디스크를 덮어쓴다.
- 그래서 디스크 수정이 사라지고, 에디터에도 변화가 안 보였다.
- `UseSkillAction.cs`만 남아 있었다.

## 구현 결과

- `Editor/EnemyBTMeleeLoopBuilder.cs`를 추가했다.
- 메뉴 `PCM/Apply Melee Loop To EnemyBT`가 authoring API로 On Start / ATTACK을 고치고 runtime 그래프를 다시 만든다.
- 컴파일 후 ATTACK이 Stop만 있거나 On Start Repeat가 켜져 있으면 자동으로 한 번 시도한다.

## 사용자가 할 일

1. EnemyBT Behavior 창을 닫는다.
2. 컴파일이 끝난 뒤 메뉴 `PCM/Apply Melee Loop To EnemyBT`를 실행한다. 자동 적용이 됐다면 Console에 로그가 뜬다.
3. EnemyBT를 다시 연다.
