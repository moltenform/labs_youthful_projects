using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Multimedia.Midi.UI
{
    public partial class DeviceDialog : Form
    {
        public DeviceDialog()
        {
            InitializeComponent();

            ASCIIEncoding encoding = new ASCIIEncoding();

            string name;

            if(InputDevice.DeviceCount > 0)
            {
                for(int i = 0; i < InputDevice.DeviceCount; i++)
                {
                    name = encoding.GetString(InputDevice.GetDeviceCapabilities(i).name);
                    inputComboBox.Items.Add(name);
                }

                inputComboBox.SelectedIndex = 0;
            }

            if(OutputDevice.DeviceCount > 0)
            {
                for(int i = 0; i < OutputDevice.DeviceCount; i++)
                {
                    name = encoding.GetString(OutputDevice.GetDeviceCapabilities(i).name);
                    outputComboBox.Items.Add(name);
                }

                outputComboBox.SelectedIndex = 0;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        public int InputDeviceID
        {
            get
            {
                return inputComboBox.SelectedIndex;
            }
        }

        public int OutputDeviceID
        {
            get
            {
                return outputComboBox.SelectedIndex;
            }
        }
    }
}