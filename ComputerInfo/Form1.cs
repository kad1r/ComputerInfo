using System;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace ComputerInfo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                var cpuid = string.Empty;
                var hddserial = string.Empty;
                var motherboardserial = string.Empty;
                var macaddress = string.Empty;

                #region cpu id

                string sQuery = "SELECT ProcessorId FROM Win32_Processor";
                ManagementObjectSearcher oManagementObjectSearcher = new ManagementObjectSearcher(sQuery);
                ManagementObjectCollection oCollection = oManagementObjectSearcher.Get();
                foreach (ManagementObject oManagementObject in oCollection)
                {
                    cpuid = (string)oManagementObject["ProcessorId"];
                    break;
                }

                #endregion cpu id

                #region mac address

                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                String sMacAddress = string.Empty;
                foreach (NetworkInterface adapter in nics)
                {
                    if (sMacAddress == String.Empty)
                    {
                        IPInterfaceProperties properties = adapter.GetIPProperties();
                        macaddress = adapter.GetPhysicalAddress().ToString();
                    }
                }

                #endregion mac address

                #region hdd serial

                ManagementObject dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""c:""");
                dsk.Get();
                hddserial = dsk["VolumeSerialNumber"].ToString();

                #endregion hdd serial

                #region motherboard serial

                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
                ManagementObjectCollection moc = mos.Get();
                foreach (ManagementObject mo in moc)
                {
                    motherboardserial = (string)mo["SerialNumber"];
                    break;
                }

                #endregion motherboard serial

                var code = cpuid.Trim() + " " + hddserial.Trim() + " " + motherboardserial.Trim() + " " + macaddress.Trim();
                code = GetMd5Sum(code);
                txtLicence.Text = code;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public static string GetMd5Sum(string str)
        {
            Encoder enc = System.Text.Encoding.Unicode.GetEncoder();
            byte[] unicodeText = new byte[str.Length * 2];
            enc.GetBytes(str.ToCharArray(), 0, str.Length, unicodeText, 0, true);

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(unicodeText);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}