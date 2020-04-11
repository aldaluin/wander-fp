// #define FULLLOG
#undef FULLOG

using System;
using Goldraven.Generic;
using Goldraven.Interfaces;
using Goldraven.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using XenStudio.UI;

namespace Goldraven.Mgmt {

    public class CampaignManager : MonoBehaviour, iUniRegister {

        /*
         *       cmcb 2019/02/26
         *
         *       Production -- cmcb  2019/07/11
         *
         *       Provide  a consistent interface for starting and ending a campaign
         *       and managing the level/scene transitions within it.
         *
         *
         *       Deliberately no UI except modal displays and/or splash screens.
         *       If you have choices, make them elsewhere and add a hook here.
         *       This goes in the initial scene only.
         */

        [Serializable]
        public struct LevelOrder {
            public string LevelSceneName;
            public bool ForceCampaignWin, ForceCampaignLose;
            public int NextIfWin, NextIfLose;

            public LevelOrder (string name) {
                LevelSceneName = name;
                ForceCampaignWin = false;
                ForceCampaignLose = false;
                NextIfWin = -1;
                NextIfLose = 0;
            }
        }

        #region Properties
        // Constants
        public const string newline = "\r\n";

        // Static
        private static int _count = 0;
        public static readonly int MaxCampaigns = 11;
        private static CampaignManager[] allCampaigns = new CampaignManager[MaxCampaigns];
        public static CampaignManager campaign { get; private set; }
        public static int count { get { return _count; } }
        // Local

        // Inspector entries
        [SerializeField]
        private string CampaignName = "";
        public string cname { get { return CampaignName; } }

        [TextArea]
        public string CampaignDescription, StartingInfo, SuccessInfo, FailureInfo;
        public int OperationTerseness = 2; // lower is more
        public bool ForceMissionTerseness = false;
        public string[] PlayerOverlayNames; //TODO: pick one
        public string[] CampaignOverlayNames; // campaign overlay scenes reload with each new level
        public LevelOrder[] Levels;

        public UnityEvent OnBeginCampaign, OnEndCampaign, OnWinCampaign, OnLoseCampaign;

        // Internal
        private bool _active = false;
        public bool isActive { get { return _active; } set { setActive (value); } }
        public int currentLevel { get; private set; }
        #endregion

        #region Methods

        // Unity methods
        void Awake () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
            MoreDebug.Log ("Campaign count: " + _count);
#endif

            // class init
            campaign = null;
            allCampaigns[_count] = this;
            _count++;

            // local init
            currentLevel = -1;

            // Create events
            if (OnBeginCampaign == null) {
                OnBeginCampaign = new UnityEvent ();
            }
            if (OnEndCampaign == null) {
                OnEndCampaign = new UnityEvent ();
            }
            if (OnWinCampaign == null) {
                OnWinCampaign = new UnityEvent ();
            }
            if (OnLoseCampaign == null) {
                OnLoseCampaign = new UnityEvent ();
            }
            isActive = false;
        }

        void Start () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif
            // DONT USE START()
            RegisterSelf ();
        }

        void OnEnable () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif
            StartCampaign ();

        }

        public void StartLevel () {
            // Must be called from LevelManager.Start() only
            // todo: is this needed or can it be moved to StartCampaign()?
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif

            // Add overlays
#if FULLLOG
            MoreDebug.Log ("set player scene: " + PlayerOverlayNames[0]);
#endif
            SceneLoader.only.PlayerSceneName = PlayerOverlayNames[0];
            foreach (string scenename in CampaignOverlayNames) {
#if FULLLOG
                MoreDebug.Log ("add scene: " + scenename);
#endif
                SceneLoader.only.LoadOverlay (scenename);
            }

        }

        void OnDestroy () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
            // QuitClean.QuitGame();
