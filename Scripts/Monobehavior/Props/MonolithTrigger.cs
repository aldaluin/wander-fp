using UnityEngine;

namespace Goldraven.Props {

    public class MonolithTrigger : MonoBehaviour {

        public bool SingleTriggerOnly = false;
        public bool RandomTrigger = false;
        public float ChanceOfTriggering = 0.5f; // between 0.0 and 1.0
        public bool Inner = true;
        public bool Outer = true;
        public bool HasBeenTriggered { get; private set; }
        private Monolith anchor;

        void Awake () {
            HasBeenTriggered = false;
            anchor = gameObject.GetComponentInParent<Monolith> ();
            if (anchor == null) {
                Debug.LogError ("Monolith trigger is not attached to any monolith.");
            }
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
                    if (Inner) anchor.DoInnerTrigger ();
                    if (Outer) anchor.DoOuterTrigger ();
                }
            }
        }
    }
}