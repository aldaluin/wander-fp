// #define FULLLOG
#undef FULLOG

using System;
using System.Collections;
using Goldraven.Generic;
using Goldraven.Interfaces;
#if FULLLOG
using Goldraven.Generic;
#endif
using UnityEngine;

namespace Goldraven.Props {

  [RequireComponent (typeof (Rigidbody))]
  public class FlareShot : MonoBehaviour, iCmdable, iTriggered {

    /*
     *   cmcb  2019/10/27
     *
     *   Provides a remotely activated flare that can be added to any
     *   object.
     *
     */

    // TODO: Do the art
    // TODO: Sounds (swish bang)

    #region Properties

    public float LaunchForce = 50f;
    public float Thrust = 10f;
    public float ThrustDelay = 1f;
    public float Height = 100f;
    public Color BoomColor = new Color (1.0f, 0f, 0f, 0f);
    public bool ShowSmoke = true;
    public bool ShowFlash = true;
    private Rigidbody rb = null;
    private DigitalRuby.PyroParticles.FireBaseScript fbs;
    private Light StartLight;
    private float StartLightIntensity;
    private const string StartLightName = "FireBallLight";
    private Light BoomLight;
    private float BoomLightIntensity;
    private const string BoomLightName = "FireBallLight (1)";
    private bool EngineOn = false;
    private bool Blown = false;
    private float StartHeight;
    private float TargetHeight;
    #endregion

    #region Methods

    void Awake () {
#if FULLLOG
      MoreDebug.Log (".");
#endif
      rb = gameObject.GetComponent<Rigidbody> ();
      fbs = gameObject.GetComponentInChildren<DigitalRuby.PyroParticles.FireBaseScript> ();
      StartLight = transform.Find (StartLightName).gameObject.GetComponent<Light> ();
      StartLightIntensity = StartLight.intensity;
      StartLight.intensity = 0f;
      BoomLight = transform.Find (BoomLightName).gameObject.GetComponent<Light> ();
      BoomLightIntensity = BoomLight.intensity;
      BoomLight.intensity = 0f;
      // Height only matters here to keep blowup from blowing up
      // It must be recalculated at launch (because the unit could have moved)
      StartHeight = transform.position.y;
      TargetHeight = StartHeight + Height;
    }

    void Start () {
#if FULLLOG
      MoreDebug.Log (".");
#endif

    }

    void Update () {
      if (EngineOn && !Blown) {
#if FULLLOG
        MoreDebug.Log (".");
#endif
        rb.AddForce (0f, Thrust, 0f, ForceMode.Force);
      }
      if ((transform.position.y > TargetHeight) && (!Blown)) {
#if FULLLOG
        MoreDebug.Log (".");
#endif
        BlowUp ();
      }
    }

    IEnumerator StartEngine (float waitTime) {
#if FULLLOG
      MoreDebug.Log (".");
#endif
      yield return new WaitForSeconds (waitTime);
#if FULLLOG
      MoreDebug.Log (".");
#endif
      EngineOn = true;
      fbs.ManualParticleSystems[2].Play ();
      fbs.ManualParticleSystems[3].Play ();
      //TODO: Turn on the light
      rb.AddForce (0f, Thrust, 0f, ForceMode.Force);

    }

    void BlowUp () {
#if FULLLOG
      MoreDebug.Log (".");
#endif
      Blown = true;
      rb.drag *= 25f;
      fbs.ManualParticleSystems[4].Play ();
      fbs.ManualParticleSystems[5].Play ();
      fbs.ManualParticleSystems[6].Play ();
      fbs.ManualParticleSystems[2].Stop ();
      BoomLight.intensity = BoomLightIntensity;
      Destroy (BoomLight.gameObject, 1f);

    }

    public void Launch () {
#if FULLLOG
      MoreDebug.Log (".");
#endif
      StartHeight = transform.position.y;
      TargetHeight = StartHeight + Height;
      fbs.ManualParticleSystems[0].Play ();
      fbs.ManualParticleSystems[1].Play ();
      StartLight.intensity = StartLightIntensity;
      rb.AddForce (0f, LaunchForce, 0f, ForceMode.Impulse);
      StartCoroutine (StartEngine (ThrustDelay));

    }

    public void Activate () {
#if FULLLOG
      MoreDebug.Log (".");
#endif
      Launch ();
    }

    public void Deactivate () {
#if FULLLOG
      MoreDebug.Log (".");
#endif
      // does nothing
    }
    public void ActivateTrigger () {
#if FULLLOG
      MoreDebug.Log (".");
#endif
      Launch ();
    }

    public void DeactivateTrigger () {
#if FULLLOG
      MoreDebug.Log (".");
#endif
      // does nothing
    }

    public void SetParam (string name, string val) {
#if FULLLOG
      MoreDebug.Log (".");
#endif
      if (name == "Height") {
        Height = float.Parse (val);
      } else if (name == "Color") {
        BoomColor = MoreColor.StringToColor (val);
      } else if (name == "Smoke") {
        ShowSmoke = (val == "true");
      } else if (name == "Flash") {
        ShowFlash = (val == "true");
      } else throw new ArgumentException ();
    }

    #endregion

  }

}