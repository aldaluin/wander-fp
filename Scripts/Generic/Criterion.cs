// #define FULLLOG
#undef FULLOG

#if FULLLOG
using Goldraven.Generic;
#endif

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Goldraven.Generic {

    [Serializable]
    public class Criterion {

        public enum CriteriaMatch {
            matchone,
            matchtwo,
            matchhalf,
            matchall
        }

        public static readonly string[] CriteriaMatchStrings = { "match one", "matchtwo", "matchhalf", "match all" };

        public string Description;
        public bool TargetReached { get; private set; }
        public int StartAmount = 0;
        public int TargetAmount = 1;
        public int actualAmount { get; private set; }
        public bool recording = false;
        public UnityEvent onCriterionMet, onAmountChange;

        public Criterion () {
            actualAmount = StartAmount;
            TargetReached = false;
            if (onCriterionMet == null) {
                onCriterionMet = new UnityEvent ();
            }
            if (onAmountChange == null) {
                onAmountChange = new UnityEvent ();
            }

        }

        public void init (UnityAction eventToTrigger) {
#if FULLLOG
            MoreDebug.Log (Description);
#endif
            onCriterionMet.AddListener (eventToTrigger);

            //Debug.Log("Criterion init: " +  ShowCompletion());
        }

        public void reset () {
#if FULLLOG
            MoreDebug.Log (Description);
#endif
            actualAmount = StartAmount;
            TargetReached = false;
            // no effect on recording or events
        }

        public string ShowCompletion () {
#if FULLLOG
            MoreDebug.Log (Description);
#endif

            string s = actualAmount + " of " + TargetAmount;
            return s;
        }

        public void Increment () {
#if FULLLOG
            MoreDebug.Log (Description);
#endif
            if ((!TargetReached) && recording) {
                actualAmount++;
                onAmountChange.Invoke ();
                Debug.Log ("Event fired:" + ShowCompletion ());
                if (actualAmount >= TargetAmount) {
                    TargetReached = true;
                    onCriterionMet.Invoke ();
                }
            }
        }

#if FULLLOG

        public string StringDump () {
            string s = Description + ": " + StartAmount + "/" + actualAmount + "/" + TargetAmount + "(" +
                TargetReached + ", " + recording + ")";
            return s;

        }

#endif

    }

}