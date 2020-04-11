using UnityEngine;
using UnityEngine.UI;

namespace Goldraven.Gui {

    public abstract class MenuManager : UIManager {

        /*
         *  cmcb 2018/12/11
         *
         *  Generic-ish menus
         *
         */

        #region Properties

        protected Button[] MenuChoices;
        protected string[] MenuLabels;

        #endregion

        #region Methods

        // Unity methods
        protected override void Awake () {
            base.Awake ();
            // find all the buttons and put them in MenuChoices
            Component[] buttons = GetComponentsInChildren (typeof (Button), true);
            int buttonCount = buttons.Length;

            MenuChoices = new Button[buttonCount];
            MenuLabels = new string[buttonCount];
            int cc = 0;
            foreach (Button choice in buttons) {
                MenuChoices[cc] = choice;
                MenuLabels[cc] = choice.GetComponentInChildren<Text> ().text;
                cc++;
            }

        }

        protected override void Start () {
            base.Start ();
        }

        // Local methods

        public bool this [string index] {
            get {
                return MenuChoices[findIndex (index)].interactable;
            }
            set {
                MenuChoices[findIndex (index)].interactable = value;

            }
        }

        protected abstract int findIndex (string idxstr);

        public bool this [int index] {
            get {
                return MenuChoices[index].interactable;
            }
            set {
                MenuChoices[index].interactable = value;

            }
        }

        public virtual void DisableButtons () {
            for (int cc = 0; cc < MenuChoices.Length; cc++) {
                this [cc] = false;
            }
        }

        #endregion
    }

}