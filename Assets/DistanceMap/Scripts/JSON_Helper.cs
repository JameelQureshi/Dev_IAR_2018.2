using System;
using System.Collections.Generic;

public class JSON_Helper
{

	public List<CircleLineRenderer.JSonObject> objects;
	public float largestDistance = -1.0f;

	public JSON_Helper(string[] file)
	{

		string[] header = file[1].Split(new char [] { '"', ',', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

		objects = new List<CircleLineRenderer.JSonObject>();
		for (int i = 0; i < header.Length; i++)
		{
			if(!string.IsNullOrEmpty(header[i]) && !String.IsNullOrWhiteSpace(header[i]))
			{
                CircleLineRenderer.JSonObject pushThis;
				pushThis.Name = header[i];
				pushThis.Distance = -1.0f;
				objects.Add(pushThis);
			}
		}

		string[] info = file[6].Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
		string[] distancesTemp = new string[(info.Length - 1) / 6];
		List<string> distanceString = new List<string>();
		for (int i = 2, j = 0; i < info.Length; i+=6, j++)
		{
			distancesTemp[j] = info[i];
			string[] distanceTemp1 = distancesTemp[j].Split(new char[] { '"' }, StringSplitOptions.RemoveEmptyEntries);
			for (int k = 3; k < distanceTemp1.Length; k+=7)
			{
				distanceString.Add(distanceTemp1[k]);
			}
		}

		for (int i = 0; i < distanceString.Count; i++)
		{
			distanceString[i] = distanceString[i].Remove(distanceString[i].Length - 3, 3);
		}

		for (int i = 0; i < objects.Count; i++)
		{
			float f;
			bool b = float.TryParse(distanceString[i], out f);
            CircleLineRenderer.JSonObject passThis;
			passThis.Name = objects[i].Name;
			passThis.Distance = f;
			objects[i] = passThis;			
			if(f > largestDistance)
			{
				largestDistance = f;
			}
		}
				
	}

}
