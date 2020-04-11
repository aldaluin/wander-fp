using System;
using System.Collections.Generic;
using Goldraven.Mgmt;
using UnityEngine;
using UnityEngine.Events;

namespace Goldraven.Gui {

  public class GuiLevelStatus : UIDisplayManager {

    /*
     *  cmcb 2019/11/06
     *
     *  display screens for operation descriptions and completion data
     *
     */

    public enum Status {
      inactive,
      assumewin,
      assumelose,
      haswon,
      haslost,
      active
    }

    public const string S_inactive = "inactive";
    public const string S_assumewin = "winning";
    public const string S_assumelose = "losing";
    public const string S_haswon = "has won";
    public const string S_haslost = "has lost";
    public const string S_active = "indeterminate";
    public struct LevelOpData {
      public string LevelDescription;
      public string MapName;
      public string OpName;
      public string PlayerName;
      public int MapOverlayCount;
      public string OpDescription;
      public string StartingInfo;
      public int MissionCount;
      public Status OpStatus;

    }

    public struct MissionData {
      public string Title, Description, StartMessage, WinMessage, LoseMessage, Level;
      public Status CurrentStatus;
      public bool IsActive;
      public int RegistryIndex, ActualIndex;
    }

    public struct GoalData {
      public Status InitialStatus, CurrentStatus;
      public bool IsActive;
      public int RegistryIndex, ActualIndex;

    }

    public struct CriteriaData {
      public string Description;
      public bool IsActive, IsDone;
      public int StartAmt, TargetAmt, CurrentAmt;
      public int RegistryIndex, ActualIndex;

    }

    #region Properties

    #endregion

    #region Methods

    // Unity methods
    protected override void Awake () {
      base.Awake ();
      // does not appear to be functioning
    }

    protected override void Start () {
      base.Start ();
      // does not appear to be functioning
    }

    // Local methods

    public LevelOpData GatherSummary () {
      LevelOpData smy = new LevelOpData ();
      LevelManager lm = Registry.only.GetObject<LevelManager> ();
      OperationManager opm = Registry.only.GetObject<OperationManager> ();
      smy.LevelDescription = lm.Description;
      smy.MapName = lm.MapOverlayName;
      smy.OpName = lm.OperationOverlayName;
      smy.PlayerName = lm.DefaultPlayerName;
      if (lm.MapOverlayNames == null) smy.MapOverlayCount = 0;
      else smy.MapOverlayCount = lm.MapOverlayNames.Length;
      smy.OpDescription = opm.OperationDescription;
      smy.StartingInfo = opm.StartingInfo;
      smy.MissionCount = opm.Missions.Length;
      smy.OpStatus = Status.active;
      return smy;

    }

    public MissionData[] GatherMissions () {
      List<MissionData> allmd = new List<MissionData> ();
      int ccmax = Registry.only.GetCount<MissionManager> ();
      for (int cc = 0; cc < ccmax; cc++) {
        MissionData md = new MissionData ();
        MissionManager mm = Registry.only.GetObject<MissionManager> (cc);
        md.Title = mm.MissionTitle;
        md.Description = mm.MissionDescription;
        md.StartMessage = mm.StartingInfo;
        md.WinMessage = mm.SuccessInfo;
        md.LoseMessage = mm.FailureInfo;
        md.Level = mm.CurrentLevel;
        md.CurrentStatus = GoalStateToStatus (mm.currentState);
        md.IsActive = mm.isActive;
        md.RegistryIndex = mm.ListIndex;
        md.ActualIndex = cc;
        allmd.Add (md);
      }
      return allmd.ToArray ();
    }

    public GoalData[] GatherGoals () {
      List<GoalData> allgd = new List<GoalData> ();
      int ccmax = Registry.only.GetCount<GoalManager> ();
      for (int cc = 0; cc < ccmax; cc++) {
        GoalData gd = new GoalData ();
        GoalManager gm = Registry.only.GetObject<GoalManager> (cc);
        gd.InitialStatus = GoalStateToStatus (gm.initialState);
        gd.CurrentStatus = GoalStateToStatus (gm.currentState);
        gd.IsActive = gm.isActive;
        gd.RegistryIndex = gm.ListIndex;
        gd.ActualIndex = cc;
        allgd.Add (gd);
      }
      return allgd.ToArray ();
    }

