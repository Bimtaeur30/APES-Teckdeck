# PHASE-007 — List + Clamp index로 스테이지 전환

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-14 15:33 (UTC+9)`
- 사용자 승인: `승인`

## 요청과 목표

- 원래 요청: Next/Prev 시 List로 SO를 들고 Clamp 형식 index 증감
- 이 Phase의 목표: `SceneChose`가 스테이지 목록을 관리하고 양 끝에서 index가 막히게 함
- 완료 조건: `Next`/`Prev`가 Clamp되고, 현재 SO로 UI·씬 로드 가능

## 승인된 구현 범위

- 변경 파일:
  - `Scripts/StageChoose/Runtime/SceneChose.cs`
  - `Scene/StageChoose Scene.unity` (직렬화 필드 갱신)
  - `_AgentLog/*`
- 제외: 버튼 OnClick Inspector 연결(수동), 추가 SO 에셋 생성

## 구현 결과

- `List<StageTypeSO> stages` + `currentIndex`
- `Next()` / `Prev()`에서 `Mathf.Clamp`로 index 증감 후 UI 갱신
- `SceneChange()`는 현재 index SO 사용
- 양 끝에서 `nextButton`/`prevButton` interactable 갱신
- 씬에 stages(기존 StageType)·next/prev 버튼 참조 반영

## 학습 노트

- Clamp: `Mathf.Clamp(value, 0, count - 1)` → 범위를 벗어나도 끝 값으로 고정 (순환 없음)
- wrap(`%`)과 달리 첫/끝에서 더 이상 넘어가지 않음
- 버튼 OnClick에는 `SceneChose.Next` / `Prev` / `SceneChange`를 연결하면 됨
- 스테이지를 늘리려면 Inspector `Stages` 리스트에 SO만 추가

## 검증

- Unity에서 `Stages`에 SO 2개 이상 넣고
  - Prev at 0 → index 유지, Prev 비활성
  - Next at 마지막 → index 유지, Next 비활성
  - 중간에서 이름/썸네일 변경 확인
- Editor Play: 이 환경에서 미확인

## 다음 단계

- Button / Button (1) OnClick에 Prev / Next 연결
- Go Challenge OnClick에 SceneChange 연결
- 스테이지 SO 추가
