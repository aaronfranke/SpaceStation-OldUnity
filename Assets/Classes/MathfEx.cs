#if REAL_T_IS_DOUBLE
using real_t = System.Double;
#else
using real_t = System.Single;
#endif

public static class MathfEx
{
    /// <summary>
    /// Performs a canonical Modulus operation, where the output is on the range [0, b).
    /// </summary>
    public static real_t PosMod(real_t a, real_t b)
    {
        real_t c = a % b;
        if ((c < 0 && b > 0) || (c > 0 && b < 0))
        {
            c += b;
        }
        return c;
    }

    /// <summary>
    /// Performs a canonical Modulus operation, where the output is on the range [0, b).
    /// </summary>
    public static int PosMod(int a, int b)
    {
        int c = a % b;
        if ((c < 0 && b > 0) || (c > 0 && b < 0))
        {
            c += b;
        }
        return c;
    }
}