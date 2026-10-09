# PHASE-002 — 반투명 왼쪽 패널

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-11 14:20 (UTC+9)`
- 사용자 승인: `작업을 시작해줘`

## 요청과 목표

- 원래 요청: 반투명 패널만 만들어 달라
- 이 Phase의 목표: Canvas + 좌측 반투명 Image 패널만 씬에 구성
- 완료 조건: 씬 오픈 시 왼쪽 반투명 패널이 보임

## 승인된 구현 범위

- 변경 파일: `StageChoose Scene.unity`, `_AgentLog`
- 구현 방법: Screen Space Overlay Canvas + LeftPanel Image
- 검증 방법: Unity Game 뷰 확인
- 제외: 맵/텍스트/코스/스크립트/애니메이션

## 구현 결과

- 수행한 작업: Canvas, LeftPanel, EventSystem 추가. 카메라 배경을 초록 Solid Color로 바꿔 반투명이 보이도록 함
- 생성·수정한 파일: `StageChoose Scene.unity`, `_AgentLog/*`
- 계획과 달라진 점: 반투명 확인용으로 Main Camera Clear Flags/배경색만 조정

## 학습 노트

- 구조: `Canvas` → `LeftPanel`(Image)
- LeftPanel: Anchor (0,0)~(0.38,1) → 화면 왼쪽 약 38% 전체 높이
- Color: RGB(0.08, 0.12, 0.08), Alpha 0.72 → 어두운 초록 반투명
- Image + 내장 UISprite + Color.a 로 반투명 사각형 표현
- Awaitable/DOTween: 미사용

## 검증

- 자동 검증: 씬에 Canvas/LeftPanel 존재 확인
- Unity Editor: `StageChoose Scene` 열어 Game 뷰 확인
- 미검증: 실제 Editor 시각 확인(이 환경)

## 다음 단계

- 폭·투명도 조정, 또는 패널 안 텍스트/목록 추가 시 새 Phase 승인 후 진행
