# PHASE-003 — 오른쪽 끝 페이드 LeftPanel (Lonely Mountains 스타일)

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-11 14:45 (UTC+9)`
- 사용자 승인: `오른쪽 끝에만 연한 느낌 + Lonely Mountain UI 참고해서 만들자`

## 요청과 목표

- 원래 요청: MapUiSG로 옆으로 갈수록 연해지는 UI
- 이 Phase의 목표: 왼쪽은 진하고, 오른쪽 끝만 부드럽게 투명해지는 패널
- 완료 조건: LeftPanel에 페이드 머티리얼 적용되어 Game 뷰에서 확인 가능

## 승인된 구현 범위

- 변경 파일 (PCM만):
  - `MapUiSG.shader` (빈 Shader Graph → Canvas UI용 셰이더로 구현)
  - `MapUiMat.mat`
  - `StageChoose Scene.unity` (`LeftPanel` 재추가)
- 제외: 텍스트/챌린지/코스/애니메이션

## 구현 결과

- `MapUiSG`: UV.x 기준 `1 - smoothstep(FadeStart, FadeEnd, uv.x)` 로 우측 끝만 알파 감소
- `MapUiMat`: Tint 어두운 초록, FadeStart=0.78, FadeEnd=1.0
- `LeftPanel`: 화면 왼쪽 ~42% 폭, Material = MapUiMat

## 학습 노트

- Lonely Mountains 왼쪽 패널은 전체가 반투명이 아니라, **맵과 만나는 오른쪽 가장자리만** 흐려짐
- UI Image는 Stencil/클립이 필요해서, 빈 Unlit Shader Graph보다 **UI 전용 셰이더**가 Canvas와 잘 맞음
- `FadeStart`를 키우면(예: 0.85) 페이드 구간이 더 짧아져 “끝만” 연해짐
- Image.Color는 흰색 유지, 색은 Material `_Color`에서 조절

## 검증

- 자동: 파일/씬 참조 작성 완료
- Editor Game 뷰: 이 환경에서 미확인 → Unity에서 `StageChoose Scene` 열어 확인

## 다음 단계

- FadeStart/색 미세 조정
- 요청 시 패널 안 텍스트·챌린지 UI 추가 (새 Phase)
