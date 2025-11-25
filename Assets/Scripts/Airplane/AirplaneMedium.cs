public class AirplaneMedium : Airplane
{
    public override void Awake()
    {
        base.Awake();

        Range = 10000;
        Speed *= 4; // 700
        Capacity = 200;
        Price = 20000;
    }

    public override void SetTailNumber()
    {
        tailNumber.text = $"ME{Id}";
    }
}