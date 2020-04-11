// #define FULLLOG
#undef FULLOG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if FULLLOG
using Goldraven.Generic;
#endif

namespace Goldraven.Gui {

    /* cmcb  2019/06/27
     *
     * Display information in a slot system
     *
     * Singleton
     */

    public delegate string StringGetter ();
    public delegate void DisplayPutter (HudBridgeElement he);

    public class GuiHudBasic : MonoBehaviour {

        #region Properties
        // Singleton
        public static GuiHudBasic only { get; private set; }

        public static bool exists { get; private set; }

        // Local

        [SerializeField]
        private Text[] TextSlots;
        [HideInInspector]
        public List<HudBridgeElement> DataSlots;
        public float ReleaseDelay = 15f; // delay to release a used (time)slot

        public const string NewLine = "\x0A\x0C";

        public float DisplaySwitchDelay = 1.5f; // seconds

        // Internal
        private int slotcount;
        private int[] timeslotcounts;
        private int maxtimeslot = 0;
        private int[] activetimeslots;
        private int indexcounter = 0;

        #endregion

        // methods
        // Unity methods

        void Awake () {
#if FULLLOG
            MoreDebug.Log ("Awake");
#endif
            // singleton
            exists = false;
            if (only == null) {
                // DontDestroyOnLoad (gameObject);
                only = this;
            } else if (only != this) {
                Destroy (gameObject);
            }
            exists = true;
            // local init
            DataSlots = new List<HudBridgeElement> ();
            FullReset ();
        }

        // local

        public int RegisterHudData (HudBridgeElement data) {
#if FULLLOG
            MoreDebug.Log ("RegisterHudData");
#endif

            data.InitFromHud (indexcounter++, HudUpdate);
            SetTimeSlot (data);
            DataSlots.Add (data);
            return data.Key;
        }

        public void ReleaseHudData (int key) {
#if FULLLOG
            MoreDebug.Log ("ReleaseHudData");
#endif
            DataSlots.RemoveAt (KeyToIndex (key));
            RecalcSlots ();
        }

        private IEnumerator DelayedRelease (int key) {
#if FULLLOG
            MoreDebug.Log ("DelayedRelease");
#endif
            yield return new WaitForSeconds (ReleaseDelay);
            ReleaseHudData (key);
        }

        public void FullReset () {
#if FULLLOG
            MoreDebug.Log ("FullReset");
#endif
            // only way to change slotcount after adding/removing textslots
            // Only needed if changing display, not timing - cmcb 2019/11/21
            StopAllCoroutines ();
            DataSlots.Clear ();
            activetimeslots = null;
            timeslotcounts = null;

            slotcount = TextSlots.Length;
            maxtimeslot = 0;
            activetimeslots = new int[slotcount];
            timeslotcounts = new int[slotcount];
            RecalcSlots ();
            StartCoroutine (RollTimeslot ());

        }

        public int KeyToIndex (int key) {
            for (int cc = 0; cc < DataSlots.Count; cc++) {
                if (DataSlots[cc].Key == key) {
                    return cc;
                }
            }
            return -1;
        }

        private void RecalcSlots () {
#if FULLLOG
            MoreDebug.Log ("RecalcSlots");
#endif
            // Use this to reset slots after a timing change - cmcb 2019/11/21
            maxtimeslot = 0;
            // clear all timeslot counters
            for (int cc = 0; cc < slotcount; cc++) {
                timeslotcounts[cc] = 0;
                TextSlots[cc].text = "";
            }
            // refill timeslot counters from data
            for (int cc = 0; cc < DataSlots.Count; cc++) {
                SetTimeSlot (DataSlots[cc]);
            }

        }

        private void SetTimeSlot (HudBridgeElement hd) {
#if FULLLOG
            MoreDebug.Log ("SetTimeSlot");
#endif
            hd.TimeSlot = timeslotcounts[hd.DisplaySlot];
            timeslotcounts[hd.DisplaySlot]++;
            if (timeslotcounts[hd.DisplaySlot] > maxtimeslot) {
                maxtimeslot = timeslotcounts[hd.DisplaySlot];
            }

        }
        // Use ReleaseHudData to clear a time slot

        // event handlers

        public void HudUpdate (HudBridgeElement be) { // display change event handler
#if FULLLOG
            MoreDebug.Log ("HudUpdate: " + be.Key + " (" + be.DataIsValid + ")(" + be.ReleasePending + ")");
#endif

            be.Value = be.Callback ();
            if (be.TimeSlot == activetimeslots[be.DisplaySlot]) {
                UpdateText (TextSlots[be.DisplaySlot], be);
            }
        }

        private IEnumerator RollTimeslot () {

#if FULLLOG
            MoreDebug.Log ("RollTimeslot");
#endif
            bool[] slotchanged = new bool[slotcount];
            while (true) {
                yield return new WaitForSeconds (DisplaySwitchDelay);
                for (int cc = 0; cc < slotcount; cc++) {
                    slotchanged[cc] = false;
                }

                foreach (HudBridgeElement data in DataSlots) {
                    if ((!data.DataIsValid) && (!data.ReleasePending)) {
#if FULLLOG
                        MoreDebug.Log ("***   RollTimeslot starting delayed release");
#endif
                        StartCoroutine (DelayedRelease (data.Key));
                        data.ReleasePending = true;
                    }
                    if (!slotchanged[data.DisplaySlot]) {
                        activetimeslots[data.DisplaySlot]++;
                        slotchanged[data.DisplaySlot] = true;
                    }
                    if (activetimeslots[data.DisplaySlot] >= timeslotcounts[data.DisplaySlot]) {
                        activetimeslots[data.DisplaySlot] = 0;
                    }
                    if (data.TimeSlot == activetimeslots[data.DisplaySlot]) {
                        UpdateText (TextSlots[data.DisplaySlot], data);
                    }
                }
            }

        }

        private void UpdateText (Text ht, HudBridgeElement hd) {
            ht.text = hd.Caption + NewLine + hd.Value;
#if FULLLOG
            MoreDebug.Log ("(" + hd.Key + ") Update text: " + hd.Caption + " / " + hd.Value);
#endif

        }

        // display modifiers

    }
}