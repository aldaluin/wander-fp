using System;

namespace Goldraven.Gui {

    public class MainMenuManager : MenuManager {

        /*
         *  cmcb 2018/12/11
         *
         *  Generic-ish main menu
         *
         */

        #region Properties

        // public readonly string TutorialButtonName, CampaignButtonName, MapButtonName, SavegameButtonName,
        // PrefsButtonName, QuitButtonName;

        public readonly string[] ButtonNames = {
            "Tutorial",
            "Campaign",
            "Map",
            "Savegame",
            "Prefs",
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

        public override void DisableButtons () {
            base.DisableButtons ();
            this [ButtonNames[5]] = true;
        }

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