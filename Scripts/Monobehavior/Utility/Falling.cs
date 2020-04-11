using UnityEngine;
using UnityEngine.Events;
using Goldraven.Utility;

namespace Goldraven.Utility {

    /*
    *       cmcb 2018/04/08
    *
    *       Utility to make shit happen if an object (e.g. player)
    *       falls off or through the world.
    *
    *       cmcb 2018/04/27
    *       Now requires a quitclean
     */


    public enum FallingReset { Quit, Respawn, RestartLevel, NextLevel, LogOnly }


    public class Falling : MonoBehaviour {

        public FallingReset whatToDo;
        public float LowerLimit = -1000;

        private UnityEvent FallQuitEvent;
        public UnityEvent FallRespawnEvent, FallRestartEvent, FallNextEvent;

        public void Awake() {

            if (FallQuitEvent  == null) FallQuitEvent = new UnityEvent();
            if (FallRespawnEvent  == null) FallRespawnEvent = new UnityEvent();
            if (FallRestartEvent  == null) FallRestartEvent = new UnityEvent();
            if (FallNextEvent  == null) FallNextEvent = new UnityEvent();
        }


        public void Start(){
            FallQuitEvent.AddListener(LogFallQuit);
            if (QuitClean.exists) FallQuitEvent.AddListener(QuitClean.only.FailScene);
            FallRespawnEvent.AddListener(LogFallRespawn);
            FallRestartEvent.AddListener(LogFallRestart);
            FallNextEvent.AddListener(LogFallNext);
        }


        public void Update(){
            if (gameObject.transform.position.y < LowerLimit) {
                switch (whatToDo){
                    case FallingReset.Quit:
                        FallQuitEvent.Invoke();
                        break;
                    case FallingReset.Respawn:
                        FallRespawnEvent.Invoke();
                        break;
                    case FallingReset.RestartLevel:
                        FallRestartEvent.Invoke();
                        break;
                    case FallingReset.NextLevel:
                        FallNextEvent.Invoke();
                        break;
                    case FallingReset.LogOnly:
                    default:
                        Debug.Log(gameObject.name +  " has fallen.");
                        break;
                }
            }
        }

        private void LogFallQuit() {
            Debug.Log(gameObject.name + " has fallen.  Quit event triggered");
        }

        private void LogFallRespawn() {
            Debug.Log(gameObject.name + " has fallen.  Respawn event triggered");
        }

        private void LogFallRestart() {
            Debug.Log(gameObject.name + " has fallen.  Restart level event triggered");
        }

        private void LogFallNext() {
            Debug.Log(gameObject.name + " has fallen.  Next level event triggered");
        }

    }
}