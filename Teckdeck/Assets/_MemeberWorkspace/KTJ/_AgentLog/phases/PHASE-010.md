# PHASE-010 — World Space Canvas 가상 커서 uGUI 입력

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-15 (Asia/Seoul)`
- 사용자 승인: `승인`

## 요청과 목표

- 원래 요청: World Space Canvas에서 가상 마우스로 uGUI 조작
- 이 Phase의 목표: 가상 커서의 World 위치와 실제 마우스 버튼 상태를 Input System 가상 Mouse에 전달해 기존 uGUI 입력 모듈로 처리
- 완료 조건: 가상 커서 위치에서 uGUI 호버·클릭·드래그 입력이 발생할 수 있는 코드 경로가 구성됨

## 승인된 구현 범위

- 변경 파일: `VirtualMouse.cs`, KTJ 작업 기록
- 구현 방법: 캡처 카메라 기준 화면 좌표 변환, 원본 World Canvas의 GraphicRaycaster 직접 실행, 표준 포인터·클릭·드래그 이벤트 전달, 커서 Graphic의 Raycast 차단 방지
- 검증 방법: `KTJ.csproj` 빌드, 정적 코드 검사, Unity Play Mode 수동 확인
- 범위 밖 항목: 씬·프리팹 수정, 새 uGUI 생성, 기존 버튼 이벤트 변경

## 구현 결과

- 수행한 작업: 합성 Mouse가 InputSystemUIInputModule에서 Hover Raycast를 만들지 못하는 플레이 결과에 따라 가상 장치와 UI Action Map 제한을 제거. 원본 World Canvas의 GraphicRaycaster를 직접 실행하고 `PointerEventData`로 Enter·Exit·Move·Down·Up·Click·BeginDrag·Drag·EndDrag·Drop·Scroll 이벤트를 전달하는 보조 모듈 구현. 클릭 이벤트가 즉시 View를 전환해 `Exit`을 호출하는 재진입 상황에서도 포인터 정리를 현재 이벤트 처리 뒤로 미루도록 보호
- 생성·수정한 파일: `VirtualMouse.cs`, `PROGRESS.md`, `PHASE-010.md`
- 계획과 달라진 점 및 이유: 없음

## 검증

- 자동 검증: `dotnet build KTJ.csproj --no-restore`, 작업 파일 diff 검사, Unity uGUI `BaseInputModule`·`PointerInputModule`의 표준 Hover·Press·Release·Drag 흐름 비교, 씬의 EventSystem·GraphicRaycaster·World Space Canvas Event Camera 정적 확인
- 결과: 빌드 경고 0개·오류 0개, 작업 파일 공백 오류 없음, 합성 Mouse 및 bitfield 변경 코드 제거, 필수 씬 구성 존재
- 검증하지 못한 항목: Unity Play Mode에서 실제 Button 하이라이트·클릭, Slider 드래그 및 클릭 직후 View 전환

## 학습 노트

- 전체 실행 흐름: View 진입 → 실제 Mouse와 원본 Canvas의 GraphicRaycaster 확보 → 매 프레임 가상 커서 이동 → 커서의 World 중심을 캡처 카메라 화면 좌표로 변환 → 원본 Canvas 직접 Raycast → 이전/현재 대상 비교로 Hover 이벤트 전달 → 실제 좌클릭 상태 변화로 Press·Release·Click 또는 Drag 이벤트 전달 → View 종료 시 포인터 Exit 및 진행 중 Press·Drag 정리
- 주요 클래스/메서드 역할: `GraphicRaycaster.Raycast`는 원본 Canvas Graphic 중 커서 위치의 최상위 대상을 찾고, `PointerEventData`는 위치·이동량·누른 대상·드래그 상태를 보관하며, `ExecuteEvents`는 uGUI 인터페이스에 표준 이벤트를 전달. `VirtualMousePointerModule`은 `BaseInputModule`의 Hover 계층 처리 기능을 재사용하지만 EventSystem의 자동 활성 입력 모듈 경쟁에는 참여하지 않음
- 중요한 구현 원리: RenderTexture의 RawImage를 클릭하는 것이 아니라 원본 `ComputerScreenCanvas`의 Graphic을 캡처 카메라 좌표로 Raycast해야 실제 Button과 Slider 정보를 찾을 수 있음
- Unity 생명주기: `Enter`에서 가상 입력을 준비하고 `HandleInput`에서 매 프레임 상태를 전달하며 `Exit` 또는 `OnDisable`에서 장치·Action Map·Raycast 설정을 복구
- 예외 상황과 대응: EventSystem·GraphicRaycaster·실제 Mouse·Event Camera가 없으면 경고 또는 해당 프레임 생략. 커서 자식 Graphic은 진입 중 Raycast를 막지 않도록 비활성화하고 원래 값을 기억해 복구. Button Click이 View 전환을 즉시 발생시켜 `Exit`이 이벤트 처리 도중 호출되면 포인터 정리를 처리 종료 시점까지 지연해 중복 해제와 NullReference를 방지
- UniTask/DOTween: 프레임별 동기 입력 전달이며 시간 기반 연출이 아니므로 사용하지 않음

## 다음 단계

- 남은 문제: Unity Play Mode에서 실제 uGUI 상호작용 확인
- 제안하는 다음 Phase: 없음
- 추가 승인이 필요한 사항: 없음
