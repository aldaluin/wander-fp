using System;
using Goldraven.Interfaces;
using Goldraven.Props;
using UnityEngine;

namespace Goldraven.Actions {

    [RequireComponent (typeof (SphereCollider))]
    public class ProximitySensor : MonoBehaviour {

        private SphereCollider sc = null;
        public float Radius {
            // the range of the sensor
            get {
                return sc.radius;
            }
            private set {
                sc.radius = value;
            }
        }
        public bool SingleAlarmOnly = false;
        // true = single use only
        // false = can be reset
        public bool ContinuousAlarm = false;
        // true = activates every frame intruder remains
        // false = activates only on the frame intruder enters
        private int IntruderCount; // only matters if continuous alarm

        public float InitialChanceOfAlarming = 1.0f; // between 0.0 and 1.0
        // chance of initial intrusion triggering the alarm
        public float OngoingChanceOfAlarming = 0.0f; // between 0.0 and 1.0
        // if initial intrusion was undetected, chance of triggering while intruder remains
        public float AlarmingRecheckInterval = 5.0f; // in game seconds
        // interval to use when checking ongoing chance of alarm
        private float TimeHack; // used for alarming recheck
        public bool wasTriggered { get; private set; }
        // the alarm was set off and has not been reset (but may have been shut off)
        public bool isAlarming { get; private set; }
        // the sensor is currently in an alarm state
        public iTriggered Anchor { get; private set; }
        // the gameobject this sensor is attached to
        public string IntruderTag = null;
        // the tag that sets off the alarm; null = anything

        void Awake () {
            wasTriggered = false;
            isAlarming = false;
            IntruderCount = 0;
        }

        void Start () {
            sc = gameObject.GetComponent<SphereCollider> ();
            Anchor = gameObject.GetComponentInParent<iTriggered> ();
            if (Anchor == null) {
                Debug.LogError ("Sensor is not attached to a triggerable game object.");
            }
        }

        void OnTriggerEnter (Collider intruder) {
            if (SingleAlarmOnly && wasTriggered) return;
            if (IntruderTag != "")
                if (!intruder.CompareTag (IntruderTag)) return;
            // else...
            // Debug.Log("Attempting to trigger...");
            IntruderCount++;
            if (InitialChanceOfAlarming < 1f) {
                if (UnityEngine.Random.value > InitialChanceOfAlarming) {
                    TimeHack = Time.time + AlarmingRecheckInterval;
                    return; // missed triggering
                } // random, triggered
            } // non-random, triggered
            wasTriggered = true;
            isAlarming = true;
            Anchor.ActivateTrigger ();

        }

        void OnTriggerStay (Collider intruder) {
            if (SingleAlarmOnly && wasTriggered) return;
            if (isAlarming) {
                if (!ContinuousAlarm) {
                    isAlarming = false; // deactivate a discrete alarm on the frame after activation
                    Anchor.DeactivateTrigger ();
                    if (!SingleAlarmOnly) {
                        wasTriggered = false; // auto-reset a discrete, multiple use alarm
                    }
                } // do not deactivate a continuous alarm until ontriggerexit
                return;
            } // else not alarming
            if (wasTriggered) return; // don't try to trigger an alarm that's been shut off already
            if (IntruderTag != "")
                if (!intruder.CompareTag (IntruderTag)) return;
            // Debug.Log("Attempting to trigger...");
            if (Time.time < TimeHack) return;
            if (OngoingChanceOfAlarming < 1f) {
                if (UnityEngine.Random.value > OngoingChanceOfAlarming) {
                    TimeHack = Time.time + AlarmingRecheckInterval;
                    return; // missed triggering
                } // random, triggered
            } // non-random, triggered
            wasTriggered = true;
            isAlarming = true;
            Anchor.ActivateTrigger ();

        }

        void OnTriggerExit (Collider intruder) {
            IntruderCount--;
            if (!ContinuousAlarm) return;
            if (IntruderCount > 0) return;
            isAlarming = false;
            Anchor.DeactivateTrigger ();

        }
    }
}