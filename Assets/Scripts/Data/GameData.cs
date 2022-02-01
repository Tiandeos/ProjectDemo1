using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GameData 
{
    public float totalmoney;//şimdilik dursun sadece
    public bool[] levelunlocked;
    public bool[] carunlocked;
    public int[] NitroLevel;
    public int[] EngineLevel;
    public int[] TireLevel;
    public int[] AeroDynamicLevel;
    public int[] SuspansionLevel;
    public int[] TransmissionLevel;
    public int[] EcuLevel;
    public bool isinfiniteunlocked;
    public int ControlType;
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
        ControlType = 0;
        levelunlocked = new bool[5];
        levelunlocked[0] = true;
        isinfiniteunlocked = false;
        carunlocked = new bool[5];
        carunlocked[0] = true;
        NitroLevel = new int[5];
        NitroLevel[0] = 0;
        EngineLevel = new int[5];
        EngineLevel[0] = 0;
        TireLevel = new int[5];
        TireLevel[0] = 0;
        AeroDynamicLevel = new int[5];
        AeroDynamicLevel[0] = 0;
        SuspansionLevel = new int[5];
        SuspansionLevel[0] = 0;
        TransmissionLevel = new int[5];
        TransmissionLevel[0] = 0; 
        language = Language.English;
    }
}
