using System;
using Goldraven.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Goldraven.Gui {

    [Serializable]
    public struct ButtonStruct {
        public string name;
        public string label;
        public bool active;
        public Button.ButtonClickedEvent onClick;
    }

    public class ActionMenuManager : MenuManager {

        /*
         *  cmcb 2018/12/11
         *
         *  Generic-ish pause menu without fixed button names
         *
         */

        #region Properties

        public string[] ActionButtonLabels = { "bt1", "bt2", "bt3", "bt4", "bt5", "bt6" };
        public readonly string YankButtonLabel = "Do Emergency Yank";
        private const int YankButtonIndex = 7;
        public bool YankIsActive {
            get {
                return MenuChoices[YankButtonIndex].interactable;
            }
            set {
                MenuChoices[YankButtonIndex].interactable = value;
            }
        }

        public new ButtonStruct this [int index] {
            get {
                ButtonStruct bs = new ButtonStruct ();
                bs.name = MenuChoices[index].name;
                bs.label = MenuLabels[index];
                bs.active = MenuChoices[index].interactable;
                bs.onClick = MenuChoices[index].onClick;
                return bs;
            }
            set {
                MenuChoices[index].interactable = value.active;
                MenuChoices[index].name = value.name;
                MenuLabels[index] = value.label;
                MenuChoices[index].onClick = value.onClick;

            }
        }
        #endregion

        #region Methods

        // Unity methods
        protected override void Awake () {
            base.Awake ();
            for (int cc = 0; cc < ActionButtonLabels.Length; cc++) {
                ButtonStruct bs = this [cc];
                bs.name = ActionButtonLabels[cc];
                bs.label = ActionButtonLabels[cc];
                bs.active = false;
                bs.onClick = new Button.ButtonClickedEvent ();
                this [cc] = bs;
            }
        }

        protected override void Start () {
            base.Start ();
        }

        // local methods

        protected override int findIndex (string idxstr) {
            for (int cc = 0; cc < ActionButtonLabels.Length; cc++) {
                if (ActionButtonLabels[cc] == idxstr) {
                    return cc;
                } // else nothing
            }
            throw new IndexOutOfRangeException ();
        }

        #endregion
    }
}