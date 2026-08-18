# PHASE-014 — UI 토글 포인터 입력 대상 분리

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-18 (Asia/Seoul)`
- 사용자 승인: `Pointer Target Graphic 선택 기능 계획 승인`

## 요청과 목표

- 원래 요청: 마우스 Hover 시 어떤 Graphic 위에 마우스를 올려야 토글이 실행될지 지정 가능하게 변경
- 이 Phase의 목표: 속성이 변하는 UI 대상과 포인터 입력을 받는 Graphic을 Inspector에서 독립적으로 지정
- 완료 조건: 지정한 Graphic의 Hover·Click·PointerDown 이벤트가 토글로 전달되고 여러 토글이 같은 Graphic을 공유 가능

## 승인된 구현 범위

- 변경 파일: `UIToggleComponent.cs`, 신규 `UIPointerEventRelay.cs`, 각 메타 파일, KTJ 작업 기록
- 구현 방법: 공통 토글에 `Pointer Target` 직렬화 필드를 추가하고, 런타임 Relay가 지정 Graphic의 포인터 이벤트를 등록된 토글에 전달
- 검증 방법: Unity 참조 기반 신규 스크립트 별도 컴파일, 입력 대상 등록·해제 흐름 정적 검토, diff 공백 검사
- 명시적으로 제외한 항목: 씬·프리팹 변경, 커스텀 Inspector, 포인터 입력 시스템 교체

## 구현 결과

- 수행한 작업: `Pointer Target`에 지정한 Graphic에서 Hover·Click·PointerDown을 감지하도록 분리. 미지정 시 토글과 같은 오브젝트의 Graphic을 기본값으로 사용하고, 여러 토글이 하나의 Relay를 공유하도록 구현
- 생성·수정한 파일: `UIToggleComponent.cs`, `UIPointerEventRelay.cs`, `UIPointerEventRelay.cs.meta`, `PROGRESS.md`, `PHASE-014.md`
- 계획과 달라진 점 및 이유: 없음

## 학습 노트

- 전체 실행 흐름: 토글 활성화 → Pointer Target의 Relay 조회 또는 런타임 생성 → 토글 등록 → Graphic이 포인터 이벤트 수신 → Relay가 등록된 토글에 전달 → 토글이 재생조건에 따라 상태 전환
- 주요 클래스/메서드의 역할: `UIToggleComponent.BindPointerTarget`은 입력 Graphic과 연결하고 `UnbindPointerTarget`은 비활성화 시 등록 해제. `UIPointerEventRelay`는 Unity EventSystem 이벤트를 여러 토글에 안전하게 중계
- UniTask 사용 위치와 이유: 비동기 작업이 없어 사용하지 않음
- DOTween 사용 위치와 이유: 기존 속성 보간 구조를 그대로 사용하며 입력 대상 변경은 Tween 종류에 영향을 주지 않음
- 중요한 구현 원리: 변화 대상인 Size·Color·Alpha Target과 이벤트 Raycast 대상인 Pointer Target은 서로 다른 책임이므로 별도 참조로 관리
- 예외 상황과 대응: Pointer Target이 비어 있으면 같은 오브젝트의 Graphic을 탐색하며 그것도 없으면 경고 출력. 이벤트 전달 중 토글이 비활성화되어 목록이 변경되는 경우를 위해 복사본을 순회

## 검증

- 자동 검증: Unity 6000.3.10f1 및 프로젝트 DOTween 참조 기반 전체 UI 컴포넌트 별도 컴파일, `git diff --check`, 코드 정적 검토
- Unity Editor 수동 확인 절차: 토글 컴포넌트의 Pointer Target에 입력 영역으로 사용할 Image 등을 지정 → 해당 Graphic의 Raycast Target 활성화 확인 → Play Mode에서 지정 Graphic과 변화 대상 각각에 Hover하여 지정 Graphic에서만 실행되는지 확인
- 결과: 신규 UI 스크립트 별도 컴파일 경고 0개·오류 0개, 공백 오류 없음
- 검증하지 못한 항목: 열린 Unity Editor의 Play Mode 실제 포인터 입력

## 다음 단계

- 남은 문제: Unity Play Mode에서 EventSystem 및 GraphicRaycaster 환경의 실제 동작 확인
- 제안하는 다음 Phase: 없음
- 추가 승인이 필요한 사항: 없음
