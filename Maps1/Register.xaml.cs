using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
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

namespace Maps1
{
    public partial class Page3 : Page
    {
        private readonly EmailAddressAttribute emailValidator = new EmailAddressAttribute();
        public Page3()
        {
            InitializeComponent();
        }

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameBox.Text;
            string applicationName = applicationNameBox.Text;
            string password = passwordBox.Password;
            string repeatPassword = passwordRepeatBox.Password;

            if(String.IsNullOrEmpty(username) || !emailValidator.IsValid(username)) 
            {
                errMessage.Text = "Wprowadź poprawny email.";
                return;
            }
            if (String.IsNullOrEmpty(applicationName))
            {
                errMessage.Text = "Wprowadź poprawną nazwę użytkownika.";
                return;
            }
            if (String.IsNullOrEmpty(password) || password.Length < 8)
            {
                errMessage.Text = "Hasło musi zawierać minimum 8 znaków.";
                return;
            }
            if (String.IsNullOrEmpty(repeatPassword) || !password.Equals(repeatPassword))
            {
                errMessage.Text = "Wprowadzone hasła nie zgadzają się.";
                return;
            }
            RegisterDTO registerDTO = new RegisterDTO(username, applicationName, password);
            string requestBodyString = JsonConvert.SerializeObject(registerDTO);
            byte[] requestBody = Encoding.ASCII.GetBytes(requestBodyString);
            WebClient webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            try 
            {
                webClient.UploadData(ServerState.SERVER_URL + "/register", requestBody);
                errMessage.Text = "Rejestracja przebiegła poprawnie, na twój mail został wysłany link aktywacyjny.";
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        int code = (int)response.StatusCode;
                        errMessage.Text = code == 409 ? "Podany email istnieje już w bazie danych Air Matrix." : "Coś poszło nie tak, spróbuj ponownie potem.";
                    }
                }
                else
                {
                    errMessage.Text = "Coś poszło nie tak, spróbuj ponownie potem.";
                }
            }
        }
        partial class RegisterDTO
        {
            public RegisterDTO(string username, string applicationName, string password)
            {
                this.username = username;
                this.applicationName = applicationName;
                this.password = password;
            }

            public string username { get; set; }
            public string applicationName { get; set; }
            public string password { get; set; }
        }

        private void loginNow_Click(object sender, RoutedEventArgs e)
        {
            Page1 page = new Page1();
            NavigationService.Navigate(page);
        }
    }
}
