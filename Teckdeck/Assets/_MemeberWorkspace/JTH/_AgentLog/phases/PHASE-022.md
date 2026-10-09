# PHASE-022 — RegistryRuntime 커스텀 에디터(사본 → SO 적용)

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-10 (KST)`
- 사용자 승인: `2026-10-10 "응 그대로 진행해"` (B안: 항목별 + 전체 적용)
- 판정: 에디터 코드(판정 없음)

## 구현 결과

- `Editor/RegistryRuntimeEditor.cs` (`[CustomEditor(typeof(RegistryRuntime))]`, UI Toolkit, UXML 없이 코드로 구성)
  - `registrySO` PropertyField: 플레이 중 비활성
  - "전체 적용" 버튼 + 항목 목록(기본 리스트의 +/- 없음). 플레이 중이 아니면 HelpBox 안내, 플레이 중 사본 0개면 "만들어진 사본이 없습니다"
  - 항목: `키 (타입)` Foldout(`viewDataKey = registry-runtime-{enumValue}`로 펼침 상태 유지) + "적용" 버튼. 안에는 PHASE-019처럼 자식 필드만
  - `runtimeEntries.Array.size`를 `TrackPropertyValue`로 지켜보다 바뀌면 다시 그림(늦게 생성되는 경우)
- 적용(`ApplyEntries`)
  - `enumValue`로 SO 항목을 찾음. 사본 없음·짝 없음·SO 인스턴스 없음·타입 다름은 `[Registry]` 경고 후 건너뜀
  - `Undo.RecordObject(registrySO)` → `CopyWithoutReadOnly` → `SetDirty`
  - `CopyWithoutReadOnly`: SO 항목을 JSON으로 백업 → 사본 JSON을 `FromJsonOverwrite` → `[ReadOnlyField]` 필드만 백업 값으로 되돌림(부모 클래스 private 필드까지 수집)

## 검증

- 재컴파일 오류 0
- `eval_file` 편집 모드(임시 SO·HideAndDontSave GameObject, 끝나고 삭제)
  - 편집 모드 인스펙터: "플레이 중에만 사본이 만들어집니다" HelpBox
  - 사본의 `cooldown`, `hitCount`, `remainTime`, `history` 모두 수정 후 적용 → SO는 `cooldown`만 바뀌고 ReadOnly 3개는 원래 값(`7`, `2`, `[1,2]`)
  - 항목 0만 적용 시 항목 1은 그대로, 전체 적용 시 항목 1 반영
  - SO에서 지운 번호 2: "SO에 같은 번호의 항목이 없어 건너뜁니다" 경고
  - 사본과 SO 항목·리스트가 서로 다른 객체
  - Undo 맨 위 "Apply Registry Runtime" 확인 후 PerformUndo → 원래 값
  - `CreateEntryElement`: Foldout 텍스트·viewDataKey·적용 버튼·필드 5개 바인딩 경로, 필드 없는 항목은 안내 Label
- 검증하지 못한 항목
  - 실제 플레이 모드 화면(열린 씬에 저장 안 된 변경이 있어 플레이 진입하지 않음). 사용자 확인 필요
  - 같이 열린 RegistrySOEditor 패널이 적용 직후 갱신되는지
  - CLI 연속 호출이라 Undo 그룹이 합쳐짐(메모 참고). 실제 버튼 클릭은 클릭마다 Undo 한 번이어야 한다

## 남은 위험

- 항목 안 중첩 클래스 필드의 `[ReadOnlyField]`는 적용 제외 대상이 아니다(항목 바로 아래 필드만).
- 항목 안에 `[SerializeReference]` 필드가 있으면 JSON 복사가 지원되지 않아 빠질 수 있다.
- 플레이 중 적용한 값은 SO 에셋에 남는다(의도). 프로젝트 저장 전까지는 디스크에 기록되지 않는다.
