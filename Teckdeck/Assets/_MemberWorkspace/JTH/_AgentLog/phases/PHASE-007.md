# PHASE-007 — 오류 문구 켜고 끄는 방식 통일

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-04 (KST)`
- 사용자 승인: `2026-10-04 "네가 클래스로 error 키고 끄는걸로 통일하고 uxml 고치고 뭐 고쳤는지 알려줘"`

## 요청과 목표

- 오류 라벨마다 켜는 방법이 달라 헷갈린다(목록·pending은 부모 `registry-row--invalid`, 설정·enum은 C# `style.display`). 오류 라벨 자신에 클래스 하나를 붙였다 떼는 방식으로 통일한다.

## 승인된 구현 범위

- 변경 파일: `ComponentRegistryEditor UI.uxml`, `ComponentRegistryRow UI.uxml`, `ComponentRegistryEditor UI Style.uss`
- 명시적으로 제외한 항목: C# 코드(`SetErrorMsg`, `HandleBindItem` 수정은 사용자가 한다).

## 구현 결과

- 오류 라벨 4개 모두 `.registry__error` 사용
  - `settings-error`, `enum-error`: `registry__error`
  - `pending-error`, 목록 줄 `error`: `registry__error registry__error--compact`
- USS
  - `.registry__error`: 기본 `display: none` 추가
  - 추가: `.registry__error--compact`(줄 안 오류: 11px, 위 여백 3px), `.registry__error--visible { display: flex; }`
  - 삭제: `.registry-row__error`, `.registry-row--invalid .registry-row__error`
- `.registry-row--invalid`는 이제 빨간 테두리/배경만 담당한다. 오류 문구를 켜지 않는다.

## 학습 노트

- 오류 문구는 어디서든 `label.EnableInClassList("registry__error--visible", hasError)` + `label.text`.
- 목록/pending 줄을 빨갛게 하려면 줄에 `registry-row--invalid`를 따로 붙인다.

## 검증

- Unity CLI `run_script`로 세 파일 다시 임포트 후 확인.
  - 4개 라벨 기본 `None` → `--visible` 붙이면 `Flex`. 크기: 설정/enum 12px, 줄 안 11px.
  - 목록 줄에 `--invalid`만 붙이면 테두리는 빨강, 오류 문구는 `None`.
  - `registry-row__error` 클래스가 남은 곳 없음.
- 결과: 통과.

## 다음 단계

- 사용자가 `SetErrorMsg`(인라인 `style.display` → 클래스)와 `HandleBindItem`(오류 라벨에 `--visible`)을 고친다.
