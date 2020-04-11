using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Goldraven.Gui {

    public class ListMenuManager : MenuManager {

        /*
         *  cmcb 2018/12/11
         *
         *  Generic-ish main menus
         *
         */

        [System.Serializable]
        public struct ListElement {
            public string option; // also goes to button name
            public string title; // button text
            public bool clickable; // is button interactable?
            public Button.ButtonClickedEvent onClick; // what to do when clicked
        }

        #region Properties

        public ListElement[] Elements;
        public readonly int MaximumElements = 15;

        #endregion

        #region Methods

        // Unity methods
        protected override void Awake () {
            base.Awake ();
            if (MenuChoices.Length != 2) {
                Debug.LogError ("ListMenuManager: Initial button list needs exactly two items.");
                Debug.Log ("The list has " + MenuChoices.Length + " items.");
            }
            // add a button for each list element
            foreach (ListElement item in Elements) {
                // init and place this button
                Button thisButton = Instantiate (MenuChoices[0]);
                thisButton.transform.SetParent (MenuChoices[0].transform.parent);
                thisButton.name = item.option;
                Text thisText = thisButton.GetComponentInChildren<Text> ();
                thisText.text = item.title;
                thisButton.onClick = item.onClick;
                thisButton.onClick.AddListener (hide);
                // thisButton.onClick.AddListener(delegate()  {MainMenuTree.only.mapstub(item.option);});
                thisButton.interactable = item.clickable;
                // Debug.Log("New button made");
            }
            // move the quit button to the bottom
            MenuChoices[1].transform.SetAsLastSibling ();
            // remove the prototype
            MenuChoices[0].gameObject.SetActive (false);

        }

        protected override void Start () {
            base.Start ();

        }

        // local methods

        protected override int findIndex (string idxstr) {
            for (int cc = 0; cc < Elements.Length; cc++) {
                if (Elements[cc].title == idxstr) {
                    return cc;
                } // else nothing
            }
            throw new IndexOutOfRangeException ();
        }

        /*
            protected sealed override void indexButtons() {

            }

            */

        #endregion
    }

}