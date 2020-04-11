using UnityEngine;

namespace Goldraven.Utility {

	/*
	 * cmcb 2017/10/29
	 *
	 * Just calls DontDestroyOnLoad so the GameObject and
	 * all children will be maintained until the end
	 * of the game.
	 *
	 */

	public class KeepInAllScenes : MonoBehaviour {

		void Awake () {
			DontDestroyOnLoad (gameObject);
		}

	}
}