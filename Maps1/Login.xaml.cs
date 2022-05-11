using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
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

namespace Maps1
{
    /// <summary>
    /// Logika interakcji dla klasy Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        private readonly EmailAddressAttribute emailValidator = new EmailAddressAttribute();
        public Page1()
        {
            InitializeComponent();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = usernameBox.Text;
                string password = passwordBox.Password;
                if (String.IsNullOrEmpty(username) || !emailValidator.IsValid(username))
                {
                    errMessage.Text = "Wprowadź poprawny email.";
                    return;
                }
                if (String.IsNullOrEmpty(password))
                {
                    errMessage.Text = "Wprowadź poprawne hasło.";
                    return;
                }
                LoginDTO loginDTO = new LoginDTO(username, password);
                string jsonBody = JsonConvert.SerializeObject(loginDTO);
                WebClient webClient = new WebClient();
                byte[] resp = webClient.UploadData(ServerState.SERVER_URL + "/login", Encoding.ASCII.GetBytes(jsonBody));
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(Encoding.ASCII.GetString(resp));
                ServerState.auth_token = loginResponse.accessToken;
                Page2 page = new Page2();
                NavigationService.Navigate(page);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        int code = (int)response.StatusCode;
                        errMessage.Text = code == 403 ? "Nieprawidołowa nazwa użytkownika lub hasło." : "Coś poszło nie tak, spróbuj ponownie potem.";
                    }
                }
                else
                {
                    errMessage.Text = "Coś poszło nie tak, spróbuj ponownie potem.";
                }
            }

        }

        partial class LoginDTO
        {
            public LoginDTO(string username, string password)
            {
                this.username = username;
                this.password = password;
            }

            public string username { get; set; }
            public string password { get; set; }
        }
        partial class LoginResponse
        {
            // in JWT RSA format
            public string accessToken { get; set; }
            public string refreshToken { get; set; }
        }

        private void registerNow_Click(object sender, RoutedEventArgs e)
        {
            Page3 page = new Page3();
            NavigationService.Navigate(page);
        }
    }
}

