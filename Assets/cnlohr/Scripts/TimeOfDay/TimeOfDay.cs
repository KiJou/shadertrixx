﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using BrokeredUpdates;

namespace cnlohr
{
	[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
	public class TimeOfDay : UdonSharpBehaviour
	{
		public Light lightToControl;
		public Material matSquareStarrySky;
		private bool bHeld;
		Quaternion qch_mirror;
		
		void Start()
		{
			qch_mirror = new Quaternion( 0, 1, 0, 0 );  //Flips sun around over the floor.
		}
		
		override public void OnPickup ()
		{
			bHeld = true;
		}
		
		override public void OnDrop()
		{
			bHeld = false;
		}

		void Update()
		{
			if( !bHeld )
			{
				transform.localRotation *= Quaternion.Euler((float)((Time.deltaTime)*.5f),0,0);
			}

			Quaternion quat = transform.localRotation;

			matSquareStarrySky.SetInt( "_UseInputBasis", 1 );
			matSquareStarrySky.SetVector( "_InputBasisQuaternion", new Vector4( quat.x, quat.y, quat.z, quat.w ) );

			Vector3 OriginalLightAngle = quat * new Vector3( 0, 0, 1 );
			if( OriginalLightAngle.y > 0 )
			{
				// At night, need to reflect around y.  To do this, it's a little
				// weird, but we rotate around XZ. Note that this only works
				// if transforming the light vector since it would break chirality.
				//Alternatively, output = .x = -.z, .y = .w, .z = .x, .w = -.y, if you don't want to do the work of the multiply
				//But, Udon makes this faster.
				quat = quat * qch_mirror;
				
				lightToControl.intensity = .04f;
			}
			else
			{
				lightToControl.intensity = 1.0f;
			}
			lightToControl.transform.localRotation = quat;
			quat = transform.localRotation;
		}
	}
}
