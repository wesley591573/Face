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
    /// Create_Person.xaml 的互動邏輯
    /// </summary>
    public partial class Create_Person : Window
    {
        public Create_Person()
        {
            InitializeComponent();

            // List();

            // CreatePerson();

            // Detect();



        } // Create_Person()

        // create new person in person group
        private async void CreatePerson( string name, string userData )
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            string result;

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "9afa2810150f48d999afac58761d4773");
            var uri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/persongroups/111_group_111522109/persons?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{'name':'" + name  + "', 'userData':'" + userData + "'}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);

                result = response.Content.ReadAsStringAsync().Result;
            }

            Console.WriteLine("PersonGroup Msg: " + result);


        } // CreatePerson()




        private async void List()
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



        } // List()

        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            Close();

        } // Button_Click_Cancel()

        private void Button_Click_Create(object sender, RoutedEventArgs e)
        {

            CreatePerson( tb_Name.Text.ToString(), tb_Name.Text.ToString() + "_userData");

            Close();

        } // Button_Click_Create()
    }

}
