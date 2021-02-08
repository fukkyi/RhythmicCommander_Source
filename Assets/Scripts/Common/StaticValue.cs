using UnityEngine;

public static class StaticValue
{
    public static string loadName = string.Empty;
    public static string playerName = "Guest";
    public static float noteSpeed = 1.0f;
    public static int judgeOffset = 0;
    public static int trackCount = 1;
    public static bool isAuto = false;
    public static Difficulty difficulty = Difficulty.Standard;
    public static GaugeRule gaugeRule = GaugeRule.Normal;
    public static JudgeStyle judgeStyle = JudgeStyle.Normal;
    public static CacheStruct[] cacheStructs = null;
}
