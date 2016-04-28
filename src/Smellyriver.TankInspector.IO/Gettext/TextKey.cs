using System;
using System.Linq;

namespace Smellyriver.TankInspector.IO.Gettext
{
    public class TextKey
    {

        public static TextKey Parse(string info)
        {

            if (info.First() != '#')
            {
                return new TextKey() { BaseName = "", MessageID = info };
            }


            var rawInfo = info.Substring(1, info.Length - 1);

            var infos = rawInfo.Split(':');

            if (infos.Length != 2)
            {
                throw new InvalidOperationException(@"Text Data Info must Split with ':' ");
            }

            var textDataInfo = new TextKey();
            textDataInfo.BaseName = infos[0];
            textDataInfo.MessageID = infos[1];

            return textDataInfo;
        }

        public string BaseName { get; private set; }
        public string MessageID { get; private set; }


        private TextKey()
        {

        }


    }
}
