using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int Score { get; private set; }
    public int Money { get; private set; }
    public List<Airplane> Airplanes { get; private set; }
}
