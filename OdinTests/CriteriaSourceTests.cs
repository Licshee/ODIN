using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class CriteriaSourceTests
{
    [Fact]
    public void ForLazy()
    {
        var cs = new CriteriaSource<string>(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            { "a", new Lazy<object>(() => new string("omg".ToCharArray())) },
            { "b", CriterionMeta.Create(new Lazy<object>(() => "wtf"), "A") },
            { "C", CriterionMeta.Create(new Lazy<object>(() => "bbq".Clone()), "B") },
        });

        Assert.Equal(0, cs.Count());

        Assert.Equal("omg", cs["a"]);
        Assert.True(new HashSet<string>(new[] { "A" }, cs.Comparer).SetEquals(cs.ReadKeys));

        Assert.Equal("wtf", cs["B"]);
        Assert.True(new HashSet<string>(new[] { "b" }).SetEquals(cs.ReadKeys));

        Assert.Equal("bbq", cs["c"]);
        Assert.True(new HashSet<string>(new[] { "c" }, cs.Comparer).SetEquals(cs.ReadKeys));

        Assert.True(cs.Select(kvp => kvp.Value).SequenceEqual(new[] { "bbq" }));
    }

    [Fact]
    public void ForInit()
    {
        var cs = new CriteriaSource<string>(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            { "a", CriterionMeta.Create<string>("foo", true) },
            { "b", CriterionMeta.Create<string>("bar", false) },
        });
        Assert.Equal("bar", cs["B"]);
        Assert.True(new HashSet<string>(new[] { "A" }, cs.Comparer).SetEquals(cs.ReadKeys));
    }

    [Fact]
    public void ForInitCovers()
    {
        var cs = new CriteriaSource<string>(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            { "a", CriterionMeta.Create<string>("foo", true) },
            { "b", CriterionMeta.Create("bar", true, "A") },
        });
        Assert.True(new HashSet<string>(new[] { "B" }, cs.Comparer).SetEquals(cs.ReadKeys));
    }
}
