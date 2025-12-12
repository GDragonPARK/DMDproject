//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // instance : 오디오 매니저의 싱글턴 인스턴스.
    //            게임 전체에서 하나만 존재해야 하며,
    //            BGM/SFX 재생을 어디서든 접근 가능하게 하기 위해 static으로 선언.
    public static AudioManager instance;

    [Header("#BGM")]
    // bgmclip : 배경음악으로 사용할 오디오 클립.
    // bgmVolume : 배경음의 기본 볼륨값.
    public AudioClip bgmclip;
    public float bgmVolume;

    // bgmPlayer : 실제로 BGM을 재생해주는 AudioSource.
    // bgmEffect : BGM에 하이패스 필터를 적용해 사운드 변조 효과를 줄 때 사용.
    AudioSource bgmPlayer;
    AudioHighPassFilter bgmEffect;

    [Header("#SFX")]
    // sfxclips : 효과음들을 담아놓은 배열. enum Sfx의 순서 기반으로 접근한다.
    public AudioClip[] sfxclips;
    public float sfxVolume;

    // channels : SFX를 동시에 여러 개 재생하기 위해 필요한 채널 개수.
    // sfxPlayers : 각 채널마다 할당된 AudioSource 배열.
    public int channels;
    AudioSource[] sfxPlayers;

    // channelIndex : 다음 재생할 채널 번호를 순환시키기 위한 인덱스.
    int channelIndex;

    // Sfx : 효과음 종류를 정리해놓은 열거형.
    // Dead = 0, Hit = 1, LevelUp = 3, Range = 7 ... 이런 식으로
    // 효과음 종류를 직관적 이름으로 사용하기 위해 만든 enum.
    public enum Sfx { Dead, Hit, LevelUp = 3, Lose, Melee, Range = 7, Select, Win }

    private void Awake()
    {
        // 싱글턴 초기화. 오디오 매니저를 다른 스크립트에서 쉽게 접근할 수 있게 만든다.
        instance = this;
        Init();
    }

    void Init()
    {
        // -------------------------------
        // 1) 배경음(BGM) 플레이어 초기화
        // -------------------------------
        // 배경음을 재생할 전용 오브젝트 생성.
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;

        // 오디오 소스 추가 + 설정
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;    // 시작하자마자 재생하지 않는다.
        bgmPlayer.loop = true;            // 배경음은 반복 재생.
        bgmPlayer.volume = bgmVolume;     // Inspector에서 설정한 볼륨값.
        bgmPlayer.clip = bgmclip;         // 사용할 배경음.

        // bgmEffect : BGM을 변조할 하이패스 필터. 화면 연출용.
        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

        // -------------------------------
        // 2) 효과음(SFX) 플레이어 초기화
        // -------------------------------
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;

        // 재생 채널 수만큼 AudioSource를 생성하여 배열에 넣어둔다.
        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;   // 자동재생 방지
            sfxPlayers[index].bypassEffects = true;  // 효과음은 필터 영향 받지 않도록 설정
            sfxPlayers[index].volume = sfxVolume;    // 기본 볼륨 설정
        }
    }

    public void PlayBgm(bool isPlay)
    {
        // isPlay가 true면 BGM 재생, false면 정지.
        if (isPlay)
        {
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }

    public void EffectBgm(bool isPlay)
    {
        // bgmEffect의 활성/비활성만 담당.
        // true면 필터 적용, false면 필터 제거.
        bgmEffect.enabled = isPlay;
    }

    public void Playsfx(Sfx sfx)
    {
        // 효과음을 재생할 가장 적절한 채널을 찾기 위한 반복문.
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            // loopIndex : channelIndex부터 차례대로 검사하며
            //             재생 중이 아닌 AudioSource를 찾기 위한 회전 인덱스.
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            // 해당 채널이 이미 재생 중이면 skip.
            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            // 특정 효과음은 다수의 랜덤한 변형이 있기 때문에
            // Hit, Melee는 0~1 중에서 랜덤 인덱스를 가져오도록 함.
            int ranIndex = 0;
            if (sfx == Sfx.Hit || sfx == Sfx.Melee)
            {
                ranIndex = Random.Range(0, 2);
            }

            // 다음 채널 순회를 위해 channelIndex를 갱신.
            channelIndex = loopIndex;

            // 선택된 채널에 클립을 넣고 재생.
            sfxPlayers[loopIndex].clip = sfxclips[(int)sfx];
            sfxPlayers[loopIndex].Play();

            // 한 번 재생하면 반복문 종료.
            break;
        }
    }
}

