# PHASE-021 — ReadOnlyField 어트리뷰트

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-10 (KST)`
- 사용자 승인: `2026-10-10 "응 그대로 진행해"`
- 판정: `어려움 + 모름`. Claude는 코드만 작성하고 해설 주석은 사용자가 단다.
- 목적: 런타임 상태 필드를 인스펙터에서 보이되 수정은 막고, RegistryRuntime "적용" 때 SO로 복사하지 않기 위한 표시.
- Alchemy `[ReadOnly]`를 쓰지 않은 이유: `PropertyAttribute`가 아니라 Alchemy 에디터 안에서만 동작하고, 커스텀 에디터의 `PropertyField`에선 무시된다. 편집 모드 허용 옵션도 없다.

## 구현 결과

- `_Shared/Systems/InspectorSystem/Runtime/`
  - `InspectorSystem_runtime_assembly.asmdef`(GUID `0c9dc5b4ec960444d890ccccc0239846`)
  - `ReadOnlyFieldAttribute.cs`: `PropertyAttribute(applyToCollection: true)`, `EditableInEditMode`
- `_Shared/Systems/InspectorSystem/Editor/`
  - `InspectorSystem_editor_assembly.asmdef`(Editor 전용, 위 런타임 참조)
  - `ReadOnlyFieldDrawer.cs`: IMGUI `OnGUI`. 읽기 전용 = `!EditableInEditMode || EditorApplication.isPlaying`. `DisabledScope` 안에서 `EditorGUI.PropertyField(..., true)`
  - UI Toolkit `CreatePropertyGUI` 대신 IMGUI를 쓴 이유: 드로어 안에서 같은 프로퍼티로 `PropertyField`를 만들면 같은 드로어로 다시 들어갈 위험이 있다. IMGUI는 중첩 호출 시 다음 드로어(기본 그리기)로 넘어가는 처리가 있다. UI Toolkit 인스펙터는 IMGUIContainer로 감싸 그린다.
- `RegistrySystem_editor_assembly.asmdef`: InspectorSystem 런타임 참조 추가(PHASE-022 적용 제외용)
- `JTH/Scripts/RegistryTest/RegistryTestItems.cs`: `RegistryTestDefaultCtorItem`에 `hitCount`(ReadOnly), `remainTime`(편집 모드 허용), `history` List(ReadOnly)

## 검증

- 재컴파일 오류 0(테스트 필드 미사용 경고 CS0414 2개)
- `eval_file`, `ScriptAttributeUtility.GetHandler`로 드로어 확인
  - `cooldown` 없음 / `hitCount` ReadOnlyFieldDrawer(false) / `remainTime` ReadOnlyFieldDrawer(true)
  - `history` 리스트 자체에 드로어, `history.Array.data[0]` 요소엔 없음 → 리스트 통째로 비활성(크기·+/- 포함)
- 검증하지 못한 항목: 실제 화면에서 회색 표시(사용자 확인). 편집 모드에서 `remainTime`만 수정 가능, 플레이 중엔 셋 다 회색이어야 한다.

## 남은 위험

- 항목 안 중첩 클래스의 필드에 붙인 `[ReadOnlyField]`는 화면 표시는 되지만 PHASE-022 적용 제외는 항목 바로 아래 필드만 본다.
