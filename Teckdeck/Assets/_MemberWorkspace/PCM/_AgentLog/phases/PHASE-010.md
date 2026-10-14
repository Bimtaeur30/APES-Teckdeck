# PHASE-010 — RefreshPath 선 자동 생성

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-15 16:02 (UTC+9)`
- 사용자 승인: `승인`

## 요청과 목표

- 원래 요청: `RefreshPath`에서 선도 자동 생성 (수동 Instantiate 제거)
- 목표: 포인트 목록만 있으면 필요한 UI Image 선분이 자동으로 생기고 갱신됨
- 완료 조건: 씬에 Line 오브젝트를 미리 두지 않아도 경로가 보임

## 승인된 구현 범위

- 변경 파일:
  - `Scripts/StageChoose/Runtime/StagePathDrawer.cs`
  - `Scene/StageChoose Scene.unity` (수동 Line 제거)
  - `_AgentLog/*`
- 제외: SceneChose 연동, 곡선·해금 연출

## 구현 결과

- `lineSegments` 직렬화 목록 제거 → 런타임/에디터 내부 리스트로 관리
- 포인트 `n`개면 선 `n-1`개를 `EnsureLineCount`로 생성
- 남는 선은 Play 중 `Destroy`, Edit 중 `DestroyImmediate`
- 씬의 `Line_01_02`, `Line_02_03` 수동 배치 제거

## 학습 노트

- 생성: `GameObject + RectTransform + Image`
- 배치: 두 점 중간 위치, `sizeDelta.x = 거리`, `z회전 = Atan2`
- `[ExecuteAlways]`라서 Scene 뷰에서도 포인트만 옮기면 선이 따라옴
- 포인트만 Inspector에 등록하면 되고, 선은 코드가 만듦

## 검증

- 코드상 자동 생성/정리 로직 확인
- Unity Play: 포인트 3개만으로 선 2개가 생기는지 확인 필요
- 미검증: 실제 Editor 렌더링

## 다음 단계

- 포인트에 StageTypeSO 연결
- 필요 시 선 생성 시점을 Start 한 번 + dirty 플래그로 최적화
