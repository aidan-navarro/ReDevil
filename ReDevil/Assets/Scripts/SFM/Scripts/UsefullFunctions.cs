using UnityEngine;


public static class UsefullFunctions  {

	
	public static void DebugRay (Vector3 origin, Vector3 v, Color c) 
	{
		Debug.DrawRay (origin, v*v.magnitude, c);
	}
	
	
	public static Vector3 ClampMagnitude (Vector3 v, float max) {
		
		if(v.magnitude > max)
			return v.normalized*max;
		else
			return v;
	
	}

	public static float Vec2Magnitude (Vector2 a)
    {
		float result = a.x * a.x + a.y * a.y;
		return result;
    }
}
