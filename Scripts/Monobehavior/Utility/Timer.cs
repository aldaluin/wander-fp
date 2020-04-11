// #define FULLLOG
#undef FULLOG

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
#if FULLLOG
using Goldraven.Generic;
#endif

namespace Goldraven.Utility {

    public class Timer : MonoBehaviour {

        /*
         *   cmcb 2018/02/06
         *
         *   Production
         *
         *   Do a timer that can be paused
         *   todo:add a timercancel function and event
         *
         */

        public bool TimerBegun { get; private set; }
        public bool TimerDone { get; private set; }
        public int minutes = 15; // ignored after BeginTimer()
        public string timerDoneMessage = "Time!";
        public bool ManualStart = false; // ignored after BeginTimer()
        public UnityEvent timerBegunEvent, timerCancelEvent, timerExpiredEvent, timerDoneEvent, timerTickEvent;
        private float timerLength; // readonly after Start()
        public bool paused { get; private set; }
        public float currentFinish { get; private set; }
        private float currentLength; // time left after last pause
        private IEnumerator timeUpCoroutine, tickCoroutine;

        void Awake () {
#if FULLLOG
            MoreDebug.Log (".");
#endif
            if (timerBegunEvent == null) {
                timerBegunEvent = new UnityEvent ();
            }
            if (timerCancelEvent == null) {
                timerCancelEvent = new UnityEvent ();
            }
            if (timerExpiredEvent == null) {
                timerExpiredEvent = new UnityEvent ();
            }
            if (timerDoneEvent == null) {
                timerDoneEvent = new UnityEvent ();
            }
            if (timerTickEvent == null) {
                timerTickEvent = new UnityEvent ();
            }
            timerLength = minutes * 60f;
            paused = false;
            TimerBegun = false;
            TimerDone = false;
        }

        void Start () {
#if FULLLOG
            MoreDebug.Log (".");
#endif
            if (!ManualStart) {
                BeginTimer ();
            }
        }

        public void BeginTimer () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name + " (" + TimerBegun + ")");
#endif
            if (!TimerBegun) {
                currentFinish = Time.time + timerLength;
                currentLength = timerLength;
                timeUpCoroutine = TimeUpAlert (currentLength);
                StartCoroutine (timeUpCoroutine);
                tickCoroutine = TickAlert ();
                StartCoroutine (tickCoroutine);
                TimerBegun = true;
                timerBegunEvent.Invoke ();
            }
        }

        public void CancelTimer () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name + " (" + TimerBegun + ")");
#endif
            if (TimerBegun) {
                StopCoroutine (timeUpCoroutine);
                StopCoroutine (tickCoroutine);
                TimerDone = true;
                timerCancelEvent.Invoke ();
                timerDoneEvent.Invoke ();
            }
        }

        public void Pause () {
#if FULLLOG
            MoreDebug.Log (".");
#endif
            StopCoroutine (timeUpCoroutine);
            StopCoroutine (tickCoroutine);
            currentLength = currentFinish - Time.time;
            paused = true;
        }

        public void Resume () {
#if FULLLOG
            MoreDebug.Log (".");
#endif
            currentFinish = Time.time + currentLength;
            paused = false;
            timeUpCoroutine = TimeUpAlert (currentLength);
            StartCoroutine (timeUpCoroutine);
            tickCoroutine = TickAlert ();
            StartCoroutine (tickCoroutine);

        }

        public int Elapsed () {
#if FULLLOG
            MoreDebug.Log (".");
#endif
            if (!TimerBegun) {
                return 0;
            } // else...
            if (TimerDone) {
                return minutes * 60;
            } // else...
            float t;
            if (paused) {
                t = timerLength - currentLength;
            } else {
                t = currentFinish + timerLength - currentLength - Time.time;
            }
            return Mathf.FloorToInt (t);
        }

        public string ShowElapsed () {
#if FULLLOG
            MoreDebug.Log ("go: " + gameObject.name + " (" + TimerBegun + ")");
#endif
            if (TimerDone) {
                return timerDoneMessage;
            } // else...
            int t = Elapsed ();
            int minutes = (t / 60);
            int seconds = (t - minutes * 60);
            return string.Format ("{0:0}:{1:00}", minutes, seconds);

        }

        public int Remaining () {
#if FULLLOG
            MoreDebug.Log (".");
#endif
            if (!TimerBegun) {
                return minutes * 60;
            } // else...
            if (TimerDone) {
                return 0;
            } // else...
            float t;
            if (paused) {
                t = currentLength;
            } else {
                t = currentFinish - Time.time;
            }
            return Mathf.FloorToInt (t);
        }

        public string ShowRemaining () {
#if FULLLOG
            MoreDebug.Log (".");
#endif
            if (TimerDone) {
                return timerDoneMessage;
            } // else...
            int t = Remaining ();
            int minutes = (t / 60);
            int seconds = (t - minutes * 60);
            return string.Format ("{0:0}:{1:00}", minutes, seconds);
        }

        private IEnumerator TimeUpAlert (float waittime) {
#if FULLLOG
            MoreDebug.Log (".");
#endif
            yield return new WaitForSeconds (waittime);
            StopCoroutine (tickCoroutine);
            TimerDone = true;
            timerExpiredEvent.Invoke ();
            timerDoneEvent.Invoke ();
        }

        private IEnumerator TickAlert () {
#if FULLLOG
            MoreDebug.Log (".");
#endif
            while (true) {
                yield return new WaitForSeconds (1f);
                timerTickEvent.Invoke ();
            }

        }
    }

}