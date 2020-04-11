using Goldraven.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Goldraven.Props {

    public class Scorch : MonoBehaviour {

        public GameObject Physicalgo, Innergo, Middlego, Outergo;
        public float OuterRadius {
            get { return Outergo.transform.localScale.x; }
            set { Outergo.transform.localScale = new Vector3 (value, value, value); }
        }
        public UnityEvent OnOuterTrigger, OnMiddleTrigger, OnInnerTrigger;

        public void DoInnerTrigger () {
            OnInnerTrigger.Invoke ();
        }

        public void DoMiddleTrigger () {
            OnMiddleTrigger.Invoke ();
        }

        public void DoOuterTrigger () {
            OnOuterTrigger.Invoke ();
        }

    }
}