using System.Collections;
using Goldraven.Utility;
using UnityEngine;

namespace Goldraven.Deprecated {

    public class MarkSet : MonoBehaviour {

        /*
         *       cmcb 2018/04/27
         *
         *       Iterate a special mark through an array of game objects
         *       The array is filled from all IMarkable
         *       children of the markset object.
         */

        // fields

        private IMarkable[] assets;
        private int[] indexes;
        private bool areNoAssets = true;
        public bool isSolid = true;
        public GameObject activeMarkRef = null;
        public Vector3 activeMarkOffset = Vector3.zero;
        public Vector3 activeMarkRotation = Vector3.zero;
        public GameObject usedMarkRef = null;
        public Vector3 usedMarkOffset = Vector3.zero;
        public Vector3 usedMarkRotation = Vector3.zero;
        private int current;

        // methods

        IEnumerator Start () {
            yield return new WaitForSeconds (2);
            assets = GetComponentsInChildren<IMarkable> ();
            indexes = new int[assets.Length];
            Markable m;
            foreach (IMarkable a in assets) {
                m = a.getGameObject ().AddComponent<Markable> ();
                a.setMark (m);
                m.markSet = this;
                m.isSolid = isSolid;
                m.activeMarkRef = activeMarkRef;
                m.activeMarkOffset = activeMarkOffset;
                m.activeMarkRotation = activeMarkRotation;
                m.usedMarkRef = usedMarkRef;
                m.usedMarkOffset = usedMarkOffset;
                m.usedMarkRotation = usedMarkRotation;
            }
            for (int i = 0; i < indexes.Length; i++) {
                indexes[i] = i;
            }
            if (indexes.Length > 0) areNoAssets = false;
            Debug.Log ("Started markset with " + indexes.Length + " indexes and " + assets.Length + " assets.");
            MoreRandom.Shuffle (indexes);

            yield return new WaitForSeconds (2);
            ActivateFirst ();
        }

        public void ActivateFirst () {
            if (areNoAssets) return;
            // Debug.Log("MarkSet activating first");
            current = -1;
            Activate ();
        }

        public void ActivateNext () {
            if (areNoAssets) return;
            // Debug.Log("MarkSet activating next");
            assets[indexes[current]].getMark ().markOn = false;
            Activate ();
        }

        private void Activate () {
            int counter = current;
            do {
                counter++;
                if (counter >= indexes.Length) counter = 0;
            } while (assets[indexes[counter]].isDead () && counter != current);
            if (current == counter) {
                areNoAssets = true;
                Debug.Log ("Markset is empty.");
                return;
            } else {
                current = counter;
                assets[indexes[current]].getMark ().markOn = true;
                Debug.Log ("Markset " + current + " activated on " + assets[indexes[current]].getGameObject ().name);
            }

        }

    }
}