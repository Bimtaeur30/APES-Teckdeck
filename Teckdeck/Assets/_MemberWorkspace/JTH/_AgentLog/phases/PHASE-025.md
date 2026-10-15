# PHASE-025 — enum 폴더를 폴더 선택 창·드래그로 고르기

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-10 (KST)`
- 사용자 승인: `2026-10-10 "응 진행해 드래그는 유지해줘"`
- 문제: enum 폴더가 `ObjectField(DefaultAsset)`라 선택 창에 폴더·기타 에셋이 섞여 찾기 어려움

## 구현 결과

- `Editor/RegistryEditorUI.uxml`: `ObjectField` → `enum-folder-row`(읽기 전용 `TextField` `enum-folder-field` + `선택` 버튼 `enum-folder-btn`)
- `Editor/RegistryEditorUIStyle.uss`: `.registry__folder-row/-field/-btn`, 드래그 중 표시 `.registry__folder-field--drop` 추가(사용자 작업 중인 다른 규칙은 건드리지 않음)
- `Editor/RegistrySOEditor.cs`
  - 버튼·경로 칸 클릭 → `EditorUtility.OpenFolderPanel`(시작: 현재 폴더 → SO 폴더 → Assets)
  - `ToProjectPath`: 절대 경로 → `Assets/...`(역슬래시·끝 슬래시·대소문자 정리, `AssetsBackup` 같은 접두어 함정 제외). 밖이면 null
  - `SetEnumFolder`: 밖이면 오류, 임포트 전 새 폴더는 `ImportAsset`으로 .meta 생성 후 GUID 저장(Undo `Change Enum Folder`, SetDirty)
  - `FillEnumFolderField`: GUID → 현재 경로 표시(폴더 이동·이름 변경 따라감), 못 찾으면 `Missing` + 오류
  - 드래그: `DragAndDrop.paths`가 폴더 1개일 때만 Link 표시·받기
  - 제거: `HandleEnumFolderObjectFieldChange`, `GetGuid`, `FillObjectField`, 상수 `GuidIsNull`/`NoMeta`/`NoObject`

## 검증

- 재컴파일 오류 0
- `eval_file`(메모리 SO, 끝나고 삭제)
  - ToProjectPath: Assets 자체 → `Assets`, 하위·역슬래시·끝 슬래시·소문자 → 정상, `AssetsBackup/x`·`C:/Temp` → null
  - 시작 경로: 미설정 → dataPath, 설정 → 그 폴더 절대 경로
  - 정상 폴더 → GUID 저장·경로 표시·오류 없음, Undo 맨 위 `Change Enum Folder`
  - 바깥 → 오류, 기존 값 유지
  - 디스크에만 만든 새 폴더 → 임포트 후 GUID 저장(임시 폴더는 DeleteAsset으로 삭제)
  - 없는 GUID → `Missing` + 오류
  - 드래그: 폴더 1개 → 경로, 파일·2개 → null
- 검증하지 못한 항목: OS 폴더 창이 실제로 열리고 고르는 동작, 실제 마우스 드래그(하이라이트 포함). 사용자 확인 필요.
