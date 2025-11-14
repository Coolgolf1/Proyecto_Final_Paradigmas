public class AirplaneLarge : Airplane
{
    public override void Awake()
    {
        base.Awake();

        Range = 20000;
        Speed = 500; // 900
        Capacity = 300;
    }
}