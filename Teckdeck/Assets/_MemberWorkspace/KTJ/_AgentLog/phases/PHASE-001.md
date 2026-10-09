# PHASE-001 — AnimParamSO 타입 로드 오류 수정

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-13 15:07 (Asia/Seoul)`
- 사용자 승인: `승인`

## 요청과 목표

- 원래 요청: `GGMLib.AnimationSystem.AnimParamSO` TypeLoadException만 수정
- 이 Phase의 목표: UXML이 현재 프로젝트에 존재하는 필드 타입을 로드하도록 참조 갱신
- 완료 조건: 오래된 타입 문자열이 제거되고 실제 타입과 어셈블리 이름이 설정됨

## 승인된 구현 범위

- 변경 예정 파일: `Assets/_Shared/Systems/FsmSystem/Editor/StateSO View.uxml`
- 구현 방법: `ObjectField`의 `type` 속성 한 줄 교체
- 검증 방법: 실제 `HashDataSO` 선언과 asmdef 이름 비교, 저장소 내 이전 참조 검색
- 명시적으로 제외한 항목: 다른 오류, 경고, 코드 및 에셋 수정

## 구현 결과

- 수행한 작업: `GGMLib.AnimationSystem.AnimParamSO, Assembly-CSharp`를 `AnimatorSystem.HashDataSO, AnimatorSystem_assembly`로 교체
- 생성·수정한 파일: 공용 UXML 1개, KTJ 작업 기록 2개
- 계획과 달라진 점 및 이유: 없음

## 학습 노트

- 전체 실행 흐름: Unity가 UXML을 임포트하면서 `type` 문자열을 런타임 타입으로 해석하고 ObjectField에 적용함
- 주요 클래스/메서드의 역할: `StateSO.stateParam`은 `HashDataSO` 에셋을 보관하고, UXML `ObjectField`는 Inspector UI에서 그 값을 편집함
- UniTask 사용 위치와 이유: 사용하지 않음
- DOTween 사용 위치와 이유: 사용하지 않음
- 중요한 구현 원리: UXML 타입 문자열은 `전체 네임스페이스.타입명, 어셈블리명`과 실제 선언이 정확히 일치해야 함
- 예외 상황과 대응: asmdef 이름이 바뀌면 UXML 타입 문자열도 함께 갱신해야 함

## 검증

- 자동 검증: 이전 타입 참조 제거 및 새 타입·어셈블리 선언 일치 확인
- Unity Editor 수동 확인 절차: Unity로 돌아가 임포트 완료를 기다린 뒤 Console을 Clear하고 해당 UXML을 Reimport
- 결과: 정적 참조 검증 통과
- 검증하지 못한 항목: 실행 중인 Unity Editor의 재임포트 결과

## 다음 단계

- 남은 문제: Unity Editor에서 오류 소멸 최종 확인
- 제안하는 다음 Phase: 없음
- 추가 승인이 필요한 사항: 추가 오류 수정 요청 시 별도 승인 필요
