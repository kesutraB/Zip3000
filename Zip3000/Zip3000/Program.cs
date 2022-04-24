
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices.ComTypes;
using Ionic.Zip;

namespace Zip3000
{
	internal class Program
	{
		// TEXT FILE: https://www.gutenberg.org/cache/epub/67766/pg67766.txt
		// MP4 FILE: https://jsoncompare.org/LearningContainer/SampleFiles/Video/MP4/sample-mp4-file.mp4
		static Dictionary<CompressionLevel, long> compressionTimes = new Dictionary<CompressionLevel, long>();

		static void Main(string[] args)
		{
			GetSum();
			PrintTable();
			SaveToFile();
			File.Delete(Constant.PathToZip);
		}

		private static long GetAvg(long sum)
		{
			return sum / 10;
		}

		private static void GetSum()
		{
			using (FileStream zipToOpen = new FileStream(Constant.PathToZip, FileMode.Create))
			{
				using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
				{
					long sum = 0;
					for (int i = 0; i < 3; i++)
					{
						for (int j = 0; j < 10; j++)
						{
							var stopwatch = Stopwatch.StartNew();
							OpenZip(i, archive);
							stopwatch.Stop();
							var timeToZip = stopwatch.ElapsedMilliseconds;
							sum += timeToZip;
						}
						var avg = GetAvg(sum);
						compressionTimes.Add(GetLevel(i), avg);
						Console.WriteLine("hroch");
					}
				}
			}
		}


		private static void OpenZip(int i, ZipArchive archive)
		{
			archive.CreateEntryFromFile(Constant.PathToVideo, Constant.VideoName, GetLevel(i));
			archive.CreateEntryFromFile(Constant.PathToText, Constant.TextName, GetLevel(i));
		}
		private static void SaveToFile()
		{
			foreach (KeyValuePair<CompressionLevel, long> time in compressionTimes)
			{
				File.AppendAllText(Constant.PathToTable, $"{time.Key},{time.Value}\n");
			}
		}
		private static CompressionLevel GetLevel(int counter)
		{
			switch (counter)
			{
				case 1:
					{
						return CompressionLevel.Fastest;
					}
				case 2:
					{
						return CompressionLevel.Optimal;
					}
				default:
					{
						return CompressionLevel.NoCompression;
					}
			}
		}

		private static void PrintTable()
		{
			Console.WriteLine("Compression Times\n*********************");
			foreach (KeyValuePair<CompressionLevel, long> time in compressionTimes)
			{
				Console.WriteLine($"{time.Key}\t{time.Value}ms");
			}
		}
	}
}