#endif

            // Unsub events
            OnBeginCampaign.RemoveAllListeners ();
            OnEndCampaign.RemoveAllListeners ();
            OnWinCampaign.RemoveAllListeners ();
            OnLoseCampaign.RemoveAllListeners ();
            _count--;

        }

        // local methods

        public static void BeginCampaign (string _cn_) {
            // Called by game (or pregame) manager -- entry point
#if FULLLOG
            MoreDebug.Log (_cn_ + " (" + count + ") " + MoreDebug.splatRow);
#endif

            if (count == 0) {
                MoreDebug.LogError ("No campaigns registered.");
                QuitClean.QuitGame ();
            }

            foreach (CampaignManager cm in allCampaigns) {
                if (cm == null) {
#if FULLLOG
                    MoreDebug.Log ("Null skipped.");
#endif
                    continue;
                } else if (cm.CampaignName == _cn_) {
#if FULLLOG
                    MoreDebug.Log (cm.CampaignName + " selected.");
#endif
                    campaign = cm;
                    campaign.isActive = true;
                    campaign.enabled = true;
#if FULLLOG
                    if (campaign.isActiveAndEnabled) {
                        MoreDebug.Log (campaign.CampaignName + " is active and enabled(tm).");
                    } else {
                        MoreDebug.Log (campaign.CampaignName + " is NOT active and enabled(tm).");
                    }
#endif

                } else {
#if FULLLOG
                    MoreDebug.Log (cm.CampaignName + " destroyed.");
#endif
                    Destroy (cm.gameObject);
                }
            }

        }

        public static void EndCampaign () {
            PauseGame.only.endPlay ();
            foreach (CampaignManager cm in allCampaigns) {
                if (cm == null) {
                    continue;;
                } else {
                    Destroy (cm.gameObject.transform.parent.gameObject);
                }
            }
            campaign = null;
            // return to pregame manager or quit
            if (PregameManager.exists) {
                PregameManager.only.CleanUp ();
            } else {
                QuitClean.QuitGame ();
            }

        }

        private void StartCampaign () {
            // Called when enabled
            // todo: can we just put this in OnEnable() ?
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif
            // announce the start of the campaign
            PauseGame.only.beginPlay ();
            OnBeginCampaign.Invoke ();
            // show the campaign start screen

            if (!PauseGame.exists) {
#if FULLLOG
                MoreDebug.Log ("No PauseGame -----");
#endif
            }
            MessageBoxParams parms = new MessageBoxParams ();
            parms.Message = StartingInfo;
            parms.MessageBoxTitle = CampaignName;
            parms.Button1Action = PauseGame.only.queueResume;
            parms.MultipleCallBehaviour = MultipleCallBehaviours.Queue;
            EasyMessageBox.Show (parms);
            PauseGame.only.queuePause ();

            // load the first level
            currentLevel = 0;
            LoadLevel (currentLevel);
        }

        private void LoadLevel (int levelindex) {
#if FULLLOG
            MoreDebug.Log ("level:" + levelindex + " scene:" + campaign.Levels[levelindex].LevelSceneName + MoreDebug.splatRow);
#endif
            if (!isActive) {
                MoreDebug.LogError ("Trying to run an inactive campaign!");
                return;
            }
            // Yup - load the level
            SceneLoader.only.BeginTransition (true);
            SceneLoader.only.LoadLevel (campaign.Levels[levelindex].LevelSceneName);
            // SceneManager.LoadScene(campaign.Levels [levelindex].LevelSceneName);

        }

        private void WinCampaign () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif

            if (!PauseGame.exists) {
#if FULLLOG
                MoreDebug.Log ("No PauseGame -----");
#endif
            }
            MessageBoxParams parms = new MessageBoxParams ();
            parms.Message = SuccessInfo;
            parms.MessageBoxTitle = CampaignName;
            parms.Button1Action = PauseGame.only.queueResume;
            parms.MultipleCallBehaviour = MultipleCallBehaviours.Queue;
            EasyMessageBox.Show (parms);
            PauseGame.only.queuePause ();
            OnWinCampaign.Invoke ();
            CampaignManager.EndCampaign ();
        }

        private void LoseCampaign () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif

            if (!PauseGame.exists) {
#if FULLLOG
                MoreDebug.Log ("No PauseGame -----");
#endif
            }
            MessageBoxParams parms = new MessageBoxParams ();
            parms.Message = FailureInfo;
            parms.MessageBoxTitle = CampaignName;
            parms.Button1Action = PauseGame.only.queueResume;
            parms.MultipleCallBehaviour = MultipleCallBehaviours.Queue;
            EasyMessageBox.Show (parms);
            PauseGame.only.queuePause ();
            OnLoseCampaign.Invoke ();
            CampaignManager.EndCampaign ();
        }

        private void setActive (bool isTrue) {
            _active = isTrue;
        }

        public void WinLevel () {
#if FULLLOG
            MoreDebug.Log ("Current Level: " + currentLevel);
#endif
            if (Levels[currentLevel].ForceCampaignWin) {
                WinCampaign ();
            } else {
                currentLevel = Levels[currentLevel].NextIfWin;
                LoadLevel (currentLevel);
            }
        }

        public void LoseLevel () {
#if FULLLOG
            MoreDebug.Log ("Current Level: " + currentLevel);
#endif
            if (Levels[currentLevel].ForceCampaignLose) {
                LoseCampaign ();
            } else {
                currentLevel = Levels[currentLevel].NextIfLose;
                LoadLevel (currentLevel);
            }
        }

        public bool RegisterSelf () {
            bool rtn = Registry.only.Register (this);
            return rtn;
        }

        #endregion

    }
}