using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager battleManager { get; private set; }

    public List<GameObject> allyList; // 아군 캐릭터들 앞 오른쪽 왼쪽 뒤 순서
    public List<GameObject> enemyList; // 1웨이브 적 캐릭터들

    public GameObject[] spawnArea; //씬 로드 시 아군 캐릭터 소환 위치
    public GameObject[] anemySpawnArea; //씬 로드 시 적 캐릭터 소환 위치

    GameObject attacker;
    GameObject victim;

    [SerializeField]
    private int turnCount = 0;
    private bool allyTurn = true;
    private bool mouseInputStop = false;

    public float speed;

    Vector3 returnPos;
    bool battleFinish = false;

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
        Battle();
        CharacterMove();
    }

    private void CharacterMove()
    {
        if(victim == null || attacker == null || battleFinish == true)
        {
            return;
        }

        Vector3 dir = (victim.transform.position - attacker.transform.position).normalized;
        if ((victim.transform.position - attacker.transform.position).magnitude > 2.0f )
        {
            attacker.transform.Translate(dir*Time.deltaTime*speed);
        }
        else
        {
            mouseInputStop = false;
            if (victim.GetComponent<Stat>().Hp <= 0)
            {
                victim.GetComponent<Stat>().Hp = 0;
                OnDead(victim);
            }
        }
    }

    void Battle()
    {
    
        if (Input.GetMouseButtonDown(0) && allyTurn && turnCount < allyList.Count && mouseInputStop ==false)
        {
            mouseInputStop = true;
            if (turnCount == 0 && attacker != null)
            {
                attacker.transform.position = returnPos;
                attacker = null;
            }

            if (enemyList.Count==0)
            {
                Debug.Log("enemy 전멸");
                attacker.transform.position = returnPos;
                battleFinish = true;
                return;
            }

            if (victim!= null && victim.GetComponent<Stat>().Hp <= 0)
            {
                victim.GetComponent<Stat>().Hp = 0;
                OnDead(victim);
            }

            OnAttack(allyList[turnCount], enemyList[0]);
            if (attacker != null && attacker.tag == "Ally")
            {
                attacker.transform.position = returnPos;
            }

            returnPos = allyList[turnCount].transform.position;
            attacker = allyList[turnCount];
            if (enemyList.Count != 0)
            {
                victim = enemyList[0];
            }
            else
            {
                battleFinish = true;
            }
          
            turnCount++;
        }
        else if (Input.GetMouseButtonDown(0) && !allyTurn && turnCount < enemyList.Count && mouseInputStop == false)
        {
            mouseInputStop = true;
            if(turnCount == 0 && attacker != null)
            {
                attacker.transform.position = returnPos;
                attacker = null;
            }
            if (allyList.Count == 0)
            {
                Debug.Log("ally 전멸");
                attacker.transform.position = returnPos;
                battleFinish = true;
                return;
            }
            OnAttack(enemyList[turnCount], allyList[0]);
            
            if (attacker != null && attacker.tag == "Enemy")
            {
                attacker.transform.position = returnPos;
            }

            returnPos = enemyList[turnCount].transform.position;
            attacker = enemyList[turnCount];
            if (enemyList.Count != 0)
            {
                victim = allyList[0];
            }
            else
            {
                battleFinish = true;
            }
            turnCount++;
        }

        if (turnCount >= allyList.Count && allyTurn)
        {
            allyTurn = false;
            turnCount = 0;
        }
        else if (turnCount >= enemyList.Count && !allyTurn)
        {
            turnCount = 0;
            allyTurn = true;
        }
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

    }

    protected void OnDead(GameObject victim)
    {
        if(victim.activeSelf == true)
        {
            Debug.Log($"{victim.name} 캐릭터 사망");
        }     
        if (victim.tag == "Enemy")
        {
            enemyList.Remove(victim);
        }
        else if (victim.tag == "Ally")
        {
            allyList.Remove(victim);
        }
        victim.SetActive(false);
    }

    void MouseInput()
    {
        //RaycastHit Hit = 
            
    }
}
