public class AirplaneSmall : Airplane
{
    public override void Awake()
    {
        base.Awake();

        Range = 5000;
        Speed = 100; // 500
        Capacity = 100;
    }
}