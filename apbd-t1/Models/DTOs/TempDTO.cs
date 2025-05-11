namespace apbd_t1.Models.DTOs;

public class TempDTO
{
    public int IdTemp { get; set; }
    public string A { get; set; } = String.Empty;
    public List<ThingDTO> Things { get; set; } = [];
}

public class ThingDTO
{
    public int IdThing { get; set; }
    public string B { get; set; } = String.Empty;
}