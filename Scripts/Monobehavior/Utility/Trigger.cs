using UnityEngine;
using UnityEngine.Events;

namespace Goldraven.Utility {

    public class Trigger : MonoBehaviour {

        public TriggerAnchor anchor;
        public bool SingleTriggerOnly = false;
        public bool RandomTrigger = false;
        public float ChanceOfTriggering = 0.5f; // between 0.0 and 1.0
        public bool HasBeenTriggered { get; private set; }

        void Awake () {
            HasBeenTriggered = false;
        }

        void OnTriggerEnter (Collider victim) {
            if (victim.CompareTag ("Player")) {
                // Debug.Log("Attempting to trigger...");
                if ((!SingleTriggerOnly) || (SingleTriggerOnly && (!HasBeenTriggered))) {
                    if (RandomTrigger) {
                        if (UnityEngine.Random.value >= ChanceOfTriggering) {
                            return; // missed triggering
                        } // random, triggered
                    } // non-random, triggered
                    HasBeenTriggered = true;
                    anchor.TriggerTheEvent ();
                }
            }
        }
    }
}