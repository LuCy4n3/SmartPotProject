using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITDaysDemo.Communication.RS485;
using Timer = System.Windows.Forms.Timer;

namespace ITDaysDemo
{
    public partial class Demo : Form
    {
        public Demo()
        {
            InitializeComponent();
        }

        private void Demo_Load(object sender, EventArgs e)
        {
            Task.Run(RS485Master.Instance.ListenForCommandsAsync);
            Task.Run(RS485Master.Instance.ReadTemperatureAndHumidityAsync);
            Task.Run(RS485Master.Instance.ReadCOValuesAsync);
            RS485Master.Instance.DimmingValue = 100;
            RS485Master.Instance.LightStatus = true;
            RS485Master.Instance.LightOnOffPressed();
            //Task.Run(RS485Master.Instance.ReadVoltageAndCurrentAsync);
        }

        private void btnDimming_Click(object sender, EventArgs e)
        {
            RS485Master.Instance.DimmingPressed();
        }

        private void btnOnOff_Click(object sender, EventArgs e)
        {
            RS485Master.Instance.LightOnOffPressed();
        }

    }
}
