// #define FULLLOG
#undef FULLOG

using System.Collections;
using Goldraven.Generic;
using Goldraven.Interfaces;
using Goldraven.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using XenStudio.UI;

namespace Goldraven.Mgmt {

    public class OperationManager : MonoBehaviour, iUniRegister {

        /*
         *       cmcb 2019/02/26
         *
         *       Provide  a consistent interface for managing a single scenario
         *       (set of victory &/or loss conditions)
         *       regardless of the map and props used.
         *
         *
         *       Deliberately no UI except modal displays and/or splash screens.
         *       One of these goes in each operation overlay scene only.
         *
         *       Operations can be nested.  If they are, sub-operations ignored
         *       by the parent op.  All ops run at start, and concurrently.
         */

        #region Properties
        // Constants
        public const string newline = "\r\n";

        // Static

        // Local

        // Inspector entries

        [SerializeField]
        private string OperationName = "";
        public string oname { get { return OperationName; } }
        //[SerializeField]
        //private string LibraryName;   todo: decide if a library is needed
        [TextArea, SerializeField]
        public string OperationDescription = "";
        [TextArea, SerializeField]
        public string StartingInfo = "";
        [TextArea, SerializeField]
        public string SuccessInfo = "";
        [TextArea, SerializeField]
        public string FailureInfo = "";
        public int InfoPriority = 3;
        [SerializeField]
        public string[] OperationOverlayNames { get; private set; } // can be bling or sub-ops
        [SerializeField]
        public bool ForceLevelWin { get; private set; } // implies force operation end
        [SerializeField]
        public bool ForceLevelLose { get; private set; } // implies force operation end
        public int MissionTerseness = 2; // Lower is more
        public MissionManager[] Missions;
        [SerializeField]
        private GoalManager.GoalState OperationBias = GoalManager.GoalState.neutral; // neutral == assumeloss here
        public EndManager[] OperationEndConditions;
        // [SerializeField]
        // public GameObject [] [] ItemPrimeSiblings;
        public GameObject[] ItemPrimeParents;
        public GameObject ItemPrimeGrandparent;
        public Vector3 LocationOffset = new Vector3 (0f, 3f, 0f);

        public UnityEvent OnBeginOperation, OnEndOperation, OnWinOperation, OnLoseOperation;

        // Internal
        private bool _active = false;
        public bool isActive { get { return _active; } set { setActive (value); } }

        private GameObject[][] ItemPrimesActual;

        // private bool _opIsStarted  = false;
        // private EndManager.GoalState _opstate = EndManager.GoalState.neutral;
        private LevelManager level = null;
        private ActivityCounter missioncount = new ActivityCounter (2, new string[] {
            ActivityCounter.losestring, ActivityCounter.winstring
        });

        #endregion

        #region Methods

        // Unity methods

        void Awake () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif

            // local init

            // Check for level manager
            if (LevelManager.exists) {
                level = LevelManager.only;
            }
            // Create events
            if (OnBeginOperation == null) {
                OnBeginOperation = new UnityEvent ();
            }
            if (OnEndOperation == null) {
                OnEndOperation = new UnityEvent ();
            }
            if (OnWinOperation == null) {
                OnWinOperation = new UnityEvent ();
            }
            if (OnLoseOperation == null) {
                OnLoseOperation = new UnityEvent ();
            }

            // set internal variables
            isActive = false;

            // Create the actual item prime array and link the persistent primes to it
            InitItemPrimes ();

        }

        void Start () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif
            RegisterSelf ();
            // Warn if there's no mission
            if (Missions.Length == 0) {
                Debug.LogWarning ("OperationManager: No missions set.");
            }

            // Set up each mission
            foreach (MissionManager mi in Missions) {
                if (mi == null) continue;
                mi.SetOperation (this);
                if (mi.StartOnLoad) {
                    OnBeginOperation.AddListener (mi.BeginMission);
                }
            }

