/// Author: Eugene Laptev physicalwalk@gmail.com, http://physicalwalk.joomla.com
/// You are free to use this code in any way you want as long as this note remains intact.
using UnityEngine;

namespace PhysicalWalk
{
	public partial class DampedSpringMotionCopier : MonoBehaviour
	{
		public enum eMotionCopyingType
		{
			INSTANT_REPOSITIONING,
			VIA_DAMPED_SPRING,
			NO_MOTION_COPYING
		};
	
		/// Positional stuff
		[System.Serializable] public class PositionalSpringTweaksAndState
		{
			[TooltipAttribute("The object to copy the motion from via the spring.")]
			public Transform sourceObject;

			public eMotionCopyingType motionCopyingType = eMotionCopyingType.VIA_DAMPED_SPRING;
		
			[TooltipAttribute("Damping dampingCriticality: 0..1-under damped, 1-critically damped, >1-over-damped. See http://en.wikipedia.org/wiki/Damping")]
			public float dampingCriticality = 1.0f;

			[TooltipAttribute("The higher the value the faster motion copying will be. In units of 1/second, roughly.")]
			public Vector3 naturalFrequency = new Vector3(4f, 4f, 4f); /// as in wikipedia

			[TooltipAttribute("If true the spring will be applied in the target object's r.f. This is good for objects that normally move as opposed to stay still.")]
			public bool applySpringInMovingReferenceFrame = true;
		
			public enum eFreezeType
			{
				DONT_FIX_INITIAL_RELATIVE_POSITION,
				IN_WORLD_FRAME,
				IN_SOURCE_LOCAL_FRAME,
			};
			public eFreezeType fixRelativePositionAtStart = eFreezeType.IN_WORLD_FRAME;

			[System.Serializable]
			public class ScalesAndLimits
			{
				public Vector3 displacementScale = Vector3.one;
				public Vector3 maxDisplacement = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
				public Vector3 minDisplacement = new Vector3(-float.MaxValue, -float.MaxValue, -float.MaxValue);
			}
			[TooltipAttribute("Advanced settings")]
			public ScalesAndLimits scalesAndLimits = new ScalesAndLimits();

			[System.NonSerialized] public Vector3 frozenLocalOffset = Vector3.zero;
			[System.NonSerialized] public Vector3 frozenWorldlOffset = Vector3.zero;

			/// State
			[System.NonSerialized] public Vector3 lastSourcePosition;
			[System.NonSerialized] public Vector3 lastPosition;
		
			[System.NonSerialized] public Vector3 springVelocity = Vector3.zero;
		}
	
		public PositionalSpringTweaksAndState positionalSpring = new PositionalSpringTweaksAndState();
	
		private bool resetOnNextUpdate = false;
	
		/// Rotational stuff
		[System.Serializable] public class RotationalSpringTweaksAndState
		{
			[TooltipAttribute("The object to copy the motion from via the spring.")]
			public Transform sourceObject;

			public eMotionCopyingType motionCopyingType = eMotionCopyingType.VIA_DAMPED_SPRING;

			[TooltipAttribute("Damping dampingCriticality: 0..1-under damped, 1-critically damped, >1-over-damped. See http://en.wikipedia.org/wiki/Damping")]
			public float dampingCriticality = 1.0f;
		
			[TooltipAttribute("The higher the value the faster motion copying will be. In units of radians/second, roughly.")]
			public float naturalFrequency = 4f; /// as in wikipedia

			public bool applySpringInMovingReferenceFrame = true;
		
			public bool fixRelativeOrientationAtStart = true;
		
			[System.NonSerialized] public Quaternion frozenLocalOffset;

			/// State
			[System.NonSerialized] public Quaternion lastSourceRotation;
			[System.NonSerialized] public Quaternion lastRotation;

			[System.NonSerialized] public Vector3 springVelocity = Vector3.zero;
		}

		public RotationalSpringTweaksAndState rotationalSpring = new RotationalSpringTweaksAndState();
	

