using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace MathsSiege.Client
{
    public class UserPreferences
    {
        public static readonly string Filename = "UserPreferences.xml";

        public string HostAddress
        {
            get => GetPreference(document, nameof(HostAddress));
            set => SetPreference(document, nameof(HostAddress), value);
        }

        public bool IsWindowFullScreen
        {
            get
            {
                string value = GetPreference(document, nameof(IsWindowFullScreen));
                return value is null ? false : bool.Parse(value);
            }
            set => SetPreference(document, nameof(IsWindowFullScreen), value.ToString());
        }

        public bool IsAudioEnabled
        {
            get
            {
                string value = GetPreference(document, nameof(IsAudioEnabled));
                return value is null ? true : bool.Parse(value);
            }
            set => SetPreference(document, nameof(IsAudioEnabled), value.ToString());
        }

        private XDocument document;

        public void Load()
        {
            var fs = File.Open(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            try
            {
                document = XDocument.Load(fs);
            }
            catch (XmlException)
            {
                document = Create(fs);
            }
            finally
            {
                fs.Close();
            }
        }

        public void Save()
        {
            var fs = File.OpenWrite(Filename);

            fs.SetLength(0);

            try
            {
                document.Save(fs);
            }
            finally
            {
                fs.Close();
            }
        }

        private XDocument Create(FileStream fs)
        {
            fs.SetLength(0);

            var document = new XDocument(
                new XElement("UserPreferences"));

            SetPreference(document, nameof(HostAddress), "");
            SetPreference(document, nameof(IsWindowFullScreen), false.ToString());
            SetPreference(document, nameof(IsAudioEnabled), false.ToString());

            document.Save(fs);

            return document;
        }

        private string GetPreference(XDocument document, string name)
        {
            var preference = document.Root
                .Descendants(name)
                .FirstOrDefault();

            return preference?.Value;
        }

        private void SetPreference(XDocument document, string name, string value)
        {
            var preference = document.Root
                .Descendants(name)
                .FirstOrDefault();

            if (preference == null)
            {
                document.Root
                    .Add(new XElement(name,
                        new XText(value)));
            }
            else
            {
                preference.Value = value;
            }
        }
    }
}
