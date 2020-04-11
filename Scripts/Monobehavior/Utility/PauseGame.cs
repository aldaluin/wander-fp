#define CURSOR_LOCK
//  CURSOR_LOCK here should agree with vShooterMeleeInput/Input cursor settings
// #undef CURSOR_LOCK

// #define FULLLOG
#undef FULLOG

using System;
using Goldraven.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Goldraven.Utility {

    [Serializable]
    public struct PauseTrigger {
        public string Keyname;
        public string Pausename;
        public UnityEvent GamePauseEvent, GameResumeEvent, ManualPauseEvent, ManualResumeEvent;
    }

    public class PauseGame : MonoBehaviour {
        /*
         * cmcb  2017/11/13
         * Production
         *
         * cmcb 2018/01/09
         * Added player reference.  Player must be active when start is called.
         *
         * cmcb 2018/02/07
         * Changed delegates to unity events
         * Made script a non-serializable singleton
         *
         * cmcb 2018/04/27
         * Now requires a quitclean
         *
         * cmcb 2019/02/27
         * Added code to allow pre-play and post-play cursor control.
         * Will break earlier uses because it requires explicit calls to beginPlay and endPlay to work.
         *
         * cmcb 2019/06/27
         * Split pause and resume events into two (each)
         *
         * cmcb 2019/10/23
         * Added multiple pause types
         */

        // Properties
        // Singleton
        public static PauseGame only { get; private set; }
        public static bool exists { get { return (only != null); } }

        // Local
        private bool inaPause = false;
        public bool paused { get { return inaPause; } private set { inaPause = value; } }
        private bool inPlay = false;
        private bool manual = false;
        public bool playing { get { return inPlay; } private set { inPlay = value; } }
        private int queuestatus = 0;
        private GameObject Player = null;
        public PauseTrigger[] PauseTriggers;
        private int TriggerIndex;
        public UnityEvent PlayBeginEvent, PlayEndEvent;

        // Methods
        // Unity methods

        void Awake () {
            // implement singleton
            if (only == null) {
                DontDestroyOnLoad (gameObject);
                only = this;
#if FULLLOG
                MoreDebug.Log ("Singleton created.");
#endif
            } else if (only != this) {
                Destroy (this);
#if FULLLOG
                MoreDebug.Log ("Duplicate singleton destroyed.");
#endif
            }
        }

        void Start () {
#if FULLLOG
            MoreDebug.Log (" ");
#endif
            if (PlayBeginEvent == null) {
                PlayBeginEvent = new UnityEvent ();
            }
            if (PlayEndEvent == null) {
                PlayEndEvent = new UnityEvent ();
            }
            if (PauseTriggers.Length == 0) {
                Debug.LogError ("FATAL:There are no pause triggers set.");
            }
            for (int cc = 0; cc < PauseTriggers.Length; cc++) {

                if (PauseTriggers[cc].GamePauseEvent == null) {
                    PauseTriggers[cc].GamePauseEvent = new UnityEvent ();
                }
                if (PauseTriggers[cc].GameResumeEvent == null) {
                    PauseTriggers[cc].GameResumeEvent = new UnityEvent ();
                }
                if (PauseTriggers[cc].ManualPauseEvent == null) {
                    PauseTriggers[cc].ManualPauseEvent = new UnityEvent ();
                }
                if (PauseTriggers[cc].ManualResumeEvent == null) {
                    PauseTriggers[cc].ManualResumeEvent = new UnityEvent ();
                }
            }
            RecheckPlayer ();
            setPauseMode (); // before play begins...
        }

        void Update () {
            for (int cc = 0; cc < PauseTriggers.Length; cc++) {

                if (Input.GetButtonDown (PauseTriggers[cc].Keyname) && playing) {
                    if (paused) {
                        if (TriggerIndex != cc) {
                            continue; //for loop
                        } // else...

#if FULLLOG
                        MoreDebug.Log ("Pause button hit - resuming.");
#endif
                        // TriggerIndex = -1;
                        manual = true;
                        forceResume ();
                    } else {
#if FULLLOG
                        MoreDebug.Log ("Pause button hit - pausing.");
#endif
                        // TriggerIndex = cc;
                        manual = true;
                        forcePause (cc);
                    }
                    break; // for loop
                }
            }
        }

        // Local methods

        private void setPlayMode () {
#if FULLLOG
            MoreDebug.Log ("Setting to play mode.");
#endif
#if CURSOR_LOCK
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            CancelInvoke ();
            ShowPlayer ();
#endif
            Time.timeScale = 1; // time is normal
        }

        private void setPauseMode () {
#if FULLLOG
            MoreDebug.Log ("Setting to pause mode.");
#endif
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0; // time stopped
#if CURSOR_LOCK
            Invoke ("HidePlayer", 0.5f); // delay the call for a half second
#endif
        }

        public void beginPlay () {
#if FULLLOG
            MoreDebug.Log (" ");
#endif
            if (!playing) {
                playing = true;
                paused = false;
                TriggerIndex = -1;
                setPlayMode ();
                PlayBeginEvent.Invoke ();
            }
        }

        public void endPlay () {
#if FULLLOG
            MoreDebug.Log (" ");
#endif
            if (playing) {
                playing = false;
                paused = false;
                queuestatus = 0;
                TriggerIndex = -1;
                setPauseMode ();
                PlayEndEvent.Invoke ();
            }
        }

        public void resetPlay () {
#if FULLLOG
            MoreDebug.Log (" ");
#endif
            if (playing) {
                Debug.LogError ("PauseGame: Tried to reset while playing.");
            } else {
                RecheckPlayer ();
            }

        }

        public void forcePause (int counter) {
#if FULLLOG
            MoreDebug.Log (" ");
#endif
            if (!playing) return;
#if FULLLOG
            MoreDebug.Log ("Queuestatus: " + queuestatus);
#endif
            if (queuestatus == 0) {
                TriggerIndex = counter;
                paused = true;
                setPauseMode ();
                if (manual) {
                    PauseTriggers[TriggerIndex].ManualPauseEvent.Invoke ();
                    manual = false;
                }
                PauseTriggers[TriggerIndex].GamePauseEvent.Invoke ();
            }
            queuestatus = 9999;
        }

        public void forceResume () {
#if FULLLOG
            MoreDebug.Log (" ");
#endif
            if (!(paused && playing)) return;
#if FULLLOG
            MoreDebug.Log ("Queuestatus: " + queuestatus);
#endif
            setPlayMode ();
            queuestatus = 0;
            if (manual) {
                PauseTriggers[TriggerIndex].ManualResumeEvent.Invoke ();
                manual = false;
            }
            PauseTriggers[TriggerIndex].GameResumeEvent.Invoke ();
            TriggerIndex = -1;
            paused = false;
        }

        public void queuePause (int counter) {
#if FULLLOG
            MoreDebug.Log (" ");
#endif
            if (!playing) return;
#if FULLLOG
            MoreDebug.Log ("Queuestatus: " + queuestatus);
#endif
            if (queuestatus > 0) {
                queuestatus++;
            } else { // queuestatus == 0
                paused = true;
                TriggerIndex = counter;
                setPauseMode ();
                PauseTriggers[TriggerIndex].GamePauseEvent.Invoke ();
                queuestatus++;
            }
        }

        public void queuePause () {
            queuePause (0);
        }
        public void queueResume () {
#if FULLLOG
            MoreDebug.Log (" ");
#endif
            if (!(paused && playing)) {
                return;
            }
#if FULLLOG
            MoreDebug.Log ("Queuestatus: " + queuestatus);
#endif
            if (queuestatus > 1) {
                queuestatus--;
            } else { // queuestatus == 1
                setPlayMode ();
                paused = false;
                PauseTriggers[TriggerIndex].GameResumeEvent.Invoke ();
                TriggerIndex = -1;
                queuestatus = 0;
            }
        }

        public bool IsQueueEmpty () {
            return (queuestatus == 0);
        }

        public void RecheckPlayer () {
#if FULLLOG
            MoreDebug.Log (" ");
#endif
            // may be needed when loading a level
            GameObject[] Players = GameObject.FindGameObjectsWithTag ("Player");
            if (Players.Length == 0) {
                Debug.LogWarning ("Game pauser does not have player reference. Not changed.");
            } else if (Players.Length > 1) {
                Debug.LogError ("Game pauser has multiple player references. Not changed.");
            } else {
#if FULLLOG
                MoreDebug.Log ("Player found.");
#endif
                Player = Players[0];
            }

        }
        private void HidePlayer () {
            if (Player != null) {
                Player.SetActive (false);
            }
        }

        private void ShowPlayer () {
            if (Player != null) {
                Player.SetActive (true);
            }

        }

    }
}