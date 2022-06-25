using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Attributions
 * https://stackoverflow.com/questions/767999/random-number-generator-only-generating-one-random-number/768001#768001
*/

public class RNG : MonoBehaviour
{
	//Function to get a random number 
	private static readonly System.Random random = new System.Random(); 
	private static readonly object syncLock = new object(); 
	public static int RandomNumber(int min, int max)
	{
		lock(syncLock) { // synchronize
			return random.Next(min, max);
		}
	}
}
