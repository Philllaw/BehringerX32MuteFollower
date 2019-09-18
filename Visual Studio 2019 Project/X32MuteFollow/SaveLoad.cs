using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace X32MuteFollow
{
    public class SaveLoad
    {
        public static void saveToFile(ProgramSettings settings)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ProgramSettings));
            TextWriter tw = new StreamWriter(@"X32MuteFollowerSettings.xml");
            xs.Serialize(tw, settings);
            tw.Close();
        }

        internal static void loadFromFile(ProgramSettings settings)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(ProgramSettings));
                using (var sr = new StreamReader(@"X32MuteFollowerSettings.xml"))
                {
                    ProgramSettings loadedSettings = (ProgramSettings)xs.Deserialize(sr);
                    settings.MasterIP = loadedSettings.MasterIP;
                    settings.FollowerIP = loadedSettings.FollowerIP;
                    settings.Running = loadedSettings.Running;
                    for (int i = 0; i < 48; i++)
                    {
                        settings.Checkboxes[i] = loadedSettings.Checkboxes[i];
                    }
                }
            }
            catch (Exception ex)
            {
                //if failed then we have the defaults anyway
            }
        }

        [Serializable]
        public class ProgramSettings
        {

            public String MasterIP;
            public String FollowerIP;
            public Boolean Running;
            public Boolean[] Checkboxes;

            public ProgramSettings()
            {
                MasterIP = "192.168.1.4";
                FollowerIP = "192.168.1.5";
                Running = false;
                Checkboxes = new Boolean[48];
            }
        }
    }



}
