# PHASE-012 — 열린 줄 파란 표시 갱신

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-06 (KST)`
- 사용자 승인: `2026-10-06 "파란 표시 RefreshItems 문제도 고쳐줘"` (PHASE-010에서 범위 밖으로 보고한 문제)
- 작업 방식: 사용자 판정 `완벽히 앎`. Claude가 작성, 사용자 검수.

## 요청과 목표

- 인스펙터를 열고 닫을 때, Missing 항목을 열 때, 항목을 추가한 직후에 줄의 `registry-row--open`(파란 표시)이 바로 갱신되게 한다.

## 구현 결과

- `Editor/ComponentRegistrySOEditor.cs`
  - `OnComponentItemFocus`: `_openedItem`과 패널 클래스를 바꾼 직후 `_entryList.RefreshItems()` 호출(닫기·Missing·정상 모든 경로). 사용자가 정상 경로 끝에만 넣어 둔 호출은 이쪽으로 옮김.
  - `TryAddToComponentList`: `OnComponentItemFocus`가 새로고침하므로 바로 앞의 `RefreshItems()` 중복 호출 삭제.

## 검증

- Unity CLI `recompile`: 오류 0.
- `eval_file`: 임시 EditorWindow에 인스펙터 UI를 붙여 ListView가 실제 줄을 만들게 한 뒤, 줄의 `registry-row--open` 상태 확인(테스트 에셋 사용, 끝나고 복구 + Undo 기록 정리)
  - A 추가 → `A:OPEN`
  - B 추가 → `A:- | B:OPEN`
  - A 포커스 → `A:OPEN | B:-`
  - A 다시 포커스(닫힘) → `A:- | B:-`
  - Missing B 포커스 → `A:- | B:OPEN`
- 검증하지 못한 항목: 사용자 인스펙터 창에서 마우스로 누르는 흐름(로직은 같은 메서드).
