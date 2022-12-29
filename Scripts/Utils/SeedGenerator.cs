using System;
using System.Text;

namespace Utils
{
	public class SeedGenerator
	{

        public static long TOTAL_SEEDS = 5429503678976L;
        public SeedGenerator()
		{
		}

		//Seed codes take the form @@@-@@@-@@@ where @ is any letter from A to Z (only uppercase)
		//This is effectively a base-26 number system, therefore 26^9 unique seeds are possible.

		//Seed codes exist to make sharing and inputting seeds easier
		//ZZZ-ZZZ-ZZZ is much easier to enter and share than 5,429,503,678,975

		//generates a random seed, omitting seeds that contain vowels (to minimize real words appearing randomly)
		//This means that there are 21^9 = 794,280,046,581 unique seeds that can be randomly generated
		public static long RandomSeed()
		{
			long seed;
			string seedText;
			do
			{
				seed = RandomNumberGenerator.Long(TOTAL_SEEDS);
				seedText = ConvertToCode(seed);
			} while (seedText.Contains("A") || seedText.Contains("E") || seedText.Contains("I") || seedText.Contains("O") || seedText.Contains("U"));
			return seed;
		}

		public static String ConvertToCode(long seed)
		{
			// if (seed < 0 || seed >= TOTAL_SEEDS)
			// {
			// 	throw new IllegalArgumentException("seeds must be within the range [0, TOTAL_SEEDS)");
			// }

			//this almost gives us the right answer, but its 0-p instead of A-Z
			String interrim = seed.ToString();
			StringBuilder result = new();

			//so we convert
			for (int i = 0; i < 9; i++)
			{

				if (i < interrim.Length)
				{
					int c = (int)Char.GetNumericValue(interrim[i]);
					if (c <= '9') c += 17; //convert 0-9 to A-J
					else c -= 22; //convert a-p to K-Z

					result.Append(Convert.ToChar(c));

				}
				else
				{
					result.Insert(0, 'A'); //pad with A (zeroes) until we reach length of 9

				}
			}

			//insert dashes for readability
			result.Insert(3, '-');
			result.Insert(7, '-');

			return result.ToString();

		}

	}
}
