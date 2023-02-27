using RestSharp;
using System;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using System.IO;

namespace RedditArchive
{
    /// <summary>
    /// Interaction logic for EnterCredentials.xaml
    /// </summary>
    public partial class EnterCredentials : Window
    {
        public class Token
        {
            public string Access_Token;
            public string Token_Type;
            public string Expires_In;
            public string Scope;
        }

        public MainWindow.Credentials CredentialsToSave = new MainWindow.Credentials();

        public EnterCredentials()
        {
            InitializeComponent();
        }

        private void EnterCredentials_LostFocus(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }

        private void btnFinish_Clicked(object sender, RoutedEventArgs e)
        {
            if (!TextBox_ValidationChecks())
                return;

            if (TestSQLConnection() && TestRedditAppCredentials())
            {
                SaveCredentials();
            }

            this.Close();
        }

        private bool TestRedditAppCredentials()
        {
            try
            {
                RestClient client = new RestClient();

                RestRequest request = new RestRequest("https://www.reddit.com/api/v1/access_token", Method.Post);

                request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(txtAppID.Text + ":" + txtAppSecret.Text)));
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("grant_type", "password");
                request.AddParameter("username", txtUsername.Text);
                request.AddParameter("password", txtPassword.Text);

                RestResponse response = client.Execute(request);

                if (response.ResponseStatus != ResponseStatus.Completed)
                {
                    MessageBox.Show("Reddit credentials are incorrect");
                    return false;
                }

                Token AppToken = JsonConvert.DeserializeObject<Token>(response.Content);

                CredentialsToSave.AppID = txtAppID.Text;
                CredentialsToSave.AppSecret = txtAppSecret.Text;
                CredentialsToSave.Username = txtUsername.Text;
                CredentialsToSave.Password = txtPassword.Text;
                CredentialsToSave.AccessToken = AppToken.Access_Token;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Reddit credentials are incorrect");
                return false;
            }
        }

        private bool TestSQLConnection()
        {
            try
            {
                SqlConnection SQLConn = new SqlConnection("Data Source=" + txtDataSource.Text + ";Initial Catalog=" + txtDatabase.Text + ";Integrated Security=true;");
                SQLConn.Open();

                CredentialsToSave.DataSource = txtDataSource.Text;
                CredentialsToSave.DatabaseName = txtDatabase.Text;

                SQLConn.Close();

                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Failed to connect to SQL database");
                return false;
            }
        }

        private void SaveCredentials()
        {
            try
            {
                string ProjectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

                string Path = ProjectDirectory + @"\Credentials.json";

                string JsonOutput = JsonConvert.SerializeObject(CredentialsToSave);

                using (StreamWriter writer = new StreamWriter(Path, true))
                {
                    writer.WriteLine(JsonOutput);
                    writer.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool TextBox_ValidationChecks()
        {
            if (txtAppID.Text.Replace(" ", "").Length < 1)
            {
                MessageBox.Show("Please enter an App ID");
                txtAppID.Focus();
                return false;
            }
            
            if (txtAppSecret.Text.Replace(" ", "").Length < 1)
            {
                MessageBox.Show("Please enter an App Secret");
                txtAppSecret.Focus();
                return false;
            }

            if (txtUsername.Text.Replace(" ", "").Length < 1)
            {
                MessageBox.Show("Please enter your reddit username");
                txtUsername.Focus();
                return false;
            }

            if (txtPassword.Text.Replace(" ", "").Length < 1)
            {
                MessageBox.Show("Please enter your reddit password");
                txtPassword.Focus();
                return false;
            }

            if (txtDataSource.Text.Replace(" ", "").Length < 1)
            {
                MessageBox.Show("Please enter your SQL data source");
                txtDataSource.Focus();
                return false;
            }

            if (txtDatabase.Text.Replace(" ", "").Length < 1)
            {
                MessageBox.Show("Please enter the SQL database you'd like to access");
                txtDatabase.Focus();
                return false;
            }

            return true;
        }
    }
}
