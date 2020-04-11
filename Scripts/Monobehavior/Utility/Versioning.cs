// #define FULLLOG
#undef FULLOG

#if FULLLOG
using Goldraven.Generic;
#endif

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Goldraven.Utility {

  [Serializable]
  public class Versioning : MonoBehaviour {

    /*
     *   cmcb - 2019/10/23
     *
     *    This tracks overall software version in a single place
     *    and makes it available for display.
     *
     */

    #region properties

    const int VersionMajor = 0;
    const int VersionMinor = 1;
    const int VersionBuild = 5;
    const char VersionType = 'a';

    public Text ControlText;

    #endregion

    #region Methods

    public void Awake () {
      if (ControlText != null) {
        ControlText.text += Version ();
      }
    }

    static public string Version () {
      string version = VersionMajor.ToString () + "." + VersionMinor.ToString () + "." +
        VersionBuild.ToString () + VersionType;
      return version;
    }

    #endregion

  }
}