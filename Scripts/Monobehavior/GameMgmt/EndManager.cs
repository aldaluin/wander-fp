// #define FULLLOG
#undef FULLOG

using System;
using Goldraven.Generic;
using Goldraven.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Goldraven.Mgmt {

    [Serializable]
    public class EndManager : MonoBehaviour, iUniRegister {

        /*
         *       cmcb 2019/05/03
         *       Status: Production  2019/07/11
         *
         *       Provide  a consistent interface defining a condition to end a manager.
         *
         *       In general, to trigger completion, any criteria may be met.   todo: verify the code
         *
         *
         *       Deliberately no UI
         *       This goes... where??
         */

        #region Properties

        // Constants

        // Local

        public Criterion[] EndCriteria;
        public UnityEvent OnEnd;
        public bool ForceEnd = false;
        private bool _active = false;
        public bool isActive { get { return _active; } set { setActive (value); } }

        #endregion

        #region Methods

        // Unity methods
        void Awake () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif
            // create events
            if (OnEnd == null) {
                OnEnd = new UnityEvent ();
            }
            // Warn if there's no criteria (mission can't finish)
            if (EndCriteria.Length == 0) {
                Debug.LogWarning ("EndManager: No criteria set for this end.");
            }
            isActive = false;
        }

        void Start () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif

            // this logic forces immediate goal finish when any succeed or fail criterion is hit
            foreach (Criterion cr in EndCriteria) {
                cr.init (ExecuteEnd);
            }
            RegisterSelf ();
        }

        // local methods

        void CleanUp () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif

            foreach (Criterion cr in EndCriteria) {
                cr.reset ();
            }
            isActive = false;
        }

        void setActive (bool isTrue) {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name + " -- Active = " + isTrue);
#endif
            foreach (Criterion cr in EndCriteria) {
                cr.recording = isTrue;
            }
            _active = isTrue;

        }

        public void IncrementEndCriterion (int critIdx) {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif
            EndCriteria[critIdx].Increment ();
        }

        void ExecuteEnd () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif
            // do stuff
            OnEnd.Invoke ();
            CleanUp ();
        }

        public bool RegisterSelf () {
            return Registry.only.Register (this);

        }

        #endregion

    }

}