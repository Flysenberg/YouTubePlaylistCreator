using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

using System;
using System.Collections.Generic;
using System.IO;

using YouTubePlaylistCreator.Properties;

namespace YouTubePlaylistCreator.Handlers
{
	public class VideoHandler
    {
		/// <summary>
		/// Holds a list of all the video names loaded from the file specified in LoadSongsFromFile() method
		/// </summary>
		public static List<string> VideoNames { get; set; } = new List<string>();
		/// <summary>
		/// Holds all the video Ids found from the LoadSongsFromFile() method.
		/// </summary>
		public static List<string> VideoIds { get; set; } = new List<string>();
		/// <summary>
		/// Holds all the failed results
		/// </summary>
		public static List<string> FailedToFind { get; set; } = new List<string>();

		private static string AppName = Resources.AppName;
		private static YouTubeService YouTubeService;

		/// <summary>
		/// Created a new YouTubeService using a regular API to search for videos
		/// </summary>
		/// <param name="apiKey">Api key must be generated from http://console.developers.google.com</param>
		public static void RegisterService(string apiKey)
		{
			try
			{
				YouTubeService = new YouTubeService(new BaseClientService.Initializer()
				{
					ApiKey = apiKey,
					ApplicationName = AppName
				});
			}
			catch (Exception e)
			{
				Console.WriteLine("Failed to register YouTube service using the provided API key\nException:\n" + e);
			}
		}

		/// <summary>
		/// Specify a file URI to load songs from, songs must be
		/// on individual lines inside the file, with only the name of the
		/// video yu want to search for, no other data necessary.
		/// </summary>
		/// <param name="fileUri">Specify a URI to the file from which the songs will be loaded from</param>
		public static void LoadSongsFromFile(string fileUri)
		{
			try
			{
				using (StreamReader sr = new StreamReader(fileUri))
				{
					while (!sr.EndOfStream)
					{
						VideoNames.Add(sr.ReadLine());
					}
				}

				foreach (string songName in VideoNames)
				{
					GetVideoId(songName, 1);
				}

				Console.WriteLine("Seached for {0} videos, found {1} results.", VideoNames.Count, VideoIds.Count);

				if (FailedToFind.Count > 0)
				{
					DisplayFailedToFindVideos();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Failed to locate file\nException:\n" + e);
			}
		}
		
		/// <summary>
		/// Finds a video Id based on a search query
		/// </summary>
		/// <param name="videoName">Query to search</param>
		/// <param name="results">Number of results to return</param>
		public static async void GetVideoId(string videoName, int results)
		{
			try
			{
				SearchResource.ListRequest searchListRequest = YouTubeService.Search.List("snippet");
				searchListRequest.Q = videoName; // Replace with your search term.
				searchListRequest.MaxResults = results;

				SearchListResponse searchListResponse = await searchListRequest.ExecuteAsync();

				foreach (var searchResult in searchListResponse.Items)
				{
					switch (searchResult.Id.Kind)
					{
						case "youtube#video":
							VideoIds.Add(searchResult.Id.VideoId);
							break;
						default:
							FailedToFind.Add(videoName);
							break;
					}
				}
			}
			catch
			{
				FailedToFind.Add(videoName);
			}
		}

		/// <summary>
		/// Asks the user if he wishes to see all the videos that failed the search query
		/// and did not get an Id
		/// </summary>
		private static void DisplayFailedToFindVideos()
		{
			try
			{
				Console.WriteLine("Failed to find Ids of {0} videos\n", FailedToFind.Count);
				Console.WriteLine("You want to see a list of all failed videos? (y/N)\n");

				string response = Console.ReadLine().ToLower();

				if (response == "y")
				{
					foreach (string videoName in FailedToFind)
					{
						Console.WriteLine(videoName);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception:\n" + e);
			}
		}
	}
}
