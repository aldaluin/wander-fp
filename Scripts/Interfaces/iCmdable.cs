using UnityEngine;

namespace Goldraven.Interfaces {

  public interface iCmdable {

    /*
     *   cmcb  2019/10/27
     *
     *   This interface defines an object type that can be controlled in some way
     *   through the Activity menu or some other player input method.  It lists the
     *   commands that are to instantiated by the object to be controlled.
     *
     *   Each command below also needs a unique subclass of Cmd.
     *
     */

    // TODO: Write the invoker class
    // TODO: Write the activity menu and registration

    #region Properties

    #endregion

    #region Methods

    void Activate ();
    void Deactivate ();
    void SetParam (string name, string val);

    #endregion

  }

}