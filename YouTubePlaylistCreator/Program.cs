using System.Threading.Tasks;

using YouTubePlaylistCreator.Handlers;

namespace YouTubePlaylistCreator
{
	class Program
    {
		static void Main(string[] args)
			=> new Program().Run().GetAwaiter().GetResult();

		private async Task Run()
		{
			InteractionHandler.Run();
		}
	}
}