            // Set up end managers
            foreach (EndManager em in OperationEndConditions) {
                em.OnEnd.AddListener (EndOperation);
                em.isActive = true;
            }

            // let the level know you're here
            level.IncrementOperations ();

            // load overlays
            if (OperationOverlayNames != null) {
                foreach (string scenename in OperationOverlayNames) {
                    SceneManager.LoadScene (scenename, LoadSceneMode.Additive); // todo: what if its not there?
                }
            }

            // Warn if there's no end criteria (level won't end)
            if (OperationEndConditions.Length == 0) {
                Debug.LogWarning ("OperationManager: No criteria set to end the level.");
            }

            // Place the items
            StartCoroutine (PlaceItems ());

            // show the op start screen
            if (!PauseGame.exists) {
#if FULLLOG
                MoreDebug.Log ("No PauseGame -----");
#endif
            }
            if (CampaignManager.campaign.ForceMissionTerseness) {
                MissionTerseness = CampaignManager.campaign.OperationTerseness;
            }
            if (InfoPriority >= CampaignManager.campaign.OperationTerseness) {
                MessageBoxParams parms = new MessageBoxParams ();
                parms.Message = StartingInfo;
                parms.MessageBoxTitle = OperationName;
                parms.Button1Action = PauseGame.only.queueResume;
                parms.MultipleCallBehaviour = MultipleCallBehaviours.Queue;
                EasyMessageBox.Show (parms);
                PauseGame.only.queuePause ();
            }

