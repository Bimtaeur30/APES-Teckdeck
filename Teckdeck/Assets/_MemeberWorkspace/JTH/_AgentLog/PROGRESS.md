# 작업 진행 현황

## 사용자

- 이름/이니셜: `JTH`
- 작업폴더: `Assets/_MemeberWorkspace/JTH/`
- 마지막 갱신: `2026-10-08 (KST)`

## 현재 요청

- 요청 요약: enum 키로 항목을 관리하는 레지스트리 시스템(`_Shared/Systems/RegistrySystem`). 2026-10-05 저장 방식을 프리팹 → `[SerializeReference] IRegistryItem`으로 바꿨다(사용자 작성). 2026-10-06부터 에디터 C#은 Claude가 맡는다(판정 `완벽히 앎`, 검수만). 런타임 생성/`Init`은 사용자 담당.
- 승인된 범위: PHASE-001(에디터 구현, 이후 코드 삭제), PHASE-002(UI만 남기기), PHASE-003(Row UI 정리), PHASE-004(추가 입력 줄 배치), PHASE-005(is-delayed, 인스펙터 표시 클래스), PHASE-006(타입 선택 팝업, `어려움 + 모름`이라 Claude가 코드만 작성, 주석은 사용자가 단다), PHASE-007(오류 표시 통일), PHASE-008(네임스페이스 제외 목록 칸), PHASE-009(베이스 타입 드롭다운), PHASE-010(항목 생성·표시 정리) — 009/010은 2026-10-06 "승인, 009부터 진행", PHASE-011(베이스는 IRegistryItem/IInitRegistryItem 직속 구현만, 두 인터페이스 자신은 제외) — 2026-10-06 "승인", PHASE-012(열린 줄 파란 표시 갱신, 2026-10-06 "파란 표시 RefreshItems 문제도 고쳐줘"), PHASE-013(베이스 변경 시 항목 비우기 + 확인 창 + UI 정리, 2026-10-06 "네가 넣어줘. Display랑 다른 처리까지 해줘."), PHASE-014(Component~ 이름 정리, 2026-10-07, 소급 기록), PHASE-015(항목 경고 + typeName 저장 + 타입 변환 통일, 2026-10-07 "응 그렇게 맞추고 진행해"), PHASE-016(JsonUtility 깊은 복사 검증, 임시 클래스 생성 후 삭제, 2026-10-07 "응 진행해"), PHASE-017(ItemWarning 되돌리기, 2026-10-08 "되돌려줘"), PHASE-018(RegistryRuntime 사본 생성·보관·조회 + 테스트 컴포넌트, 2026-10-08 "JSON 관련 빼고는 다 앎이야 해줘" — JSON 복제 부분은 `어려움 + 모름`이라 해설 주석 없이 코드만, 나머지는 `완벽히 앎`), PHASE-019(인스펙터 패널에서 registryItem Foldout 대신 자식 필드만 그리기, 2026-10-08 "A로 하자"), PHASE-020(IInitRegistryItem → IRegistryCreatedReceiver 분리, OnRuntimeCreated, 2026-10-09 "바로 진행해"), PHASE-021(ReadOnlyField 어트리뷰트, `_Shared/Systems/InspectorSystem`, `어려움 + 모름`이라 Claude가 코드만 작성·주석은 사용자) + PHASE-022(RegistryRuntime 커스텀 에디터: 항목별/전체 적용, ReadOnly 필드 제외) — 2026-10-10 "응 그대로 진행해"
- 범위 밖 항목: 런타임 생성(`MemberwiseClone` 복제)과 `Init` 호출(사용자 담당), `RegistryRuntime` 커스텀 에디터, 테스트 스크립트·프리팹 정리, Skill/State 이전

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
| 009 | 완료 | 베이스 타입을 AdvancedDropdown으로 고르고 타입 이름 문자열로 저장 | `phases/PHASE-009.md` |
| 010 | 완료 | 항목 생성(기본 생성자 우선)·Missing 표시·[Serializable] 필터·삭제 후 키 검사 | `phases/PHASE-010.md` |
| 011 | 완료 | 베이스 후보를 IRegistryItem/IInitRegistryItem 직속 구현 타입으로 제한 | `phases/PHASE-011.md` |
| 012 | 완료 | 인스펙터 열기/닫기/추가 시 줄 파란 표시 즉시 갱신 | `phases/PHASE-012.md` |
| 013 | 완료 | 베이스 변경 시 같은 타입 무시, 확인 창, 항목 비우기, 입력 줄·인스펙터·키 오류·enum 버튼 정리 | `phases/PHASE-013.md` |
| 014 | 완료(소급 기록) | Component~ 이름 정리(RegistrySO, RegistryEntry, entries, RegistryTypeDropdown, UXML/USS 파일명) | `phases/PHASE-014.md` |
| 015 | 완료 | [Serializable] 없음·매개변수 생성자를 막지 않고 경고, typeName 저장, 타입 변환·경고 static 캐시 검사기 | `phases/PHASE-015.md` |
| 016 | 완료 | JsonUtility 깊은 복사 검증(통과, FromJson(json, type) 권장) | `phases/PHASE-016.md` |
| 017 | 완료 | ItemWarning enum·저장 필드 되돌리기(검사기는 사용자가 Runtime으로 이동) | `phases/PHASE-017.md` |
| 018 | 완료 | RegistryRuntime 사본 생성(JsonUtility)·보관(딕셔너리+인스펙터 목록)·조회, 에디터 전용 경고 로그, 테스트 컴포넌트. SO 적용 버튼은 범위 밖 | `phases/PHASE-018.md` |
| 019 | 완료 | 인스펙터 패널: registryItem Foldout(화살표가 패널 밖으로 나감) 대신 자식 필드만, 필드 없으면 안내 문구 | `phases/PHASE-019.md` |
| 020 | 완료 | 초기화 인터페이스 분리: `IRegistryCreatedReceiver.OnRuntimeCreated`(IRegistryItem 상속 제거), 베이스는 IRegistryItem 직속만 | `phases/PHASE-020.md` |
| 021 | 완료 | `ReadOnlyField` 어트리뷰트(`_Shared/Systems/InspectorSystem`, 편집 모드 허용 옵션, 리스트 통째). 코드만 Claude, 주석은 사용자 | `phases/PHASE-021.md` |
| 022 | 완료 | `RegistryRuntimeEditor`: 사본 목록(+/- 없음), 항목별·전체 적용, `[ReadOnlyField]` 필드는 적용 제외, Undo | `phases/PHASE-022.md` |

