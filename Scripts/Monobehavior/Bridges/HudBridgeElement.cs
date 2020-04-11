// #define FULLLOG
#undef FULLOG

using UnityEngine.Events;
#if FULLLOG
using Goldraven.Generic;
#endif

namespace Goldraven.Gui {

    /*
     * cmcb  2019/07/04
     *
     *   Production - 2019/07/11
     *
     * Bridge a single hud element to a single data source
     *
     * Put it on the same gameobject as the bridge.
     */

    public class HudBridgeElement {

        #region Properties

        private const string Delimiter = "|";

        // Local

        public int Key;
        public StringGetter Callback;
        public DisplayPutter CallForward;
        public UnityEvent onDataChange;
        public UnityEvent onDataInvalidated;
        public string Caption;
        public string Value;
        public string OverrideValue;
        public bool DataIsValid = true;
        public bool ReleasePending = false;
        public int DisplaySlot;
        public int TimeSlot;

        //internal

        #endregion

        // constructors and initializers

        // Must run both initializers
        public void InitFromHud (int k, DisplayPutter cf) {
#if FULLLOG
            MoreDebug.Log ("InitFromHud");
#endif

            Key = k;
            CallForward = cf;
        }

        public void InitFromBridge (StringGetter cb, UnityEvent dc, UnityEvent di, string cap, string orv, int ds) {
#if FULLLOG
            MoreDebug.Log ("InitFromBridge");
#endif

            Callback = cb;
            onDataChange = dc;
            onDataInvalidated = di;
            Caption = cap;
            OverrideValue = orv;
            DisplaySlot = ds;

            onDataChange.AddListener (UpdateFromData);
            onDataInvalidated.AddListener (InvalidateDataSource);

        }

        // methods

        public void ForceUpdate () {
#if FULLLOG
            MoreDebug.Log ("ForceUpdate");
#endif
            UpdateFromData ();
        }

#if FULLLOG

        public string DumpToString () {
            return Key + Delimiter + Callback + Delimiter + CallForward + Delimiter + onDataChange + Delimiter +
                onDataInvalidated + Delimiter + Caption + Delimiter + Value + Delimiter + OverrideValue + Delimiter +
                DataIsValid + Delimiter + ReleasePending + Delimiter + DisplaySlot + Delimiter + TimeSlot;

        }
#endif

        // event handlers

        private void UpdateFromData () {
#if FULLLOG
            MoreDebug.Log ("UpdateFromData (" + Key + ")");
#endif
            if (DataIsValid) {
                Value = Callback ();
            }
            CallForward (this);
        }

        private void InvalidateDataSource () {
#if FULLLOG
            MoreDebug.Log ("InvalidateDataSource (" + Key + ")");
#endif
            Value = OverrideValue;
            DataIsValid = false;
            onDataChange.RemoveListener (UpdateFromData);
            onDataInvalidated.RemoveListener (InvalidateDataSource);
#if FULLLOG
            MoreDebug.Log (DumpToString ());
#endif

        }

    }
}