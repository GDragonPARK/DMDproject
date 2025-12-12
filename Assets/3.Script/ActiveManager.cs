using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveManagewr : MonoBehaviour
{
    // lockCharacter : 아직 잠겨있는 캐릭터들을 보여주는 오브젝트 배열.
    // unlockCharacter : 잠금 해제된 캐릭터 UI를 보여주는 오브젝트 배열.
    // 두 배열은 인덱스 기반으로 대응하는 캐릭터를 표현한다.
    public GameObject[] lockCharacter;
    public GameObject[] unlockCharacter;

    // uiNotice : 잠금 해제 시 화면에 띄워줄 UI 패널.
    public GameObject uiNotice;

    // Achive : 어떤 업적을 달성하면 캐릭터를 언락하는지 구분하기 위한 열거형.
    // unlockPotato → 감자 캐릭터 언락 조건
    // unlockBean   → 콩 캐릭터 언락 조건
    enum Achive { unlockPotato, unlockBean }

    // achives : 위 열거형을 배열 형태로 담아 반복문에서 사용하기 위함.
    Achive[] achives;

    // wait : 알림 UI를 얼마나 띄울지 대기시간을 실시간 기준으로 담아둔 변수.
    WaitForSecondsRealtime wait;

    private void Awake()
    {
        // Enum.GetValues : Achive 안에 들어있는 모든 열거값을 배열로 꺼내오기.
        achives = (Achive[])Enum.GetValues(typeof(Achive));

        // 알림 표시 시간 5초.
        wait = new WaitForSecondsRealtime(5);

        // PlayerPrefs에 MyData가 없다면 → 게임을 처음 실행한 것으로 간주.
        if (!PlayerPrefs.HasKey("MyData"))
        {
            Init();
        }
    }

    void Init()
    {
        // MyData를 1로 저장해 “초기 설정이 완료되었음”을 기록.
        PlayerPrefs.SetInt("MyData", 1);

        // 모든 업적을 0으로 초기화. (0 = 미달성)
        foreach (Achive achive in achives)
        {
            PlayerPrefs.SetInt(achive.ToString(), 0);
        }
    }

    void UnLockCharacter()
    {
        // lockCharacter, unlockCharacter 배열의 인덱스는 각각 캐릭터를 의미.
        for (int index = 0; index < lockCharacter.Length; index++)
        {
            // enum 이름을 문자열로 변환 → PlayerPrefs에서 동일 키로 저장됨.
            string achiveName = achives[index].ToString();

            // 업적을 달성했는가? (1이면 언락)
            bool isUnlock = PlayerPrefs.GetInt(achiveName) == 1;

            // 잠겨있어야 하는애는 반대로, 언락된 애는 true로 활성화.
            lockCharacter[index].SetActive(!isUnlock);
            unlockCharacter[index].SetActive(isUnlock);
        }
    }

    private void Start()
    {
        // 게임 시작 시 캐릭터 언락 여부를 UI에 반영.
        UnLockCharacter();
    }

    private void LateUpdate()
    {
        // 매 프레임 업적들을 체크하여 달성 여부를 확인.
        foreach (Achive achive in achives)
        {
            CheckAchive(achive);
        }
    }

    void CheckAchive(Achive achive)
    {
        bool isAchive = false;  // 이번 업적이 달성되었는지를 저장.

        // 업적마다 다른 조건을 부여.
        switch (achive)
        {
            case Achive.unlockPotato:
                // 감자 언락 조건 : 총 10킬 이상 달성.
                isAchive = GameManager.instance.Kill >= 10;
                break;

            case Achive.unlockBean:
                // 콩 언락 조건 : 제한 시간까지 생존.
                isAchive = GameManager.instance.gameTime == GameManager.instance.maxGameTime;
                break;
        }

        // 이미 달성한 업적이면 무시하고, 처음 달성했을 때만 처리.
        if (isAchive && PlayerPrefs.GetInt(achive.ToString()) == 0)
        {
            // 업적 달성 기록을 PlayerPrefs에 저장.
            PlayerPrefs.SetInt(achive.ToString(), 1);

            // 업적 알림 UI에서 어떤 알림(자식 오브젝트)을 띄울지 결정.
            for (int index = 0; index < uiNotice.transform.childCount; index++)
            {
                // index와 업적 enum의 순서를 맞춰 해당 알림만 활성화.
                bool isActive = index == (int)achive;
                uiNotice.transform.GetChild(index).gameObject.SetActive(isActive);
            }

            // UI 띄우는 코루틴 실행.
            StartCoroutine(NoticeRoutine());
        }
    }

    IEnumerator NoticeRoutine()
    {
        // 알림 UI 활성화.
        uiNotice.SetActive(true);

        // 업적 달성 사운드 재생.
        AudioManager.instance.Playsfx(AudioManager.Sfx.LevelUp);

        // wait(5초) 동안 대기.
        yield return wait;

        // 알림 UI 숨김.
        uiNotice.SetActive(false);
    }
}

