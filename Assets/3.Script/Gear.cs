using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    /*
      이 스크립트는 “장비 효과 시스템”이다.
      장비는 플레이어의 몸에 부착되며,
      플레이어의 무기 속도, 이동 속도 등 핵심 능력치를 강화하는 역할을 한다.

      즉, ItemData(아이템 DB)로부터 장비 정보를 받아오고,
      그 장비가 적용되어야 할 능력치를 즉시 계산한 뒤,
      Weapon 또는 Player에게 반영해주는 구조다.
    */

    // 장비 종류 (장갑 / 신발)
    public ItemData.ItemType type;

    // 강화율. 예: 0.2f면 20% 강화
    public float rate;


    public void Init(ItemData data)
    {
        // --- 기본 세팅 ---
        name = "Gear" + data.itemId;  // 장비 이름 설정
        transform.parent = GameManager.instance.player.transform; // 플레이어 몸에 장착
        transform.localPosition = Vector3.zero;                   // 정확히 플레이어 위치에 붙인다.

        // --- 능력치 세팅 ---
        type = data.itemType;     // 장비 종류 (장갑 or 신발)
        rate = data.damages[0];   // 장비의 강화율 (ItemData에서 첫 번째 값)

        // 장비 적용
        ApplyGear();
    }


    public void LevelUp(float rate)
    {
        // 장비가 레벨업하면 강화율(rate)이 갱신된다.
        this.rate = rate;
        ApplyGear();
    }


    void ApplyGear()
    {
        // 장비 종류에 따라 어떤 능력치를 강화할지 결정한다.
        switch (type)
        {
            case ItemData.ItemType.Glove:
                // 장갑 → 무기 속도 증가
                RateUp();
                break;

            case ItemData.ItemType.Shoe:
                // 신발 → 이동 속도 증가
                SpeedUp();
                break;
        }
    }


    void RateUp()
    {
        // 플레이어가 들고 있는 모든 무기를 찾는다.
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();

        foreach (Weapon weapon in weapons)
        {
            // 무기 ID에 따라 근접/원거리 공식을 다르게 적용한다.
            switch (weapon.id)
            {
                // 0번 무기 = 회전형 근접무기(삽)
                case 0:
                    float speed = 150 * Character.WeaponSpeed; // 기본 회전 속도
                    weapon.speed = speed + (speed * rate);      // 속도 증가 적용
                    break;

                // 나머지 = 원거리 무기 (발사 속도는 빠를수록 좋으므로 계산 방식 다름)
                default:
                    speed = 0.5f * Character.WeaponRate;       // 발사 간격
                    weapon.speed = speed * (1f - rate);        // rate만큼 발사 간격 감소 (=실질적 속도 증가)
                    break;
            }
        }
    }


    void SpeedUp()
    {
        // 기본 이동속도는 Character.Speed 로 계산된다.
        float speed = 3 * Character.Speed;

        // 플레이어 이동속도에 (기본속도 * rate)의 성능을 추가
        GameManager.instance.player.Speed = speed + speed * rate;
    }

}

