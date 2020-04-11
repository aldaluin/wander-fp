using Goldraven.Interfaces;
using UnityEngine;

namespace Goldraven.Generic.Commands {

  public abstract class Cmd {

    /*
     *   cmcb  2019/10/28
     *
     *   This abstract is used with iremotable implementations to control objects
     *   through the Activity menu or some other player input method.  Each command
     *   instantiated by iremotable requires it's own subclass of Cmd.
     *
     *
     */

    protected iCmdable Receiver;

    public Cmd (iCmdable rcvr) {
      Receiver = rcvr;
    }

    public abstract void Execute ();
    public abstract void Execute (string name, string val);

  }

}