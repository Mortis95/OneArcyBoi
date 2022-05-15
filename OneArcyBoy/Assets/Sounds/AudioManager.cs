using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    
    public Sound[] sounds;

    private static AudioManager instance;

    public static AudioManager getInstance(){
        return instance;
    }
    

    void Awake(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }

        foreach (Sound sound in sounds){
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.isLooping;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(string name){
        Sound sound = Array.Find(sounds, s => s.name == name);
        if (sound == null){
            Debug.LogWarning("Sound " + name + " konnte nicht gefunden werden im sound Array und wird deswegen nicht abgespielt. \nTypo? Vergessen den Sound hinzuzufügen? Sonderzeichen?");
            return;
        }
        if (!sound.source.isPlaying){
            /* Debug.Log("Playing Sound:" + sound.name); */
            sound.source.Play();;
        }
    }

    public void Stop(string name){
        Sound sound = Array.Find(sounds, s => s.name == name);
        if (sound == null){
            Debug.LogWarning("Sound " + name + " konnte nicht gefunden werden im sound Array und wird deswegen nicht abgespielt. \nTypo? Vergessen den Sound hinzuzufügen? Sonderzeichen?");
            return;
        }
        if (sound.source.isPlaying){
            /* Debug.Log("Stopping Sound:" + sound.name); */
            sound.source.Stop();
        }
    }
}
