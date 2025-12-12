using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // ===============================
    //  #1. 프리팹 원본들을 저장하는 배열
    //  - 예: Enemy, Bullet, Effect 등.
    //  - index를 통해 종류를 지정할 수 있다.
    // ===============================
    public GameObject[] prefabs;

    // ===============================
    //  #2. 풀링된 오브젝트들을 보관할 리스트 배열
    //  - pools[0] → Enemy 0번 프리펩 객체들
    //  - pools[1] → Bullet 1번 프리펩 객체들
    //  - 각 리스트는 '같은 종류'를 묶어둔다
    //  - 동적 크기이기 때문에 List 사용
    // ===============================
    List<GameObject>[] pools;


    void Awake()
    {
        // prefabs.Length만큼 풀을 생성한다.
        // 예: 프리펩 8개 → pools도 8개의 리스트를 가진다.
        pools = new List<GameObject>[prefabs.Length];

        // 각 칸마다 독립적인 리스트를 새로 만들어 넣는다.
        for (int index = 0; index < pools.Length; index++)
        {
            pools[index] = new List<GameObject>();
            // → "이 종류의 오브젝트들은 여기로 보관하겠다"는 뜻
        }
    }


    // ==========================================================
    //  #3. 오브젝트를 꺼내는 함수 Get(int index)
    //  - index : 가져오려는 프리팹 종류 (prefabs[index])
    //  - 반환 : 실제 사용 가능한 GameObject
    //  - 로직 :
    //      1) 해당 풀 안의 '비활성화된' 오브젝트 탐색
    //      2) 있다면 꺼내 활성화 → 재사용
    //      3) 없다면 새로 Instantiate 후 풀에 추가
    // ==========================================================
    public GameObject Get(int index)
    {
        // 어떤 오브젝트를 선택했는지 담아둘 변수
        // 초기값 null은 "아직 선택된 오브젝트 없음"을 의미
        GameObject select = null;


        // --------------------------------------------------------
        // 3-1. 풀 내부에 '비활성화된(item.activeSelf == false)' 오브젝트가 있는지 탐색
        //      foreach를 사용하는 이유 :
        //      - 리스트는 크기가 계속 늘어날 수 있기 때문에
        //      - index 기반 for문보다 읽기 쉽고 안정적으로 순회할 수 있다
        // --------------------------------------------------------
        foreach (GameObject item in pools[index])
        {
            if (!item.activeSelf)
            {
                // 사용되지 않는(놀고 있는) 오브젝트 발견
                select = item;
                select.SetActive(true);   // 재사용을 위해 활성화
                break;                   // 찾았으니 더 탐색할 필요 없음
            }
        }


        // --------------------------------------------------------
        // 3-2. 풀에 비활성화된 오브젝트가 하나도 없을 경우
        //       → 새로 생성하여 select에 할당
        //
        // select == null 과 동일한 의미.
        // --------------------------------------------------------
        if (!select)
        {
            // prefab[index] 원본을 기반으로 새 객체 생성
            select = Instantiate(prefabs[index], transform);

            // 새로 만든 객체를 해당 풀 리스트에 추가
            pools[index].Add(select);
        }


        // 최종적으로 선택된 오브젝트 리턴
        return select;
    }
}

