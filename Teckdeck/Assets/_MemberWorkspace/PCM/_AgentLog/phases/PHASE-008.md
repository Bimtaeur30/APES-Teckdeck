# PHASE-008 — 버튼 클릭 불가 수정

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-14 15:56 (UTC+9)`
- 사용자 승인: `승인`

## 요청과 목표

- 원래 요청: 버튼이 안 눌림 확인·수정
- 목표: UI 클릭이 EventSystem → Button까지 전달되게 복구
- 완료 조건: Play에서 Go Lobby / Go Challenge 클릭 가능 (Next/Prev는 SO 2개 이상일 때)

## 구현 결과

- Canvas에 `GraphicRaycaster` 추가
- EventSystem: `StandaloneInputModule` → `InputSystemUIInputModule` (+ DefaultInputActions)
- 빈 `SceneChose` 오브젝트 제거 (null AddListener NRE 원인)
- `SceneChose.cs`: null 체크, Lobby/SceneChange 분리, Editor using 제거
- 씬 직렬화: `stageName`, `goLobby`(Button), thumbnail→Thumnail Image

## 학습 노트

- uGUI 클릭 = GraphicRaycaster + 올바른 InputModule + EventSystem
- Input System Only 프로젝트에서는 StandaloneInputModule이 클릭을 못 받음
- Awake에서 null 참조에 AddListener하면 예외로 이후 등록도 실패할 수 있음
- stages가 1개면 Clamp 설계상 Next/Prev는 비활성(정상)

## 검증

- Unity Play: Go Lobby / Challenge 클릭
- Next/Prev: Stages에 SO 2개 이상 추가 후 확인
- Editor Play: 이 환경 미확인

## 다음 단계

- Stages 리스트에 SO 추가
- InputSystemUIInputModule Actions가 Missing이면 Inspector에서 Assign Default Actions
