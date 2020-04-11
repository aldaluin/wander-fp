using UnityEngine;

namespace Goldraven.Generic {

	static class MoreColor {

		/*
		 *     cmcb 2019/10/29
		 *
		 *     Handy conversions for use with Unity colors.
		 *
		 *
		 */

		public static string RgbToString (float r, float g, float b, float a) {
			return new Color (r, g, b, a).ToString ();
		}

		public static string RgbToString (byte r, byte g, byte b, byte a) {
			return new Color32 (r, g, b, a).ToString ();
		}

		public static Color StringToColor (string str) {
			//TODO: Find out what Color.ToString looks like and reverse it here.
			return Color.grey;
		}

	}
}