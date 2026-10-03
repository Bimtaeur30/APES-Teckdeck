# PHASE-008 — 렌더 Canvas와 실제 Canvas 레이어 분리

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-14 (Asia/Seoul)`
- 사용자 승인: `승인 (Layer 7 지정)`

## 요청과 목표

- 원래 요청: 카메라와 레이어를 분리해 렌더용 Canvas와 실제 출력 Canvas를 별도로 렌더링
- 이 Phase의 목표: 원본 UI는 캡처 카메라에만, RenderTexture 출력은 메인 카메라에만 표시
- 완료 조건: ComputerScreenCanvas 계층은 Layer 7, 캡처 카메라는 Layer 7만 포함, 메인 카메라는 Layer 7 제외, RealComputerScreenCanvas는 UI Layer 5 유지

## 승인된 구현 범위

- 변경 파일: `WorldCanvasPixelDisplay.cs`, `RepairShop_KTJ.unity`, 승인받은 공용 `ProjectSettings/TagManager.asset`, KTJ 작업 기록
- 구현 방법: Layer 7을 `MonitorRender`로 등록하고 원본 Canvas 전체 계층과 두 카메라의 Culling Mask를 씬 설정으로 직렬화. 런타임 코드는 Layer와 Camera Mask를 변경하지 않음
- 검증 방법: C# 컴파일, 씬의 Layer 및 Culling Mask 직렬화 값 검사
- 범위 밖 항목: 다른 카메라 설정 변경

## 구현 결과

- 수행한 작업: Layer 7을 `MonitorRender`로 등록. Layer 7 전용 캡처 카메라를 씬에 저장하고 메인 카메라에서 Layer 7을 제외. 원본 Canvas 전체 계층은 씬에서 Layer 7, 실제 출력 Canvas와 RawImage는 UI Layer 5로 설정. 코드의 Layer 및 Camera Mask 변경 로직 제거
- 생성·수정한 파일: `WorldCanvasPixelDisplay.cs`, `RepairShop_KTJ.unity`, 작업 기록
- 계획과 달라진 점 및 이유: 실행 중인 Unity가 이전 외부 씬 변경을 저장해 캡처 오브젝트가 사라진 상태를 발견하여 현재 씬을 기준으로 캡처 오브젝트를 다시 직렬화함

## 학습 노트

- 전체 실행 흐름: Layer 7 원본 UI → Layer 7 전용 캡처 Camera → 178×110 RenderTexture → UI Layer 5 RawImage → Main Camera
- 주요 데이터 역할: 씬에 저장된 `MonitorRender` Layer는 원본 UI 격리, Main Camera Culling Mask는 원본 UI 제외, Capture Camera Culling Mask는 원본 UI만 캡처
- 중요한 구현 원리: 두 Canvas가 서로 다른 Layer와 Camera Culling Mask를 사용하므로 원본의 고해상도 UI가 실제 화면에 중복 출력되지 않고 RenderTexture 결과만 보임
- Unity 생명주기: 코드는 ExecuteAlways에서 캡처 카메라 정렬만 담당하며 Layer와 Camera Mask는 생명주기 코드에서 변경하지 않음
- 예외 상황과 대응: Layer 7을 다른 오브젝트가 사용하면 캡처에 함께 보일 수 있으므로 이 씬에서는 렌더 UI 전용으로 사용해야 함
- UniTask/DOTween: 렌더 설정을 즉시 유지하는 동기 작업이므로 사용하지 않음

## 검증

- 자동 검증: `dotnet build KTJ.csproj --no-restore`, TagManager Layer 인덱스, 원본 Canvas 전체 자식 Layer, 두 Camera Culling Mask 및 직렬화 참조 검사, `git diff --check`
- 결과: 컴파일 오류 0개, Layer 7 `MonitorRender` 등록, 렌더 Canvas 전체 계층 Layer 7, 실제 Canvas/RawImage Layer 5, 캡처 Mask 128, 메인 Mask 4294967167 확인
- 기존 경고: `VirtualMouse.isEnabled` 미사용 경고 1개는 이번 범위 밖
- 검증하지 못한 항목: Scene/Game View에서 원본 UI가 숨고 RenderTexture 출력만 보이는지 육안 확인

## 다음 단계

- 남은 문제: 실행 중인 Unity에서 외부 씬 변경 Reload 후 시각 확인
- 제안하는 다음 Phase: 없음
- 추가 승인이 필요한 사항: 없음
