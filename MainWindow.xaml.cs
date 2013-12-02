using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MemWord.db;
using System.Windows.Threading;
using System.Collections;
using System.Threading;
using SpeechLib;

namespace MemWord
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        int i ;
        private lcdDisplay lcdDis;
        Dbcon dbco;
        bool autoRead = false;
        bool lcdOn = false;
        DispatcherTimer mytimer;
        ArrayList alist;
        Thread myThread ;
  //      Thread speechThread;
        string richWords;
 //       clsMCI mc;
        clsMCI ms;
        public MainWindow()
        {
            InitializeComponent();
            dbco = new Dbcon();
            dbco.getCon();
 //           mc = new clsMCI();
            richTextBox1.Document.LineHeight = 10;
            if (Properties.Settings.Default.bookmarks==null)
            {
                alist = new ArrayList();
            }else{
                alist = Properties.Settings.Default.bookmarks;
            }
            updateBookmark();
            i = Properties.Settings.Default.wordPos;
            button1_Click(null, null);
            mytimer = new DispatcherTimer();
            slider1.Value = 7;
            myThread = new Thread(new ThreadStart(playMusic));
 //           speechThread = new Thread(new ThreadStart(SpeechString));
            ms = new clsMCI();
            mytimer.IsEnabled = false;
            mytimer.Interval = TimeSpan.FromSeconds(3);
            mytimer.Tick+=new EventHandler(mytimer_Tick);
        }

        private void mytimer_Tick(object sender, EventArgs e){
            button1_Click(null, null);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            
            if(i>15328){
                i = 15328;
            }
            if (i < 1) {
                i = 1;
            }
            voca vo = dbco.getVocaByID(i);
            textBox1.Text = vo.word;
            if (lcdOn)
            {
                lcdDis.resetDisplay();
                lcdDis.writeString(0, 0, vo.word);
            }
            webBrowser1.NavigateToString(ConvertExtendedASCII("<body oncontextmenu='return false' bgcolor='#3AE6E6'><font size='3' face='微软雅黑'>" + vo.meaning + "</font></body>"));
            richTextBox1.SelectAll();
 //           richTextBox1.Cut();
            richTextBox1.Selection.Text = "";
            string s = vo.example;
            s=s.Replace("/r/n", "\r\n");
            richTextBox1.AppendText(s);
            if (autoRead)
            {
                read_Click(null, null);
            }
            
            textBox2.Text=""+i;
            input.Clear();
            i++;
        }

        private static string ConvertExtendedASCII(string HTML)
        {
            string retVal = "";
            char[] s = HTML.ToCharArray();

            foreach (char c in s)
            {
                if (Convert.ToInt32(c) > 127)
                    retVal += "&#" + Convert.ToInt32(c) + ";";
                else
                    retVal += c;
            }

            return retVal;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            string s = "hello\r\njiu\r\nren";
            textBox2.Text=s;
        }

        private void GO_Click(object sender, RoutedEventArgs e)
        {
            
            int num=0;
            try
            {
                num = Convert.ToInt32(textBox2.Text);
            }
            catch (System.Exception ex)
            {

                MessageBox.Show(ex.Message.ToString());
                return;
            }
            finally
            {
                i = num;
            }
            button1_Click(sender, e);
            
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key==Key.Enter)
            {
                if (input.Text.Equals(textBox1.Text))
                {
                    button1_Click(null, null);
                }
                else
                {
                    input.SelectAll();
                    clsMCI mci = new clsMCI();
                    mci.FileName = "E:\\Program Files\\Tencent\\QQ\\Misc\\Sound\\Classic\\system.wav";
                    mci.play();
                }
                
            }
        }

        private void buttonPREV_Click(object sender, RoutedEventArgs e)
        {
            i--;
            i--;
            button1_Click(null, null);
        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key==Key.Enter)
            {
                GO_Click(null,null);
            }
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
          //  MessageBox.Show();
            double d = dbco.getIDByFirstLetter((e.Source as MenuItem).Header.ToString().ToLower());
            textBox2.Text = d.ToString();
            GO_Click(null, null);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            dbco.closeAll();
            Properties.Settings.Default.wordPos=i-1;
            Properties.Settings.Default.bookmarks = alist;
            Properties.Settings.Default.Save();
            if (lcdOn)
            {
                lcdDis.resetDisplay();
                lcdDis.closePort();
                lcdDis = null;
            }
            
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            textBox1.Foreground = Brushes.Black;
        }

        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            textBox1.Foreground = Brushes.LightBlue;
        }

        private void checkBox2_Checked(object sender, RoutedEventArgs e)
        {
            if (webBrowser1!=null)
            {
                webBrowser1.Visibility = Visibility.Visible;
            }
        }

        private void checkBox2_Unchecked(object sender, RoutedEventArgs e)
        {
           webBrowser1.Visibility = Visibility.Hidden;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            mytimer.Interval = TimeSpan.FromSeconds(slider1.Value);
            if (PlayButton.Content.Equals("PLAY"))
            {
                mytimer.IsEnabled = true;
                PlayButton.Content = "STOP";
            //    PlayButton.Background = Brushes.OrangeRed;
            }else{
                mytimer.IsEnabled = false;
                PlayButton.Content = "PLAY";
            //    PlayButton.Background = Brushes.Silver;
            }
            
        }



        private void read_Click(object sender, RoutedEventArgs e)
        {
            clsMCI mc = new clsMCI();
            string path = "E:\\softwaress\\Translator2\\speech\\Lingoes English\\voice\\";
            string word=textBox1.Text;
            mc.FileName = path + word.Substring(0, 1) + "\\" + word+".wav";
           
            mc.play();
            }

        private void checkBox3_Checked(object sender, RoutedEventArgs e)
        {
            autoRead = true;
        }

        private void checkBox3_Unchecked(object sender, RoutedEventArgs e)
        {
            autoRead = false;
        }

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            if (read!=null)
            {
                read.Visibility = Visibility.Visible;
            }
            if (checkBox3!=null)
            {
                checkBox3.Visibility = Visibility.Visible;
            }
            
        }

        private void MenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBox3.IsChecked = false;
           read.Visibility = Visibility.Collapsed;
           checkBox3.Visibility = Visibility.Collapsed;
        }

        private void MenuItem_Bookmark(object sender, RoutedEventArgs e)
        {
            if (!alist.Contains(i - 1))
            {
                alist.Add(i - 1);
                updateBookmark();
            }
            
        }

        private void BookmarkCollection_Click(object sender, RoutedEventArgs e)
        {
            i = Convert.ToInt32((e.Source as MenuItem).Header.ToString());
            button1_Click(null,null);
        }

        private void MenuItem_ClearAll(object sender, RoutedEventArgs e)
        {
            alist.Clear();
            updateBookmark();
        }

        private void updateBookmark(){
            if (BookmarkCollection.Items.Count>0)
            {
                BookmarkCollection.Items.Clear();
            }
            if (alist!=null&&alist.Count>0)
            {
                for (int m = 0; m < alist.Count; m++)
                {
                    MenuItem ite = new MenuItem();
                    ite.Header = (int)alist[m];
                    BookmarkCollection.Items.Add(ite);
                }
            }
            else{
                MenuItem it = new MenuItem();
                it.Header = "No bookmarks";
                it.IsEnabled = false;
                BookmarkCollection.Items.Add(it);
            }
           
        }

        private void FindWord_Click(object sender, RoutedEventArgs e)
        {
   //         MessageBox.Show(myThread.ThreadState.ToString());
            Query queryWin = new Query();
            queryWin.Left = this.Left+70;
            queryWin.Top = this.Top+70;
            queryWin.ShowDialog();
            if (queryWin.word.Equals(""))
            {
            } 
            else
            {
                double d=dbco.getIDByFirstLetter(queryWin.word);
                
                if (d<0.5)
                {
                    MessageBox.Show("No word found!");
                    return;
                }
                i = (int)d;
                button1_Click(null,null);
            }
        }

        private void BgMusic_Click(object sender, RoutedEventArgs e)
        {
            if (myThread.ThreadState==ThreadState.Unstarted)
            {
                myThread.Start();
            }else{
                ms.Puase();
            }
        }

        
        private void playMusic(){
            
            ms.FileName = "E:\\mm.wav";
            ms.play();
            
        }

        private void SpeechString(string words)
        {
            SpeechLib.SpVoiceClass sp = new SpeechLib.SpVoiceClass();
            SpeechLib.ISpeechObjectTokens voices=sp.GetVoices("Language = 409","");
            sp.Voice = voices.Item(0);
            sp.Speak(words);
        }

        private void SpeechString() {
            SpeechString(richWords);
        }

        
        private  void TTS_Click(object sender, RoutedEventArgs e)
        {
            string words = richTextBox1.Selection.Text;
            richWords = words;
            if (!words.Equals(""))
            {
               Thread speechThread = new Thread(new ThreadStart(SpeechString));
               speechThread.Start();
            }
   //         MessageBox.Show("hello");
        }

        private void ThumbButtonInfo_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ThumbButtonInfo_Click_1(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void ThumbButtonInfo_Click_2(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void backgroundC_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
			Microsoft.Win32.OpenFileDialog open = new Microsoft.Win32.OpenFileDialog();//定义打开文本框实体
           open.Title = "Select the background picture";//对话框标题
            open.Filter = "image file（.jpg,*.bmp,*.png）|*.jpg;*.bmp;*.png";//文件扩展名
            if ((bool)open.ShowDialog().GetValueOrDefault())//打开
            {
                ImageBrush img = new ImageBrush();
                img.Stretch = Stretch.Fill;
                img.ImageSource = new BitmapImage(new Uri(open.FileName));
                grid.Background = img;
                
            }
			
        }

        private void SendToLCD_Click(object sender, RoutedEventArgs e)
        {
            if (!lcdOn)
            {
                lcdDis = new lcdDisplay();
                if (!lcdDis.openPort())
                {
                    lcdOn = false;
                    SendToLCD.IsChecked = false;
                    return;
                }
                //lcdDis.openPort();
                lcdDis.resetDisplay();
                lcdOn = true;
                SendToLCD.IsChecked = true;
            }
            else
            {
                lcdOn = false;
                lcdDis.resetDisplay();
                lcdDis.closePort();
                lcdDis = null;
                SendToLCD.IsChecked = false;
            }
            
        }



    }
}
