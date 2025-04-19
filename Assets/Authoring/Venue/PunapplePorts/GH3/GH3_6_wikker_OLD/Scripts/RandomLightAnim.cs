using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YARG
{
    public class RandomLightAnim : MonoBehaviour
    {

		public Animator anim;
		public float WaitTimeMin = 4;
		public float WaitTimeMax = 8;

		IEnumerator Start()
		{






			//anim = GetComponent<Animator>();

			while (true)
			{
				yield return new WaitForSeconds(Random.Range(WaitTimeMin, WaitTimeMax));
				anim.SetInteger("LightIndex", Random.Range(0, 2));
				anim.SetTrigger("LightGo");
			}







		}
    }
}
