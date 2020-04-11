// #define INVECTOR
#undef INVECTOR

using System.Collections;
#if INVECTOR
using Goldraven.IvWeapons;
using Invector.vShooter;
#endif
using Goldraven.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Goldraven.Deprecated {

    /* cmcb  2017/11/12
     *
     * Display time, shots fired, targets hit, and a running score
     *
     * Assumes Invector shooter is being used.
     * Singleton
     */

    public class GuiScoreboard : MonoBehaviour {

        #region Properties
        // Singleton
        public static GuiScoreboard only { get; private set; }
        public static bool exists { get; private set; }

        // Local
        public Timer[] GameTimers;
        public int ActiveTimer = 0;
#if INVECTOR
        private Iv_ShooterManager shooterManager;
#endif

        public Text TimeText = null;
        public Text HitsText = null;
        public Text MarkHitsText = null;
        public Text AmmoText = null;
        public Text KillsText = null;
        public Text MarkKillsText = null;
        public Text ScoreText = null;
        public Text PlaceText = null; // scene location, i.e. trigger collider to walk into
        public Text MarkPlaceText = null;

        public string TimeLabel = "Time: ";
        public string HitsLabel = "Hits: ";
        public string MarkHitsLabel = "MHits: ";
        public string AmmoLabel = "Ammo: ";
        public string KillsLabel = "Kills: ";
        public string MarkKillsLabel = "MKills: ";
        public string PlaceLabel = "Places: ";
        public string MarkPlaceLabel = "MPlaces: ";
        public string ScoreLabel = "Score: ";

        public const string NewLine = "\x0A\x0C";

        public string Score {
            get {
                return ScoreCount.ToString ("D5");
            }
        }

        public int HitValue = 2;
        public int MarkHitValue = 1; // also gets hit
        public int KillValue = 5;
        public int MarkKillValue = 3; // also gets kill
        public int ShotValue = -1;
        public int PlaceValue = 10;
        public int MarkPlaceValue = 5;
        public int TimeOutValue = -100;

        public float DisplaySwitchDelay = 1.5f; // seconds

        public int HitsCount { get; private set; }
        public int MarkHitsCount { get; private set; }
        public int AmmoCount { get; private set; }
        // 0 = unlimited and count is up
        // >0 = limited and counts down
        private bool AmmoUp = true;
        public int KillsCount { get; private set; }
        public int MarkKillsCount { get; private set; }
        public int ScoreCount { get; private set; }
        public int PlaceCount { get; private set; }
        public int MarkPlaceCount { get; private set; }

        public UnityEvent onHit, onKill, onEnter, onMHit, onMKill, onMEnter, onAmmo, onTimer;
        #endregion

        // methods
        // Unity methods

        void Awake () {
            // singleton
            exists = false;
            if (only == null) {
                // DontDestroyOnLoad (gameObject);
                only = this;
            } else if (only != this) {
                Destroy (gameObject);
            }
            exists = true;
            // local init
            if (GameTimers.Length == 0) {
                GameTimers = new Timer[1];
                GameTimers[0] = gameObject.AddComponent<Timer> ();
                ActiveTimer = 0;
            }
            if (AmmoCount == 0) {
                AmmoUp = true;
            } else {
                AmmoUp = false;
            }
            HitsCount = 0;
            MarkHitsCount = 0;
            AmmoCount = 0;
            KillsCount = 0;
            MarkKillsCount = 0;
            ScoreCount = 0;
            PlaceCount = 0;
            MarkPlaceCount = 0;
            // build events
            if (onHit == null) {
                onHit = new UnityEvent ();
            }
            if (onKill == null) {
                onKill = new UnityEvent ();
            }
            if (onEnter == null) {
                onEnter = new UnityEvent ();
            }
            if (onMHit == null) {
                onMHit = new UnityEvent ();
            }
            if (onMKill == null) {
                onMKill = new UnityEvent ();
            }
            if (onMEnter == null) {
                onMEnter = new UnityEvent ();
            }
            if (onAmmo == null) {
                onAmmo = new UnityEvent ();
            }
            if (onTimer == null) {
                onTimer = new UnityEvent ();
            }

        }

        // Use this for initialization
        void Start () {
            // listeners need to be added after Awake()
#if INVECTOR

            if (shooterManager == null) {
                shooterManager = GameObject.FindWithTag ("Player").GetComponent<Iv_ShooterManager> ();
            }
            if (shooterManager == null) {
                Debug.LogError ("Scoreboard has no shooterManager and there is no player tagged.");
            }
            shooterManager.onShoot.AddListener (AddShot);
#endif

            foreach (Timer gt in GameTimers) {
                gt.timerDoneEvent.AddListener (TimeUp);
            }
            gameObject.SetActive (true);
        }

        // Update is called once per frame
        void Update () {
            // Debug.Log("GuiScoreboard updating...");
            TimeText.text = TimeLabel + NewLine + GameTimers[ActiveTimer].ShowElapsed ();
            HitsText.text = HitsLabel + NewLine + HitsCount.ToString ();
            MarkHitsText.text = MarkHitsLabel + NewLine + MarkHitsCount.ToString ();
            KillsText.text = KillsLabel + NewLine + KillsCount.ToString ();
            MarkKillsText.text = MarkKillsLabel + NewLine + MarkKillsCount.ToString ();
            AmmoText.text = AmmoLabel + NewLine + AmmoCount.ToString ();
            ScoreText.text = ScoreLabel + NewLine + ScoreCount.ToString ("D5");
            PlaceText.text = PlaceLabel + NewLine + PlaceCount.ToString ();
            MarkPlaceText.text = MarkPlaceLabel + NewLine + MarkPlaceCount.ToString ();

        }

        // local
        #region Event Triggers

        public void AddHit () {
            HitsCount++;
            ScoreCount += HitValue;
            onHit.Invoke ();
        }

        public void AddShot () {
            if (AmmoUp) {
                AmmoCount++;
            } else {
                AmmoCount--;
            }
            ScoreCount += ShotValue;
            onAmmo.Invoke ();
        }

        public void AddKill () {
            KillsCount++;
            ScoreCount += KillValue;
            onKill.Invoke ();
        }

        public void MarkHit () {
            MarkHitsCount++;
            ScoreCount += MarkHitValue;
            onMHit.Invoke ();
        }

        public void MarkKill () {
            // Debug.Log("Scoreboard mark kill");
            MarkKillsCount++;
            ScoreCount += MarkKillValue;
            onMKill.Invoke ();
        }

        public void AddPlace () {
            PlaceCount++;
            ScoreCount += PlaceValue;
            onEnter.Invoke ();
        }

        public void MarkPlace () {
            MarkPlaceCount++;
            ScoreCount += MarkPlaceValue;
            onMEnter.Invoke ();
        }

        public void TimeUp () {
            ScoreCount += TimeOutValue;
            onTimer.Invoke ();
        }

        #endregion

        // event responder

#if INVECTOR

        public void AddShot (vShooterWeapon weapon) {
            // Debug.Log ("Iv shot seen by scoreboard");
            AddShot ();
        }
#endif

        // display modifiers

        public void ShowMarkHits () {
            if (HitsText != null && MarkHitsText != null && !MarkHitsText.gameObject.activeSelf) {
                HitsText.gameObject.SetActive (false);
                MarkHitsText.gameObject.SetActive (true);
            }
        }

        public void ShowAllHits () {
            if (HitsText != null && MarkHitsText != null && !HitsText.gameObject.activeSelf) {
                MarkHitsText.gameObject.SetActive (false);
                HitsText.gameObject.SetActive (true);
            }
        }

        public void ShowMarkKills () {
            if (KillsText != null && MarkKillsText != null && !MarkKillsText.gameObject.activeSelf) {
                KillsText.gameObject.SetActive (false);
                MarkKillsText.gameObject.SetActive (true);
            }
        }

        public void ShowAllKills () {
            if (KillsText != null && MarkKillsText != null && !KillsText.gameObject.activeSelf) {
                MarkKillsText.gameObject.SetActive (false);
                KillsText.gameObject.SetActive (true);
            }
        }

    }
}