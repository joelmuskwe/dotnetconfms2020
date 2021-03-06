using Domain;
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Xunit;

namespace test
{
  public class DomainClassTests
  {
    private static Team CreateTeamAjax()
    {
      return new Team("AFC Ajax", "The Lancers", "1900", "Amsterdam Arena");
    }

    [Fact]
    public void NewTeamGetsId()
    {
      var team = CreateTeamAjax();
      var privateId = typeof(Team).GetProperty("Id", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.NotEqual(Guid.Empty, privateId.GetValue(team));
    }

    [Fact]
    public void TeamAllowsNewPlayer()
    {
      var team = CreateTeamAjax();
      team.AddPlayer("André", "Onana", out string response);
      Assert.Equal("André Onana", team.Players.First().Name);
    }

    [Fact]
    public void TeamAllowsMultiplePlayers()
    {
      var team = CreateTeamAjax();
      team.AddPlayer("André", "Onana", out string response);
      team.AddPlayer("Matthijs", "de Ligt", out response);
      Assert.Equal(2, team.Players.Count());
    }

    [Fact]
    public void TeamPreventsDuplicatePlayer()
    {
      var team = CreateTeamAjax();
      team.AddPlayer("André", "Onana", out string response);
      team.AddPlayer("André", "Onana", out response);
      Assert.Single(team.Players);
    }

    [Fact]
    public void TeamReturnsDuplicateMessageForDuplicatePlayer()
    {
      var team = CreateTeamAjax();
      team.AddPlayer("André", "Onana", out string response);
      team.AddPlayer("André", "Onana", out response);
      Assert.Equal("Duplicate player", response);
    }

    [Fact]
    public void CanAddManager()
    {
      var team = CreateTeamAjax();
      var firstmanager = new Manager("Marcel", "Keizer");
      team.ChangeManagement(firstmanager);
      var privateId = typeof(Team).GetProperty("Id", BindingFlags.NonPublic | BindingFlags.Instance);

      Assert.Equal(privateId.GetValue(team), firstmanager.CurrentTeamId);
      Assert.Equal(firstmanager.Name, team.ManagerName);
    }

    [Fact]
    public void CanReplaceManager()
    {
      var team = CreateTeamAjax();
      var firstmanager = new Manager("Marcel", "Keizer");
      team.ChangeManagement(firstmanager);
      var newmanager = (new Manager("Erik", "ten Hag"));
      team.ChangeManagement(newmanager);
      var privateId = typeof(Team).GetProperty("Id", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.Equal(privateId.GetValue(team), newmanager.CurrentTeamId);
      Assert.Equal(newmanager.Name, team.ManagerName);
    }

    [Fact]
    public void ReplacedManagerHasNoCurrentTeamId()
    {
      var team = CreateTeamAjax();
      var firstmanager = new Manager("Marcel", "Keizer");
      team.ChangeManagement(firstmanager);
      var newmanager = (new Manager("Erik", "ten Hag"));
      team.ChangeManagement(newmanager);
      Assert.Null(firstmanager.CurrentTeamId);
    }

    [Fact]
    public void ReplacedManagerHasOldTeamIdInPastIds()
    {
      var team = CreateTeamAjax();
      var firstmanager = new Manager("Marcel", "Keizer");
      team.ChangeManagement(firstmanager);
      var newmanager = (new Manager("Erik", "ten Hag"));
      team.ChangeManagement(newmanager);
      var privateId = typeof(Team).GetProperty("Id", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.Equal(privateId.GetValue(team), firstmanager.PastTeams.FirstOrDefault().TeamId);
    }

    [Fact]
    public void CanSetHomeColors()
    {
      var team = CreateTeamAjax();
      team.SpecifyHomeUniformColors(Color.White, Color.Red);
      Assert.Equal(new Color[] { Color.White, Color.Red }, new Color[] { team.HomeColors.Primary, team.HomeColors.Secondary });
    }
  }
}