# PHASE-023 — RegistryRuntime 직렬화 필드 되돌리기

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-10 (KST)`
- 사용자 승인: `2026-10-10 "3번은 네가 되돌려줘"`
- 문제: 사용자가 FSM 적용 중 `registrySO`를 `[field: SerializeField]` 자동 프로퍼티로, `runtimeEntries`를 직렬화 안 되는 `_runtimeEntries`로 바꿔 `RegistryRuntimeEditor`의 `FindProperty`가 null → 인스펙터 열 때 `ArgumentNullException`. 자동 프로퍼티는 직렬화 이름이 `<RegistrySO>k__BackingField`라 씬의 SO 연결도 끊길 위험.

## 구현 결과

- `Runtime/RegistryRuntime.cs`: 직렬화 관련만 되돌림
  - `[SerializeField] private RegistrySO registrySO;` + `public RegistrySO RegistrySO => registrySO;`
  - `[SerializeField] private List<RegistryEntry> runtimeEntries`(주석 위치 복구, `_registryDict` 이름 반영)
- 사용자가 추가한 `DefaultExecutionOrder(-100)`, `Count`, `GetItemList<T>`(OfType, `_registryDict` 검사), `_registryDict` 이름은 유지

## 검증

- 재컴파일 오류 0
- `Test-Skill_Scene.unity`는 아직 `registrySO:` 키로 저장돼 있어 연결 유지
- `CreateEntryElement` UI 확인 eval 재실행 통과(Foldout·적용 버튼·필드 바인딩 경로 `runtimeEntries...`)

## 추가 수정 (2026-10-10)

- 되돌릴 때 `Editor/RegistryRuntimeEditor.cs`의 `FindProperty("RegistrySO")`(IDE 이름 변경으로 문자열까지 바뀐 것)를 놓쳐 SO 필드가 안 보였다. `"registrySO"`로 고침.
- 확인: 임시 RegistryRuntime으로 인스펙터 생성 → PropertyField `bindingPath=registrySO`, 프로퍼티 찾음.