            // announce the start of the op
            // _opIsStarted = true;
            OnBeginOperation.Invoke ();

        }

        /*
        void OnDestroy() {
            #if FULLLOG
            MoreDebug.Log("go: " + gameObject.name);
            #endif

            // Tear down each end condition
            foreach (EndManager e in OperationEndConditions) {
                e.OnEnd.RemoveAllListeners();
                e.OnForceSuccess.RemoveAllListeners();
                e.OnForceFail.RemoveAllListeners();
            }

            // Tear down each mission
            foreach (MissionManager mi in Missions) {
                if (mi == null) continue;
                mi.ClearOperation();
                OnBeginOperation.RemoveListener(mi.BeginMission);
                mi.isActive = false;
            }

            // Remove events
            OnBeginOperation.RemoveAllListeners();
            OnEndOperation.RemoveAllListeners();
            OnWinOperation.RemoveAllListeners();
            OnLoseOperation.RemoveAllListeners();

        }
        */

        // local methods

        private void setActive (bool isTrue) {
            _active = isTrue;
        }

        private void InitItemPrimes () {
            // Grandparents, then parents, then sibs
            // Count them first, then link them

            // Count groups
            // grandparent
            int gptgpct;
            int[] gptgp;
            if (ItemPrimeGrandparent == null) {
                gptgpct = 0;
                gptgp = null;
            } else {
                gptgpct = ItemPrimeGrandparent.transform.childCount;
                gptgp = new int[gptgpct];
            }
            // parents
            int pargpct = ItemPrimeParents.Length;
            int[] pargp;
            if (pargpct == 0) {
                pargp = null;
            } else {
                pargp = new int[pargpct];
            }
            // sibs
            // int sibgpct = ItemPrimeSiblings.Length;
            // int [] sibgp = new int[sibgpct];

            // Create and fill arrays
            // ItemPrimesActual = new GameObject [gptgpct + pargpct + sibgpct] [] ;
            ItemPrimesActual = new GameObject[gptgpct + pargpct][];
            // grandparent
            for (int cc = 0; cc < gptgpct; cc++) {
                gptgp[cc] = ItemPrimeGrandparent.transform.GetChild (cc).childCount;
                ItemPrimesActual[cc] = new GameObject[gptgp[cc]];
                for (int dd = 0; dd < gptgp[cc]; dd++) {
                    ItemPrimesActual[cc][dd] = ItemPrimeGrandparent.transform.GetChild (cc).GetChild (dd).gameObject;
                }
            }
            // parents
            for (int cc = 0; cc < pargpct; cc++) {
                pargp[cc] = ItemPrimeParents[cc].transform.childCount;
                ItemPrimesActual[cc + gptgpct] = new GameObject[pargp[cc]];
                for (int dd = 0; dd < pargp[cc]; dd++) {
                    ItemPrimesActual[cc + gptgpct][dd] = ItemPrimeParents[cc].transform.GetChild (dd).gameObject;
                }
            }
            // sibs
            /*
            for (int cc = 0; cc < sibgpct ; cc++){
                sibgp [cc] = ItemPrimeSiblings [cc].Length;
                ItemPrimesActual [cc + gptgpct + pargpct ] = new GameObject[sibgp [cc]];
                for (int dd = 0; dd < sibgp [cc]; dd++){
                    ItemPrimesActual [cc + gptgpct + pargpct][dd] = ItemPrimeSiblings [cc][dd];
                }
            }
            */
        }

        private IEnumerator PlaceItems () {
#if FULLLOG
            MoreDebug.Log ("Placing items.");
#endif

            while (!level.LocationActive) {
#if FULLLOG
                MoreDebug.Log ("Item placer is waiting for locations.");
#endif
                yield return new WaitForSecondsRealtime (2);
            }
            for (int cc = 0; cc < ItemPrimesActual.Length; cc++) {
                if (ItemPrimesActual[cc].Length == 1) {
                    while (!level.EndOfLocations) {
                        level.GetLocation (cc);
                        yield return null;
                        PlaceAnItem (cc, ItemPrimesActual[cc][0], level.NextLocation, true);
                    }
                } else {
                    foreach (GameObject go in ItemPrimesActual[cc]) {
                        if (level.EndOfLocations) break;
                        level.GetLocation (cc);
                        yield return null;
                        PlaceAnItem (cc, go, level.NextLocation, false);
                    }
                }
            }
        }

        private void PlaceAnItem (int gp, GameObject go, GameObject loc, bool clone) {
#if FULLLOG
            MoreDebug.Log ("Placing item: " + go.ToString ());
            MoreDebug.Log ("Placing at: " + loc.transform.position);
#endif
            if (clone) Instantiate (go, go.transform.parent, true);
            go.transform.SetPositionAndRotation (loc.transform.position + LocationOffset, loc.transform.rotation);
            Rigidbody rb = go.GetComponent<Rigidbody> ();
            rb.velocity = new Vector3 (0, 0, 0);
            rb.angularVelocity = new Vector3 (0, 0, 0);
            Destroy (loc);
#if FULLLOG
            MoreDebug.Log ("Item placed - " + go.transform.ToString () + " " + go.transform.position);
#endif

        }

        private bool CalculateOperationWinStatus () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif

            // if the operation ends now, will it be won or lost?
            int misswon = 0, misslose = 0;
            foreach (MissionManager mi in Missions) {
                if (mi == null) break;
                if ((mi.currentState == GoalManager.GoalState.assumewin) || (mi.currentState == GoalManager.GoalState.realwin)) {
                    misswon++;
                } else if ((mi.currentState == GoalManager.GoalState.assumelose) || (mi.currentState == GoalManager.GoalState.reallose)) {
                    misslose++;
                }
            }
            bool opwon;
            bool assumewin = ((OperationBias == GoalManager.GoalState.realwin) || (OperationBias == GoalManager.GoalState.assumewin));
            if ((misswon > misslose) || ((misswon == misslose) && assumewin)) {
                opwon = true;
            } else {
                opwon = false;
            }
            return opwon;
        }

        private void WinOperation () {
#if FULLLOG
            MoreDebug.Log ("qaa go: " + gameObject.name);
#endif

            if (!PauseGame.exists) {
#if FULLLOG
                MoreDebug.Log ("No PauseGame -----");
#endif
            }
            if (InfoPriority > CampaignManager.campaign.OperationTerseness) {
                MessageBoxParams parms = new MessageBoxParams ();
                parms.Message = SuccessInfo;
                parms.MessageBoxTitle = oname;
                parms.Button1Action = PauseGame.only.queueResume;
                parms.MultipleCallBehaviour = MultipleCallBehaviours.Queue;
                EasyMessageBox.Show (parms);
                PauseGame.only.queuePause ();
            }
            OnWinOperation.Invoke ();
            level.WinAnOperation ();
            // Remember winning the operation does not win the level or (necessarily) end the operation
            if (ForceLevelWin) {
                EndOperation ();
                level.WinLevel ();
            }
        }

        private void LoseOperation () {
#if FULLLOG
            MoreDebug.Log ("qaa go: " + gameObject.name);
#endif

            if (!PauseGame.exists) {
#if FULLLOG
                MoreDebug.Log ("No PauseGame -----");
#endif
            }
            if (InfoPriority > CampaignManager.campaign.OperationTerseness) {
                MessageBoxParams parms = new MessageBoxParams ();
                parms.Message = FailureInfo;
                parms.MessageBoxTitle = oname;
                parms.Button1Action = PauseGame.only.queueResume;
                parms.MultipleCallBehaviour = MultipleCallBehaviours.Queue;
                EasyMessageBox.Show (parms);
                PauseGame.only.queuePause ();
            }
            OnLoseOperation.Invoke ();
            level.LoseAnOperation ();
            // Remember winning the operation does not win the level or (necessarily) end the operation
            if (ForceLevelLose) {
                EndOperation ();
                level.LoseLevel ();
            }

        }

        private void EndOperation () {
            // Ending the operation does not win or lose it.
#if FULLLOG
            MoreDebug.Log (" qaa go: " + gameObject.name);
#endif

            foreach (MissionManager mm in Missions) {
                mm.isActive = false;
            }
            if (!missioncount.isDone ()) {
                // then win status has not been determined and announced yet
                if (CalculateOperationWinStatus ()) {
                    WinOperation ();
                } else {
                    LoseOperation ();
                }
            }
            StartCoroutine (ShowOperationStatus ());
            OnEndOperation.Invoke ();
            level.DecrementOperations ();

        }

        public void WinMission () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif

            missioncount.Decrement (ActivityCounter.winstring);

