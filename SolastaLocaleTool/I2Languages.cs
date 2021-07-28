using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityAssetLib.Types;
using UnityAssetLib.Serialization;

namespace SolastaLocaleTool
{
    [UnitySerializable]
    class I2Languages : MonoBehaviour
    {
        public class LanguageSourceData
        {
            public bool UserAgreesToHaveItOnTheScene;
            public bool UserAgreesToHaveItInsideThePluginsFolder;
            public bool GoogleLiveSyncIsUptoDate;

            public TermData[] mTerms;
        }

        [UnitySerializable]
        public class TermData
        {
            public string Term;
            public int TermType;
            public string Description;
            public string[] Languages;
            public byte[] Flags;
            public string[] Languages_Touch;
        }

    }
}
