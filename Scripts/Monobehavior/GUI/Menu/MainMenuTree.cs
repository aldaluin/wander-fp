using Goldraven.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Goldraven.Gui {

    public class MainMenuTree : MonoBehaviour {

        /*
         *  cmcb 2019/02/22
         *
         *  Generic-ish main menus
         *  Requires a quitclean but it's not checked for.
         *
         */

        #region Properties
        // Constants

        // Singleton
        public static MainMenuTree only { get; private set; }
        public static bool exists { get; private set; }

        // Local
        // Main menu manager and top level choices
        public MainMenuManager MainMenu;
        public UIDisplayManager TutorialInput;
        public ListMenuManager CampaignPicker;
        public ListMenuManager MapPicker;
        public ListMenuManager SavegamePicker;
        public UIDisplayManager PrefsInput;
        /*

        public MainMenuManager MainMenu;
        public UIInputManager TutorialInput;
        public ListMenuManager CampaignPicker;
        public ListMenuManager MapPicker;
        public ListMenuManager SavegamePicker;
        public UIInputManager PrefsInput;
        */

        public UnityEvent onCampaignPick, onMapPick, onSavegamePick;

        public bool TutorialActive {
            get {
                if (TutorialInput == null) {
                    return false;
                } else {
                    return MainMenu["Tutorial"];
                }
            }
            set {
                if (TutorialInput == null) {
                    return;
                } else {
                    MainMenu["Tutorial"] = value;
                }
            }
        }

        public bool CampaignActive {
            get {
                if (CampaignPicker == null) {
                    return false;
                } else {
                    return MainMenu["Campaign"];
                }
            }
            set {
                if (CampaignPicker == null) {
                    return;
                } else {
                    MainMenu["Campaign"] = value;
                }
            }
        }

        public bool MapActive {
            get {
                if (MapPicker == null) {
                    return false;
                } else {
                    return MainMenu["Map"];
                }
            }
            set {
                if (MapPicker == null) {
                    return;
                } else {
                    MainMenu["Map"] = value;
                }
            }
        }

        public bool SavegameActive {
            get {
                if (SavegamePicker == null) {
                    return false;
                } else {
                    return MainMenu["Savegame"];
                }
            }
            set {
                if (SavegamePicker == null) {
                    return;
                } else {
                    MainMenu["Savegame"] = value;
                }
            }
        }

        public bool PrefsActive {
            get {
                if (PrefsInput == null) {
                    return false;
                } else {
                    return MainMenu["Prefs"];
                }
            }
            set {
                if (PrefsInput == null) {
                    return;
                } else {
                    MainMenu["Prefs"] = value;
                }
            }
        }

        #endregion

        #region Methods

        // Unity methods
        void Awake () {
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
#if FULLLOG
            MoreDebug.Log ("singleton, nokeep.");
#endif
            if (onCampaignPick == null) {
                onCampaignPick = new UnityEvent ();
            }
            if (onMapPick == null) {
                onMapPick = new UnityEvent ();
            }
            if (onSavegamePick == null) {
                onSavegamePick = new UnityEvent ();
            }
            if (MainMenu == null) {
                Debug.LogError ("MainMenuTree: Main menu not set.");
            }

        }

        void Start () {

#if FULLLOG
            MoreDebug.Log (".");
#endif
        }

        public void StartMenus (bool quitonly = false) {
            if (quitonly) {
                MainMenu.DisableButtons ();
            }
            showmain ();
        }

        private void hideall () {
            TutorialInput.hide ();
            CampaignPicker.hide ();
            MapPicker.hide ();
            SavegamePicker.hide ();
            PrefsInput.hide ();
            MainMenu.hide ();
        }

        public void showtutor () {
            // begin tutorial
            hideall ();
            TutorialInput.show ();
            //EasyMessageBox.Show("Tutorial begins here.",messageBoxTitle:"Placeholder", button1Action:showmain); // STUB:
        }

        public void showmain () {
            // go back to main menu
            hideall ();
            MainMenu.show ();
        }

        public void showcamp () {
            // show campaign selection menu
            hideall ();
            CampaignPicker.show ();
        }

        public void showmap () {
            // show individual map selection menu
            hideall ();
            MapPicker.show ();
        }

        public void showsave () {
            // show savegame menu
            hideall ();
            SavegamePicker.show ();
        }

        public void showprefs () {
            // show preferences
            hideall ();
            PrefsInput.show ();
            //EasyMessageBox.Show("Preferences screen goes here.",messageBoxTitle:"Placeholder", button1Action:showmain); // STUB:
        }

        public void quit () {
            // quit the game after confirming
            hideall ();
            MessageBoxParams param = new MessageBoxParams ();
            param.Message = "Do you really want to stop the game now?";
            param.Button1Text = "Yes";
            param.Button1Action = QuitClean.QuitGame;
            param.Button2Text = "No";
            param.Button2Action = showmain;
            EasyMessageBox.Show (param);
        }

        /*
                public void campaignstub (string campaignname) {
                    // begin selected campaign
                    hideall ();
                    EasyMessageBox.Show (campaignname + " campaign begins here.", messageBoxTitle: "Placeholder", button1Action : showmain);
                    //cb();
                }

                public void mapstub(string mapname){
                    // begin selected map
                    hideall();
                    Debug.Log("Comparing " + mapname + " with " + SceneManager.GetActiveScene().name + ".");
                    // EasyMessageBox.Show("Selected map begins here.",messageBoxTitle:"Placeholder", button1Action:showmain);
                    if (mapname == SceneManager.GetActiveScene().name) {
                        cb();
                    }
                    else {
                        SceneManager.LoadScene(mapname);
                    }
                }

        public void savestub () {
            // resume selected save game
            hideall ();
            EasyMessageBox.Show ("Selected savegame begins here.", messageBoxTitle: "Placeholder", button1Action : showmain);
        }
         */

        #endregion
    }

}