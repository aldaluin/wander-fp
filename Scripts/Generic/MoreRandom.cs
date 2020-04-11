
namespace Goldraven.Utility {

	static class MoreRandom {

        /*
        *     cmcb 2018/02/25
        *
        *     Shuffle will shuffle array elements in place (pseudo) randomly.
        *     Usage: MoreRandom.Shuffle(anyarray);
		*
         */

		public static void Shuffle<T>(T[] array) {
			int n = array.Length;
			while (n > 1) {
				int k = UnityEngine.Random.Range(0, n--);
				T temp = array[n];
				array[n] = array[k];
				array[k] = temp;
			}
		}
	}
}