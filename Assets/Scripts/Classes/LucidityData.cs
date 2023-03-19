using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEditorInternal;
using UnityEngine;

public class LucidityData
{
    private static Dictionary<string, LevelData> levels;

    // lightLevels
    private static float baseLight = 0.7f; 
    private static float highLight = 1.4f; 
    private static float medLight = 2f; 
    private static float lowLight = 3.5f;

    // attackDamageMod
    private static float baseAttackDamageMod = 1;
    private static float highAttackDamageMod = 0.8f;
    private static float medAttackDamageMod = 0.6f;
    private static float lowAttackDamageMod = 0.5f;

    // increasedDamageFeltMod
    private static float baseIncreaseDamageFelt = 1;
    private static float highIncreaseDamageFelt = 1.25f;
    private static float medIncreaseDamageFelt = 1.5f;
    private static float lowIncreaseDamageFelt = 2f;

    // musicChanges
    private static float baseMusicChanges = 1;
    private static float highMusicChanges = 1.1f;
    private static float medMusicChanges = 1.2f;
    private static float lowMusicChanges = 1.5f;

    // SFXToLoop
    private static string baseSFXToLoop = "";
    private static string highSFXToLoop = "";
    private static string medSFXToLoop = "";
    private static string lowSFXToLoop = "";

    //Declarations
    public static LevelData baseLevel = new LevelData(baseLight, baseAttackDamageMod, baseIncreaseDamageFelt, baseMusicChanges, baseSFXToLoop);
    public static LevelData highLevel = new LevelData(highLight, highAttackDamageMod, highIncreaseDamageFelt, highMusicChanges, highSFXToLoop);
    public static LevelData medLevel = new LevelData(medLight, medAttackDamageMod, medIncreaseDamageFelt, medMusicChanges, medSFXToLoop);
    public static LevelData lowLevel = new LevelData(lowLight, lowAttackDamageMod, lowIncreaseDamageFelt, lowMusicChanges, lowSFXToLoop);


    static LucidityData()
    {
        levels = new Dictionary<string, LevelData>
        {
            // add level data
            { "Base", new LevelData(baseLevel) },
            { "High", new LevelData(highLevel) },
            { "Med", new LevelData(medLevel) },
            { "Low", new LevelData(lowLevel) }
        }; 
    }

    public static LevelData GetValue(string key)
    {
        if (levels.ContainsKey(key)) { return levels[key]; }
        else { Debug.LogWarning($"Key '{key}' not found in dictionary!"); return null; }
    }

    [Serializable]
    public class LevelData
    {
        public float lightValue;
        public float attackDamageMod;
        public float increaseDamageMod;
        public float musicToPlay;
        public string SFXToLoop;

        public LevelData(float lightValue, float attackDamageMod, float increaseDamageMod, float musicToPlay, string SFXToLoop)
        {
            this.lightValue = lightValue;
            this.attackDamageMod = attackDamageMod;
            this.increaseDamageMod = increaseDamageMod;
            this.musicToPlay = musicToPlay;
            this.SFXToLoop = SFXToLoop;
        }

        public LevelData(LevelData level)
        {
            this.lightValue = level.lightValue;
            this.attackDamageMod = level.attackDamageMod;
            this.increaseDamageMod = level.increaseDamageMod;
            this.musicToPlay = level.musicToPlay;
            this.SFXToLoop = level.SFXToLoop;
        }
    }


}
