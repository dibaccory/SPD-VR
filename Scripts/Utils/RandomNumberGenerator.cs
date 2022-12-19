using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utils
{

    public static class RandomNumberGenerator
    {
        private const ulong V = 0xbea225f9eb34556dL;

        //we store a stack of random number generators, which may be seeded deliberately or randomly.
        //top of the stack is what is currently being used to generate new numbers.
        //the base generator is always created with no seed, and cannot be popped.
        private static List<Random> generators;
        //TODO INITIALIZE resetGenerators();


        public static void ResetGenerators()
        {
            generators = new List<Random> { new Random() };
        }

        public static void AddGenerator() => generators.Add(new Random());

        public static void AddGenerator(long seed)
        {
            generators.Add(new Random((int)ScrambleSeed((ulong)seed)));
        }

        //scrambles a given seed, this helps eliminate patterns between the outputs of similar seeds
        //Algorithm used is MX3 by Jon Maiga (jonkagstrom.com), CC0 license.
        private static long ScrambleSeed(ulong seed)
        {
            seed ^= seed >> 32;
            seed *= V;
            seed ^= seed >> 29;
            seed *= V;
            seed ^= seed >> 32;
            seed *= V;
            seed ^= seed >> 29;
            return (long)seed;
        }

        public static void PopGenerator()
        {
            if (generators.Count == 1)
            {
                // Game.reportException(new RuntimeException("tried to pop the last random number generator!"));
            }
            else
            {
                generators.RemoveAt(generators.Count - 1); //TODO: May need UTIL pop function
            }
        }

        //returns a uniformly distributed double in the range [0, 1)
        public static double Double()
        {
            return generators.Last().NextDouble();
        }

        //returns a uniformly distributed double in the range [0, max)
        public static double Double(double max)
        {
            return Double() * max;
        }

        //returns a uniformly distributed double in the range [min, max)
        public static double Double(double min, double max)
        {
            return min + Double(max - min);
        }

        //returns a triangularly distributed double in the range [min, max)
        public static double NormalDouble(double min, double max)
        {
            return min + ((Double(max - min) + Double(max - min)) / 2f);
        }

        //returns a uniformly distributed int in the range [0, max)
        public static int Int(int max)
        {
            return max > 0 ? generators.Last().Next() : 0;
        }
        //returns a uniformly distributed int in the range [min, max)
        public static int Int(int min, int max)
        {
            return min + Int(max - min);
        }

        //returns a uniformly distributed int in the range [min, max]
        public static int IntRange(int min, int max)
        {
            return min + Int(max - min + 1);
        }

        //returns a triangularly distributed int in the range [min, max]
        public static int NormalIntRange(int min, int max)
        {
            return min + (int)((Double() + Double()) * (max - min + 1) / 2f);
        }

        //returns a uniformly distributed long in the range [-2^63, 2^63)
        public static long Long()
        {
            var buffer = new byte[sizeof(long)];
            generators.Last().NextBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }

        //returns a uniformly distributed long in the range [0, max)
        public static long Long(long max)
        {
            long result = Long();
            if (result < 0) result += long.MaxValue;
            return result % max;
        }

        //returns an index from chances, the probability of each index is the weight values in changes
        public static int Chances(float[] chances)
        {

            int length = chances.Length;

            double sum = 0;
            for (int i = 0; i < length; i++)
            {
                sum += chances[i];
            }

            double value = Double(sum);
            sum = 0;
            for (int i = 0; i < length; i++)
            {
                sum += chances[i];
                if (value < sum)
                {
                    return i;
                }
            }

            return -1;
        }

        //public static <K> K chances(HashMap<K, Float> chances)
        //{

        //    int size = chances.size();

        //    Object[] values = chances.keySet().toArray();
        //    float[] probs = new float[size];
        //    float sum = 0;
        //    for (int i = 0; i < size; i++)
        //    {
        //        probs[i] = chances.get(values[i]);
        //        sum += probs[i];
        //    }

        //    if (sum <= 0)
        //    {
        //        return null;
        //    }

        //    float value = Float(sum);

        //    sum = probs[0];
        //    for (int i = 0; i < size; i++)
        //    {
        //        if (value < sum)
        //        {
        //            return (K)values[i];
        //        }
        //        sum += probs[i + 1];
        //    }

        //    return null;
        //}

        //    public static int Index(Collection<?> collection)
        ////    {
        //        return Int(collection.size());
        //    }

        //    //@SafeVarargs
        public static T OneOf<T>(params T[] array )
        {
            return array[Int(array.Length)];
        }

        public static T Element<T>(T[] array)
        {
            return element(array, array.Length);
        }

        public static T element<T>(T[] array, int max)
        {
            return array[Int(max)];
        }

        //@SuppressWarnings("unchecked")
        public static T Element<T>(List<T> collection)
        {
            int size = collection.Count();
            return size > 0 ?
                collection[Int(size)] :
                default;
        }

        public static void Shuffle<T>(List<T> list)
        {
            list.OrderBy(i => new Random().Next()).ToList();
        }

        public static void Shuffle<T>(T[] array)
        {
            array.OrderBy(i => new Random().Next()).ToList();
        }

        public static void Shuffle<U, V>(U[] u, V[] v)
        {
            for (int i = 0; i < u.Length - 1; i++)
            {
                int j = Int(i, u.Length);
                if (j != i)
                {
                    U ut = u[i];
                    u[i] = u[j];
                    u[j] = ut;

                    V vt = v[i];
                    v[i] = v[j];
                    v[j] = vt;
                }
            }
        }
    }
}