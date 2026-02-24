using BG_Services;
using BG_WorkFlow;
using BGCommunication;
using BGDAL;
using BGModel;
using BGUserControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static CMW.Common.Utilities.Common;
using static CMW.Common.Utilities.CommonFunc;
using static CMW.Common.Utilities.CommonDeleget;
using BG_Entities;
using CMW.Common.Utilities;

namespace LoginModules
{
    [Export("UserPage", typeof(IUserControlView))]
    [CustomExportMetadata(1, "LoginModules", "登录模块", "ZZW", "1.0.0")]
    public class LoginModule : CommonModules
    {
        LoginModel lm = new LoginModel();
        UserConfigBLL uc = new UserConfigBLL();
        LoginDataBaseDal ldb = new LoginDataBaseDal();
        LicenseDal ld = new LicenseDal();
        /// <summary>
        /// 获取角色的
        /// </summary>
        RoleUserBLL rub;

        List<string> ButtonList = new List<string>();
        List<License> License = new List<License>();
        Grid _MainGrid = new Grid();
        Border _MainBorder = new Border()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(0),
            CornerRadius = new CornerRadius(6, 6, 6, 6),
        };
        PasswordBox tb; PasswordBox tbAuthorCode;
        Border txtUserName;
        Border txtPassWord;
        static CheckBox cbRemerber;
        bool? isVisible = false;
        [ImportingConstructor]
        public LoginModule()
        {
            Loaded += LoginModule_Loaded;
            Unloaded += LoginModule_Unloaded;
            PreviewKeyDown += LoginModule_KeyDown;
            IsVisibleChanged += LoginModule_IsVisibleChanged;
            _MainGrid.Width = 600; _MainGrid.Height = 450;
            if (ConfigServices.GetInstance().localConfigModel.IsRemUser)
            {
                var LoginModel = ldb.GetLoginUserByRememberStatus();
             
                lm.UserName = LoginModel == null?ConfigServices.GetInstance().localConfigModel.Login.LoginUser: LoginModel.LOGIN_USERNAME;
                lm.Password = LoginModel == null ? ConfigServices.GetInstance().localConfigModel.Login.LoginUserPwd : LoginModel.LOGIN_PASSWORD;
            }
            License = ld.QueryLicense();
            lm.License = License.Count > 0 ? License[0].LICENSE_CODE : string.Empty;
        }

