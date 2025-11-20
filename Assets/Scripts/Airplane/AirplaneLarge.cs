public class AirplaneLarge : Airplane
{
    public override void Awake()
    {
        base.Awake();

        Range = 20000;
        Speed *= 4.7;
        Capacity = 300;
    }
}