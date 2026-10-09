# PHASE-008 — 네임스페이스 제외 목록 칸

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-05 (KST)`
- 사용자 승인: `2026-10-05 "namespaceFoldersToSkip, 경로 전체로 진행해" → 이후 "skipNamespaces로 바꿔줘"` 이후 "네임스페이스 하나하나 나눠서 할 건데 그걸 네가 알 필요는 없다"로 정정. 값 형식은 C# 쪽에서 정한다.

## 요청과 목표

- enum 생성 시 네임스페이스에서 뺄 값을 SO에 `List<string>`으로 보관하고, 인스펙터에서 편집한다.
- Rider DotSettings에서 읽는 방식은 쓰지 않는다(레지스트리 어셈블리용 파일이 없고, 인코딩된 경로라 다루기 까다롭고, 실제 코드 네임스페이스와도 맞지 않음).

## 승인된 구현 범위

- Claude: `ComponentRegistryEditor UI.uxml`의 `enum-folder-field` 아래에 `PropertyField` 추가.
- 사용자: `ComponentRegistrySO`에 `public List<string> skipNamespaces` 추가, 생성 로직에서 사용.

## 구현 결과

- `<uie:PropertyField name="namespace-skip-field" binding-path="skipNamespaces" label="네임스페이스 제외"/>`
- 값 형식 설명(툴팁)은 넣지 않았다. 형식은 C#이 정한다.

## 검증

- UXML 다시 임포트 후 `namespace-skip-field`가 `PropertyField`로 생성되고 `bindingPath = skipNamespaces`, `enum-folder-field` 바로 아래에 있음을 확인.
- 사용자가 SO에 `skipNamespaces`를 추가한 뒤, UXML만 레지스트리 에셋에 Bind해서 확인: `PropertyField` 안에 ListView(제목 "네임스페이스 제외")가 생성됨. 에디터 전체 인스펙터는 `ComponentRegistrySOEditor.cs:89`의 `_pendingKeyField` 미할당 NullReference로 열리지 않아 확인하지 못함(사용자 코드 작성 중).

## 다음 단계

- 사용자가 `_pendingKeyField`를 할당한 뒤 인스펙터에서 +/- 동작과 저장을 확인한다.
