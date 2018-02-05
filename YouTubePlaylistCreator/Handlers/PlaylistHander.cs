using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace YouTubePlaylistCreator.Handlers
{
	public class PlaylistHander
    {
		private static Playlist Playlist;
		private static YouTubeService YouTubeService;
		public static List<string> FailedIds = new List<string>();


		/// <summary>
		/// Find an already existing playlist using it's ID
		/// </summary>
		/// <param name="id">Id of the playlist on YouTube (can be found in the URL when viewing the playlist's details)</param>
		public static void GetPlaylist(string id)
		{
			try
			{
				PlaylistsResource.ListRequest searchListRequest = YouTubeService.Playlists.List("snippet");
				searchListRequest.Id = id; // Replace with your search term.
				searchListRequest.MaxResults = 1;

				PlaylistListResponse searchListResponse = searchListRequest.Execute();

				Playlist = searchListResponse.Items.FirstOrDefault();

				if (Playlist is null)
				{
					Console.WriteLine("Failed to find playlist");
					Thread.Sleep(1000);
					return;
				}
			}
			catch
			{
				Console.WriteLine("Error ocurred");
			}
		}

		/// <summary>
		/// Create a new playlist, will ask for the title of the playlist, the description (can be left blank)
		/// and a playlist status (public|private) which will be private by default.
		/// </summary>
		/// <param name="service">YouTubeService parameter is generated using the AuthHandler.Login() method</param>
		public static async void CreatePlaylist(YouTubeService service)
		{
			try
			{
				YouTubeService = service;

				Playlist = new Playlist
				{
					Snippet = new PlaylistSnippet()
				};

				Console.Clear();

				Console.WriteLine("Playlist's title:");
				string title = Console.ReadLine();

				Playlist.Snippet.Title = title;

				Console.WriteLine("Playlist description:");
				string description = Console.ReadLine();

				Playlist.Snippet.Description = description;

				Console.WriteLine("Public/Private: (Leave blank for Private)");
				string status = Console.ReadLine().ToLower();

				if (status is null || status == "" || !Regex.IsMatch(status, "private|public"))
				{
					status = "private";
				}

				Playlist.Status = new PlaylistStatus
				{
					PrivacyStatus = status
				};

				Playlist = await YouTubeService.Playlists.Insert(Playlist, "snippet,status").ExecuteAsync();
			}
			catch
			{
				Console.WriteLine("Failed to create playlist");
			}
		}

		/// <summary>
		/// Add a song to the previously created playlist
		/// </summary>
		/// <param name="videoId">VideoId must be passed from a previous search using the VideoHandler.FindSong() method</param>
		public static void AddSongToPlaylist(string videoId)
		{
			try
			{
				var newPlaylistItem = new PlaylistItem
				{
					Snippet = new PlaylistItemSnippet()
				};
				newPlaylistItem.Snippet.PlaylistId = Playlist.Id;
				newPlaylistItem.Snippet.ResourceId = new ResourceId
				{
					Kind = "youtube#video",
					VideoId = videoId
				};

				newPlaylistItem = YouTubeService.PlaylistItems.Insert(newPlaylistItem, "snippet").Execute();
			}
			catch
			{
				FailedIds.Add(videoId);
			}
		}
	}
}