## 현재 재개 지점

- 마지막 완료 작업: enum 영역에 `skipNamespaces` PropertyField 추가(PHASE-008). 이후 사용자가 SerializeReference 방식으로 에디터를 고쳤다.
- 마지막 완료 작업(갱신): PHASE-022 RegistryRuntime 커스텀 에디터(적용 버튼), PHASE-021 ReadOnlyField. 이전: PHASE-020 초기화 인터페이스 분리. 그 전: PHASE-019 인스펙터 패널 자식 필드만 그리기. 이전: PHASE-018 RegistryRuntime 사본 생성·보관·조회 + 에디터 전용 경고 로그 + 테스트 컴포넌트. 다음 후보: 플레이 모드 화면 확인(PHASE-021/022), `ReadOnlyFieldAttribute`/`Drawer` 주석(사용자), `IsAssignableFrom` 삭제·항목 검사 통합(보류), 테스트 스크립트 정리.
- 다음에 할 작업: 사용자가 인스펙터에서 확인하고 런타임 생성/`OnRuntimeCreated`를 작성한다. 검증용 `JTH/Scripts/RegistryTest/RegistryTestItems.cs`는 확인 후 지워도 된다.
- 사용자 승인이 필요한 사항: 없음
- 관련 파일: `Assets/_Shared/Systems/RegistrySystem/Editor/`(`RegistrySOEditor.cs`, `RegistryRuntimeEditor.cs`, `RegistryTypeDropdown.cs`, `RegistryEditorUI.uxml`, `RegistryRowUI.uxml`, `RegistryEditorUIStyle.uss`), `Runtime/`(`RegistrySO.cs`, `RegistryRuntime.cs`, `RegistryItemValidator.cs`, `IRegistryItem.cs`, `IRegistryCreatedReceiver.cs`), `Assets/_Shared/Systems/InspectorSystem/`(`Runtime/ReadOnlyFieldAttribute.cs`, `Editor/ReadOnlyFieldDrawer.cs`).
- 알려진 문제 또는 위험: 기존 레지스트리 에셋은 베이스를 다시 골라야 함(`baseScriptGuid` → `baseTypeName`).

## 검증 요약

- 수행한 검증: 줄/메인 UXML 다시 임포트, 요소 이름·인라인 스타일·`--active`/`--invalid` 토글 시 표시 확인
- 통과 여부: 통과
- 아직 검증하지 못한 항목: 없음
