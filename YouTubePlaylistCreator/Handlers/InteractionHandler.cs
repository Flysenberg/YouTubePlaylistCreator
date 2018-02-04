using Google.Apis.YouTube.v3;

using System;
using System.IO;
using System.Threading;

using YouTubePlaylistCreator.Properties;

namespace YouTubePlaylistCreator.Handlers
{
	public class InteractionHandler
    {
		public static string ApiKey = Resources.APIKey;
		public static string ClientSecrets = "client_secrets.json";
		private static string Back = "Type back to go back to revious menu";
		
		private static bool RegisteredClientSecrets = false;
		private static bool PlaylistCreated = false;
		private static bool VideoInfoFileLoaded = false;

		public static void Run()
		{
			string command = "";

			do
			{
				Console.Clear();
				Console.WriteLine("What do you want to do? (just write the number)");
				Console.WriteLine("If you don't have a client_secrets.json file please create first");
				Console.WriteLine("	0 - Create new client_secrets.json file");
				Console.WriteLine("	1 - Create new API key");
				Console.WriteLine("	2 - Register client_secrets.json file");
				Console.WriteLine("	3 - Register API key (Optional)");

				if (RegisteredClientSecrets)
				{
					Console.WriteLine("	4 - Create new playlist");
					if (PlaylistCreated)
					{
						Console.WriteLine("	5 - Load video information file");
					}

					if (PlaylistCreated && VideoInfoFileLoaded)
					{
						Console.WriteLine("	6 - Add all songs from file to the playlist");
					}
				}

				Console.WriteLine("	7 - Exit");

				command = Console.ReadLine().Trim();

				switch (command)
				{
					case "0": CreateNewClientSecretsFile(); break;
					case "1": CreateNewApiKey(); break;
					case "2": RegisterNewClientSecretsFile(); break;
					case "3": RegisterNewApiKey(); break;
					case "4": CreateNewPlaylist(); break;
					case "5": LoadSongsFile(); break;
					case "6": AddAllSongsToPlaylist(); break;
					default:
						break;
				}
			}
			while (command != "7");
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
			Console.WriteLine("Specify new API Key: (Type default to use the default API already present)");
			string apiKey = Console.ReadLine();

			if (apiKey == "back")
			{
				return;
			}

			if (apiKey == "default")
			{
				return;
			}

			if (apiKey is null || apiKey == "")
			{
				RegisterNewApiKey("Invalid API key");
			}

			ApiKey = apiKey;
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

				VideoHandler.RegisterService(ApiKey);
				VideoHandler.LoadSongsFromFile(fileName);
				VideoInfoFileLoaded = true;
			}
			else
			{
				LoadSongsFile("Failed to locate file, try again");
			}
		}

		private static void AddAllSongsToPlaylist()
		{
			foreach (string songId in VideoHandler.VideoIds)
			{
				PlaylistHander.AddSongToPlaylist(songId);
			}

			Console.WriteLine("Succesfully added songs to the playlist");
			Thread.Sleep(100);
		}
	}
}
