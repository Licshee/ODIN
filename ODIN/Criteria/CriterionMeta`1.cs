using System;
using System.Collections.Generic;
using System.Linq;

public class CriterionMeta<TKey>
{
    private readonly HashSet<TKey> _Covers = null;
    public IEnumerable<TKey> CoveredKeys
    {
        get
        {
            if (_Covers == null) yield break;
            foreach (var key in _Covers)
                yield return key;
        }
    }
    public bool? Read { get; }
    public object Value { get; }

    public CriterionMeta(object value, bool? read = null, IEnumerable<TKey> covers = null, IEqualityComparer<TKey> comparer = null)
    {
        if (covers != null)
            _Covers = new HashSet<TKey>(covers, comparer ?? EqualityComparer<TKey>.Default);

        Read = read;

        Value = value;
    }
    public CriterionMeta(object value, HashSet<TKey> covers, bool? read = null)
      : this(value, read, covers, covers?.Comparer)
    { }
    public CriterionMeta(object value, bool? read, IEqualityComparer<TKey> comparer, params TKey[] covers)
        : this(value, read, covers.AsEnumerable(), comparer)
    { }
    public CriterionMeta(object value, IEqualityComparer<TKey> comparer, params TKey[] covers)
        : this(value, null, covers.AsEnumerable(), comparer)
    { }
    public CriterionMeta(object value, bool? read, params TKey[] covers)
        : this(value, read, covers.AsEnumerable())
    { }
    public CriterionMeta(object value, params TKey[] covers)
        : this(value, covers: covers.AsEnumerable())
    { }
}
