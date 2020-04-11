using System;
using System.IO;
using CI.HttpClient;
using Goldraven.Mgmt;
using UnityEngine;
using UnityEngine.UI;

namespace Goldraven.Generic {

  public class PostANote {

    /*
     *   cmcb  2019/11/04
     *
     *   This is used to post a short note to the Goldraven API website.
     *
     *   Parameters must include the game, campaign, and level names.
     *
     */
    [Serializable]
    struct JsonNote {
      public string NoteText;
      public string GameName;
      public string CampaignName;
      public string LevelName;
      public string CharacterName;
    }
    private const string Site = "http://goldravengaming.com/restapi/public/levelnote";
    private const string DefaultCharacterName = "DefaultCharacter";

    static void Post (string Note) {
      JsonNote NoteObj = new JsonNote ();
      NoteObj.NoteText = Note;
      NoteObj.GameName = Application.productName;
      NoteObj.CampaignName = CampaignManager.campaign.cname;
      NoteObj.LevelName = LevelManager.only.gameObject.scene.name;
      NoteObj.CharacterName = DefaultCharacterName;
      string outstr = JsonUtility.ToJson (NoteObj);
      HttpClient client = new HttpClient ();
      StringContent content = new StringContent (outstr);
      client.Post (new System.Uri (Site), content, HttpCompletionOption.AllResponseContent, (r) => {
        Debug.Log (r.ReadAsString ());
      });
    }

  }

}