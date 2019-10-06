using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum playEffect {
    explosion,
    hit,
    fire,
    powerUp
}

public enum AudioSourceType {
    master,
    effects,
    music
}

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class SaveAudioSettings : ISavable {
        public float masterVolume;
        public float musicVolume;
        public float effectsVolume;

        public SaveAudioSettings() {
            masterVolume = AudioManager.masterVolume;
            musicVolume = AudioManager.musicVolume;
            effectsVolume = AudioManager.effectsVolume;
        }

        public void empty() {
            //nothing
        }

        public void load() {
            AudioManager.effectsVolume = effectsVolume;
            AudioManager.masterVolume = masterVolume;
            AudioManager.musicVolume = musicVolume;
            AudioManager.muteAll = !(masterVolume > 0);
            AudioManager.muteEffects = !(effectsVolume > 0);
            AudioManager.muteMusic = !(effectsVolume > 0);
        }
    }

    public const string audioSettingsFile = "audios.gsd";

    public static void saveAudioSettings() {
        SaveSystem<SaveAudioSettings>.saveSettings(new SaveAudioSettings(), audioSettingsFile);
    }

    public static void loadAudioSettings() {
        SaveSystem<SaveAudioSettings>.loadSettings(audioSettingsFile);
    }




    public const int skipCount = 3;

    public static AudioManager instance;

    public static float masterVolume = 0.75f;
    public static float musicVolume = 0.75f;
    public static float effectsVolume = 0.75f;
    public static bool muteEffects = false;
    public static bool muteMusic = false;
    public static bool muteAll = false;

    public Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    public AudioClip select;

    [SerializeField] private AudioSource[] mediumEffectsSources;
    [SerializeField] private AudioSource[] shortEffectsSources;
    //public AudioSource main;

    [SerializeField] private AudioClip[] backgroundMusic;
    [SerializeField] private AudioClip[] explosionClips;
    [SerializeField] private AudioClip[] hitClips;
    [SerializeField] private AudioClip[] fireClips;
    [SerializeField] private AudioClip[] powerUpClips;

    public AudioClip bossIsCommingClip;
    private DoubleAudioSource dAudio;

    public Transform audioEffectsHolder;
    public Transform shortAudioSourcesHolder;
    public AudioSource longEffectAudio;

    List<int> usedIds = new List<int>();

    int shortIndex = 0;
    int longIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        dAudio = GetComponent<DoubleAudioSource>();
        //AudioClip[] all_clips = Resources.LoadAll<AudioClip>("Effects");
        //foreach (AudioClip clip in all_clips) {
        //    audioClips.Add(clip.name, clip);
        //}
        instance = this;

        if(mediumEffectsSources.Length == 0) {
            mediumEffectsSources = audioEffectsHolder.GetComponents<AudioSource>();
        }
        if(shortEffectsSources.Length == 0) {
            shortEffectsSources = shortAudioSourcesHolder.GetComponents<AudioSource>();
        }
        StartCoroutine(PlayMusic());

        loadAudioSettings();
        onVolumeChange();
    }

    public void onVolumeChange() {
        dAudio.upgdateVolume();
    }

    public static void playOther(playEffect type, float volume) {
        instance.longEffectAudio.PlayOneShot(instance.getRandomClip(type), volume * masterVolume * effectsVolume);
    }

    public static void playMediumEffect(float volume) {
        //instance.playShortClip(instance.getRandomClip(playEffect.explosion), volume * 0.5f);
        instance.playMediumEffect(instance.getRandomClip(playEffect.hit), volume);
    }

    public static void playOnce(AudioClip clip, float volume) {
        instance.playShortClip(clip, volume);
    }

    public static void playRepeatedEffect(AudioClip clip, float delay, int repeats, float volume) {
        instance.StartCoroutine(instance.playMultipleTimes(clip, delay, repeats, volume));
    }

    int reset = 0;
    int vr = 0;
    public void playMediumEffect(AudioClip clip, float volume) {
        if (muteEffects || muteAll)
            return;
        //mediumEffectsSources[longIndex].clip = clip;
        //mediumEffectsSources[longIndex].Play();
        if (reset < 3) {
            vr++;
            if (vr > 2) {
                longIndex++;
                if (longIndex > mediumEffectsSources.Length - 1)
                    longIndex = 0;
                vr = 0;
                mediumEffectsSources[longIndex].Stop();
            }

            mediumEffectsSources[longIndex].PlayOneShot(clip, volume * masterVolume * effectsVolume);
            StartCoroutine(resetAudio(0.15f));
        }
    }

    IEnumerator resetAudio(float w8) {
        reset++;
        yield return new WaitForSeconds(w8);
        reset--;
    }

    public void playShortClip(AudioClip clip, float volume) {
        if (muteEffects || muteAll)
            return;
        shortEffectsSources[shortIndex].PlayOneShot(clip, volume * masterVolume * effectsVolume);

        shortIndex++;
        if (shortIndex > shortEffectsSources.Length - 1)
            shortIndex = 0;
    }

    IEnumerator playMultipleTimes(AudioClip clip, float delay, int times, float volume) {
        for (int i = 0; i < times; i++) {
            //playClip(clip, volume);
            longEffectAudio.PlayOneShot(clip, volume * masterVolume * effectsVolume);
            yield return new WaitForSeconds(delay);
        }
    }

    public AudioClip getRandomClip(playEffect type) {
        AudioClip clip = null;
        try {
            switch (type) {
                case playEffect.explosion:
                    clip = explosionClips[Random.Range(0, explosionClips.Length)];
                    break;
                case playEffect.fire:
                    clip = fireClips[Random.Range(0, fireClips.Length)];
                    break;
                case playEffect.hit:
                    clip = hitClips[Random.Range(0, hitClips.Length)];
                    break;
                case playEffect.powerUp:
                    clip = powerUpClips[Random.Range(0, powerUpClips.Length)];
                    break;
            }
        }
        catch {
            Debug.LogError("Missing " + type.ToString() + " sound clips!");
        }

        return clip;
    }

    IEnumerator PlayMusic() {
        dAudio.CrossFade(backgroundMusic[getNotUsedID()], 1, 0);
        yield return new WaitForSecondsRealtime(5);
        float w8time = 0;
        while (true) {
            if (!muteAll && !muteMusic) {
                int id = getNotUsedID();
                AudioClip clip = backgroundMusic[id];
                w8time = clip.length;
                if (w8time < 25f) {
                    w8time *= Random.value * 2 + 2f;
                    w8time = Mathf.Clamp(w8time, 18f, 26f);
                    dAudio.CrossFade(clip, 1, w8time * 0.75f);
                }
                yield return new WaitForSecondsRealtime(w8time);
            } else {
                yield return new WaitForSecondsRealtime(2f);
            }
            
        }
    }

    public int getNotUsedID() {
        int id = Random.Range(0, backgroundMusic.Length);
        for (int i = 0; i < usedIds.Count; i++) {
            if (id == usedIds[i])
                return getNotUsedID();
        }
        usedIds.Add(id);
        if (usedIds.Count > skipCount)
            usedIds.RemoveAt(0);
        return id;
    }

    public void playAnyThatStartWith(string starts) {
        AudioClip clip = null;
        float set = float.MinValue;
        foreach (string name in audioClips.Keys) {
            if (name.StartsWith(starts)) {
                float ns = Random.Range(0f, 100f);
                if(ns > set) {
                    set = ns;
                    clip = audioClips[name];
                }
            }
        }

        if(clip) {
            mediumEffectsSources[0].PlayOneShot(clip, 1);
        }
    }

}
