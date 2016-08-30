
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SoundControllScript : MonoBehaviour
{
    private GameObject BGM;
    private GameObject EffectSound;

    public Slider BGMSlider;
    public Slider EffectSoundSlider;

    private bool bgmMute;
    private bool EffectSoundMute;
    private float BGMOffset;
    private float EffectSoundOffset;

	// Use this for initialization
	void Start () {
       

	    BGM = this.transform.FindChild("BGM").gameObject;
	    EffectSound = this.transform.FindChild("EffectSound").gameObject;
        BGM.GetComponent<AudioSource>().volume = BGMSlider.value;
	    //EffectSound.GetComponent<AudioSource>().volume = EffectSoundSlider.value; 현재 이펙트 사운드는 안되므로 나중에 추가되면 전체적으로 묶어서 처리해버리기

	}
	
    //bgm 슬라이더 함수
    public void BGMSliderInvoke()
    {
        BGM.GetComponent<AudioSource>().volume = BGMSlider.value;
    }

    //SoundEffect 슬라이더 함수
    public void EffectSoundSliderInvoke()//
    {
        Debug.Log(EffectSoundSlider.value);//전체 사운드 설정
    }


    //bgm 뮤트버튼 함수
    public void BGMMuteButtonInvoke()
    {

        if (bgmMute == false)
        {
            BGMOffset = BGM.GetComponent<AudioSource>().volume;
            BGM.GetComponent<AudioSource>().volume = 0;
            BGMSlider.value = 0;
            BGMSlider.interactable = false;
            bgmMute = true;
        }
        else
        {
            BGM.GetComponent<AudioSource>().volume = BGMOffset;
            BGMSlider.value = BGMOffset;
            BGMSlider.interactable = true;
            bgmMute = false;
        }
        
    }
    //effectsound 뮤트버튼 함수
    public void EffectSoundButtonInvoke()
    {
        if (EffectSoundMute == false)
        {
            Debug.Log("effectsound 뮤트온");

            EffectSoundMute = true;
        }
        else
        {
            Debug.Log("effectsound 뮤트 오프");

            EffectSoundMute = false;
        }
    }
}
