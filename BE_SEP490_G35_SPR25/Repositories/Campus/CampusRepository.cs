using Microsoft.EntityFrameworkCore;
using Planify_BackEnd.DTOs.Campus;
using Planify_BackEnd.Models;

public class CampusRepository : ICampusRepository
{
    private readonly PlanifyContext _context;
    public CampusRepository(PlanifyContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateCampus(Campus campus)
    {
        try
        {
            _context.Add(campus);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);

        }
    }

    public async Task<bool> DeleteCampus(int id)
    {
        try
        {
            var campus = await _context.Campuses
                .FirstOrDefaultAsync(c => c.Id == id);
            campus.Status = 0;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);

        }
    }

    public async Task<IEnumerable<Campus>> getAllCampus()
    {
        try
        {
            return await _context.Campuses
                .Where(c => c.Status == 1)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);

        }
    }

    public async Task<Campus?> GetCampusById(int id)
    {
        try
        {
            return await _context.Campuses
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);

        }
    }

    public async Task<Campus> GetCampusByName(string campusName)
    {
        try
        {
            var campus = await _context.Campuses
                .FirstOrDefaultAsync(c => c.CampusName==campusName
                && c.Status == 1);
            return campus;
        }
        catch
        {
            throw new Exception();
        }
    }

    public async Task<bool> UpdateCampus(Campus campus)
    {
        try
        {
            _context.Update(campus);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);

        }
    }
}
