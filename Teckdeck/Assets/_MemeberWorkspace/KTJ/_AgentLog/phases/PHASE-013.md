# PHASE-013 — 조건 선택형 UI 토글 컴포넌트

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-18 (Asia/Seoul)`
- 사용자 승인: `width/height 해석 확인 및 구현 승인`

## 요청과 목표

- 원래 요청: Width/Height, 색상, 알파값을 전환하는 UI 컴포넌트 3종과 마우스 Hover·Click 등의 재생조건 선택 기능 구현
- 이 Phase의 목표: 세 컴포넌트가 동일한 입력·상태·DOTween 설정을 공유하면서 각자 담당하는 UI 속성을 전환
- 완료 조건: Inspector에서 두 상태의 값, 재생조건, 시간, Ease를 지정하고 uGUI 포인터 입력으로 실행 가능

## 승인된 구현 범위

- 변경 파일: `02_Script/UI/Components/` 신규 C# 스크립트 및 KTJ 작업 기록
- 구현 방법: 공통 추상 컴포넌트가 상태와 입력을 처리하고 Size·Color·Alpha 구현이 속성별 Tween을 생성
- 검증 방법: 프로젝트 빌드, 정적 코드 및 diff 공백 검사, Unity Editor 수동 확인 절차 작성
- 명시적으로 제외한 항목: 씬·프리팹·공용 경로 변경, 커스텀 Inspector, 추가 UI 효과 컴포넌트

## 구현 결과

- 수행한 작업: Hover, Click, PointerDown, OnEnable 조건과 외부 호출용 `Toggle`·`SetState`를 구현. `RectTransform` Width/Height, `Graphic.color`, `CanvasGroup.alpha`를 DOTween으로 전환하는 컴포넌트 3종 추가
- 생성·수정한 파일: `UITogglePlaybackCondition.cs`, `UIToggleComponent.cs`, `UISizeToggle.cs`, `UIColorToggle.cs`, `UIAlphaToggle.cs`, 각 `.meta`, `PROGRESS.md`, `PHASE-013.md`
- 계획과 달라진 점 및 이유: 없음

## 학습 노트

- 전체 실행 흐름: 포인터 이벤트 수신 → 선택된 재생조건인지 확인 → On/Off 상태 결정 → 기존 Tween Kill → 대상 속성의 새 Tween 실행
- 주요 클래스/메서드의 역할: `UIToggleComponent`는 입력과 상태를 통합하고, 각 파생 컴포넌트의 `ApplyImmediate`와 `CreateTween`이 실제 속성을 처리. `SetState`는 다른 스크립트나 UnityEvent에서도 호출 가능
- UniTask 사용 위치와 이유: 비동기 대기 작업이 없어 사용하지 않음
- DOTween 사용 위치와 이유: 짧은 UI 속성 보간을 일관된 Duration·Ease·Unscaled Time 설정으로 관리하기 위해 사용
- 중요한 구현 원리: Hover와 PointerDown은 진입/누름에서 On, 이탈/뗌에서 Off로 복귀하며 Click은 누를 때마다 상태를 반전. Width/Height는 Stretch Anchor에서도 실제 사각형 크기를 다루도록 `SetSizeWithCurrentAnchors` 사용
- 예외 상황과 대응: 대상 참조가 없으면 속성 변경을 건너뛰며, 비활성화·파괴 시 진행 중 Tween을 Kill. EventSystem과 Raycast 가능한 Graphic이 없으면 포인터 이벤트가 전달되지 않음

## 검증

- 자동 검증: 기존 `KTJ.csproj` 빌드, Unity 6000.3.10f1 참조 어셈블리를 사용한 신규 스크립트 별도 컴파일, `git diff --check`, 신규 코드 정적 검토
- Unity Editor 수동 확인 절차: 대상 UI에 알맞은 컴포넌트 추가 → Target과 Off/On 값 설정 → Playback Condition 선택 → Play Mode에서 Hover/Click/PointerDown 확인 → OnEnable은 오브젝트 재활성화로 확인
- 결과: 기존 생성 프로젝트 및 신규 UI 스크립트 별도 컴파일 모두 경고 0개·오류 0개, 신규 파일 공백 오류 없음
- 검증하지 못한 항목: 열린 Unity Editor 때문에 배치 임포트가 실행되지 않아 Unity Console 임포트 결과와 Play Mode 동작은 수동 확인 필요

## 다음 단계

- 남은 문제: Unity가 신규 스크립트를 임포트한 뒤 Console 오류 여부 및 실제 포인터 동작 확인
- 제안하는 다음 Phase: 필요 시 Scale·회전·Sprite 전환 컴포넌트 추가
- 추가 승인이 필요한 사항: 추가 컴포넌트 구현 시 별도 승인
