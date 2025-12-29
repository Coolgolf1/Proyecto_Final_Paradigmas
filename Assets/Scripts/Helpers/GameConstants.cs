using UnityEngine;

public static class GameConstants
{
    // Game parameters
    public const int minTravellersCreatedInAirport = 100;
    public const int maxTravellersCreatedInAirport = 1000;

    public const int minTravellersRandom = 0;
    public const int maxTravellersRandom = 10;

    public const int maxTravellersInAirport = 500; // 500
    public const float AirportTravellersUpgrade = 0.5f;

    public static Vector3 mainMenuCameraPosition = new Vector3(27.6100006f, 36.0400009f, -28.3400002f);
    public static Quaternion mainMenuCameraRotation = new Quaternion(-0.331260532f, 0.501139224f, -0.113941371f, -0.791292191f);
    public static Vector3 initCameraPosition = new Vector3(52.46f, 1.1f, 8.75f);
    public static Quaternion initCameraRotation = new Quaternion(0f, 0.766044259f, 0f, -0.642787814f);

    public static Vector3 settingsCameraPosition = new Vector3(3.74000001f, -31.1100006f, 6.69000006f);
    public static Quaternion settingsCameraRotation = new Quaternion(-0.2301296f, -0.702412069f, -0.428652734f, 0.519532919f);
    public static Quaternion settingsSunRotation = new Quaternion(-0.288833886f, 0.484140009f, 0.719054401f, -0.406379402f);


    public static Quaternion menuSunRotation = new Quaternion(-0.549872696f, 0.283733606f, 0.545989275f, -0.564828336f);

    // Airplane stats
    public const double relativeSpeed = 0.0463;
    public const double speedMultiplier = 1000;

    // Small Airplane
    public const double smallRange = 5000;
    public const double smallSpeedMultiplier = 3;
    public const double smallPriceMultiplier = 1.06;
    public const int smallCapacity = 100;
    public const int smallPrice = 10000;

    // Medium Airplane
    public const double mediumRange = 10000;
    public const double mediumSpeedMultiplier = 4;
    public const double mediumPriceMultiplier = 1.07;
    public const int mediumCapacity = 200;
    public const int mediumPrice = 22500;

    // Large Airplane
    public const double largeRange = 20000;
    public const double largeSpeedMultiplier = 4.7;
    public const double largePriceMultiplier = 1.08;
    public const int largeCapacity = 300;
    public const int largePrice = 50000;
}