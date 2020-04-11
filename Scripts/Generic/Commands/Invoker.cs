using UnityEngine;

namespace Goldraven.Generic.Commands {

  public class Invoker {

    /*
     *   cmcb  2019/10/29
     *
     *   Invokes commands of the Cmd class hierarchy.
     *
     */

    private Cmd command;

    public void SetCommand (Cmd cmd) {
      command = cmd;
    }

    public void ExecuteCommand () {
      command.Execute ();
    }

    public void ExecuteCommand (string name, string val) {
      command.Execute (name, val);
    }

  }

}