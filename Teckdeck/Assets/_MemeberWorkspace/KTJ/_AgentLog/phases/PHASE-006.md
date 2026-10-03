# PHASE-006 — 에디터 애셋 기반 픽셀 화면 구성

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-14 (Asia/Seoul)`
- 사용자 승인: `승인 (씬 연결 포함)`

## 요청과 목표

- 원래 요청: 카메라와 RenderTexture를 동적으로 만들지 않고 에디터 애셋으로 사용해 비재생 상태에서도 저화질 화면 확인
- 이 Phase의 목표: 프로젝트 RenderTexture와 씬 Camera를 직렬화하고 ExecuteAlways 컴포넌트로 편집 모드 미리보기 제공
- 완료 조건: 씬을 열었을 때 Camera, RenderTexture, RawImage 참조가 저장되어 있고 Play Mode 없이도 캡처 결과가 갱신됨

## 승인된 구현 범위

- 변경 파일: `WorldCanvasPixelDisplay.cs`, `ComputerScreenPixel.renderTexture`, `RepairShop_KTJ.unity`, KTJ 작업 기록
- 구현 방법: 동적 생성과 해제를 제거하고 씬 Camera 및 RenderTexture 애셋을 직접 참조. `ExecuteAlways`에서 카메라 정렬과 참조 설정 수행
- 검증 방법: KTJ 어셈블리 빌드, 씬 YAML 참조 및 RenderTexture GUID 정적 검사, Unity Editor 임포트 로그 확인
- 범위 밖 항목: CRT 스캔라인·노이즈·색수차, 공용 Layer 및 ProjectSettings 변경

## 구현 결과

- 수행한 작업: 267×165 RenderTexture 애셋 생성, 기존 `WorldCanvasPixelDisplay` 오브젝트에 캡처 Camera와 URP Camera Data 저장, RawImage와 RenderTexture 연결, 출력 Canvas를 캡처 Layer에서 분리
- 생성·수정한 파일: `WorldCanvasPixelDisplay.cs`, `ComputerScreenPixel.renderTexture` 및 meta, `RepairShop_KTJ.unity`, `PROGRESS.md`, `PHASE-006.md`
- 계획과 달라진 점 및 이유: KTJ님이 이미 생성한 `RealComputerScreenCanvas`와 `RawImage`를 재사용하여 계층을 중복 생성하지 않음

## 학습 노트

- 전체 실행 흐름: 에디터가 씬 Camera로 원본 World Canvas를 RenderTexture에 렌더링 → RawImage가 같은 애셋을 표시 → `ExecuteAlways`가 캔버스 이동·크기 변경에 맞춰 카메라 위치와 Orthographic Size 갱신
- 주요 데이터 역할: `ComputerScreenPixel.renderTexture`는 267×165 픽셀 버퍼, `captureCamera`는 원본 UI 캡처, `outputImage`는 실제 모니터 표면 출력
- 중요한 구현 원리: Camera와 RenderTexture가 씬·프로젝트에 저장되어 있으므로 Play Mode 진입 없이도 Unity Editor가 렌더링할 수 있음. Point 필터는 확대 과정의 색상 보간을 막음
- Unity 생명주기: `OnEnable`과 `OnValidate`에서 참조 설정, 편집 모드를 포함한 `LateUpdate`에서 카메라 정렬
- 예외 상황과 대응: 참조가 없거나 원본 Canvas가 World Space가 아니면 정렬을 건너뛰며, 출력 RawImage가 원본 Canvas의 자식이면 피드백 방지를 위해 정렬하지 않음
- UniTask/DOTween: 비동기 또는 시간 기반 연출이 없는 편집·렌더 설정이므로 사용하지 않음

## 검증

- 자동 검증: `dotnet build KTJ.csproj --no-restore`, `git diff --check`, 씬의 Camera/RenderTexture/RawImage GUID 참조 검색
- 결과: 컴파일 오류 0개, 새 코드 경고 0개, 직렬화 참조 일치, diff whitespace 오류 없음
- 기존 경고: `VirtualMouse.isEnabled` 미사용 경고 1개는 이번 범위 밖
- 검증하지 못한 항목: 사용자의 현재 Scene/Game View에서 실제 픽셀 화면이 보이는지에 대한 육안 확인

## 다음 단계

- 남은 문제: Unity가 외부 씬 변경 알림을 표시하면 Reload 선택 후 화면 확인
- 제안하는 다음 Phase: 필요할 경우 CRT 스캔라인·노이즈·색수차 추가
- 추가 승인이 필요한 사항: 추가 연출 또는 해상도 기본값 변경
