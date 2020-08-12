using System.Collections;
using System.Collections.Generic;
using Packages.EZRollback.Runtime.Scripts.Utils;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    List<AudioSource> emitters = new List<AudioSource>();

    public enum MusicList
    {
        NONE
    }
    MusicList currentMusicPlaying = MusicList.NONE;

    public enum SoundList
    {
        TING
    }

    [Header("VolumeSounds")]
    [SerializeField] AudioMixer audioMixer;
    
    [Header("HitClips")]
    [SerializeField] AudioClip tingClip;

    [Header("Emmiters")]
    [SerializeField] GameObject emitterPrefab;
    [SerializeField] int emitterNumber;

    [SerializeField] AudioSource musicEmitter;

    // Use this for initialization
    void Start ()
    {
        DontDestroyOnLoad(gameObject);

        for(int i = 0; i <= emitterNumber;i++)
        {
            GameObject audioObject = Instantiate(emitterPrefab, emitterPrefab.transform.position, emitterPrefab.transform.rotation);
            emitters.Add(audioObject.GetComponent<AudioSource>());
            DontDestroyOnLoad(audioObject);
        }

        //listHitSounds = new List<AudioClip> { hitClip1, hitClip2};

        //PlayMusic(MusicList.MENU);
    }

    public AudioSource PlaySound(SoundList sound)
    {
        //return null;
        AudioSource emitterAvailable = null;

        foreach(AudioSource emitter in emitters)
        {
            if(!emitter.isPlaying)
            {
                emitterAvailable = emitter;
            }
        }

        if (emitterAvailable != null)
        {
            emitterAvailable.loop = false;
            switch (sound)
            {
                case SoundList.TING:
                    emitterAvailable.clip = tingClip;
                    emitterAvailable.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Effect")[0];
                    break;
            }

            emitterAvailable.Play();
            return emitterAvailable;
        }
        else
        {
            Debug.Log("no emitter available");
            return null;
        }
    }

    public void PlayMusic(MusicList music)
    {
        if (currentMusicPlaying != music)
        {
            musicEmitter.loop = true;

            switch (music)
            {
                case MusicList.NONE:
                    musicEmitter.Stop();
                    break;
            }
            currentMusicPlaying = music;
        }
    }

    public void StopSound(AudioSource source)
    {
        source.Stop();
    }
}
