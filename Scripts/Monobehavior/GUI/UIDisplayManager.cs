using System;
using UnityEngine;
using UnityEngine.Events;

namespace Goldraven.Gui {

    public class UIDisplayManager : UIManager {

        /*
         *  cmcb 2019/0222
         *
         *  Generic-ish display screens
         *
         */

        #region Properties

        public int TemplateID = 0;
        public string Title;
        [TextArea]
        public string Header;
        [TextArea]
        public string Message;
        public UnityAction ClickAction;
        [SerializeField]
        private UnityEvent onClick;

        #endregion

        #region Methods

        // Unity methods
        protected override void Awake () {
            base.Awake ();
            // does not appear to be functioning
        }

        protected override void Start () {
            base.Start ();
            // does not appear to be functioning
        }

        // Local methods

        public override void show () {
            MessageBoxParams boxDef = new MessageBoxParams ();
            boxDef.MessageBoxTitle = Title;
            boxDef.MessageTitle = Header;
            boxDef.Message = Message;
            boxDef.TemplateId = TemplateID;
            if (ClickAction != null) {
                boxDef.Button1Action = ClickAction;
            } else {
                string actionname = onClick.GetPersistentMethodName (0);
                object actionobject = onClick.GetPersistentTarget (0);
                boxDef.Button1Action = (UnityAction) Delegate.CreateDelegate (typeof (UnityAction), actionobject, actionname);
            }

            EasyMessageBox.Show (boxDef);
        }

        public override void hide () { }

        #endregion
    }

}