using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VolumeSlider : MonoBehaviour
{

    public AudioSourceType type;
    public float value;
    public Slider slider;

    // Start is called before the first frame update

    public void Start() {
        slider.onValueChanged.AddListener(delegate { onSliderValueChange(); });

        switch (type) {
            case AudioSourceType.effects:
                slider.value = AudioManager.effectsVolume;
                break;
            case AudioSourceType.master:
                slider.value = AudioManager.masterVolume;
                break;
            case AudioSourceType.music:
                slider.value = AudioManager.musicVolume;
                break;
        }
        onSliderValueChange();

        Debug.Log(AudioManager.effectsVolume);
    }

    void onSliderValueChange() {
        value = slider.value;
        switch (type) {
            case AudioSourceType.effects:
                AudioManager.effectsVolume = value;
                AudioManager.muteEffects = value > 0 ? false : true; 
                break;
            case AudioSourceType.master:
                AudioManager.masterVolume = value;
                AudioManager.muteAll = value > 0 ? false : true;
                break;
            case AudioSourceType.music:
                AudioManager.musicVolume = value;
                AudioManager.muteMusic = value > 0 ? false : true;
                break;
        }
        AudioManager.instance.onVolumeChange();
    }

}
