using UnityEngine;

namespace Goldraven.Interfaces {

  public interface iTriggered {

    /*
     *   cmcb  2019/10/27
     *
     *   This interface defines an object type that is triggered automatically,
     *   usually by a proximity contact.
     */

    #region Properties

    #endregion

    #region Methods

    void ActivateTrigger ();
    void DeactivateTrigger ();

    #endregion

  }

}