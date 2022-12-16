using System.Collections.Generic;
using UnityEngine;
namespace Utils
{
	public static class VectorExtensions
	{
		public static Vector2 Offset(this Vector2 v, float dy, float dx)
		{
			v.Set(v.x + dx, v.y + dy);
			return v;
		}
	}
}