		void Start()
		{
			if(positionalSpring.sourceObject == null &&	rotationalSpring.sourceObject == null)
			{
				Debug.LogWarning("DampedSpringMotionCopier: you need to set positionalSpring.sourceObject and/or rotationalSpring.sourceObject for the component to have any affect.");
				return;
			}

			if(positionalSpring.fixRelativePositionAtStart == PositionalSpringTweaksAndState.eFreezeType.IN_WORLD_FRAME)
			{
				positionalSpring.frozenWorldlOffset = transform.position - positionalSpring.sourceObject.transform.position;
				positionalSpring.lastSourcePosition = transform.position;
			}
			else if(positionalSpring.fixRelativePositionAtStart == PositionalSpringTweaksAndState.eFreezeType.IN_SOURCE_LOCAL_FRAME)
			{
				positionalSpring.frozenLocalOffset = positionalSpring.sourceObject.InverseTransformPoint(transform.position);
				positionalSpring.lastSourcePosition = transform.position;
			}
			else
			{
				positionalSpring.frozenLocalOffset = Vector3.zero;
				positionalSpring.lastSourcePosition = positionalSpring.sourceObject.position;
			}
		
			positionalSpring.lastPosition = transform.position;

			
			
			if(!rotationalSpring.sourceObject)
				Debug.LogError("You need to set rotationalSpring.sourceObject!");

			if(rotationalSpring.fixRelativeOrientationAtStart)
			{
				/// transform.rotation = rotationalSpring.sourceObject.rotation * rotationalSpring.frozenLocalOffset;
				/// =>
				rotationalSpring.frozenLocalOffset = Quaternion.Inverse(rotationalSpring.sourceObject.rotation) * transform.rotation;
				rotationalSpring.lastSourceRotation = transform.rotation;
			}
			else
			{
				rotationalSpring.frozenLocalOffset = Quaternion.identity;
				rotationalSpring.lastSourceRotation = rotationalSpring.sourceObject.rotation;
			}

			rotationalSpring.lastRotation = transform.rotation;
		}
	
		void ApplyPositionalSpring(float zDt)
		{
			if(positionalSpring.sourceObject == null)
				return;

			if(positionalSpring.motionCopyingType == eMotionCopyingType.NO_MOTION_COPYING)
				return;

			Vector3 sourcePosition = positionalSpring.frozenWorldlOffset
				+ positionalSpring.sourceObject.TransformPoint(positionalSpring.frozenLocalOffset);
		
			if(positionalSpring.motionCopyingType  == eMotionCopyingType.INSTANT_REPOSITIONING)
			{
				transform.position = sourcePosition;
				return;
			}
		
			Vector3 sourceVelocity = (sourcePosition - positionalSpring.lastSourcePosition)/zDt;
			Vector3 targetPosition = sourcePosition;
			Vector3 currentRelativePosition = positionalSpring.lastPosition - targetPosition;
			if(positionalSpring.applySpringInMovingReferenceFrame)
				currentRelativePosition += sourcePosition - positionalSpring.lastSourcePosition;
		
			Vector3 newRelativePosition = Vector3.zero;
		
			for(int d=0; d<3; d++)
			{
				float newRelativePosition_d, newSpringVelocity_d;
				DampedSpringGeneralSolution
				(
					 out newRelativePosition_d, out newSpringVelocity_d,
					 currentRelativePosition[d],
					 positionalSpring.springVelocity[d]	- (positionalSpring.applySpringInMovingReferenceFrame ? sourceVelocity[d] : 0.0f),
					 zDt,
					 positionalSpring.dampingCriticality,
					 positionalSpring.naturalFrequency[d]
				);
			
				newRelativePosition[d] = newRelativePosition_d;
				positionalSpring.springVelocity[d] = newSpringVelocity_d + (positionalSpring.applySpringInMovingReferenceFrame ? sourceVelocity[d] : 0.0f);
			}
		
			positionalSpring.lastPosition = targetPosition + newRelativePosition;
		
		
			// Convert to sourceObject's local frame to apply scales and limits
			Vector3 delta = positionalSpring.sourceObject.transform.InverseTransformDirection(newRelativePosition);
			// Apply scale
			delta = Vector3.Scale(delta, positionalSpring.scalesAndLimits.displacementScale);
			// Apply limit
			delta = Vector3.Min(delta, positionalSpring.scalesAndLimits.maxDisplacement);
			delta = Vector3.Max(delta, positionalSpring.scalesAndLimits.minDisplacement);
			// Convert back
			delta = positionalSpring.sourceObject.transform.TransformDirection(delta);
		
		
			transform.position = targetPosition + delta;
		
			positionalSpring.lastSourcePosition = sourcePosition;
		}
	
