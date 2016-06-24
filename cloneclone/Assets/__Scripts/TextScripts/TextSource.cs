using UnityEngine;
using System.Collections;

using System.Xml;
using System.IO;

public class TextSource : MonoBehaviour {

	private static bool initialized = false;
	public static TextAsset copySource;

	public static void Initialize(){
		if (!initialized){
			copySource = Resources.Load("PortraitOfTheArtistText") as TextAsset;

			if (copySource != null){
				Debug.Log ("Text Loaded for " + Application.systemLanguage);
				Debug.Log(GetText("test_me_too"));
			}
		}
	}

	public static string GetText(string key){

		XmlTextReader reader = new XmlTextReader(new StringReader(copySource.text));

		string outString = "";

		// replace "EN" with language code, once implemented

		while(reader.Read()){
			if (reader.IsStartElement(key)){
				reader.ReadToFollowing("EN");
				outString = reader.ReadElementContentAsString("EN", reader.NamespaceURI);
			}
		}

		return outString;

	}
}
