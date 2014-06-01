using System.Threading.Tasks;
using TeamBot.Features.TeamCity.Models;

namespace TeamBot.Features.TeamCity
{
    public interface ITeamCityClient
    {
        Task<Builds> GetBuilds();
        Task<BuildExtended> GetBuild(int id);
    }
}