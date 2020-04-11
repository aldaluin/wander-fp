// #define FULLLOG
#undef FULLOG

using Goldraven.Generic;
using Goldraven.Interfaces;
using Goldraven.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Goldraven.Mgmt {

    [System.Serializable]
    public class LocationEvent : UnityEvent<int> { }

    public class LevelManager : MonoBehaviour, iUniRegister {

        /*
         *       cmcb 2019/02/26
         *       Status: production, enhancement
         *
         *       Provide  a consistent container for a map and a set of victory and failure conditions
         *       (an operation) to play on it.
         *
         *
         *       Deliberately no UI except modal displays and/or splash screens.
         *       This goes only in the (non-overlay) scene it manages.
         *       The scene should have nothing else.  Everything is handled by overlay scenes.
         */

        #region Properties

        // Constants

        public const string newline = "\r\n";

        // Singleton
        public static LevelManager only { get; private set; }
        public static bool exists { get; private set; }

        // Local

        [HideInInspector]
        public PregameManager Game = null;
        [HideInInspector]
        public CampaignManager Campaign = null;
        [TextArea]
        public string Description;
        public string MapOverlayName, OperationOverlayName, DefaultPlayerName;

        public string[] MapOverlayNames; // can be bling todo: look into map extensions
        public UnityEvent OnBeginLevel, OnEndLevel;
        [HideInInspector]
        public LocationEvent OnGetLocation;
        public bool LocationActive = false;
        public bool EndOfLocations = false;

        // Internal
        [HideInInspector]
        public GameObject NextLocation = null;
        private ActivityCounter operationcounts = new ActivityCounter (2, new string[] {
            ActivityCounter.losestring, ActivityCounter.winstring
        }); // 0=lose, 1=win

        #endregion

        #region Methods

        // Unity methods
        void Awake () {
#if FULLLOG
            MoreDebug.Log ("singleton, nokeep.");
#endif
            // singleton
            exists = false;
            if (only == null) {
                // DontDestroyOnLoad (gameObject);
                only = this;
#if FULLLOG
                MoreDebug.Log ("Singleton created. qqq");
#endif
            } else if (only != this) {
                MoreDebug.LogError ("More than one level manager found.");
                Destroy (gameObject);
#if FULLLOG
                MoreDebug.Log ("Duplicate singleton destroyed.");
#endif
            }
            exists = true;

            // local init

            // Check for a pregame manager
            if (PregameManager.only != null) { // Created but maybe not awake yet
                Game = PregameManager.only;
            }
            // Check for a campaign manager
            if ((CampaignManager.count > 0) && (CampaignManager.campaign != null)) {
                Campaign = CampaignManager.campaign;
            }
            // Create events
            if (OnBeginLevel == null) {
                OnBeginLevel = new UnityEvent ();
            }
            if (OnEndLevel == null) {
                OnEndLevel = new UnityEvent ();
            }
            if (OnGetLocation == null) {
                OnGetLocation = new LocationEvent ();
            }

            // done with Awake()
        }

        void Start () {
#if FULLLOG
            MoreDebug.Log ("Level manager Start() qqq");
#endif
            RegisterSelf ();
            // Subscribe to quitclean
            if (QuitClean.exists) {
                QuitClean.only.QuitWithFailureEvent.AddListener (LoseLevel);
                QuitClean.only.QuitWithSuccessEvent.AddListener (WinLevel);
            }

            // Add overlays

            if (Campaign != null) {
                Campaign.StartLevel ();
            } else {
                SceneLoader.only.PlayerSceneName = DefaultPlayerName;
                PauseGame.only.beginPlay ();
                if (this.gameObject.scene == UnityEngine.SceneManagement.SceneManager.GetActiveScene ()) {
                    // transition has no sceneloader, so running standalone
                    SceneLoader.only.BeginTransition (false);
                } else {
                    // running solo level from pregame
                }
            }
            SceneLoader.only.LoadOverlay (MapOverlayName, true);
            SceneLoader.only.LoadOverlay (OperationOverlayName);
            SceneLoader.only.EndTransition ();

            // Announce the start of the level
            OnBeginLevel.Invoke ();
        }

        void OnDestroy () {
#if FULLLOG
            MoreDebug.LogError ("Verify this isn't a duplicate!");
#endif
            only = null;
            exists = false;
        }

        // local methods

        public void WinLevel () {
#if FULLLOG
            MoreDebug.Log (".");
#endif
            OnEndLevel.Invoke ();
            if (Campaign == null) {
                WrapUp ();
            } else {
                Campaign.WinLevel ();
            }

        }

        public void LoseLevel () {
#if FULLLOG
            MoreDebug.Log (".");
#endif
            OnEndLevel.Invoke ();
            if (Campaign == null) {
                WrapUp ();
            } else {
                Campaign.LoseLevel ();
            }

        }

        public void WrapUp () {
            PauseGame.only.endPlay ();
            if (PregameManager.exists) {
                PregameManager.only.CleanUp ();
            } else {
                QuitClean.QuitGame ();
            }

        }

        public void EndLevel () {
            // NOT a wrap-up method -- calls either winlevel or loselevel
#if FULLLOG
            MoreDebug.Log (".");
#endif
            if (operationcounts.MaxIdx () == 0) { // loses > wins
                LoseLevel ();
            } else {
                WinLevel ();
            }
        }

        public void IncrementOperations () {
#if FULLLOG
            MoreDebug.Log (".");
#endif

            operationcounts.Increment ();
        }

        public void WinAnOperation () {
#if FULLLOG
            MoreDebug.Log (".");
#endif

            operationcounts.GroupInc (ActivityCounter.winstring);
        }

        public void LoseAnOperation () {
#if FULLLOG
            MoreDebug.Log (".");
#endif

            operationcounts.GroupInc (ActivityCounter.losestring);
        }

        /*
        public void WinAnOperationWeighted(int weight){
            #if FULLLOG
            MoreDebug.Log(".");
            #endif
            operationcounts.GroupInc(ActivityCounter.winstring, weight);
        }

        public void LoseAnOperationWeighted(int weight){
            #if FULLLOG
            MoreDebug.Log(".");
            #endif
            operationcounts.GroupInc(ActivityCounter.losestring, weight);
        }
        */

        public void DecrementOperations () {
#if FULLLOG
            MoreDebug.Log (".");
#endif

            operationcounts.Decrement ();

#if FULLLOG
            if (!operationcounts.isValid ()) {
                MoreDebug.LogError ("Op count invalid");
            }
#endif

            if (operationcounts.isDone ()) {
                EndLevel ();
            }
        }

        public void GetLocation (int gp) {
#if FULLLOG
            MoreDebug.Log (".");
#endif
            OnGetLocation.Invoke (gp);
#if FULLLOG
            if (NextLocation == null) {
                MoreDebug.Log ("Next location is null.");
            } else {
                MoreDebug.Log (NextLocation.ToString ());
            }
#endif
            // return NextLocation;
        }
        public bool RegisterSelf () {
            return Registry.only.Register (this);

        }

        #endregion

    }
}