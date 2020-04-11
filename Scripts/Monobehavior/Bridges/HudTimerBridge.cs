// #define FULLLOG
#undef FULLOG

using Goldraven.Generic;
using Goldraven.Utility;
using UnityEngine;

namespace Goldraven.Gui {

    /*
     * cmcb  2019/05/22
     *
     * Production -- cmcb  2019/07/11
     *
     * Bridge hud information from a timer
     * to a display gui slot
     *
     * Put it on the same gameobject as the timer (duh)
     */

    [RequireComponent (typeof (Timer))]

    public class HudTimerBridge : HudBridge {

        #region Properties

        // Local
        public string Caption = "Timer";

        //internal
        private Timer t;

        #endregion

        // methods
        // Unity methods

        protected override void Awake () {
            base.Awake ();
            t = GetComponent<Timer> ();
#if FULLLOG
            if (t == null) {
                MoreDebug.LogError ("Hud Timer Bridge found no timer");
            }
#endif
            LoadHudData (t.ShowElapsed, t.timerTickEvent, t.timerDoneEvent, Caption, t.timerDoneMessage, 0);
        }

        // local

    }
}