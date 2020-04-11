using System;
using System.Linq;
using UnityEngine.Events;

namespace Goldraven.Generic {

    //[Serializable]
    public class ActivityCounter {

        /*      cmcb    2019/06/06
         *
         *
         *       Counts things and groups them on completion
         *       This needs better docs; I don't remember wtf it does.
         *
         */

        #region Properties

        public const string nullstring = "";
        public const string winstring = "win";
        public const string losestring = "lose";
        public const string drawstring = "draw";

        public int CurrentCount { get; private set; }
        public int MaximumCount { get; private set; }
        public int[] GroupCount { get; private set; }
        public string[] GroupNames { get; private set; }

        #endregion

        #region Methods

        // constructors

        public ActivityCounter (int numberofgroups, string[] groupnames) {

            CurrentCount = 0;
            MaximumCount = 0;
            GroupCount = new int[numberofgroups];
            GroupNames = new string[numberofgroups];
            int cc = 0;
            int cc1 = Math.Min (numberofgroups, groupnames.Length);
            while (cc < cc1) {
                GroupCount[cc] = 0;
                GroupNames[cc] = groupnames[cc];
                cc++;
            }
            while (cc < numberofgroups) {
                GroupCount[cc] = 0;
                GroupNames[cc] = nullstring;
                cc++;
            }
        }

        public ActivityCounter (int numberofgroups) : this (numberofgroups, new string[numberofgroups]) {

        }

        public ActivityCounter () : this (2) {

        }

        // getters

        public bool isValid () {
            return CurrentCount >= 0;
        }

        public bool isActive () {
            return (isValid () && (MaximumCount > 0));
        }

        public bool isDone () {
            return ((CurrentCount == 0) && (MaximumCount > 0));
        }

        // setters

        public void IncrementWeighted (int weight) {
            CurrentCount += weight;
            MaximumCount += weight;
        }

        public void Increment () {
            IncrementWeighted (1);
        }

        public void DecrementWeighted (int weight) {
            CurrentCount -= weight;
        }

        public void Decrement () {
            DecrementWeighted (1);
        }

        public void DecrementWeighted (int idx, int weight) {
            DecrementWeighted (weight);
            GroupIncWeighted (idx, weight);
        }

        public void Decrement (int idx) {
            DecrementWeighted (idx, 1);
        }

        public void DecrementWeighted (string str, int weight) {
            int ii = Array.IndexOf (GroupNames, str);
            DecrementWeighted (ii, weight);
        }

        public void Decrement (string str) {
            int ii = Array.IndexOf (GroupNames, str);
            Decrement (ii);
        }

        public void GroupIncWeighted (int idx, int weight) {
            GroupCount[idx] += weight;
        }

        public void GroupInc (int idx) {
            GroupIncWeighted (idx, 1);
        }

        public void GroupIncWeighted (string str, int weight) {
            int ii = Array.IndexOf (GroupNames, str);
            GroupIncWeighted (ii, weight);
        }

        public void GroupInc (string str) {
            int ii = Array.IndexOf (GroupNames, str);
            GroupInc (ii);
        }

        // calculators

        public int MaxVal () {
            return GroupCount.Max ();
        }

        public int MaxIdx () {
            return Array.IndexOf (GroupCount, MaxVal ());
        }

        public int MinVal () {
            return GroupCount.Min ();
        }

        public int MinIdx () {
            return Array.IndexOf (GroupCount, MinVal ());
        }

        #endregion

    }
}