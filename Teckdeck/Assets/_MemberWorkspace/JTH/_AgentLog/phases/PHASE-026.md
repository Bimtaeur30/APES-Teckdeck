# PHASE-026 — 네임스페이스 제외를 드롭다운으로 고르기 + 미리보기

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-10 (KST)`
- 사용자 승인: `2026-10-10 "진행하고 검사는 필요 없음. 스테이징까지 해줘."` (네임스페이스 식별자 검사는 제외)
- 문제: `skipNamespaces`를 PropertyField 리스트에 직접 타이핑해야 했다

## 구현 결과

- `Editor/RegistryEditorUI.uxml`: `namespace-skip-field` PropertyField → `namespace-skip-row`(읽기 전용 TextField + `선택` 버튼, 레이아웃은 `registry__folder-*` 재사용) + `namespace-preview` Label
- `Editor/RegistryEditorUIStyle.uss`: `.registry__namespace-preview` 추가
- `Editor/RegistrySOEditor.cs`
  - `GetEnumFolderPath`(유효한 폴더만), `GetNamespaceSegments`(Assets 아래 폴더 이름들), `BuildNamespace`(사용자가 넣은 제외·숫자 앞 `_` 로직을 옮김, 생성과 미리보기가 같이 사용)
  - `BuildNamespaceSkipMenu`: GenericMenu. 현재 경로 폴더 이름을 체크로, 경로에 없는 기존 항목은 구분선 아래 "다른 경로"로, 폴더 없으면 비활성 안내
  - `ToggleNamespaceSkip`: Undo `Change Namespace Skip` + SetDirty + 갱신
  - `FillNamespaceSkipField`: 제외 목록 쉼표 표시, 미리보기 갱신. `FillValues`(Undo 포함)·`SetEnumFolder`에서 호출
  - `HandleGenerateBtn`은 `BuildNamespace` 사용
- 메뉴는 GenericMenu(네이티브 메뉴라 GUI 스킨 문제 없음)를 버튼 `worldBound`로 바로 띄움
- 저장 형식(`skipNamespaces` 문자열 리스트) 변경 없음

## 검증

- 재컴파일 오류 0
- `eval_file`(메모리 SO, 끝나고 삭제)
  - 폴더 없음: 미리보기 "(enum 폴더 없음)", 메뉴는 비활성 안내 1개
  - `Assets/_Shared/Systems/RegistrySystem/Runtime`: 메뉴 4항목, 미리보기 `_Shared.Systems.RegistrySystem.Runtime`
  - `_Shared` 체크 → 필드 `_Shared`, 미리보기 `Systems.RegistrySystem.Runtime`, 다시 체크 해제 → 원복
  - 경로에 없는 `_MemberWorkspace` → 구분선 + "다른 경로"(비활성) + 체크된 항목
  - `BuildNamespace`: `.../KTJ/02_Script/Player/FSM`(+_MemberWorkspace 제외) → `KTJ._02_Script.Player.FSM`, `Assets` → `Assets`, 전부 제외 → `None`
  - Undo(다음 프레임에서 확인): 맨 위 `Change Namespace Skip`, PerformUndo → 원래 목록
- 검증하지 못한 항목: 실제 메뉴 클릭(네이티브 메뉴는 모달이라 CLI로 띄우지 않음)과 메뉴 위치. 사용자 확인 필요.
