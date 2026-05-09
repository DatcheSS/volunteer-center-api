using VolunteerCenter.Models;

namespace VolunteerCenter.Services;

public interface IVolunteerService
{
    Task<IEnumerable<Volunteer>> GetAllAsync(int maxItems = 50);
    
    Task<IEnumerable<Volunteer>> GetVolunteersWithCuratorAsync(int maxItems = 50);
    
    Task<IEnumerable<Volunteer>> GetVolunteersByCuratorAsync(int curatorId, int maxItems = 50);
    
    Task<IEnumerable<Beneficiary>> GetBeneficiariesAsync(int maxItems = 50);
    
    Task<int> GetVolunteerCountAsync();
}