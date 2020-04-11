using Goldraven.Utility;
using UnityEngine;

namespace Goldraven.Gui {


    public class PauseMenuTree : MonoBehaviour  {

        /*
        *  cmcb 2018/12/11
        *  cmcb 2019/02/27
        *
        *  Generic-ish pause menus
        *  Requires a quitclean but its not checked for.
        *
         */

        #region Properties
        // Constants

        // Singleton
        public static PauseMenuTree only { get; private set;}
        public static bool exists { get; private set;}

        // Local
        // Pause menu manager and choices
        public PauseMenuManager PauseMenu;
        public UIDisplayManager PlayerInfoDisplay;
        public UIDisplayManager CampaignInfoDisplay;
        public UIDisplayManager LevelInfoDisplay;
        public UIInputManager SavegameInput;
        public UIInputManager PrefsInput;



        #endregion

        #region Methods

        // Unity methods
        void Awake() {
            // singleton
            exists = false;
            if (only == null) {
                // DontDestroyOnLoad (gameObject);
                only = this;
                // Debug.Log("PauseMenuTree: singleton created");
    
            } else if (only != this) {
                Destroy (gameObject);
            }
            exists = true;
            // local init

        }


        public void InvokePauseMenu(){
            showpause();
        }
        
        public void CancelPauseMenu() {
            hideall();
        }
        
        private void hideall(){
            PauseMenu.hide();
            PlayerInfoDisplay.hide();
            CampaignInfoDisplay.hide();
            LevelInfoDisplay.hide();
            SavegameInput.hide();
            PrefsInput.hide();
        }
        
        public void showplayerinfo(){
            // display player character status screen
            hideall();
            PlayerInfoDisplay.show();
            //EasyMessageBox.Show("This is the player status screen.",messageBoxTitle:"Placeholder", button1Action:showpause);
        }

        public void showpause(){
            // go back to pause menu
            hideall();
            PauseMenu.show();
        }

        public void showcampinfo(){
            // display campaign status screen
            hideall();
            CampaignInfoDisplay.show();
            //EasyMessageBox.Show("This is the campaign status screen.",
            //  messageBoxTitle:"Placeholder", button1Action:showpause);

        }

        public void showlevelinfo(){
            // display this level's victory condition(s) and status
            hideall();
            LevelInfoDisplay.show();
            //EasyMessageBox.Show("This is the level victory conditions and  status screen.",
            //    messageBoxTitle:"Placeholder", button1Action:showpause);

        }

        public void showsave(){
            // show savegame menu
            hideall();
            SavegameInput.show();
            //EasyMessageBox.Show("This will be an info box showing the campaign progress has been saved.",
            //    messageBoxTitle:"Placeholder", button1Action:showpause);

        }

        public void showprefs(){
            // show preferences
            hideall();
            PrefsInput.show();
            //EasyMessageBox.Show("Preferences screen goes here.",messageBoxTitle:"Placeholder", button1Action:showpause);
        }
    
        
        public void quitlevel(){
            // quit the level after confirming
            hideall();
            MessageBoxParams param = new MessageBoxParams();
            param.Message = "Do you really want to end this level?";
            param.Button1Text = "Yes";
            param.Button1Action = QuitClean.only.FailScene;
            param.Button2Text = "No";
            param.Button2Action = showpause;
            EasyMessageBox.Show(param);
        }
    
        public void quitgame(){
            // quit the game after confirming
            hideall();
            MessageBoxParams param = new MessageBoxParams();
            param.Message = "Do you really want to quit?";
            param.Button1Text = "Yes";
            param.Button1Action = QuitClean.QuitGame;
            param.Button2Text = "No";
            param.Button2Action = showpause;
            EasyMessageBox.Show(param);
        }

#endregion
    }


}
