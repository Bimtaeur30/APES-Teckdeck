# PHASE-014 — 레지스트리 이름 정리(Component~ 제거)

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-07 (KST)` (작업 후 소급 기록)
- 사용자 승인: `2026-10-07 "네이밍은 네가 다 바꿔줘. RegistryRuntime으로 할게"`, `"응 RegistrySO로 바꿔줘"`, `"응 둘 다 바꿔줘"`
- 작업 방식: 이름 변경만. 파일 이름은 `git mv`로 바꿔 `.meta` GUID를 유지했다.
- 기록 누락: Phase 계획·기록 없이 바로 진행했다. PHASE-015 시작 전에 소급 기록한다.

## 구현 결과

- 런타임
  - `RegistryConstructor` → `RegistryRuntime` (사용자가 만든 빈 MonoBehaviour)
  - `ComponentRegistrySO` → `RegistrySO`, `CreateAssetMenu` → `Lib/Registry/RegistrySO`
  - `ComponentListItem` → `RegistryEntry`
  - 필드 `components` → `entries` + `[FormerlySerializedAs("components")]`
- 에디터
  - `ComponentRegistrySOEditor` → `RegistrySOEditor`, `ComponentTypeDropdown` → `RegistryTypeDropdown`
  - 내부 이름: `OnEntryFocus`, `TryAddEntry`, `_pendingItemType`, `GetEntryEnum`, Undo 이름 `Add/Remove Registry Entry`
  - `FindProperty("components")` → `FindProperty("entries")`
  - UXML/USS: `RegistryEditorUI.uxml`, `RegistryRowUI.uxml`, `RegistryEditorUIStyle.uss` (두 UXML의 `<Style src>` 경로 수정)
- 에셋: `Runtime/Test.asset`, `JTH/GameModules/RegistryTest/ComponentRegistrySO.asset`의 `m_EditorClassIdentifier`와 `components:` → `entries:` (JTH 에셋 파일 이름·`m_Name`은 유지)

## 검증

- grep으로 시스템 폴더 안 `Component` 잔여 확인: `FormerlySerializedAs("components")`만 남음.
- 검증하지 못한 항목: Unity 컴파일, 인스펙터 표시. PHASE-015 검증 때 함께 확인한다.
