using UnityEngine;

public struct GrappleEnterContainer
{
    public Vector3 EnemyPosition { get; }
    
    public GrappleEnterContainer(Vector3 enemyPosition)
    {
        EnemyPosition = enemyPosition;
    }
}
