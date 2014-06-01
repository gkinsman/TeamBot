using System.Threading.Tasks;

namespace TeamBot.Features.Giphy
{
    public interface IGiphyClient
    {
        Task<string> Random(string input = null);

        Task<string[]> Search(string input, int limit = 25, int offset = 0);
    }
}