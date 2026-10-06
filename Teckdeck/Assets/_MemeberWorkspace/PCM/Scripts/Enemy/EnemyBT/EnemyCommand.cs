using Unity.Behavior;
using UnityEngine;

namespace Enemy.BT
{
    [BlackboardEnum]
    public enum StateCommands
    {
        IDLE,
        CHASE,
        ATTACK,
        HIT,
        DIE
    }
}