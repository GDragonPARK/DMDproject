using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    /*
     이 스크립트는 적 몬스터의 ‘추적, 피격, 넉백, 사망’ 전체 생애주기를 관리한다.
     즉, 생성되면 플레이어를 향해 따라붙고,
     총알에 맞으면 데미지를 계산하며,
     체력이 0이 되면 죽는 일련의 과정이 모두 들어있는 핵심 로직이다.
    */

    // 몬스터의 이동속도(초당)
    public float speed;

    // 현재 체력과 최대 체력
    public float health;
    public float maxHealth;

    // 추적할 목표. Rigidbody2D로 잡은 이유는 MovePosition과 위치 비교 때문.
    public Rigidbody2D target;

    // 몬스터 종류별 애니메이터 컨트롤러 묶음 (스폰 시 외형을 여기서 결정)
    public RuntimeAnimatorController[] animcon;

    // 생존 여부 체크용
    bool isLive;

    // 몬스터 자신이 가진 물리, 충돌, 애니메이션, 스프라이트 렌더러
    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    SpriteRenderer spriter;

    // 넉백 효과를 줄 때 한 물리 프레임 기다리기 위한 용도
    WaitForFixedUpdate wait;


    void Awake()
    {
        // 컴포넌트 초기화. 적 오브젝트가 생성되면 실행됨.
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
    }


    private void OnEnable()
    {
        // 플레이어의 Rigidbody를 타겟으로 설정
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();

        // 부활 또는 재사용되었으니 상태 초기화
        isLive = true;
        coll.enabled = true;      // 충돌 가능
        rigid.simulated = true;   // 물리 연산 활성화
        spriter.sortingOrder = 2; // 살아있는 동안은 몬스터가 플레이어 위에 표시됨
        anim.SetBool("Dead", false);
        health = maxHealth;
    }


    public void Init(SpawnData data)
    {
        // 외형 선택
        anim.runtimeAnimatorController = animcon[data.spriteType];

        // 스탯 설정
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
    }


    void FixedUpdate()
    {
        // 게임 오버면 적 움직임도 멈춘다.
        if (!GameManager.instance.isLive)
            return;

        // (플레이어 위치 - 내 위치) = 방향 벡터
        Vector2 dirVec = target.position - rigid.position;

        // 정규화해서 방향만 얻고, 속도와 fixedDeltaTime을 곱해 ‘다음 위치값’ 생성
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;

        // rigid 이동. MovePosition을 쓰기 때문에 물리적으로 부드러운 추적
        rigid.MovePosition(rigid.position + nextVec);

        // 충돌 시 반동이 발생하면 안되므로 속도는 항상 0으로 유지
        rigid.linearVelocity = Vector2.zero;
    }


    void LateUpdate()
    {
        // 게임오버 / 죽음 / 피격모션 중에는 방향전환 금지
        if (!GameManager.instance.isLive)
            return;
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;

        // 플레이어 위치 기준으로 좌우 방향을 판정하여 sprite.flipX 조정
        spriter.flipX = target.position.x < rigid.position.x;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 총알이 아니거나 이미 죽어있다면 무시
        if (!collision.CompareTag("bullet") || !isLive)
            return;

        // 피격시 체력 감소
        health -= collision.GetComponent<Bullet>().damage;

        // 넉백 효과 코루틴 실행
        StartCoroutine(KonckBack());

        // 체력이 남아있으면 피격 애니메이션 + 효과음
        if (health > 0)
        {
            anim.SetTrigger("Hit");
            AudioManager.instance.Playsfx(AudioManager.Sfx.Hit);
        }
        else
        {
            // 이제 사망 처리
            isLive = false;
            coll.enabled = false;    // 충돌 제거
            rigid.simulated = false; // 물리 제거
            spriter.sortingOrder = 1; // 죽은 시체는 배경처럼 아래쪽 레이어로
            anim.SetBool("Dead", true);

            // 킬 증가 및 경험치 획득
            GameManager.instance.Kill++;
            GameManager.instance.GetExp();

            if (GameManager.instance.isLive)
                AudioManager.instance.Playsfx(AudioManager.Sfx.Dead);
        }


        // --- 내부 코루틴 및 함수 ---

        IEnumerator KonckBack()
        {
            // 한 프레임 기다렸다가 넉백 처리
            yield return wait;

            Vector3 playerPos = GameManager.instance.player.transform.position;
            Vector3 dirVec = transform.position - playerPos;

            // 플레이어와 반대 방향으로 순간적 넉백
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }

        void Dead()
        {
            // 풀링을 위한 비활성화 처리
            gameObject.SetActive(false);
        }
    }
}

