using Unity.Behavior;
using UnityEngine;

namespace Enemy.BT
{
    [BlackboardEnum]
    public enum StateCommands
    {
        SPAWNED,
        IDLE,
        CHASE,
        ATTACK,
        GRABED,
        HIT,
        DIE 
    }
}