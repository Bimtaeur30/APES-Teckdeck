namespace JTH.Vehicles.Movement
{
    public interface IBrakable
    {
        public bool IsBraking { get; }
        public bool ShouldBrake(float longitudinalInput);
        public void Brake();
        public void EndBrake();
    }
}
