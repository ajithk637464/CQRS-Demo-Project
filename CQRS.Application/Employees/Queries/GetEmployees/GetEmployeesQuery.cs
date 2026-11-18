using MediatR;

namespace CQRS.Application.Employees.Queries.GetEmployees;

public record GetEmployeesQuery : IRequest<IReadOnlyList<EmployeeDto>>;
