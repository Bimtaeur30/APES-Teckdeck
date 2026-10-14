# PHASE-017 — ItemWarning 되돌리기

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-08 (KST)`
- 사용자 승인: `2026-10-08 "되돌려줘"`
- 배경: 런타임 경고 로그를 위해 사용자가 `[Flags] enum ItemWarning` + `RegistryEntry.warning` 저장 + 플레이 진입 전 재검사를 계획했다. 이후 사용자가 `RegistryItemValidator`를 Runtime 어셈블리로 옮기면서 런타임이 검사기를 직접 부를 수 있게 되어, 저장 필드와 훅이 필요 없어졌다. 경고 종류별로 다르게 처리할 일도 없어 enum도 빼기로 했다(검사기가 문자열만 반환).

## 구현 결과

- `Runtime/RegistrySO.cs`: `ItemWarning` enum, `RegistryEntry.warning` 필드 삭제 → HEAD(8fc1409)와 같음
- `Runtime/RegistryItemValidator.cs`: `GetItemWarning`/`GetWarningMsg(ItemWarning)` 삭제, `GetWarningMsg(Type)`이 문자열 반환 + `Dictionary<Type, string>` 캐시로 복구. 네임스페이스는 사용자가 옮긴 대로 `Runtime` 유지 (HEAD 대비 네임스페이스 한 줄만 다름)
- `Editor/RegistrySOEditor.cs` `HandleBindItem`: `SetWarningMsg(itemWarning, itemType == null ? null : RegistryItemValidator.GetWarningMsg(itemType))`로 복구
- 유지: 사용자의 검사기 Runtime 이동(git rename), `ObjectFieldError` 상수 정리, 드롭다운 주석, `RegistryRuntime` 작업 중 상태

## 검증

- Unity `AssetDatabase.Refresh` 후 `recompile_status`: 오류 0
- grep: `ItemWarning`/`GetItemWarning` 참조 없음
- 기존 SO 에셋에 `warning:` 줄이 저장돼 있었다면 유니티가 무시하고 다음 저장 때 지운다.

## 다음

- 사용자가 `RegistryRuntime`에 `FromJson` 복제와 경고 로그(`RegistryItemValidator.GetWarningMsg`)를 작성한다.