        private void LoginModule_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            isVisible = e.NewValue as bool?;
        }

        private void LoginModule_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                BtnLoginSendCommand_PreviewMouseDown(BtnLoginSendCommand, null);
            }
            else if (e.Key == System.Windows.Input.Key.Escape)
            {
                BtnLoginSendCommand_PreviewMouseDown(BtnCancelSendCommand, null); ;
            }
        }

        public override void SetCarVersion(ControlVersion cv)
        {
            rub = new RoleUserBLL(cv);
        }

        public override void Show(Window _OwnerWin)
        {
            CurrentWindow = _OwnerWin;
        }

        public override string GetName()
        {
            return "登录";
        }

        public override bool IsConnectionEquipment()
        {
            return IsConnection;
        }

        public override double GetHeight()
        {
            return 345;
        }

        public override double GetWidth()
        {
            return 561;
        }

        private Grid InitGrid()
        {
            Grid _MainGrid = MakeMainGrid();
            #region 设置标题
            Label dp = InitTitle(_MainGrid);
            #endregion

            #region 内容
            MakeLoginNameLbl(_MainGrid);

            MakeLoginTextBox(_MainGrid);

            MakeScanAddress(_MainGrid);

            MakeTxtPassword(_MainGrid);

            MakeLicenceLbl(_MainGrid);

            MakeTxtLicence(_MainGrid);

            MakeRemember(_MainGrid);

            StackPanel sp = MakeLoginAndCancel();

            _MainGrid.Children.Add(sp);
            #endregion
            return _MainGrid;
        }
        Border BtnLoginSendCommand;
        Border BtnCancelSendCommand;
        private StackPanel MakeLoginAndCancel()
        {
            BtnLoginSendCommand = MakeLoginBorder(UpdateStatusNameAction("Login"));
            BtnLoginSendCommand.Tag = "Login";
            Grid.SetRow(BtnLoginSendCommand, 2);
            Grid.SetColumn(BtnLoginSendCommand, 3);
            BtnLoginSendCommand.PreviewMouseDown -= BtnLoginSendCommand_PreviewMouseDown;
            BtnLoginSendCommand.PreviewMouseDown += BtnLoginSendCommand_PreviewMouseDown;
            BtnLoginSendCommand.Width = 100; BtnLoginSendCommand.Margin = new Thickness(0, 0, 20, 0);

            BtnCancelSendCommand = MakeLoginBorder(UpdateStatusNameAction("Cancel"));//, "diyBtnCarHandCancel"
            BtnCancelSendCommand.Tag = "Cancel";
            //BtnCancelSendCommand.Style = (Style)this.FindResource("diyBtnCarHandCancel");
            Grid.SetRow(BtnCancelSendCommand, 2);
            Grid.SetColumn(BtnCancelSendCommand, 4);
            BtnCancelSendCommand.PreviewMouseDown -= BtnLoginSendCommand_PreviewMouseDown;
            BtnCancelSendCommand.PreviewMouseDown += BtnLoginSendCommand_PreviewMouseDown;
            BtnCancelSendCommand.Width = 100;
            StackPanel sp = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(0,0,0,0)
            };
            sp.Children.Add(BtnLoginSendCommand);
            sp.Children.Add(BtnCancelSendCommand);

            Grid.SetRow(sp, 5);
            Grid.SetColumn(sp, 1);
            Grid.SetColumnSpan(sp, 2);
            return sp;
        }

        private void MakeRemember(Grid _MainGrid)
        {
            cbRemerber = new CheckBox()
            {
                Content = UpdateStatusNameAction("RememberMe"),
                HorizontalAlignment = HorizontalAlignment.Left,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontFamily = new FontFamily("宋体"),
            };
            cbRemerber.IsChecked = ConfigServices.GetInstance().localConfigModel.IsRemUser;
            Grid.SetRow(cbRemerber, 4);
            Grid.SetColumn(cbRemerber, 1);
            _MainGrid.Children.Add(cbRemerber);
        }

        private void MakeTxtPassword(Grid _MainGrid)
        {
            txtPassWord = MakeTextBox("pack://Application:,,,/Properties/密码.png", UpdateStatusNameAction("EnterPassword"), "Password");
            Grid.SetRow(txtPassWord, 2);
            Grid.SetColumn(txtPassWord, 1);
            Grid.SetColumnSpan(txtPassWord, 2);
            _MainGrid.Children.Add(txtPassWord);
        }
        private void MakeTxtLicence(Grid _MainGrid)
        {
            txtPassWord = MakeTextBox("pack://Application:,,,/Properties/密码.png", UpdateStatusNameAction("License"), "License");
            Grid.SetRow(txtPassWord, 3);
            Grid.SetColumn(txtPassWord, 1);
            Grid.SetColumnSpan(txtPassWord, 2);
            _MainGrid.Children.Add(txtPassWord);
        }
        /// <summary>
        /// 授权码名称
        /// </summary>
        /// <param name="_MainGrid"></param>
        private void MakeLicenceLbl(Grid _MainGrid)
        {
            Label lblPlcAddress = new Label()
            {
                Content = UpdateStatusNameAction("License"),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalContentAlignment = VerticalAlignment.Center,
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#515151")
            };
            lblPlcAddress.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Grid.SetRow(lblPlcAddress, 3);
            Grid.SetColumn(lblPlcAddress, 0);
            _MainGrid.Children.Add(lblPlcAddress);
        }
        private void MakeScanAddress(Grid _MainGrid)
        {
            Label lblScanAddress = new Label()
            {
                Content = UpdateStatusNameAction("PassWord"),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalContentAlignment = VerticalAlignment.Center,
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#515151")
            };
            lblScanAddress.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Grid.SetRow(lblScanAddress, 2);
            Grid.SetColumn(lblScanAddress, 0);
            _MainGrid.Children.Add(lblScanAddress);
        }

        private void MakeLoginTextBox(Grid _MainGrid)
        {
            txtUserName = MakeTextBox("pack://Application:,,,/Properties/登录用户.png", UpdateStatusNameAction("EnterUserName"), "User");
            Grid.SetRow(txtUserName, 1);
            Grid.SetColumn(txtUserName, 1);
            Grid.SetColumnSpan(txtUserName, 2);
            _MainGrid.Children.Add(txtUserName);
        }

        private void MakeLoginNameLbl(Grid _MainGrid)
        {
            Label lblPlcAddress = new Label()
            {
                Content = UpdateStatusNameAction("UserName"),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalContentAlignment = VerticalAlignment.Center,
                Style = (Style)this.FindResource("diyLabel"),
                Foreground = StrToBrush("#515151")
            };
            lblPlcAddress.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            Grid.SetRow(lblPlcAddress, 1);
            Grid.SetColumn(lblPlcAddress, 0);
            _MainGrid.Children.Add(lblPlcAddress);
        }

        private static Grid MakeMainGrid()
        {
            Grid _MainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });
            _MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100, GridUnitType.Pixel) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(7, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(7, GridUnitType.Star) });
            _MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(7, GridUnitType.Star) });
            return _MainGrid;
        }

        private Border MakeTextBox(string TextBoxType, string HoldWater, string tbType)
        {
            Border bd = new Border()
            {
                Width = 225,
                Height = 30,
                BorderBrush = StrToBrush("#59B5FA"),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                BorderThickness = new Thickness(1),
                Background = StrToBrush("#FFFFFF")
            };
            StackPanel sp = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Orientation = Orientation.Horizontal,
            };
            Label Image = new Label()
            {
                Width = 25,
                Height = 25,
                Margin = new Thickness(0, 0, 5, 0),
                Background = new ImageBrush(new BitmapImage(new Uri(TextBoxType, UriKind.RelativeOrAbsolute))) { Stretch = Stretch.Uniform },
            };
            sp.Children.Add(Image);
            Border Line = new Border()
            {
                Height = 20,
                Width = 1,
                Background = StrToBrush("#59B5FA"),
                Margin = new Thickness(0, 0, 5, 0),
            };
            sp.Children.Add(Line);

            if (tbType == "User")
            {
                TextBox tb = new TextBox()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Width = 180,
                    Height = 30,
                    Style = (Style)this.TryFindResource("txtUserBox"),
                    TabIndex = 1
                };
                tb.SetBinding(TextBox.TextProperty, new Binding("UserName") { Source = lm, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });
                tb.BorderThickness = new Thickness(0);
                sp.Children.Add(tb);
            }
            else if(tbType == "Password")
            {
                tb = new PasswordBox()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Width = 180,
                    Height = 30,
                    TabIndex = 2
                    //Style = (Style)this.TryFindResource("TxtPwd")
                };
                tb.Password = lm.Password;

                //tb.SetBinding(PasswordBox., new Binding("UserName") { Source = lm, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });
                tb.BorderThickness = new Thickness(0);
                sp.Children.Add(tb);
                tb.SetBinding(PasswordBox.PasswordCharProperty, new Binding("Password") { Source = lm, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });
            }
            else if (tbType == "License")
            {
                tbAuthorCode = new PasswordBox()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Width = 180,
                    Height = 30,
                    TabIndex = 3
                    //Style = (Style)this.TryFindResource("txtLincese")
                };
                //tbAuthorCode.IsEnabled = string.IsNullOrEmpty(lm.License);
                tbAuthorCode.Password = lm.License;
                tbAuthorCode.SetBinding(PasswordBox.PasswordCharProperty, new Binding("License") { Source = lm, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });
                tbAuthorCode.BorderThickness = new Thickness(0);
                sp.Children.Add(tbAuthorCode);
            }
            bd.Child = sp;
            return bd;
        }


        private void BtnLoginSendCommand_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border bd = sender as Border;
            string tag = bd.Tag as string;
            switch (tag)
            {
                case "Login":
                    (bd.Child as Label).Content = UpdateStatusNameAction("Logining");
                    bd.IsEnabled = false;
                    Task.Run(() => {
                        try
                        {
                           
                            if (!CheckInternetAndPort()) {

                                LoginResultAction(LoginStatus.LinkServerFaild, ButtonList);
                                this.Dispatcher.BeginInvoke((Action)delegate ()
                                {
                                    (bd.Child as Label).Content = UpdateStatusNameAction("Login");
                                    bd.IsEnabled = true;
                                });
                                return;
                            }
                            //if (!CheckInternet())
                            //{
                            //    LoginResultAction(LoginStatus.FTPServerFaild, ButtonList);
                            //    this.Dispatcher.BeginInvoke((Action)delegate ()
                            //    {
                            //        (bd.Child as Label).Content = UpdateStatusNameAction("Login");
                            //        bd.IsEnabled = true;
                            //    });
                            //    return;
                            //}
                            Login();
                            GetFTPParamater();
                           
                            Channel();
                           
                        }
                        catch (Exception ex)
                        {
                            if (ex is WebException)
                            {
                                LoginStatus ls = CommonFunc.ConvertSocketErrorToLogginStatus(((ex as WebException).InnerException as SocketException).SocketErrorCode);
                                {
                                    LoginResultAction(ls, null);
                                }
                            }
                            else
                            {
                                LoginResultAction(LoginStatus.Faild, null);
                            }
                        }
                        finally
                        {
                            this.Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                (bd.Child as Label).Content = UpdateStatusNameAction("Login");
                                bd.IsEnabled = true;
                            });
                        }
                    });
                    break;
                case "Cancel":
                    ConfigServices.GetInstance().localConfigModel.Login.LoginStatus = LoginStatus.Cancel;
                    LoginResultAction(LoginStatus.Cancel, null);
                    break;
                default:
                    break;
            }
        }

        private async void Channel()
        {
            try
            {
                var channelNoObject = await rub.GetChannel(ConfigServices.GetInstance().localConfigModel.EquipmentNo);
                ConfigServices.GetInstance().localConfigModel.ChannelNo = channelNoObject.cm?.ChannelId;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ConfigServices.GetInstance().localConfigModel.ChannelNo = string.Empty;
            }
        }
        /// <summary>
        /// 获取FTP账号密码
        /// </summary>
        private void GetFTPParamater()
        {
            try
            {
                RequestModel fTPHTTPData = new RequestModel() { OrderType = UploadImageOrderType.GetClientConfig,Data = CommonFunc.AesEncrypt(CommonFunc.ObjectToJson<NullClass>(new NullClass()), ConfigServices.GetInstance().localConfigModel.IsAES),Token = ConfigServices.GetInstance().localConfigModel.Login.sccessToken };
                
                var RsposeData = UploadWebServiceControl.GetInstance().CreateWebServicesControl(GetFtpParamaterServices.GetInstance()).UploadData(CommonFunc.ObjectToJson<RequestModel>(fTPHTTPData));

                ResposeModel ftpResposeModel = CommonFunc.JsonToObject<ResposeModel>(RsposeData);

                FTPResposeObject fTPResposeObject = CommonFunc.JsonToObject<FTPResposeObject>(CommonFunc.AesDecrypt(ftpResposeModel.Data.ToString(), ConfigServices.GetInstance().localConfigModel.IsAES));

                /*FTP配置*/
                ConfigServices.GetInstance().localConfigModel.ftp_user.FtpPassword = fTPResposeObject.FTP_PASSWORD;
                ConfigServices.GetInstance().localConfigModel.ftp_user.FtpUserName = fTPResposeObject.FTP_USERNAME ;
                ConfigServices.GetInstance().localConfigModel.ftp_user.IpAddr = fTPResposeObject.FTP_IP;
                Common._ftpHelper = new FtpHelper(ConfigServices.GetInstance().localConfigModel.ftp_user.IpAddr, 
                    ConfigServices.GetInstance().localConfigModel.ftp_user.FtpUserName
                    , ConfigServices.GetInstance().localConfigModel.ftp_user.FtpPassword);
                /*FTP配置*/
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ConfigServices.GetInstance().localConfigModel.ChannelNo = string.Empty;
            }
        }

      
        /// <summary>
        /// 检查是否和服务器相同
        /// </summary>
        /// <returns></returns>
        private bool CheckInternet()
        {
            return CommonFunc.PingIp(ConfigServices.GetInstance().localConfigModel.ftp_user.IpAddr);
        }

        private bool CheckInternetAndPort()
        {
            string[] ServerPort = ConfigServices.GetInstance().localConfigModel.Server.Trim(@"http:/".ToCharArray()).Split(':');
            string server = ServerPort[0];
            int Port = Convert.ToInt32(ServerPort[1]);
            return CommonFunc.AddressPort(server,Port);
        }

        private void Login()
        {

            //var text = CommonFunc.AesDecrypt(@"8yk/S466odXxbYz5qLfRa/kEkTEEuJT20HsADeR8AJI=");
            lm.Password = tb.Password;
            lm.License = tbAuthorCode.Password;
            var LoginResult = rub.Login(lm, ref ButtonList);
            if (isVisible != true)
            {
                return;
            }
            ConfigServices.GetInstance().localConfigModel.Login.LoginStatus = LoginResult;
            ConfigServices.GetInstance().localConfigModel.Login.LoginUser = lm.UserName;
            ConfigServices.GetInstance().localConfigModel.Login.LoginUserPwd = lm.Password;
            if (string.IsNullOrEmpty(lm.License)) 
            { 
                LoginResultAction(LoginStatus.AuthorizationCodeNull, ButtonList); 
                return;
            }; 
            LoginResultAction(LoginResult, ButtonList);

            this.Dispatcher.BeginInvoke((Action)delegate () {
                ConfigServices.GetInstance().localConfigModel.IsRemUser = cbRemerber.IsChecked == true;
                UpdateConfigs("IsRemberber", ConfigServices.GetInstance().localConfigModel.IsRemUser.ToString(), Section.SOFT);
                if (ConfigServices.GetInstance().localConfigModel.IsRemUser)
                {
                    UpdateConfigs("User", lm.UserName, Section.SOFT);
                    UpdateConfigs("PassWord", lm.Password, Section.SOFT);
                   
                }
               
                if (LoginResult == LoginStatus.Success)
                {
                    if (License.Count == 0)
                    { 
                        ld.InsertLicense(new License() { LICENSE_CODE = lm.License, LICENSE_EFFECTTIME = DateTime.Now.ToString() });
                    }
                    else
                    {
                        ld.UpdateLicense(new License() { LICENSE_CODE = lm.License, LICENSE_EFFECTTIME = DateTime.Now.ToString() });
                    }

                    ldb.InsertOrUpdateLoginModelObject(
                   new LoginModelObject()
                   {
                       LOGIN_USERNAME = lm.UserName,
                       LOGIN_PASSWORD = lm.Password,
                       LOGIN_UPDATETIME = DateTime.Now.ToString(),
                       LOGIN_STATUS = ConfigServices.GetInstance().localConfigModel.IsRemUser ? "1" : "0"
                   }
                   );
                    uc.AddOrRemove(lm);
                }
            });
        }




        private Border MakeLoginBorder(string lblContent,string _Style = "diyLabel")
        {
            Border BtnSendCommand = new Border()
            {
                Width = 50,
                Height = 30,
                Style = (Style)this.FindResource("diyBtnCarHand"),
            };
            Label btnScanInner = new Label()
            {
                Content = lblContent,// UpdateStatusNameAction("Login"),
                Style = (Style)this.TryFindResource(_Style),
                


                Foreground = StrToBrush("#FFFFFF")
            };
            btnScanInner.FontSize = UpdateFontSizeAction(CMWFontSize.Normal);
            BtnSendCommand.Child = btnScanInner;
            return BtnSendCommand;
        }

        /// <summary>
        /// 设置软件标题
        /// </summary>
        /// <param name="_MainGrid"></param>
        /// <returns></returns>
        private Label InitTitle(Grid _MainGrid)
        {
            Label _lblTitle = new Label()
            {
                Content = UpdateStatusNameAction("UserLogin"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontSize = 18,
                Margin = new Thickness(0, 15, 0, 0),
                FontWeight = FontWeights.Bold,
                Foreground = StrToBrush("#000000"),
                FontFamily = new FontFamily("Microsoft YaHei"),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            _lblTitle.FontSize = UpdateFontSizeAction(CMWFontSize.Big);

            Grid.SetColumn(_lblTitle, 0);
            Grid.SetRow(_lblTitle, 0);
            Grid.SetColumnSpan(_lblTitle, 5);
            _MainGrid.Children.Add(_lblTitle);
            return _lblTitle;
        }

        private void LoginModule_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            BuryingPoint("Exit the login module interface");
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
        }

        private void LoginModule_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            BuryingPoint("Enter the login module to load the module");
            ButtonInvoke.SwitchLanguageEvent -= SwitchLanguage;
            ButtonInvoke.SwitchLanguageEvent += SwitchLanguage;
            ButtonInvoke.SwitchFontSizeEvent -= SwitchFontSize;
            ButtonInvoke.SwitchFontSizeEvent += SwitchFontSize;
            _MainGrid = InitGrid();
            _MainBorder.Child = _MainGrid;
            Content = _MainBorder;
        }

        
        public void SwitchFontSize(string FontSize)
        {
            LoginModule_Loaded(null, null);
        }
        public void SwitchLanguage(string language)
        {
            base.SwitchLanguage();
            LoginModule_Loaded(null, null);
        }

    }
}
