using System;
using System.Collections.Generic;

public class CriterionMeta
{
    public static CriterionMeta<TKey> Create<TKey>(object value, bool? read = null, IEnumerable<TKey> covers = null, IEqualityComparer<TKey> comparer = null)
        => new CriterionMeta<TKey>(value, read, covers, comparer);
    public static CriterionMeta<TKey> Create<TKey>(object value, HashSet<TKey> covers, bool? read = null)
        => new CriterionMeta<TKey>(value, covers, read);
    public static CriterionMeta<TKey> Create<TKey>(object value, bool? read, IEqualityComparer<TKey> comparer, params TKey[] covers)
        => new CriterionMeta<TKey>(value, read, comparer, covers);
    public static CriterionMeta<TKey> Create<TKey>(object value, IEqualityComparer<TKey> comparer, params TKey[] covers)
        => new CriterionMeta<TKey>(value, comparer, covers);
    public static CriterionMeta<TKey> Create<TKey>(object value, bool? read, params TKey[] covers)
        => new CriterionMeta<TKey>(value, read, covers);
    public static CriterionMeta<TKey> Create<TKey>(object value, params TKey[] covers)
        => new CriterionMeta<TKey>(value, covers);
}
