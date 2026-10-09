# PHASE-010 — 항목 생성과 표시 정리

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-06 (KST)`
- 사용자 승인: `2026-10-06 "승인, 009부터 진행"` → PHASE-009 보고 후 `"ㅇㅇ 010 진행해"`
- 작업 방식: 사용자 판정 `완벽히 앎`. Claude가 작성, 사용자 검수.

## 요청과 목표

- `+`로 항목을 만들 때 기본 생성자(private 포함)가 있으면 생성자로 만들어 필드 초기값을 살리고, 없으면 `GetUninitializedObject`로 만든다.
- `registryItem`이 null(missing type)일 때 줄/인스펙터에서 터지지 않고 "Missing"으로 보인다.
- 항목 드롭다운에서 `[Serializable]`이 없는 타입을 뺀다.
- 삭제 후 키 검사를 다시 해서 지난 중복 오류가 남지 않게 한다.

## 승인된 구현 범위

- `Editor/ComponentRegistrySOEditor.cs`
  - `MissingTypeMsg` 상수, `using System.Reflection`
  - `HandleBindItem`: 타입 라벨 `Missing`, 다른 오류가 없으면 줄 오류에 `MissingTypeMsg`
  - `OnComponentItemFocus`: null이면 제목 `Missing`, 본문 Label(`MissingTypeMsg`)
  - `CreateRegistryItem(Type)`: 기본 생성자 → `Invoke`, 없으면 `RuntimeHelpers.GetUninitializedObject`
  - `TryAddToComponentList`: 생성 실패(예외) 시 pending 오류 "인스턴스를 생성할 수 없습니다: …", `IRegistryItem`이 아니면 오류. 생성 성공 후에 `Undo.RecordObject`
  - `HandleRemoveBtn`: 삭제 후 `CheckKeysValid()`
- `Editor/ComponentTypeDropdown.cs`: `IsAddable`에 `type.IsSerializable` 검사, 주석 갱신
- 검증용 `JTH/Scripts/RegistryTest/RegistryTestItems.cs`(사용자 작업폴더, 확인 후 지워도 됨): 추상 베이스, 기본 생성자 타입, 인수 생성자만 있는 타입, `[Serializable]` 없는 타입
- 제외: 런타임 생성/`Init`, 이름 변경, 기존 테스트 스크립트·프리팹 정리

## 검증

- Unity CLI `recompile`: 오류 0.
- `eval_file`로 직접 호출(테스트 에셋 사용, 끝나고 원래 상태로 복구 + Undo 기록 정리):
  - 베이스 `RegistryTestItemBase` → 항목 후보 `RegistryTestArgsCtorItem`, `RegistryTestDefaultCtorItem`(추상 베이스·`[Serializable]` 없는 타입 제외)
  - 기본 생성자 타입 추가 → `radius=3`, `cooldown=1`(초기값 적용), enum 0, 인스펙터 열림, `PropertyField.bindingPath = components.Array.data[0].registryItem`, 직렬화 `radius` 3
  - 인수 생성자만 있는 타입 추가 → `comboCount=0`, `cooldown=0`(초기값 없음, 예상대로), enum 1
  - 줄 표시: 정상 줄 타입 이름, null 줄 `Missing` + 오류 문구. 인스펙터 null 항목 → 제목 `Missing` + 안내 문구
  - 키 중복 → 오류 1 → 한 줄 삭제 → 오류 0, 남은 줄 1
- 검증하지 못한 항목: 실제 클래스 이름을 바꿔서 생기는 진짜 missing type(null로 직접 대입해 흉내만 냄), 인스펙터 화면에서의 클릭·Undo 흐름.

## 발견한 문제(범위 밖, 수정 안 함)

- `OnComponentItemFocus`에서 `_entryList.RefreshItems()`가 빠져 있어, 타입 라벨을 눌러 인스펙터를 열거나 닫을 때 줄의 파란 표시(`registry-row--open`)가 바로 갱신되지 않는다. 추가 직후에도 `RefreshItems`가 포커스보다 먼저 불려 새 줄이 파랗게 안 된다.

## 다음 단계

- 사용자가 인스펙터에서 추가/삭제/Undo/값 편집을 확인한다.
- 런타임 생성과 `Init`은 사용자가 작성한다.
