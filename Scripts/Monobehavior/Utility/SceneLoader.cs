// #define FULLLOG
#undef FULLOG

using System.Collections;
using System.Collections.Generic;
#if FULLLOG
using Goldraven.Generic;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Goldraven.Utility {

    public class SceneLoader : MonoBehaviour {

        /*
         *       cmcb 2019/06/11
         *
         *       Production
         *
         *       Provide a consistent means of loading a new scene, with transition.
         *
         */

        #region Properties
        // Constants
        public const int LoadWait = 1; // seconds

        // Static

        // Singleton
        public static SceneLoader only { get; private set; }
        public static bool exists { get; private set; }

        // Local

        // Inspector entries
        public string TransitionSceneName;
        public string PlayerSceneName; // main camera is the important part here
        // player controllers assume it doesn't change
        // so it needs to load on the PRIMARY not the TRANSITION scene.

        // Status properties

        // Internal
        private string PrimarySceneName;
        private bool UseTransitionScene = false;
        private Queue<string> loadqueue = new Queue<string> ();
        private bool QueueActive = false;

        #endregion

        #region Methods

        // Unity methods

        void Awake () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif

            // singleton
            exists = false;
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
            exists = true;

            // local init

            // set internal variables
            PrimarySceneName = SceneManager.GetActiveScene ().name; // fallback should be replaced by loadlevel or loadoverlay

        }

        // local methods

        public void BeginTransition (bool useScene) {
#if FULLLOG
            MoreDebug.Log (".");
#endif

            loadqueue.Enqueue (TransitionSceneName);
            UseTransitionScene = useScene;
            StartCoroutine (doStartProcessingQueue (useScene));

        }

        public void LoadLevel (string levelname, bool primary = false) {
#if FULLLOG
            MoreDebug.Log ("Level: " + levelname);
#endif

            loadqueue.Enqueue (levelname);
            if (primary) {
                PrimarySceneName = levelname;
            }

        }

        public void LoadOverlay (string overlayname, bool primary = false) {
#if FULLLOG
            MoreDebug.Log ("Overlay: " + overlayname);
#endif

            loadqueue.Enqueue (overlayname);
            if (primary) {
                PrimarySceneName = overlayname;
            }

        }

        public void EndTransition () {
#if FULLLOG
            MoreDebug.Log (".");
#endif

            StartCoroutine (doEndTransition ());
        }

        private IEnumerator doStartProcessingQueue (bool UseTrans) {
#if FULLLOG
            MoreDebug.Log ("Starting scene loader queue.");
#endif

            QueueActive = true;
            yield return new WaitForSecondsRealtime (LoadWait); // for queue to fill
            if (PauseGame.exists) {
                while (PauseGame.only.paused) {
#if FULLLOG
                    MoreDebug.Log ("Holding for game pause.");
#endif
                    yield return new WaitForSecondsRealtime (LoadWait);
                }
            }
            if (UseTrans) {
                string transname = loadqueue.Dequeue ();
                AsyncOperation ao = SceneManager.LoadSceneAsync (transname, LoadSceneMode.Single);
                do {
#if FULLLOG
                    MoreDebug.Log ("Holding for transition to load.");
#endif
                    yield return new WaitForSecondsRealtime (LoadWait);
                } while (ao.progress < 0.9f);
            } else {
                // else throw away transname
                if (loadqueue.Count > 0) {
                    loadqueue.Dequeue ();
                }
            }
            while (loadqueue.Count > 0) {
                string scenename = loadqueue.Dequeue ();
                AsyncOperation ao = SceneManager.LoadSceneAsync (scenename, LoadSceneMode.Additive);
                do {
#if FULLLOG
                    MoreDebug.Log ("Holding for scene load.");
#endif
                    yield return new WaitForSecondsRealtime (LoadWait);
                    // } while (ao.progress < 0.9f);
                }
                while (!ao.isDone);
            }
            QueueActive = false;

        }

        public IEnumerator doEndTransition () {
#if FULLLOG
            MoreDebug.Log (".");
#endif
            do {
                yield return new WaitForSecondsRealtime (LoadWait);
            } while (QueueActive);

#if FULLLOG
            MoreDebug.Log ("Setting active scene:" + PrimarySceneName);
#endif

            SceneManager.SetActiveScene (SceneManager.GetSceneByName (PrimarySceneName));
            if (UseTransitionScene) {
#if FULLLOG
                MoreDebug.Log ("Unloading transition scene:" + TransitionSceneName);
#endif
                yield return null;
                SceneManager.UnloadSceneAsync (TransitionSceneName);
            }
            if (PlayerSceneName.Length > 0) {
                SceneManager.LoadSceneAsync (PlayerSceneName, LoadSceneMode.Additive);
            }
        }

        #endregion

    }
}