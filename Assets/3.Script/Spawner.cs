//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // ================================================
    //  #1. 스포너가 몬스터를 소환하기 위해 필요한 것들
    // ------------------------------------------------
    //  spawnPoint : 스포너 오브젝트의 ‘자식 위치들’
    //               → 여기 있는 각각의 위치 중 랜덤으로 소환한다.
    //
    //  spawnData  : 시간(레벨) 단계에 따라
    //               어떤 몬스터를, 얼마나 자주, 어떤 체력/속도로
    //               소환할 것인지 담아둔 데이터 배열.
    // ================================================
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;

    //레벨은 시간 흐름에 따라 자연스럽게 증가한다 (10초마다 1씩 증가)
    [SerializeField] int level;

    //timer : 마지막 소환 이후 경과한 시간
    float timer;

    private void Awake()
    {
        // ================================================
        //  #2. 자식 오브젝트들의 Transform을 전부 읽어오기
        // ------------------------------------------------
        //  스포너 오브젝트 하위에 Point1, Point2, Point3 ... 를 달아두면,
        //  그 모든 위치를 자동으로 배열에 넣는다.
        //
        //  spawnPoint[0] 은 자기 자신이므로 사용 시 제외한다.
        // ================================================
        spawnPoint = GetComponentsInChildren<Transform>();
    }

    void Update()
    {
        //게임 진행 중이 아닐 때는 소환 중단
        if (!GameManager.instance.isLive)
            return;

        // 시간 흐르기
        timer += Time.deltaTime;

        // ================================================
        //  #3. 게임 진행 시간에 따라 소환 난이도(레벨) 상승
        // ------------------------------------------------
        //  gameTime / 10f → 10초가 지날 때마다 1레벨 증가한다.
        //  Mathf.Min으로 spawnData 범위를 넘지 않게 조정.
        // ================================================
        level = Mathf.Min(
            Mathf.FloorToInt(GameManager.instance.gameTime / 10f),
            spawnData.Length - 1
        );

        // ================================================
        //  #4. 현재 레벨의 spawnTime을 초과하면 소환 실행
        // ------------------------------------------------
        if (timer > spawnData[level].spawnTime)
        {
            timer = 0;
            Spawn();
        }
    }

    void Spawn()
    {
        // ================================================
        //  #5. Object Pool에서 Enemy 프리펩 가져오기
        // ------------------------------------------------
        //  PoolManager의 index=0이 Enemy라고 가정.
        //  Get(0) → (비활성 Enemy 있으면) 꺼내서 활성화,
        //           없으면 새로 생성해서 반환.
        // ================================================
        GameObject enemy = GameManager.instance.pool.Get(0);

        // ================================================
        //  #6. 랜덤 위치에 몬스터 배치
        // ------------------------------------------------
        //  spawnPoint[0]은 자기 자신이므로 1부터 시작.
        //  Random.Range(1, spawnPoint.Length)
        //  → 자식 전부 중 랜덤 하나의 위치에 소환.
        // ================================================
        enemy.transform.position =
            spawnPoint[Random.Range(1, spawnPoint.Length)].position;

        // ================================================
        //  #7. Enemy의 스탯 초기화
        // ------------------------------------------------
        //  현재 레벨에 해당하는 스폰 데이터(속도·체력 등)를 전달한다.
        // ================================================
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }
}

// ======================================================
//  SpawnData : "이 레벨의 몬스터는 어떤 스펙을 가진다"를 저장하는 구조체
// ------------------------------------------------------
//  spriteType : Enemy 애니메이션 종류 구분
//  spawnTime  : 이 몬스터가 몇 초 간격으로 나타나는가
//  health     : 최대 체력
//  speed      : 이동 속도
// ======================================================
[System.Serializable]
public class SpawnData
{
    public int spriteType;
    public float spawnTime;
    public int health;
    public float speed;
}
