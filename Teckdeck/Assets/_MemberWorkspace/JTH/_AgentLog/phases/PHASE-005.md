# PHASE-005 — key 입력칸 is-delayed, 인스펙터 표시 클래스

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-04 (KST)`
- 사용자 승인: `2026-10-04 "둘 다 UXML에 넣어줘"`, 인스펙터 표시 방법 질문("active 같은거 설정할 필요 없음?")

## 요청과 목표

- `key-field`, `pending-key-field`에 `is-delayed="true"`를 넣는다. 글자마다가 아니라 Enter/포커스 해제 때 한 번만 값 변경 이벤트가 오게 한다.
- `.registry__inspector`가 기본 `display: none`인데 보이게 할 클래스가 없었다. 인라인 스타일 없이 열 수 있게 `.registry__inspector--open`을 추가한다.

## 승인된 구현 범위

- 변경 파일: `ComponentRegistryRow UI.uxml`, `ComponentRegistryEditor UI.uxml`, `ComponentRegistryEditor UI Style.uss`
- 명시적으로 제외한 항목: C# 코드.

## 구현 결과

- `key-field`, `pending-key-field`: `is-delayed="true"`
- USS: `.registry__inspector--open { display: flex; }`
- 추가(사용자 요청 "hint에 Esc로 취소도 써야겠다"): `pending-hint` 문구를 `클래스 선택 후 key(enum) 작성 후 [Enter], [Esc]로 취소`로 바꿨다.

## 학습 노트

- `is-delayed`는 Enter뿐 아니라 포커스가 빠질 때도 확정한다. pending 줄에서 "Enter로만 생성"을 지키려면 `KeyDownEvent`로 Enter를 따로 받는 편이 안전하다.
- 인스펙터 열기: `inspector-panel`에 `registry__inspector--open`을 붙이고, 열린 줄에는 `registry-row--open`을 붙인다. 내용은 `inspector-body.Clear()` 후 `new InspectorElement(component)`.

## 검증

- 자동 검증: Unity CLI `run_script`로 다시 임포트 후 확인.
  - `key-field.isDelayed = True`, `pending-key-field.isDelayed = True`
  - `inspector-panel` 표시: 기본 `None` → `--open` `Flex` → 뗌 `None`
- 결과: 통과.

## 다음 단계

- 남은 문제: 없음.
- 추가 승인이 필요한 사항: 없음.
