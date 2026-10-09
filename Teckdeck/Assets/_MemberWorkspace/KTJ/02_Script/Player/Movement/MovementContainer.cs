public struct MovementContainer
{
    public MovementVector Vector { get; }
    public float JumpCharge { get; }

    public MovementContainer(MovementVector vector, float jumpCharge)
    {
        Vector = vector;
        JumpCharge = jumpCharge;
    }
}
