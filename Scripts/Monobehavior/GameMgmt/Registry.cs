// #define FULLLOG
#undef FULLOG

using System;
using System.Collections.Generic;
using Goldraven.Interfaces;
using UnityEngine;
#if FULLLOG
using Goldraven.Generic;
#endif

namespace Goldraven.Mgmt {

  public class Registry : MonoBehaviour {

    /*
     *     cmcb 2019/11/17
     *
     *     Object locator
     *
     *     Provides a global PoC for game management objects / services
     *
     */

    // Singleton
    public static Registry only { get; private set; }
    public static bool exists { get; private set; }

    private IDictionary<Type, iUniRegister> ms_registry;
    private IDictionary<Type, List<iMultiRegister>> mg_registry;

    public virtual void Awake () {
      // singleton
#if FULLLOG
      MoreDebug.Log ("singleton, keep.");
#endif
      // singleton
      exists = false;
      if (only == null) {
        DontDestroyOnLoad (gameObject);
        only = this;
#if FULLLOG
        MoreDebug.Log ("Singleton created");
#endif
      } else if (only != this) {
#if FULLLOG
        MoreDebug.LogError ("More than one registry found.");
#endif
        Destroy (gameObject);
#if FULLLOG
        MoreDebug.Log ("Duplicate singleton destroyed.");
#endif
      }
      exists = true;

      // local init

      ms_registry = new Dictionary<Type, iUniRegister> ();
      mg_registry = new Dictionary<Type, List<iMultiRegister>> ();

    }

    public bool Register (iMultiRegister mgr, out int idx) {
      List<iMultiRegister> llist;
      if (!mg_registry.ContainsKey (mgr.GetType ())) {
        llist = MakeList ();
        mg_registry.Add (mgr.GetType (), llist);
      } else {
        llist = mg_registry[mgr.GetType ()];
      }
      llist.Add (mgr);
      idx = llist.Count;
      return true;
      // TODO: Catch exceptions

    }

    public bool Register (iUniRegister mgr) {
      if (ms_registry.ContainsKey (mgr.GetType ())) {
        ms_registry[mgr.GetType ()] = mgr;
      } else {
        ms_registry.Add (mgr.GetType (), mgr);
      }
      return true;
      // TODO: Catch exceptions
    }

    public T GetObject<T> () {
      return (T) ms_registry[typeof (T)];
    }

    /*
        public List<iMultiRegister> GetObjects<T> () {
          return mg_registry[typeof (T)];
          //TODO: Try not to use this
        }
    */

    public T GetObject<T> (int idx) {
      return (T) mg_registry[typeof (T)][idx];
      // TODO: Catch exceptions

    }

    public int GetCount<T> () {
      return mg_registry[typeof (T)].Count;
    }

    public List<iMultiRegister> MakeList () {
      return new List<iMultiRegister> ();
    }

    /*
    public static IEnumerable Cast (this IEnumerable self, Type innerType) {
      var methodInfo = typeof (Enumerable).GetMethod ("Cast");
      var genericMethod = methodInfo.MakeGenericMethod (innerType);
      return genericMethod.Invoke (null, new [] { self }) as IEnumerable;
    }
    */

  }

}