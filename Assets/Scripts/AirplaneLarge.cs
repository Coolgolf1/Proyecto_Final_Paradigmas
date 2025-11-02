using Assets.Scripts;
using UnityEngine;

public class AirplaneLarge : Airplane
{
    public override void Awake()
    {
        base.Awake();
        Speed = 40;
    }
}
