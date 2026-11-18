using MediatR;

namespace CQRS.Application.Employees.Commands.CreateEmployee;

public record CreateEmployeeCommand(
    string FirstName,
    string LastName,
    string Email,
    string Department) : IRequest<int>;
