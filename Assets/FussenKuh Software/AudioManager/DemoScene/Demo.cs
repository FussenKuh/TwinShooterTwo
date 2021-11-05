using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FKS;
using System;

public class Demo : MonoBehaviour {

    /// <summary>
    /// Structure to hold information about looping audio and music
    /// </summary>
    [Serializable]
    struct LoopingInfo
    {
        public int id;
        public bool playing;
        public bool muted;
        public TextMesh ui;
    }

    #region Fields
    [HideInInspector]
    [SerializeField]
    TextMesh audioSourceInfo = new TextMesh();
    [HideInInspector]
    [SerializeField]
    TextMesh musicSourceInfo = new TextMesh();
    [HideInInspector]
    [SerializeField]
    TextMesh volumeTestInfo = new TextMesh();
    [HideInInspector]
    [SerializeField]
    LoopingInfo[] loopingInfo = new LoopingInfo[5];
    [HideInInspector]
    [SerializeField]
    LoopingInfo[] musicInfo = new LoopingInfo[5];

    [Header("Debug Information:")]
    [ReadOnly]
    [SerializeField]
    bool allAudioMuted = false;
    [ReadOnly]
    [SerializeField]
    bool allMusicMuted = false;
    [ReadOnly]
    [SerializeField]
    bool everythingMuted = false;
    [ReadOnly]
    [SerializeField]
    bool volumeTestInProgress = false;
    [ReadOnly]
    [SerializeField]
    bool audioPaused = false;
    [ReadOnly]
    [SerializeField]
    bool musicPaused = false;

    #endregion

    #region AudioManager Interfacing Examples

    /// <summary>
    /// Uses the AudioManager interface to display the number of active and unallocated audio sources
    /// </summary>
    void UpdateSourceInfo()
    {
        audioSourceInfo.text =
            "Audio:\n" + AudioManager.AudioItemsPlaying +
            " In Use\n" + AudioManager.UnallocatedAudioItems +
            " Available";

        musicSourceInfo.text =
            "Music:\n" + AudioManager.MusicItemsPlaying +
            " In Use\n" + AudioManager.UnallocatedMusicItems +
            " Available";
    }

    /// <summary>
    /// Uses the AudioManager interface to play/stop or mute/unmute audio items
    /// </summary>
    /// <param name="index">The index of the item we're dealing with</param>
    /// <param name="toggleMute">Are we trying to (un)mute the item or simply play/stop the item?</param>
    void UpdateLoopingInfo(int index, bool toggleMute=false)
    {
        if (toggleMute)
        {
            if (loopingInfo[index].muted)
            {
                loopingInfo[index].muted = !AudioManager.UnmuteAudio(loopingInfo[index].id);
            }
            else
            {
                loopingInfo[index].muted = AudioManager.MuteAudio(loopingInfo[index].id);
            }
        }
        else
        {
            if (loopingInfo[index].playing)
            {
                loopingInfo[index].playing = !AudioManager.StopAudio(loopingInfo[index].id);
                loopingInfo[index].muted = false;
            }
            else
            {
                loopingInfo[index].id = AudioManager.PlayAudio("Test", 1, true);
                if (loopingInfo[index].id > 0)
                {
                    loopingInfo[index].playing = true;
                }
            }
        }
    }

    /// <summary>
    /// Uses the AudioManager interface to play/stop or mute/unmute music items
    /// </summary>
    /// <param name="index">The index of the item we're dealing with</param>
    /// <param name="toggleMute">Are we trying to (un)mute the item or simply play/stop the item?</param>
    void UpdateMusicInfo(int index, bool toggleMute = false)
    {
        if (toggleMute)
        {
            if (musicInfo[index].muted)
            {
                musicInfo[index].muted = !AudioManager.UnmuteMusic(musicInfo[index].id);
            }
            else
            {
                musicInfo[index].muted = AudioManager.MuteMusic(musicInfo[index].id);
            }
        }
        else
        {
            if (musicInfo[index].playing)
            {
                musicInfo[index].playing = !AudioManager.StopMusic(musicInfo[index].id);
                musicInfo[index].muted = false;
            }
            else
            {
                musicInfo[index].id = AudioManager.PlayMusic("Song", 1, true);
                if (musicInfo[index].id > 0)
                {
                    musicInfo[index].playing = true;
                }
            }
        }
    }

