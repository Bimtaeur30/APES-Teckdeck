# PHASE-019 — PCM_assembly 참조 연결

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-05 23:45 (UTC+9)`
- 사용자 승인: `PCM 폴더에서 다른 Assembly를 못 가져오는 오류를 해결해 달라는 요청`

## 구현 결과

- `PCM_assembly.asmdef`를 `Scripts`에서 PCM 폴더 루트로 옮겼다. `GameModule`의 `CommandChange`와 `Scripts`의 `AbstractEnemy`가 같은 어셈블리가 된다.
- References에 `Combat_assembly`, `ModuleSystem`을 추가했다. `AgentSystem_assembly`, `Unity.Behavior`는 유지했다.

## 학습 노트

- asmdef 참조는 전달되지 않는다. `Agent`를 참조해도 `Agent`가 쓰는 `HealthModule`과 `ModuleOwner`는 따로 참조해야 한다.
- `Scripts`에만 asmdef가 있으면 그 바깥의 `CommandChange`는 `Assembly-CSharp`라서 PCM 어셈블리가 볼 수 없다.

## 검증

- asmdef references 목록을 확인했다.
- Unity 컴파일은 직접 확인하지 못했다.
