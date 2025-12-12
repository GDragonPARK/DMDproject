using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // inputVec : 플레이어가 어느 방향으로 움직이고 싶은지 저장하는 2D 벡터.
    //            x는 좌우, y는 상하 입력값을 담는다.
    public Vector2 inputVec;

    // rigid : 플레이어를 실제로 움직이게 만드는 물리 컴포넌트(Rigidbody2D).
    [SerializeField] Rigidbody2D rigid;

    // sprite : 플레이어의 스프라이트(이미지)를 담당. flipX로 좌우 반전할 때 필요.
    SpriteRenderer sprite;

    // Speed : 플레이어 기본 이동 속도. 캐릭터 특성(Character.Speed)까지 곱해져 최종 속도가 된다.
    public float Speed;

    // anim : 플레이어 애니메이션 제어(Idle, Walk, Dead 등).
    Animator anim;

    // scanner : 주변 적을 탐색해 가장 가까운 적을 찾는 역할(원거리 무기 조준용).
    public Scanner scanner;

    // animCon : 캐릭터 종류(PlayerId)에 따라 애니메이션 컨트롤러를 바꿔주기 위한 배열.
    public RuntimeAnimatorController[] animCon;


    // OnMove : New Input System용 콜백 함수.
    //          value로부터 방향 입력(Vector2)을 받아 inputVec에 저장한다.
    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }


    private void Awake()
    {
        // 플레이어가 생성될 때(씬 로드 시) 필요한 컴포넌트들을 모두 가져온다.
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
    }


    private void OnEnable()
    {
        // 캐릭터 선택에 따라 이동 속도 보정값을 곱해 최종 이동 속도를 결정한다.
        Speed *= Character.Speed;

        // PlayerId에 맞는 애니메이션 컨트롤러를 장착한다.
        anim.runtimeAnimatorController = animCon[GameManager.instance.PlayerId];
    }


    private void FixedUpdate()
    {
        // 게임이 끝났다면 더 이상 움직이지 않는다.
        if (!GameManager.instance.isLive)
            return;

        // inputVec을 정규화(normalized)하여 대각선 이동 시 속도 과다 문제를 해결한다.
        // Speed와 fixedDeltaTime을 곱해 어느 컴퓨터에서든 일정한 속도로 이동하도록 보정한다.
        Vector2 Newvec = inputVec.normalized * Speed * Time.fixedDeltaTime;

        // Rigidbody2D를 이용해 부드럽게 이동.
        rigid.MovePosition(rigid.position + Newvec);
    }


    private void Update()
    {
        // 마찬가지로 게임 오버면 입력을 받지 않는다.
        if (!GameManager.instance.isLive)
            return;

        // 구 Input 시스템(키보드 직접 입력)으로 방향 값을 갱신.
        // 현재는 New Input System과 혼용된 상태.
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");

        // 걷기 애니메이션을 기본적으로 켜둔다.
        // 실제 속도는 LateUpdate에서 Speed 파라미터로 제어.
        anim.SetBool("walk", true);
    }


    private void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        // 애니메이터의 "Speed" 파라미터에 현재 입력 벡터의 크기를 전달.
        // 입력이 없으면 0 → Idle, 입력이 크면 걷기/뛰기 애니메이션으로 자연스럽게 전환.
        anim.SetFloat("Speed", inputVec.magnitude);

        // 좌우 방향 전환.
        // inputVec.x가 음수면 왼쪽을 바라보도록 flipX = true, 양수면 오른쪽.
        if (inputVec.x != 0)
        {
            sprite.flipX = (inputVec.x < 0);
        }
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        // 게임이 이미 끝난 상태라면 충돌로 인한 데미지도 받지 않는다.
        if (!GameManager.instance.isLive)
        {
            return;
        }

        // 적과 부딪혀 있는 동안 체력이 초당 10씩 감소.
        GameManager.instance.health -= Time.deltaTime * 10;

        // 체력이 0보다 작아졌다면 사망 처리.
        if (GameManager.instance.health < 0)
        {
            // 자식 오브젝트들(무기, 장비 등)을 모두 비활성화 → 플레이어 주변 장비 제거.
            for (int index = 0; index < transform.childCount; index++)
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }

            // 죽는 애니메이션 트리거 발동.
            anim.SetTrigger("Dead");

            // GameManager에 게임오버 요청.
            GameManager.instance.GameOver();
        }
    }

}

