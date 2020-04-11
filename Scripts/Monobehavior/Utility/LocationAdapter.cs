// #define FULLLOG
#undef FULLOG

using System;
using System.Collections.Generic;
using UnityEngine;
#if FULLLOG
using Goldraven.Generic;
#endif

namespace Goldraven.Mgmt {

  public enum PlacementType {
    Front,
    Back,
    Random
  }

  public class LocationAdapter : MonoBehaviour {

    /*
     *    cmcb   2019/10/31
     *
     *    Adapts a dis- or semi-organized group of map locations into a series of arrays
     *    that can be used by a prefab library to place things.

     *
     */

    public const string DefaultString = "default";
    private Dictionary<string, List<Transform>> Locations;
    private Dictionary<string, int> NextPtr;
    private List<Transform> w_group;

    void Awake () {
      Locations = new Dictionary<string, List<Transform>> ();
      Locations.Add (DefaultString, new List<Transform> ());
      NextPtr = new Dictionary<string, int> ();
      NextPtr.Add (DefaultString, 0);
      w_group = null;
    }

    public void StoreLocation (
      Transform loc,
      string group = DefaultString,
      PlacementType how = PlacementType.Back
    ) {
      if (!Locations.TryGetValue (group, out w_group)) {
        w_group = new List<Transform> ();
        Locations.Add (group, w_group);
        NextPtr.Add (group, 0);
      }
      AddLocation (loc, w_group, how);
    }

    public void StoreLocations (
      Transform[] locs,
      string group = DefaultString,
      PlacementType how = PlacementType.Back
    ) {
      if (!Locations.TryGetValue (group, out w_group)) {
        w_group = new List<Transform> ();
        Locations.Add (group, w_group);
        NextPtr.Add (group, 0);
      }
      for (int cc = 0; cc < locs.Length; cc++) {
        AddLocation (locs[cc], w_group, how);
      }
    }

    public void StoreAllChildren (
      Transform tp,
      string group = DefaultString,
      PlacementType how = PlacementType.Back
    ) {
      if (!Locations.TryGetValue (group, out w_group)) {
        w_group = new List<Transform> ();
        Locations.Add (group, w_group);
        NextPtr.Add (group, 0);
      }
      for (int cc = 0; cc < tp.childCount; cc++) {
        AddLocation (tp.GetChild (cc), w_group, how);
      }
    }

    public void StoreAllGrands (
      Transform gp,
      string group = DefaultString,
      PlacementType how = PlacementType.Back
    ) {
      for (int cc = 0; cc < gp.childCount; cc++) {
        StoreAllChildren (gp.GetChild (cc), group, how);
      }
    }

    private void AddLocation (Transform loc, List<Transform> grp, PlacementType how) {
      if (how == PlacementType.Back) {
        grp.Add (loc);
      } else if (how == PlacementType.Front) {
        grp.Insert (0, loc);
      } else { // how == Random
        if (grp.Count < 2) {
          grp.Add (loc);
        } else {
          grp.Insert (UnityEngine.Random.Range (1, grp.Count - 1), loc);
        }
      }
    }

    public Transform NextPlace (string group, out bool rolled) {
      rolled = false;
      w_group = Locations[group];
      int nn = NextPtr[group];
      Transform nt = w_group[nn];
      nn++;
      if (nn > w_group.Count) {
        nn = 0;
        rolled = true;
      }
      NextPtr[group] = nn;
      return nt;
    }

    public Transform NextPlace (string group) {
      bool bo;
      return NextPlace (group, out bo);
    }

    public Transform[] NextPlaces (int cnt, string[] groups, bool AllowRollover = true) {
      int gpidx = 0;
      List<Transform> tx = new List<Transform> ();
      bool ro = false;
      for (int cc = 0; cc < cnt; cc++) {
        tx.Add (NextPlace (groups[gpidx], out ro));
        if (ro == true) {
          gpidx++;
          if (gpidx >= groups.Length) {
            if (AllowRollover) {
              gpidx = 0;
            } else {
              return tx.ToArray ();
            }
          } // no else
        } // no else

      }
      return tx.ToArray ();
    }
  }
}