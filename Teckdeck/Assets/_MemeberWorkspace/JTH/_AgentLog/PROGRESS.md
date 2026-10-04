# 작업 진행 현황

## 사용자

- 이름/이니셜: `JTH`
- 작업폴더: `Assets/_MemeberWorkspace/JTH/`
- 마지막 갱신: `2026-10-05 (KST)`

## 현재 요청

- 요청 요약: 컴포넌트 프리팹을 enum 키로 관리하는 레지스트리 시스템(`_Shared/Systems/RegistrySystem`). Claude는 UI(UXML/USS)만 만들고, C# 코드는 사용자가 직접 짠다.
- 승인된 범위: PHASE-001(에디터 구현, 이후 코드 삭제), PHASE-002(UI만 남기기), PHASE-003(Row UI 정리), PHASE-004(추가 입력 줄 배치), PHASE-005(is-delayed, 인스펙터 표시 클래스), PHASE-006(타입 선택 팝업, `어려움 + 모름`이라 Claude가 코드만 작성, 주석은 사용자가 단다), PHASE-007(오류 표시 통일), PHASE-008(네임스페이스 제외 목록 칸)
- 범위 밖 항목: C# 로직 작성(사용자 담당), 런타임 `Build`, 플레이 중 반영, Skill/State 이전

## Phase 현황

| Phase | 상태 | 목표 | 기록 |
|---|---|---|---|
| 001 | 완료(코드는 PHASE-002에서 삭제) | Registry 에디터 | `phases/PHASE-001.md` |
| 002 | 완료 | UI만 남기기 | `phases/PHASE-002.md` |
| 003 | 완료 | Row UI 정리(인라인 스타일, hint, type-button, key-label 제거) | `phases/PHASE-003.md` |
| 004 | 완료 | 추가 입력 줄(pending) 배치 | `phases/PHASE-004.md` |
| 005 | 완료 | key 입력칸 is-delayed, 인스펙터 `--open` 클래스 | `phases/PHASE-005.md` |
| 006 | 완료 | 타입 선택 팝업 `ComponentTypeDropdown`(Claude 작성, 주석은 사용자) | `phases/PHASE-006.md` |
| 007 | 완료 | 오류 문구 켜고 끄기를 `registry__error--visible` 하나로 통일 | `phases/PHASE-007.md` |
| 008 | 완료 | 네임스페이스 제외 목록 칸(`skipNamespaces`) | `phases/PHASE-008.md` |

## 현재 재개 지점

- 마지막 완료 작업: enum 영역에 `skipNamespaces` PropertyField 추가(PHASE-008).
- 다음에 할 작업: 사용자가 에디터 코드를 직접 작성한다. Claude는 막힌 지점에만 답한다.
- 사용자 승인이 필요한 사항: 없음
- 관련 파일: `Assets/_Shared/Systems/RegistrySystem/Editor/`(`ComponentRegistryEditor UI.uxml`, `ComponentRegistryRow UI.uxml`, `ComponentRegistryEditor UI Style.uss`). 파일 이름은 사용자가 직접 바꾼 이름이다.
- 알려진 문제 또는 위험: 없음

## 검증 요약

- 수행한 검증: 줄/메인 UXML 다시 임포트, 요소 이름·인라인 스타일·`--active`/`--invalid` 토글 시 표시 확인
- 통과 여부: 통과
- 아직 검증하지 못한 항목: 없음
