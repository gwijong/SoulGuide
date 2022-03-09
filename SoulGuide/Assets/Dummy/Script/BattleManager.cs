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
    public Queue<AnimationContainer> animQueue = new Queue<AnimationContainer>();

    Stat nextTurnCharacter;

    public Transform[] spawnArea; //씬 로드 시 아군 캐릭터 소환 위치
    public Transform[] enemySpawnArea; //씬 로드 시 적 캐릭터 소환 위치

    public float speedStackLimit = 100;
    public int maxPriorityCount = 10;

    int turnCount = 0;          //턴이 끝날 때 하나 늘어나서 "지난 턴 수 * 버프" 받는 애들 전용으로 두긴 할 거임

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
        //for(int i = 0; i < 8; i++)
        //{
        //    Attack(allyList[0], enemyList[0]);
        //};
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            List<Stat> allCharacter = new List<Stat>();
            allCharacter.AddRange(allyList);
            allCharacter.AddRange(enemyList);

            int fastestTurn = Stat.GetFastestTurn(allCharacter);

            for (int i = 0; i < allCharacter.Count; i++)
            {
                allCharacter[i].TurnPass(fastestTurn);
            };

            int fastestCharacterIndex = Stat.GetFastestCharacter(allCharacter);

            nextTurnCharacter = allCharacter[fastestCharacterIndex];

            Debug.Log(nextTurnCharacter);
            nextTurnCharacter.speedStack -= speedStackLimit;
        }

        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            SpeedSort();
        };

        if(animQueue.Count > 0)
        {
            AnimationContainer currentAnim = animQueue.Peek();
            
            if(currentAnim.Calculate())//끝났는지 확인
            {
                animQueue.Dequeue();//삭제!
            };
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                Attack(allyList[0], enemyList[0]);
                Debug.Log("1번 실행");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Attack(allyList[0], enemyList[1]);
                Debug.Log("2번 실행");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Attack(allyList[0], enemyList[2]);
                Debug.Log("3번 실행");
            };
        };
    }


    void TurnEnd()
    {
        turnCount++;
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
        };
    }

    void Battle()
    {
        if (AnnihilationCheck())
        {
            return;
        }
    }

    void Attack(Stat attacker, Stat victim)
    {
        victim.hp -= attacker.attackDamage;
        animQueue.Enqueue(new AnimationContainer(AnimationType.Move, 0.2f, attacker.transform, victim.transform.position + new Vector3(-1.0f, 0, 0)));
        animQueue.Enqueue(new AnimationContainer(AnimationType.Wait, 1.0f));

        if(victim.hp <= 0)
        {
            animQueue.Enqueue(new AnimationContainer(AnimationType.Die, 1.0f, victim.gameObject));
            enemyList.Remove(victim);
        };

        animQueue.Enqueue(new AnimationContainer(AnimationType.Move, 0.2f, attacker.transform, attacker.transform.position));
    }

    void SpeedSort()  // 다음에 공격할 것으로 예상되는 캐릭터들을 구하기
    {
        Debug.Log("------------------------------------");
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
            int fastestTurn = Stat.GetFastestTurn(allCharList, expectArray);

            //가장 빠른 캐릭터가 100 이상 될 때까지 모든 애들 뱅뱅 돌려줌
            for (int i = 1; i < expectArray.Length; i++)
            {
                expectArray[i] += fastestTurn * allCharList[i].speedWeight;
            };

            while (turnQueue.Count < maxPriorityCount)
            {
                int maxIndex = Stat.GetFastestCharacter(allCharList, expectArray);

                if (expectArray[maxIndex] >= speedStackLimit) //제일 빠른 캐릭터 스피드 누적치가 100 이상이면
                {
                    turnQueue.Add(allCharList[maxIndex]);  //제일 빠른 캐릭터 큐에 넣고
                    Debug.Log(allCharList[maxIndex]);
                    expectArray[maxIndex] -= speedStackLimit; //제일 빠른 캐릭터 스피드를 limit 만큼 빼줌
                }
                else
                {
                    break;
                };
            };
        };
    }



}
