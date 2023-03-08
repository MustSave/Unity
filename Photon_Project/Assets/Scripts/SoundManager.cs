using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private Dictionary<string, AudioClip> strToClip = new Dictionary<string, AudioClip>();
    private Dictionary<float, AudioSource> volumeDict = new Dictionary<float, AudioSource>();
    public AudioClip[] clips;
    public AudioClip BGM;
    private AudioSource audioSource;

    private void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        volumeDict.Add(audioSource.volume, audioSource);

        if (BGM != null)
        {
            AudioSource bgm_AudioSource = CreateNewAudioSource("BGM", 0.05f, false);
            bgm_AudioSource.loop = true;
            bgm_AudioSource.clip = BGM;
            bgm_AudioSource.Play();
        }

        GenerateDictionary();
    }

    private void GenerateDictionary()
    {
        foreach (var clip in clips)
        {
            if (strToClip.ContainsKey(clip.name) == false)
            {
                strToClip.Add(clip.name, clip);
            }
        }
    }

    public void PlayOneShot(string clipName, float volume=0.5f)
    {
        if (strToClip.ContainsKey(clipName) == false)
        {
            print("AudioClip Not Found");
            return;
        }

        if (volumeDict.ContainsKey(volume) == false)
        {
            CreateNewAudioSource("Volume_" + volume.ToString(), volume);
        }

        volumeDict[volume].PlayOneShot(strToClip[clipName]);
    }

    private AudioSource CreateNewAudioSource(string name, float volume, bool addIntoDictionary=true)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = transform;
        AudioSource source = go.AddComponent<AudioSource>();
        source.volume = volume;

        if (addIntoDictionary) volumeDict.Add(volume, source);

        return source;
    }
}
