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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            // List_Person_Group();

            //
            // _Person_Group();

            // Create_Person_Group();


        }


        // make person group
        private async void Create_Person_Group()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            string result;

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "9afa2810150f48d999afac58761d4773");
            var uri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/persongroups/111_group_111522109?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{'name':'Wesley', 'userData':'Wesley_UserData', 'recognitionModel':'recognition_01'}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PutAsync(uri, content);

                result = response.Content.ReadAsStringAsync().Result;
            }

            Console.WriteLine("PersonGroup Msg: " + result);

        } // Create_Person_Group()

        private async void Delete_Person_Group()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            string result;

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "9afa2810150f48d999afac58761d4773");

            var uri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/persongroups/111_group_111522109?" + queryString;

            var response = await client.DeleteAsync(uri);

            result = response.Content.ReadAsStringAsync().Result;


        } // Delete_Person_Group()

        private async void List_Person_Group()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            string result;

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "9afa2810150f48d999afac58761d4773");

            // Request parameters
            queryString["start"] = "";
            queryString["top"] = "1000";
            queryString["returnRecognitionModel"] = "false";
            var uri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/persongroups?" + queryString;

            var response = await client.GetAsync(uri);

            result = response.Content.ReadAsStringAsync().Result;


        } // List_Person_Group()



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



            return jArray;

        } // List()

        // 根據url去detect人物資訊
        private async Task<JObject> Detect( string url )
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            string jstring;

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "9afa2810150f48d999afac58761d4773");

            // Request parameters
            queryString["returnFaceAttributes"] = "age,gender";


            var uri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/detect?" + queryString;


            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{'url':'" + url + "'}");

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


            return jobject;

           

        } // Detect()

        // Made new person in person group
        private void Button_Click_Create_Person(object sender, RoutedEventArgs e)
        {
            // Display new window
            Create_Person window = new Create_Person();
            window.ShowDialog();

        } // Button_Click_Create_Person()



        private void Button_Click_Preview(object sender, RoutedEventArgs e)
        {
            BitmapImage bi = new BitmapImage();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            bi.BeginInit();
            bi.UriSource = new Uri(@tb_Url.Text.ToString(), UriKind.RelativeOrAbsolute);
            bi.DecodePixelHeight = 200; // 調整圖片高度
            bi.EndInit();

            Img_Picture.Source = bi;

        } // Button_Click_Preview()


        private async void Button_Click_Identify(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            string result;
            JArray person_list = await List();


            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "9afa2810150f48d999afac58761d4773");

            var uri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/identify?" + queryString;

            JObject face_Object = await Detect(tb_Url.Text);

            HttpResponseMessage response;


            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{'faceIds':['" + face_Object["faceId"].ToString() + "'], 'personGroupId':'111_group_111522109'}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }

            result = response.Content.ReadAsStringAsync().Result;





            // Name and Confidence Process
            // 前後的[]要刪除
            result = result.Remove(0, 1);
            result = result.Remove(result.Length - 1, 1);
            JObject result_Jobj = JObject.Parse(result);


            result = result_Jobj["candidates"].ToString();

            // 前後的[]要刪除
            result = result.Remove(0, 1);
            result = result.Remove(result.Length - 1, 1);

            result_Jobj = JObject.Parse(result);


            // Gender and Age Process
            face_Object = await Detect(tb_Url.Text);


            // Name and Confidence Output
            tb_Confidence.Text = result_Jobj["confidence"].ToString();

            JObject target = (JObject)person_list.SelectToken("$.[?(@.personId=='" + result_Jobj["personId"].ToString() + "')]");

            tb_Name.Text = target["name"].ToString();


            // Gender and Age Output
            tb_Age.Text = face_Object["faceAttributes"]["age"].ToString();

            tb_Gender.Text = face_Object["faceAttributes"]["gender"].ToString();



        } // Button_Click_Identify()

        // 跳轉頁面
        private void Button_Click_Show_Pictures(object sender, RoutedEventArgs e)
        {
            // Display new window
            TrainPicture window = new TrainPicture();
            window.ShowDialog();
        }
    }

}
