// #define FULLLOG
#undef FULLOG

using System;
using System.Collections.Generic;
using UnityEngine;
#if FULLLOG
using Goldraven.Generic;
#endif

namespace Goldraven.Mgmt {

  public class PrefabLibrary : MonoBehaviour {

    /*
     *    cmcb   2019/10/30
     *
     *    Maintains a repository for objects that will begin doing something as soon as they are
     *    instantiated, that you don't want happening at scene start.
     *
     *    Do not populate with anything except prefabs; it kind of defeats the purpose.
     *
     *
     *
     */

    public GameObject[] TheLibrary;

    private Dictionary<string, int> TheCatalog;

    void Awake () {
      RebuildCatalog ();
    }

    void Start () {
#if FULLLOG
      ListAll ();
#endif

    }

    private void RebuildCatalog () {
      TheCatalog.Clear ();
      for (int cc = 1; cc < TheLibrary.Length; cc++) {
        TheCatalog.Add (TheLibrary[cc].name, cc);
      }

    }

    public void Spawn (string card, Transform place) {
      // spawn once in a place
      Instantiate (TheLibrary[TheCatalog[card]], place.position, place.rotation);
    }

    public void Spawn (string card, Transform[] places) {
      // spawn the same thing multiple places
      for (int cc = 0; cc < places.Length; cc++) {
        Instantiate (TheLibrary[TheCatalog[card]], places[cc].position, places[cc].rotation);
      }
    }
    public void Spawn (string[] cards, Transform[] places) {
      // spawn multiple things in rotation until you run out of places
      int cardsub = 0;
      int cardsize = cards.Length;
      for (int cc = 0; cc < places.Length; cc++) {
        if ((cc - cardsub) >= cardsize) cardsub += cardsize;
        Instantiate (TheLibrary[TheCatalog[cards[cc - (cardsub)]]],
          places[cc].position, places[cc].rotation);
      }
    }

    public void SpawnOn (string card, GameObject place) {
      // spawn once on an object
      Instantiate (TheLibrary[TheCatalog[card]], place.transform);
    }

    public void SpawnOn (string card, GameObject[] places) {
      // spawn the same thing on multiple objects
      for (int cc = 0; cc < places.Length; cc++) {
        Instantiate (TheLibrary[TheCatalog[card]], places[cc].transform);
      }
    }
    public void SpawnOn (string[] cards, GameObject[] places) {
      // spawn multiple things in rotation until you run out of objects to put them on
      // one thing on each object
      int cardsub = 0;
      int cardsize = cards.Length;
      for (int cc = 0; cc < places.Length; cc++) {
        if ((cc - cardsub) >= cardsize) cardsub += cardsize;
        Instantiate (TheLibrary[TheCatalog[cards[cc - cardsub]]], places[cc].transform);
      }
    }

#if FULLLOG
    public void ListAll () {
      MoreDebug.Log ("List of prefab library entries follows: ");

      for (int cc = 0; cc < TheCatalog.Count; cc++) {
        MoreDebug.Log (cc + ". -> " + TheCatalog[cc]);

      }
    }
#endif

  }

}