using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    /*
      이 스크립트는 “아이템의 원본 데이터(설계도)”를 ScriptableObject로 관리한다.

      게임에서 생성되는 모든 무기·장비·회복 아이템은
      이 ItemData를 기반으로 능력치가 계산된다.

      ScriptableObject를 활용함으로써,
      아이템 데이터가 ‘씬에 종속되지 않고’ 독립적으로 관리되어
      메모리 효율과 유지보수가 모두 좋아진다.
    */

    // 아이템 종류 : 근접, 원거리, 장갑, 신발, 힐
    public enum ItemType { Melee, Range, Glove, Shoe, Heal }

    [Header("# Main Info")]
    // 아이템의 종류 (장비인지 무기인지)
    public ItemType itemType;

    // 고유 ID. Weapon.cs와 Gear.cs에서 필요한 연결 고리 역할
    public int itemId;

    // UI 표시용 이름
    public string itemName;

    // UI 상 설명. TextArea는 여러 줄 입력 가능하도록 한다.
    [TextArea]
    public string itemDesc;

    // UI에 표시될 아이콘 이미지
    public Sprite itemIcon;


    [Header("# Level Data")]
    /*
       아래는 아이템 레벨업에 따른 능력치 변화를 저장하는 데이터들이다.

       각 무기는 레벨업하면 damage와 count 등 속성이 달라지므로
       damages[], counts[] 배열을 활용해 “레벨별 능력치 테이블”을 만든다.
    */

    // 무기의 기본 데미지
    public float baseDamage;

    // 기본 개수 (회전 무기라면 기본 1개)
    public int baseCount;

    // 레벨업 시 증가하는 데미지 표
    public float[] damages;

    // 레벨업 시 증가하는 무기 개수 표
    public int[] counts;


    [Header("# Weapon")]
    // 이 아이템이 무기라면, 무기의 발사체(prefab)를 명시한다.
    // 근접무기라면 회전하는 삽,
    // 원거리무기라면 총알 또는 정령(프로젝트타일) 등이 들어온다.
    public GameObject projectile;
}