    public Status GoalStateToStatus (GoalManager.GoalState gs) {
      if (gs == GoalManager.GoalState.neutral) return Status.active;
      if (gs == GoalManager.GoalState.assumewin) return Status.assumewin;
      if (gs == GoalManager.GoalState.assumelose) return Status.assumelose;
      if (gs == GoalManager.GoalState.realwin) return Status.haswon;
      if (gs == GoalManager.GoalState.reallose) return Status.haslost;
      return Status.inactive;
    }

    /*
        public string FormatDebugSummary (LevelOpData lvl) {
          string st01 = string.Format ("Level Description: {0}\n", lvl.LevelDescription);
          string st02 = string.Format ("Map File: {0}\n", lvl.MapName);
          string st03 = string.Format ("Operation File: {0}\n", lvl.OpName);
          string st04 = string.Format ("Player File: {0}*\n", lvl.PlayerName);
          string st05 = string.Format ("Map Count: {0}\n", lvl.MapOverlayCount);
          string st06 = string.Format ("Operation Description: {0}\n", lvl.OpDescription);
          string st07 = string.Format ("Starting Info: {0}\n", lvl.StartingInfo);
          string st08 = string.Format ("Mission Count: {0}\n", lvl.MissionCount);
          return st01 + st02 + st03 + st04 + st05 + st06 + st07 + st08;
        }

        public string FormatDebugMissions (MissionData[] mda) {
          string str = "";
          string spacer = "------------------------\n";
          foreach (MissionData md in mda) {
            string st01 = string.Format ("Mission Title: {0}\n", md.Title);
            string st02 = string.Format ("Description: {0}\n", md.Description);
            string st03 = string.Format ("Start Message: {0}\n", md.StartMessage);
            string st04 = string.Format ("Win Message: {0}\n", md.WinMessage);
            string st05 = string.Format ("Lose Message: {0}\n", md.LoseMessage);
            string st05a = string.Format ("Operation name: {0}\n", md.Opname);
            string st06 = string.Format ("Status: {0}\n", md.CurrentStatus);
            string st07 = string.Format ("Active: {0}\n", md.IsActive);
            string st08 = string.Format ("Registry Index: {0}\n", md.RegistryIndex);
            string st09 = string.Format ("Actual Index: {0}\n", md.ActualIndex);
            str = str + spacer + st01 + st02 + st03 + st04 + st05 + st05a + st06 + st07 + st08 + st09;
          }
          return str;
        }

    */
    public string FormatSummary (LevelOpData lvl) {
      string st01 = string.Format ("Level Description: {0}\n\n", lvl.LevelDescription);
      string st02 = string.Format ("Location: {0}\n", lvl.MapName);
      string st03 = string.Format ("Operation: {0}\n", lvl.OpName);
      string st06 = string.Format ("Operation: {0}\n", lvl.OpDescription);
      string st08 = string.Format ("Missions in the Op: {0}\n", lvl.MissionCount);
      return st01 + st02 + st03 + st06 + st08;
    }

    public string FormatMissions (MissionData[] mda) {
      string str = "";
      string spacer = "------------------------\n";
      foreach (MissionData md in mda) {
        string st01 = string.Format ("Mission: {0}\n", md.Title);
        string st02 = string.Format ("Description: {0}\n", md.Description);
        string st05a = string.Format ("Level: {0}\n", md.Level);
        string st06 = string.Format ("Status: {0}\n", StatusToString (md.CurrentStatus));
        string st07 = string.Format ("Active: {0}\n", md.IsActive);
        str = str + spacer + st01 + st02 + st05a + st06 + st07;
      }
      return str;
    }

    public string FormatMessage (LevelOpData lvl, MissionData[] mda) {
      // Get level data
      // Get op data
      // For each mission, get mission data (optionally suppressing missions that haven't started)
      // For each goal in each mission, get goal data
      // If count required is one, display counts as bool instead
      // For each end, get end data
      return FormatSummary (lvl) + FormatMissions (mda);
    }

    public string StatusToString (Status val) {
      if (val == Status.active) return S_active;
      if (val == Status.assumewin) return S_assumewin;
      if (val == Status.assumelose) return S_assumelose;
      if (val == Status.haswon) return S_haswon;
      if (val == Status.haslost) return S_haslost;
      return S_inactive;
    }

    public override void show () {
      Message = FormatMessage (GatherSummary (), GatherMissions ());
      base.show ();
    }

    #endregion
  }
}