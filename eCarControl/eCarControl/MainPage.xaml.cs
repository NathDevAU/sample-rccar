using eCarControl.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Core;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using SharpDX.XInput;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using System.Diagnostics;

using Windows.System.Threading;
using Windows.UI.Core;

namespace eCarControl
{

    
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private Controller rcControl;
        //holds udp socket
        private DatagramSocket udpSocket;
        private DataWriter udpWriter;
        //sets control input type
        private bool XBOX_MODE;
        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        
        public static BitmapImage ImageFromRelativePath(FrameworkElement parent, string path)
        {
            var uri = new Uri(parent.BaseUri, path);
            BitmapImage bmp = new BitmapImage();
            bmp.UriSource = uri;
            return bmp;
        }


        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;


            connect();

            XBOX_MODE = false;

            Window.Current.CoreWindow.KeyDown += keyDownHandler;
            Window.Current.CoreWindow.KeyUp += keyUpHandler;

            rcControl = new Controller(0);

            TimeSpan period = TimeSpan.FromMilliseconds(100);

            //Periodic polling of the controller state
            ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer((source) =>
            {

                if (!XBOX_MODE) return;
                State s = rcControl.GetState();

                // 
                // Update the UI thread by using the UI core dispatcher.
                // 
                Dispatcher.RunAsync(CoreDispatcherPriority.High,
                    () =>
                    {
                        /**
                         * Checks the status of the dpad and left thumbstick and uses that to send commands 
                         * 
                         **/

                        if((s.Gamepad.Buttons & GamepadButtonFlags.DPadUp) > 0 || s.Gamepad.LeftThumbY > 10000)
                        {
                            send_to_board(1); //forward
                            UP_IMAGE.Source = (BitmapSource)Resources["UP_ACTIVE"];
                        }
                        else
                        {
                           send_to_board(5); //stop forward
                           UP_IMAGE.Source = (BitmapSource)Resources["UP_INACTIVE"];
                        }

                        if((s.Gamepad.Buttons & GamepadButtonFlags.DPadDown) > 0 || s.Gamepad.LeftThumbY < -10000)
                        {
                            send_to_board(3); //back
                            DOWN_IMAGE.Source = (BitmapSource)Resources["DOWN_ACTIVE"];
                        }
                        else
                        {
                            send_to_board(7); //stop down
                            DOWN_IMAGE.Source = (BitmapSource)Resources["DOWN_INACTIVE"];
                        }

                        if((s.Gamepad.Buttons & GamepadButtonFlags.DPadRight) > 0 || s.Gamepad.LeftThumbX > 10000){
                            send_to_board(2); //right
                            RIGHT_IMAGE.Source = (BitmapSource)Resources["RIGHT_ACTIVE"];
                        }
                        else
                        {
                            send_to_board(6); //stop right
                            RIGHT_IMAGE.Source = (BitmapSource)Resources["RIGHT_INACTIVE"];
                        }

                        if ((s.Gamepad.Buttons & GamepadButtonFlags.DPadLeft) > 0 || s.Gamepad.LeftThumbX < -10000)
                        {
                            send_to_board(4); //left
                            LEFT_IMAGE.Source = (BitmapSource)Resources["LEFT_ACTIVE"];
                        }
                        else
                        {
                            send_to_board(8); //stop left
                            LEFT_IMAGE.Source = (BitmapSource)Resources["LEFT_INACTIVE"];
                        }
                               

                    });

            }, period);

               
            
        
        }

        async void connect()
        {
            if (udpSocket == null) udpSocket = new DatagramSocket(); 
            await udpSocket.BindServiceNameAsync("10001");
            HostName remoteHost = new HostName("HOSTNAME");
            await udpSocket.ConnectAsync(remoteHost, "10001");
            udpWriter = new DataWriter(udpSocket.OutputStream);
        }


        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void pageTitle_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        async private void send_to_board(int message)
        {       
            udpWriter.WriteString(message.ToString());
            await udpWriter.StoreAsync();

        }

        private void keyDownHandler(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {
            if (XBOX_MODE) return;
            switch (e.VirtualKey.ToString())
            {
                case "W":
                case "Up":
                    send_to_board(1); //forward
                    UP_IMAGE.Source = (BitmapSource) Resources["UP_ACTIVE"];
                    break;
                case "D":
                case "Right":
                    send_to_board(2); //left
                    RIGHT_IMAGE.Source = (BitmapSource) Resources["RIGHT_ACTIVE"];;
                    break;
                case "S":
                case "Down":
                    send_to_board(3); //back
                    DOWN_IMAGE.Source = (BitmapSource)Resources["DOWN_ACTIVE"];
                    break;
                case "A":
                case "Left":
                    send_to_board(4); //left
                    LEFT_IMAGE.Source = (BitmapSource) Resources["LEFT_ACTIVE"];;
                    break;
                default:
                    break;
            }
        }

        private void keyUpHandler(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {
            if (XBOX_MODE) return;
            switch (e.VirtualKey.ToString())
            {
                case "W":
                case "Up":
                    UP_IMAGE.Source = (BitmapSource) Resources["UP_INACTIVE"];;
                    send_to_board(5); //stop left
                    break;
                case "D":
                case "Right":
                    RIGHT_IMAGE.Source = (BitmapSource) Resources["RIGHT_INACTIVE"];;
                    send_to_board(6); //stop left
                    break;
                case "S":
                case "Down":
                    DOWN_IMAGE.Source = (BitmapSource) Resources["DOWN_INACTIVE"];;
                    send_to_board(7); //stop left
                    break;
                case "A":
                case "Left":
                    LEFT_IMAGE.Source = (BitmapSource) Resources["LEFT_INACTIVE"];;
                    send_to_board(8); //stop left
                    break;
                default:
                    break;
            }
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (CONTROL_SWITCH.IsOn)
            {
                XBOX_MODE = true;
            }
            else
            {
                XBOX_MODE = false;
            };

            if (sender.Equals(NETWORK_SWITCH))
            {
                send_to_board(9);
            }
        }


    }
}
