using System.Collections.Generic;
using Goldraven.Mgmt;
using UnityEngine;

namespace Goldraven.Interfaces {

  public interface iMultiRegister {

    /*
     *   cmcb  2019/11/18
     *
     *   This interface defines an object type that can be added to Registry.cs
     *    and uses that registry to decouple itself from callers.
     *
     *    This version is for registering multiple instances.
     *    See iUniRegister to register single (or singleton) instances.
     *
     */

    #region Properties

    // List<iMultiRegister> EntryList { get; } //implement as static
    int ListIndex { get; }

    #endregion

    #region Methods
    bool RegisterSelf (); // and set listindex
    // int AddSelfToList ();

    #endregion

  }

}