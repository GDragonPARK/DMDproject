using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /*
      이 스크립트는 전체 게임의 ‘심장’이다.
      게임의 흐름 시작 / 종료 / 승리 / 패배 / 시간 진행 / 경험치 관리 / 플레이어 초기화 등
      모든 시스템이 여기서 제어된다.

      싱글턴 패턴을 사용하여 어디서든 GameManager.instance 로 접근 가능하도록 설계되어 있다.
    */

    public static GameManager instance;

    [Header("# Game Control")]
    // 게임이 진행 중인지 여부. true면 모든 스크립트가 동작하고 false면 정지한다.
    public bool isLive;

    // 게임이 경과한 시간 (초 단위)
    public float gameTime;

    // 게임 승리까지 필요한 제한 시간 (기본: 5 * 10초 = 50초)
    public float maxGameTime = 5 * 10f;


    [Header("# Player Info")]
    // 선택한 캐릭터 ID
    public int PlayerId;

    // 플레이어의 현재 체력, 최대 체력
    public float health;
    public int maxHealth = 100;

    // 현재 레벨 / 총 처치 수 / 현재 경험치량
    public int level;
    public int Kill;
    public int exp;

    // 다음 레벨까지 필요한 경험치 테이블
    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };


    [Header("# Game Object")]
    // 풀 매니저, 플레이어, UI, 결과창 등
    public PoolManager pool;
    public Player player;
    public LeveUp uiLevelUp;
    public Result uiResult;
    public GameObject enemyCleaner;


    private void Awake()
    {
        // 싱글턴 초기화
        instance = this;
    }


    public void GameStart(int id)
    {
        // 어떤 캐릭터를 플레이하는지 저장
        PlayerId = id;
        health = maxHealth;

        // 플레이어 등장
        player.gameObject.SetActive(true);

        // 레벨업 UI의 선택지 또한 캐릭터 ID에 따라 달라짐
        uiLevelUp.Select(PlayerId % 2);

        // 게임 재개
        ReSume();

        // 배경음악 및 효과음 재생
        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.Playsfx(AudioManager.Sfx.Select);
    }


    public void GameOver()
    {
        // 패배 연출 전용 코루틴으로 분리
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        // 더 이상 몬스터 이동, 탄환, 플레이어 이동 등 모든 동작 정지
        isLive = false;

        // 약간의 연출 딜레이
        yield return new WaitForSeconds(0.5f);

        // 패배 UI 띄우기
        uiResult.gameObject.SetActive(true);
        uiResult.Lose();

        // 게임 정지
        Stop();

        // 소리 재생
        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.Playsfx(AudioManager.Sfx.Lose);
    }


    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }

    IEnumerator GameVictoryRoutine()
    {
        isLive = false;

        // 모든 적 즉시 정리 (이펙트용)
        enemyCleaner.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Win();

        Stop();

        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.Playsfx(AudioManager.Sfx.Win);
    }


    public void GameRetry()
    {
        // 씬을 다시 로딩해서 게임을 리셋한다.
        SceneManager.LoadScene(0);
    }


    void Update()
    {
        // 게임 오버 중이면 시간도 멈춘다
        if (!isLive)
            return;

        // 경과 시간 측정
        gameTime += Time.deltaTime;

        // 제한 시간 넘기면 자동 승리
        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameVictory();
        }
    }


    public void GetExp()
    {
        // 게임이 이미 종료된 상태라면 경험치 무효
        if (!isLive)
            return;

        exp++;

        // 현재 레벨 인덱스에 맞는 필요 경험치 도달 시 레벨업
        if (exp == nextExp[Mathf.Min(level, nextExp.Length - 1)])
        {
            level++;
            exp = 0;

            // 레벨업 UI 열기
            uiLevelUp.Show();
        }
    }


    public void Stop()
    {
        // 게임 논리 정지 + TimeScale 0으로 프레임도 멈춤
        isLive = false;
        Time.timeScale = 0;
    }


    public void ReSume()
    {
        // 게임 재개
        isLive = true;
        Time.timeScale = 1;
    }
}

