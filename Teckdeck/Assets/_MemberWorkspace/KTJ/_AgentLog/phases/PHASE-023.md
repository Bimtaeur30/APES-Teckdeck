# PHASE-023 — KTJ 기준 main 머지

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-15 (Asia/Seoul)`
- 사용자 승인: `이번 머지는 추가 승인 없이 진행` (2026-10-15 대화)

## 요청과 목표

- 원래 요청: KTJ 브랜치 기준으로 충돌을 해결하고 main에 머지.
- 이 Phase의 목표: 팀원의 main 변경은 유지하면서 KTJ 최신 플레이어 코드와 상태 에셋을 보존하고 원격 main에 반영.
- 완료 조건: 충돌 0건, KTJ 원본 파일과 Unity GUID 보존, Unity Editor 컴파일 성공, 머지 커밋·푸시 확인.

## 승인된 구현 범위

- 변경 예정 파일: `Assets/_MemberWorkspace/KTJ/`의 KTJ 파일과 기록, `Assets/_Shared/Systems/FsmSystem/Runtime/` 및 `Editor/`의 기존 FSM 호환 파일.
- 구현 방법: KTJ의 210개 추적 파일을 새 작업 폴더명 아래에 그대로 이관하고, 새 FSM과 이름·어셈블리가 다른 기존 FSM을 별도 경로에 복원한다. 중복된 `.meta` GUID는 기존 상태 에셋 참조를 해치지 않는 파일에만 새 GUID를 부여한다.
- 검증 방법: Git blob·충돌·트리 비교, Unity `.meta` GUID 중복 검사, Unity Editor 재컴파일, 원격 ref 확인.
- 명시적으로 제외한 항목: 게임플레이 기능 수정, 다른 사용자 에셋의 임의 수정, 패키지 변경, Unity Play Mode 동작 검증.

## 구현 결과

- 수행한 작업: KTJ 파일 210개를 `_MemberWorkspace/KTJ`로 이관. PCM 경로로 잘못 잡힌 KTJ 상태 에셋 충돌 제거. 기존 FSM 런타임·에디터를 별도 어셈블리로 복원.
- 생성·수정한 파일: KTJ 폴더와 작업 기록, `Assets/_Shared/Systems/FsmSystem/Runtime/`, `Assets/_Shared/Systems/FsmSystem/Editor/`.
- 계획과 달라진 점 및 이유: `main`에서 공용 FSM과 작업 폴더명이 바뀌어 충돌 파일만 선택하면 KTJ 파일·Unity 상태 에셋이 누락되므로 폴더 단위 보존과 별도 FSM 복원이 필요했다.

## 학습 노트

- 전체 실행 흐름: Unity가 KTJ 어셈블리를 컴파일할 때 기존 FSM 런타임을 참조하고, 팀원이 도입한 새 FSM은 별도의 어셈블리에서 동작한다.
- 주요 클래스/메서드의 역할: 기존 `StateMachine`은 KTJ `StateSO` 목록으로 상태를 구성하며, 새 `StateMachine`은 `RegistryRuntime`으로 상태를 구성한다. 서로 다른 네임스페이스와 어셈블리로 구분한다.
- UniTask 사용 위치와 이유: 이번 머지에서 새 비동기 처리를 작성하지 않았다.
- DOTween 사용 위치와 이유: 이번 머지에서 새 연출을 작성하지 않았다.
- 중요한 구현 원리: Unity 에셋은 스크립트 `.meta` GUID와 어셈블리 이름을 참조하므로 기존 `StateSO`·`StateListSO`의 GUID와 기존 런타임 어셈블리 이름을 유지한다.
- 예외 상황과 대응: Git의 폴더 이동 추정으로 KTJ 에셋이 PCM 경로에 잡힌 충돌은 KTJ 원본 위치로 복구하고, 중복 meta GUID는 런타임의 비직렬화 스크립트 쪽에서 갱신한다.

## 검증

- 자동 검증: KTJ 파일 210개가 `HEAD` 원본과 Git blob 일치, 미해결 충돌 0건, Unity `.meta` GUID 중복 0건, Unity Editor 스크립트 재컴파일 오류 0개.
- Unity Editor 수동 확인 절차: 프로젝트를 다시 열거나 새로고침한 뒤 Console의 컴파일 오류가 없는지 확인한다. KTJ의 상태 목록·상태 에셋을 선택해 참조가 유지되는지 확인하고 PlayerTestScene에서 비행·점프 상태 전환을 Play Mode로 확인한다.
- 결과: 충돌 해결과 Unity Editor 재컴파일 통과. 머지 커밋·원격 ref는 완료 보고에서 최종 확인.
- 검증하지 못한 항목: Unity Play Mode 동작 및 팀원 PCM Behavior 에셋의 기존 managed reference 문제.

## 다음 단계

- 남은 문제: Unity Play Mode 동작 확인과 PCM Behavior 에셋의 별도 오류 확인.
- 제안하는 다음 Phase: 새 기능 작업 시 별도 계획 수립.
- 추가 승인이 필요한 사항: 이번 머지는 추가 승인 면제. 이후 새 기능은 저장소 지침에 따라 승인 필요.
