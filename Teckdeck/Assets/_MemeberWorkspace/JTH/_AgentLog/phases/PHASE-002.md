# PHASE-002 — 다중 레이 지면 판정

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-14 02:25 (KST)`
- 사용자 승인: `PlayerGroundChecker에 리스트 위치에서 보드 아래로 레이를 쏴 전부 맞으면 IsGrounded`

## 요청과 목표

- 원래 요청: 직렬화 간격마다 로컬 위치 리스트에서 보드 아래 방향으로 레이를 쏴, 전부 히트일 때만 착지로 본다.
- 이 Phase의 목표: `PlayerGroundChecker`가 지면 판정을 담당하고, Movement는 그 결과로 `_isGrounded`와 `OnGrounded`를 갱신한다.
- 완료 조건: 모든 레이가 Ground 레이어에 맞으면 `IsGrounded == true`, 하나라도 빗나가면 false.

## 승인된 구현 범위

- 변경 예정 파일: `PlayerGroundChecker.cs`, `PlayerMovement.cs`
- 구현 방법: FixedUpdate에서 간격 누적 후 Raycast. 자기 콜라이더를 피하려고 LayerMask 사용. Movement는 GetContacts 대신 체커를 읽는다.
- 검증 방법: Unity 컴파일. Play Mode는 레이 원점·레이어 할당 후 사용자가 확인.
- 명시적으로 제외한 항목: Jump 힘 적용, 씬에 체커 배치, 애니메이션 이벤트 리시버

## 구현 결과

- 수행한 작업: 다중 레이 지면 체커 구현, Movement의 잘못된 GetContacts 제거, 착지 상승 엣지에서 OnGrounded 호출. 컴파일이 깨져 있던 Jump 본문은 가드만 남김.
- 생성·수정한 파일:
  - 수정: `Scripts/Player/PlayerGroundChecker.cs`
  - 수정: `Scripts/Player/Movement/PlayerMovement.cs`
- 계획과 달라진 점 및 이유: 자기 자신 히트를 막기 위해 기존에 있던 `groundLayer`를 유지했다. 점프 본문은 수업 코드가 섞여 컴파일이 안 되어 가드만 남겼다.

## 학습 노트

- 전체 실행 흐름: FixedUpdate마다 간격을 재고, 로컬 오프셋을 월드로 변환한 뒤 `-transform.up`으로 Raycast. 전부 히트면 착지. Movement는 그 값을 읽어 땅에서 막 닿은 프레임에 OnGrounded를 쏜다.
- 주요 클래스/메서드의 역할:
  - `PlayerGroundChecker.CheckGround`: 레이 일괄 판정
  - `PlayerMovement.CheckGround`: 체커 결과 복사 + 착지 이벤트
- UniTask 사용 위치와 이유: 사용하지 않음.
- DOTween 사용 위치와 이유: 사용하지 않음.
- 중요한 구현 원리: 레이는 물리 쿼리라 FixedUpdate에서 돌린다. 전부 맞아야 true인 이유는 보드가 한 점이 아니라 면이기 때문이다.
- 예외 상황과 대응: 원점 리스트가 비면 false. Ground 레이어가 0이면 Assert. 레이어에 플레이어가 포함되면 자기 콜라이더에 맞는다.

## 검증

- 자동 검증: Unity 컴파일 예정
- Unity Editor 수동 확인 절차:
  1. Player 하위에 PlayerGroundChecker를 붙인다
  2. Ground 레이어, 레이 원점(보드 앞/뒤 등), Ray Length를 넣는다
  3. 선택 시 노란 기즈모가 레이 경로를 보여준다
  4. Play에서 공중이면 IsGrounded false, 네 점이 모두 땅에 붙으면 true
- 결과: 기록 시점 기준 컴파일 확인 중
- 검증하지 못한 항목: Play Mode 착지 판정

## 다음 단계

- 남은 문제: Jump 힘/커브 미구현, 씬에 체커·레이어 할당
- 제안하는 다음 Phase: Jump 시 속도 적용과 JumpState 착지 전이
- 추가 승인이 필요한 사항: 씬에 GroundChecker 배치
