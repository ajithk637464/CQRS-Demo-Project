using CQRS.Application.Interfaces;
using CQRS.Domain.Entities;
using CQRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(Employee employee, CancellationToken cancellationToken)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);
        return employee.Id;
    }

    public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return _context.Employees.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Employees.AsNoTracking()
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);
    }
}
