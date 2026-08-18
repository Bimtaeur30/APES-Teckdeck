# 작업 진행 현황

## 사용자

- 이름/이니셜: `KTJ`
- 작업폴더: `Assets/_MemeberWorkspace/KTJ/`
- 마지막 갱신: `2026-08-18 (Asia/Seoul)`

## 현재 요청

- 요청 요약: UI 토글의 포인터 입력 Graphic을 변화 대상과 별도로 지정
- 승인된 범위: 공통 토글의 Pointer Target 필드와 Hover·Click·PointerDown 이벤트 Relay
- 범위 밖 항목: 씬·프리팹·공용 경로 변경, 커스텀 Inspector, 추가 효과 컴포넌트

## Phase 현황

| Phase | 상태 | 목표 | 기록 |
|---|---|---|---|
| 001 | 완료 | UXML의 오래된 타입 참조를 현재 타입으로 갱신 | `phases/PHASE-001.md` |
| 002 | 완료 | 월드 Canvas용 가상 마우스 UI 구현 | `phases/PHASE-002.md` |
| 003 | 완료 | 가상 커서 위치를 오브젝트 월드 X/Z 이동에 연결 | `phases/PHASE-003.md` |
| 004 | 완료 | 대상의 최초 위치를 X/Z 이동 중심으로 사용 | `phases/PHASE-004.md` |
| 005 | 완료 | 월드 Canvas 전체를 동일한 픽셀 격자로 표시 | `phases/PHASE-005.md` |
| 006 | 완료 | 에디터 애셋 기반 픽셀 화면 및 비재생 미리보기 구성 | `phases/PHASE-006.md` |
| 007 | 완료 | 1/3 해상도 출력의 픽셀 선명도 강화 | `phases/PHASE-007.md` |
| 008 | 완료 | 렌더 Canvas와 실제 Canvas의 Layer·Camera 분리 | `phases/PHASE-008.md` |
| 009 | 완료 | 운영체제 커서 잠금 및 상대 이동 입력 적용 | `phases/PHASE-009.md` |
| 010 | 완료 | World Space Canvas 직접 GraphicRaycaster 입력 처리 | `phases/PHASE-010.md` |
| 011 | 완료 | `cursor.rect` 네 모서리 기준 경계 계산 | `phases/PHASE-011.md` |
| 012 | 완료 | RenderTexture 좌우 반 픽셀 오버스캔 | `phases/PHASE-012.md` |
| 013 | 완료 | 조건 선택형 UI 토글 컴포넌트 3종 | `phases/PHASE-013.md` |
| 014 | 완료 | UI 토글 포인터 입력 대상 분리 | `phases/PHASE-014.md` |

## 현재 재개 지점

- 마지막 완료 작업: Phase 014 UI 토글 포인터 입력 대상 분리
- 현재 작업: 없음
- 다음에 할 작업: Unity Play Mode에서 지정 Graphic의 포인터 입력 동작 확인
- 사용자 승인이 필요한 사항: 없음
- 관련 파일: `Assets/_MemeberWorkspace/KTJ/02_Script/UI/Components/`
- 알려진 문제 또는 위험: EventSystem·GraphicRaycaster가 없거나 Pointer Target의 Raycast Target이 꺼져 있으면 이벤트가 전달되지 않음

## 검증 요약

- 수행한 검증: Phase 001 정적 참조 검증 통과
- 수행한 검증: Phase 006 변경이 포함된 `KTJ.csproj` 빌드 및 직렬화 참조 검사
- 수행한 검증: Phase 009 변경이 포함된 `KTJ.csproj` 빌드 및 작업 파일 diff 검사
- 수행한 검증: Phase 012 변경이 포함된 `KTJ.csproj` 빌드, 작업 파일 diff 검사, 534×330 기준 가로 캡처 수식 확인
- 수행한 검증: Phase 013 기존 `KTJ.csproj` 빌드, Unity 참조 기반 신규 스크립트 별도 컴파일, 코드 정적·공백 검사
- 수행한 검증: Phase 014 Unity 참조 기반 전체 UI 컴포넌트 별도 컴파일, 등록·해제 흐름 정적 검사, 공백 검사
- 통과 여부: 기존 프로젝트와 신규 UI 스크립트 별도 컴파일 모두 경고 0개·오류 0개, 신규 작업 파일 공백 오류 없음
- 아직 검증하지 못한 항목: 열린 Unity Editor의 Play Mode에서 지정 Graphic 포인터 동작
