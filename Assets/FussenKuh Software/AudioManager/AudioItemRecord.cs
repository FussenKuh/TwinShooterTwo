using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FKS
{
    [Serializable]
    public class AudioItemRecord
    {
        [Serializable]
        public class AudClip
        {
            [Tooltip("Should the AudClip's own volume value be used to override the normal volume level?")]
            public bool overrideVolume = false;
            [Tooltip("The clip's volume (assuming overrideVolume is 'True')")]
//            [Range(0.0f, 1.0f)]
            public float volume = 1.0f;
            [Tooltip("The AudioClip")]
            public AudioClip clip;
        }

        #region Fields
        static int nextIdIndex = 0;

        [SerializeField]
        [Tooltip("The name used to reference the audio item")]
        string audioName;
        [SerializeField]
        [Tooltip("Is this a looping audio item?")]
        bool looping;
        [SerializeField]
        [Tooltip("The default volume of the audio item ( 0.0 - 1.0 )")]
        [Range(0.0f, 1.0f)]
        float volume = 1.0f;
        //[SerializeField]
        //[Tooltip("The AudioClip used by the audio item")]
        //AudioClip clip;
        [SerializeField]
        [Tooltip("The AudioClip used by the audio item")]
        AudClip[] clips;
        int id;
        bool available = true;
        AudioSource source;
        #endregion

        #region Properties
        /// <summary>
        /// The item's unique ID 
        /// </summary>
        public int ID { get { return id; } }
        /// <summary>
        /// Whether or not the item is available for use
        /// </summary>
        public bool Available { get { return available; } }
        /// <summary>
        /// The item's name
        /// </summary>
        public string AudioName { get { return audioName; } }
        /// <summary>
        /// Whether the item is configured to loop or not
        /// </summary>
        public bool Looping { get { return looping; } }
        /// <summary>
        /// The item's volume
        /// </summary>
        public float Volume { get { return volume; } }
        ///// <summary>
        ///// The AudioClip associated with the item
        ///// </summary>
        //public AudioClip Clip { get { return clip; } }
        /// <summary>
        /// The AudClips associated with the item
        /// </summary>
        public AudClip[] Clips { get { return clips; } }
        /// <summary>
        /// The AudioSource associated with the item
        /// </summary>
        public AudioSource Source { get { return source; } }
        #endregion

        /// <summary>
        /// Configure the audio item for use
        /// </summary>
        /// <param name="argName">The item's name</param>
        /// <param name="argVolume">The item's volume (0.0f - 1.0f)</param>
        /// <param name="argLoop">Should the item be configured to loop?</param>
        /// <param name="argClip">The AudioClip associated with the item</param>
        /// <param name="argSource">The AudioSource associated with the item</param>
        /// <returns>The item's unique ID</returns>
        public int Configure(string argName, float argVolume, bool argLoop, AudClip[] argClips, AudioSource argSource)
        {
            id = ++nextIdIndex;
            available = false;
            audioName = argName;
            volume = argVolume;
            looping = argLoop;
            clips = (AudClip[])argClips.Clone();
            source = argSource;

            source.loop = looping;
            source.volume = volume; // Set the default volume. If the particular clip has override set, we'll override the volume later

            int clipIndex = UnityEngine.Random.Range(0, clips.Length);
            if (clips[clipIndex].overrideVolume) { source.volume = clips[clipIndex].volume; }
            source.clip = clips[clipIndex].clip; // Pre-populate the source with a random clip

            return id;
        }

        /// <summary>
        /// Adjusts the item's volume
        /// </summary>
        /// <param name="argVolume">The item's new volume (0.0f - 1.0f)</param>
        public void AdjustAudio(float argVolume)
        {
            if (source == null) { return; }

            volume = argVolume;
            source.volume = volume;
        }

        /// <summary>
        /// Adjusts the item's tempo
        /// </summary>
        /// <param name="argTempo">The item's new tempo (1.0f - 2.0f)</param>
        public void AdjustAudioTempo(float argTempo)
        {
            if (source == null) { return; }

            source.pitch = argTempo;
            //source.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", 1f / argTempo);
        }

        /// <summary>
        /// Start playback
        /// </summary>
        public void Play()
        {
            if (source == null) { return; }

            source.volume = volume; // Set the default volume. If the particular clip has override set, we'll override the volume later

            int clipIndex = UnityEngine.Random.Range(0, clips.Length);
            if (clips[clipIndex].overrideVolume) { source.volume = clips[clipIndex].volume; }
            source.clip = clips[clipIndex].clip; // Populate the source with a random clip

            source.Play();
        }

        /// <summary>
        /// Stop playback
        /// </summary>
        public void Stop()
        {
            if (source == null) { return; }

            source.Stop();
            id = 0;
            available = true;
        }

        /// <summary>
        /// Pause playback
        /// </summary>
        public void Pause()
        {
            if (source == null) { return; }

            source.Pause();
        }

        /// <summary>
        /// UnPause playback
        /// </summary>
        public void Unpause()
        {
            if (source == null) { return; }

            source.UnPause();
        }

        /// <summary>
        /// Mute playback
        /// </summary>
        public void Mute()
        {
            if (source == null) { return; }

            source.mute = true;
        }

        /// <summary>
        /// Unmute playback
        /// </summary>
        public void Unmute()
        {
            if (source == null) { return; }

            source.mute = false;
        }
    }
}
