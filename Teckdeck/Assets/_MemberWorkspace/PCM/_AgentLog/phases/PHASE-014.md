# PHASE-014 — AbstractEnemy가 Agent를 상속

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-05 20:19 (UTC+9)`
- 사용자 승인: `부모로 상속이 안 됐다는 수정 요청`

## 요청과 목표

- 원래 요청: `AbstractEnemy`의 부모를 `Agent`로 둔다
- 원인: `AbstractEnemy`는 `AssemblyStageDefinition` 안에 있고, `Agent`는 기본 어셈블리 `Assembly-CSharp`에 있다. 커스텀 어셈블리는 기본 어셈블리를 참조할 수 없다.
- 완료 조건: `public class AbstractEnemy : Agent` 가 컴파일된다

## 구현 결과

- `AbstractEnemy.cs`를 `Scripts/Enemy/Runtime/`에서 `Scripts/Enemy/`로 이동했다. 이 위치는 asmdef 밖이라 기본 어셈블리에 들어간다.
- 스크립트 GUID `d44376a573b903c49b00992afa72789a`는 유지했다.
- `AssemblyEnemyDefinition.asmdef`는 `EnemyCommand`용으로 그대로 뒀다. 공용 `Agent.cs`는 수정하지 않았다.

## 학습 노트

- 상속은 같은 어셈블리이거나, 자식 어셈블리가 부모 어셈블리를 References에 넣어야 한다.
- `Agent` 폴더에는 asmdef가 없어서 부모를 참조 목록에 추가할 수 없다. 그래서 자식 스크립트를 asmdef 밖으로 뺐다.
- `AbstractEnemy`는 `Agent`의 `Awake`에서 모듈을 초기화한다. 체력은 `Health` 프로퍼티로 쓴다.

## 검증

- Unity가 리임포트한 뒤 Console에 `Agent`를 찾을 수 없다는 에러가 없어야 한다.
- 이 환경에서는 Editor 컴파일을 실행하지 못했다.
