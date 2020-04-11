using UnityEngine;
using UnityEngine.UI;

namespace Goldraven.Gui {

    /* cmcb  2019/05/22
     *
     * Display running status information with a slot system
     * Hardcoded for eight slots
     *
     * Leaving as singleton for now
     * Not persistent
     */

    public class StatusGui : MonoBehaviour {

        #region Properties
        // Singleton
        public static StatusGui only { get; private set; }
        public static bool exists { get; private set; }

        // Local

        public Text Slot1 = null;
        public Text Slot2 = null;
        public Text Slot3 = null;
        public Text Slot4 = null;
        public Text Slot5 = null;
        public Text Slot6 = null;
        public Text Slot7 = null;
        public Text Slot8 = null;

    
    
        public const string NewLine = "\x0A\x0C";
        // public const float DisplaySwitchDelay = 1.5f; // seconds

        #endregion


        // methods
        // Unity methods

        void Awake ()
        {
            // singleton
            exists = false;
            if (only == null) {
                // DontDestroyOnLoad (gameObject);
                only = this;
            } else if (only != this) {
                Destroy (gameObject);
            }
            exists = true;
            // local init
        }

        // Use this for initialization
        void Start() {
            gameObject.SetActive(true);
        }


        // local
        #region Event Triggers

        public void ChangeValue(int slot, string label, string newval){
            switch (slot ){
                case 1:
                    Slot1.text = label + NewLine + newval;
                    break;
                case 2:
                    Slot1.text = label + NewLine + newval;
                    break;
                case 3:
                    Slot1.text = label + NewLine + newval;
                    break;
                case 4:
                    Slot1.text = label + NewLine + newval;
                    break;
                case 5:
                    Slot1.text = label + NewLine + newval;
                    break;
                case 6:
                    Slot1.text = label + NewLine + newval;
                    break;
                case 7:
                    Slot1.text = label + NewLine + newval;
                    break;
                case 8:
                    Slot1.text = label + NewLine + newval;
                    break;
                default:
                    Slot1.text = label + NewLine + newval;
                    break;
            }
        }

        #endregion

        // event responder
        

        // display modifiers


    }
}
