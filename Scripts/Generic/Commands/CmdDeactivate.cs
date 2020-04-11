using System;
using Goldraven.Interfaces;
using UnityEngine;

namespace Goldraven.Generic.Commands {

  public class CmdDeactivate : Cmd {

    /*
     *   cmcb  2019/10/28
     *
     *   Concrete class for the deactivate command
     *
     *
     */

    public CmdDeactivate (iCmdable rcvr) : base (rcvr) { }

    public override void Execute () {
      Receiver.Deactivate ();
    }
    public override void Execute (string name, string val) {
      throw new InvalidOperationException ();
    }

  }

}