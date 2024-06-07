using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RJCP.IO.Ports;

namespace ProgrammerUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (SerialPortStream serialPortStream = new SerialPortStream("COM4", 9600))
            {
                serialPortStream.Open();
                byte[] buffer = { 0x2c, 0x8e, 0x23, 0x02, 0xFF /*which ports to change status*/, 0x00, 0xFF, 0xFF };
                //byte[] buffer = new byte[] { 0x2c, 0x8e, 0x23, 0x01, 0xFF, 0xFF, 0xFF, 0xFF };//restart
                serialPortStream.Write(buffer, 0, buffer.Length);
                serialPortStream.Flush();
                serialPortStream.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                using (SerialPortStream serialPortStream = new SerialPortStream("COM4", 9600))
                {
                    serialPortStream.Open();
                    while (true)
                    {
                        if (serialPortStream.BytesToRead > 0)
                        {
                            //byte[] barr = new byte[serialPortStream.BytesToRead];
                            //serialPortStream.Read(barr, 0, serialPortStream.BytesToRead);
                            Invoke(new Action<String>(UpdateRichText), serialPortStream.ReadExisting());
                        }

                        Thread.Sleep(500);
                    }

                    serialPortStream.Close();
                }

            });
        }

        void UpdateRichText(String val)
        {
            richTextBox1.AppendText(val + "\r\n");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (Byte.TryParse(textBox1.Text, out var dimmingVal))
            {
                using (var serialPort = new SerialPortStream("COM4", 9600))
                {
                    serialPort.Open();
                    serialPort.WriteByte(dimmingVal);
                }
            }
            else
            {
                MessageBox.Show("Invalid dimming value");
            }
        }

        private void SendLightStatuses(byte[] statuses)
        {
            try
            {
                for (var i = 0; i < statuses.Length; i++)
                {
                    statuses[i] = (byte)(0b10000000 | statuses[i]);
                }

                using (var serialPort = new SerialPortStream("COM4", 9600))
                {
                    serialPort.ReadBufferSize = 8096;
                    serialPort.Open();
                    serialPort.WriteByte(0x2C); //magic header
                    serialPort.WriteByte(0x8E); //magic header
                    serialPort.WriteByte(0x23);
                    //serialPort.WriteByte(0x27); //device id
                    serialPort.WriteByte(0x02); //cmd set light statuses
                    serialPort.Write(statuses, 0, statuses.Length);
                    serialPort.Flush();
                    richTextBox1.Text += "Command sent\r\n";
                    Thread.Sleep(3000);
                    if (serialPort.BytesToRead > 0)
                    {
                        var data = serialPort.ReadExisting();
                        richTextBox1.Text += data;
                    }
                    else
                    {
                        richTextBox1.Text += "No data received\r\n";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.ForeColor = Color.White;
                richTextBox1.Text += "Sending command\r\n";

                byte[] lightsCommand = new byte[8];
                lightsCommand[0] = (byte)(0b10000000 | byte.Parse(textBox2.Text));
                lightsCommand[1] = (byte)(0b10000000 | byte.Parse(textBox3.Text));
                lightsCommand[2] = (byte)(0b10000000 | byte.Parse(textBox4.Text));
                lightsCommand[3] = (byte)(0b10000000 | byte.Parse(textBox5.Text));
                lightsCommand[4] = (byte)(0b10000000 | byte.Parse(textBox6.Text));
                lightsCommand[5] = (byte)(0b10000000 | byte.Parse(textBox7.Text));
                lightsCommand[6] = (byte)(0b10000000 | byte.Parse(textBox8.Text));
                lightsCommand[7] = (byte)(0b10000000 | byte.Parse(textBox9.Text));

                SendLightStatuses(lightsCommand);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void button6_Click(object sender, EventArgs e)
        {
            using (var serialPort = new SerialPortStream("COM4", 9600))
            {
                serialPort.ReadBufferSize = 8096;
                serialPort.Open();
                serialPort.WriteByte(0x2C);//magic header
                serialPort.WriteByte(0x8E);//magic header
                //serialPort.WriteByte(0x27);//device id
                serialPort.WriteByte(0x23);//device id
                serialPort.WriteByte(0x01);//cmd restart
                byte[] data = Enumerable.Range(1, 8).Select(r => (byte)r).ToArray();
                serialPort.Write(data, 0, data.Length);
                serialPort.Flush();

                richTextBox1.Text += "Restart sent\r\n";
                Thread.Sleep(1000);
                String response = serialPort.ReadExisting();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SendLightStatuses(new byte[8]);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SendLightStatuses(Enumerable.Repeat((byte)100, 8).ToArray());
        }

        private void btnReadStatuses_Click(object sender, EventArgs e)
        {
            using (var serialPort = new SerialPortStream("COM4", 9600))
            {
                serialPort.ReadBufferSize = 8096;
                serialPort.Open();
                serialPort.WriteByte(0x2C);//magic header
                serialPort.WriteByte(0x8E);//magic header
                //serialPort.WriteByte(0x27);//device id
                serialPort.WriteByte(0x23);//device id
                serialPort.WriteByte(0x04);//cmd get statuses
                byte[] data = new byte[8];
                serialPort.Write(data, 0, data.Length);
                serialPort.Flush();

                Thread.Sleep(1000);

                if (serialPort.BytesToRead >= 12)
                {
                    byte[] buffer = new byte[12];
                    serialPort.Read(buffer, 0, buffer.Length);
                    for (var i = 0; i < 8; i++)
                    {
                        var textBox = panel2.Controls.OfType<TextBox>().Single(c => c.Tag.ToString() == i.ToString());
                        textBox.Text = buffer[i + 4].ToString();
                    }
                }
                richTextBox1.Text += "Read lights done \r\n";

            }
        }
        private bool rtsState = false;
        SerialPortStream sport;
        private void button9_Click(object sender, EventArgs e)
        {
            if (sport == null)
            {
                sport = new SerialPortStream("COM5", 9600);
                sport.Open();
            }

            for (var i = 0; i < 30; i++)
            {
                rtsState = !rtsState;
                sport.RtsEnable = sport.DtrEnable = rtsState;
                Thread.Sleep(2000);
            }
        }
    }
}