		void ApplyRotationalSpring(float zDt)
		{
			if(rotationalSpring.sourceObject == null)
				return;

			if(rotationalSpring.motionCopyingType == eMotionCopyingType.NO_MOTION_COPYING)
				return;

			Quaternion sourceRotation = rotationalSpring.sourceObject.rotation * rotationalSpring.frozenLocalOffset;

			if(rotationalSpring.motionCopyingType == eMotionCopyingType.INSTANT_REPOSITIONING)
			{
				transform.rotation = sourceRotation;
				return;
			}

			Vector3 sourceVelocity = ToAngularVelocity(sourceRotation, rotationalSpring.lastSourceRotation, zDt);
			Quaternion currentRelativeRotation = rotationalSpring.lastRotation * Conjugate(sourceRotation);

			/// Moving r.f. does not work well with high damping. Apply a bit of hackery.
			bool useMovingRf = rotationalSpring.applySpringInMovingReferenceFrame && (rotationalSpring.dampingCriticality <= 1f);
			if(useMovingRf)
				currentRelativeRotation = sourceRotation * Conjugate(rotationalSpring.lastSourceRotation) * currentRelativeRotation;
			Vector3 rfVelocity = (useMovingRf ? sourceVelocity : Vector3.zero);
		
			Quaternion newRelativeRotation;
			Vector3 newSpringVelocity;

			DampedSpringGeneralSolution
			(
				out newRelativeRotation, out newSpringVelocity,
				currentRelativeRotation, rotationalSpring.springVelocity - rfVelocity,
				zDt,
				rotationalSpring.dampingCriticality,
				rotationalSpring.naturalFrequency
			);

			/// Limit the spring velocity so that over the timestep zDt it cannot rotate by more than 180 deg.
			float mag = newSpringVelocity.magnitude;
			if (mag * zDt > Mathf.PI)
				newSpringVelocity *= Mathf.PI / mag;

			rotationalSpring.springVelocity = newSpringVelocity + rfVelocity;

			Quaternion newRotation = newRelativeRotation * sourceRotation;

			transform.rotation = newRotation;

			rotationalSpring.lastRotation = newRotation;
			rotationalSpring.lastSourceRotation = sourceRotation;
		}
	
		public void Reset()
		{
			resetOnNextUpdate = true;
		}
	
//		public void FixedUpdate()
 		public void Update()
		{
			if(resetOnNextUpdate)
			{
				transform.position = positionalSpring.sourceObject.TransformPoint(positionalSpring.frozenLocalOffset);

				if(positionalSpring.fixRelativePositionAtStart == PositionalSpringTweaksAndState.eFreezeType.DONT_FIX_INITIAL_RELATIVE_POSITION)
					positionalSpring.lastSourcePosition = positionalSpring.sourceObject.position;
				else
					positionalSpring.lastSourcePosition = transform.position;
		
				positionalSpring.lastPosition = transform.position;
				positionalSpring.springVelocity = Vector3.zero;
			
				resetOnNextUpdate = false;
			}
			else
			{
				ApplyPositionalSpring(Time.deltaTime);
				ApplyRotationalSpring(Time.deltaTime);
			}
		}
	}
}// namespace PhysicalWalk
