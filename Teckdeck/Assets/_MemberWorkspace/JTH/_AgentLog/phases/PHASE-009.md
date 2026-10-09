# PHASE-009 — 베이스 타입 드롭다운

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-06 (KST)`
- 사용자 승인: `2026-10-06 "승인, 009부터 진행"` (판정 `완벽히 앎`, 베이스 저장은 타입 이름 문자열, 생성은 기본 생성자 우선)
- 작업 방식: 사용자 판정 `완벽히 앎`. Claude가 에디터 C#까지 작성하고 사용자는 검수한다.

## 요청과 목표

- 사용자가 저장 방식을 `[SerializeReference] IRegistryItem`으로 바꾼 뒤, 베이스 스크립트 `ObjectField`(MonoScript)를 `AdvancedDropdown` 선택으로 바꾼다.
- 후보: `IRegistryItem`을 구현한 인터페이스·추상 클래스·클래스와 `IRegistryItem` 자신. `UnityEngine.Object` 계열과 열린 제네릭은 제외.

## 승인된 구현 범위

- `Runtime/ComponentRegistrySO.cs`: `baseScriptGuid` → `baseTypeName`(`"FullName, AssemblyName"`)
- `Editor/ComponentRegistryEditor UI.uxml`: `base-script-field` → `base-type-row`(읽기 전용 `TextField base-type-field` + `Button base-type-btn "선택"`)
- `Editor/ComponentRegistryEditor UI Style.uss`: `registry__base-type-row/field/btn`
- `Editor/ComponentTypeDropdown.cs`: 생성자에 선택 인자 `title`(기본 "컴포넌트 타입"), `isAddable`(기본 `IsAddable`) 추가. 안내 문구 "베이스 타입을 먼저 지정하세요". 사용자 주석 유지(baseScript→baseType, MonoBehaviour→UnityEngine.Object 표현만 수정).
- `Editor/ComponentRegistrySOEditor.cs`: `HandleBaseTypeBtn`, `HandleBaseTypeSelected`(Undo "Change Base Type"), `IsBaseTypeSelectable`, `GetBaseType`, `FillBaseTypeField` 추가. `HandleBaseScriptObjectFieldChange` 삭제. `HandleAddBtn`은 베이스가 없거나 `IRegistryItem`이 아니면 열지 않음.
- 제외: 항목 생성 방식, Missing 표시, `[Serializable]` 필터, 삭제 후 키 검사(PHASE-010). 런타임 생성/`Init`(사용자).

## 구현 결과

- 베이스 표시: 찾은 타입이면 `Name`(툴팁 `FullName`), 못 찾으면 저장된 문자열 그대로.
- 오류 문구: 비어 있음 "베이스 타입을 지정하세요", 해석 실패 "타입을 찾을 수 없습니다: …", `IRegistryItem` 아님/UnityEngine.Object "베이스 타입으로 쓸 수 없는 타입입니다: …".
- 값 채우기는 `SetValueWithoutNotify`라 Undo 후 다시 기록되지 않는다. Undo 시 `FillValues` → `FillBaseTypeField`로 갱신.

## 검증

- Unity CLI `recompile`: 컴파일 오류 0(경고는 기존 `RegistryTestFireball.explosisonRadius`뿐).
- `eval_file`로 직접 호출(테스트 에셋 `JTH/GameModules/RegistryTest/ComponentRegistrySO.asset`, 끝나고 원래 값 `""`로 복구 + 그 에셋 Undo 기록 정리):
  - 베이스 드롭다운 후보: `IInitRegistryItem`, `IRegistryItem`(제목 "베이스 타입")
  - 항목 드롭다운(기본 필터, 베이스 `IRegistryItem`): 구현 클래스가 아직 없어 "추가할 수 있는 타입이 없습니다"(비활성) — 정상
  - UI: `base-type-field` 읽기 전용, 버튼 "선택", 처음 오류 "베이스 타입을 지정하세요"
  - `IInitRegistryItem` 선택 → `baseTypeName = "_Shared.Systems.RegistrySystem.Runtime.IInitRegistryItem, RegistrySystem_runtime_assembly"`, 필드 `IInitRegistryItem`, 오류 없음, dirty, `Type.GetType`으로 다시 해석됨
  - 없는 타입/`System.String`/빈 값 → 각 오류 문구 확인
- 검증하지 못한 항목: 실제 인스펙터 화면에서 버튼/필드 클릭으로 팝업이 뜨는 위치와 모양, Ctrl+Z로 베이스 되돌리기. 사용자가 인스펙터에서 확인한다.

## 다음 단계

- PHASE-010 진행 전 사용자 승인 재확인.
- 기존 레지스트리 에셋은 `baseScriptGuid` 값이 사라지므로 베이스를 다시 골라야 한다.
