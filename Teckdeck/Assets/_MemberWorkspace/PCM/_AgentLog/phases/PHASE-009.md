# PHASE-009 — UI 경로 포인트 예시

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-15 15:50 (UTC+9)`
- 사용자 승인: `승인`

## 요청과 목표

- 원래 요청: 스테이지 선택 씬에서 포인트를 배치하고 선으로 잇는 예시 제작
- 목표: 포인트 3개를 움직이면 경로 선이 자동으로 따라오는 최소 예시 제공
- 완료 조건: `MapPath_Example` 아래의 `Point_01~03`이 순서대로 연결됨

## 승인된 구현 범위

- 변경 파일:
  - `Scripts/StageChoose/Runtime/StagePathPoint.cs`
  - `Scripts/StageChoose/Runtime/StagePathDrawer.cs`
  - `Scene/StageChoose Scene.unity`
  - `_AgentLog/*`
- 구현 방법: 포인트 사이에 얇은 UI Image를 배치하고 길이·회전을 자동 계산
- 검증 방법: Scene/Game 뷰에서 포인트 이동 및 경로 갱신 확인
- 제외: `SceneChose`와의 연동, 실제 스테이지 SO 연결, 곡선·점선·해금 연출

## 구현 결과

- `StagePathPoint`: 경로 위치를 나타내는 마커 컴포넌트
- `StagePathDrawer`: 등록된 포인트와 UI Image 선분을 순서대로 읽어 위치·길이·회전 갱신
- `MapPath_Example`: 전체 화면 기준 예시 컨테이너
- `Point_01~03`: 지그재그 위치의 주황색 예시 포인트

## 학습 노트

- 각 선분은 얇은 UI Image이며, 두 점의 중간 위치에 놓임
- 두 점 사이 거리를 Image 폭으로, `Atan2`로 구한 각도를 Image 회전으로 적용함
- 포인트 순서가 곧 경로 순서이므로 Inspector의 `Points` 목록 순서가 중요함
- `[ExecuteAlways]`와 `LateUpdate()`로 Scene 뷰에서 포인트를 움직여도 선이 갱신됨
- 비동기 처리와 DOTween은 필요하지 않아 사용하지 않음

## 검증

- 자동 검증: 스크립트 참조 GUID와 씬의 포인트 목록 연결 확인
- Unity Editor 수동 확인:
  1. `StageChoose Scene` 열기
  2. Hierarchy에서 `Canvas/MapPath_Example` 펼치기
  3. `Point_01~03` 중 하나를 이동해 선이 따라오는지 확인
  4. `MapPath_Example`의 Color와 Line Thickness 변경 확인
- 미검증: 현재 환경에서 Unity 실제 렌더링·컴파일 확인

## 다음 단계

- 실제 맵 포인트 개수와 위치로 예시 교체
- 포인트에 `StageTypeSO`를 연결하고 선택/해금 상태 표현
- 필요하면 DOTween으로 경로가 순서대로 나타나는 연출 추가
