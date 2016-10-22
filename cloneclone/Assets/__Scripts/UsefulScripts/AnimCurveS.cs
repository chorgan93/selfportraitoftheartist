using UnityEngine;
using System.Collections;

public class AnimCurveS : MonoBehaviour {
	
	// from http://wpf-animation.googlecode.com/svn/trunk/src/WPF/Animation/PennerDoubleAnimation.cs
	
	/// <summary>
	/// Easing equation function for a quadratic (t^2) easing out: 
	/// decelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Final value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static float QuadEaseOut( float t, float b, float c, float d )
	{
		return -c * ( t /= d ) * ( t - 2 ) + b;
	}
	
	/// <summary>
	/// Easing equation function for a quadratic (t^2) easing in: 
	/// accelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Final value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static float QuadEaseIn( float t, float b, float c, float d )
	{
		return c * ( t /= d ) * t + b;
	}
	
	/// <summary>
	/// Easing equation function for a quadratic (t^2) easing in/out: 
	/// acceleration until halfway, then deceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Final value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static float QuadEaseInOut( float t, float b, float c, float d )
	{
		if ( ( t /= d / 2 ) < 1 )
			return c / 2 * t * t + b;
		
		return -c / 2 * ( ( --t ) * ( t - 2 ) - 1 ) + b;
	}
	
	/// <summary>
	/// Easing equation function for a quadratic (t^2) easing out/in: 
	/// deceleration until halfway, then acceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Final value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static float QuadEaseOutIn( float t, float b, float c, float d )
	{
		if ( t < d / 2 )
			return QuadEaseOut( t * 2, b, c / 2, d );
		
		return QuadEaseIn( ( t * 2 ) - d, b + c / 2, c / 2, d );
	}
}
