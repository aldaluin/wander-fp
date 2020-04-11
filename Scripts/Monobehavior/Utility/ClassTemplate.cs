#define FULLLOG
// #undef FULLOG


using Goldraven.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Goldraven.Utility {
    
    public class ClassTemplate : MonoBehaviour {
        
        /*
        *       cmcb yyyy/mm/dd
        *
        *       Description here.
        *
        */
    
        #region Properties
        // Constants
    
        // Static
    
        // Local
    
    
        // Inspector entries
    
    
        // Internal
    

        #endregion
    
    
    
    
        #region Methods
    
        // Unity methods
    
        void Awake() {
            #if FULLLOG
            MoreDebug.Log("go: " + gameObject.name);
            #endif
        
            // local init
        
            // Create events
            /*
            if (OnEvent == null) {
                OnEvent = new UnityEvent();
            }
            */
            
            // set internal variables
        
        
        }
    
    
        void Start() {
            #if FULLLOG
            MoreDebug.Log("go: " + gameObject.name);
            #endif
        
        
        }
    
        void OnDestroy() {
            #if FULLLOG
            MoreDebug.Log("go: " + gameObject.name);
            #endif
        
            // Remove events
        
        }
    
        // local methods
    
    
    
    
    
    
    
    
    
    
    
    
    
        #endregion
    
    
    }
}