    /// <summary>
    /// Uses the AudioManager interface to adjust the volume of an audio item. 
    /// While not demonstrated here, adjusting music items works the same way.
    /// </summary>
    /// <returns>N/A</returns>
    IEnumerator AdjustVolumeTest()
    {
        if (!volumeTestInProgress)
        {
            Debug.Log(this.DebugClassName() + " Starting Adjust volume test...");
            volumeTestInProgress = true;
            int i = 0;
            int id = AudioManager.PlayAudio("Test", 1, true); // Loop a sound at max volume

            while (i < 2)
            {
                i++;
                for (float f = 1.0f; f > 0.0f; f -= 0.05f)
                {
                    yield return new WaitForSeconds(0.1f);
                    AudioManager.AdjustAudio(id, f);
                }
                yield return new WaitForSeconds(0.1f);
                for (float f = 0.0f; f < 1.0f; f += 0.05f)
                {
                    yield return new WaitForSeconds(0.1f);
                    AudioManager.AdjustAudio(id, f);
                }
            }
            AudioManager.StopAudio(id);

            volumeTestInProgress = false;
            Debug.Log(this.DebugClassName() + " Adjust volume test complete");
        }
        else
        {
            Debug.Log(this.DebugClassName() + " Adjust volume test already in progress");
        }

    }

    #endregion

    #region Internal Helper Functions

    /// <summary>
    /// Responsible for Updating items 1-5 in the UI
    /// </summary>
    void UpdateLoopingUICollection()
    {
        for (int i = 0; i < loopingInfo.Length; i++)
        {
            if (loopingInfo[i].ui == null)
            {
                loopingInfo[i].ui = GameObject.Find("Text - Audio (" + (i+1) + ")").GetComponent<TextMesh>();
            }

            if (loopingInfo[i].muted || allAudioMuted)
            {
                loopingInfo[i].ui.text = (i+1) + "\nM";
            }
            else if (loopingInfo[i].playing)
            {
                loopingInfo[i].ui.text = (i + 1) + "\nP";
            }
            else
            {
                loopingInfo[i].ui.text = (i + 1) + "\n-";
            }
            
        }
    }

    /// <summary>
    /// Responsible for Updating items 6-0 in the UI
    /// </summary>
    void UpdateMusicUICollection()
    {
        for (int i = 0; i < musicInfo.Length; i++)
        {
            if (musicInfo[i].ui == null)
            {
                musicInfo[i].ui = GameObject.Find("Text - Music (" + (i + 1) + ")").GetComponent<TextMesh>();
            }

            if (musicInfo[i].muted || allMusicMuted)
            {
                musicInfo[i].ui.text = (i + 6)%10 + "\nM";
            }
            else if (musicInfo[i].playing)
            {
                musicInfo[i].ui.text = (i + 6)%10 + "\nP";
            }
            else
            {
                musicInfo[i].ui.text = (i + 6)%10 + "\n-";
            }

        }
    }

