using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager battleManager { get; private set; }

    public GameObject[] allyCharacter;  // 아군 캐릭터들
    public GameObject[] enemyCharacter1;  // 1웨이브 적 캐릭터들
    public GameObject[] enemyCharacter2;  // 2웨이브 적 캐릭터들
    public GameObject[] enemyCharacter3;  // 3웨이브 적 캐릭터들

    public GameObject[] spawnArea;
    public GameObject[] anemySpawnArea;

    private void Awake()
    {
        if (battleManager == null)
        {
            battleManager = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    void Turn()
    {
        OnAttack(allyCharacter[0], enemyCharacter1[0]);
        OnAttack(allyCharacter[1], enemyCharacter1[1]);
        OnAttack(allyCharacter[2], enemyCharacter1[2]);
        OnAttack(enemyCharacter1[0], allyCharacter[0]);
        OnAttack(enemyCharacter1[1], allyCharacter[1]);
        OnAttack(enemyCharacter1[2], allyCharacter[2]);
    }

    public void OnAttack(GameObject attacker, GameObject victim)
    {
        int damage = attacker.GetComponent<Stat>().Attack;
        int Hp = victim.GetComponent<Stat>().Hp;
        Hp = Hp - damage;
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead(victim);
        }
    }

    protected void OnDead(GameObject victim)
    {
        Debug.Log("캐릭터 사망");
        this.enabled = false;
    }

}
