using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    /*
      이 스크립트는 화면에 표시되는 각종 정보를 “자동으로 갱신”하는 UI 시스템이다.
      HUD는 플레이어의 상태값(경험치, 레벨, 처치 수, 남은 시간, 체력)을
      GameManager에서 읽어와 즉시 반영한다.

      InfoType을 통해 이 오브젝트가 어떤 정보를 담당하는지 구분하고,
      LateUpdate에서 매 프레임 UI를 갱신한다.
    */

    // 어떤 정보를 표시할지 선택한다.
    public enum InfoType { Exp, Level, Kill, Time, Health }
    public InfoType type;

    // 텍스트 UI 또는 슬라이더 UI (경험치/체력바)
    Text myText;
    Slider mySlider;


    private void Awake()
    {
        // 이 HUD가 어떤 UI 컴포넌트를 갖고 있는지 초기화한다.
        // Exp/Health는 Slider, 나머지는 Text를 주로 사용한다.
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }


    private void LateUpdate()
    {
        // LateUpdate를 사용하는 이유:
        //  다른 스크립트의 Update에서 수치가 변경된 뒤,
        //  가장 마지막에 UI를 갱신하여 화면에 정확한 정보를 보여주기 위함이다.

        switch (type)
        {
            // ------------------------------
            // 1) 경험치 표시 (슬라이더)
            // ------------------------------
            case InfoType.Exp:
                float curExp = GameManager.instance.exp;
                float maxExp =
                    GameManager.instance.nextExp[
                        Mathf.Min(
                            GameManager.instance.level,
                            GameManager.instance.nextExp.Length - 1
                        )
                    ];

                // 경험치바 = 현재Exp / 필요Exp
                mySlider.value = curExp / maxExp;
                break;


            // ------------------------------
            // 2) 레벨 표시 (텍스트)
            // ------------------------------
            case InfoType.Level:
                myText.text = string.Format("Lv.{0:F0}", GameManager.instance.level);
                break;


            // ------------------------------
            // 3) 처치 수 (Kill count)
            // ------------------------------
            case InfoType.Kill:
                myText.text = string.Format("{0:F0}", GameManager.instance.Kill);
                break;


            // ------------------------------
            // 4) 남은 시간 표시 (mm:ss)
            // ------------------------------
            case InfoType.Time:
                float remainTime =
                    GameManager.instance.maxGameTime - GameManager.instance.gameTime;

                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);

                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;


            // ------------------------------
            // 5) 체력 표시 (슬라이더)
            // ------------------------------
            case InfoType.Health:
                float curHealth = GameManager.instance.health;
                float maxHealth = GameManager.instance.maxHealth;

                mySlider.value = curHealth / maxHealth;
                break;
        }
    }
}

