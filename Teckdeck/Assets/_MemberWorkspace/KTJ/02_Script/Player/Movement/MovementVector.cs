using UnityEngine;

public struct MovementVector
{
    public Vector3 Direction { get; private set; }
    public float Distance { get; private set; }

    public MovementVector(Vector3 direction, float distance)
    {
        Direction = direction;
        Distance = distance;
    }
}