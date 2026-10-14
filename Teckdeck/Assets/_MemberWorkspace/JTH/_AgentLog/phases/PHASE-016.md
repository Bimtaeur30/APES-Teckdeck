# PHASE-016 — JsonUtility 깊은 복사 검증

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-07 (KST)`
- 사용자 승인: `2026-10-07 "응 진행해"` (임시 테스트 클래스 파일 생성 → 검증 → 삭제)
- 목적: 런타임 사본을 SO 객체의 복제로 만들 때, `MemberwiseClone`(얕은 복사) 대신 JsonUtility로 깊은 복사가 되는지 확인. 런타임 복제 코드는 사용자 담당.

## 진행

- eval 스크립트 안에서는 클래스를 선언할 수 없다(코드가 static 클래스의 `Execute()` 안으로 들어감). 그래서 `JTH/Scripts/RegistryTest/RegistryTestJsonCopy.cs`에 테스트 클래스(`JcSub`, `JcSubDerived`, `JcPlain`, `JcItem`)를 만들었다가 검증 후 `AssetDatabase.DeleteAsset`으로 지웠다(.meta 포함, 재컴파일 오류 0).
- 두 방식 비교: A `JsonUtility.FromJson(json, type)`, B `RuntimeHelpers.GetUninitializedObject` + `JsonUtility.FromJsonOverwrite`.

## 결과 (A, B 같음. 9번만 다름)

| # | 항목 | 결과 |
|---|---|---|
| 1 | 값 필드 | 복사됨 |
| 2 | `List<int>` | 다른 객체. 사본에 Add해도 원본 그대로 |
| 3 | `GameObject` 에셋 참조 | 같은 에셋 유지(JSON에는 `instanceID`) |
| 4 | 일반 `[Serializable]` 클래스 필드 | 다른 객체로 복사 |
| 5 | `[SerializeReference]` 필드 | 다른 객체로 복사 |
| 6 | 두 필드가 같은 객체를 가리킬 때 | 사본에서도 같은 객체(공유 관계 유지) |
| 7 | 다형성(파생 타입) | 파생 타입과 필드 유지 |
| 8 | `[SerializeReference]` 리스트 | 원소가 다른 객체로 복사 |
| 9 | `[NonSerialized]` 필드(초기화식 7, 원본 99) | A: 7(기본 생성자 실행), B: 0(생성자·초기화식 안 돎) |
| 10 | 매개변수 생성자만 있는 타입 `FromJson` | 예외 없이 생성됨 |

## 결론

- JsonUtility로 깊은 복사가 된다. `FromJson(json, type)`이 더 낫다: 기본 생성자가 있으면 실행해서 직렬화 안 되는 필드의 초기화식도 살고, 매개변수 생성자만 있는 타입도 실패하지 않는다.
- 생성자 경고는 지금 문구("호출되지 않습니다") 그대로 맞다.

## 추가 검증: 매개변수 생성자만 있는 타입 (2026-10-07 "응 확인해봐")

- 임시 클래스 `JcArgsOnly`(`public int s = 3;`, `[NonSerialized] public int x = 7;`, 생성자 `(int a)`만 있음)를 같은 파일에 만들었다가 지웠다(재컴파일 오류 0).
- 원본 `new JcArgsOnly(99) { s = 5 }` → `ToJson` = `{"s":5}`
- `FromJson`: `s=5`, `x=0` → 생성자도 필드 초기화식도 실행되지 않음. `GetUninitializedObject`와 같은 결과.
- `FromJson("{}")`: `s=0` → JSON에 없는 필드는 직렬화 필드라도 초기화식 값이 아니라 0. (전체 객체를 `ToJson`한 문자열에는 모든 직렬화 필드가 들어가므로 복제에는 영향 없음)
- 결론: 기본 생성자가 없으면 FromJson은 생성자 없이 만들고 JSON 값만 채운다. 이런 타입은 직렬화 안 되는 필드가 0/null로 시작하므로 `Init`에서 준비해야 한다(현재 경고 문구와 일치).

## 검증하지 못한 항목

- 에디터(편집 모드)에서만 확인했다. 플레이 모드·빌드(IL2CPP)에서 `instanceID` 참조 복원과 스트리핑 영향은 확인하지 않았다.
