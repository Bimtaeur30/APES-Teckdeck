namespace JTH.Vehicles.Movement
{
    public interface IDriftable
    {
        public bool DoDrift { get; set; }
        public void ApplySideGrip(bool isTurning);
    }
}