using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // damage : 총알이 적에게 닿았을 때 줄 데미지.
    // per    : 총알의 관통력. 0 이상이면 관통 가능 횟수, -1이면 무한 관통(근접무기).
    public float damage;
    public int per;

    // rigid : 총알을 움직이기 위한 물리 엔진(Rigidbody2D).
    Rigidbody2D rigid;

    private void Awake()
    {
        // 총알이 생성될 때, 자신의 Rigidbody2D를 가져와 초기화한다.
        // 이후 velocity(선형 속도) 조작을 위해 반드시 필요.
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, int per, Vector3 dir)
    {
        // Init은 PoolManager에서 총알을 꺼낼 때 매번 호출된다.
        // 총알이 새로 만들어지는 것이 아니라, "재사용"되기 때문.
        // 따라서 총알이 어떤 능력치(damage, per)로 동작해야 하는지 매번 설정해줘야 한다.
        this.damage = damage;
        this.per = per;

        // per == -1 인 경우 : 근접 무기처럼 '회전만 하고 날아가지 않는' 무기.
        // 따라서 velocity를 주지 않는다.
        if (per > -1)
        {
            // dir 방향으로 속도를 준다.
            // 15f는 bullet 기본 이동속도 (단위: 유니티 유닛/초).
            rigid.linearVelocity = dir * 15f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Enemy와 충돌하지 않았다면 의미 없음 → return.
        // 또한 per == -1이면 무한 관통근접무기이므로 닿아도 소모되지 않게 early return.
        if (!collision.CompareTag("Enemy") || per == -1)
        {
            return;
        }

        // per 감소.
        // 총알이 적을 한 번 때리면 관통력 1이 줄어든다.
        per--;

        // per가 -1이 되는 시점 → 관통력 모두 소진.
        if (per == -1)
        {
            // 더 이상 날아가지 않아야 하므로 속도 제거.
            rigid.linearVelocity = Vector2.zero;

            // 풀링 방식이므로 지우지 않고 비활성화시켜 풀로 되돌린다.
            gameObject.SetActive(false);
        }
    }

}

