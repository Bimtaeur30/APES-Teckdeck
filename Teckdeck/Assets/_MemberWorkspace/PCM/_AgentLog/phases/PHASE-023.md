# PHASE-023 — 풀 스폰 초기화

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-14 20:07 (UTC+9)`
- 사용자 승인: 감지 콜라이더 하나 기준으로 이 방향대로 수정

## 구현 결과

- `AbstractEnemy`가 루트의 `Collider` 하나를 `_bodyCollider`로 보관한다.
- 사망 시 그 콜라이더만 끄고, `PrepareForSpawn`에서 다시 켠다.
- `PrepareForSpawn`은 사망 플래그, 체력, 이동, 타겟 블랙보드를 되돌리고 BT를 `Restart`한다.
- `EnemyPoolable.ResetItem`이 `PrepareForSpawn`을 호출한다.

## 검증

- 플레이 검증은 하지 않았다. 풀에서 두 번째 소환 때 체력과 타겟이 리셋되는지는 씬에서 확인이 필요하다.
