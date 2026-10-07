# PHASE-018 — AgentSystem이 Combat을 참조

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-05 23:41 (UTC+9)`
- 사용자 승인: `Shared의 CombatSystem과 AgentSystem이 통신되게 해결해 달라는 요청`

## 구현 결과

- `AgentSystem_runtime_assembly.asmdef`가 `Combat_assembly`와 `AnimatorSystem_assembly`를 참조하게 추가했다.
- `Combat_assembly`는 `AgentSystem`을 참조하지 않는다. `Agent`가 `HealthModule`을 쓰고, Combat은 `Agent`를 쓰지 않기 때문이다.

## 학습 노트

- asmdef끼리는 References에 넣은 방향만 타입을 볼 수 있다.
- 양쪽이 서로를 참조하면 순환 참조라서 둘 다 컴파일되지 않는다.

## 검증

- asmdef JSON의 references에 Combat, Animator GUID가 있는지 확인했다.
- Unity 컴파일은 직접 확인하지 못했다.
