using System;
using System.Reflection;
using UnityEngine;

namespace SS_ScriptHandler

{
	public class DoorColToggler : MonoBehaviour
	{
		private void OnEnable()
		{
			this.ToggleDoorCol(true);
		}

		private void OnDisable()
		{
			this.ToggleDoorCol(false);
		}

		private void ToggleDoorCol(bool state)
		{
			if (this.door == null)
			{
				this.door = base.GetComponent<GPButtonTrapdoor>();
			}
			Transform transform = (Transform)typeof(GPButtonTrapdoor).GetField("walkCol", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this.door);
			if (transform == null)
			{
				return;
			}
			transform.gameObject.SetActive(state);
		}

		private GPButtonTrapdoor door;
	}
}

