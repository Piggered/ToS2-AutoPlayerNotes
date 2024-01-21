using System.Collections.Generic;
using System.Linq;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace AutoPlayerNotes;

public enum Flag
{
    HasProsecuted
}

public class PlayerFlags
{
    private readonly List<Flag>[] _flags = new List<Flag>[15];

    public PlayerFlags()
    {
        for (var i = 0; i < _flags.Length; i++)
        {
            _flags[i] = new List<Flag>();
        }
    }

    public void Add(int position, Flag flag) => _flags[position].Add(flag);
    public void Remove(int position, Flag flag) => _flags[position].Remove(flag);
    public bool Has(int position, Flag flag) => _flags[position].Contains(flag);

    public List<int> GetList(Flag flag) =>
        _flags.Where((flags, index) => Has(index, flags[index])).Select((_, index) => index).ToList();

    public void Reset()
    {
        foreach (var flag in _flags)
        {
            flag.Clear();
        }
    }
}