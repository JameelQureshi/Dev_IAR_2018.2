using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class FileManager  {

		public static readonly string ROOT_SAVE_DIR = Application.persistentDataPath + "/";
		public static readonly string TOKEN_FILE = "tokenData.dat";

		// Saves the data class T
		public static void SaveData<T>(T t, string filename) {
			string path = ROOT_SAVE_DIR + filename;
			BinaryFormatter bf = new BinaryFormatter();
			FileStream fs = File.Create (path);

			bf.Serialize(fs, t);
			fs.Close();
		}

		/// <summary>
		/// Loads data from specified filename. The file should contain only one serialized c# object.
		/// </summary>
		/// <returns>The deserialized object from the file</returns>
		/// <param name="filename">Name of the file that was previously saved including it's extension.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T LoadData<T>(string filename) {
			string path = ROOT_SAVE_DIR + filename;
			if (File.Exists(path)) {
				BinaryFormatter bf = new BinaryFormatter();
				FileStream fs = File.Open (path, FileMode.Open);

				T loadedData = (T) bf.Deserialize(fs);
				fs.Close();

				return loadedData;
			} else {
				throw new IOException("The file does not exist");
			}
		}

		/// <summary>
		/// Just a convenience method that shortens the code to check if a file exists.
		/// This relies upon pre-existing filenames within this class so it's a bit more specific
		/// </summary>
		/// <returns><c>true</c>, if exists was filed, <c>false</c> otherwise.</returns>
		/// <param name="filename">Filename.</param>
		public static bool FileExists(string filename) {
			return File.Exists(ROOT_SAVE_DIR + filename);
		}

		public static void DeleteData(string filename) {
			string path = ROOT_SAVE_DIR + filename;
			if (File.Exists(path)) {
				File.Delete(path);
			}
		}

	}