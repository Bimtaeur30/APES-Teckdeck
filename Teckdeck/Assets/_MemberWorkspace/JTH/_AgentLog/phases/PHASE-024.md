# PHASE-024 — 타입 드롭다운 빨간 글씨(스타일 못 찾음) 수정

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-10 (KST)`
- 사용자 승인: `2026-10-10 "진행하고 이 버그 고친 것만 스테이징해줘"`
- 문제: 베이스 타입·항목 추가 드롭다운 글씨가 빨갛고 줄 간격이 벌어짐. 콘솔 `Unable to find style 'DD ItemStyle' in skin 'GameSkin' mouseUp` 등 7개.
- 원인: `AdvancedDropdownGUI.Styles`(internal static)가 처음 쓰일 때 `GUISkin.current`에서 스타일을 이름으로 찾아 저장한다. UI Toolkit `Button.clicked` 콜백은 IMGUI 밖이라 그 순간 스킨이 `GameSkin`이어서 `StyleNotFoundError`(빨간 글씨, fixedHeight 0)가 저장되고 도메인 리로드 전까지 남는다. 모든 AdvancedDropdown(Add Component 포함)이 공유.

## 구현 결과

- `Editor/RegistrySOEditor.cs`
  - 1x1 `IMGUIContainer`(`_dropdownHost`, absolute, PickingMode.Ignore)를 루트에 추가
  - `RequestDropdown(dropdown, anchor)`: 띄울 드롭다운과 기준 요소를 기억하고 `MarkDirtyRepaint`
  - `ShowPendingDropdown`(컨테이너 OnGUI): `Show(_dropdownHost.WorldToLocal(anchor.worldBound))` — OnGUI 안에선 좌표 기준이 컨테이너
  - `HandleAddBtn`, `HandleBaseTypeBtn`이 `RequestDropdown` 사용
- 리플렉션으로 내부 Styles를 미리 초기화하는 방법은 내부 이름 의존이라 쓰지 않음

## 검증

- 재컴파일 오류 0(도메인 리로드로 static 초기화된 상태에서 시작)
- `eval_file`: 임시 RegistrySO + 임시 EditorWindow에 인스펙터를 붙이고 `HandleBaseTypeBtn` 호출 → `RepaintImmediately`
  - `_pendingDropdown`이 비워짐(OnGUI 안에서 Show 실행됨), 새 `Unable to find style` 경고 없음
  - `Styles.itemStyle = DD ItemStyle`(회색, fixedHeight 17), `header = DD HeaderStyle`, `checkMark`, `lineSeparator`, `rightArrow` 정상
  - 임시 창·에디터·SO 정리
- 검증하지 못한 항목: 드롭다운 창 위치(유니티가 백그라운드라 팝업이 포커스를 잃고 바로 닫혀 위치를 못 봄). 사용자가 버튼 바로 아래에 뜨는지 확인.

## 남은 위험

- 이미 깨진 세션은 못 고친다(재컴파일하면 풀림).
- 다른 도구가 재컴파일 직후 UI Toolkit 콜백에서 AdvancedDropdown을 먼저 열면 여전히 깨진다.
