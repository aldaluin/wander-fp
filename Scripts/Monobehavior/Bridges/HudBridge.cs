// #define FULLLOG
#undef FULLOG

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if FULLLOG
using Goldraven.Generic;
#endif

namespace Goldraven.Gui {

    /* cmcb  2019/05/22
     *
     * Production -- cm 2019/07/11
     *
     * Bridge hud information (like mission status specs and a score generator)
     * to the display gui slots
     *
     * ABSTRACT -- don't put it anywhere; put a derived class on a manager
     * (so far just goalmanager and endmanager).
     */

    public class HudBridge : MonoBehaviour {

        #region Properties

        // Local

        private List<int> hudkeys;

        //internal
        private GuiHudBasic hud;

        #endregion

        // methods
        // Unity methods

        protected virtual void Awake () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif
            hud = GuiHudBasic.only;
            hudkeys = new List<int> ();
        }

        protected virtual void OnDestroy () {
#if FULLLOG
            MoreDebug.Log (DumpToString ());
            MoreDebug.Log ("List: " + DumpList ());
#endif

            foreach (int cc in hudkeys) {
                hud.ReleaseHudData (cc);
            }
        }

        // local

        protected int LoadHudData (StringGetter cb, UnityEvent datachange, UnityEvent datadone, string cap, string donecap, int slot) {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name);
#endif
            HudBridgeElement data = new HudBridgeElement ();
            data.InitFromBridge (cb, datachange, datadone, cap, donecap, slot);
            hudkeys.Add (hud.RegisterHudData (data));
            data.ForceUpdate ();
            return hudkeys.Count - 1;

        }

#if FULLLOG

        public string DumpList () {
            string st = "(" + hudkeys.Count.ToString () + ") ";
            foreach (int cc in hudkeys) {
                st = st + cc.ToString () + "  ";
            }
            return st;
        }

        public string DumpToString (int key) {
            return hud.DataSlots[hud.KeyToIndex (key)].DumpToString ();
        }

        public string DumpToString () {
            string ob = "{ ";
            string cb = " }";
            string comma = ", ";
            string spaces = "   ";
            System.Text.StringBuilder sb = new System.Text.StringBuilder ();
            foreach (int key in hudkeys) {
                sb.Append (key + comma + ob + DumpToString (key) + cb + spaces);
            }
            return sb.ToString ();
        }

#endif

    }
}