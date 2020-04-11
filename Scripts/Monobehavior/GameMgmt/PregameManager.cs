// #define FULLLOG
#undef FULLOG

using Goldraven.Generic;
using Goldraven.Gui;
using Goldraven.Interfaces;
using Goldraven.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;
using XenStudio.UI;

namespace Goldraven.Mgmt {

    public class PregameManager : MonoBehaviour, iUniRegister {

        /*
         *       cmcb 2019/04/06
         *
         *       Provide  a consistent interface for starting a game
         *       and managing the menu UI within it.
         *
         *       It starts automatically with Start()
         *       Call BeginGame() to shut this down and move on to something playable.
         *
         *       Deliberately no UI except modal displays and/or splash screens.
         *       If you have choices, make them elsewhere and add a hook here.
         *       This goes in the game start scene only.
         *
         *       Assumes but does not test for QuitClean and PauseGame.
         */

        #region Properties
        // Constants

        // Singleton
        public static PregameManager only { get; private set; }
        public static bool exists { get; private set; }
        public static bool pregameState { get; private set; }
        public static string InitialScene { get; private set; }

        // Local
        public MainMenuTree PregameMenus;
        // public PauseMenuTree PauseMenus;
        public bool ForceQuit = false;
        [TextArea]
        public string DefaultHeader = "Wander";
        [TextArea]
        public string StartingInfo = "";

        #endregion

        #region Methods

        // Unity methods
        void Awake () {
            // singleton
            exists = false;
            if (only == null) {
                DontDestroyOnLoad (gameObject);
                only = this;
            } else if (only != this) {
                Destroy (gameObject);
            }
            exists = true;
            // local init
#if FULLLOG
            MoreDebug.Log ("singleton, keep.");
#endif
            pregameState = false;
        }

        void Start () {
#if FULLLOG
            MoreDebug.Log (MoreDebug.splatRow);
#endif
            RegisterSelf ();
            InitialScene = SceneManager.GetActiveScene ().name;
            EasyMessageBox.Show (StartingInfo,
                messageBoxTitle : DefaultHeader,
                button1Action: () => { BeginMenus (); },
                multipleCallBehaviour : MultipleCallBehaviours.Queue);
        }

        // local methods

        public void CleanUp () {
#if FULLLOG
            MoreDebug.Log (MoreDebug.splatRow);
#endif
            SceneLoader.only.BeginTransition (true);
            SceneLoader.only.PlayerSceneName = "";
            SceneLoader.only.LoadLevel (InitialScene, true);
            SceneLoader.only.EndTransition ();
            BeginMenus (ForceQuit);
        }

        public void BeginMenus (bool quitonly = false) {
#if FULLLOG
            MoreDebug.Log (".");
#endif
            pregameState = true;

            PregameMenus.StartMenus (quitonly);
            // PauseGame.only.beginPlay();
        }

        void PlayTheTutorial () {
#if FULLLOG
            MoreDebug.Log (".");
#endif
            pregameState = false;

        }

        public void PlayACampaign (string campaignname) {
#if FULLLOG
            MoreDebug.Log (campaignname);
#endif
            pregameState = false;
            CampaignManager.BeginCampaign (campaignname);
        }

        public void PlayALevel (string primaryscenename) {
            //todo: Trigger this at listmenumanager instead of each individual element
#if FULLLOG
            MoreDebug.Log ("Level: " + primaryscenename);
#endif
            pregameState = false;
            SceneLoader.only.BeginTransition (true);
            SceneLoader.only.LoadLevel (primaryscenename, true);
        }

        void LoadAGame (string savegamename) {
#if FULLLOG
            MoreDebug.Log (".");
#endif
            pregameState = false;

        }

        public bool RegisterSelf () {
            return Registry.only.Register (this);

        }

        #endregion

    }
}