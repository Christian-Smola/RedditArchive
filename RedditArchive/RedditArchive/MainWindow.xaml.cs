using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using Newtonsoft.Json;
using RestSharp;

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

        public class RedditResponse
        {
            public string kind;
            public RedditData data;
        }

        public class RedditData
        {
            [JsonProperty("children")]
            public List<PostContainer> Containers;
        }

        public class PostContainer
        {
            public string kind;
            [JsonProperty("data")]
            public Post post;
        }

        public class Post
        {
            [JsonProperty("name")]
            public string PostID { get; set; }

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

        public bool IsLoaded = false;
        public Credentials MyCredentials;
        public SqlConnection SQLConn;
        public ObservableCollection<Post> PostsToDisplay = new ObservableCollection<Post>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void RedditArchive_IsLoaded(object sender, RoutedEventArgs e)
        {
            LoadCredentials();
            EstablishSQLConn();
            ImportSavedPostsFromReddit("Before");
            LoadPosts();

            IsLoaded = true;
        }

        private void btnLoadOldPosts_Clicked(object sender, RoutedEventArgs e)
        {
            ImportSavedPostsFromReddit("After");
        }

        private void DisplayedSubreddit_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            PostsToDisplay.Clear();

            string strSQL = "";

            if (cboDisplayedSubreddit.SelectedIndex == 0)
                strSQL = "Select Title, Text, Poster, Subreddit, Score, ContentLink, CommentLink From MyRedditArchiveDemo Order By LocalIndex";
            else
                strSQL = "Select Title, Text, Poster, Subreddit, Score, ContentLink, CommentLink From MyRedditArchiveDemo Where Subreddit ='" + cboDisplayedSubreddit.SelectedValue.ToString() + "' Order By LocalIndex";

            SqlCommand cmd = new SqlCommand(strSQL, SQLConn);

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Post post = new Post();

                post.Title = reader.GetString(0);
                post.Text = reader.GetString(1);
                post.Poster = reader.GetString(2);
                post.Subreddit = reader.GetString(3);
                post.Score = reader.GetInt32(4);
                post.ContentLink = reader.GetString(5);
                post.CommentLink = reader.GetString(6);

                PostsToDisplay.Add(post);
            }

            reader.Close();
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
                //Populates Datagrid
                string strSQL = "Select Title, Text, Poster, Subreddit, Score, ContentLink, CommentLink From MyRedditArchiveDemo Order By LocalIndex";

                SqlCommand cmd = new SqlCommand(strSQL, SQLConn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Post post = new Post();

                    post.Title = reader.GetString(0);
                    post.Text = reader.GetString(1);
                    post.Poster = reader.GetString(2);
                    post.Subreddit = reader.GetString(3);
                    post.Score = reader.GetInt32(4);
                    post.ContentLink = reader.GetString(5);
                    post.CommentLink = reader.GetString(6);

                    PostsToDisplay.Add(post);
                }

                reader.Close();

                MyDataGrid.ItemsSource = PostsToDisplay;

                //Populates Dropdown
                cboDisplayedSubreddit.Items.Add("All");

                strSQL = "Select Distinct Subreddit From MyRedditArchiveDemo";

                cmd = new SqlCommand(strSQL, SQLConn);

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cboDisplayedSubreddit.Items.Add(reader.GetString(0));
                }

                reader.Close();

                cboDisplayedSubreddit.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ImportSavedPostsFromReddit(string strSuffix)
        {
            try
            {
                string strSQL = "";
                int StartingIndex = 0;
                string strPostID = "";

                if (strSuffix == "Before")
                {
                    strSQL = "Select Top(1) PostID From MyRedditArchiveDemo Order By LocalIndex";

                    SqlCommand cmd = new SqlCommand(strSQL, SQLConn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        strPostID = reader.GetString(0);
                    }

                    reader.Close();
                }
                else if (strSuffix == "After")
                {
                    strSQL = "Select Top(1) PostID, LocalIndex From MyRedditArchiveDemo Order By LocalIndex Desc";

                    SqlCommand cmd = new SqlCommand(strSQL, SQLConn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        strPostID = reader.GetString(0);
                        StartingIndex = reader.GetInt32(1);
                    }

                    reader.Close();
                }

                RestClient client = new RestClient("https://oauth.reddit.com/user/" + MyCredentials.Username + "/saved.json?limit=20&" + strSuffix.ToLower() + "=" + strPostID);
                RestRequest request = new RestRequest();

                request.AddHeader("Authorization", "bearer " + MyCredentials.AccessToken);

                RestResponse response = client.Execute(request);

                RedditResponse redditResponse = JsonConvert.DeserializeObject<RedditResponse>(response.Content);

                List<Post> posts = new List<Post>();

                foreach (PostContainer container in redditResponse.data.Containers)
                    posts.Add(container.post);

                if (StartingIndex == 0 && posts.Count > 0)
                    UpdatePostIndex(posts.Count);

                //This section then saves those posts to our local SQL database
                int i = 0;
                foreach (Post post in posts)
                {
                    strSQL = "Insert Into MyRedditArchiveDemo(PostID, LocalIndex, Title, Text, Poster, Subreddit, Score, ContentLink, CommentLink) Values(";

                    strSQL += "'" + post.PostID.Replace("'", "''") + "',";

                    if (StartingIndex == 0)
                        strSQL += i + ", ";
                    else
                        strSQL += (StartingIndex + i) + ", ";

                    strSQL += "'" + post.Title.Replace("'", "''") + "', ";
                    strSQL += "'" + post.Text.Replace("'", "''") + "', ";
                    strSQL += "'" + post.Poster.Replace("'", "''") + "', ";
                    strSQL += "'" + post.Subreddit.Replace("'", "''") + "', ";
                    strSQL += post.Score + ", ";
                    strSQL += "'" + post.ContentLink.Replace("'", "''") + "', ";
                    strSQL += "'" + post.CommentLink.Replace("'", "''") + "')";

                    SqlCommand cmd = new SqlCommand(strSQL, SQLConn);
                    cmd.ExecuteNonQuery();

                    i++;
                }

                if (strSuffix == "After")
                    foreach (Post post in posts)
                        PostsToDisplay.Add(post);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdatePostIndex(int IncrementIndicesBy)
        {
            string strSQL = "Select LocalIndex From MyRedditArchiveDemo Order By LocalIndex Desc";

            SqlCommand cmd = new SqlCommand(strSQL, SQLConn);
            SqlDataReader reader = cmd.ExecuteReader();

            List<int> IndexList = new List<int>();

            while (reader.Read())
            {
                IndexList.Add(reader.GetInt32(0));
            }

            reader.Close();

            foreach (int i in IndexList)
            {
                strSQL = "Update MyRedditArchiveDemo Set LocalIndex = " + (i + IncrementIndicesBy) + " Where LocalIndex = " + i;
                cmd = new SqlCommand(strSQL, SQLConn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
