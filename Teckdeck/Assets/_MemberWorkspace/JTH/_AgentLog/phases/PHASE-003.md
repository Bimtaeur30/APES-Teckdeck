# PHASE-003 — Row UI 정리

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-04 (KST)`
- 사용자 승인: `2026-10-04 "key-label도 지워서 진행해"`

## 요청과 목표

- 원래 요청: 인라인 스타일은 없어야 한다. `hint`는 없앤다. 처음 만들 때는 pending 줄에서 바꾸니 `type-button`은 필요 없다. 이름은 TextField를 눌러서 바꾼다.
- 이 Phase의 목표: 줄 UXML에서 인라인 스타일, `hint`, `type-button`, `key-label`을 없애고 오류 표시를 USS 클래스로 켜고 끈다.
- 완료 조건: 줄 UXML에 `style` 속성이 없고, `registry-row--invalid` 클래스만으로 오류 문구가 보였다 숨겨진다.

## 승인된 구현 범위

- 변경 파일: `RegistrySystem/Editor/ComponentRegistryRow UI.uxml`, `RegistrySystem/Editor/ComponentRegistryEditor UI Style.uss`
- 명시적으로 제외한 항목: C# 코드, 메인 UXML(`ComponentRegistryEditor UI.uxml`, 인라인 스타일 없음), `.registry-row--editing`(pending 줄 강조용으로 남김).
- 파일 이름은 사용자가 고친 이름(`ComponentRegistry... UI.uxml`, `... UI Style.uss`)을 그대로 쓴다.

## 구현 결과

- 줄 UXML: `row > body > (key-field, type-label)`, `row > error`만 남겼다. `style="display: none;"`을 모두 지웠다.
- USS
  - 지움: `.registry-row__key`, `.registry-row__type-button`, `.registry-row__hint`
  - `.registry-row__key-field`: 굵은 글씨를 옮겨 왔다.
  - `.registry-row__error`: 기본 `display: none`.
  - 추가: `.registry-row--invalid .registry-row__error { display: flex; }`
- 계획과 달라진 점: 없음.

## 학습 노트

- 숨김을 인라인 스타일이나 `style.display`로 하지 않고, 상태 클래스 하나에 묶었다. C#은 `row.EnableInClassList("registry-row--invalid", hasError)`만 호출하면 빨간 줄과 오류 문구가 같이 바뀐다.
- 클래스를 붙일 대상은 `TemplateContainer`가 아니라 안쪽 `row`(`.registry-row`)다.
- `key-field`는 확정 시점에만 값 변경이 오도록 `isDelayed = true`를 고려한다(UXML `is-delayed` 또는 C#).
- 추가 흐름: `+` → 타입 드롭다운 → pending 줄(`type-label`에 고른 타입, `key-field`에 키 입력) → Enter로 생성.

## 검증

- 자동 검증: Unity CLI `run_script`로 줄 UXML을 다시 임포트하고 임시 창에 붙여 확인했다.
  - 요소 이름: `row, body, key-field, type-label, error`
  - 인라인 `display` 값이 있는 요소: 0개
  - `error` 표시: 기본 `None` → `--invalid` 붙임 `Flex` → 뗌 `None`
  - `key-field` 글꼴: `Bold`
- Unity Editor 수동 확인 절차: UI Builder에서 `ComponentRegistryRow UI.uxml`을 열고 `row`에 `registry-row--invalid` 클래스를 붙였다 떼어 본다.
- 결과: 통과.

## 다음 단계

- 남은 문제: 없음.
- 제안하는 다음 Phase: 사용자가 에디터 코드를 계속 직접 작성한다.
- 추가 승인이 필요한 사항: 없음.
