using UnityEngine;

namespace Goldraven.Props {

	public class Teleport : MonoBehaviour {

		public Vector3 destination = new Vector3 (-40, 0, 40);
		public GameObject anchor;

		private Transform _location;

		// Use this for initialization
		void Start () {
			if (!anchor) {
				anchor = this.gameObject;
			}

		}

		void OnTriggerEnter (Collider victim) {
			if (victim.CompareTag ("Player")) {
				victim.transform.position = anchor.transform.position + destination;
			}
		}
	}
}