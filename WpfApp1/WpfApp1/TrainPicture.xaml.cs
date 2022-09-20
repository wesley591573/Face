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
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WpfApp1
{
    /// <summary>
    /// TrainPicture.xaml 的互動邏輯
    /// </summary>
    public partial class TrainPicture : Window
    {
        List<Person> person_List = new List<Person>();

        public TrainPicture()
        {
            InitializeComponent();

            // List();

            //  Detect();

            // Show_Pictures("Jisoo");

            // Delete_Person("Yuna");

            Show_All_People();
        }

        // 把所有的Person都顯示出來，並存進JsonArray回傳
        private async Task<JArray> List()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            string jstring;

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "9afa2810150f48d999afac58761d4773");
            var uri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/persongroups/111_group_111522109/persons?" + queryString;


            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.GetAsync(uri);

                jstring = response.Content.ReadAsStringAsync().Result;
            }


            JArray jArray = JArray.Parse(jstring);

            Debug.Text = response.Content.ReadAsStringAsync().Result;

            return jArray;

        } // List()


        // 根據 URL 和 personId 把照片新增進 person 裡面
        private async void Add_Face(string personId)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "9afa2810150f48d999afac58761d4773");

            // Request parameters
            queryString["userData"] = tb_URL.Text; // 直接把URL當作userData，之後也比較好找圖片
            // queryString["targetFace"] = "{string}";
            queryString["detectionModel"] = "detection_01";
            var uri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/persongroups/111_group_111522109/persons/" + personId + "/persistedFaces?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{'url':'" + tb_URL.Text + "'}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }

            Debug.Text = response.Content.ReadAsStringAsync().Result;

            // Close();

        } // Add_Face()


        private async void Detect()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            string jstring;

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "9afa2810150f48d999afac58761d4773");
            var uri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/detect" + queryString;


            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{'url':'https://cdn.bella.tw/index_image/RhIdpwQFOdE4qq3JWIgN8BBP3SKnPPSEcdtLhoQF.jpeg'}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);

                jstring = response.Content.ReadAsStringAsync().Result;
            }



            // 前後的[]要刪除
            jstring = jstring.Remove(0, 1);
            jstring = jstring.Remove(jstring.Length - 1, 1);

            JObject jobject = JObject.Parse(jstring);



        } // Detect()



        
        private async void Show_Pictures( string personName)
        {
            JArray result = await List();
            

            // 根據 personName 尋找 personId
            if ((JObject)result.SelectToken("$.[?(@.name=='" + personName + "')]") != null)
            {
                JObject target = (JObject)result.SelectToken("$.[?(@.name=='" + personName + "')]");

                // 尋找該person的每個face的id

                JArray faces = JArray.Parse(target["persistedFaceIds"].ToString());



                // 把照片新增進去
                List<Pictures_Url> pictures_Url_List = new List<Pictures_Url>();


                // 把每個url都列出來
                string url = "";
                for( int i = 0; i < faces.Count; i++)
                {
                    url = await Get_Face_Url(target["personId"].ToString(), faces[i].ToString());


                    BitmapImage bi = new BitmapImage();
                    // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                    bi.BeginInit();
                    bi.UriSource = new Uri(@url, UriKind.RelativeOrAbsolute);
                    bi.DecodePixelHeight = 100; // 調整圖片高度
                    bi.EndInit();

                    Pictures_Url new_pictures_Url = new Pictures_Url();
                    new_pictures_Url.Url = bi;

                    pictures_Url_List.Add(new_pictures_Url);

                    

                } // for()


                lb_Pictures_Grid.ItemsSource = pictures_Url_List;

            } // if()
            else
            {

            } // else


        } // Show_Pictures()

        // 把group裡面的所有人都放出來，並放到畫面上的button中
        private async void Show_All_People()
        {
            JArray result = await List();

            

            for ( int i = 0; i < result.Count; i++)
            {
                Person new_person = new Person();

                new_person.Name = result[i]["name"].ToString();

                person_List.Add(new_person);

            } // for()

            lb_Pictures_Person.ItemsSource = person_List;


        } // Show_All_People()


        private async Task<string> Get_Face_Url( string personId, string faceId )
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "9afa2810150f48d999afac58761d4773");

            var uri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/persongroups/111_group_111522109/persons/" + personId + "/persistedFaces/" + faceId + "?" + queryString;

            var response = await client.GetAsync(uri);

            string jstring = response.Content.ReadAsStringAsync().Result;

            JObject result = JObject.Parse(jstring);

            return result["userData"].ToString();

        } // Get_Face_Url()


        // 查看目前訓練狀況
        private async Task<JObject> Get_Train_Status()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "9afa2810150f48d999afac58761d4773");

            var uri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/persongroups/111_group_111522109/training?" + queryString;

            var response = await client.GetAsync(uri);

            string jstring = response.Content.ReadAsStringAsync().Result;

            JObject result = JObject.Parse(jstring);

            return result;

        } // Get_Train_Status()

        private async void Add_Image( string personName )
        {
            JArray result = await List();

            // 根據 personName 尋找 personId，並新增照片進去
            if ((JObject)result.SelectToken("$.[?(@.name=='" + personName + "')]") != null)
            {
                JObject target = (JObject)result.SelectToken("$.[?(@.name=='" + personName + "')]");



                Add_Face(target["personId"].ToString());

            } // if()
            else
            {

            } // else


            result = await List();
        }


        private async void Delete_Person( string personName )
        {
            JArray result = await List();

            JObject target = (JObject)result.SelectToken("$.[?(@.name=='" + personName + "')]");

            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "9afa2810150f48d999afac58761d4773");

            var uri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/persongroups/111_group_111522109/persons/" + target["personId"].ToString() + "?" + queryString;

            var response = await client.DeleteAsync(uri);

        } // Delete_Person()


        private async void Button_Click_Add_Image(object sender, RoutedEventArgs e)
        {
            string personName = person_List[lb_Pictures_Person.SelectedIndex].Name;

            Add_Image(personName);


        } // Button_Click_Add_Image()


        private async void Button_Click_Train(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "9afa2810150f48d999afac58761d4773");

            var uri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/persongroups/111_group_111522109/train?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{body}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }

            JObject result = await Get_Train_Status();





        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string nowPerson = lb_Pictures_Person.SelectedIndex.ToString();

            nowPerson = person_List[lb_Pictures_Person.SelectedIndex].Name;


            Show_Pictures(nowPerson);

        }
    }

    public class Pictures_Url
    {
        public BitmapImage Url { get; set; }
    }
    
    public class Person
    {
        public string Name { get; set; }

    }
}
