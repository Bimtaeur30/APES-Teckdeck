# PHASE-021 — 근접 적 BT를 추적-공격 루프로 전환

## 기본 정보

- 상태: `완료`
- 작성/갱신 일시: `2026-10-07 23:40 (UTC+9)`
- 사용자 승인: `다운로드한 근접 BT를 통째로 복사하지 말고, 현재 EnemyBT를 그 흐름에 맞게 바꿔 달라는 요청`

## 구현 결과

- `UseSkillAction`을 추가했다. 스킬 인덱스 0을 대상에게 쓰고, `OnCurrentSkillEnd`까지 Running을 유지한다.
- On Start의 Repeat를 껐다. 맨 앞의 IDLE Trigger는 실행 경로에서 뺐다. 찾기 성공 때의 CHASE Trigger만 남긴다.
- ATTACK은 Stop Enemy, Use Skill(0), Trigger CHASE 순서다.
- CHASE 끝의 Trigger는 ATTACK(2) 그대로 두었다. Rotate To Target은 바꾸지 않았다.

## 학습 노트

- 플레이에 쓰이는 그래프와 에디터에 보이는 그래프를 같이 고쳐야 한다. 에디터에서 저장하면 보이는 그래프가 플레이 그래프를 다시 만든다.
- On Start가 끝날 때마다 다시 시작하면, 타겟이 이미 있는 찾기가 실패하고 그 상태가 반복된다.

## 검증

- 런타임 그래프에서 On Start 자식이 Repeat Until Success이고, ATTACK 자식이 Stop, Use Skill, Trigger CHASE인 것을 확인했다.
- Unity에서 그래프를 열거나 플레이하지는 못했다.
