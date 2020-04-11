using UnityEngine;
using UnityEngine.Events;

namespace Goldraven.Utility {

    public class QuitClean : MonoBehaviour {

        /*
         *       cmcb 2018/04/27
         *
         *       Production
         *
         *       Provide  a consistent interface for ending a scene
         *       That can be toggled between a force quit and
         *       a graceful return to some other scene.
         *
         *       Call FinishScene() for a successful (or neutral) quit
         *       Call FailScene() for a defeat
         *
         *       Deliberately no UI; if you want to cancel a quit, do it before it gets here.
         */

        // properties
        // Singleton
        public static QuitClean only { get; private set; }
        public static bool exists { get; private set; }

        // Local
        public bool ForceKillGame = false;

        // Events
        public UnityEvent QuitWithSuccessEvent, QuitWithFailureEvent;

        // methods
        // Unity methods
        public void Awake () {
            // singleton
            exists = false;
            if (only == null) {
                DontDestroyOnLoad (gameObject);
                only = this;
            } else if (only != this) {
                Destroy (this);
            }
            exists = true;

            // build events
            if (QuitWithSuccessEvent == null) QuitWithSuccessEvent = new UnityEvent ();
            if (QuitWithFailureEvent == null) QuitWithFailureEvent = new UnityEvent ();

            // local init
        }

        public void Start () {
            QuitWithSuccessEvent.AddListener (LogQuitSuccess);
            QuitWithFailureEvent.AddListener (LogQuitFailure);
            //QuitWithSuccessEvent.AddListener(CheckForceQuit);
            //QuitWithFailureEvent.AddListener(CheckForceQuit);
            // Officially, order of event execution is undefined.
            // In reality, it goes top to bottom with inspector events first.
            // Neither of these is what I want for CheckForceQuit, so I'm moving the check.
        }

        // Event listeners
        private void LogQuitSuccess () {
            Debug.Log ("QuitClean:Success quit triggered.");
        }

        private void LogQuitFailure () {
            Debug.Log ("QuitClean:Failure quit triggered.");
        }

        private void CheckForceQuit () {
            if (ForceKillGame == true) {
                QuitGame ();
            }
        }

        // Event triggers
        public void FinishScene () {
            CheckForceQuit (); // Bypasses all events and just kills the game.
            QuitWithSuccessEvent.Invoke ();
        }

        public void FailScene () {
            CheckForceQuit (); // Bypasses all events and just kills the game.
            QuitWithFailureEvent.Invoke ();
        }

        public static void QuitGame () {
            Application.Quit ();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

    }
}