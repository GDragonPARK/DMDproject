using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    // UI 오브젝트(HP바, 경험치바 등)를 화면에서 플레이어 위치에 따라
    // 자동으로 따라가게 만들기 위한 스크립트이다.
    // RectTransform을 사용하는 이유는, 이 스크립트가 '캔버스(UI)' 안에서
    // 동작하기 때문이며 UI의 위치는 Transform이 아니라 RectTransform이 관리한다.

    RectTransform rect;

    void Awake()
    {
        // Follow 스크립트가 붙어 있는 UI 오브젝트의 RectTransform을 가져온다.
        // 이 초기화가 없다면 UI의 위치를 갱신할 수 없으므로 필수 작업이다.
        rect = GetComponent<RectTransform>();
    }


    void FixedUpdate()
    {
        // UI를 화면에서 플레이어 캐릭터가 서 있는 지점으로 이동시키기 위한 핵심 로직.
        // '월드 좌표(플레이어의 실제 2D 위치)'를
        // '스크린 좌표(모니터 픽셀 위치)'로 변환해야
        // UI가 정확히 따라붙을 수 있다.
        //
        // Camera.main.WorldToScreenPoint()
        // → 월드 공간의 좌표를 화면 상의 좌표로 변환하는 함수.
        //
        // rect.position
        // → UI의 화면 위치(스크린 좌표)를 직접 바꿀 수 있다.

        rect.position =
            Camera.main.WorldToScreenPoint(
                GameManager.instance.player.transform.position
            );
    }
}

