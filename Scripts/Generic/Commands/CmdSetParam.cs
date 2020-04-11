using System;
using Goldraven.Interfaces;
using UnityEngine;

namespace Goldraven.Generic.Commands {

  public class CmdSetParam : Cmd {

    /*
     *   cmcb  2019/10/28
     *
     *   Concrete class to set command parameters
     *
     *
     */

    public CmdSetParam (iCmdable rcvr) : base (rcvr) { }

    public override void Execute () {
      throw new InvalidOperationException ();
    }
    public override void Execute (string name, string val) {
      Receiver.SetParam (name, val);
    }

  }

}