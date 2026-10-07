# 작업 진행 현황

## 사용자

- 이름/이니셜: `PCM`
- 작업폴더: `Assets/_MemeberWorkspace/PCM/`
- 마지막 갱신: `2026-10-07 23:40 (UTC+9)`

## 현재 요청

- 요청 요약: 현재 EnemyBT를 하데스식 추적-공격 루프로 전환
- 승인된 범위: PCM의 EnemyBT와 UseSkillAction
- 범위 밖 항목: Rotate 수정, 다운로드 BT 통째 복사, MOVE, 래그돌, AnimationChannel

## Phase 현황

| Phase | 상태 | 목표 | 기록 |
|---|---|---|---|
| 011 | 완료 | 스폰/타이머/클리어 SO | `phases/PHASE-011.md` |
| 012 | 완료 | TestStage Canvas 타이머 | `phases/PHASE-012.md` |
| 013 | 완료 | Inspector 참조 연결 | `phases/PHASE-013.md` |
| 014 | 완료 | AbstractEnemy가 Agent 상속 | `phases/PHASE-014.md` |
| 015 | 완료 | EnemyBT가 AbstractEnemy 타입을 찾게 수정 | `phases/PHASE-015.md` |
| 016 | 완료 | EnemyBT Null 타입을 현재 클래스에 연결 | `phases/PHASE-016.md` |
| 017 | 완료 | Enemy 칸을 AbstractEnemy 변수에 연결 | `phases/PHASE-017.md` |
| 018 | 완료 | AgentSystem이 Combat을 참조 | `phases/PHASE-018.md` |
| 019 | 완료 | PCM_assembly가 필요한 어셈블리를 참조 | `phases/PHASE-019.md` |
| 020 | 완료 | EnemyBT 블랙보드 null 변수 복구 | `phases/PHASE-020.md` |
| 021 | 완료 | 근접 적 BT를 추적-공격 루프로 전환 | `phases/PHASE-021.md` |
| 022 | 완료 | EnemyBT를 Editor API로 적용 | `phases/PHASE-022.md` |

## 현재 재개 지점

- 마지막 완료 작업: Editor 컴파일 오류 수정 후 EnemyBT melee loop 자동 적용 완료
- 다음에 할 작업: EnemyBT 창을 다시 열어 ATTACK에 Use Skill이 보이는지, 플레이에서 CHASE↔ATTACK이 도는지 확인
- 관련 파일: `Editor/EnemyBTMeleeLoopBuilder.cs`, `GameModule/Enemy/BT/EnemyBT.asset`, `UseSkillAction.cs`

## 검증 요약

- 수행한 검증: 그래프 YAML에서 On Start Repeat, ATTACK 순서, 스킬 노드 타입을 확인
- 통과 여부: 부분 (Unity 플레이는 확인하지 못함)
- 아직 검증하지 못한 항목: 컴파일 후 그래프에 Use Skill이 빈 노드로 보이지 않는지, 스킬 종료 후 CHASE가 다시 도는지
