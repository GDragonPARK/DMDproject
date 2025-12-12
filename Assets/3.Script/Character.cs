using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    /*
     이 스크립트는 “플레이어의 고유 능력치 보정값”을 담당한다.
     즉, PlayerId에 따라 속도, 데미지, 무기 개수 등의 특성이 자동으로 달라지는 시스템이다.

     여러 스탯이 개별적으로 분리된 것이 아니라, “정적 프로퍼티(Static Property)”로 관리되며,
     GameManager.instance.PlayerId 값만 바꾸면 
     게임 전체의 밸런스가 즉시 반영되는 구조다.

     모든 값은 삼항연산자를 통해 
     특정 캐릭터일 때만 보너스를 적용하도록 설계되어 있다.
    */

    // Speed : 이동속도 보정값.
    // PlayerId가 0번 캐릭터라면 속도가 +10% 증가한다.
    // 그 외 캐릭터는 1배(기본) 유지.
    public static float Speed
    {
        get { return GameManager.instance.PlayerId == 0 ? 1.1f : 1f; }
    }

    // WeaponSpeed : 회전형 무기의 회전속도 또는 무기 에니메이션 속도에 영향을 준다.
    // PlayerId == 1 (2번 캐릭터)이면 무기 속도가 증가한다.
    public static float WeaponSpeed
    {
        get { return GameManager.instance.PlayerId == 1 ? 1.1f : 1f; }
    }

    // WeaponRate : 원거리 무기의 발사 간격 보정.
    // 값이 작을수록 더 빠르게 발사되므로 PlayerId==1은 발사 속도가 증가한 셈이다.
    public static float WeaponRate
    {
        get { return GameManager.instance.PlayerId == 1 ? 0.9f : 1f; }
    }

    // WeaponDamage : 무기의 데미지 보정.
    // PlayerId==2이면 데미지가 20% 증가한다.
    public static float WeaponDamage
    {
        get { return GameManager.instance.PlayerId == 2 ? 1.2f : 1f; }
    }

    // Count : 근접무기의 무기 개수(오브젝트 개수)
    // PlayerId==3 캐릭터만 기본 무기 개수 +1 개의 혜택을 받는다.
    public static int Count
    {
        get { return GameManager.instance.PlayerId == 3 ? 1 : 0; }
    }
}

