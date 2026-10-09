# PHASE-011 — 베이스 후보를 직속 구현 타입으로 제한

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-06 (KST)`
- 사용자 승인: `2026-10-06 "승인"` (IRegistryItem, IInitRegistryItem 자신은 "둘 다 빼기")
- 작업 방식: 사용자 판정 `완벽히 앎`. Claude가 작성, 사용자 검수.

## 요청과 목표

- 베이스 타입은 `IRegistryItem` 또는 `IInitRegistryItem`을 직접 구현(선언)한 타입만 고를 수 있다. 부모 클래스나 다른 인터페이스를 거쳐 구현한 타입은 제외. 두 인터페이스 자신도 제외.

## 승인된 구현 범위

- `Editor/ComponentRegistrySOEditor.cs`
  - `IsBaseTypeSelectable`: 두 인터페이스 자신 제외, 열린 제네릭·`UnityEngine.Object` 제외, `GetDirectInterfaces`에 두 인터페이스 중 하나가 있어야 true
  - `GetDirectInterfaces(Type)`: `GetInterfaces()` − 부모 클래스의 인터페이스 → 그중 다른 인터페이스가 이미 포함하는 것 제거
  - `FillBaseTypeField` 오류 문구: "IRegistryItem 또는 IInitRegistryItem을 직접 구현한 타입이어야 합니다: …"
  - `HandleAddBtn`: 저장된 베이스가 이 기준에 안 맞으면 열지 않음
- 검증용 `JTH/Scripts/RegistryTest/RegistryTestItems.cs`에 `IRegistryTestSkill`, `RegistryTestViaInterfaceItem`, `RegistryTestInitBase`, `RegistryTestInitItem` 추가

## 검증

- Unity CLI `recompile`: 완료, 컴파일 오류 0(콘솔의 `UnityConnectWebRequestException`은 Unity 계정 토큰 교환 오류로 무관).
- `eval_file`: 베이스 후보 = `IRegistryTestSkill`, `RegistryTestInitBase`, `RegistryTestItemBase`
  - 빠진 것: `IRegistryItem`, `IInitRegistryItem`(자신), `RegistryTestDefaultCtorItem`·`ArgsCtorItem`·`NotSerializableItem`·`InitItem`(부모 클래스 경유), `RegistryTestViaInterfaceItem`(인터페이스 경유)
- 직속이 아닌 `RegistryTestDefaultCtorItem`이 저장돼 있을 때 오류 문구 표시 확인. 테스트 에셋 값은 원래대로 복구(dirty 아님).

## 한계

- 리플렉션은 소스에 적은 목록과 상속으로 들어온 인터페이스를 구분하지 못한다. 그래서 `class A : B, IRegistryItem`(B가 이미 구현)이나 `class A : IRegistryItem, ISkill`(ISkill이 이미 IRegistryItem 상속)은 직속으로 보지 않는다.

## 다음 단계

- 사용자가 인스펙터에서 확인한다.
