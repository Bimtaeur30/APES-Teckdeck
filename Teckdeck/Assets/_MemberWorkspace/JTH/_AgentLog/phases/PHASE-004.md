# PHASE-004 — 추가 입력 줄(pending) 배치

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-04 (KST)`
- 사용자 승인: `2026-10-04` 계획 제시 후 흐름과 hint 문구를 정해 줌("+ → 드롭다운 선택 → key 미리 채움 → 수정 → Enter", hint 고정)

## 요청과 목표

- 원래 요청: 목록 줄 템플릿에는 hint/드롭다운이 없으니, 추가용 입력 줄은 pending 아래에 따로 둔다.
- 흐름(사용자 결정): `+` → 드롭다운에서 클래스 선택 → 드롭다운 닫힘 → 선택한 클래스 이름으로 key 미리 채움 → 필요하면 수정 → Enter로 추가.
- 이 Phase의 목표: 메인 UXML의 `pending-row-container` 안에 추가 전용 입력 줄을 배치한다. Row 템플릿은 목록 전용으로 둔다.

## 승인된 구현 범위

- 변경 파일: `RegistrySystem/Editor/ComponentRegistryEditor UI.uxml`, `RegistrySystem/Editor/ComponentRegistryEditor UI Style.uss`
- 명시적으로 제외한 항목: C# 코드(드롭다운 열기, key 미리 채우기, Enter 처리), Row 템플릿.

## 구현 결과

- UXML: `pending-row-container > pending-row(.registry-row .registry-row--editing) > pending-hint, pending-body(pending-key-field, pending-type-label), pending-error`
  - 타입 다시 고르기 버튼은 두지 않았다. 드롭다운은 `+`로만 열고, 고른 타입은 읽기 전용 `pending-type-label`에 표시한다.
  - hint 문구는 UXML에 고정: `클래스 선택 후 key(enum) 작성 후 [Enter]`
- USS
  - `.registry__pending`: 기본 `display: none`. `.registry__pending--active`일 때 `flex`.
  - `.registry-row__hint` 다시 추가.
  - `.registry-row--editing .registry-row__type:hover`: 입력 줄의 타입 라벨은 클릭 대상이 아니라 hover 색 변화를 없앴다.
  - 오류 문구는 목록과 같은 `.registry-row--invalid` 규칙을 쓴다. 붙이면 테두리가 파랑에서 빨강으로 바뀐다.
- 인라인 스타일 없음.

## 추가 변경 (같은 날)

- 사용자 요청("Dropdown 쓰면 되잖아")으로 `pending-type-label`을 UI Toolkit `DropdownField`(`pending-type-dropdown`, `.registry-row__type-dropdown`)로 바꿨다. AdvancedDropdown(검색되는 팝업)은 쓰지 않는다.
- 쓰이지 않게 된 `.registry-row--editing .registry-row__type:hover` 규칙을 지웠다.
- C#: 선택지는 `choices`에 타입 이름 목록을 넣고, 선택은 `RegisterValueChangedCallback`으로 받는다. 검색은 없다.
- 검증: 요소 이름에 `pending-type-dropdown`이 있고 `DropdownField`로 생성됨, `min-width` 120 적용, 나머지 토글 결과는 위와 같다.
- 되돌림(같은 날, 사용자 요청 "원래 팝업 방식이 나은 것 같네"): `DropdownField`는 자동으로 열리지 않고 검색이 없어서 AdvancedDropdown 팝업 방식으로 돌아갔다. `pending-type-label`과 `.registry-row--editing .registry-row__type:hover`를 되살리고 `.registry-row__type-dropdown`을 지웠다. 다시 검증한 결과는 위 "검증" 항목과 같다.

## 학습 노트

- C#: `+` 클릭 시 `pending-row-container`에 `registry__pending--active`를 붙이고, 추가 완료나 취소 시 뗀다. 오류는 `pending-row`에 `registry-row--invalid`.
- `.registry-row--invalid`가 `.registry-row--editing`보다 USS에서 뒤에 있어서, 둘 다 붙으면 빨간 테두리가 이긴다.
- AdvancedDropdown은 UXML 요소가 아니라 코드에서 `Show(rect)`로 여는 창이다. 이번에는 아래 추가 변경대로 `DropdownField`를 쓴다.

## 검증

- 자동 검증: Unity CLI `run_script`로 메인 UXML을 다시 임포트하고 임시 창에 붙여 확인했다.
  - 요소 이름: `pending-row-container, pending-row, pending-hint, pending-body, pending-key-field, pending-type-label, pending-error`
  - hint 문구 확인
  - 컨테이너 표시: 기본 `None` → `--active` `Flex` → 뗌 `None`
  - 오류 문구: 기본 `None` → `--invalid` `Flex`, 테두리 파랑 → 빨강
  - 인라인 `display`가 있는 요소는 ListView 내부 `Scroller` 2개뿐(Unity가 넣는 것, UXML과 무관).
- Unity Editor 수동 확인 절차: UI Builder에서 메인 UXML을 열고 `pending-row-container`에 `registry__pending--active`를 붙여 본다.
- 결과: 통과.

## 다음 단계

- 남은 문제: 없음.
- 제안하는 다음 Phase: 사용자가 추가 흐름 C# 코드를 직접 작성한다.
- 추가 승인이 필요한 사항: 없음.
