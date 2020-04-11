using UnityEngine;

namespace Goldraven.Props {

    public class ScorchTrigger : MonoBehaviour {

        public bool SingleTriggerOnly = false;
        public bool RandomTrigger = false;
        public float ChanceOfTriggering = 0.5f; // between 0.0 and 1.0
        public bool Inner = true;
        public bool Middle = true;
        public bool Outer = true;
        public bool HasBeenTriggered { get; private set; }
        private Scorch anchor;

        void Awake () {
            HasBeenTriggered = false;
            anchor = gameObject.GetComponentInParent<Scorch> ();
            if (anchor == null) {
                Debug.LogError ("Scorch trigger is not attached to any scorch object.");
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
                    if (Middle) anchor.DoMiddleTrigger ();
                    if (Outer) anchor.DoOuterTrigger ();
                }
            }
        }
    }
}