using UnityEngine;
using UnityEngine.Events;

namespace Goldraven.Deprecated {

    public interface IMarkable {

        // Implement in anything you want to put in a mark set

        void setMark (Markable mark);
        Markable getMark ();
        bool isMarked ();
        bool isDead ();
        GameObject getGameObject ();
    }

    /*
    // Use  if if you need it, but you shouldn't need it -- cmcb 2018/05/10

    public class MarkableMB : MonoBehaviour, IMarkable{

        private Markable  _mark  = null;

        public  void setMark(Markable mark){
            _mark = mark;
        }

        public Markable  getMark(){
            return _mark;
        }

        public bool isMarked(){
            return _mark != null;
        }

        public GameObject getGameObject(){
            return gameObject;
        }
    }
    */

    public class Markable : MonoBehaviour {

        // properties
        public MarkSet markSet;
        public bool wasUsed { get; private set; } // Was it hit/entered while the mark was on?
        public bool isSolid { get; set; } // Can you not trigger this by walking through it?
        private GameObject _activeMark = null;
        public GameObject activeMarkRef = null;
        public Vector3 activeMarkOffset = Vector3.zero;
        public Vector3 activeMarkRotation = Vector3.forward;
        private GameObject _usedMark = null;
        public GameObject usedMarkRef = null;
        public Vector3 usedMarkOffset = Vector3.zero;
        public Vector3 usedMarkRotation = Vector3.forward;
        private bool _markon = false;

        public bool markOn {
            get {
                return _markon;
            }
            set {
                _markon = value;
                if (value) {
                    // Debug.Log("MarkOn set true");
                    if (activeMarkRef != null) {
                        //Debug.Log("MarkOn instantiating active mark");
                        _activeMark = Instantiate (activeMarkRef, gameObject.transform.position + activeMarkOffset,
                            Quaternion.Euler (activeMarkRotation), gameObject.transform);
                        _activeMark.SetActive (true);
                    }
                    if (_usedMark != null) {
                        // Debug.Log("MarkOn destroying used mark");
                        Destroy (_usedMark);
                        _usedMark = null;
                    }
                } else {
                    // Debug.Log("MarkOn set false");
                    if (usedMarkRef != null) {
                        // Debug.Log("MarkOn instantiating used mark");
                        _usedMark = Instantiate (usedMarkRef, gameObject.transform.position + usedMarkOffset,
                            Quaternion.Euler (usedMarkRotation), gameObject.transform);
                        _usedMark.SetActive (true);
                    }
                    if (_activeMark != null) {
                        // Debug.Log("MarkOn destroying active mark");
                        Destroy (_activeMark);
                        _activeMark = null;
                    }

                }
            }
        } // Is the mark able to be hit/entered?

        // methods

        void Start () {
            wasUsed = false;
        }

        public bool Trigger () {
            if (!markOn) return false;
            markSet.ActivateNext ();
            if (wasUsed) return false;
            wasUsed = true;
            return true;
        }

    }

}