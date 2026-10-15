# PHASE-009 — 운영체제 커서 잠금 및 상대 이동 입력

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-15 (Asia/Seoul)`
- 사용자 승인: `승인`

## 요청과 목표

- 원래 요청: VirtualMouse 진입/종료 시 운영체제 마우스를 숨김/표시하고 숨긴 동안 화면 끝에 막히지 않게 처리
- 이 Phase의 목표: 진입 중 운영체제 커서를 숨겨 중앙에 잠그고 상대 이동량으로 가상 커서를 조작
- 완료 조건: Enter에서 숨김·잠금, Exit에서 잠금 해제·표시가 적용되고 가상 커서가 Mouse delta로 이동

## 승인된 구현 범위

- 변경 파일: `VirtualMouse.cs`, KTJ 작업 기록
- 구현 방법: `Cursor.visible`과 `Cursor.lockState`로 운영체제 커서를 제어하고 `Mouse.current.delta`를 UI 로컬 이동량으로 변환
- 검증 방법: `KTJ.csproj` 빌드, 변경 코드 정적 검사, Unity Play Mode 수동 확인
- 범위 밖 항목: 가상 커서 UI 변경, 씬·프리팹 수정, 이동 영역 및 월드 대상 매핑 변경

## 구현 결과

- 수행한 작업: Enter에서 운영체제 커서를 숨겨 중앙에 잠그고 Exit에서 잠금 해제 후 다시 표시. 입력 중에는 `Mouse.current.delta`를 월드 Canvas 로컬 이동량으로 변환해 가상 커서에 적용
- 생성·수정한 파일: `VirtualMouse.cs`, `PROGRESS.md`, `PHASE-009.md`
- 계획과 달라진 점 및 이유: 없음

## 검증

- 자동 검증: `dotnet build KTJ.csproj --no-restore`, 작업 파일 diff 및 공백 검사
- 결과: 빌드 경고 0개·오류 0개, Phase 009 작업 파일 공백 오류 없음
- 검증하지 못한 항목: Unity Play Mode 실제 커서 잠금 및 가상 커서 조작

## 다음 단계

- 남은 문제: Unity Play Mode에서 실제 커서 잠금·해제와 상대 이동 체감 확인
- 제안하는 다음 Phase: 없음
- 추가 승인이 필요한 사항: 없음
