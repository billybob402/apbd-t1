namespace apbd_t1.Models.DTOs;

public class VisitResponseDto
{
    public DateTime Date { get; set; }
    public VisitResponseClientDto Client { get; set; } = new VisitResponseClientDto();
    public VisitResponseMechanicDto Mechanic { get; set; } = new VisitResponseMechanicDto();
    public List<VisitResponseServiceDto> VisitServices { get; set; } = new List<VisitResponseServiceDto>();
}

public class VisitResponseClientDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}

public class VisitResponseMechanicDto
{
    public int MechanicId { get; set; }
    public string LicenceNumber { get; set; } = string.Empty;
}

public class VisitResponseServiceDto
{
    public string Name { get; set; } = string.Empty;
    public decimal ServiceFee { get; set; }
}