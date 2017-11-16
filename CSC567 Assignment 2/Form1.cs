using System;
using System.Text;
using System.Windows.Forms;

namespace CSC567_Assignment_2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
 
        private void button1_Click_1(object sender, EventArgs e)
        {
            // Capture user input
            string bitsEntered = textBox1.Text;
            string divisorEntered = textBox2.Text;

            // Clear labels
            label4.Text = "";
            label6.Text = "";
            label7.Text = "";
            label8.Text = "";

            // If valid user input, then create CRC, send to receiver, and decode CRC
            if (validateInput(bitsEntered) && validateInput(divisorEntered))
            {
                string sentMsg = encodeCRC(bitsEntered, divisorEntered);
                sendToReceiver();
                decodeCRC(sentMsg, divisorEntered);
            }
        }

        // Validate user input
        private bool validateInput(string s)
        {
            // Error checking: must ensure text is not empty
            if (s.Length == 0)
            {
                MessageBox.Show("Please enter bits!");
                return false;
            }

            // Error checking: must ensure entered text are valid bits
            bool isValidBits = true;
            foreach (char c in s)
            {
                if (c != '0' && c != '1')
                    isValidBits = false;
            }

            if (isValidBits == false)
            {
                MessageBox.Show("Error: Must only enter a combination of 0's and 1's!");
                return false;
            }
            return true;
        }

        // Encode bits to CRC
        private string encodeCRC(string msg, string divisor)
        {
            int m = msg.Length;
            int n = divisor.Length;
            string encoded = "";

            // Use long division for binary numbers
            encoded += msg;
            for (int i = 1; i <= n - 1; i++)
            {
                encoded += "0";
            }
            for (int i = 0; i <= encoded.Length - n;)
            {
                for (int j = 0; j < n; j++)
                {
                    StringBuilder sb = new StringBuilder(encoded);
                    sb[i + j] = encoded[i + j] == divisor[j] ? '0' : '1';
                    encoded = sb.ToString();
                }
                for (; i < encoded.Length && encoded[i] != '1'; i++) ;
            }

            // result is the remainder of long division
            string result = encoded.Substring(encoded.Length - n + 1);
            encoded = msg + result;
            
            label4.Text = "Remainder: " + result;
            label6.Text = "Encoded Message: " + encoded;

            // If user checks box, then use a random integer to change the bit so message is different
            if (checkBox1.Checked)
            {
                Random rand = new Random();
                int random = rand.Next(0, msg.Length);

                StringBuilder sb = new StringBuilder(msg);
                if (sb[random] == '1')
                {
                    sb[random] = '0';
                }
                else if (sb[random] == '0')
                {
                    sb[random] = '1';
                }
                msg = sb.ToString();
                string corruptedMsg = msg + result;
                return corruptedMsg;
            }
            else
            {
                return encoded;
            }
        }

        private void sendToReceiver()
        {
            label7.Text = "Sending message to receiver...\nDecoding CRC...";
        }

        // Decode CRC to find if message is corrupted or not
        private void decodeCRC(string encMsg, string divisor)
        {
            int m = encMsg.Length;
            int n = divisor.Length;
            string encoded = "";

            // Use long division for binary numbers
            encoded += encMsg;
            for (int i = 1; i <= n - 1; i++)
            {
                encoded += "0";
            }
            for (int i = 0; i <= encoded.Length - n;)
            {
                for (int j = 0; j < n; j++)
                {
                    StringBuilder sb = new StringBuilder(encoded);
                    sb[i + j] = encoded[i + j] == divisor[j] ? '0' : '1';
                    encoded = sb.ToString();
                }
                for (; i < encoded.Length && encoded[i] != '1'; i++) ;
            }

            // If the result of the long division contains any 1's, then message is corrupt
            bool corrupted = false;
            for(int i=0; i<encoded.Length; i++)
            {
                if (encoded[i] == '1')
                {
                    corrupted = true;
                    break;
                }
            }
            if (corrupted)
            {
                label8.Text = "CRC does not match! Message corrupted!\nCorrupted Message: " + encMsg;
            }
            else
            {
                label8.Text = "CRC matches! Message not corrupted!\nDecoded Message: " + encMsg;
            }

        }
    }
}
