using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GatherImagesToFolders
{
	class Program
	{
		private static string path_to_csv = @"D:\Github\GatherImagesToFolders\GatherImagesToFolders\data.csv";
		private static string path_to_img_folder = @"D:\Github\102 Category Flower\jpg";
		private static string path_to_img_folder2 = @"D:\102 Category Flower Dataset\jpg";
		private static string path_to_categories_json = @"D:\Github\GatherImagesToFolders\GatherImagesToFolders\cat_to_name.json";
		private static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;


		static void Main(string[] args)
		{
			Function2();
			Console.ReadKey();
		}

		/// <summary>
		/// Gather images to folders
		/// </summary>
		static void Function1()
		{
			var data1 = ReadCsv();
			var data2 = ReadJson();
			GatherImages(data1, data2);
		}

		// Combine json and csv
		static void Function2()
		{
			var data1 = ReadCsv();
			var data2 = ReadJson();
			WriteTagFile(data1, data2);
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

		static List<KeyValuePair<int, string>> ReadJson()
		{
			var jsonStr = File.ReadAllText(path_to_categories_json);
			var jsonObj = JObject.Parse(jsonStr);
			List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
			foreach (var property in jsonObj.Properties())
			{
				list.Add(new KeyValuePair<int, string>(Convert.ToInt32(property.Name), property.Value.ToString()));
			}
			return list;
		}

		static void GatherImages(List<KeyValuePair<int, int>> listNumericLabel, List<KeyValuePair<int, string>> listTextLabel)
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
					int folderID = listNumericLabel.Where(pair => pair.Key == id).FirstOrDefault().Value;
					var result = listTextLabel.Where(pair => pair.Key == folderID).FirstOrDefault().Value;
					string folderName = textInfo.ToTitleCase(result);
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

		static void WriteTagFile(List<KeyValuePair<int, int>> listNumericLabel, List<KeyValuePair<int, string>> listTextLabel)
		{
			DirectoryInfo dir = new DirectoryInfo(path_to_img_folder2);
			FileInfo[] files = dir.GetFiles();
			using (StreamWriter writer = new StreamWriter(path_to_img_folder2 + "\\tag.csv"))
			{
				foreach (var item in files)
				{
					if (item.Extension == ".jpg")
					{
						// "img_00001.jpg" => "img_00001" => "00001"
						string idStr = Path.GetFileNameWithoutExtension(item.FullName).Split('_')[1];
						// "00001" => 1
						int id = Convert.ToInt32(idStr);
						int folderID = listNumericLabel.Where(pair => pair.Key == id).FirstOrDefault().Value;
						var result = listTextLabel.Where(pair => pair.Key == folderID).FirstOrDefault().Value;
						string label_output = textInfo.ToTitleCase(result);
						string filename_input = item.Name;
						string line = filename_input + '\t' + label_output;
						writer.WriteLine(line);
						Console.WriteLine(line);
					}
				}
			}
		}

	}
}
