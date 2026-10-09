# PHASE-012 — RenderTexture 좌우 반 픽셀 오버스캔

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-15 (Asia/Seoul)`
- 사용자 승인: `승인`

## 요청과 목표

- 원래 요청: RenderComputerScreenCanvas와 RenderTexture 좌우 끝에 생기는 얇은 색상선 원인 조사 및 제거
- 이 Phase의 목표: 캡처 카메라가 원본 Canvas 좌우 경계를 정확한 렌더 타깃 끝이 아닌 각각 반 픽셀 안쪽에서 촬영하도록 조정
- 완료 조건: 카메라 세로 범위는 유지하고 가로 캡처 폭만 RenderTexture 한 픽셀만큼 축소

## 승인된 구현 범위

- 변경 파일: `WorldCanvasPixelDisplay.cs`, KTJ 작업 기록
- 구현 방법: Canvas 월드 너비를 RenderTexture 너비로 나눈 한 픽셀 크기를 구해 카메라 가로 캡처 폭에서 제외하고 Aspect 계산
- 검증 방법: `KTJ.csproj` 빌드, 작업 파일 diff 검사, 534×330 기준 오버스캔 수식 확인
- 범위 밖 항목: 씬·RenderTexture·RawImage 설정 변경, 세로 오버스캔, 전용 Shader 추가

## 구현 결과

- 수행한 작업: Canvas 월드 너비를 RenderTexture 너비로 나눠 가로 한 픽셀의 월드 크기를 계산하고, 캡처 폭에서 그 한 픽셀을 제외. Orthographic Size는 Canvas 높이 절반으로 유지하고 Aspect를 축소된 캡처 폭/높이로 설정해 좌우 각각 반 픽셀 오버스캔 적용
- 생성·수정한 파일: `WorldCanvasPixelDisplay.cs`, `PROGRESS.md`, `PHASE-012.md`
- 계획과 달라진 점 및 이유: 없음

## 검증

- 자동 검증: `dotnet build KTJ.csproj --no-restore`, 작업 파일 diff 검사, 534×330 기준 수식 검토
- 결과: 빌드 경고 0개·오류 0개, 작업 파일 공백 오류 없음. Canvas 로컬 너비 534 기준 캡처 폭 533으로 좌우 각각 0.5픽셀 안쪽 촬영
- 검증하지 못한 항목: Unity RenderTexture 미리보기에서 좌우 선 소멸 확인

## 학습 노트

- 전체 실행 흐름: Canvas 네 모서리에서 월드 너비·높이 계산 → 가로 한 픽셀의 월드 크기 계산 → 캡처 가로 폭에서 한 픽셀 제외 → 높이는 그대로 두고 Camera Aspect만 갱신
- 주요 데이터 역할: `worldUnitsPerPixelX`는 RenderTexture 가로 한 픽셀이 차지하는 Canvas 월드 폭이고, `captureWidth`는 좌우 합계 한 픽셀을 잘라낸 실제 카메라 가로 범위
- 중요한 구현 원리: 렌더 타깃 경계와 UI 사각형 경계를 정확히 겹치지 않고 Canvas가 좌우로 각각 반 픽셀씩 카메라 밖까지 이어지게 하여 첫·마지막 픽셀 열의 불안정한 Coverage를 방지
- Unity 생명주기: `OnEnable`, `OnValidate`, `LateUpdate`에서 `AlignCaptureCamera`가 호출될 때 동일한 오버스캔 정렬을 유지
- 예외 상황과 대응: 원본 좌우 내용이 각각 반 픽셀씩 잘리지만 534픽셀 화면에서는 육안 영향이 매우 작음. 세로 범위와 RawImage UV는 변경하지 않음
- UniTask/DOTween: 즉시 카메라 투영 계산이며 비동기 작업이나 시간 기반 연출이 아니므로 사용하지 않음

## 다음 단계

- 남은 문제: Unity 미리보기 및 Game View 수동 확인
- 제안하는 다음 Phase: 없음
- 추가 승인이 필요한 사항: 없음
