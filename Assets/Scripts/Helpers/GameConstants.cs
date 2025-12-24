using UnityEngine;

public static class GameConstants
{
    // Game parameters
    public const int minTravellersCreatedInAirport = 100;
    public const int maxTravellersCreatedInAirport = 1000;

    public const int minTravellersRandom = 0;
    public const int maxTravellersRandom = 10;

    public const int maxTravellersInAirport = 14000;

    public static Vector3 mainMenuCameraPosition = new Vector3(27.6100006f, 36.0400009f, -28.3400002f);
    public static Quaternion mainMenuCameraRotation = new Quaternion(-0.331260532f, 0.501139224f, -0.113941371f, -0.791292191f);
    public static Vector3 initCameraPosition = new Vector3(52.46f, 1.1f, 8.75f);
    public static Quaternion initCameraRotation = new Quaternion(0f, 0.766044259f, 0f, -0.642787814f);

    // Airplane stats
    public const double relativeSpeed = 0.0463;
    public const double speedMultiplier = 1000;

    // Small Airplane
    public const double smallRange = 5000;
    public const double smallSpeedMultiplier = 3;
    public const int smallCapacity = 100;
    public const int smallPrice = 10000;

    // Medium Airplane
    public const double mediumRange = 10000;
    public const double mediumSpeedMultiplier = 4;
    public const int mediumCapacity = 200;
    public const int mediumPrice = 22500;

    // Large Airplane
    public const double largeRange = 20000;
    public const double largeSpeedMultiplier = 4.7;
    public const int largeCapacity = 300;
    public const int largePrice = 50000;
}