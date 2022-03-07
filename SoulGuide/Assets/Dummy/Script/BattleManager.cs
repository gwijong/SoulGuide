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

    Stat attacker;
    Stat victim;

    [SerializeField]
    private int turnCount = 0;
    private bool allyTurn = true;
    private bool mouseInputStop = false;

    public float speed;
    public float limit = 100;
    public int maxPriorityCount = 20;

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
        };
    }

    void Start()
    {


        SpeedSort();
    }


    void Update()
    {

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

    }

    
    void SpeedSort()  //  예상 턴 정렬
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
                    Debug.Log(allCharList[maxIndex]);
                    expectArray[maxIndex] -= limit; //제일 빠른 캐릭터 스피드를 limit 만큼 빼줌
                }
                else
                {
                    break;
                }
            }
        }

    }
}
