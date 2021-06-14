internal static class AppData
{

    internal const int maxSemiLevel = 6;
    internal static int currentScore;
    internal static int watchAdCountdown = 5;
    internal static int ballsOverCooldownTime = 5;

    //Achivements
    internal const int achievementValue1 = 50;
    internal const int achievementValue2 = 100;
    internal const int achievementValue3 = 250;
    internal const int achievementValue4 = 500;
    internal const int achievementValue5 = 1000;
    internal const int achievementValue6 = 1500;
    internal const int achievementValue7 = 3000;
    internal const int achievementValue8 = 5000;
    internal const int achievementValue9 = 10000;

    //IAP
    internal const string gemsTier1ProductId = "com.bronz.bathit.coins500";
    internal const string gemsTier2ProductId = "com.bronz.bathit.coins3500";
    internal const string gemsTier3ProductId = "com.bronz.bathit.coins8000";
    internal const string gemsTier4ProductId = "com.bronz.bathit.coins15000";

    internal const int gemsTier1GemsValue = 500;
    internal const int gemsTier2GemsValue = 3000;
    internal const int gemsTier3GemsValue = 7500;
    internal const int gemsTier4GemsValue = 16000;

    //Tags
    internal const string tag_block = "Block";
    internal const string tag_ball = "Ball";
    internal const string tag_bowlingPins = "BowlingPin";

    //Ball variables
    internal const float minSpeed = 12;
    internal const float maxSpeed = 18;

    //Tile Manager
    internal const int gemSpawnRate = 8;//higher is less spawn

    //Rewards
    internal const int nextRewardInHours = 6;
    internal const int adGemsRewards = 50;
    internal const int dailyGemsRewards = 75;

    //Misc
    internal const float floorSaturation = 0.8f;
    internal const float floorLightness = 0.75f;

    internal const float shopAnimSpeed = 0.25f;
    internal const float gemsAnimSpeed = 0.75f;

    //Local Playerprefs keys
    internal const string localSaveKey = "localSave";
    internal const string oldSaveKey = "BestScore";

    internal const string shopItemsDbJsonPath = "ShopDatabase/ShopDatabase";
    internal const string allShopItemsIconPath = "AllItemIcons";
    internal const string allShopItemsMatPath = "AllItemMaterials";
    internal const string platformLevelPath = "Platforms/Level";

    internal const string gemIcon = "<sprite=0>";
    internal const string adIcon = "<sprite=1>";
    internal const string ballIcon = "<sprite=2>";

    internal const string saveVersion = "0.1";
}