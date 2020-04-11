// #define FULLLOG
#undef FULLOG

using System;
using System.Collections;
#if FULLLOG
using Goldraven.Generic;
#endif
using Goldraven.Interfaces;
using Goldraven.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Goldraven.Mgmt {

    public class SpawnManager : MonoBehaviour, iMultiRegister {

        /*
         *       cmcb 2019/06/04
         *
         *       Provide a consistent interface for spawning features by class.
         *       This unit provides sets of locations without needing to know
         *       the specifics of what's going to be put in them.
         *
         *       One of these goes in each map overlay scene only.
         *
         */

        #region Properties

        // Constants
        public const string newline = "\r\n";

        // Static

        // Local

        // Inspector entries

        [SerializeField]
        private string SpawnerName = "";
        public String oname { get { return SpawnerName; } }
        //[SerializeField]
        //private string LibraryName;   todo: decide if a library is needed
        [TextArea]
        public string MapDescription;

        // [SerializeField]
        // public GameObject [] [] PoiSiblings;
        public GameObject[] PoiParents;
        public GameObject PoiGrandparent;

        // Internal
        private GameObject[][] PoiActual;
        private int[] PoiIndexes;
        private LevelManager level = null;

        private bool _active = false;
        public bool isActive { get { return _active; } set { setActive (value); } }
        private int RegIdx;
        public int ListIndex { get { return RegIdx; } }

        #endregion

        #region Methods

        // Unity methods

        void Awake () {
#if FULLLOG
            MoreDebug.Log (".");
#endif
            // Check for level manager
            if (LevelManager.exists) {
                level = LevelManager.only;
            } else {
#if FULLLOG
                MoreDebug.Log ("Level manager is missing.");
#endif
            }

        }

        void Start () {
#if FULLLOG
            MoreDebug.Log (".");
#endif
            RegisterSelf ();
            if (LevelManager.exists) {
                // running as part of campaign or at least level...
                InitPoi ();
#if FULLLOG
                MoreDebug.Log (ShowAllPoi ());
#endif
                level.OnGetLocation.AddListener (GiveNextPoiToLevel);
                level.LocationActive = true;
                level.EndOfLocations = false;
            } else if (SceneLoader.exists) {
                if (SceneLoader.only.gameObject.scene == SceneManager.GetActiveScene ()) {
                    // transition has no sceneloader, so running standalone -- load player overlay
                    SceneLoader.only.BeginTransition (false);
                } // else running solo map, do nothing
                SceneLoader.only.EndTransition ();
                if (PauseGame.exists) {
                    PauseGame.only.beginPlay ();
                }

            }
            StartCoroutine (doSpawnPlayer ());
        }

        // Local methods
        private void setActive (bool isTrue) {
            _active = isTrue;
        }

        public void GiveNextPoiToLevel (int group) {
            // int group = 0;
#if FULLLOG
            MoreDebug.Log ("Giving poi [" + group + "][" + PoiIndexes[group] + "] is " + PoiActual[group][PoiIndexes[group]] +
                " at " + PoiActual[group][PoiIndexes[group]].transform.position);
#endif
            level.NextLocation = PoiActual[group][PoiIndexes[group]];
#if FULLLOG
            MoreDebug.Log (level.NextLocation.ToString () + " " + level.NextLocation.transform.position);
#endif
            PoiIndexes[group]++;
            if (PoiIndexes[group] >= PoiActual[group].Length) {
                level.EndOfLocations = true;
            }
        }

        private void InitPoi () {
#if FULLLOG
            MoreDebug.Log (".");
#endif
            // Grandparents, then parents, then sibs
            // Count them first, then link them

            // Count groups
            // grandparent
            int gptgpct;
            int[] gptgp;
            if (PoiGrandparent == null) {
                gptgpct = 0;
                gptgp = null;
            } else {
                gptgpct = PoiGrandparent.transform.childCount;
                gptgp = new int[gptgpct];
            }
            // parents
            int pargpct = PoiParents.Length;
            int[] pargp;
            if (pargpct == 0) {
                pargp = null;
            } else {
                pargp = new int[pargpct];
            }
            // sibs
            // int sibgpct = PoiSiblings.Length;
            // int [] sibgp = new int[sibgpct];

            // Create and fill arrays
            // PoiActual = new GameObject [gptgpct + pargpct + sibgpct] [] ;
            // PoiIndexes = new int[gptgpct + pargpct + sibgpct];
            PoiIndexes = new int[gptgpct + pargpct];
            PoiActual = new GameObject[gptgpct + pargpct][];
            // grandparent
            for (int cc = 0; cc < gptgpct; cc++) {
                gptgp[cc] = PoiGrandparent.transform.GetChild (cc).childCount;
                PoiIndexes[cc] = 0;
                PoiActual[cc] = new GameObject[gptgp[cc]];
                for (int dd = 0; dd < gptgp[cc]; dd++) {
                    PoiActual[cc][dd] = PoiGrandparent.transform.GetChild (cc).GetChild (dd).gameObject;
                    if (PoiActual[cc][dd].transform.position == Vector3.zero) {
                        Debug.LogWarning ("Poi is at origin - Check spawnmanager Poi settings.");
                    }
                }
            }
            // parents
            for (int cc = 0; cc < pargpct; cc++) {
                pargp[cc] = PoiParents[cc].transform.childCount;
                PoiIndexes[cc + gptgpct] = 0;
                PoiActual[cc + gptgpct] = new GameObject[pargp[cc]];
                for (int dd = 0; dd < pargp[cc]; dd++) {
                    PoiActual[cc + gptgpct][dd] = PoiParents[cc].transform.GetChild (dd).gameObject;
                }
            }
            // sibs
            /*
            for (int cc = 0; cc < sibgpct ; cc++){
                sibgp [cc] = PoiSiblings [cc].Length;
                PoiIndexes [cc + gptgpct + pargpct] = 0;
                PoiActual [cc + gptgpct + pargpct ] = new GameObject [sibgp [cc]];
                for (int dd = 0; dd < sibgp [cc]; dd++){
                    PoiActual [cc + gptgpct + pargpct][dd] = PoiSiblings [cc][dd];
                }
            }
            */
        }

        private string ShowAllPoi () {
            string hdr = "{{";
            string itemsep = ",";
            string groupsep = "}," + newline + "{";
            string ftr = "}}";
            string nullstr = "(null)";
            System.Text.StringBuilder sbf = new System.Text.StringBuilder ();
            sbf.Append (hdr);
            foreach (int cc in PoiIndexes) {
                sbf.Append (cc);
                sbf.Append (itemsep);
            }
            sbf.Append (ftr);
            sbf.Append (newline);
            sbf.Append (hdr);
            foreach (GameObject[] cc in PoiActual) {
                foreach (GameObject dd in cc) {
                    if (dd == null) {
                        sbf.Append (nullstr);
                    } else {
                        sbf.Append (dd.transform.position.ToString ());
                    }
                    sbf.Append (itemsep);
                }
                sbf.Append (groupsep);
            }
            sbf.Append (ftr);
            return sbf.ToString ();
        }

        public static IEnumerator doSpawnPlayer () {
#if FULLLOG
            MoreDebug.Log (".");
#endif
            const float waitTime = 3.0f;
            GameObject spawn, player;
            while (true) {
#if FULLLOG
                MoreDebug.Log (" Attempting player spawn.");
#endif
                // Put the player on an (the) entry point.
                // todo: add alternate entry points and a way to pick one.
                spawn = GameObject.FindWithTag ("Respawn");
                player = GameObject.FindWithTag ("Player");
                if ((spawn != null) && (player != null)) {
                    player.transform.position = spawn.transform.position;
                    player.transform.rotation = spawn.transform.rotation;
                    PauseGame.only.RecheckPlayer ();
#if FULLLOG
                    MoreDebug.Log (" Player spawn succeeded.");
#endif
                    break;
                } else {
                    yield return new WaitForSecondsRealtime (waitTime);
                }
            }
        }

        public bool RegisterSelf () {
            bool rtn = Registry.only.Register (this, out RegIdx);
            return rtn;
        }

        #endregion

    }
}