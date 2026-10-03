# Phase 016: Player와 이동 모듈의 구체 클래스 의존 제거

## 목표와 승인

- 사용자 KTJ가 2026-10-03에 Phase 016 계획을 승인했다.
- Player와 PlayerMovementModule 사이의 의존을 좁히고, 이동 모듈이 구체 Player 클래스를 캐스팅하지 않도록 한다.

## 변경 파일

- `Assets/_MemeberWorkspace/KTJ/02_Script/Player/Player.cs`
- `Assets/_MemeberWorkspace/KTJ/02_Script/Player/Movement/IPlayerMovementModule.cs`
- `Assets/_MemeberWorkspace/KTJ/02_Script/Player/Movement/PlayerMovementModule.cs`
- 작업 기록: `_AgentLog/PROGRESS.md`, `_AgentLog/phases/PHASE-016.md`

## 구현

- Player가 `ModuleOwner.InitializeModules()`의 공통 초기화 뒤 이동 모듈을 찾아 `Configure(transform, GetComponent<SphereCollider>())`를 호출한다.
- `IPlayerMovementModule`에 필요한 두 컴포넌트만 받는 `Configure` 계약을 추가했다.
- 이동 모듈의 `Player` 필드와 `owner as Player` 캐스팅을 제거하고, 전달받은 Transform과 SphereCollider를 사용한다.
- `IModule.Initialize(ModuleOwner owner)`는 공용 모듈 계약 때문에 유지한다. 실제 의존 주입은 Player의 구성 단계에서 수행한다.
- 벽 탐지의 중심도 주입된 Collider의 월드 중심으로 맞췄다. 현재 씬의 MovementModule 자식 위치는 원점이어서 해당 씬의 탐지 위치는 같다.
- 입력, 각도 판정, 조준선, 점프 속도와 벽 탐지 방식은 유지했다.
- UniTask·DOTween은 비동기 작업이나 연출이 없어 사용하지 않았다.

## 검증

- `dotnet build KTJ.csproj --no-restore -nologo -clp:ErrorsOnly -v:q`: 성공, 오류 0개. 기존 참조 버전 충돌 경고 8개.
- 변경 파일 `git diff --check`: 통과.
- `ModuleOwner.Awake()`에서 공통 초기화가 끝난 뒤 Player가 Configure를 호출하고, 이후 Update가 실행되는 순서를 정적 확인했다.
- Unity Play Mode에서 직접 점프 입력과 벽 접촉을 실행 검증하지 못했다.

## 범위 밖 및 위험

- 씬·프리팹·공용 ModuleOwner 파일은 수정하지 않았다.
- Player GameObject에 SphereCollider가 없으면 Configure가 예외를 던진다. 현재 PlayerTestScene에는 SphereCollider가 있다.
- Phase 015의 시작 시 벽 내부에 플레이어 중심이 들어간 경우 Raycast와 SphereCast가 놓칠 수 있는 문제는 이번 구조 정리에서 변경하지 않았다.
