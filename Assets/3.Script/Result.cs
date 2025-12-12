using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Result : MonoBehaviour
{
    // ============================================
    //  #1. 결과 화면에서 표시할 타이틀 오브젝트들
    //  - 0번 : 패배 화면
    //  - 1번 : 승리 화면
    //  - GameManager에서 게임 승패가 결정되면,
    //    각각 알맞은 타이틀만 활성화된다.
    // ============================================
    public GameObject[] titles;


    // ============================================
    //  #2. 패배 시 호출되는 함수
    //  - titles[0] : "Lose"에 해당하는 오브젝트
    //  - 단순히 활성화 시켜 표시한다
    // ============================================
    public void Lose()
    {
        titles[0].SetActive(true);
    }


    // ============================================
    //  #3. 승리 시 호출되는 함수
    //  - titles[1] : "Win"에 해당하는 오브젝트
    //  - 역시 단순히 활성화하는 구조
    // ============================================
    public void Win()
    {
        titles[1].SetActive(true);
    }

}

