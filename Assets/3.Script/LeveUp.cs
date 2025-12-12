//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeveUp : MonoBehaviour
{
    /*
      이 스크립트는 “레벨업 선택 UI”의 전체 흐름을 담당한다.

      플레이어가 레벨업하면,
      1) 일시정지
      2) 레벨업 창 표시
      3) 3개의 무작위 아이템 옵션 제공
      4) 클릭 시 무기/장비/회복을 선택

      이 일련의 과정이 모두 여기서 관리된다.
    */

    // UI 패널의 RectTransform
    RectTransform rect;

    // 레벨업 창에 들어있는 모든 아이템 슬롯들
    [SerializeField] Item[] items;


    private void Awake()
    {
        // 레벨업 패널의 RectTransform 초기화
        rect = GetComponent<RectTransform>();

        // 자식에 붙어 있는 모든 Item 스크립트 가져오기
        // true → 비활성화 된 오브젝트도 포함
        items = GetComponentsInChildren<Item>(true);
    }


    public void Show()
    {
        // 1) 다음에 보여줄 아이템 3개 뽑기
        Next();

        // 2) UI 패널 열기 (localScale로 On/Off)
        rect.localScale = Vector3.one;

        // 3) 게임 일시정지
        GameManager.instance.Stop();

        // 4) 효과음 및 필터 적용
        AudioManager.instance.Playsfx(AudioManager.Sfx.LevelUp);
        AudioManager.instance.EffectBgm(true);
    }


    public void Hide()
    {
        // UI 패널 닫기
        rect.localScale = Vector3.zero;

        // 게임 재개
        GameManager.instance.ReSume();

        // 효과음
        AudioManager.instance.Playsfx(AudioManager.Sfx.Select);
        AudioManager.instance.EffectBgm(false);
    }


    public void Select(int index)
    {
        // index에 해당하는 아이템을 선택(클릭) 처리
        items[index].OnClick();
    }


    void Next()
    {
        // ---------------------------------------------------
        // 1. 모든 아이템 비활성화하여 '초기화 상태'로 둔다
        // ---------------------------------------------------
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }


        // ---------------------------------------------------
        // 2. 서로 겹치지 않는 랜덤 3개 인덱스 생성
        // ---------------------------------------------------
        int[] ran = new int[3];

        // 세 값이 모두 서로 다를 때까지 계속 난수 생성
        while (true)
        {
            ran[0] = Random.Range(0, items.Length);
            ran[1] = Random.Range(0, items.Length);
            ran[2] = Random.Range(0, items.Length);

            if (ran[0] != ran[1] &&
                ran[1] != ran[2] &&
                ran[0] != ran[2])
                break;
        }


        // ---------------------------------------------------
        // 3. 선택된 3개 아이템을 UI에 표시
        // ---------------------------------------------------
        for (int index = 0; index < ran.Length; index++)
        {
            Item ranItem = items[ran[index]];

            // 만렙 아이템 = 더 이상 레벨업 불가 → 힐 아이템(예: items[4])로 대체
            if (ranItem.level == ranItem.data.damages.Length)
            {
                items[4].gameObject.SetActive(true);
            }
            else
            {
                // 정상적으로 UI 표시
                ranItem.gameObject.SetActive(true);
            }
        }
    }
}

