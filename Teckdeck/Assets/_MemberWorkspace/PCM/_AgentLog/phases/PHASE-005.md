# PHASE-005 — SceneChose TMP 타입 복구

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-14 03:38 (UTC+9)`
- 사용자 승인: `승인`

## 요청과 목표

- 원래 요청: TMP가 안 되는 문제 해결 + 코드 피드백
- 이 Phase의 목표: `sName` 필드 타입 누락으로 인한 컴파일 실패를 복구해 TMP 참조가 동작하게 함
- 완료 조건: `SceneChose.cs`가 컴파일되고 Inspector에 `sName`(TextMeshProUGUI) 슬롯이 노출됨

## 승인된 구현 범위

- 변경 예정 파일: `Scripts/StageChoose/Runtime/SceneChose.cs`
- 구현 방법: `private  sName` → `private TextMeshProUGUI sName`
- 검증 방법: 파일 내용 확인, Unity Console 컴파일 오류 해소 확인(수동)
- 명시적으로 제외한 항목: SO/씬 구조 변경, 필드 리네임, 잠금/썸네일 로직 개선

## 구현 결과

- 수행한 작업: `sName`에 `TextMeshProUGUI` 타입 추가
- 생성·수정한 파일:
  - `Scripts/StageChoose/Runtime/SceneChose.cs`
  - `_AgentLog/PROGRESS.md`
  - `_AgentLog/phases/PHASE-005.md`
- 계획과 달라진 점 및 이유: 없음

## 학습 노트

- 전체 실행 흐름: `ChangeStage(StageTypeSO)` 호출 시 `sName.text`에 스테이지 이름을 넣음. 타입이 있어야 TMP API(`.text`)를 쓸 수 있음
- 주요 클래스/메서드의 역할: `SceneChose`는 UI 갱신·씬 이동 진입점, `sName`은 스테이지 제목 TMP
- UniTask 사용 위치와 이유: 해당 없음
- DOTween 사용 위치와 이유: 해당 없음
- 중요한 구현 원리: C#에서 `[SerializeField] private` 다음에 타입이 없으면 문법 오류 → 스크립트 전체 컴파일 실패 → Inspector/TMP 연결 불가
- 예외 상황과 대응: 수정 후에도 `CS0246 TextMeshProUGUI`가 뜨면 asmdef에 `Unity.TextMeshPro` 참조 추가 필요(별도 승인)

## 검증

- 자동 검증: 소스상 타입 복구 확인
- Unity Editor 수동 확인 절차:
  1. Console에 `SceneChose` 관련 오류가 없는지 확인
  2. `SceneChose` 컴포넌트 Inspector에서 `S Name`에 TMP 드래그
  3. Play 후 `ChangeStage` 호출 시 텍스트가 바뀌는지 확인
- 결과: 코드 수정 완료 / Editor 실행은 이 환경에서 미확인
- 검증하지 못한 항목: Unity Import·Play Mode 시각 확인

## 다음 단계

- 남은 문제: 썸네일 `Image` 대입 방식, 잠금 버튼 재활성, `goLobby` 미사용 등(피드백 항목)
- 제안하는 다음 Phase: `StageTypeSO` 썸네일을 `Sprite`로 바꾸고 `thumbnail.sprite` 대입 + 잠금 상태 양방향 반영
- 추가 승인이 필요한 사항: 위 개선 구현, asmdef 참조 추가(필요 시)
