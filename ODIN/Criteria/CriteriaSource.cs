using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CriteriaSource<TKey> : IEnumerable<KeyValuePair<TKey, object>>
{
    class Criterion
    {
        public object Data;
        public bool? Read;
    }

    private readonly Dictionary<TKey, Criterion> _Dict;
    public IEqualityComparer<TKey> Comparer => _Dict.Comparer;

    public CriteriaSource(IEnumerable<KeyValuePair<TKey, object>> source, IEqualityComparer<TKey> comparer = null)
    {
        _Dict = new Dictionary<TKey, Criterion>(comparer ?? EqualityComparer<TKey>.Default);

        foreach (var kvp in source)
        {
            var meta = kvp.Value as CriterionMeta<TKey>;
            if (meta == null)
                meta = new CriterionMeta<TKey>(kvp.Value);

            _Dict.Add(kvp.Key, new Criterion { Data = meta });
        }

        foreach (var kvp in _Dict)
        {
            var meta = (CriterionMeta<TKey>)kvp.Value.Data;
            var read = meta.Read;
            if (read.HasValue)
                GetItem(kvp.Key, read.Value);
        }
    }
    public CriteriaSource(Dictionary<TKey, object> source)
        : this(source, source.Comparer)
    { }

    private Criterion GetItem(TKey key, bool read)
    {
        Criterion status;
        if (!_Dict.TryGetValue(key, out status))
            return null;

        if (status.Read.HasValue)
            status.Read &= read;
        else
        {
            var meta = (CriterionMeta<TKey>)status.Data;
            status.Data = meta.Value;
            status.Read = read;

            foreach (var covered in meta.CoveredKeys)
                GetItem(covered, false);
        }
        return status;
    }

    public object this[TKey key]
    {
        get
        {
            var status = GetItem(key, true);
            var lazy = status.Data as Lazy<object>;
            if (lazy != null)
                return lazy.Value;
            return status.Data;
        }
    }

    public IEnumerable<TKey> ReadKeys
        => from kvp in _Dict where kvp.Value.Read.GetValueOrDefault() select kvp.Key;

    public IEnumerator<KeyValuePair<TKey, object>> GetEnumerator()
    {
        foreach (var key in ReadKeys)
            yield return new KeyValuePair<TKey, object>(key, this[key]);
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
