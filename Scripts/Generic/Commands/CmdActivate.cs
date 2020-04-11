using System;
using Goldraven.Interfaces;
using UnityEngine;

namespace Goldraven.Generic.Commands {

  public class CmdActivate : Cmd {

    /*
     *   cmcb  2019/10/28
     *
     *   Concrete class for the activate command
     *
     *
     */

    public CmdActivate (iCmdable rcvr) : base (rcvr) { }

    public override void Execute () {
      Receiver.Activate ();
    }
    public override void Execute (string name, string val) {
      throw new InvalidOperationException ();
    }

  }

}