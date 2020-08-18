using System;
using System.Windows;

namespace AESEncryptDecrypt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Util _aesUtil;
        public MainWindow()
        {
            InitializeComponent();
            _aesUtil = new Util();
        }

        private void EncryptButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                OutputTextBox.Text = _aesUtil.EncryptToBase64(EncryptTextBox.Text, SecretKey.Text, IVKey.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DecryptButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                OutputTextBox.Text = _aesUtil.DecryptData(DecryptTextBox.Text, SecretKey.Text, IVKey.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
