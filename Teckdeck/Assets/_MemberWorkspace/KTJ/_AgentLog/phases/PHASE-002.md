# PHASE-002 — 월드 Canvas 가상 마우스 UI 구현

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-13 23:47 (Asia/Seoul)`
- 사용자 승인: `승인`

## 요청과 목표

- 원래 요청: 마우스를 움직이면 따라 움직이고 Canvas를 벗어나지 않는 가상 마우스 UI를 프로젝트에 적용
- 이 Phase의 목표: `ComputerScreenCanvas`의 `Image/Cursor`를 마우스 이동량으로 움직이고 전체 시각 요소를 이동 영역 안에 제한
- 완료 조건: 스크립트와 씬 참조가 연결되고 정적 컴파일·직렬화 검증을 통과

## 승인된 구현 범위

- 변경 예정 파일: `VirtualMouseUI.cs`, `KTJ.asmdef`, `RepairShop_KTJ.unity`, KTJ 작업 기록
- 구현 방법: Input System 마우스 화면 좌표를 월드 Canvas의 로컬 좌표로 변환하고 커서 계층 Bounds를 이동 영역 Rect 안으로 Clamp
- 검증 방법: C# 컴파일 검사, asmdef 및 씬 직렬화 참조 검사, Unity Editor 수동 Play Mode 확인 절차 제공
- 명시적으로 제외한 항목: 클릭 처리, 시스템 커서 숨김·잠금, 버튼 상호작용, CamSwitcher 수정

## 구현 결과

- 수행한 작업: Input System 기반 마우스 좌표 변환, 커서 계층 Bounds 경계 제한, 기존 월드 Canvas와 커서 씬 참조 연결
- 생성·수정한 파일: `VirtualMouseUI.cs`와 meta 생성, `KTJ.asmdef`, `RepairShop_KTJ.unity`, KTJ 작업 기록 수정
- 계획과 달라진 점 및 이유: Canvas의 Event Camera를 직접 변경하지 않고 스크립트가 `worldCanvas.worldCamera`를 우선 사용하고 비어 있으면 `Camera.main`을 사용하도록 구성. 기존 씬 설정 변경을 최소화하기 위함

## 학습 노트

- 전체 실행 흐름: `OnEnable`에서 이전 마우스 위치 저장 → `Update`에서 현재 위치 취득 → 두 화면 좌표를 이동 영역 로컬 좌표로 변환 → 차이만큼 커서 이동 → 이동 영역 Bounds 안으로 보정
- 주요 클래스/메서드의 역할: `Mouse.current`는 새 Input System 마우스 입력을 읽고, `ScreenPointToLocalPointInRectangle`은 화면 좌표를 월드 Canvas 로컬 좌표로 바꾸며, `CalculateRelativeRectTransformBounds`는 커서 자식 이미지까지 포함한 Bounds를 계산
- UniTask 사용 위치와 이유: 프레임별 동기 입력 처리이므로 사용하지 않음
- DOTween 사용 위치와 이유: 즉시 마우스 추적이 목적이며 시간 기반 보간 연출이 아니므로 사용하지 않음
- 중요한 구현 원리: 화면 픽셀 이동량을 `anchoredPosition`에 직접 더하지 않고 동일한 UI 로컬 좌표계에서 이동량을 계산해야 월드 Canvas의 크기와 카메라 원근을 반영할 수 있음
- 예외 상황과 대응: 마우스 장치가 없으면 갱신하지 않으며, Canvas의 Event Camera가 비어 있으면 Main Camera를 사용

## 검증

- 자동 검증: asmdef JSON과 Input System 참조 확인, 스크립트 meta GUID와 씬 컴포넌트 GUID 일치 확인, Canvas·이동 영역·커서 fileID 연결 확인, 기존 생성 `KTJ.csproj` 빌드
- Unity Editor 수동 확인 절차: Unity로 돌아가 임포트 완료를 기다리고 Console 오류가 없는지 확인한 뒤 `RepairShop_KTJ` 씬을 Play. 마우스를 네 방향으로 움직여 커서 이미지가 따라오고 `Image` 경계를 넘지 않는지 확인
- 결과: 정적 참조 검증 통과, 기존 생성 프로젝트 빌드 경고 0개·오류 0개
- 검증하지 못한 항목: 프로젝트가 이미 Unity Editor에서 열려 있어 새 스크립트를 포함한 별도 배치 임포트·컴파일을 수행하지 못함. 기존 csproj도 새 파일 반영 전이므로 Unity Editor Console 확인 필요

## 다음 단계

- 남은 문제: Unity Play Mode 최종 동작 확인
- 제안하는 다음 Phase: 필요 시 클릭 이벤트 또는 시스템 커서 숨김·잠금 추가
- 추가 승인이 필요한 사항: 위 추가 기능 구현
