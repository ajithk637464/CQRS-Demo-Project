using CQRS.Domain.Entities;

namespace CQRS.Application.Interfaces;

public interface IEmployeeRepository
{
    Task<int> CreateAsync(Employee employee, CancellationToken cancellationToken);
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken);
}
