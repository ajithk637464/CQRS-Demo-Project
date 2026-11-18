using CQRS.Application.Interfaces;
using MediatR;

namespace CQRS.Application.Employees.Queries.GetEmployees;

public class GetEmployeesHandler : IRequestHandler<GetEmployeesQuery, IReadOnlyList<EmployeeDto>>
{
    private readonly IEmployeeRepository _repository;

    public GetEmployeesHandler(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await _repository.GetAllAsync(cancellationToken);

        return employees
            .Select(e => new EmployeeDto(e.Id, e.FirstName, e.LastName, e.Email, e.Department))
            .ToList();
    }
}