    /// <summary>
    /// Responsible for Updating the "Volume Test In Progress" value in the UI
    /// </summary>
    void UpdateVolumeTestInfo()
    {
        if (volumeTestInProgress)
        {
            volumeTestInfo.gameObject.SetActive(true);
        }
        else
        {
            volumeTestInfo.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Standard Unity Functions (Handling Input)
    // Update is called once per frame
    void Update ()
    {
        // Deal with (un)muting our looping audio and music items
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            #region Toggle Mute Looping Audio (number keys)
            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                UpdateLoopingInfo(0, true);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                UpdateLoopingInfo(1, true);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                UpdateLoopingInfo(2, true);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha4))
            {
                UpdateLoopingInfo(3, true);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha5))
            {
                UpdateLoopingInfo(4, true);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha6))
            {
                UpdateMusicInfo(0, true);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha7))
            {
                UpdateMusicInfo(1, true);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha8))
            {
                UpdateMusicInfo(2, true);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha9))
            {
                UpdateMusicInfo(3, true);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha0))
            {
                UpdateMusicInfo(4, true);
            }
            #endregion
        }
        else
        {// Deal with playing/stopping our looping audio and music items
            #region Toggle Looping Audio (number keys)
            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                UpdateLoopingInfo(0);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                UpdateLoopingInfo(1);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                UpdateLoopingInfo(2);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha4))
            {
                UpdateLoopingInfo(3);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha5))
            {
                UpdateLoopingInfo(4);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha6))
            {
                UpdateMusicInfo(0);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha7))
            {
                UpdateMusicInfo(1);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha8))
            {
                UpdateMusicInfo(2);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha9))
            {
                UpdateMusicInfo(3);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha0))
            {
                UpdateMusicInfo(4);
            }
            #endregion
        }

        #region Pause Handler
        // Deal with Pause/Unpause
        if (Input.GetKeyUp(KeyCode.K))
        {
            if (audioPaused)
            {
                audioPaused = false;
                AudioManager.UnpauseAllAudio();
            }
            else
            {
                audioPaused = true;
                AudioManager.PauseAllAudio();
            }
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            if (musicPaused)
            {
                musicPaused = false;
                AudioManager.UnpauseAllMusic();
            }
            else
            {
                musicPaused = true;
                AudioManager.PauseAllMusic();
            }


        }
        #endregion

        #region Misc Input Handling
        if (Input.GetKeyUp(KeyCode.P))
        {
            AudioManager.PlayAudio("Test");
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            AudioManager.StopAllAudio();
            loopingInfo = new LoopingInfo[5]; // Quick and dirty, reset all 'playing' flags to false for all of our audio;
        }
        else if (Input.GetKeyUp(KeyCode.X))
        {
            AudioManager.StopAllMusic();
            musicInfo = new LoopingInfo[5]; // Quick and dirty, reset all 'playing' flags to false for all of our audio;
        }
        else if (Input.GetKeyUp(KeyCode.M))
        {
            if (!everythingMuted)
            {
                if (allAudioMuted)
                {
                    allAudioMuted = false;
                    AudioManager.UnmuteAllAudio();
                    for (int i = 0; i < loopingInfo.Length; i++)
                    {
                        loopingInfo[i].muted = false;
                    }
                }
                else
                {
                    allAudioMuted = true;
                    AudioManager.MuteAllAudio();
                    for (int i = 0; i < loopingInfo.Length; i++)
                    {
                        loopingInfo[i].muted = true;
                    }
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.N))
        {
            if (!everythingMuted)
            {
                if (allMusicMuted)
                {
                    allMusicMuted = false;
                    AudioManager.UnmuteAllMusic();
                    for (int i = 0; i < musicInfo.Length; i++)
                    {
                        musicInfo[i].muted = false;
                    }
                }
                else
                {
                    allMusicMuted = true;
                    AudioManager.MuteAllMusic();
                    for (int i = 0; i < musicInfo.Length; i++)
                    {
                        musicInfo[i].muted = true;
                    }
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.Z))
        {
            if (everythingMuted)
            {
                everythingMuted = false;
                allMusicMuted = false;
                allAudioMuted = false;
                AudioManager.UnmuteEverything();
                for (int i = 0; i < musicInfo.Length; i++)
                {
                    musicInfo[i].muted = false;
                }
                for (int i = 0; i < loopingInfo.Length; i++)
                {
                    loopingInfo[i].muted = false;
                }
            }
            else
            {
                everythingMuted = true;
                AudioManager.MuteEverything();
                for (int i = 0; i < musicInfo.Length; i++)
                {
                    musicInfo[i].muted = true;
                }
                for (int i = 0; i < loopingInfo.Length; i++)
                {
                    loopingInfo[i].muted = true;
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            // Adjust volume test
            StartCoroutine(AdjustVolumeTest());
        }
        #endregion

        // Update all of our UI... not very efficient, but, fine for a demo
        UpdateLoopingUICollection();
        UpdateMusicUICollection();
        UpdateSourceInfo();
        UpdateVolumeTestInfo();

        SceneUtils.CheckForQuit();
    }
    #endregion
}
