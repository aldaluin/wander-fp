// #define FULLLOG
#undef FULLOG

using Goldraven.Generic;
using Goldraven.Mgmt;
using UnityEngine;

namespace Goldraven.Gui {

    /*
     * cmcb  2019/05/22
     *
     * Production -- cmcb 2019/07/11
     *
     * Bridge hud information from a criterion
     * to a display gui slot
     *
     * Put it on the same gameobject as the timer (duh)
     */

    [RequireComponent (typeof (GoalManager))]

    public class HudCriterionBridge : HudBridge {

        #region Properties

        // Local
        public bool ShowSucceed = true;
        public bool ShowFail = false;

        //internal

        #endregion

        // methods
        // Unity methods

        protected override void Awake () {
            base.Awake ();
            GoalManager mgr = GetComponent<GoalManager> ();

            if (ShowSucceed) {
                for (int cc = 0; cc < mgr.succeedCriteria.Length; cc++) {
                    Criterion crit = mgr.succeedCriteria[cc];
                    LoadHudData (crit.ShowCompletion, crit.onAmountChange, crit.onCriterionMet, crit.Description, "Done", 1);
                }
            }
            if (ShowFail) {
                for (int cc = 0; cc < mgr.failCriteria.Length; cc++) {
                    Criterion crit = mgr.failCriteria[cc];
                    LoadHudData (crit.ShowCompletion, crit.onAmountChange, crit.onCriterionMet, crit.Description, "Done", 2);
                }
            }
        }

    }
}