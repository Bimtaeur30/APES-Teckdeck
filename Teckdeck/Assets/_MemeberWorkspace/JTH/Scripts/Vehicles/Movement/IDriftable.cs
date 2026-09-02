namespace JTH.Vehicles.Movement
{
    public interface IDriftable
    {
        public bool DoDrift { get; set; }
        bool IsDrifting { get; }
        public void ApplySideGrip(bool isTurning);
    }
}