#if FULLLOG
            if (!missioncount.isValid ()) {
                MoreDebug.LogError ("Op count invalid");
            }
#endif

            if (missioncount.isDone ()) {
                if (CalculateOperationWinStatus ()) {
                    WinOperation ();
                } else {
                    LoseOperation ();
                }
            }

        }

        public void LoseMission () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif

            missioncount.Decrement (ActivityCounter.losestring);

#if FULLLOG
            if (!missioncount.isValid ()) {
                MoreDebug.LogError ("Op count invalid");
            }
#endif

            if (missioncount.isDone ()) {
                if (CalculateOperationWinStatus ()) {
                    WinOperation ();
                } else {
                    LoseOperation ();
                }
            }

        }

        public void StartMission () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif

            missioncount.Increment ();

        }

        private IEnumerator ShowOperationStatus () {
#if FULLLOG
            MoreDebug.Log (".");
#endif

            yield return new WaitForSeconds (2);
            if (!PauseGame.exists) {
#if FULLLOG
                MoreDebug.Log ("No PauseGame -----");
#endif
            }
            string sceneresult = "There are " + Missions.Length + "(" + missioncount.MaximumCount + ") missions " + newline +
                "Actual wins: " + missioncount.GroupCount[1] + newline +
                "Actual losses: " + missioncount.GroupCount[0];
            MessageBoxParams parms = new MessageBoxParams ();
            parms.Message = sceneresult;
            parms.MessageBoxTitle = "Operation is Ended";
            parms.Button1Action = PauseGame.only.queueResume;
            parms.MultipleCallBehaviour = MultipleCallBehaviours.Queue;
            EasyMessageBox.Show (parms);
            PauseGame.only.queuePause ();
        }

        public bool RegisterSelf () {
            return Registry.only.Register (this);

        }

        #endregion

    }
}