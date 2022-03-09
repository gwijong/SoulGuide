using System;
using UnityEngine;

public enum AnimationType
{
    Move,
    Attack,
    Wait,
    Die
}

public class AnimationContainer
{
    public AnimationType type;      //원하는 애니메이션
    public float timeMax;           //애니메이션이 끝날 시간
    public float timeCurrent = 0;   //애니메이션이 얼마나 지났는지
    public object arg1;
    public object arg2;
    public object arg3;
    private Action animationFunction;   //애니메이션 업데이트용 함수
    private Action initialFunction;     //애니메이션 초기화용 함수
    private Action animationEnd;

    public AnimationContainer(AnimationType wantType, float wantTime, object wantArg1 = null, object wantArg2 = null, object wantArg3 = null)
    {
        type = wantType;
        switch(type)
        {
            case AnimationType.Wait:
                animationFunction = CalculateWait;
                break;
            case AnimationType.Move:
                animationFunction = CalculateMove;
                initialFunction = InitializeMove;
                break;
            case AnimationType.Die:
                animationEnd = EndDie;
                break;
            default: 
                animationFunction = null;
                break;
        };
        timeMax = wantTime;
        arg1 = wantArg1;
        arg2 = wantArg2;
        arg3 = wantArg3;
    }

    public bool Calculate()
    {
        //이제 시작했으니까   그리고 초기화 함수가 있어!         초기화!
        if (timeCurrent == 0 && initialFunction != null) initialFunction();

        timeCurrent += Time.deltaTime; //얼마나 지났는지 쓰기 위해서 시간을 넣어주기!

        if (timeCurrent >= timeMax || animationFunction == null)
        {
            if (animationEnd != null)
            {
                animationEnd();
            }
            return true;
        }
        
        animationFunction(); //애니메이션 기능 실행하기


        return false; //아직 안끝났다!
    }

    public void CalculateWait()
    {

    }

    public void InitializeMove()
    {
        arg3 = ((Transform)arg1).position;
    }

    public void CalculateMove()
    {
        //arg1 : 움직이는 애의 Transform
        //arg2 : 목적지        Vector3
        //arg3 : 시작지점      Vector3
        float timeLeft = Mathf.Clamp((timeMax - timeCurrent) / timeMax, 0, 1); //남은 시간의 비율 (0 ~ 1)

        //남은 시간이 크면 클수록 시작지점에 가깝고
        //남은 시간이 작을수록 목적지에 가깝다

        //                                            0 ~ 1                                 1 ~ 0
        // 캐릭터의 위치              =    시작지점 * 남은시간              목적지      *   (1 - 남은시간)        
        ((Transform)arg1).position = (((Vector3)arg3) * timeLeft) + (((Vector3)arg2) * (1 - timeLeft));
    }

    public void EndDie()  // 사망 애니메이션의 맨 마지막
    {
        GameObject.Destroy((GameObject)arg1);
    }
}