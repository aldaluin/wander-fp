using System;
using Goldraven.Generic;
using Goldraven.Interfaces;
using UnityEngine;

namespace Goldraven.Props {

  public class SmokeGrenade : MonoBehaviour, iItem, iCmdable {

    /*
     *   cmcb  2019/10/27
     *
     *   Spews colored smoke to mark your location.
     *
     */

    // TODO: Add the effect
    // TODO: Do the art

    #region Properties

    public Color color { get; private set; }

    #endregion

    #region Methods

    void Awake () {

    }

    void Start () {

    }
    public void Activate () {
      //TODO: Need to actually trigger the grenade.
    }

    public void Deactivate () {
      // does nothing
    }

    public void SetParam (string name, string val) {
      if (name == "Color") {
        color = MoreColor.StringToColor (val);
      } else throw new ArgumentException ();
    }

    #endregion

  }

}