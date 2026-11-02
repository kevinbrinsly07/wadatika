namespace Wadatika.Application.Features.Projects;

public sealed class ProjectKeyAlreadyExistsException(string key)
    : Exception($"A project with key '{key}' already exists in this organization.");