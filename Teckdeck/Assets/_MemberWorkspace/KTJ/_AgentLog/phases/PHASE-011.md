# PHASE-011 — cursor.rect 네 모서리 기준 경계 계산

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-08-15 (Asia/Seoul)`
- 사용자 승인: `승인`

## 요청과 목표

- 원래 요청: 자식 오브젝트 크기가 아니라 `cursor.rect` 네 모서리를 기준으로 가상 커서 경계 계산
- 이 Phase의 목표: Cursor 자신의 RectTransform 영역만 `moveArea`를 넘지 않도록 Clamp하고 동일한 영역으로 위치를 정규화
- 완료 조건: 자식 계층 Bounds 계산이 제거되고 변환된 `cursor.rect` 네 모서리의 최소·최대값이 사용됨

## 승인된 구현 범위

- 변경 파일: `VirtualMouse.cs`, KTJ 작업 기록
- 구현 방법: `GetWorldCorners`로 Cursor 자신의 네 모서리를 얻고 `moveArea.InverseTransformPoint`로 기준 좌표계에 변환해 Bounds 구성
- 검증 방법: `KTJ.csproj` 빌드, 작업 파일 diff 검사, 네 모서리 경계 계산 정적 확인
- 범위 밖 항목: 자식 이미지 크기 변경, 씬·프리팹 수정, uGUI 포인터 입력 변경

## 구현 결과

- 수행한 작업: `CalculateRelativeRectTransformBounds`를 제거하고 Cursor 자신의 `GetWorldCorners` 결과만 `moveArea` 로컬 좌표로 변환. 네 점의 최소·최대값으로 Bounds를 구성해 Clamp와 월드 대상 위치 정규화에서 공통 사용. 네 모서리 배열은 필드에서 재사용해 프레임별 할당 방지
- 생성·수정한 파일: `VirtualMouse.cs`, `PROGRESS.md`, `PHASE-011.md`
- 계획과 달라진 점 및 이유: 없음

## 검증

- 자동 검증: `dotnet build KTJ.csproj --no-restore`, 작업 파일 diff 검사, `CalculateRelativeRectTransformBounds` 제거 및 새 네 모서리 변환 호출 검색
- 결과: 빌드 경고 0개·오류 0개, 작업 파일 공백 오류 없음, Clamp와 위치 정규화가 모두 새 Bounds 메서드를 사용
- 검증하지 못한 항목: Unity Play Mode에서 네 경계 도달 및 자식 이미지 돌출 확인

## 학습 노트

- 전체 실행 흐름: `cursor.GetWorldCorners`로 Cursor 자신의 네 모서리 취득 → `moveArea.InverseTransformPoint`로 이동 영역 로컬 좌표 변환 → 네 점의 최소·최대값으로 Bounds 생성 → Clamp 및 위치 정규화에 사용
- 주요 메서드 역할: `GetWorldCorners`는 RectTransform 자신의 사각형 네 점을 월드 좌표로 제공하고, `InverseTransformPoint`는 월드 좌표를 `moveArea` 기준 좌표로 변환하며, `Bounds.SetMinMax`는 네 점을 포함하는 축 정렬 경계를 구성
- 중요한 구현 원리: 자식 계층 전체를 계산하는 API를 사용하지 않으므로 Cursor 자식 이미지의 크기는 이동 제한에 영향을 주지 않음. 회전이나 스케일이 있어도 실제 변환된 네 모서리를 사용
- Unity 생명주기: `HandleInput` 중 커서 이동을 Clamp할 때 실행되고, 이어지는 대상 오브젝트 위치 갱신에서도 같은 Bounds를 다시 사용
- 예외 상황과 대응: Cursor보다 자식 이미지가 크면 자식은 이동 영역 밖으로 보일 수 있음. 네 모서리 배열은 매 프레임 새로 만들지 않고 재사용
- UniTask/DOTween: 즉시 좌표 계산이며 비동기 작업이나 시간 기반 연출이 아니므로 사용하지 않음

## 다음 단계

- 남은 문제: Unity Play Mode 수동 경계 확인
- 제안하는 다음 Phase: 없음
- 추가 승인이 필요한 사항: 없음
