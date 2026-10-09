# PHASE-018 — RegistryRuntime 사본 생성·보관·조회

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-08 (KST)`
- 사용자 승인: `2026-10-08 "JSON 관련 빼고는 다 앎이야 해줘"`
- 작업 방식: JSON 복제(`ToJson`/`FromJson`) 부분은 `어려움 + 모름` → 해설 주석 없이 코드만, 주석은 사용자가 단다. 나머지는 `완벽히 앎` → 검수만.
- 범위 밖: 사본 값을 SO에 적용하는 버튼(사용자가 나중에 버튼 방식으로 결정), enum 키 조회 오버로드, 커스텀 에디터

## 구현 결과

- `Runtime/RegistryRuntime.cs` (사용자가 시작한 타입 구하기 부분을 이어서 작성)
  - `GenerateEntryInstances()`: `args` 제거. 항목마다 타입 구하기 → (에디터만) 경고 → 타입 없음·`registryItem` null이면 건너뜀 → `ToJson`/`FromJson`으로 사본 → `IInitRegistryItem`이면 `Init()` → `registryDict`와 `runtimeEntries`에 같은 사본 저장
  - `runtimeEntries`: `[SerializeField] List<RegistryEntry>`. 플레이 중 인스펙터에서 키와 사본 값을 보고 조정
  - `Awake`에서 생성. `TryGetItem`은 아직 안 만들었으면 그때 생성(다른 스크립트 Awake가 먼저 조회해도 됨)
  - `TryGetItem(int, out IRegistryItem)`, `TryGetItem<T>(int, out T)`
  - `RegistrySO` 읽기 프로퍼티
  - `#if UNITY_EDITOR` `WarnEntry`: `RegistryItemValidator.GetWarningMsg(type)` 또는 "타입을 찾을 수 없어 건너뜁니다". static `HashSet<string>`으로 타입당 한 번만 `Debug.LogWarning(…, registrySO)`. `[RuntimeInitializeOnLoadMethod(SubsystemRegistration)]`로 플레이마다 비움(도메인 리로드 꺼도 동작)
- `JTH/Scripts/RegistryTest/RegistryRuntimeTester.cs` (신규, 확인 후 지워도 됨)
  - `[RequireComponent(typeof(RegistryRuntime))]`. `Start`와 컨텍스트 메뉴 `Compare With SO`에서 키마다 사본 조회 → 원본과 다른 객체인지, 값(JSON) 같은지, 두 JSON 로그

## 검증

- Unity 재컴파일 오류 0.
- 플레이 모드 (임시 SO `JTH/GameModules/RegistryTest/Phase018Temp.asset` + 플레이 중 임시 GameObject 2개. 끝나고 플레이 종료·SO 삭제, 씬 변경 없음)
  - SO 항목: Def(cooldown 2.5, 프리팹 참조), Args(매개변수 생성자만, comboCount 5), NS([Serializable] 없음, null), Gone(못 찾는 타입), Init(IInitRegistryItem)
  - Def/Args/Init 사본 생성, 원본과 다른 객체, 값 같음. NS/Gone 건너뜀
  - 플레이 모드에서 프리팹 참조 복원됨(PHASE-016에서 남았던 항목)
  - 사본 cooldown을 99로 바꿔도 SO는 2.5. 두 번째 RegistryRuntime(B)은 2.5로 시작하고 A와 다른 객체
  - `TryGetItem<DefaultCtorItem>(0)` true, `TryGetItem<ArgsCtorItem>(0)` false
  - `runtimeEntries` 3개가 딕셔너리와 같은 사본
  - 콘솔 경고 3개(Args 생성자, NS Serializable, Gone 타입 없음), B를 만들어도 반복 안 됨. 테스터 로그 정상
- 검증하지 못한 항목: `Init()` 호출 자체(테스트 항목의 Init이 비어 있어 관찰 불가, 코드상 확인), IL2CPP 빌드, 실제 인스펙터에서 `runtimeEntries` 표시 모양

## 남은 위험

- `GenerateEntryInstances()`를 편집 모드에서 부르면 `runtimeEntries`가 씬/프리팹에 저장된다. 지금은 Awake·TryGetItem에서만 부르므로 플레이 중에만 채워진다.
