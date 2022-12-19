using System;
namespace Utils
{
	public class Tools
	{

        //bound the given value between a min and max value ( outputs min <= Gate(val) <= max)
	    public static float Gate(float min, float value, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }
}
