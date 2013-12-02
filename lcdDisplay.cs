using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MemWord
{
    class lcdDisplay
    {
        private System.IO.Ports.SerialPort serialPort1;
        public lcdDisplay()
        {
            serialPort1 = new System.IO.Ports.SerialPort();
            serialPort1.PortName = "COM3";
        }

        public bool openPort()
        {
            try
            {
                serialPort1.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        public void closePort()
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
            
        }


        public void resetDisplay()
        {
            byte[] cmd = { 0x1F };
            serialPort1.Write(cmd, 0, 1);
        }

        public void setCursor(int row, int column)
        {
            if (row == 0)
            {
                byte[] cur = { 0x10, (byte)(column) };
                serialPort1.Write(cur, 0, 2);
            }
            else if (row == 1)
            {
                byte[] cur1 = { 0x10, (byte)(column + 20) };
                serialPort1.Write(cur1, 0, 2);
            }
        }

        public void writeString(int row, int column, string str)
        {

            setCursor(row, column);
            serialPort1.Write(str);
        } 
    }
}
