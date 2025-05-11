using apbd_t1.Models.DTOs;

namespace apbd_t1.Services;

public interface ITempService
{
    public Task<List<TempDTO>> GetTemps();
}