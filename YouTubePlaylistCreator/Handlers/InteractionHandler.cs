using Google.Apis.YouTube.v3;

using System;
using System.IO;
using System.Threading;

using YouTubePlaylistCreator.Properties;

namespace YouTubePlaylistCreator.Handlers
{
	public enum Result
	{
		EXIT = 0,
		CREATE_CLIENT_SECRET,
		CREATE_API_KEY,
		REGISTER_CLIENT_SECRET,
		REGISTER_API_KEY,
		CREATE_PLAYLIST,
		REGISTER_PLAYLIST,
		REGISTER_SONGS,
		ADD_SONGS_TO_PLAYLIST
	}

	public class InteractionHandler
    {
		public static string ApiKey = Resources.APIKey;
		public static string ClientSecrets = Resources.ClientSecretsFile;
		private static string Back = "Type back to go back to revious menu";
		
		private static bool RegisteredClientSecrets = false;
		private static bool RegisteredApiKey = false;
		private static bool PlaylistCreated = false;
		private static bool PlaylistRegistered = false;
		private static bool VideoInfoFileLoaded = false;
		
		public static void Run()
		{
			try
			{
				Result res;

				do
				{
					Console.Clear();
					Console.WriteLine("What do you want to do? (just write the number)");
					Console.WriteLine("If you don't have a client_secrets.json file please create one first");
					Console.WriteLine("	0 - Exit");
					Console.WriteLine("	1 - Create new client_secrets.json file");
					Console.WriteLine("	2 - Create new API key");
					Console.WriteLine("	3 - Register client_secrets.json file");
					Console.WriteLine("	4 - Register API key");

					if (RegisteredClientSecrets && RegisteredApiKey)
					{
						Console.WriteLine("	5 - Create new playlist");
						Console.WriteLine("	6 - Register playlist");
						if (PlaylistCreated || PlaylistRegistered)
						{
							Console.WriteLine("	7 - Load video information file");
						}

						if (VideoInfoFileLoaded)
						{
							Console.WriteLine("	8 - Add all songs from file to the playlist");
						}
					}

					res = (Result) int.Parse(Console.ReadLine().Trim());

					switch (res)
					{
						case Result.EXIT:
							return;
						case Result.CREATE_CLIENT_SECRET:
							CreateNewClientSecretsFile();
							break;
						case Result.CREATE_API_KEY:
							CreateNewApiKey();
							break;
						case Result.REGISTER_CLIENT_SECRET:
							RegisterNewClientSecretsFile();
							break;
						case Result.REGISTER_API_KEY:
							RegisterNewApiKey();
							break;
						case Result.CREATE_PLAYLIST:
							CreateNewPlaylist();
							break;
						case Result.REGISTER_PLAYLIST:
							RegisterNewPlaylist();
							break;
						case Result.REGISTER_SONGS:
							LoadSongsFile();
							break;
						case Result.ADD_SONGS_TO_PLAYLIST:
							AddAllSongsToPlaylist();
							break;
						default:
							break;
					}
				}
				while (res != Result.EXIT);
			}
			catch
			{
				Console.WriteLine("Some error ocurred... Restart the program");
			}
		}
		
		private static void CreateNewClientSecretsFile(string message = "")
		{
			Console.Clear();
			Console.WriteLine(message);
			Console.WriteLine(Back);
			Console.WriteLine("Visit " + "http://console.developers.google.com");
			string command = Console.ReadLine().Trim();

			if (command == "back")
			{
				return;
			}

			CreateNewClientSecretsFile("Invalid command");
		}

		private static void CreateNewApiKey(string message = "")
		{
			Console.Clear();
			Console.WriteLine(message);
			Console.WriteLine(Back);
			Console.WriteLine("Visit " + "http://console.developers.google.com");
			string command = Console.ReadLine().Trim();

			if (command == "back")
			{
				return;
			}

			CreateNewApiKey("Invalid command");
		}

		private static void RegisterNewClientSecretsFile(string message = "")
		{
			Console.Clear();
			Console.WriteLine(message);
			Console.WriteLine(Back);
			Console.WriteLine("Specify the file name (must be in same directory as executable)");
			string fileName = Console.ReadLine();

			if (fileName == "back")
			{
				return;
			}

			if (!fileName.EndsWith(".json"))
			{
				fileName += ".json";
			}

			if (File.Exists(fileName))
			{
				Console.WriteLine("File correctly located");
				Thread.Sleep(1000);
				ClientSecrets = fileName;
				RegisteredClientSecrets = true;
			}
			else
			{
				RegisterNewClientSecretsFile("Failed to locate file, try again");
			}
		}

		private static void RegisterNewApiKey(string message = "")
		{
			Console.Clear();
			Console.WriteLine(message);
			Console.WriteLine(Back);
			Console.WriteLine("Specify API Key:");
			string apiKey = Console.ReadLine();

			if (apiKey == "back")
			{
				return;
			}

			if (apiKey is null || apiKey == "")
			{
				RegisterNewApiKey("Invalid API key");
			}

			ApiKey = apiKey;
			RegisteredApiKey = true;
		}

		private static async void CreateNewPlaylist()
		{
			YouTubeService service = await AuthHandler.Login(ClientSecrets);
			PlaylistHander.CreatePlaylist(service);
			PlaylistCreated = true;
		}

		private static void LoadSongsFile(string message = "")
		{
			Console.Clear();
			Console.WriteLine(message);
			Console.WriteLine(Back);
			Console.WriteLine("Specify the file name (must be in same directory as executable)");
			string fileName = Console.ReadLine();

			if (fileName == "back")
			{
				return;
			}

			if (File.Exists(fileName))
			{
				Console.WriteLine("File correctly located");
				Thread.Sleep(1000);

				Console.Clear();
				Console.WriteLine("Getting video Ids from youtube... This might take a while so hang on");
				VideoHandler.RegisterService(ApiKey);
				VideoHandler.LoadSongsFromFile(fileName);
				VideoInfoFileLoaded = true;

				Console.WriteLine("Done getting video Ids");
				Thread.Sleep(1000);
			}
			else
			{
				LoadSongsFile("Failed to locate file, try again");
			}
		}

		private static void AddAllSongsToPlaylist()
		{
			Console.Clear();
			Console.WriteLine("Adding songs to playlist.. Might take a while");

			foreach (string videoId in VideoHandler.VideoIds)
			{
				PlaylistHander.AddSongToPlaylist(videoId);
			}

			if (PlaylistHander.FailedIds.Count > 0)
			{
				Console.WriteLine("Failed to add {0} videos to playlist", PlaylistHander.FailedIds.Count);
			}

			Console.WriteLine("Succesfully added songs to the playlist");
			Thread.Sleep(1000);
		}

		private static void RegisterNewPlaylist(string message = "")
		{
			Console.Clear();
			Console.WriteLine(message);
			Console.WriteLine(Back);
			Console.WriteLine("Write the playlist's ID:");
			string id = Console.ReadLine().Trim();

			if (id == "back")
			{
				return;
			}

			if (id is null || id == "")
			{
				RegisterNewPlaylist("Invalid ID");
			}

			PlaylistHander.GetPlaylist(id);
			PlaylistRegistered = true;
		}
	}
}
