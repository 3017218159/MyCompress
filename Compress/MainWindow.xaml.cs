using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;

namespace Compress
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private StreamWriter streamWriter = null;
        private string readPath;
        private string writePath;
        Byte[] compressionByte;
        string compressionString;
        private bool isMyCompress = false;
        private double before;
        private double now;
        HuffmanTree huffman = new HuffmanTree();
        CRLE rle = new CRLE();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Btn1_Click(object sender, RoutedEventArgs e)
        {
            isMyCompress = false;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "txt文件(*.txt)|*.txt|txt压缩文件(*.txt)|*.mc";
            if(openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                readPath = openFileDialog.FileName;
                FileInfo fileInfo = new FileInfo(readPath);
                System.Diagnostics.FileVersionInfo info = System.Diagnostics.FileVersionInfo.GetVersionInfo(readPath);
                tbx1.Text = System.IO.Path.GetFullPath(openFileDialog.FileName);
                if (openFileDialog.FileName.Contains(".mc"))
                {
                    isMyCompress = true;
                    btn3.IsEnabled = false;
                    btn4.IsEnabled = true;
                }
                else
                {
                    before = Math.Ceiling(fileInfo.Length / 1024.0);
                    btn4.IsEnabled = false;
                    btn3.IsEnabled = true;
                }
            }
        }

        private void Btn2_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (!isMyCompress)
            {
                saveFileDialog.Filter = "txt压缩文件(*.txt)|*.mc";
            }
            else
            {
                saveFileDialog.Filter = "txt压缩文件(*.txt)|*.txt";
            }
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                writePath = saveFileDialog.FileName;
                tbx2.Text = System.IO.Path.GetFullPath(saveFileDialog.FileName);
            }
        }

        private void Btn3_Click(object sender, RoutedEventArgs e)
        {
            if (rdb1.IsChecked == true)
            {
                try
                {
                    //streamReader = File.OpenText(readPath);
                    string content = File.ReadAllText(readPath);
                    compressionString = huffman.Compression(content);
                    streamWriter = File.CreateText(writePath);
                    streamWriter.Write(compressionString);
                    System.Windows.MessageBox.Show("压缩成功");
                    streamWriter.Flush();
                    streamWriter.Close();
                    FileInfo fileInfo = new FileInfo(writePath);
                    System.Diagnostics.FileVersionInfo info = System.Diagnostics.FileVersionInfo.GetVersionInfo(writePath);
                    now = Math.Ceiling(fileInfo.Length / 1024.0);
                    double result = now / before;
                    System.Windows.MessageBox.Show("压缩率为" + (result * 100).ToString("#0.#0") + "%");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
            else if (rdb2.IsChecked == true)
            {
                //System.Windows.MessageBox.Show("no");
                try
                {
                    string content = File.ReadAllText(readPath);
                    Byte[] getByte = Encoding.Default.GetBytes(content);
                    compressionByte = rle.Compress(getByte);
                    //System.Windows.MessageBox.Show(getByte.ToString());
                    streamWriter = File.CreateText(writePath);
                    string write = Encoding.Default.GetString(compressionByte);
                    streamWriter.Write(write);
                    System.Windows.MessageBox.Show("压缩成功");
                    streamWriter.Flush();
                    streamWriter.Close();
                    FileInfo fileInfo = new FileInfo(writePath);
                    System.Diagnostics.FileVersionInfo info = System.Diagnostics.FileVersionInfo.GetVersionInfo(writePath);
                    now = Math.Ceiling(fileInfo.Length / 1024.0);
                    double result = now / before;
                    System.Windows.MessageBox.Show("压缩率为" + (result * 100).ToString("#0.#0") + "%");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("请选择一种压缩方式");
            }
        }

        private void Btn4_Click(object sender, RoutedEventArgs e)
        {
            if (rdb1.IsChecked == true)
            {
                //System.Windows.MessageBox.Show("yes");
                try
                {
                    compressionString = File.ReadAllText(readPath);
                    string unZipContent = huffman.Unzip(compressionString);
                    streamWriter = File.CreateText(writePath);
                    streamWriter.Write(unZipContent);
                    System.Windows.MessageBox.Show("解压成功");
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
            else if (rdb2.IsChecked == true)
            {
                try
                {
                    string content = File.ReadAllText(readPath);
                    Byte[] getByte = Encoding.Default.GetBytes(content);
                    Byte[] unCompressContent = rle.UnCompress(getByte);
                    string write = Encoding.Default.GetString(unCompressContent);
                    streamWriter = File.CreateText(writePath);
                    streamWriter.Write(write);
                    System.Windows.MessageBox.Show("解压成功");
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                catch(Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("请选择一种解压方式");
            }
        }
    }
}
