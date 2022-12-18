﻿using System;
using System.Collections.Generic;
using Utils;
using Levels.Rooms;

namespace Levels.Rooms.Secret
{
	public abstract class SecretRoom : Room
	{

		private static float[] baseRegionSecrets = new float[]{2f, 2.25f, 2.5f, 2.75f, 3.0f};
		private static int[] regionSecretsThisRun = new int[5];

		public static void initForRun()
		{

			float[] regionChances = baseRegionSecrets.clone();

			for (int i = 0; i < regionSecretsThisRun.length; i++){
				regionSecretsThisRun[i] = (int)regionChances[i];
				if (Random.Float() < regionChances[i] % 1f){
					regionSecretsThisRun[i]++;
				}
			}

			//runSecrets = new List<>(ALL_SECRETS);
			//RandomNumberGenerator.shuffle(runSecrets);

		}
	}
}
