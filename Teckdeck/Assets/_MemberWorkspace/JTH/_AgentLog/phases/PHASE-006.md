# PHASE-006 — 타입 선택 팝업(ComponentTypeDropdown)

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-04 (KST)`
- 사용자 승인: `2026-10-04 "ㅇㅇ 지웠어 그 이름으로 진행해"`
- 작업 방식: 사용자 판정 `어려움 + 모름`. Claude는 코드만 쓰고, 주석은 사용자가 직접 단다. (처음에 Claude가 해설 주석을 달았다가 사용자 정정으로 지웠다.) 이 시스템에서 Claude가 쓴 C#은 이 파일뿐이다.

## 요청과 목표

- `+`를 누르면 뜨는 검색 가능한 타입 선택 팝업을 만든다. 고른 `Type`은 콜백으로 에디터에 돌려준다.

## 승인된 구현 범위

- 변경 파일: `RegistrySystem/Editor/ComponentTypeDropdown.cs`(신규)
- 명시적으로 제외한 항목: 에디터 연결(`HandleAddBtn`에서 띄우기, 콜백에서 pending 줄 켜기). 사용자가 직접 쓴다.

## 구현 결과

- 생성자 `(AdvancedDropdownState state, Type baseType, Action<Type> onSelected)`, `minimumSize = (250, 300)`
- `BuildRoot`: `TypeCache.GetTypesDerivedFrom(baseType)` + 베이스 자신 → 추상/열린 제네릭/MonoBehaviour 아님 제외 → 이름순. 이름이 겹치는 타입만 `Name (Namespace)`로 표시. 베이스가 null이거나 후보가 없으면 고를 수 없는 안내 항목.
- 항목: `AdvancedDropdownItem`을 상속한 private `TypeItem`이 `Type`을 들고 다닌다.
- `ItemSelected`: `TypeItem`이면 `onSelected(type)` 호출.

## 검증

- 컴파일: 오류 없음.
- Unity CLI `run_script`로 `BuildRoot`/`ItemSelected`를 직접 호출.
  - `MonoBehaviour` 기준: 384개, 추상 0, 열린 제네릭 0, MonoBehaviour 아님 0, 이름순 정렬 확인, 이름 겹침 표시 2개
  - 4번째 항목(`AspectRatioFitter`) 선택 → 콜백에 `UnityEngine.UI.AspectRatioFitter`
  - 구체 베이스(`BaseInput`) 기준: `BaseInput, BaseInputOverride`(베이스 자신 포함)
  - `null` 베이스: "베이스 스크립트를 먼저 지정하세요"(비활성), `ScriptableObject` 베이스: "추가할 수 있는 타입이 없습니다"(비활성)
- 검증하지 못한 항목: 실제 `+` 버튼에서 `Show(_addBtn.worldBound)`로 띄운 화면. 에디터 연결 코드가 아직 없다. 연결 후 인스펙터에서 `+`를 눌러 확인한다.

## 다음 단계

- 사용자가 `HandleAddBtn`과 콜백을 직접 연결한다.
- 사용자가 이 파일에 직접 주석을 단다.
