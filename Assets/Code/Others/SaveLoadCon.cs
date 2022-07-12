using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoadCon : MonoBehaviour {

	static List<Data> allData = new List<Data>();

	[SerializeField]
	private string pathtoshow;

	private class Data
	{
		string path;
		public string Name;

		private object data;

		public Data(string newName)
		{
			Name = newName;
			path = Path.Combine (Application.persistentDataPath, Name + ".data");
		}

		public void SetData(object newdata)
        {
			data = newdata;
        }

		public void Save()
		{
			FileStream str = File.Create (path);
			BinaryFormatter form = new BinaryFormatter ();
			form.Serialize (str, data);
			str.Close ();
		}

		public object Load()
		{
			object value = null;
			if (File.Exists (path)) 
			{
				FileStream str = File.Open (path, FileMode.Open);
				BinaryFormatter form = new BinaryFormatter ();
				value = form.Deserialize(str);
				str.Close ();
			} 
			return value;
		}
	}

	void Start () {
		pathtoshow = Path.Combine(Application.persistentDataPath, "here is the saved data");
	}
	
	void Update () {
	
	}

	public static void SaveData(object value, string Name)
	{
		for (int i = 0; i < allData.Count; i++) 
		{
			if (allData [i].Name == Name) 
			{
				allData[i].SetData(value);
				allData [i].Save ();
				return;
			}
		}
		Data newData = new Data (Name);
		newData.SetData(value);
		newData.Save();
		allData.Add(newData);
	}

	public static object LoadData(string Name)
	{	
		for (int i = 0; i < allData.Count; i++) 
		{
			if (allData [i].Name == Name) 
			{
				return allData [i].Load ();
			}
		}
		Data newData = new Data(Name);
		allData.Add(newData);
		return newData.Load();
	}

	[System.Serializable]
	public class BinaryColor
	{
		float[] binary = new float[4];
		public BinaryColor(Color c)
		{
			binary[0] = c.r;
			binary[1] = c.g;
			binary[2] = c.b;
			binary[3] = c.a;
		}

		/// <summary>
		/// Returns the Color type if the parameter is BinaryColor. Either returns black color.
		/// </summary>
		/// <param name="_data">The data of type "BinaryColor"</param>
		/// <returns></returns>
		public static Color DeserializeColor(object _data)
		{
			if (_data != null)
			{
				BinaryColor data = (BinaryColor)_data;
				Color result;
			    result = new Color(data.binary[0], data.binary[1], data.binary[2], data.binary[3]);
				return result;
			}
			else
			{
				return Color.black;
			}
		}

	}

}
