using System;
using UnityEngine;
using Zone.Core.Utils;

// Author: Anik Patel
// Description: AudioManager using the Singleton Class, modified from Brackey's Tutorial
//              use with 'AudioManager.instance.METHOD'
// Version: 1.0
// Changes: [N/A]
namespace Zone.Core.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        // Add each sound effect to this array through the Inspector
        public Sound[] sounds;
        void Start()
        {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
        }

        // Plays the specified sound
        // Param {name}: Name of the sound effect in the array
        public void Play(string name)
        {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("ERROR: INVALID SOUND");
            return;
        }
        s.source.Play();
        }

        // Stops playing the specified sound
        // Param {name}: Name of the sound effect in the array
        public void Stop(string name)
        {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("ERROR: INVALID SOUND");
            return;
        }
        s.source.Stop();
        }
    }
}
