using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    /*
      이 스크립트는 “레벨업 UI에서 한 칸씩 등장하는 아이템 선택 슬롯”을 담당한다.
      즉, 플레이어가 레벨업했을 때,
      어떤 무기 혹은 장비를 선택할지 보여주고,
      클릭 시 무기/장비를 생성하거나 강화하는 역할을 맡는다.
    */

    // 이 슬롯이 참조해야 할 아이템 데이터 (ScriptableObject)
    public ItemData data;

    // 현재 레벨 — 슬롯이 몇 레벨까지 강화되었는지 기록
    public int level;

    // 무기 또는 장비를 보관하기 위한 참조
    public Weapon weapon;
    public Gear gear;

    // UI 컴포넌트
    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;


    private void Awake()
    {
        // 이 Item 오브젝트 내부의 Image들을 가져온다.
        // [0]은 배경, [1]이 아이콘 이미지이기 때문에 [1]을 사용.
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        // Text는 순서대로 Level, Name, Desc가 온다.
        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2];

        // 이름은 데이터에서 읽어온다.
        textName.text = data.itemName;
    }


    private void OnEnable()
    {
        // 레벨 표시 (UI 갱신)
        textLevel.text = "Lv." + (level + 1);

        // 아이템 종류에 따라 설명 UI에 표시되는 포맷이 다르다.
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                // 근접/원거리 무기: 데미지 %, 카운트 표시
                textDesc.text = string.Format(
                    data.itemDesc,
                    data.damages[level] * 100,
                    data.counts[level]
                );
                break;

            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                // 장비: 오직 강화율만 사용
                textDesc.text = string.Format(
                    data.itemDesc,
                    data.damages[level] * 100
                );
                break;

            default:
                // 힐 등 기타 아이템은 단순한 설명
                textDesc.text = string.Format(data.itemDesc);
                break;
        }
    }


    public void OnClick()
    {
        // 플레이어가 이 아이템을 실제로 선택할 때 실행되는 로직이다.
        switch (data.itemType)
        {
            // --------------------------
            // 무기 (근접/원거리)
            // --------------------------
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:

                if (level == 0)
                {
                    // 첫 선택: 새로운 무기 생성
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data); // 능력치 초기화
                }
                else
                {
                    // 레벨업: 무기 데미지/카운트 증가
                    float nextDamage = data.baseDamage;
                    int nextCount = 0;

                    // 기본 데미지에 레벨 보정치를 추가
                    nextDamage += data.baseDamage * data.damages[level];
                    nextCount += data.counts[level];

                    weapon.LevelUp(nextDamage, nextCount);
                }

                level++;
                break;


            // --------------------------
            // 장비 (장갑/신발)
            // --------------------------
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:

                if (level == 0)
                {
                    // 첫 선택: 새로운 장비 생성
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else
                {
                    // 레벨업: 강화율 갱신
                    float nextRate = data.damages[level];
                    gear.LevelUp(nextRate);
                }

                level++;
                break;


            // --------------------------
            // 힐 아이템: 체력 즉시 회복
            // --------------------------
            case ItemData.ItemType.Heal:
                GameManager.instance.health =
                    GameManager.instance.maxHealth;
                break;
        }


        // 만약 더 이상 레벨업 불가(배열 끝)라면 버튼 비활성화
        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }
}

