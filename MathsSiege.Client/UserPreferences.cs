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
            get => this.GetPreference(this.document, nameof(this.HostAddress));
            set => this.SetPreference(this.document, nameof(this.HostAddress), value);
        }

        public bool IsWindowFullScreen
        {
            get => bool.Parse(this.GetPreference(this.document, nameof(this.IsWindowFullScreen)));
            set => this.SetPreference(this.document, nameof(this.IsWindowFullScreen), value.ToString());
        }

        private XDocument document;

        public void Load()
        {
            var fs = File.Open(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            try
            {
                this.document = XDocument.Load(fs);
            }
            catch (XmlException)
            {
                this.document = this.Create(fs);
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
                this.document.Save(fs);
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

            this.SetPreference(document, nameof(this.HostAddress), "");
            this.SetPreference(document, nameof(this.IsWindowFullScreen), false.ToString());

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
