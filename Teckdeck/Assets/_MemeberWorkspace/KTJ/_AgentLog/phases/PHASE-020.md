# Phase 020: 차징 UI를 너비 0부터 표시

## 목표와 승인

- KTJ가 2026-10-07에 제시한 계획을 `진행`으로 승인했다.
- 점프 배율 1~최댓값은 유지하면서 UI 표시 비율을 0~1로 변환한다.

## 변경 파일

- `02_Script/Player/Movement/JumpCharging/ChargingUI.cs`
- `_AgentLog/PROGRESS.md`, `_AgentLog/phases/PHASE-020.md`

## 구현

- `Charge`가 `(current - 1) / (max - 1)`을 0~1로 제한해 최대 UI 너비에 곱한다.
- `max <= 1`일 때는 나누기 대신 0을 사용한다.
- `Set`이 이미지를 활성화하기 전에 가로 너비를 0으로 초기화한다.
- `Player`의 점프 배율과 씬·프리팹은 변경하지 않았다.

## 검증

- 코드 정적 확인: `current=1, max=3`에서 비율 0, `current=2`에서 0.5, `current=3`에서 1. 초기화가 활성화보다 먼저 실행됨.
- `dotnet build KTJ.csproj --no-restore -nologo -v:q`: 실패(종료 코드 1). 오류·경고 0개로 출력되지만 MSBuild의 `_GetProjectReferenceTargetFrameworkProperties` 프로젝트 참조 단계가 실패함. 코드 컴파일 성공으로 간주하지 않음.
- Unity Play Mode에서 실제 첫 프레임 표시와 차징 동작은 확인하지 못했다.
