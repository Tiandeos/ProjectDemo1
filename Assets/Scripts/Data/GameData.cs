using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GameData 
{
    public float totalmoney;//şimdilik dursun sadece
    public bool[] levelunlocked;
    public bool[] carunlocked;
    public bool[] patternunlocked;
    public bool[] colorunlockedforneedle;
    public bool[] colurunlockedforsteer;
    public int[] NitroLevel;
    public int[] EngineLevel;
    public bool isControlChanged;
    public int[] TireLevel;
    public int[] AeroDynamicLevel;
    public int[] SuspansionLevel;
    public int[] TransmissionLevel;
    public int[] EcuLevel;
    public float[] topspeed;
    public float[] motorforce;
    public int[] maxgear;
    public bool isinfiniteunlocked;
    public enum LaneChangeType
    {
        StrictLaneChange,
        FreeLaneChange
    }
    public LaneChangeType laneChangeType;
    public int qualitysetting;//Oyunun ayarı filan kullanılır mı bilmiyorum
    public enum Language //Ulan ben bu videoyu izlediğim kişi niye yapmıyor diye düşünüyorum hala gdrkoısergkoersa
    {
        Turkish,
        English
    }
    public Language language;
    public GameData() 
    {
        totalmoney = 0;
        levelunlocked = new bool[999];
        levelunlocked[0] = true;
        isinfiniteunlocked = false;
        carunlocked = new bool[999];
        carunlocked[0] = true;
        NitroLevel = new int[999];
        NitroLevel[0] = 0;
        EngineLevel = new int[999];
        EngineLevel[0] = 0;
        TireLevel = new int[999];
        isControlChanged = false;
        TireLevel[0] = 0;
        AeroDynamicLevel = new int[999];
        AeroDynamicLevel[0] = 0;
        SuspansionLevel = new int[999];
        SuspansionLevel[0] = 0;
        TransmissionLevel = new int[999];
        TransmissionLevel[0] = 0;
        EcuLevel = new int[999]; 
        EcuLevel[0] = 0;
        patternunlocked = new bool[999];
        colorunlockedforneedle = new bool[999];
        colurunlockedforsteer = new bool[999];
        language = Language.English;
    }
}
