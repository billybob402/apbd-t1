namespace apbd_t1.Models.DTOs;

public class VisitRequestDto
{
    public int VisitId { get; set; }
    public int ClientId { get; set; }
    public string MechanicLicenceNumber { get; set; } = string.Empty;
    public List<VisitRequestServiceDto> Services { get; set; } = new List<VisitRequestServiceDto>();
}

public class VisitRequestServiceDto
{
    public string ServiceName { get; set; } = string.Empty;
    public decimal ServiceFee { get; set; }
}