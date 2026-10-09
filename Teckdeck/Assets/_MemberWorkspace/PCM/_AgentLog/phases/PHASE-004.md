# PHASE-004 — MapUiSG Shader Graph 복구

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-11 14:55 (UTC+9)`
- 사용자 승인: `쉐이더 그래프가 고장났어 구쳐줘`

## 요청과 목표

- 원인: Phase 003에서 빈 Shader Graph를 `.shader`로 대체해 Graph 에셋이 사라짐
- 목표: `MapUiSG.shadergraph`를 다시 열고 쓸 수 있게 복구 + 오른쪽 끝 페이드 유지

## 구현 결과

- `MapUiSG.shader` 제거
- `MapUiSG.shadergraph` 복구 (UGUI Canvas Basic 기반 + RightEdgeFade)
- GUID 유지 → `MapUiMat` 연결 유지
- LeftPanel Image Color를 어두운 초록으로 설정
- Canvas TexCoord0 채널 활성화

## 학습 노트

- Canvas UI용 Shader Graph는 **Canvas Target**이 필요함
- 페이드: `Fade = 1 - smoothstep(FadeStart, FadeEnd, UV.x)` → Alpha에 Multiply
- 패널 색은 Image.Color, 페이드 폭은 Material의 Fade Start/End

## 검증

- Unity에서 `MapUiSG` 더블클릭 → Graph 창 열리는지 확인
- `StageChoose Scene` Game 뷰에서 우측 끝 페이드 확인
- 핑크( Missing Shader )면 잠시 Import 대기 후 MapUiMat Shader 재지정

## 다음 단계

- Graph에서 노드 위치/페이드 값 미세 조정
