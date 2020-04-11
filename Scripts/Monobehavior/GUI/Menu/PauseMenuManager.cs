using System;
using UnityEngine;

namespace Goldraven.Gui {

    public class PauseMenuManager : MenuManager {

        /*
         *  cmcb 2018/12/11
         *
         *  Generic-ish pause menu
         *
         */

        #region Properties

        public readonly string[] ButtonNames = {
            "PlayerInfo",
            "CampaignInfo",
            "LevelInfo",
            "Savegame",
            "Prefs",
            "QuitLevel",
            "QuitGame"
        };

        #endregion

        #region Methods

        // Unity methods
        protected override void Awake () {
            base.Awake ();
        }

        protected override void Start () {
            base.Start ();
        }

        // local methods

        protected override int findIndex (string idxstr) {
            for (int cc = 0; cc < ButtonNames.Length; cc++) {
                if (ButtonNames[cc] == idxstr) {
                    return cc;
                } // else nothing
            }
            throw new IndexOutOfRangeException ();
        }

        #endregion
    }

}