# PHASE-015 — EnemyBT가 열리게 수정

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-05 20:40 (UTC+9)`
- 사용자 승인: `EnemyBT가 안 열리는 문제를 해결해 달라는 요청`

## 요청과 목표

- 원래 요청: EnemyBT 그래프가 열리게 한다
- 원인: 그래프가 `AbstractEnemy`를 `AssemblyStageDefinition`에서 찾는다. 클래스는 `Assembly-CSharp`로 옮겨져 타입이 없다고 표시된다. 스크립트 컴파일 오류가 있으면 그 어셈블리에도 타입이 없다.
- 완료 조건: 그래프의 AbstractEnemy 참조가 `Assembly-CSharp`를 가리키고, `AbstractEnemy`가 컴파일된다

## 구현 결과

- `GameModule/EnemyBT.asset`의 AbstractEnemy 직렬화 타입 3곳을 `Assembly-CSharp`로 바꿨다.
- `Enemy.BT.EnemyCommands` 참조는 `AssemblyStageDefinition`에 그대로 뒀다.
- `AbstractEnemy`가 이미 쓰고 있던 없는 타입을 `Scripts/Enemy/EnemyBtTypes.cs`에 최소 선언으로 추가했다.

## 학습 노트

- Unity Behavior 그래프는 변수 타입을 `클래스 이름, 어셈블리 이름`으로 저장한다. 클래스가 다른 어셈블리로 가면 그래프 창이 그 타입을 못 찾아 열리지 않는다.
- `EnemyCommands`는 아직 asmdef 안에 있으므로 그 이름은 바꾸면 안 된다.

## 검증

- 에셋 문자열 3곳이 `AbstractEnemy, Assembly-CSharp`인지 확인했다.
- Unity 에디터에서 그래프를 직접 열지는 못했다. Console 컴파일 후 EnemyBT를 더블클릭해 확인한다.
