using UnityEngine;
using UnityEngine.Events;

namespace Goldraven.Utility {
    
    public class TriggerAnchor : MonoBehaviour {
    
        public UnityEvent onTrigger;
        
        void Awake(){
            if (onTrigger  == null) onTrigger = new UnityEvent();
    
        }
        
        public void TriggerTheEvent(){
            // Debug.Log("TriggerAnchor: Invoking trigger.");
            onTrigger.Invoke();
        }
    }
}