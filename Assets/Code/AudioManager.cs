using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoSingleton<AudioManager>
{
    public AudioSource music_audioSrc;
    [Header("Global sounds:")]
    public AudioClip hurt_1_sfx;

    [Header("Mixers:")]
    public AudioMixerGroup master_mixer;
    public AudioMixerGroup music_mixer;
    public AudioMixerGroup sfx_mixer;

    void Start()
    {
        //ReadPrefs();
        //battle_music_audioSrc.volume = 0;
        FadeVolumeTo(music_audioSrc, 1, 0.25f);
        //calm_music_audioSrc.volume = 1;
    }

    public GameObject audio_3d_common_prefab;
    public GameObject audio_3d_bigRange_prefab;

    public static void PlayAt_Common(Vector3 pos, AudioClip clip, float pitch = 1, float volume = 1)
    {
        AudioSource _audio = Instantiate(Instance.audio_3d_common_prefab, pos, Quaternion.identity).GetComponent<AudioSource>();
        _audio.volume = volume;
        _audio.pitch = pitch;
        _audio.PlayOneShot(clip);
        Destroy(_audio.gameObject, clip.length * 2);
    }

    public static void PlayAt_BigRange(Vector3 pos, AudioClip clip, float pitch = 1, float volume = 1)
    {
        AudioSource _audio = Instantiate(Instance.audio_3d_bigRange_prefab, pos, Quaternion.identity).GetComponent<AudioSource>();
        _audio.volume = volume;
        _audio.pitch = pitch;
        _audio.PlayOneShot(clip);
        Destroy(_audio.gameObject, clip.length * 2);
    }

    public static void PlayHurtAt(Vector3 pos, float pitch = 1, float volume = 1)
    {
        PlayAt_Common(pos, Instance.hurt_1_sfx, pitch, volume);
    }

    static float small_impact_timeStamp;
    

    public static void FadeVolumeTo(AudioSource _audio_src, float _target_volume, float speed)
    {
        if(!_audio_src)
            return;

        Instance.StartCoroutine(Instance._FadeVolumeTo(_audio_src, _target_volume, speed));
    }

    IEnumerator _FadeVolumeTo(AudioSource a, float _target_volume, float speed)
    {
        while(a && a.volume != _target_volume)
        {
            a.volume = Mathf.MoveTowards(a.volume, _target_volume, speed * Time.deltaTime);
            yield return null;
        }
    }


    public static float VolumeToDB(float v)
    {
        v = Mathf.Clamp(v, 0.0001f, 1f);
        return Mathf.Log10(v) * 20f;
    }

    public static void PlayUIClick()
    {
    }

    public static void PlayUIClick2()
    {
    }

    public static void SetMasterVolume(float vol)
    {
        // Debug.Log("Trying to set MasterVolume: " + vol);
        Instance.master_mixer.audioMixer.SetFloat("MasterVolume", VolumeToDB(vol));
        // float k;
        // Singleton().master_mixer.audioMixer.GetFloat("MasterVolume", out k);
        // Debug.Log("MasterVolume after: " + k);
    }

    public static void SetMusicVolume(float vol)
    {
        Instance.music_mixer.audioMixer.SetFloat("MusicVolume", VolumeToDB(vol));
    }
}

