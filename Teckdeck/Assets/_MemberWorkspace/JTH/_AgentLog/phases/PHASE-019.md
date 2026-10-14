# PHASE-019 — 인스펙터 패널에서 자식 필드만 그리기

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-08 (KST)`
- 사용자 승인: `2026-10-08 "A로 하자"`
- 문제: 항목 인스펙터 패널이 `PropertyField(registryItem)`로 SerializeReference 필드를 통째로 그려 `Registry Item` Foldout 줄이 생기고, Foldout 화살표가 왼쪽 여백 쪽으로 그려져 패널(`registry__inspector-body` padding 4px) 밖으로 튀어나왔다. 제목에 타입 이름이 이미 있어 Foldout 줄도 중복.
- 선택지: A 자식 필드만 그리기(채택), B USS `padding-left` 늘리기

## 구현 결과

- `Editor/RegistrySOEditor.cs` `OnEntryFocus`
  - `itemProp.GetEndProperty()`까지 `NextVisible(enterChildren)`로 자식을 돌며 필드마다 `PropertyField(child.Copy())` + `Bind`
  - 필드가 없으면 `NoFieldMsg`("표시할 필드가 없습니다") Label
- `NoFieldMsg` 상수 추가

## 검증

- Unity 재컴파일 오류 0.
- `eval_file` (임시 SO `Phase019Temp.asset` 생성 후 삭제, Undo 정리)
  - DefaultCtor: `cooldown`, `effectPrefab` PropertyField 2개, Foldout 없음
  - ArgsCtor: `cooldown`, `comboCount` PropertyField 2개
  - InitItem(필드 없음): 안내 Label
- 검증하지 못한 항목: 실제 인스펙터 화면 모양(사용자 확인). 패널에 붙지 않은 상태라 PropertyField 내부 요소는 만들어지지 않으므로 Foldout 개수 0은 참고값.

## 남은 위험

- 항목 안의 필드가 List·배열·중첩 클래스·SerializeReference면 그 필드 자체는 여전히 Foldout으로 그려져 화살표가 같은 이유로 튀어나올 수 있다. 생기면 USS `.registry__inspector-body`에 `padding-left`(약 15px)를 더한다(B).
