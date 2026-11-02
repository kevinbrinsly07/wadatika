
using Wadatika.Application.Features.Teams;
using Wadatika.Application.Interfaces;
using Wadatika.Application.DTOs.Teams;
using Wadatika.Domain.Entities;
using Wadatika.Domain.Enums;

namespace Wadatika.UnitTests.Features.Teams;

public sealed class TeamServiceTests
{
     [Fact]
     public async Task CreateAsync_WhenUserIsOrganizationMember_CreatesTeamAndAssignsLead()
     {
          // Arrange
          var organizationId = Guid.NewGuid();
          var userId = Guid.NewGuid();

          var repository = new FakeTeamRepository
          {
               IsMember = true
          };

          var service = new TeamService(repository);

          var request = new CreateTeamRequest("Development Team");

          // Act
          var result = await service.CreateAsync(
              organizationId,
              request,
              userId,
              CancellationToken.None);

          // Assert
          Assert.Equal("Development Team", result.Name);
          Assert.Equal(organizationId, result.OrganizationId);

          Assert.NotNull(repository.CreatedTeam);
          Assert.NotNull(repository.CreatedTeamLead);

          Assert.Equal(
              repository.CreatedTeam.Id,
              repository.CreatedTeamLead.TeamId);

          Assert.Equal(userId, repository.CreatedTeamLead.UserId);

          Assert.Equal(
              TeamRole.Lead,
              repository.CreatedTeamLead.Role);
     }

     [Fact]
     public async Task CreateAsync_WhenUserIsNotOrganizationMember_ThrowsAccessException()
     {
          // Arrange
          var organizationId = Guid.NewGuid();
          var userId = Guid.NewGuid();

          var repository = new FakeTeamRepository
          {
               IsMember = false
          };

          var service = new TeamService(repository);

          var request = new CreateTeamRequest("Development Team");

          // Act
          var exception = await Assert.ThrowsAsync<TeamOrganizationAccessException>(
              () => service.CreateAsync(
                  organizationId,
                  request,
                  userId,
                  CancellationToken.None));

          // Assert
          Assert.Contains(organizationId.ToString(), exception.Message);
          Assert.Null(repository.CreatedTeam);
          Assert.Null(repository.CreatedTeamLead);
     }

     private sealed class FakeTeamRepository : ITeamRepository
     {
          public bool IsMember { get; set; }

          public Team? CreatedTeam { get; private set; }

          public TeamMember? CreatedTeamLead { get; private set; }

          public Task<bool> IsOrganizationMemberAsync(
              Guid organizationId,
              Guid userId,
              CancellationToken cancellationToken)
          {
               return Task.FromResult(IsMember);
          }

          public Task AddAsync(
              Team team,
              TeamMember teamLead,
              CancellationToken cancellationToken)
          {
               CreatedTeam = team;
               CreatedTeamLead = teamLead;

               return Task.CompletedTask;
          }

          public Task<IReadOnlyList<Team>> GetByOrganizationForMemberAsync(
               Guid organizationId,
               Guid userId,
               CancellationToken cancellationToken)
          {
               IReadOnlyList<Team> teams = CreatedTeam is not null
                   && CreatedTeam.OrganizationId == organizationId
                   ? [CreatedTeam]
                   : [];

               return Task.FromResult(teams);
          }
     }
}
