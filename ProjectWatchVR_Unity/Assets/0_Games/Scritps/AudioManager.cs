using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    static AudioManager _instance;
    public static AudioManager instance
    {
        get
        {
            if (_instance == null)
                _instance = Initiate();
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    [SerializeField]
    AudioMixerGroup sfxMixerGroup;
    [SerializeField]
    AudioMixerGroup voiceMixerGroup;

    public enum AudioOutput { Sfx, Voice }

    static AudioManager Initiate()
    {
        GameObject host = new GameObject();
        host.name = "AudioManager";
        DontDestroyOnLoad(host);
        return host.AddComponent<AudioManager>();
    }

    Queue<AudioSource> sources = new Queue<AudioSource>();

    AudioSource GetAvailableSource()
    {
        AudioSource result = null;
        if (sources.Count > 0)
            result = sources.Dequeue();
        else {
            result = gameObject.AddComponent<AudioSource>();
        }
        return result;
    }

    public static void PlaySFX(AudioClip clip, AudioOutput output = AudioOutput.Sfx)
    {
        if (clip == null)
            return;
        instance.StartCoroutine(instance.SFXManager(clip, output));
    }

    public static void PlaySFX(AudioClip[] clips, AudioOutput output = AudioOutput.Sfx)
    {
        PlaySFX(clips[Random.Range(0, clips.Length)], output);
    }

    IEnumerator SFXManager(AudioClip clip, AudioOutput output = AudioOutput.Sfx)
    {
        AudioSource source = GetAvailableSource();
        source.enabled = true;
        source.clip = clip;
        source.outputAudioMixerGroup = sfxMixerGroup;

        if (output == AudioOutput.Voice)
            source.outputAudioMixerGroup = voiceMixerGroup;

        source.Play();
        while (source.isPlaying)
            yield return null;

        source.clip = null;
        source.enabled = false;
        sources.Enqueue(source);
    }
}
