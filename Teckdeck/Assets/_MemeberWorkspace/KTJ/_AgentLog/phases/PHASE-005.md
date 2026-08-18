# PHASE-005 — 월드 캔버스 전체 픽셀 저하 효과

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-14 (Asia/Seoul)`
- 사용자 승인: `승인`

## 요청과 목표

- 원래 요청: 월드 캔버스 자식 UI 전체에 일관된 픽셀 저하 효과 적용
- 이 Phase의 목표: 자식 UI를 한 장의 저해상도 화면으로 합성해 동일한 픽셀 격자로 표시
- 완료 조건: 텍스트, 이미지, 커서가 같은 크기의 픽셀로 보이고 기존 월드 UI 입력 좌표가 유지됨

## 승인된 구현 범위

- 변경 예정 파일: `WorldCanvasPixelDisplay.cs`, KTJ 작업 기록
- 구현 방법: 기존 World Space Canvas를 전용 직교 카메라로 저해상도 RenderTexture에 촬영하고 Point 필터로 RawImage에 표시
- 검증 방법: C# 컴파일 및 Unity Play Mode 수동 확인
- 명시적으로 제외한 항목: 스캔라인, 화면 왜곡, 깜빡임, 노이즈, RGB 색수차, 씬 자동 수정

## 구현 결과

- 수행한 작업: World Space Canvas 전용 캡처 카메라와 저해상도 RenderTexture를 런타임에 생성하고 Point 필터로 RawImage에 출력하는 컴포넌트 구현
- 생성·수정한 파일: `WorldCanvasPixelDisplay.cs`, `PROGRESS.md`, `PHASE-005.md`
- 계획과 달라진 점 및 이유: 없음

## 학습 노트

- 전체 실행 흐름: 컴포넌트 활성화 → 원본 Canvas 계층을 캡처 Layer로 임시 전환 → 저해상도 RenderTexture와 직교 카메라 생성 → Canvas의 월드 사각형에 카메라 정렬 → RawImage에 결과 출력 → 비활성화 시 Layer와 런타임 리소스 복구
- 주요 데이터 역할: `textureWidth/textureHeight`는 최종 픽셀 격자 수, `captureLayer`는 캡처할 UI만 분리, `outputImage`는 합성된 한 장의 화면 표시
- 중요한 구현 원리: 각 UI에 효과를 따로 적용하지 않고 먼저 한 텍스처로 합치며, 텍스처의 `FilterMode.Point`가 확대 시 픽셀 사이 보간을 막음
- Unity 생명주기: `OnEnable`에서 자원을 준비하고 `LateUpdate`에서 움직일 수 있는 Canvas에 카메라를 맞추며 `OnDisable`에서 원래 Layer와 GPU 자원을 복구
- 예외 상황과 대응: 참조 누락, World Space가 아닌 Canvas, 원본 Canvas 내부에 배치된 출력 RawImage를 감지하면 오류를 알리고 컴포넌트를 비활성화
- UniTask/DOTween: 비동기 처리나 시간 기반 연출이 없는 렌더 구성 작업이므로 사용하지 않음

## 검증

- 자동 검증: `dotnet build KTJ.csproj --no-restore`
- Unity Editor 수동 확인 절차: 원본 Canvas 밖에 별도 World Space Canvas와 전체 크기 RawImage를 만들고 컴포넌트의 Source Canvas/Output Image를 연결한다. 출력 Canvas를 원본보다 카메라 쪽으로 조금 앞에 둔 뒤 Play하여 텍스트·이미지·커서가 같은 픽셀 격자를 공유하는지 확인한다.
- 결과: 컴파일 오류 0개. 기존 `VirtualMouse.isEnabled` 미사용 경고 1개는 이번 Phase 범위 밖의 기존 코드임
- 검증하지 못한 항목: 실제 씬 계층 연결 후 Play Mode 시각 결과와 캡처 Layer 충돌 여부

## 다음 단계

- 남은 문제: Unity Inspector 연결 및 Play Mode 시각 확인
- 제안하는 다음 Phase: 필요할 경우 스캔라인·색수차·노이즈 추가
- 추가 승인이 필요한 사항: 추가 CRT 연출 구현 또는 씬 파일 자동 연결
