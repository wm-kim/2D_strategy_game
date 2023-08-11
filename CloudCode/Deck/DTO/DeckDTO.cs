using System;
using System.Collections.Generic;

[Serializable]
public class DeckDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<int> CardIds { get; set; }
    public long DateModified { get; set; }
    
    public DeckDTO()
    {
        Id = -1;
        Name = string.Empty;
        CardIds = new List<int>();
        DateModified = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
    
    public DeckDTO(int id, string name, List<int> cardIds)
    {
        Id = id;
        Name = name;
        CardIds = cardIds;
        DateModified = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}