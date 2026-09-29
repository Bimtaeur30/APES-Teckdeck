using _MemeberWorkspace.KTJ._02_Script.Player.InputSystem;
using ModuleSystem;
using UnityEngine;

/*
구조 설계

플레이어(인풋)
- 무브먼트 모듈(점프, 이동)
- 헬스 모듈(체력관리)
*/
public class Player : ModuleOwner
{
    [SerializeField] private PlayerInputSO playerInputSO;
    protected override void InitializeModules()
    {
        base.InitializeModules();
        
    }
}
