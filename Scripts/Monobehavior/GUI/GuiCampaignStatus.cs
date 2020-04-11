using System;
using System.Collections.Generic;
using Goldraven.Mgmt;
using UnityEngine;
using UnityEngine.Events;

namespace Goldraven.Gui {

  public class GuiCampaignStatus : UIDisplayManager {

    /*
     *  cmcb 2019/11/06
     *
     *  display screens for campaign descriptions and completion data
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
    public const string S_assumewin = "assume win";
    public const string S_assumelose = "assume lose";
    public const string S_haswon = "has won";
    public const string S_haslost = "has lost";
    public const string S_active = "active";
    public struct CampaignData {
      public string Title, Description, StartMessage, WinMessage, LoseMessage;
      public string[] PlayerName;
      public int OverlayCount;
      public int LevelCount, CurrentLevel;
      public bool IsActive;

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

    public CampaignData GatherActiveCampaign () {
      CampaignManager cm = CampaignManager.campaign;
      // TODO: Replace hard reference above.
      CampaignData cd = new CampaignData ();
      cd.Title = cm.cname;
      cd.Description = cm.CampaignDescription;
      cd.StartMessage = cm.StartingInfo;
      cd.WinMessage = cm.SuccessInfo;
      cd.LoseMessage = cm.FailureInfo;
      cd.PlayerName = cm.PlayerOverlayNames;
      cd.OverlayCount = cm.CampaignOverlayNames.Length;
      cd.LevelCount = cm.Levels.Length;
      cd.CurrentLevel = cm.currentLevel;
      cd.IsActive = cm.isActive;
      return cd;
    }

    /*
        private CampaignData GatherOneCampaign (int idx) {
          CampaignData cd = new CampaignData ();
          CampaignManager cm = Registry.only.GetObject<CampaignManager> (idx);
          cd.Title = cm.cname;
          cd.Description = cm.CampaignDescription;
          cd.StartMessage = cm.StartingInfo;
          cd.WinMessage = cm.SuccessInfo;
          cd.LoseMessage = cm.FailureInfo;
          cd.PlayerName = cm.PlayerOverlayNames;
          cd.OverlayCount = cm.CampaignOverlayNames.Length;
          cd.LevelCount = cm.Levels.Length;
          cd.CurrentLevel = cm.currentLevel;
          cd.IsActive = cm.isActive;
          cd.RegistryIndex = cm.ListIndex;
          cd.ActualIndex = idx;
          return cd;
        }

        public CampaignData[] GatherAllCampaign () {
          List<CampaignData> allcd = new List<CampaignData> ();
          int ccmax = Registry.only.GetCount<CampaignManager> ();
          for (int cc = 0; cc < ccmax; cc++) {
            allcd.Add (GatherOneCampaign (cc));
          }
          return allcd.ToArray ();
        }

    */
    public Status GoalStateToStatus (GoalManager.GoalState gs) {
      // TODO: Consolidate all these states and statuses.
      if (gs == GoalManager.GoalState.neutral) return Status.active;
      if (gs == GoalManager.GoalState.assumewin) return Status.assumewin;
      if (gs == GoalManager.GoalState.assumelose) return Status.assumelose;
      if (gs == GoalManager.GoalState.realwin) return Status.haswon;
      if (gs == GoalManager.GoalState.reallose) return Status.haslost;
      return Status.inactive;
    }

    public string FormatMessage (CampaignData cd) {
      string s01 = string.Format ("Title: {0}\n\n", cd.Title);
      string s02 = string.Format ("Description: {0}\n", cd.Description);
      string s09 = string.Format ("Level Count: {0}\n", cd.LevelCount);
      string s10 = string.Format ("Current Level: {0}\n", cd.CurrentLevel + 1);

      return s01 + s02 + s09 + s10;
    }

    /*
       public string FormatDebugMessage (CampaignData cd) {
      string s01 = string.Format ("Title: {0}\n\n", cd.Title);
      string s02 = string.Format ("Description: {0}\n", cd.Description);
      string s03 = string.Format ("Start Message: {0}\n", cd.StartMessage);
      string s04 = string.Format ("Win Message: {0}\n", cd.WinMessage);
      string s05 = string.Format ("Lose Message: {0}\n", cd.LoseMessage);
      string s06 = string.Format ("Player Count: {0}\n", cd.PlayerName.Length);
      string s07 = string.Format ("Player Name: {0}\n", cd.PlayerName[0]);
      string s08 = string.Format ("Overlay Count: {0}\n", cd.OverlayCount);
      string s09 = string.Format ("Level Count: {0}\n", cd.LevelCount);
      string s10 = string.Format ("Current Level: {0}\n", cd.CurrentLevel);
      string s11 = string.Format ("Active: {0}\n", cd.IsActive);

      return s01 + s02 + s03 + s04 + s05 + s06 + s07 + s08 + s09 + s10 + s11;
    }

     public string FormatMessage (CampaignData[] cda) {
          string str = "";
          foreach (CampaignData cd in cda) {
            str += FormatMessage (cd);
            str += "----------------------------";
          }
          return str;
        }
    */

    public string StatusToString (Status val) {
      if (val == Status.active) return S_active;
      if (val == Status.assumewin) return S_assumewin;
      if (val == Status.assumelose) return S_assumelose;
      if (val == Status.haswon) return S_haswon;
      if (val == Status.haslost) return S_haslost;
      return S_inactive;
    }

    public override void show () {
      Message = FormatMessage (GatherActiveCampaign ());
      base.show ();
    }

    #endregion
  }
}