# PHASE-015 — 항목 경고와 typeName 저장

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-07 (KST)`
- 사용자 승인: `2026-10-07 "그 방향으로 가자. 완벽히 앎이니까 맡길게."`, 캐시·타입 변환 방식 조정 후 `"응 그렇게 맞추고 진행해"`
- 작업 방식: 사용자 판정 `완벽히 앎`. `RegistryEntry.typeName` 필드는 사용자가 먼저 추가했고(커밋 648dfc2), Claude는 에디터 쪽을 맡았다.

## 결정 배경

- 런타임은 SO 객체를 `MemberwiseClone`으로 복제하고 초기화는 `Init`에서 한다. 생성자는 런타임에 호출되지 않는다.
- `[Serializable]` 없음, 매개변수 있는 생성자를 추가 단계에서 막으면 왜 안 되는지 보이지 않는다 → 막지 않고 경고.
- `[Serializable]`이 없으면 SerializeReference가 저장하지 못한다 → `registryItem = null` + `typeName`으로 어떤 타입이었는지 남긴다.

## 구현 결과

- `Editor/RegistryItemValidator.cs` (신규, static)
  - `GetTypeName(Type)`: `"FullName, Assembly"` 형식(`baseTypeName`과 같음)
  - `ResolveType(string)`: `Type.GetType` 결과를 `Dictionary<string, Type>`에 캐시(못 찾은 null도 캐시)
  - `IsCandidate(Type)`: 추상·인터페이스, 제네릭, `UnityEngine.Object` 제외
  - `GetWarningMsg(Type)`: `[Serializable]` 없음, 매개변수 있는 생성자(private 포함). `Dictionary<Type, string>`에 캐시
  - 캐시 무효화 없음: 타입 정보는 재컴파일 때만 바뀌고 그때 도메인 리로드로 static이 초기화된다. 캐시 키가 Type이라 같은 타입 항목이 여러 개여도 한 번만 검사한다.
- `Editor/RegistryTypeDropdown.cs`
  - `IsAddable` → `RegistryItemValidator.IsCandidate` (`IsSerializable` 필터 제거)
  - 항목 추가용(`isAddable == null`)일 때만 경고 있는 타입에 `console.warnicon.sml` 아이콘
- `Editor/RegistrySOEditor.cs`
  - `SyncEntryTypes()`: `CreateInspectorGUI`에서 한 번. 객체가 있으면 `typeName`을 `GetType()` 기준으로 맞춤(빈 값·[MovedFrom] 후 낡은 값). null인데 타입을 찾고 `[Serializable]`이 있으면 인스턴스 재생성(복구). 바뀌면 `SetDirty`, Undo 기록 없음. 재생성 실패는 `Debug.LogWarning`
  - `GetEntryType(entry)`: `registryItem.GetType()` 우선, 없으면 `ResolveType(typeName)`
  - `GetEntryTypeDisplayName`: 타입 이름 → 못 찾으면 저장된 이름의 짧은 이름 → 둘 다 없으면 `Missing`
  - `HandleBindItem`: 타입 null이면 기존 빨간 오류, 있으면 노란 경고(`warning` Label)
  - `OnEntryFocus`: null 항목은 제목을 타입 이름으로, 본문은 오류/경고(경고도 없으면 `NoInstanceMsg`)
  - `TryAddEntry`: `IRegistryItem` 구현 여부를 먼저 검사, `[Serializable]` 없으면 null로 추가, `typeName` 저장
  - `GetBaseType`, `HandleBaseTypeSelected`가 검사기의 `ResolveType`/`GetTypeName` 사용
  - `SetWarningMsg(Label, msg)`: `registry__warning--visible` 토글
- `Editor/RegistryRowUI.uxml`: `<ui:Label name="warning" class="registry__warning"/>` (error 아래)
- `Editor/RegistryEditorUIStyle.uss`: `.registry__warning`(노란색, 기본 숨김), `.registry__warning--visible`

## 검증

- Unity CLI `recompile`: 오류 0.
- `eval_file` (임시 에셋 `JTH/GameModules/RegistryTest/Phase015Temp.asset`을 만들고 끝나고 삭제, Undo 기록 정리. 기존 테스트 에셋은 건드리지 않음)
  - 검사기: NotSerializable → Serializable 경고, ArgsCtor → 생성자 경고, DefaultCtor → null. 추상 베이스는 후보 아님. 빈 문자열 → null
  - Sync: 빈 `typeName` 채움, 낡은 `Old.Name` 교정, `[Serializable]` 있는 null 항목(Fix) 인스턴스 복구
  - 줄: NotSerializable/ArgsCtor 노란 경고, 못 찾은 타입은 `GoneItem` + 빨간 오류, `typeName` 없는 옛 데이터는 `Missing` + 빨간 오류
  - 인스펙터 패널: NotSerializable 제목·경고, 못 찾은 타입 제목·오류
  - `TryAddEntry`로 NotSerializable 추가 → null + typeName으로 추가됨
  - 드롭다운: ArgsCtor·NotSerializable에 아이콘, DefaultCtor는 없음
  - 저장된 YAML: null 항목은 `rid: -2` + `typeName` 유지
- 검증하지 못한 항목: 실제 인스펙터 화면의 색·배치, 드롭다운 아이콘 모양(CLI로 화면을 보지 않음). 도메인 리로드 후 다시 불러온 상태는 디스크 YAML로만 확인.
