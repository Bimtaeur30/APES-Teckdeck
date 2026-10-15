public struct MovementEnterContainer
{
    public MovementVector Vector { get; }
    public float JumpCharge { get; }

    public MovementEnterContainer(MovementVector vector, float jumpCharge)
    {
        Vector = vector;
        JumpCharge = jumpCharge;
    }
}
