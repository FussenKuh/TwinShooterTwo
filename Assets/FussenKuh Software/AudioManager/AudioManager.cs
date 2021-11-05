using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FKS
{
    public class AudioManager : MonoBehaviour
    {
        #region Fields
        [Tooltip("Enable to prevent the AudioManager from getting destroyed when a new scene is loaded")]
        public bool dontDestroyOnLoad = true;
        [Tooltip("The prefab GameObject used as our audio sources")]
        public GameObject audioItemPrefab;
        [Tooltip("Define all potential 'music' items in this list")]
        public List<AudioItemRecord> musicItems = new List<AudioItemRecord>();
        [Tooltip("Define all potential 'audio' (sfx, etc.) items in this list")]
        public List<AudioItemRecord> audioItems = new List<AudioItemRecord>();

        [Header("Pool Information:")]
        [Tooltip("Maximum number of 'music' items that can be played simultaneously")]
        public int musicPoolSize = 10;
        [Tooltip("Maximum number of 'audio' items that can be played simultaneously")]
        public int audioPoolSize = 10;
        List<AudioItemComponent> musicPool = new List<AudioItemComponent>();
        List<AudioItemComponent> audioPool = new List<AudioItemComponent>();

        [Header("Internal Information:")]
        [ReadOnly]
        [SerializeField]
        [Tooltip("True if all sound items controlled by the AudioManager should be muted")]
        bool muteEverything = false;
        [ReadOnly]
        [SerializeField]
        [Tooltip("True if all 'music' items controlled by the AudioManager should be muted")]
        bool muteAllMusic = false;
        [ReadOnly]
        [SerializeField]
        [Tooltip("True if all 'audio' items controlled by the AudioManager should be muted")]
        bool muteAllAudio = false;

        static AudioManager mySelf = null;
        static int nextPoolID = 0;
        #endregion

        #region Properties

        /// <summary>
        /// The number of music items currently playing
        /// </summary>
        static public int MusicItemsPlaying
        {
            get
            {
                if (mySelf != null)
                {
                    return mySelf.musicPool.Count(x => x.Available == false);
                }
                else
                {
                    return - 1;
                }
            }
        }
        /// <summary>
        /// The number of unallocated music items
        /// </summary>
        static public int UnallocatedMusicItems
        {
            get
            {
                if (mySelf != null)
                {
                    return mySelf.musicPool.Count(x => x.Available == true);
                }
                else
                {
                    return -1;
                }
            }
        }
        /// <summary>
        /// The number of audio items currently playing
        /// </summary>
        static public int AudioItemsPlaying
        {
            get
            {
                if (mySelf != null)
                {
                    return mySelf.audioPool.Count(x => x.Available == false);
                }
                else
                {
                    return -1;
                }
            }
        }
        /// <summary>
        /// The number of unallocated music items
        /// </summary>
        static public int UnallocatedAudioItems
        {
            get
            {
                if (mySelf != null)
                {
                    return mySelf.audioPool.Count(x => x.Available == true);
                }
                else
                {
                    return -1;
                }
            }
        }
        #endregion

        #region Manager-wide Functions
        static public void MuteEverything()
        {
            if (mySelf == null) { return; }
            mySelf.muteEverything = true;
            AudioManager.MuteAllAudio();
            AudioManager.MuteAllMusic();
        }
        static public void UnmuteEverything()
        {
            if (mySelf == null) { return; }
            mySelf.muteEverything = false;
            AudioManager.UnmuteAllAudio();
            AudioManager.UnmuteAllMusic();
        }

        #endregion

        #region Music Functions
        /// <summary>
        /// Pause the music associated with the passed in ID
        /// </summary>
        /// <param name="itemID">The ID of the music item to pause</param>
        /// <returns>False if the itemID does not exist, otherwise, True</returns>
        static public bool PauseMusic(int itemID)
        {
            if (mySelf == null) { return false; }
            return mySelf._PauseAudio(itemID, mySelf.musicPool);
        }
        /// <summary>
        /// Unpause the music associated with the passed in ID
        /// </summary>
        /// <param name="itemID">The ID of the music item to unpause</param>
        /// <returns>False if the itemID does not exist, otherwise, True</returns>
        static public bool UnpauseMusic(int itemID)
        {
            if (mySelf == null) { return false; }
            return mySelf._UnpauseAudio(itemID, mySelf.musicPool);
        }
        /// <summary>
        /// Pause all music items
        /// </summary>
        static public void PauseAllMusic()
        {
            if (mySelf == null) { return; }
            mySelf._PauseAllAudio(mySelf.musicPool);
        }
        /// <summary>
        /// Unpause all music items
        /// </summary>
        static public void UnpauseAllMusic()
        {
            if (mySelf == null) { return; }
            mySelf._UnpauseAllAudio(mySelf.musicPool);
        }
        /// <summary>
        /// Play a music item using predefined default values
        /// </summary>
        /// <param name="audioName">The name of the music item to play</param>
        /// <returns>A unique ID associated with the playing music item. -1 if the item could NOT be played</returns>
        static public int PlayMusic(string audioName)
        {
            if (mySelf == null) { return -1; }
            return mySelf._PlayAudio(audioName, mySelf.musicPool, mySelf.musicItems);
        }
        /// <summary>
        /// Play a music item at the given volume
        /// </summary>
        /// <param name="audioName">The name of the music item to play</param>
        /// <param name="volume">The volume (0.0f - 1.0f) for the music item</param>
        /// <returns>A unique ID associated with the playing music item. -1 if the item could NOT be played</returns>
        static public int PlayMusic(string audioName, float volume)
        {
            if (mySelf == null) { return -1; }
            return mySelf._PlayAudio(audioName, volume, mySelf.musicPool, mySelf.musicItems);
        }
        /// <summary>
        /// Play/loop a music item at its default volume
        /// </summary>
        /// <param name="audioName">The name of the music item to play</param>
        /// <param name="loop">Whether the music item should loop or not</param>
        /// <returns>A unique ID associated with the playing music item. -1 if the item could NOT be played</returns>
        static public int PlayMusic(string audioName, bool loop)
        {
            if (mySelf == null) { return -1; }
            return mySelf._PlayAudio(audioName, loop, mySelf.musicPool, mySelf.musicItems);
        }
        /// <summary>
        /// Play/loop a music item at the given volume
        /// </summary>
        /// <param name="audioName">The name of the music item to play</param>
        /// <param name="volume">The volume (0.0f - 1.0f) for the music item</param>
        /// <param name="loop">Whether the music item should loop or not</param>
        /// <returns>A unique ID associated with the playing music item. -1 if the item could NOT be played</returns>
        static public int PlayMusic(string audioName, float volume, bool loop)
        {
            if (mySelf == null) { return -1; }
            return mySelf._PlayAudio(audioName, volume, loop, mySelf.musicPool, mySelf.musicItems);
        }
        /// <summary>
        /// Stop the music associated with the passed in ID
        /// </summary>
        /// <param name="itemID">The ID of the music item to stop playing</param>
        /// <returns>False if the itemID does not exist, otherwise, True</returns>
        static public bool StopMusic(int itemID)
        {
            if (mySelf == null) { return false; }
            return mySelf._StopAudio(itemID, mySelf.musicPool);
        }
        /// <summary>
        /// Stop all music items
        /// </summary>
        static public void StopAllMusic()
        {
            if (mySelf == null) { return; }
            mySelf._StopAllAudio(mySelf.musicPool);
        }
        /// <summary>
        /// Adjust a value associated with the music item associated with the passed in ID
        /// </summary>
        /// <param name="itemID">The ID of the music item to adjust</param>
        /// <param name="volume">The new desired volume (0.0f - 1.0f)</param>
        /// <returns>False if the itemID does not exist, otherwise, True</returns>
        static public bool AdjustMusic(int itemID, float volume)
        {
            if (mySelf == null) { return false; }
            return mySelf._AdjustAudio(itemID, volume, mySelf.musicPool);
        }
        /// <summary>
        /// Mute the music item associated with the passed in ID
        /// </summary>
        /// <param name="itemID">The ID of the music item to mute</param>
        /// <returns>False if the itemID does not exist, otherwise, True</returns>
        static public bool MuteMusic(int itemID)
        {
            if (mySelf == null) { return false; }
            return mySelf._MuteAudio(itemID, mySelf.musicPool);
        }
        /// <summary>
        /// Mute all music items
        /// </summary>
        static public void MuteAllMusic()
        {
            if (mySelf == null) { return; }
            mySelf.muteAllMusic = true;
            mySelf._MuteAllAudio(mySelf.musicPool);
        }
        /// <summary>
        /// Unmute the music item associated with the passed in ID
        /// </summary>
        /// <param name="itemID">The ID of the music item to unmute</param>
        /// <returns>False if the itemID does not exist, otherwise, True</returns>
        static public bool UnmuteMusic(int itemID)
        {
            if (mySelf == null) { return false; }
            if (mySelf.muteEverything || mySelf.muteAllMusic)
            {
                Debug.LogWarning(mySelf.DebugClassName() + " Cannot unmute an audio item when the whole manager is in Mute mode");
                return false;
            }

            return mySelf._UnmuteAudio(itemID, mySelf.musicPool);
        }
        /// <summary>
        /// Unmute all audio items
        /// </summary>
        static public void UnmuteAllMusic()
        {
            if (mySelf == null) { return; }
            mySelf.muteAllMusic = false;
            mySelf._UnmuteAllAudio(mySelf.musicPool);
        }
        //int PlayMusic(string audioName, float volume, bool loop)
        //{
        //    if (mySelf == null) { return -1; }

        //    return -1;
        //}
        //bool StopMusic(int itemID)
        //{
        //    if (mySelf == null) { return false; }

        //    return true;
        //}
        //bool StopAllMusic()
        //{
        //    if (mySelf == null) { return false; }

        //    return true;
        //}
        //bool AdjustMusic(int itemID, float volume, bool loop)
        //{
        //    if (mySelf == null) { return false; }

        //    return true;
        //}
        //bool MuteMusic(int itemID)
        //{
        //    if (mySelf == null) { return false; }

        //    return true;
        //}
        //bool MuteAllMusic()
        //{
        //    if (mySelf == null) { return false; }

        //    return true;
        //}
        //bool UnmuteMusic(int itemID)
        //{
        //    if (mySelf == null) { return false; }

        //    return true;
        //}
        //bool UnmuteAllMusic()
        //{
        //    if (mySelf == null) { return false; }

        //    return true;
        //}
        #endregion

        #region Audio Functions
        /// <summary>
        /// Pause the audio associated with the passed in ID
        /// </summary>
        /// <param name="itemID">The ID of the audio item to pause</param>
        /// <returns>False if the itemID does not exist, otherwise, True</returns>
        static public bool PauseAudio(int itemID)
        {
            if (mySelf == null) { return false; }
            return mySelf._PauseAudio(itemID, mySelf.audioPool);
        }
        /// <summary>
        /// Unpause the audio associated with the passed in ID
        /// </summary>
        /// <param name="itemID">The ID of the audio item to unpause</param>
        /// <returns>False if the itemID does not exist, otherwise, True</returns>
        static public bool UnpauseAudio(int itemID)
        {
            if (mySelf == null) { return false; }
            return mySelf._UnpauseAudio(itemID, mySelf.audioPool);
        }
        /// <summary>
        /// Pause all audio items
        /// </summary>
        static public void PauseAllAudio()
        {
            if (mySelf == null) { return; }
            mySelf._PauseAllAudio(mySelf.audioPool);
        }
        /// <summary>
        /// Unpause all audio items
        /// </summary>
        static public void UnpauseAllAudio()
        {
            if (mySelf == null) { return; }
            mySelf._UnpauseAllAudio(mySelf.audioPool);
        }
        /// <summary>
        /// Play an audio item using predefined default values
        /// </summary>
        /// <param name="audioName">The name of the audio item to play</param>
        /// <returns>A unique ID associated with the playing audio item. -1 if the item could NOT be played</returns>
        static public int PlayAudio(string audioName)
        {
            if (mySelf == null) { return -1; }
            return mySelf._PlayAudio(audioName, mySelf.audioPool, mySelf.audioItems);
        }
        /// <summary>
        /// Play an audio item at the given volume
        /// </summary>
        /// <param name="audioName">The name of the audio item to play</param>
        /// <param name="volume">The volume (0.0f - 1.0f) for the audio item</param>
        /// <returns>A unique ID associated with the playing audio item. -1 if the item could NOT be played</returns>
        static public int PlayAudio(string audioName, float volume)
        {
            if (mySelf == null) { return -1; }
            return mySelf._PlayAudio(audioName, volume, mySelf.audioPool, mySelf.audioItems);
        }
        /// <summary>
        /// Play/loop an audio item at its default volume
        /// </summary>
        /// <param name="audioName">The name of the audio item to play</param>
        /// <param name="loop">Whether the audio item should loop or not</param>
        /// <returns>A unique ID associated with the playing audio item. -1 if the item could NOT be played</returns>
        static public int PlayAudio(string audioName, bool loop)
        {
            if (mySelf == null) { return -1; }
            return mySelf._PlayAudio(audioName, loop, mySelf.audioPool, mySelf.audioItems);
        }
        /// <summary>
        /// Play/loop an audio item at the given volume
        /// </summary>
        /// <param name="audioName">The name of the audio item to play</param>
        /// <param name="volume">The volume (0.0f - 1.0f) for the audio item</param>
        /// <param name="loop">Whether the audio item should loop or not</param>
        /// <returns>A unique ID associated with the playing audio item. -1 if the item could NOT be played</returns>
        static public int PlayAudio(string audioName, float volume, bool loop)
        {
            if (mySelf == null) { return -1; }
            return mySelf._PlayAudio(audioName, volume, loop, mySelf.audioPool, mySelf.audioItems);
        }
        /// <summary>
        /// Stop the audio associated with the passed in ID
        /// </summary>
        /// <param name="itemID">The ID of the audio item to stop playing</param>
        /// <returns>False if the itemID does not exist, otherwise, True</returns>
        static public bool StopAudio(int itemID)
        {
            if (mySelf == null) { return false; }
            return mySelf._StopAudio(itemID, mySelf.audioPool);
        }
        /// <summary>
        /// Stop all audio items
        /// </summary>
        static public void StopAllAudio()
        {
            if (mySelf == null) { return; }
            mySelf._StopAllAudio(mySelf.audioPool);
        }
        /// <summary>
        /// Adjust a value associated with the audio item associated with the passed in ID
        /// </summary>
        /// <param name="itemID">The ID of the audio item to adjust</param>
        /// <param name="volume">The new desired volume (0.0f - 1.0f)</param>
        /// <returns>False if the itemID does not exist, otherwise, True</returns>
        static public bool AdjustAudio(int itemID, float volume)
        {
            if (mySelf == null) { return false; }
            return mySelf._AdjustAudio(itemID, volume, mySelf.audioPool);
        }
        /// <summary>
        /// Mute the audio item associated with the passed in ID
        /// </summary>
        /// <param name="itemID">The ID of the audio item to mute</param>
        /// <returns>False if the itemID does not exist, otherwise, True</returns>
        static public bool MuteAudio(int itemID)
        {
            if (mySelf == null) { return false; }
            return mySelf._MuteAudio(itemID, mySelf.audioPool);
        }
        /// <summary>
        /// Mute all audio items
        /// </summary>
        static public void MuteAllAudio()
        {
            if (mySelf == null) { return; }
            mySelf.muteAllAudio = true;
            mySelf._MuteAllAudio(mySelf.audioPool);
        }
        /// <summary>
        /// Unmute the audio item associated with the passed in ID
        /// </summary>
        /// <param name="itemID">The ID of the audio item to unmute</param>
        /// <returns>False if the itemID does not exist, otherwise, True</returns>
        static public bool UnmuteAudio(int itemID)
        {
            if (mySelf == null) { return false; }
            if (mySelf.muteEverything || mySelf.muteAllAudio)
            {
                Debug.LogWarning(mySelf.DebugClassName() + " Cannot unmute an audio item when the whole manager is in Mute mode");
                return false;
            }

            return mySelf._UnmuteAudio(itemID, mySelf.audioPool);
        }
        /// <summary>
        /// Unmute all audio items
        /// </summary>
        static public void UnmuteAllAudio()
        {
            if (mySelf == null) { return; }
            mySelf.muteAllAudio = false;
            mySelf._UnmuteAllAudio(mySelf.audioPool);
        }
        #endregion

        #region Internal Generic Audio Functions
        /// <summary>
        /// Play an audio item using predefined default values
        /// </summary>
        /// <param name="audioName">The name of the audio item to play</param>
        /// <param name="audioPool">The pool of AudioItemComponents to search through</param>
        /// <param name="audioItems">The pool of AudioItemRecord templates to search through</param>
        /// <returns>A unique ID associated with the playing audio item. -1 if the item could NOT be played</returns>
        int _PlayAudio(string audioName, List<AudioItemComponent> audioPool, List<AudioItemRecord> audioItems)
        {
            if (mySelf == null) { return -1; }

            AudioItemComponent itemToPlay = mySelf.RetrieveAvailableItem(audioPool);
            AudioItemRecord template = mySelf.RetrieveItem(audioItems, audioName);

            if (template == null)
            {
                Debug.LogWarning(mySelf.DebugClassName() + " '" + audioName + "' can not be found. Did you define it?");
                return -1;
            }

            if (itemToPlay == null)
            {
                Debug.LogWarning(mySelf.DebugClassName() + " No available audio sources.");
                return -1;
            }

            return mySelf._PlayAudio(audioName, template.Volume, template.Looping, audioPool, audioItems);
        }
        /// <summary>
        /// Play an audio item at the given volume
        /// </summary>
        /// <param name="audioName">The name of the audio item to play</param>
        /// <param name="volume">The volume (0.0f - 1.0f) for the audio item</param>
        /// <param name="audioPool">The pool of AudioItemComponents to search through</param>
        /// <param name="audioItems">The pool of AudioItemRecord templates to search through</param>
        /// <returns>A unique ID associated with the playing audio item. -1 if the item could NOT be played</returns>
        int _PlayAudio(string audioName, float volume, List<AudioItemComponent> audioPool, List<AudioItemRecord> audioItems)
        {
            if (mySelf == null) { return -1; }

            AudioItemComponent itemToPlay = mySelf.RetrieveAvailableItem(audioPool);
            AudioItemRecord template = mySelf.RetrieveItem(audioItems, audioName);

            if (template == null)
            {
                Debug.LogWarning(mySelf.DebugClassName() + " '" + audioName + "' can not be found. Did you define it?");
                return -1;
            }

            if (itemToPlay == null)
            {
                Debug.LogWarning(mySelf.DebugClassName() + " No available audio sources.");
                return -1;
            }

            return mySelf._PlayAudio(audioName, volume, template.Looping, audioPool, audioItems);
        }
        /// <summary>
        /// Play/loop an audio item at its default volume
        /// </summary>
        /// <param name="audioName">The name of the audio item to play</param>
        /// <param name="loop">Whether the audio item should loop or not</param>
        /// <param name="audioPool">The pool of AudioItemComponents to search through</param>
        /// <param name="audioItems">The pool of AudioItemRecord templates to search through</param>
        /// <returns>A unique ID associated with the playing audio item. -1 if the item could NOT be played</returns>
        int _PlayAudio(string audioName, bool loop, List<AudioItemComponent> audioPool, List<AudioItemRecord> audioItems)
        {
            if (mySelf == null) { return -1; }

            AudioItemComponent itemToPlay = mySelf.RetrieveAvailableItem(audioPool);
            AudioItemRecord template = mySelf.RetrieveItem(audioItems, audioName);

            if (template == null)
            {
                Debug.LogWarning(mySelf.DebugClassName() + " '" + audioName + "' can not be found. Did you define it?");
                return -1;
            }

            if (itemToPlay == null)
            {
                Debug.LogWarning(mySelf.DebugClassName() + " No available audio sources.");
                return -1;
            }

            return mySelf._PlayAudio(audioName, template.Volume, loop, audioPool, audioItems);
        }
        /// <summary>
        /// Play/loop an audio item at the given volume
        /// </summary>
        /// <param name="audioName">The name of the audio item to play</param>
        /// <param name="volume">The volume (0.0f - 1.0f) for the audio item</param>
        /// <param name="loop">Whether the audio item should loop or not</param>
        /// <param name="audioPool">The pool of AudioItemComponents to search through</param>
        /// <param name="audioItems">The pool of AudioItemRecord templates to search through</param>
        /// <returns>A unique ID associated with the playing audio item. -1 if the item could NOT be played</returns>
        int _PlayAudio(string audioName, float volume, bool loop, List<AudioItemComponent> audioPool, List<AudioItemRecord> audioItems)
        {
            if (mySelf == null) { return -1; }

            AudioItemComponent itemToPlay = mySelf.RetrieveAvailableItem(audioPool);
            AudioItemRecord template = mySelf.RetrieveItem(audioItems, audioName);

            if (template == null)
            {
                Debug.LogWarning(mySelf.DebugClassName() + " '" + audioName + "' can not be found. Did you define it?");
                return -1;
            }

            if (itemToPlay == null)
            {
                Debug.LogWarning(mySelf.DebugClassName() + " No available audio sources.");
                return -1;
            }

            itemToPlay.Configure(audioName, volume, loop, template.Clips);

            if (itemToPlay.ID > 0)
            {
                if (mySelf.muteAllAudio) { itemToPlay.Mute(); } // If the whole Audio system is muted, make sure we start out this item in a muted state;
                itemToPlay.Play();
            }

            return itemToPlay.ID;
        }

        /// <summary>
        /// Unpause the audio associated with the passed in ID
        /// </summary>
        /// <param name="itemID">The ID of the audio item to pause</param>
        /// <param name="audioPool">The pool of AudioItemRecord templates to search through</param>
        /// <returns></returns>
        bool _PauseAudio(int itemID, List<AudioItemComponent> audioPool)
        {
            if (mySelf == null) { return false; }

            bool retVal = false;
            var item = mySelf.RetrievePlayingItem(audioPool, itemID);

            if (item != null)
            {
                item.Pause();
                retVal = true;
            }

            return retVal;
        }

        /// <summary>
        /// Unpause the audio associated with the passed in ID
        /// </summary>
        /// <param name="itemID">The ID of the audio item to unpause</param>
        /// <param name="audioPool">The pool of AudioItemRecord templates to search through</param>
        /// <returns></returns>
        bool _UnpauseAudio(int itemID, List<AudioItemComponent> audioPool)
        {
            if (mySelf == null) { return false; }
            bool retVal = false;

            var item = mySelf.RetrievePlayingItem(audioPool, itemID);

            if (item != null)
            {
                item.Unpause();
                retVal = true;
            }

            return retVal;
        }
        /// <summary>
        /// Pause all audio items
        /// </summary>
        /// <param name="audioPool">The pool of AudioItemComponents to search through</param>
        void _PauseAllAudio(List<AudioItemComponent> audioPool)
        {
            if (mySelf == null) { return; }

            foreach (var item in audioPool)
            {
                item.Pause();
            }
        }
        /// <summary>
        /// Unpause all audio items
        /// </summary>
        /// <param name="audioPool">The pool of AudioItemComponents to search through</param>
        void _UnpauseAllAudio(List<AudioItemComponent> audioPool)
        {
            if (mySelf == null) { return; }

            foreach (var item in audioPool)
            {
                item.Unpause();
            }
        }
        /// <summary>
        /// Stop the audio associated with the passed in ID
        /// </summary>
        /// <param name="itemID">The ID of the audio item to stop playing</param>
        /// <param name="audioPool">The pool of AudioItemComponents to search through</param>
        /// <returns>False if the itemID does not exist, otherwise, True</returns>
        bool _StopAudio(int itemID, List<AudioItemComponent> audioPool)
        {
            if (mySelf == null) { return false; }

            bool retVal = false;

            var item = mySelf.RetrievePlayingItem(audioPool, itemID);

            if (item != null)
            {
                item.Stop();
                retVal = true;
            }

            return retVal;
        }
        /// <summary>
        /// Stop all audio items
        /// </summary>
        /// <param name="audioPool">The pool of AudioItemComponents to search through</param>
        void _StopAllAudio(List<AudioItemComponent> audioPool)
        {
            if (mySelf == null) { return; }

            foreach (var item in audioPool)
            {
                item.Stop();
            }
        }
        /// <summary>
        /// Adjust a value associated with the audio item associated with the passed in ID
        /// </summary>
        /// <param name="itemID">The ID of the audio item to adjust</param>
        /// <param name="volume">The new desired volume (0.0f - 1.0f)</param>
        /// <param name="audioPool">The pool of AudioItemComponents to search through</param>
        /// <returns>False if the itemID does not exist, otherwise, True</returns>
        bool _AdjustAudio(int itemID, float volume, List<AudioItemComponent> audioPool)
        {
            if (mySelf == null) { return false; }

            bool retVal = false;

            var item = mySelf.RetrievePlayingItem(audioPool, itemID);

            if (item != null)
            {
                item.AdjustAudio(volume);
                retVal = true;
            }

            return retVal;
        }
        /// <summary>
        /// Mute the audio item associated with the passed in ID
        /// </summary>
        /// <param name="itemID">The ID of the audio item to mute</param>
        /// <param name="audioPool">The pool of AudioItemComponents to search through</param>
        /// <returns>False if the itemID does not exist, otherwise, True</returns>
        bool _MuteAudio(int itemID, List<AudioItemComponent> audioPool)
        {
            if (mySelf == null) { return false; }

            bool retVal = false;

            var item = mySelf.RetrievePlayingItem(audioPool, itemID);

            if (item != null)
            {
                item.Mute();
                retVal = true;
            }

            return retVal;
        }
        /// <summary>
        /// Mute all audio items
        /// </summary>
        /// <param name="audioPool">The pool of AudioItemComponents to search through</param>
        void _MuteAllAudio(List<AudioItemComponent> audioPool)
        {
            if (mySelf == null) { return; }

            foreach (var item in audioPool)
            {
                item.Mute();
            }
        }
        /// <summary>
        /// Unmute the audio item associated with the passed in ID
        /// </summary>
        /// <param name="itemID">The ID of the audio item to unmute</param>
        /// <param name="audioPool">The pool of AudioItemComponents to search through</param>
        /// <returns>False if the itemID does not exist, otherwise, True</returns>
        bool _UnmuteAudio(int itemID, List<AudioItemComponent> audioPool)
        {
            if (mySelf == null) { return false; }

            bool retVal = false;

            var item = mySelf.RetrievePlayingItem(audioPool, itemID);

            if (item != null)
            {
                item.Unmute();
                retVal = true;
            }

            return retVal;
        }
        /// <summary>
        /// Unmute all audio items
        /// </summary>
        /// <param name="audioPool">The pool of AudioItemComponents to search through</param>
        void _UnmuteAllAudio(List<AudioItemComponent> audioPool)
        {
            if (mySelf == null) { return; }

            foreach (var item in audioPool)
            {
                item.Unmute();
            }
        }
        #endregion

        #region Internal Helper Functions

        /// <summary>
        /// Attempt to retrieve an AudioItemRecord from the given pool
        /// </summary>
        /// <param name="pool">The pool to search through</param>
        /// <param name="audioName">The name of the Audio Item we're looking for</param>
        /// <returns>If found, the AudioItemRecord, otherwise null</returns>
        AudioItemRecord RetrieveItem(List<AudioItemRecord> pool, string audioName)
        {
            // Use Linq to find the first available item that hasn't been assigned an ID (i.e. unallocated item). Returns null if all items are in use
            return pool.FirstOrDefault(s => s.AudioName == audioName);
        }

        /// <summary>
        /// Attempt to retrieve an AudioItemComponent from the given pool
        /// </summary>
        /// <param name="pool">The pool to search through</param>
        /// <returns>An unallocated AudioItemComponent. If all components are in use, null will be returned</returns>
        AudioItemComponent RetrieveAvailableItem(List<AudioItemComponent> pool)
        {
            return pool.FirstOrDefault(s => s.Available == true);
        }

        /// <summary>
        /// Attempt to retrieve an active AudioItemComponent
        /// </summary>
        /// <param name="pool">The pool to search through</param>
        /// <param name="id">The ID of the item we're trying to find</param>
        /// <returns>If found, the AudioItemComponent, otherwise null</returns>
        AudioItemComponent RetrievePlayingItem(List<AudioItemComponent> pool, int id)
        {
            return pool.FirstOrDefault(s => s.ID == id);
        }

        /// <summary>
        /// Initialize a pool of AudioItemComponents to the given size
        /// </summary>
        /// <param name="pool">The pool to populate</param>
        /// <param name="size">The number of items to create in the pool</param>
        /// <param name="poolName">The (optional) name for the pool</param>
        void InitPool(List<AudioItemComponent> pool, int size, string poolName=null)
        {

            // Create an empty child to simply hold the audioItem gameobjects we'll be making
            GameObject tmpObj;
            if (poolName == null)
            {
                tmpObj = new GameObject("Pool (" + (++nextPoolID) + ")");
            }
            else
            {
                tmpObj = new GameObject(poolName);
            }

            tmpObj.transform.parent = transform;

            for (int i=0; i< size; i++)
            {
                AudioItemComponent tmp = GameObject.Instantiate(audioItemPrefab, tmpObj.transform).GetComponent<AudioItemComponent>();
                if (tmp != null)
                {
                    tmp.name = "Item (" + (i + 1).ToString() + ")"; 
                    pool.Add(tmp);
                }
                else
                {
                    Debug.LogWarning(this.DebugClassName() + " Attempting to initialize AudioItem pool with a GameObject that does NOT have an AudioItem component.");
                }
            }
        }

        int CountItems(List<AudioItemComponent> pool, bool countAvailable)
        {
            if (countAvailable)
            {
                return 0;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Standard Unity Functions
        private void Awake()
        {
            if (mySelf == null)
            {
                mySelf = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            InitPool(musicPool, musicPoolSize, "MusicPool");
            InitPool(audioPool, audioPoolSize, "AudioPool");
        }
        #endregion
    }


}