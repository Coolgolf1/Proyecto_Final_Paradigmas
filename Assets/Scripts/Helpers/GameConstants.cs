using UnityEngine;

public static class GameConstants
{
    // Game parameters
    public const int minTravellersCreatedInAirport = 100;

    public const int maxTravellersCreatedInAirport = 1000;

    public const double relativeSpeed = 0.0463;

    public static Vector3 initCameraPosition = new Vector3(52.46f, 1.1f, 8.75f);
    public static Quaternion initCameraRotation = new Quaternion(0f, 0.766044259f, 0f, -0.642787814f);
}