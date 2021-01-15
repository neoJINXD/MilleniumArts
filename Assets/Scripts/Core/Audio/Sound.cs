using UnityEngine;

// Author: Anik Patel
// Description: Base class with the information for sounds based on Brackey's Tutorial
// Version: 1.0
// Changes: [N/A]
namespace Zone.Core.Audio
{
    [System.Serializable] public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
        [Range(0.1f, 3f)] public float pitch;
        public bool loop;

        // Reference to Audio Source
        [HideInInspector] public AudioSource source;
}
}
