namespace Wadatika.Application.Features.Authentication;

public sealed class EmailAlreadyRegisteredException(string email)
    : Exception($"An account with email '{email}' already exists.")
{
}