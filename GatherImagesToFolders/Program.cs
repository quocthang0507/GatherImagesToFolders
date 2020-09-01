using System;
using System.Collections.Generic;
using System.IO;

namespace GatherImagesToFolders
{
	class Program
	{
		private static string path_to_csv = @"D:\Github\GatherImagesToFolders\GatherImagesToFolders\data.csv";
		private static string path_to_img_folder = @"D:\Github\102 Category Flower\jpg";

		static void Main(string[] args)
		{
			var data = ReadCsv();
			GatherImages(data);
			Console.ReadKey();
		}

		static List<KeyValuePair<int, int>> ReadCsv()
		{
			List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
			using (var reader = new StreamReader(path_to_csv))
			{
				string data = reader.ReadToEnd();
				string[] arr = data.Split(',');
				for (int i = 1; i <= arr.Length; i++)
				{
					list.Add(new KeyValuePair<int, int>(i, Convert.ToInt32(arr[i - 1])));
				}
			}
			return list;
		}

		static void GatherImages(List<KeyValuePair<int, int>> list)
		{
			DirectoryInfo dir = new DirectoryInfo(path_to_img_folder);
			FileInfo[] files = dir.GetFiles();
			foreach (var item in files)
			{
				if (item.Extension == ".jpg")
				{
					// "img_00001.jpg" => "img_00001" => "00001"
					string idStr = Path.GetFileNameWithoutExtension(item.FullName).Split('_')[1];
					// "00001" => 1
					int id = Convert.ToInt32(idStr);
					string folderName = "" + list[id - 1].Value;
					string folderPath = path_to_img_folder + "\\" + folderName;
					string newPath = folderPath + "\\" + item.Name;
					if (!Directory.Exists(folderPath))
						Directory.CreateDirectory(folderPath);
					try
					{
						File.Move(item.FullName, newPath);
					}
					catch (Exception)
					{
						continue;
					}
					Console.WriteLine($"Moved file from {item.FullName} to {newPath}");
				}
			}
		}
	}
}
