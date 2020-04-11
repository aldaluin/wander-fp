using Goldraven.Utility;
using UnityEngine;

namespace Goldraven.Gui {

    public abstract class UIManager : MonoBehaviour {

        /*
         *  cmcb 2018/12/11
         *
         *  Generic-ish non-HUD text displays
         *  Requires a PauseGame somewhere in the scene
         *
         */

        #region Properties

        #endregion

        #region Methods

        // Unity methods
        protected virtual void Awake () { }

        protected virtual void Start () {

        }

        // Local methods
        public virtual void show () {
            gameObject.SetActive (true);
        }

        public virtual void hide () {
            gameObject.SetActive (false);
        }

        #endregion
    }

}