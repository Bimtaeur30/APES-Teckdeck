# PHASE-020 — 초기화 인터페이스 분리(IRegistryCreatedReceiver)

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-09 (KST)`
- 사용자 승인: `2026-10-09 "OnRuntimeCreated로 하고 인터페이스 이름은 IRegistryCreatedReceiver, 바로 진행해"`
- 문제: `IInitRegistryItem : IRegistryItem`이라 베이스 후보가 두 인터페이스 중 하나를 직접 구현한 타입이었다. 등록 대상인지와 초기화가 필요한지는 서로 다른 성질이므로 분리한다.
- 판정: 런타임 변경은 상속 제거·이름 변경뿐이라 `완벽히 앎`. 에디터 변경은 판정 없음.

## 구현 결과

- `Runtime/IInitRegistryItem.cs` → `Runtime/IRegistryCreatedReceiver.cs`(meta GUID 유지)
  - `IRegistryItem` 상속 제거, `Init()` → `OnRuntimeCreated()`
- `Runtime/RegistryRuntime.cs`: `item is IRegistryCreatedReceiver receiver` → `receiver.OnRuntimeCreated()`
- `Runtime/RegistryItemValidator.cs`: 생성자 경고 문구를 `OnRuntimeCreated`로
- `Editor/RegistrySOEditor.cs`: `IsBaseTypeSelectable`은 `IRegistryItem` 직속 구현만, 오류 문구 수정
- `JTH/Scripts/RegistryTest/RegistryTestItems.cs`: 부모 구현(virtual)·자식 override·자식만 구현 검증 항목

## 검증

- Unity 재컴파일 오류 0
- `eval_file`(임시 SO·HideAndDontSave GameObject, 끝나고 DestroyImmediate)
  - 베이스 후보: `RegistryTestItemBase`, `RegistryTestInitBase`, `IRegistryTestSkill` true / `IRegistryItem`, `IRegistryCreatedReceiver`, 자식 타입 false
  - 사본 생성: 부모 구현 자식 `createdCount=1`, override 자식 `overrideCalled=True, createdCount=1`, 자식만 구현 `createdCount=1`, 미구현 항목 정상, 모두 원본과 다른 참조
- 검증하지 못한 항목: 실제 플레이 모드에서 `Awake` 경로(같은 `GenerateEntryInstances`를 타므로 위험 낮음)

## 남은 위험

- `IRegistryCreatedReceiver`만 구현하고 `IRegistryItem`이 없는 타입은 이제 베이스로 고를 수 없다. 기존 에셋 중 그런 베이스를 쓰던 것은 없음(검색 기준).
