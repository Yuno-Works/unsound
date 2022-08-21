using UnityEngine;

public class VectorExtensions
{
    public static Vector3 RandomVector()
    {
        Vector3 randomVector = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f));
        return randomVector.normalized;
    }
}
