//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Reposition : MonoBehaviour
{
    // 재배치 로직: 트리거가 체크된 콜라이더에서 나갔을 때 발생해야한다.
    // area와의 충돌에서 벗어났을 때 맵이 이동해줘야한다.
    // 타일맵과 플레이어의 거리를 봐야한다 (거리가 벌어진게 x축인지 y축인지)
    //타일맵과 플레이어 위치 변수필요.
    // 거리가 벌어진건 player위치에서 - 내위치 빼면 나와의 거리차이를 알려줌 음수 방지 mathf
    //플레이어 이동방향을 저장하기 위한 변수 추가 
    // 트랜스폼 태그 걸고 내자신의 태그가 어떤것인지 체크해라
    Collider2D coll;
    void Awake()
    {
        coll = GetComponent<Collider2D>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area"))
            return;

        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 myPos = transform.position;

        float diffX = Mathf.Abs(playerPos.x - myPos.x);
        float diffY = Mathf.Abs(playerPos.y - myPos.y);

        Vector3 playerDir = GameManager.instance.player.inputVec;
        float dirX = playerDir.x < 0 ? -1 : 1;
        float dirY = playerDir.y < 0 ? -1 : 1;

        switch (transform.tag)
        {
            case "Ground":
                if (diffX>diffY)
                {
                    transform.Translate(Vector3.right * dirX * 44);
                }
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * 44);
                }
                break;
            case "Enemy":
                if (coll.enabled)
                {
                    transform.Translate(playerDir * 20 + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f));
                }
                break;
        }
    }
}
