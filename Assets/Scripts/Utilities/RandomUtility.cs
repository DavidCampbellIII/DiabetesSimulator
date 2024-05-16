using System.Collections.Generic;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public static class RandomUtility
{
    //public static string currentGeneratorID { get; private set; }

    private static readonly Dictionary<string, Random> randomGenerators = new Dictionary<string, Random>();

    /// <summary>
    /// Creates a new random generator with the supplied ID. <br />
    /// If a generator with that ID already exists, it will be reseeded with the supplied seed. <br />
    /// If the seed is set to 0, the seed will be random.
    /// </summary>
    /// <param name="id">ID of the random generator</param>
    /// <param name="seed">Optional.  Seed of the random generator.  0 == random seed</param>
    public static void CreateOrReseedRandomGenerator(string id, uint seed = 0)
    {
        randomGenerators[id] = new Random(seed == 0 ? (uint)System.DateTimeOffset.Now.Millisecond : seed);
    }

    //public static void SetCurrentRandomGeneratorID(string id)
    //{
    //    currentGeneratorID = id;
    //}

    /// <summary>
    /// Returns a random value between (0f, 1f)
    /// </summary>
    /// <param name="id">ID of the random generator to be used</param>
    public static float Value(string id
#if DEBUG_RANDOM_GENERATOR
        , string source    
#endif
    )
    {
        return Range(0f, 1f, id
#if DEBUG_RANDOM_GENERATOR
        , source
#endif
        );
    }

    /// <summary>
    /// Returns a random value between [min, max]
    /// </summary>
    /// <param name="min">Min range inclusive</param>
    /// <param name="max">Max range inclusive</param>
    /// <param name="id">ID of the random generator to be used</param>
    public static float Range(float min, float max, string id
#if DEBUG_RANDOM_GENERATOR
        , string source
#endif
    )
    {
#if DEBUG_RANDOM_GENERATOR
        Debug.Log($"float range from {source}");
#endif
        Random rand = randomGenerators[id];
        float val = rand.NextFloat(min, max + float.Epsilon);
        randomGenerators[id] = rand;
        return val;
    }

    /// <summary>
    /// Returns a random value between [min, max)
    /// </summary>
    /// <param name="min">Min range inclusive</param>
    /// <param name="max">Max range exclusive</param>
    /// <param name="id">ID of the random generator to be used</param>
    public static int Range(int min, int max, string id
#if DEBUG_RANDOM_GENERATOR
        , string source
#endif
        )
    {
#if DEBUG_RANDOM_GENERATOR
        Debug.Log($"int range from {source}");
#endif
        Random rand = randomGenerators[id];
        int val = rand.NextInt(min, max);
        randomGenerators[id] = rand;
        return val;
    }

    /// <summary>
    /// Returns a random value between [min, max]
    /// </summary>
    /// <param name="min">Min range inclusive</param>
    /// <param name="max">Max range inclusive</param>
    /// <param name="id">ID of the random generator to be used</param>
    public static int RangeInclusive(int min, int max, string id
#if DEBUG_RANDOM_GENERATOR
        , string source
#endif
        )
    {
        return Range(min, max + 1, id
#if DEBUG_RANDOM_GENERATOR
            , source
#endif
            );
    }

    /// <summary>
    /// Returns a random Vector2 with values between (0f, 1f) for both the x and y components
    /// </summary>
    /// <param name="id">ID of the random generator to be used</param>
    public static Vector2 InsideUnitCircle(string id
#if DEBUG_RANDOM_GENERATOR
        , string source
#endif
        )
    {
        return new Vector2(Range(0f, 1f, id
#if DEBUG_RANDOM_GENERATOR
            , source
#endif
            ), Range(0f, 1f, id
#if DEBUG_RANDOM_GENERATOR
            , source
#endif
            ));
    }
}
