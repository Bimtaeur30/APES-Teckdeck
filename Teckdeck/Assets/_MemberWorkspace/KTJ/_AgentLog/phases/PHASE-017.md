# Phase 017: FSM 어셈블리 순환 참조 해결

## 목표와 승인

- 사용자 KTJ가 2026-10-03에 공용 FSM 파일 변경을 별도로 승인했다.
- `FsmSystem_runtime_assembly`와 `KTJ` 사이의 양방향 어셈블리 참조를 제거한다.

## 원인

- 공용 FSM 런타임 asmdef에 KTJ GUID `3a99eee98c67f0d478dede79d916c212`가 추가되어 있었다.
- KTJ.asmdef는 이미 `FsmSystem_runtime_assembly`를 참조하므로 두 어셈블리가 서로를 참조했다.
- `IStateTransition`과 `IStateEnter<TData>`가 KTJ 폴더에 있어 공용 FSM이 KTJ를 참조하게 된 것이 직접 원인이다.

## 변경 파일

- `Assets/_MemeberWorkspace/KTJ/02_Script/Player/FSM/IStateTransition.cs`와 `.meta`를 `Assets/_Shared/Systems/FsmSystem/Runtime/`으로 이동하여 GUID를 보존했다.
- `Assets/_Shared/Systems/FsmSystem/Runtime/FsmSystem_runtime_assembly.asmdef`: KTJ 참조 제거.
- `Assets/_Shared/Systems/FsmSystem/Runtime/StateMachine.cs`: 생성한 상태에 `BindTransition(this)` 호출.
- `Assets/_Shared/Systems/FsmSystem/Runtime/AbstractState.cs`: 사용하지 않는 `Unity.VisualScripting` using 제거.
- 작업 기록: `_AgentLog/PROGRESS.md`, `_AgentLog/phases/PHASE-017.md`.

## 검증

- asmdef 참조 방향: `KTJ -> FsmSystem_runtime_assembly -> AnimatorSystem_assembly, ModuleSystem`으로 확인했다.
- `dotnet build FsmSystem_runtime_assembly.csproj --no-restore -nologo -clp:ErrorsOnly -v:q`: 성공, 오류 0개, 기존 참조 경고 6개.
- `dotnet build KTJ.csproj --no-restore -nologo -clp:ErrorsOnly -v:q`: 재생성된 프로젝트로 성공, 오류 0개, 기존 참조 경고 12개. 첫 시도는 프로젝트 재생성 중 MSBuild 그래프 오류가 났고 이후 재시도는 통과했다.
- Unity Editor.log의 최신 컴파일 구간에서 어셈블리 재로드를 확인했고, 그 구간에 순환 참조 또는 C# 오류는 보이지 않았다. 이전 로그 구간의 순환 오류는 기록으로 남아 있다.
- 변경 파일 `git diff --check`: 통과.

## 범위 밖 및 남은 확인

- KTJ 상태 클래스의 동작, 씬·프리팹, ModuleOwner는 수정하지 않았다.
- Unity Play Mode의 실제 상태 전환은 이번 컴파일 복구에서 실행 검증하지 않았다.
