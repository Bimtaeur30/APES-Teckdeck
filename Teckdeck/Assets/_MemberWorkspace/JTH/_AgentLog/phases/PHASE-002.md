# PHASE-002 — UI만 남기기

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-03 00:05 (KST)`
- 사용자 승인: `2026-10-02 "UI만 남기고 코드 삭제" 선택`

## 요청과 목표

- 원래 요청
  - 처음 요청: 항목을 만들 때 타입을 먼저 고르게 하고, 프리팹 폴더는 자동으로 만든다.
  - 진행 중 정정: "UI만 만드는 거 맞아? 코드는 내가 짜야 함"
- 이 Phase의 목표: Claude가 작성한 C# 코드를 지우고 UI(UXML/USS)만 남긴다. 목록 한 줄의 모양도 UXML 템플릿으로 옮긴다.
- 완료 조건: RegistrySystem에 UXML/USS와 asmdef만 남고, 컴파일 에러가 없다.

## 승인된 구현 범위

- 변경 예정 파일: `RegistrySystem/Editor/RegistryRowUI.uxml`(신규). C# 파일과 이 코드에 기대는 테스트 에셋은 삭제한다.
- 구현 방법: Unity CLI로 `AssetDatabase.MoveAssetToTrash`. OS 휴지통에서 되살릴 수 있다.
- 명시적으로 제외한 항목: C# 로직 작성. 사용자가 직접 짠다.

## 구현 결과

- 수행한 작업
  - `RegistryRowUI.uxml`을 만들었다. 이전 `RegistryRow.cs`가 코드로 만들던 줄 모양이다.
  - 아래 파일을 휴지통으로 옮겼다.
    - `RegistrySystem/Runtime/ComponentRegistrySO.cs`
    - `RegistrySystem/Editor/ComponentRegistryEditor.cs`, `RegistryRow.cs`, `ComponentTypeDropdown.cs`, `RegistryEnumGenerator.cs`
    - 테스트 에셋 `JTH/GameModules/RegistryTest/Test registry.asset`, 생성된 enum `JTH/Scripts/RegistryTest/RegistryTestSkillType.cs`
- 남은 파일
  - `RegistryEditorUI.uxml`: 메인 배치. 프리팹 폴더 칸은 자동 생성 결정에 따라 뺐다.
  - `RegistryRowUI.uxml`: 줄 템플릿.
  - `RegistryEditorUIStyle.uss`: 두 UXML이 함께 쓰는 스타일.
  - asmdef 2개: 스크립트가 없어 지금은 컴파일되지 않는다는 경고가 뜬다.
  - 테스트용 컴포넌트(`JTH/Scripts/RegistryTest`)와 프리팹(`JTH/GameModules/RegistryTest/Prefabs`): 코드와 상관없어 남겼다. 직접 짠 코드를 시험할 때 쓰거나 지우면 된다.
- 계획과 달라진 점 및 이유: 타입 먼저 고르기와 폴더 자동 생성은 코드 수정까지 했지만, 코드를 지우면서 함께 사라졌다. 결정 내용은 아래에 남긴다.

## 추가 변경 (같은 날)

- UI Toolkit 이름 규칙에 맞췄다(사용자 요청). UXML `name` 값과 USS 클래스는 소문자-하이픈, 파일 이름은 PascalCase.
  - 파일: 사용자 정정에 따라 PascalCase로 `RegistryEditorUI.uxml`, `RegistryRowUI.uxml`, `RegistryEditorUIStyle.uss`. `RenameAsset`으로 바꿔 GUID를 유지했다.
  - `name` 예: `root-script-field`, `entry-list`, `key-label`. USS 클래스는 원래 BEM 소문자-하이픈이라 그대로 뒀다.
  - 대문자가 남은 값: `type` 속성의 C# 타입 이름, `binding-path="enumName"`(C# 필드 이름과 맞춰야 함), 화면 표시 글자.
- 확인: 두 UXML을 Instantiate해서 새 `name` 값과 USS 연결, ObjectField 타입을 확인했다.

## 학습 노트 (직접 구현할 때 참고할 결정 사항)

- 추가 순서: `+` → 타입 드롭다운(검색) → 맨 아래 입력 줄(타입 버튼으로 다시 고를 수 있음) → 키 입력 → Enter로 생성, Esc로 취소. 드롭다운을 그냥 닫으면 아무것도 생기지 않는다.
- 프리팹 폴더: 첫 추가 때 레지스트리 옆에 레지스트리와 같은 이름의 폴더를 만들고 GUID로 기억한다. 같은 이름의 폴더가 이미 있으면 남의 것일 수 있으니 새 이름으로 만든다. 폴더 지정은 Undo 대상에서 뺀다.
- PHASE-001에서 확인한 함정
  - 스크립트 리로드 때 에디터의 private 필드도 직렬화되어 돌아온다(null 문자열은 ""가 된다). UI를 새로 만들 때 상태를 초기화한다.
  - ListView는 줄 요소를 재사용한다. 이벤트는 makeItem에서 한 번만 걸고, 인덱스를 캡처하지 않는다.
  - 입력 모드에서 줄 높이가 바뀌므로 `DynamicHeight`가 필요하다.
  - 조작마다 `Undo.IncrementCurrentGroup()`으로 그룹을 나누지 않으면 여러 조작이 Undo 한 번에 되돌아갈 수 있다.
  - `t:Prefab` 검색에는 모델 파일이 섞일 수 있다.
  - XML 주석 안에는 `--`를 쓸 수 없다. 클래스 이름 `registry-row--invalid` 같은 것을 주석에 쓰면 UXML 전체가 깨진다.
- UniTask/DOTween: 사용하지 않음.

## 검증

- 자동 검증: 삭제 뒤 컴파일 실패 없음. 두 UXML을 Instantiate해서 요소 이름과 스타일시트가 붙는지 확인했다.
- Unity Editor 수동 확인 절차: UI Builder에서 두 UXML을 열어 배치를 본다.
- 결과: 통과.
- 검증하지 못한 항목: 없음(이 Phase에는 동작 코드가 없다).

## 다음 단계

- 남은 문제: 없음.
- 제안하는 다음 Phase: 사용자가 에디터 코드를 직접 작성한다. Claude는 막힌 지점에만 답한다.
- 추가 승인이 필요한 사항: 없음.
