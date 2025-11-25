public class AirplaneSmall : Airplane
{
    public override void Awake()
    {
        base.Awake();

        Range = 5000;
        Speed *= 3; // 500
        Capacity = 100;
        Price = 10000;
    }

    public override void SetTailNumber()
    {
        tailNumber.text = $"SM{Id}";
    }
}