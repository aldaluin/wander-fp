using Goldraven.Mgmt;
using UnityEngine;

namespace Goldraven.Utility {

  public class ForceLevelEnd : MonoBehaviour {

    /*
     *     cmcb 2019/11/20
     *
     *     Looks up the level and forces completion
     */

    public void Now () {

      Registry.only.GetObject<LevelManager> ().EndLevel ();
    }

  }

}