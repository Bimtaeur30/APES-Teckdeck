# PHASE-006 — TMP asmdef 참조 복구 + UI 갱신 연결

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-14 03:45 (UTC+9)`
- 사용자 승인: `아무것도 변경이 안돼었어 그리고 TMP 오류가 계속 나고 있어 이 두가지를 수정해줘`

## 요청과 목표

- 원래 요청: Phase 005 후에도 TMP 오류 지속 + 변경이 보이지 않음
- 이 Phase의 목표:
  1. `AssemblyStageDefinition`에서 `TextMeshProUGUI` 컴파일 가능
  2. Play 시 스테이지 이름/썸네일/버튼 상태가 실제로 갱신
- 완료 조건: Console에 TMP CS0246 없음, Play 시 StageName에 `테스트` 표시

## 승인된 구현 범위

- 변경 파일 (PCM만):
  - `AssemblyStageDefinition.asmdef`
  - `SceneChose.cs`
  - `StageTypeSO.cs`
  - `SO/StageType.asset`
  - `Scene/StageChoose Scene.unity`
  - `_AgentLog/*`
- 제외: 맵 UI 페이드, 로비 씬 이동 구현, 애니메이션

## 구현 결과

- asmdef에 `Unity.TextMeshPro`, `UnityEngine.UI` 참조 추가 (핵심)
- `stageThumbnail`을 `Image` → `Sprite`로 변경, `thumbnail.sprite` 대입
- 잠금 상태를 `goChallenge.interactable = stage.isUnlocked`로 양방향 반영
- `defaultStage` + `Start()`로 Play 시 `ChangeStage` 호출
- 씬 `StageChoose`에 `SceneChose` 부착, `StageName` TMP 추가 및 참조 연결

## 학습 노트

- 커스텀 asmdef는 기본으로 Unity 엔진만 보고, 패키지 조립실(TMP/UI)은 **references에 명시**해야 함
- Phase 005에서 타입만 고쳤어도 asmdef에 TMP가 없어 컴파일이 계속 실패 → “변경 없음”처럼 보임
- SO에는 Image 컴포넌트가 아니라 Sprite 데이터를 두고, UI Image의 `.sprite`만 바꿈
- 스크립트가 씬에 없으면 `Start`/`ChangeStage`가 호출되지 않음

## 검증

- 자동: 소스/asmdef/씬 참조 작성 완료
- Unity 수동:
  1. Console에 `TextMeshProUGUI`/`TMPro` 오류 없는지
  2. `StageChoose` 오브젝트에 `SceneChose`와 슬롯 연결 확인
  3. Play → StageName이 `테스트`로 바뀌는지
- Editor Play: 이 환경에서 미확인

## 다음 단계

- Go Challenge OnClick → `SceneChange` 연결
- 실제 썸네일 Sprite를 SO에 할당
- `goLobby` 버튼 클릭으로 로비 이동
