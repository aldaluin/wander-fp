// #define FULLLOG
#undef FULLOG

using Goldraven.Generic;
using Goldraven.Interfaces;
using Goldraven.Utility;
using UnityEngine;
using UnityEngine.Events;
using XenStudio.UI;

namespace Goldraven.Mgmt {

    public class MissionManager : MonoBehaviour, iMultiRegister {

        /*
         *       cmcb 2019/03/12
         *       Status: development
         *
         *       Provide  a consistent interface defining victory condition(s)
         *       to be achieved and defeat condition(s) to be avoided for a
         *       single scenario.
         *
         *       In general, to trigger mission completion, any goal may be met.   todo: verify the code
         *
         *       Missions are independent of each other
         *       Mission success or failure is binary
         *
         *       Deliberately no UI
         *       This goes... where??
         */

        #region Properties

        // Constants

        // Local

        public string MissionTitle = "Mission";
        public bool StartOnLoad = true;
        [TextArea, SerializeField]
        public string MissionDescription = "";
        [TextArea, SerializeField]
        public string StartingInfo = "";
        [TextArea, SerializeField]
        public string SuccessInfo = "";
        [TextArea, SerializeField]
        public string FailureInfo = "";
        public int InfoPriority = 3; // 3 = normal priority
        public GoalManager[] goals;
        // float successweight, failureweight   [0...1]
        private GoalManager.GoalState initialState;
        public GoalManager.GoalState currentState { get; private set; }
        public string CurrentLevel = "";
        public UnityEvent BeginEvent, SuccessEvent, FailureEvent;

        private int goalCount = 0, antigoalCount = 0; //todo: Implement
        private bool _active = false;
        public bool isActive { get { return _active; } set { setActive (value); } }
        private OperationManager operation = null;
        private int RegIdx;
        public int ListIndex { get { return RegIdx; } }

        #endregion

        #region Methods

        // Unity methods
        void Awake () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif
            // Create events
            if (BeginEvent == null) {
                BeginEvent = new UnityEvent ();
            }
            if (SuccessEvent == null) {
                SuccessEvent = new UnityEvent ();
            }
            if (FailureEvent == null) {
                FailureEvent = new UnityEvent ();
            }
            isActive = false;
        }

        void Start () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif
            RegisterSelf ();
            int goalbias = 0;
            foreach (GoalManager goal in goals) {
                goal.SuccessEvent.AddListener (IncrementSuccess);
                goal.FailureEvent.AddListener (IncrementFailure);
                if (goal.ForceMissionSuccess) goal.SuccessEvent.AddListener (SucceedMission);
                if (goal.ForceMissionFail) goal.FailureEvent.AddListener (FailMission);
                if (goal.initialState == GoalManager.GoalState.assumewin) goalbias++;
                else if (goal.initialState == GoalManager.GoalState.assumelose) goalbias--;
            }
            if (goalbias < 0) initialState = GoalManager.GoalState.assumelose;
            else if (goalbias > 0) initialState = GoalManager.GoalState.assumewin;
            else initialState = GoalManager.GoalState.neutral;
            currentState = initialState;
        }
        // local methods

        void Reset () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif
            // clear flags and counters
            // state persists until next begin
            goalCount = 0;
            antigoalCount = 0;
            isActive = false;
        }

        private void setActive (bool isTrue) {
            foreach (GoalManager goal in goals) {
                goal.isActive = isTrue;
            }
            _active = isTrue;
        }

        public void SetOperation (OperationManager op) {
            if (operation == null) {
                operation = op;
            } else {
                Debug.LogError ("Mission is trying to register multiple ops.");
            }
        }

        public void ClearOperation () {
            operation = null;
        }

        public void BeginMission () {
            // must be called after pausegame.beginplay()
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif

            if (!PauseGame.exists) {
#if FULLLOG
                MoreDebug.Log ("No PauseGame -----");
#endif
            }
            if (InfoPriority >= operation.MissionTerseness) {
                MessageBoxParams parms = new MessageBoxParams ();
                parms.Message = StartingInfo;
                parms.MessageBoxTitle = MissionTitle;
                parms.Button1Action = PauseGame.only.queueResume;
                parms.MultipleCallBehaviour = MultipleCallBehaviours.Queue;
                EasyMessageBox.Show (parms);
                PauseGame.only.queuePause ();
            }

            // reset the goal state
            currentState = initialState;
            isActive = true;
            CurrentLevel = Registry.only.GetObject<LevelManager> ().MapOverlayName;
            operation.StartMission ();
            // set flags
            BeginEvent.Invoke ();
        }

        void IncrementSuccess () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif
            if (!isActive) {
                return;
            }
#if FULLLOG
            MoreDebug.Log ("real");
#endif
            goalCount++;
        }

        void IncrementFailure () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif
            if (!isActive) {
                return;
            }
#if FULLLOG
            MoreDebug.Log ("real");
#endif
            antigoalCount++;
        }

        void FailMission () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif
            if (!isActive) {
                return;
            }
#if FULLLOG
            MoreDebug.Log ("real.");
#endif
            if (!PauseGame.exists) {
#if FULLLOG
                MoreDebug.Log ("No PauseGame -----");
#endif
            }
            if (InfoPriority > operation.MissionTerseness) {
                MessageBoxParams parms = new MessageBoxParams ();
                parms.Message = FailureInfo;
                parms.MessageBoxTitle = MissionTitle;
                parms.Button1Action = PauseGame.only.queueResume;
                parms.MultipleCallBehaviour = MultipleCallBehaviours.Queue;
                EasyMessageBox.Show (parms);
                PauseGame.only.queuePause ();
            }
            currentState = GoalManager.GoalState.reallose;
            FailureEvent.Invoke ();
            operation.LoseMission ();
            isActive = false;
            //Reset ();
        }

        void SucceedMission () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif
            if (!isActive) {
                return;
            }
#if FULLLOG
            MoreDebug.Log ("real.");
#endif
            if (!PauseGame.exists) {
#if FULLLOG
                MoreDebug.Log ("No PauseGame -----");
#endif
            }
            if (InfoPriority > operation.MissionTerseness) {
                MessageBoxParams parms = new MessageBoxParams ();
                parms.Message = SuccessInfo;
                parms.MessageBoxTitle = MissionTitle;
                parms.Button1Action = PauseGame.only.queueResume;
                parms.MultipleCallBehaviour = MultipleCallBehaviours.Queue;
                EasyMessageBox.Show (parms);
                PauseGame.only.queuePause ();
            }
            currentState = GoalManager.GoalState.realwin;
            SuccessEvent.Invoke ();
            operation.WinMission ();
            isActive = false;
            //Reset ();
        }
        public bool RegisterSelf () {
            bool rtn = Registry.only.Register (this, out RegIdx);
            return rtn;
        }

        #endregion

    }
}