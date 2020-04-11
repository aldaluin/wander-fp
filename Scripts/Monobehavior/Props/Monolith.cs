using Goldraven.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Goldraven.Props {

    public class Monolith : MonoBehaviour {

        public GameObject Spacego, Blankgo, Switchgo;
        public Teleport Teleporter;
        public GameObject Destination;
        public UnityEvent OnOuterTrigger, OnInnerTrigger;

        void Awake () {
            OnOuterTrigger.AddListener (ShowSpace);
            Teleporter.anchor = Destination;
            Teleporter.destination = Vector3.zero;
            ShowBlank ();
        }

        public void ShowSpace () {
            // Blankgo.SetActive (false);
            Spacego.SetActive (true);
        }

        public void ShowBlank () {
            // Blankgo.SetActive (true);
            Spacego.SetActive (false);
        }

        void Start () {
            ShowBlank ();
        }

        public void DoInnerTrigger () {
            OnInnerTrigger.Invoke ();
        }

        public void DoOuterTrigger () {
            OnOuterTrigger.Invoke ();
        }

    }
}