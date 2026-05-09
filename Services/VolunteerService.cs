using Microsoft.EntityFrameworkCore;
using VolunteerCenter.Data;
using VolunteerCenter.Models;

namespace VolunteerCenter.Services;

public class VolunteerService : IVolunteerService
{
    private readonly VolunteerCenterContext _db;

    public VolunteerService(VolunteerCenterContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Volunteer>> GetAllAsync(int maxItems = 50)
    {
        return await _db.Volunteers
            .Take(maxItems)
            .OrderBy(v => v.FullName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Volunteer>> GetVolunteersWithCuratorAsync(int maxItems = 50)
    {
        return await _db.Volunteers
            .Include(v => v.Curator)
            .Take(maxItems)
            .OrderBy(v => v.FullName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Volunteer>> GetVolunteersByCuratorAsync(int curatorId, int maxItems = 50)
    {
        return await _db.Volunteers
            .Include(v => v.Curator)
            .Where(v => v.CuratorId == curatorId)
            .Take(maxItems)
            .OrderBy(v => v.FullName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Beneficiary>> GetBeneficiariesAsync(int maxItems = 50)
    {
        return await _db.Beneficiaries
            .Take(maxItems)
            .OrderBy(b => b.FullName)
            .ToListAsync();
    }

    public async Task<int> GetVolunteerCountAsync()
    {
        return await _db.Volunteers.CountAsync();
    }
}