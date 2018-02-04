using System.Threading.Tasks;

using YouTubePlaylistCreator.Handlers;

namespace YouTubePlaylistCreator
{
	class Program
    {
		static void Main(string[] args)
			=> new Program().Run();

		private void Run()
		{
			InteractionHandler.Run();
		}
	}
}
