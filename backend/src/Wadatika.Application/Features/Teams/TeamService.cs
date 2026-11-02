using Wadatika.Application.DTOs.Teams;
using Wadatika.Application.Interfaces;
using Wadatika.Domain.Entities;
using Wadatika.Domain.Enums;

namespace Wadatika.Application.Features.Teams;

public sealed class TeamService(
    ITeamRepository teamRepository)
    : ITeamService
{
     public async Task<TeamResponse> CreateAsync(
         Guid organizationId,
         CreateTeamRequest request,
         Guid userId,
         CancellationToken cancellationToken)
     {
          var isOrganizationMember =
              await teamRepository.IsOrganizationMemberAsync(
                  organizationId,
                  userId,
                  cancellationToken);

          if (!isOrganizationMember)
          {
               throw new TeamOrganizationAccessException(organizationId);
          }

          var team = new Team(
              organizationId,
              request.Name);

          var teamLead = new TeamMember(
              team.Id,
              userId,
              TeamRole.Lead);

          await teamRepository.AddAsync(
              team,
              teamLead,
              cancellationToken);

          return new TeamResponse(
              team.Id,
              team.OrganizationId,
              team.Name,
              team.CreatedAt);
     }

     public async Task<IReadOnlyList<TeamResponse>> GetByOrganizationAsync(
     Guid organizationId,
     Guid userId,
     CancellationToken cancellationToken)
     {
          var isOrganizationMember =
              await teamRepository.IsOrganizationMemberAsync(
                  organizationId,
                  userId,
                  cancellationToken);

          if (!isOrganizationMember)
          {
               throw new TeamOrganizationAccessException(organizationId);
          }

          var teams = await teamRepository.GetByOrganizationForMemberAsync(
              organizationId,
              userId,
              cancellationToken);

          return teams
              .Select(team => new TeamResponse(
                  team.Id,
                  team.OrganizationId,
                  team.Name,
                  team.CreatedAt))
              .ToList();
     }
}