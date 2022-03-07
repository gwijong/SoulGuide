using System.Collections;
using System.Collections.Generic;
using UnityEngine.Android;
using UnityEngine;
using System.IO;
using System.Text;

public class GameManager : MonoBehaviour
{

    public string[] NameList;
    public Sprite[] SpriteList;
    public int[] levelList;
    public int[] expList;
    public int[] maxHpList;
    public int[] attackDamageList;
    public int[] defenseList;
    public float[] speedWeightList;
    public float[] speedStackList;
    public List<GameObject> deckList;
    public RuntimeAnimatorController[] aniList;
    public int[] deckPos;

    public int gold;


    UpdateManager _update = new UpdateManager();
    public static UpdateManager update { get { return manager._update; } }
    public static SoundManager soundmanager { get; private set; }
    public static GameManager manager { get; private set; }
    private void Update()
    {
        update.OnUpdate();
    }


    private void Awake()
    {
        soundmanager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        if (manager == null)
        {
            manager = this;
        }
        else
        {
            Destroy(this);
        }       
    }
    

}
[System.Serializable]
public class characterJsonObject
{
    public int gold;

}

[System.Serializable]
public class CharacterData
{
    public int JellyType;
    public int Level;

    public CharacterData(int type, int lv)
    {
        JellyType = type;
        Level = lv;
    }
}


