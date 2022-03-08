using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager battleManager { get; private set; }

    public List<Stat> allyList; // 아군 캐릭터들 앞 오른쪽 왼쪽 뒤 순서
    public List<Stat> enemyList; // 1웨이브 적 캐릭터들

    public List<Stat> turnQueue = new List<Stat>();

    public Transform[] spawnArea; //씬 로드 시 아군 캐릭터 소환 위치
    public Transform[] anemySpawnArea; //씬 로드 시 적 캐릭터 소환 위치

    public float speed;
    public float limit = 100;
    public int maxPriorityCount = 400;

    Stat attacker;
    Stat victim;

    bool attacking = false;
    bool move = false;
    public float attackMoveSpeed = 1.0f;
    //[SerializeField]

    int allyArrayCount = 0;
    int enemyArrayCount = 0;

    Vector3[] returnPosition = new Vector3[40];
    int turnCount = 0;



    private void Awake()
    {
        if (battleManager == null)
        {
            battleManager = this;
        }
        else
        {
            Destroy(this);
        };
    }

    void Start()
    {
        SpeedSort();
        attacker = turnQueue[turnCount];


        for(int i =0; i<turnQueue.Count; i++)
        {
            returnPosition[i] = turnQueue[i].transform.position;
        }

    }


    void Update()
    {
        Battle();
    }

    void OnAttack(Stat attacker, Stat victim)
    {
        victim.TakeDamage(attacker.attackDamage);
    }

    void Dead(Stat victim)
    {
        if (victim.hp <= 0)
        {
            victim.gameObject.SetActive(false);
            Debug.Log($"{ victim.name} 사망");
        }        
    }
    
    void TurnEnd()
    {
        ReturnPos();
        turnQueue[turnCount].speedStack -= 100;
        turnCount++;
        attacker = turnQueue[turnCount];
    }

    void AttackMove(Stat attacker, Stat victim)
    {
        float distance = (victim.transform.position - attacker.transform.position).magnitude;
        Vector2 direction = (victim.transform.position - attacker.transform.position).normalized;
        if (distance >= 2 && attacking == true)
        {
            attacker.transform.Translate(direction * attackMoveSpeed);
        }
        else
        {
            move = false;
        }
    }

    void ReturnPos()
    {
        attacker.transform.position = returnPosition[turnCount];
    }

    bool AnnihilationCheck()
    {
        if (allyList.Count == 0)
        {
            Debug.Log("아군 전멸 전투종료");
            return true;
        }
        else if (enemyList.Count == 0)
        {
            Debug.Log("적군 전멸 전투종료");
            return true;
        }
        else 
        {
            return false;
        
        }
    }

    void Battle()
    {
        if (AnnihilationCheck())
        {
            return;
        }
        if (attacker.tag == "Enemy")
        {
            if (allyList[allyArrayCount].gameObject.activeSelf == false)
            {
                allyArrayCount++;               
            }
            victim = allyList[allyArrayCount];

        }
        else if (attacker.tag == "Ally")
        {
            if (enemyList[enemyArrayCount].gameObject.activeSelf == false)
            {
                enemyArrayCount++;
            }
            victim = enemyList[enemyArrayCount];
        }

        if (!attacking)
        {
            attacking = true;
            StartCoroutine("Attacking");
        }
        
        if(move)
        {
            AttackMove(attacker, victim);
        }
        
    }



    void SpeedSort()  //  선택정렬
    {
        turnQueue.Clear();  // 기존 턴 초기화

        List<Stat> allCharList = new List<Stat>();  //적과 아군 리스트 합침
        allCharList.AddRange(allyList);
        allCharList.AddRange(enemyList);


        float[] expectArray = new float[allCharList.Count];  // 예상 스택 배열

        for (int i = 0; i < expectArray.Length; i++)
        {
            expectArray[i] = allCharList[i].speedStack;  // 모든 캐릭터 스피드 누적치 대입
        };

        while(turnQueue.Count< maxPriorityCount)
        {   
            int fastestTurn = allCharList[0].Priority(expectArray[0]); // 제일 빨리 도는 캐릭터 턴

            //제일 빨리 도는 캐릭터의 턴 수 찾기
            for (int i = 1; i < expectArray.Length; i++)
            {
                int currentPriority = allCharList[i].Priority(expectArray[i]);

                if (fastestTurn > currentPriority)
                {
                    fastestTurn = currentPriority;
                };
            };

            //가장 빠른 캐릭터가 100 이상 될 때까지 모든 애들 뱅뱅 돌려줌
            for (int i = 1; i < expectArray.Length; i++)
            {
                expectArray[i] += fastestTurn * allCharList[i].speedWeight;
            };


            while (turnQueue.Count < maxPriorityCount)
            {
                float maxSpeed = expectArray[0];  // 가장 빠른 캐릭터 누적치
                int maxIndex = 0; //예상 배열의 가장 빠른캐릭터

                for (int j = 0; j < expectArray.Length; j++)  //예상 배열 길이만큼 뱅뱅 반복
                {
                    if (maxSpeed < expectArray[j]) // 가장 빠른 캐릭터 스피드 비교
                    {
                        maxSpeed = expectArray[j]; // 예상 배열의 가장 빠른캐릭터의 스피드 누적치 대입
                        maxIndex = j; //예상 배열의 가장 빠른캐릭터의 인덱스
                    }
                }

                if (maxSpeed >= limit) //제일 빠른 캐릭터 스피드 누적치가 100 이상이면
                {
                    turnQueue.Add(allCharList[maxIndex]);  //제일 빠른 캐릭터 큐에 넣고
                    
                    //Debug.Log(allCharList[maxIndex]);
                   
                    expectArray[maxIndex] -= limit; //제일 빠른 캐릭터 스피드를 limit 만큼 빼줌
                }
                else
                {
                    break;
                }
            }
        }
    }
    IEnumerator Attacking()
    {
        move = true;
        yield return new WaitForSeconds(turnQueue[turnCount].attack1DurationTime);
        OnAttack(attacker, victim);
        move = false; 
        attacking = false;
        TurnEnd();
    }


}
