using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FKS
{
    [RequireComponent(typeof(AudioSource))]
    [Serializable]
    public class AudioItemComponent : MonoBehaviour
    {
        #region Fields
        [HideInInspector]
        public AudioItemRecord audioItem;
        AudioSource source;
        bool paused;
        #endregion
        
        #region Properties
        /// <summary>
        /// The item's unique ID 
        /// </summary>
        public int ID { get { return audioItem.ID; } }
        /// <summary>
        /// Whether or not the item is available for use
        /// </summary>
        public bool Available { get { return audioItem.Available; } }
        #endregion

        #region Interface Functions
        /// <summary>
        /// Configure the audio item for use
        /// </summary>
        /// <param name="argName">The item's name</param>
        /// <param name="argVolume">The item's volume (0.0f - 1.0f)</param>
        /// <param name="argLoop">Should the item be configured to loop?</param>
        /// <param name="argClip">The AudioClip associated with the item</param>
        /// <returns>The item's unique ID</returns>
        public int Configure(string argName, float argVolume, bool argLoop, AudioItemRecord.AudClip[] argClips)
        {
            return audioItem.Configure(argName, argVolume, argLoop, argClips, source);
        }

        /// <summary>
        /// Adjusts the component's volume
        /// </summary>
        /// <param name="volume">The item's new volume (0.0f - 1.0f)</param>
        public void AdjustAudio(float volume)
        {
            audioItem.AdjustAudio(volume);
        }

        /// <summary>
        /// Adjusts the component's Tempo
        /// </summary>
        /// <param name="tempo">The item's new tempo (1.0f - 2.0f)</param>
        public void AdjustAudioTempo(float tempo)
        {
            audioItem.AdjustAudioTempo(tempo);
        }

        /// <summary>
        /// Start playback
        /// </summary>
        /// <param name="muted">If true, the item will start playing in its muted state</param>
        public void Play(bool muted=false)
        {
            if (!source.isPlaying)
            {
                if (muted) { audioItem.Mute(); }
                audioItem.Play();
                StartCoroutine(FinalizeAudioItem());
            }
        }

        /// <summary>
        /// Stop playback
        /// </summary>
        public void Stop()
        {
            if (source.isPlaying || paused)
            {
                audioItem.Stop();
            }
        }

        /// <summary>
        /// Pause playback
        /// </summary>
        public void Pause()
        {
            if (source.isPlaying)
            {
                paused = true;
                audioItem.Pause();
            }
        }

        /// <summary>
        /// UnPause playback
        /// </summary>
        public void Unpause()
        {
            if (source.isPlaying || paused)
            {
                paused = false;
                audioItem.Unpause();
            }
        }

        /// <summary>
        /// Mute playback
        /// </summary>
        public void Mute()
        {
            audioItem.Mute();
        }

        /// <summary>
        /// Unmute playback
        /// </summary>
        public void Unmute()
        {
            audioItem.Unmute();
        }
        #endregion

        #region Internal Helper Functions
        /// <summary>
        /// Fires off when the audio starts playing. Resets item to default values when the audio finishes playing
        /// </summary>
        /// <returns>N/A</returns>
        IEnumerator FinalizeAudioItem()
        {
            // Wait around until the clip is done playing
            yield return new WaitWhile(() => (source.isPlaying || paused));
            // Once done playing, issue a 'stop' which will update the audio item to state it's available
            audioItem.Unmute();
            audioItem.Stop();
        }
        #endregion

        #region Standard Unity Functions
        private void Awake()
        {
            source = GetComponent<AudioSource>();
        }
        #endregion

    }
}
