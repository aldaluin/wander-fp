using Goldraven.Interfaces;
using UnityEngine;

namespace Goldraven.Props {

  public class FlareGun : MonoBehaviour, iItem {

    /*
     *   cmcb  2019/10/27
     *
     *   Shoots a flare that can hang and mark your location.
     *
     */

    // TODO: Add the effect
    // TODO: Do the art

    #region Properties

    public const int MaxFlares = 5;
    public int CurFlares { get; private set; }
    public

    #endregion

    #region Methods

    void Awake () {
      CurFlares = MaxFlares;
    }

    void Start () {

    }

    void Shoot () {

    }

    #endregion

  }

}