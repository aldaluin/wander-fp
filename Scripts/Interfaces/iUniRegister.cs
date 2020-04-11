using System.Collections.Generic;
using Goldraven.Mgmt;
using UnityEngine;

namespace Goldraven.Interfaces {

  public interface iUniRegister {

    /*
     *   cmcb  2019/11/18
     *
     *   This interface defines an object type that can be added to Registry.cs
     *    and uses that registry to decouple itself from callers.
     *
     *    This version is for registering single instances.
     *    See iMultiRegister to register multiple instances.
     *
     */

    #region Properties

    #endregion

    #region Methods

    bool RegisterSelf ();

    #endregion

  }

}