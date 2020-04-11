// #define FULLLOG
#undef FULLOG

using System;
using Goldraven.Generic;
using Goldraven.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Goldraven.Mgmt {

    [Serializable]
    public class GoalManager : MonoBehaviour, iMultiRegister {

        /*
         *       cmcb 2019/03/12
         *
         *       Provide  a consistent interface defining a single play goal
         *       to be achieved or anti-goal to be avoided.
         *
         *       In general, to trigger goal completion, any criteria may be met.   todo: verify the code
         *
         *       Deliberately no UI
         *       This goes... where??
         */

        #region states

        public enum GoalState {
            neutral,
            assumewin,
            assumelose,
            realwin,
            reallose,
        }

        public static readonly string[] GoalStateStrings = { "neutral", "assume win", "assume lose", "win", "lose" };

        #endregion

        #region Properties

        // Constants

        // Local

        public Criterion[] succeedCriteria, failCriteria;
        public GoalState initialState = GoalState.neutral;
        public GoalState currentState { get; private set; }
        public UnityEvent SuccessEvent, FailureEvent;
        public bool ForceMissionSuccess = true, ForceMissionFail = true; // todo: implement false
        private bool _active = false;
        public bool isActive { get { return _active; } set { setActive (value); } }

        private int RegIdx;
        public int ListIndex { get { return RegIdx; } }

        #endregion

        #region Methods

        // Unity methods
        void Awake () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif
            // create events
            if (SuccessEvent == null) {
                SuccessEvent = new UnityEvent ();
            }
            if (FailureEvent == null) {
                FailureEvent = new UnityEvent ();
            }
            // Warn if there's no succeed criteria (goal can't be met)
            if (succeedCriteria.Length == 0) {
                Debug.LogWarning ("GoalManager: No success criteria set for this goal.");
            }
            isActive = false;
        }

        void Start () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif

            // this logic forces immediate goal finish when any succeed or fail criterion is hit
            foreach (Criterion cr in succeedCriteria) {
                cr.init (SucceedGoal);
            }
            foreach (Criterion cr in failCriteria) {
                cr.init (FailGoal);
            }
            currentState = initialState;
        }

        // local methods

        void CleanUp () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif

            foreach (Criterion cr in succeedCriteria) {
                cr.reset ();
            }
            foreach (Criterion cr in failCriteria) {
                cr.reset ();
            }
            currentState = initialState;
            // reset doesn't reset isactive
        }

        void setActive (bool isTrue) {
            foreach (Criterion cr in succeedCriteria) {
                cr.recording = isTrue;
            }
            foreach (Criterion cr in failCriteria) {
                cr.recording = isTrue;
            }
            _active = isTrue;

        }

        public void IncrementSucceedCriterion (int critIdx) {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif
            succeedCriteria[critIdx].Increment ();
        }

        public void IncrementFailCriterion (int critIdx) {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif
            failCriteria[critIdx].Increment ();
        }

        void FailGoal () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif
            // do stuff
            currentState = GoalState.reallose;
            FailureEvent.Invoke ();
            CleanUp ();
        }

        void SucceedGoal () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif
            // do stuff
            currentState = GoalState.realwin;
            SuccessEvent.Invoke ();
            CleanUp ();
        }
        public bool RegisterSelf () {
            bool rtn = Registry.only.Register (this, out RegIdx);
            return rtn;
        }

        #endregion

    }

}