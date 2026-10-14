# PHASE-013 — 베이스 변경 시 항목 비우기와 화면 정리

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-06 (KST)`
- 사용자 승인: `2026-10-06 "네가 넣어줘. Display랑 다른 처리까지 해줘."`
- 작업 방식: 사용자 판정 `완벽히 앎`. 사용자가 `components.Clear()`를 먼저 넣었고, Claude가 나머지 처리를 붙였다.

## 구현 결과

- `Editor/ComponentRegistrySOEditor.cs` `HandleBaseTypeSelected`
  - 새 타입 이름이 저장된 `baseTypeName`과 같으면 아무것도 하지 않음
  - 항목이 있으면 `EditorUtility.DisplayDialog("베이스 타입 변경", "…항목 N개가 모두 삭제됩니다. Ctrl+Z로 되돌릴 수 있습니다…", "변경", "취소")`. 취소하면 중단
  - `Undo.RecordObject` → `baseTypeName` 저장 → `components.Clear()` (Undo 한 번에 베이스와 항목이 같이 돌아옴)
  - 정리: `ClosePending()`(이전 베이스 타입으로 열린 입력 줄), `HandleInspectorCloseBtn()`(인스펙터 닫기 + `RefreshItems`), `CheckKeysValid()`, `GenerateBtnDirtyCheck()`, `FillBaseTypeField()`
  - `lastEnumValue`는 그대로(enum 번호 재사용 안 함)

## 검증

- Unity CLI `recompile`: 오류 0.
- `eval_file`(테스트 에셋, 끝나고 원래 값으로 복구 + 그 에셋 Undo 기록 정리. 시작 시 베이스는 사용자가 지정해 둔 `RegistryTestInitBase`였고 그대로 복구):
  - 항목 0개에서 베이스 A 선택 → 확인 창 없이 바뀜
  - 항목 X 추가 + 입력 줄 열림 + 인스펙터 열림 상태에서 같은 A 다시 선택 → 변화 없음(항목 1, 입력 줄·인스펙터 유지)
  - 항목 0개 + 입력 줄 열린 상태에서 B 선택 → 베이스 B, 입력 줄·인스펙터 닫힘
- 검증하지 못한 항목: 항목이 있을 때 확인 창 흐름(모달이라 CLI에서 누를 수 없음). 사용자가 인스펙터에서 "변경"/"취소"와 Ctrl+Z를 확인한다.
