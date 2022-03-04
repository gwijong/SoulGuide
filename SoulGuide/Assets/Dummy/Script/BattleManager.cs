using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager battleManager { get; private set; }

    public GameObject[] allyCharacter;  // 아군 캐릭터들 앞 오른쪽 왼쪽 뒤 순서
    public GameObject[] Wave1enemyCharacter;  // 1웨이브 적 캐릭터들
    public GameObject[] Wave2enemyCharacter;  // 2웨이브 적 캐릭터들
    public GameObject[] Wave3enemyCharacter;  // 3웨이브 적 캐릭터들

    public GameObject[] spawnArea; //씬 로드 시 아군 캐릭터 소환 위치
    public GameObject[] anemySpawnArea; //씬 로드 시 적 캐릭터 소환 위치

    [SerializeField]
    private int turnCount = 1;
    private bool allyTurn = true;

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
        Battle();       
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0)&& allyTurn)
        {
            OnAttack(allyCharacter[turnCount], Wave1enemyCharacter[turnCount]);
            turnCount++;
        }else if (Input.GetMouseButtonDown(0) && !allyTurn)
        {
            OnAttack(Wave1enemyCharacter[turnCount- allyCharacter.Length], allyCharacter[turnCount- allyCharacter.Length]);
            turnCount++;
        }
        if (turnCount>= allyCharacter.Length && allyTurn)
        {
            allyTurn = false;
        }else if(turnCount >= allyCharacter.Length+Wave1enemyCharacter.Length && !allyTurn)
        {
            turnCount = 1;
            allyTurn = true;
        }
    }

    void Battle()
    {

    }

    public void OnAttack(GameObject attacker, GameObject victim)
    {
        if(attacker.activeSelf == false || victim.activeSelf == false)
        {
            return;
        }
        int damage = attacker.GetComponent<Stat>().Attack;
        int Hp = victim.GetComponent<Stat>().Hp;
        Hp = Hp - damage;
        victim.GetComponent<Stat>().Hp = Hp;
        Debug.Log
        ($"{attacker.name} 캐릭터가 {victim.name} 캐릭터를 공격, \n" +
        $"공격자 데미지{attacker.GetComponent<Stat>().Attack}, 피격자 남은 생명력{victim.GetComponent<Stat>().Hp}");
        if (victim.GetComponent<Stat>().Hp <= 0)
        {
            victim.GetComponent<Stat>().Hp = 0;
            OnDead(victim);
        }
    }

    protected void OnDead(GameObject victim)
    {
        Debug.Log($"{victim.name} 캐릭터 사망");
        victim.SetActive(false);
    }
}
