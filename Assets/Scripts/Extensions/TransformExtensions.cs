using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TransformExtensions
{
    public static Transform GetClosest(this IEnumerable<Transform> tCollection, Transform from)
    {
        if (from == null) return null;
        return tCollection.OrderBy(t => (t.position - from.position).sqrMagnitude)
            .FirstOrDefault();
    }
}
