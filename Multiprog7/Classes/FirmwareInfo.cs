using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Multiprog7.Classes
{
    public class FirmwareInfo
    {
        public static List<FirmwareInfo> firmwares = null;

        public FirmwareInfo(string deviceClass, string devicePlatform, string deviceType, string deviceLocalization, string deviceProcessor, string version, string url, string fileHash, string fileSize, string date, string description, string note)
        {
            this.deviceClass = int.Parse(deviceClass);
            this.devicePlatform = int.Parse(devicePlatform);
            this.deviceType = int.Parse(deviceType);
            String[] tmp = deviceLocalization.Split(':');
            this.localization = int.Parse(tmp[0]);
            this.implementationType = int.Parse(tmp[1]);

            this.deviceProcessor = int.Parse(deviceProcessor);
            this.version = new Version(version);
            this.url = url;
            this.fileHash = fileHash;
            this.fileSize = int.Parse(fileSize);
            this.date = date;
            this.description = description;
            this.note = note;
        }

        public FirmwareInfo(int descriptor)
        {
            this.descriptor = descriptor;
            //this.deviceProcessor = descriptor.ifd.cpu;
        }

        public FirmwareInfo()
        {
        }

        public void setLocalization(string localization)
        {
            String[] tmp = localization.Split(':');
            this.localization = int.Parse(tmp[0]);
            this.implementationType = int.Parse(tmp[1]);
        }

        public int deviceClass; // AAA 
        public int devicePlatform; // B 
        public int deviceType; // CCC 
        public int localization;
        public int implementationType;
        public int deviceProcessor;
        public Version version;
        public string url = null;
        public string fileHash = null;
        public int fileSize;
        public string date = null;
        public string description = null;
        public string note = null;

        public int descriptor;

        public static void parceFirmware(string firmwareXml)
        {
            firmwares = new List<FirmwareInfo>();
            int i = 0;
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(firmwareXml);
                XmlElement xRoot = xDoc.DocumentElement;

                foreach (XmlNode xnode in xRoot)
                {
                    i++;
                    bool allFieldsExists = true;
                    // получаем атрибут name 
                    if (xnode.Attributes.Count > 0)
                    {
                        FirmwareInfo firmware = new FirmwareInfo();

                        XmlNode attr = xnode.Attributes.GetNamedItem("url");

                        if (attr != null && attr.Value != null && attr.Value.Length > 0)
                        {
                            if (attr.Value != null)
                            {
                                Uri uri = new Uri(attr.Value);
                                string ext = System.IO.Path.GetExtension(uri.LocalPath);

                                if (ext.Substring(1, 1).Equals("c"))
                                {
                                    continue;
                                }
                            }

                            firmware.url = attr.Value;
                        }
                        else
                        {
                            Console.WriteLine("Отсутствуют обязательные параметры url.");
                            firmware.url = "url отсутствует";
                            allFieldsExists = false;
                        }

                        attr = xnode.Attributes.GetNamedItem("deviceClass");

                        if (attr != null && attr.Value != null && attr.Value.Length > 0)
                        {
                            try
                            {
                                if (int.Parse(attr.Value) < 1 || int.Parse(attr.Value) > 255)
                                {
                                    allFieldsExists = false;
                                }
                                else
                                {
                                    firmware.deviceClass = int.Parse(attr.Value);
                                }
                            }
                            catch
                            {
                                allFieldsExists = false;
                            }

                        }
                        else
                        {
                            allFieldsExists = false;
                            Console.WriteLine("Отсутствует обязательный параметр DeviceClass, {0}.", firmware.url);
                        }

                        attr = xnode.Attributes.GetNamedItem("devicePlatform");

                        if (attr != null && attr.Value != null && attr.Value.Length > 0)
                        {
                            try
                            {
                                if (int.Parse(attr.Value) < 1 || int.Parse(attr.Value) > 15)
                                {
                                    allFieldsExists = false;
                                }
                                else
                                {
                                    firmware.devicePlatform = int.Parse(attr.Value);

                                }
                            }
                            catch
                            {
                                allFieldsExists = false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Отсутствует обязательный параметр devicePlatform, {0}.", firmware.url);
                            allFieldsExists = false;
                        }

                        attr = xnode.Attributes.GetNamedItem("deviceType");

                        if (attr != null && attr.Value != null && attr.Value.Length > 0)
                        {
                            try
                            {
                                if (int.Parse(attr.Value) < 1 || int.Parse(attr.Value) > 65535)
                                {
                                    allFieldsExists = false;
                                }
                                else
                                {
                                    firmware.deviceType = int.Parse(attr.Value);
                                }
                            }
                            catch
                            {
                                allFieldsExists = false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Отсутствует обязательный параметр deviceType, {0}.", firmware.url);
                            allFieldsExists = false;
                        }

                        attr = xnode.Attributes.GetNamedItem("deviceLocalization");

                        if (attr != null && attr.Value != null && attr.Value.Length > 0)
                        {

                            String deviceLocalizationCountry = String.Empty;
                            String deviceLocalizationImplementationType = String.Empty;

                            var regex = new Regex(string.Format(@"(\d*):(\d*)"), RegexOptions.Singleline);
                            Match match = regex.Match(attr.Value);

                            if (match.Success)
                            {
                                deviceLocalizationCountry = match.Groups[1].Value;
                                deviceLocalizationImplementationType = match.Groups[2].Value;

                                deviceLocalizationCountry = match.Groups[1].Value;
                                deviceLocalizationImplementationType = match.Groups[2].Value;

                                if (Int32.Parse(deviceLocalizationCountry) < 1 || Int32.Parse(deviceLocalizationCountry) > 255)
                                {
                                    allFieldsExists = false;
                                }

                                if (Int32.Parse(deviceLocalizationImplementationType) < 1 || Int32.Parse(deviceLocalizationImplementationType) > 14)
                                {
                                    allFieldsExists = false;
                                }

                                if (Int32.Parse(deviceLocalizationImplementationType) == 15)
                                {
                                    allFieldsExists = false;
                                    Console.WriteLine("Персональная прошивкане может быть использована для обновления, {0}.",
                             firmware.url);
                                }

                                if (allFieldsExists)
                                {
                                    firmware.setLocalization(attr.Value);
                                }
                            }
                            else
                            {
                                allFieldsExists = false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Отсутствует обязательный параметр deviceLocalization, {0}.", firmware.url);
                            allFieldsExists = false;
                        }

                        attr = xnode.Attributes.GetNamedItem("deviceProcessor");

                        if (attr != null && attr.Value != null && attr.Value.Length > 0)
                        {
                            try
                            {
                                if (int.Parse(attr.Value) < 1 || int.Parse(attr.Value) > 15)
                                {
                                    allFieldsExists = false;
                                }
                                else
                                {
                                    firmware.deviceProcessor = byte.Parse(attr.Value);
                                }
                            }
                            catch
                            {
                                allFieldsExists = false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Отсутствует обязательный параметр deviceProcessor, {0}.", firmware.url);
                            allFieldsExists = false;
                        }

                        attr = xnode.Attributes.GetNamedItem("version");

                        if (attr != null && attr.Value != null && attr.Value.Length > 0)
                        {
                            Regex RgxUrl = new Regex(@"^\d+\.\d+\.\d+\.\d+$");

                            if (!RgxUrl.IsMatch(attr.Value))
                            {
                                allFieldsExists = false;
                            }
                            else
                            {
                                firmware.version = new Version(attr.Value);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Отсутствует обязательный параметр version, {0}.", firmware.url);
                            allFieldsExists = false;
                        }

                        attr = xnode.Attributes.GetNamedItem("fileHash");

                        if (attr != null && attr.Value != null && attr.Value.Length > 0)
                        {
                            firmware.fileHash = attr.Value;
                        }
                        else
                        {
                            Console.WriteLine("Отсутствует обязательный параметр fileHash, {0}.", firmware.url);
                            allFieldsExists = false;
                        }

                        attr = xnode.Attributes.GetNamedItem("fileSize");

                        if (attr != null && attr.Value != null && attr.Value.Length > 0)
                        {
                            try
                            {
                                firmware.fileSize = int.Parse(attr.Value);
                            }
                            catch
                            {
                                allFieldsExists = false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Отсутствует обязательный параметр fileSize, {0}.", firmware.url);
                            allFieldsExists = false;
                        }

                        attr = xnode.Attributes.GetNamedItem("date");

                        if (attr != null && attr.Value != null && attr.Value.Length > 0)
                        {
                            firmware.date = attr.Value;
                        }
                        else
                        {
                            Console.WriteLine("Отсутствует обязательный параметр date, {0}.", firmware.url);
                        }

                        attr = xnode.Attributes.GetNamedItem("description");

                        if (attr != null && attr.Value != null && attr.Value.Length > 0)
                        {
                            firmware.description = attr.Value;
                        }
                        else
                        {
                            Console.WriteLine("Отсутствует обязательный параметр description, {0}.", firmware.url);
                        }

                        attr = xnode.Attributes.GetNamedItem("note");

                        if (attr != null && attr.Value != null && attr.Value.Length > 0)
                        {
                            firmware.note = attr.Value;
                        }

                        if (allFieldsExists)
                        {
                            Console.WriteLine($"Success Node: {i}");
                            firmwares.Add(firmware);
                        }
                        else
                        {
                            Console.WriteLine("Прошивка не добавлена в список прошиво, ошибка в обязательных параметрах, {0}.", firmware.url);
                        }
                    }
                }
            }
            catch { }
        }

    };
}

