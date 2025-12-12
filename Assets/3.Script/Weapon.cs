using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    /*
    웨펀스크립트는 풀매니저에서 가져온 근접무기, 원거리무기를 모양새 있게 관리해주는 역할 해줄거다.
    스크립트에는 id, per, prefab id , damage, count ,speed필요.
    변수 초기화 함수 init이 필요.
    여기서 변수 id 는 무기에 id를 주고, id별로 스위치케이스로 관리할거라서 선언.
    switch case 0 : 스피드필요, 돌아야하니까 , 만들고나면 배치필요. 배치함수 선언
    */
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;

    float timer;
    Player player;

    private void Awake()
    {
        player = GameManager.instance.player;
        //게임 시작 시 플레이어 오브젝트를 미리 저장해둔다.
        //무기는 계속 플레이어 주변을 돌거나 발사해야 하므로, 빠르고 일관된 접근을 위해 참조를 잡아둔다.
    }

    private void Update()
    {
        /*이미 교수님 설명 주석이 있어서 건드리지 않음*/

        if (!GameManager.instance.isLive)
            return;

        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            default:
                timer += Time.deltaTime;

                //속사 무기들은 일정 시간 간격(speed)을 기준으로 발사해야 한다.
                if (timer > speed)
                {
                    timer = 0f;
                    Fire(); //발사 로직 실행
                }
                break;
        }

        //test code
        if (Input.GetButtonDown("Jump"))
        {
            LevelUp(10, 1);
        }
    }

    public void LevelUp(float damage, int count)
    {
        //레벨업 시 무기 데미지와 개수를 증가시킨다.
        //Character.WeaponDamage는 캐릭터의 장비 보정 수치이므로 곱해줘야 실제 능력치가 반영된다.
        this.damage = damage * Character.WeaponDamage;
        this.count += count;

        //근접무기(id=0)는 개수가 바뀌면 다시 배치해야 하므로 즉시 Batch() 호출
        if (id == 0)
            Batch();

        //플레이어 장비(장갑/신발)가 영향을 줄 수 있으므로 적용 신호를 보낸다.
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    public void Init(ItemData data)
    {
        //basic Set
        name = "Weapon" + data.itemId; //무기 이름 설정
        transform.parent = player.transform; //무기가 플레이어를 따라다니도록 부모 지정
        transform.localPosition = Vector3.zero; //플레이어 기준 위치 초기화

        //Property Set
        id = data.itemId;
        damage = data.baseDamage * Character.WeaponDamage;  //아이템 기본 데미지 + 캐릭터 보정
        count = data.baseCount + Character.Count;           //기본 개수 + 캐릭터 보정 개수

        //projectile이 prefab 배열 중 어떤 인덱스인지 직접 탐색하여 prefabId에 기록
        //→ PoolManager에서 해당 인덱스를 통해 오브젝트를 꺼내오기 위함.
        for (int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if (data.projectile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }
        }

        //무기 종류에 따라 속성 설정
        switch (id)
        {
            case 0:
                //근접무기 회전 속도
                speed = 150 * Character.WeaponSpeed;
                Batch();  //무기 개수만큼 배치
                break;
            default:
                //원거리 무기는 발사 간격을 의미하는 값
                speed = 0.5f * Character.WeaponRate;
                break;
        }

        //장비(글러브/슈즈)가 영향을 줄 가능성이 있으므로 갱신 신호 추가
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    void Batch() //무기를 플레이어 한테 붙여야한다.
    {
        /* 교수님이 이미 설명해둔 주석은 유지 */

        for (int index = 0; index < count; index++)
        {
            Transform bullet;

            //이미 배치된 총알이 있다면 재사용(오브젝트 풀링 활용)
            if (index < transform.childCount)
            {
                bullet = transform.GetChild(index);
            }
            else
            {
                //없다면 새로 풀에서 가져와 생성
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform; //웨펀을 내 부모로둬야한다,.
            }

            bullet.localPosition = Vector3.zero;       //플레이어 기준 위치 초기화
            bullet.localRotation = Quaternion.identity; //회전값 초기화

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);

            //총알을 플레이어 중심에서 일정 거리 위쪽(1.5f)으로 띄운다.
            //Space.World 로 한 이유: 플레이어의 회전값에 영향을 받지 않기 위해.
            bullet.Translate(bullet.up * 1.5f, Space.World);

            //근접무기는 무한 관통(-1)으로 설정한다.
            bullet.GetComponent<Bullet>().Init(damage, -1, Vector3.zero);
        }
    }

    void Fire()
    {
        //근처에 목표가 없으면 발사할 필요 없다.
        if (!player.scanner.nearstTarget)
        {
            return;
        }

        //타깃 방향 벡터 구하기
        Vector3 targetPos = player.scanner.nearstTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized; //정규화해서 순수 방향만 남긴다

        //새 총알을 풀에서 가져와 발사 준비
        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;

        bullet.position = transform.position; //플레이어 무기 위치에서 발사
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        //위로 향한 총알을 'dir' 방향으로 회전시킨다.

        //총알에 데미지/관통력/방향 전달
        bullet.GetComponent<Bullet>().Init(damage, count, dir);

        //발사음 재생
        AudioManager.instance.Playsfx(AudioManager.Sfx.Range);
    }
}

