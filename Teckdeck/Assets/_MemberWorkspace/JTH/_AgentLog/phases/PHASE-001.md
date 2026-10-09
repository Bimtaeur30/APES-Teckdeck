# PHASE-001 — Registry 에디터

## 기본 정보

- 상태: `완료` (C# 코드는 사용자 요청으로 PHASE-002에서 삭제했다. UXML/USS만 남았다.)
- 작성/갱신 일시: `2026-10-02 23:20 (KST)`
- 사용자 승인: `2026-10-02 "위 계획대로 진행" 선택. 판정은 완벽히 앎(검수만)`

## 요청과 목표

- 원래 요청: 지금까지 정리한 레지스트리 에디터 UI(빨간 줄 표시 포함)를 `Registry Editor UI`에 만든다. 에디터만 만든다.
- 이 Phase의 목표: 컴포넌트 프리팹을 키로 관리하는 레지스트리 인스펙터와 enum 생성까지 동작하게 한다. 런타임 `Build`는 넣지 않는다.
- 완료 조건: 컴파일 에러 없음. 추가, 이름 변경, 삭제, Undo, 열 때 정리, 빨간 줄, 순서 변경, 인스펙터 탭, enum 생성이 동작한다.

## 승인된 구현 범위

- 변경 예정 파일: `Assets/_Shared/Systems/RegistrySystem/` 아래 Runtime SO 1개, Editor 스크립트, UXML/USS. JTH 폴더에는 테스트용 컴포넌트를 둔다.
- 구현 방법: UI Toolkit `ListView`(바인딩 없이 인덱스 목록 사용) + `SerializedProperty`로 데이터 변경, `AdvancedDropdown`, `InspectorElement`.
- 검증 방법: Unity CLI(`unity command eval_file`, `recompile`)로 실제 인스펙터의 에디터 인스턴스를 조작하고 상태와 화면을 확인한다.
- 명시적으로 제외한 항목: 런타임 `Build`, 플레이 중 반영, Skill/State 이전.

## 구현 결과

- 수행한 작업: 레지스트리 SO, 커스텀 에디터, 줄 요소, 타입 드롭다운, enum 생성기, UXML/USS를 작성했다.
- 생성·수정한 파일
  - `_Shared/Systems/RegistrySystem/Runtime/ComponentRegistrySO.cs`: `RegistryEntry`(키, 번호, 프리팹)와 레지스트리 SO. 에디터 설정은 GUID 문자열로 저장한다. `OnValidate`에서 겹치거나 빈 번호에 새 번호를 준다.
  - `_Shared/Systems/RegistrySystem/Editor/ComponentRegistryEditor.cs`(+ `.meta`에 UXML 기본 참조)
  - `_Shared/Systems/RegistrySystem/Editor/RegistryRow.cs`
  - `_Shared/Systems/RegistrySystem/Editor/ComponentTypeDropdown.cs`
  - `_Shared/Systems/RegistrySystem/Editor/RegistryEnumGenerator.cs`
  - `_Shared/Systems/RegistrySystem/Editor/Registry Editor UI.uxml`(빈 파일을 채움), `Registry Editor UI.uss`(+ `.meta`)
  - 테스트용(지워도 됨): `JTH/Scripts/RegistryTest/` 스크립트 3개와 생성된 `RegistryTestSkillType.cs`, `JTH/GameModules/RegistryTest/`(Test registry, Prefabs)
- 계획과 달라진 점 및 이유
  - 줄 요소를 `RegistryRow.cs`로 분리했다. 보기 모드와 입력 모드 전환을 한곳에 모으기 위해서다.
  - 조작마다 Undo 그룹을 나누고 이름을 붙였다(`ApplyWithUndo`). CLI로 연속 조작할 때 모든 조작이 Undo 한 번에 되돌아가는 현상을 발견해서 보강했다.
  - 스크립트 리로드 뒤 에디터 상태를 초기화하는 `ResetState`를 넣었다. private 필드가 리로드를 거치며 남아(null 문자열은 ""가 됨), "'' 레지스트리와 겹칩니다"가 잘못 뜨는 버그를 찾았기 때문이다. 열려 있던 인스펙터 탭은 리로드 뒤 다시 연다.
  - 정리 대상은 `.prefab` 확장자로 한정했다. `t:Prefab` 검색에 모델 파일이 섞일 수 있어서다.

## 학습 노트

- 전체 실행 흐름: 에셋을 선택하면 `CreateInspectorGUI` → UXML 복제 → 필드와 리스트 설정 → `Refresh` → `delayCall`로 주인 없는 프리팹 정리. 데이터가 바뀌면(Undo 포함) `TrackSerializedObjectValue`가, 프로젝트 파일이 바뀌면 `projectChanged`가 `Refresh`를 부른다.
- 주요 클래스/메서드의 역할
  - `Refresh`: 데이터를 다시 읽고 줄 오류, 설정 오류, 인스펙터 탭, enum 상태, 버튼을 갱신한다.
  - `BindRow`: 재사용되는 줄 요소에 인덱스를 넣고 보기/입력 모드를 고른다.
  - `CreateEntry`: 숨김 임시 GameObject에 컴포넌트를 붙여 프리팹으로 저장하고, 새 번호로 항목을 추가한다.
  - `CleanupOrphanPrefabs`: 항목이 참조하지 않는 프리팹을 찾으면 `Undo.ClearUndo(레지스트리)` 뒤 휴지통으로 옮긴다.
  - `RegistryEnumGenerator`: 식별자 검증, 폴더 경로로 네임스페이스 추측, 코드 생성. 파일 머리에 레지스트리 GUID를 적어 다른 파일을 덮어쓰지 않는다.
- UniTask 사용 위치와 이유: 없음(에디터 동기 코드).
- DOTween 사용 위치와 이유: 없음.
- 중요한 구현 원리
  - 번호는 항목에 붙고 `lastValue`는 줄어들지 않는다. 그래서 순서를 바꾸거나 지워도 기존 번호가 유지되고, 지운 번호는 재사용되지 않는다.
  - ListView는 바인딩하지 않고 인덱스 목록을 쓴다. 순서를 바꾸면 `itemIndexChanged`에서 `MoveArrayElement`로 실제 데이터를 옮기므로 Undo가 정상 기록된다.
  - 줄 요소는 재사용되므로 이벤트는 `makeItem`에서 한 번만 걸고, 인덱스는 `row.Index`로 읽는다.
  - 입력 모드에서 줄 높이가 바뀌므로 `DynamicHeight`를 쓴다.
- 예외 상황과 대응
  - 잘못된 키, 중복 키, 프리팹 없음, 루트 타입 컴포넌트 없음, 같은 프리팹 중복 → 빨간 줄과 이유를 표시하고 enum 생성을 막는다.
  - 다른 레지스트리와 프리팹 폴더가 겹치면 정리하지 않는다.
  - 같은 이름의 enum 파일이 이 레지스트리가 만든 게 아니면 생성을 막는다.

## 검증

- 자동 검증(Unity CLI, 실제 인스펙터의 에디터 인스턴스 대상)
  - 컴파일 에러 없음.
  - 키 검증: 빈 값, `1abc`, `class`, `Fire ball`, 중복 키 → 각각 오류 문구.
  - 추가: 프리팹 생성, 초기값(cooldown=1 등) 유지, 같은 타입 두 번째는 `RegistryTestFireball 1.prefab`. 씬이 더러워지지 않고 임시 오브젝트도 남지 않음.
  - 이름 변경: 입력 모드 전환, 실시간 오류, Enter 확정, Esc 취소, 잘못된 값은 확정되지 않음.
  - 순서 변경 후 번호 유지. enum 생성 결과 확인, 생성 뒤 버튼이 "enum 최신"으로 바뀜.
  - 삭제 → Undo로 복구(프리팹 참조 정상) → 다시 삭제 → 정리: 프리팹이 휴지통으로 이동하고, 그 뒤 Undo로 줄이 살아나지 않음.
  - 빨간 줄: 잘못된 키, 같은 프리팹, 프리팹 삭제 → 표시와 enum 차단. 프리팹 없는 줄은 인스펙터가 열리지 않음.
  - 인스펙터 탭: 열기, 제목, 줄 강조, enum 생성으로 리로드된 뒤 다시 열림. 탭에서 고친 값이 프리팹 파일에 저장됨.
  - 타입 드롭다운 목록: 추상 클래스는 빠지고 구체 타입 2개만 나옴.
  - 인스펙터 화면 캡처로 배치 확인.
- Unity Editor 수동 확인 절차
  1. `JTH/GameModules/RegistryTest/Test registry`를 선택한다.
  2. 키를 더블클릭해서 이름 바꾸기, 타입 칸을 눌러 인스펙터 열고 닫기, 줄 드래그를 해본다.
  3. `+` → 키 입력 → Enter(타입 드롭다운이 열림) → 검색 후 선택 → 항목이 생성되는지 본다.
  4. `-`로 지운 뒤 다른 에셋을 선택했다가 다시 선택하면, 콘솔에 정리 로그가 뜨고 프리팹이 휴지통으로 가는지 본다.
- 결과: 위 자동 검증 항목은 모두 통과했다.
- 검증하지 못한 항목
  - 실제 마우스/키보드로 하는 조작(더블클릭, 타입 칸 클릭, 드래그, 드롭다운 팝업 표시와 검색, 다른 곳 클릭 시 확정): CLI로는 이 이벤트를 만들 수 없어 해당 메서드를 직접 호출해서 확인했다.
  - `delayCall` 정리 시점: 테스트 중 유니티 창이 백그라운드여서 `delayCall`이 돌지 않았고, 정리 함수를 직접 호출해서 확인했다.

## 다음 단계

- 남은 문제
  - enum 이름이나 enum 폴더를 바꾸면 이전 enum 파일은 남는다.
  - 키는 영문 식별자만 허용한다(한글 키 불가).
  - 정리 시 그 레지스트리의 Undo 기록이 전부 지워진다(합의된 동작).
- 제안하는 다음 Phase: 런타임 `Build(Transform parent)`와 enum 키 테이블.
- 추가 승인이 필요한 사항: 다음 Phase 착수, 테스트 에셋 정리 여부.
