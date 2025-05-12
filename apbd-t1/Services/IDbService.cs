using apbd_t1.Models.DTOs;

namespace apbd_t1.Services;

public interface IDbService
{
    public Task<VisitResponseDto> GetVisitById(int id);
    public Task AddVisitByRequestDto(VisitRequestDto requestDto);
}