using System.Collections.Generic;

public static class Player
{
    public static int Score { get; set; }
    public static int Money { get; set; }
    public static List<Airplane> Airplanes { get; private set; }
    public static List<Airport> UnlockedAirports { get; set; }
}