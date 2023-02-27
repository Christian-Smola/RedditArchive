using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace RedditArchive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class Credentials
        {
            public string AppID;
            public string AppSecret;
            public string AccessToken;
            public string Username;
            public string Password;
            public string DataSource;
            public string DatabaseName;

            public Credentials(string ID, string Secret, string Token, string User, string Pass, string Source, string DBName)
            {
                AppID = ID;
                AppSecret = Secret;
                AccessToken = Token;
                Username = User;
                Password = Pass;
                DataSource = Source;
                DatabaseName = DBName;
            }

            public Credentials()
            {

            }
        }

        public class Post
        {
            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("selftext")]
            public string Text { get; set; }

            [JsonProperty("author")]
            public string Poster { get; set; }

            [JsonProperty("subreddit")]
            public string Subreddit { get; set; }

            [JsonProperty("score")]
            public int Score { get; set; }

            [JsonProperty("url")]
            public string ContentLink { get; set; }

            [JsonProperty("permalink")]
            public string CommentLink { get; set; }
        }

        public Credentials MyCredentials;
        public SqlConnection SQLConn;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void RedditArchive_IsLoaded(object sender, RoutedEventArgs e)
        {
            LoadCredentials();
            EstablishSQLConn();
            LoadPosts();
        }

        private void LoadCredentials()
        {
            string ProjectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

            string Path = ProjectDirectory + @"\Credentials.json";

            if (File.Exists(Path))
            {
                using (StreamReader reader = new StreamReader(Path, true))
                {
                    MyCredentials = JsonConvert.DeserializeObject<Credentials>(reader.ReadToEnd());
                    reader.Close();
                }
            }
            else
            {
                EnterCredentials Page = new EnterCredentials();
                Page.ShowDialog();

                MyCredentials = Page.CredentialsToSave;
                MessageBox.Show(MyCredentials.Username);
            }
        }

        private void EstablishSQLConn()
        {
            try
            {
                SQLConn = new SqlConnection("Data Source=" + MyCredentials.DataSource + ";Initial Catalog=" + MyCredentials.DatabaseName + ";Integrated Security=true;");
                SQLConn.Open();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Failed to connect to SQL database");
            }
        }

        private void LoadPosts()
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
