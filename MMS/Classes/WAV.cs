using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MMS
{
    public class WAV
    {
        //sampling
        public static string NSampling(string filename, int n)
        {
                byte[] input = File.ReadAllBytes(filename);

                short noChannels = BitConverter.ToInt16(input, 22);
                short bps = BitConverter.ToInt16(input, 34);
                int BPS = bps / 8;

                int data = input.Length - 44;
                int offset = data / noChannels;
                int samples = offset / BPS;

                byte[] m2 = new byte[input.Length - 44];
                Buffer.BlockCopy(input, 44, m2, 0, m2.Length);

                for (int j = 0; j < noChannels; j++)
                    for (int k = 1; k < samples; k++)
                        if (k % n == 0)
                        {
                            double pom = ((k - 1) + (k + 1)) / 2;
                            m2[j * BPS + k * BPS * noChannels] = (byte)pom;
                        }
                        else
                            m2[j * BPS + k * BPS * noChannels] = (byte)(m2[j * BPS + k * BPS * noChannels]);

                Buffer.BlockCopy(m2, 0, input, 44, m2.Length);
            string newpath = Path.GetDirectoryName(filename) + "\\" + Path.GetFileNameWithoutExtension(filename) + "SAMPLED" + n.ToString() + ".wav";

            File.WriteAllBytes(newpath, input);
            return newpath;
        }

        //concatenation
        private static byte[] Concat(byte[][] datas, int tmp)
        {
            for (int i = 0; i < tmp - 1; i++)
            {
                int s1 = BitConverter.ToInt32(datas[i], 24);
                int s2 = BitConverter.ToInt32(datas[i + 1], 24);
                if (s1 != s2)
                    return null;
            }

            int finalSize = 0;
            int offset = 0;
            for (int i = 0; i < tmp; i++)
                finalSize += datas[i].Length - 44;

            byte[] final = new byte[44 + finalSize];

            Buffer.BlockCopy(datas[0], 0, final, 0, datas[0].Length);
            offset += datas[0].Length;

            for (int i = 1; i < tmp; i++)
            {
                Buffer.BlockCopy(datas[i], 44, final, offset, datas[i].Length - 44);
                offset += datas[i].Length - 44;
            }

            byte[] len = BitConverter.GetBytes(finalSize);
            Buffer.BlockCopy(len, 0, final, 40, 4);
            byte[] len2 = BitConverter.GetBytes(finalSize + 44);
            Buffer.BlockCopy(len2, 0, final, 4, 4);

            return final;
        }

        public static string Concatenation(int c)
        {
            byte[][] datas;
            datas = new byte[c][];
            string sname = "";
            string dir = "";

            for (int i = 0; i < c; i++)
            {


                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    datas[i] = File.ReadAllBytes(ofd.FileName);
                    sname += i.ToString() + Path.GetFileNameWithoutExtension(ofd.FileName);

                    if (i == 0)
                        dir = Path.GetDirectoryName(ofd.FileName);
                }
            }

            dir += "\\"; dir += sname; dir += ".wav";

            byte[] toWrite = Concat(datas, c);
            if (toWrite == null)
            {
                MessageBox.Show("Wave files are not compatible and can not be concatenated. Try wave files on same frequency.");
                return null;
            }
            File.WriteAllBytes(dir, toWrite);
            return dir;
        }
    }
}
