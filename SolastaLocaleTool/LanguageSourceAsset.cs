using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityAssetLib.Types;
using UnityAssetLib.Serialization;

#pragma warning disable 0649

namespace SolastaLocaleTool
{
    [UnitySerializable]
    class LanguageSourceAsset : MonoBehaviour
    {
        public LanguageSourceData mSource;

		[UnitySerializable]
		public class LanguageSourceData
        {
			public bool UserAgreesToHaveItOnTheScene;
			public bool UserAgreesToHaveItInsideThePluginsFolder;
			public bool GoogleLiveSyncIsUptoDate;

			public List<TermData> mTerms;

			public bool CaseInsensitiveTerms;
			public int OnMissingTranslation;
			public string mTerm_AppName;

			public List<LanguageData> mLanguages;

			public bool IgnoreDeviceLanguage;
			public int _AllowUnloadingLanguages;

			public string Google_WebServiceURL;
			public string Google_SpreadsheetKey;
			public string Google_SpreadsheetName;
			public string Google_LastUpdatedVersion;

			public int GoogleUpdateFrequency;
			public int GoogleInEditorCheckFrequency;
			public int GoogleUpdateSynchronization;

			public float GoogleUpdateDelay;

			public List<PPtr> Assets;
		}

		[UnitySerializable]
        public class TermData
        {
            public string Term;
            public int TermType;
            public string[] Languages;
            public byte[] Flags;
            public string[] Languages_Touch;
        }

		[UnitySerializable]
		public class LanguageData
        {
			public string Name;
			public string Code;
			public byte Flags;
		}
    }
}
