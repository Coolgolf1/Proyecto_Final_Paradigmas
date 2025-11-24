public class AirplaneMedium : Airplane
{
    public override void Awake()
    {
        base.Awake();

        Range = 10000;
        Speed = 300; // 700
        Capacity = 200;
    }

    public override void SetTailNumber()
    {
        tailNumber.text = $"ME{Id}";
    }
}