/*****************************************************************************
​* Copyright​ ​(C)​ ​2023 ​by​ ​ CU Boulder
​*
​* ​​Redistribution,​ ​modification​ ​or​ ​use​ ​of​ ​this​ ​software​ ​in​ ​source​ ​or​ ​binary
​* ​​forms​ ​is​ ​permitted​ ​as​ ​long​ ​as​ ​the​ ​files​ ​maintain​ ​this​ ​copyright.​ ​Users​ ​are
​​* ​permitted​ ​to​ ​modify​ ​this​ ​and​ ​use​ ​it​ ​to​ ​learn​ ​about​ ​the​ ​field​ ​of​ ​embedded
​* software.​ Nileshkartik Ashokkumar ​and​ ​the​ ​University​ ​of​ ​Colorado​ ​are​ ​not​ ​liable​ ​for
​​* ​any​ ​misuse​ ​of​ ​this​ ​material.
​*
*****************************************************************************
​​*​ ​@file​ MainPage.xaml.cs
​​*​ ​@brief ​ functionality of the contact Manager to save and delete contact information along with GPIO LED interface
​​*
​​*​ ​@Author: Alexander Bork, Malola Simman, Nileshkartik Ashokkumar, Vignesh Vadivel
​​*​ ​@date​ ​Nov ​23​ ​2023
​*​ ​@version​ ​1.0
​*
*********************************************************************************/
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
using Windows.Devices.Gpio;
using System.Threading.Tasks;
using System.Collections.ObjectModel;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ContactManager
{
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Contact> Contacts { get; set; } // Collection of contacts to store the contact information

        private const int GREENLED = 23;            // GPIO Pin for saving contact indicator
        private const int REDLED = 18;              // GPIO Pin for Deleting contact indicator

        private GpioPin _pin1;                      // GPIO pin for saving 
        private GpioPin _pin2;                      // GPIO pin for deletion

        public MainPage()
        {
            this.InitializeComponent();
            Contacts = new ObservableCollection<Contact>();         // create a contacts Collection
            InitializeGPIO();                           // Initialize the 2GPIO pin to low 
        }

        private async void AddContact_Click(object sender, RoutedEventArgs e)
        {
            string name = nameTextBox.Text;                         //Get the name from the text box
            string phone = phoneTextBox.Text;                       // Get the Contact from the text box

            //Check if both name and phone are not empty
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(phone))
            {
                Contacts.Add(new Contact { Name = name, Phone = phone });
                nameTextBox.Text = "";                              //empty the nametext box 
                phoneTextBox.Text = "";                             //empty the phone text box

                //Update the contact to the list 
                UpdateAddedContactsList();
                _pin1.Write(GpioPinValue.High);                     // set the green led on 
                await Task.Delay(1000);
                _pin1.Write(GpioPinValue.Low);                      // clear the green and red led
                _pin2.Write(GpioPinValue.Low);
                await Task.Delay(1000);
            }
        }

        private async void DeleteContact_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Contact contact)
            {
                Contacts.Remove(contact);                           //remove the contact from list
                UpdateAddedContactsList();
                _pin2.Write(GpioPinValue.High);                     // set the red led on 
                await Task.Delay(1000);
                _pin2.Write(GpioPinValue.Low);                     // clear the green led and red led
                _pin1.Write(GpioPinValue.Low);
                await Task.Delay(1000);
            }
        }

        private void UpdateAddedContactsList()
        {
            //store the contact information in the text block 
            addedContactsListTextBlock.Text = string.Join("\n", Contacts.Select(contact => $"{contact.Name}: {contact.Phone}"));
        }


        private void InitializeGPIO()
        {
            // initilize the GPIO pins for the LED
            var controller = GpioController.GetDefault();
            if (controller != null)
            {
                _pin1 = controller.OpenPin(GREENLED);
                _pin1.SetDriveMode(GpioPinDriveMode.Output);
                _pin2 = controller.OpenPin(REDLED);
                _pin2.SetDriveMode(GpioPinDriveMode.Output);
                _pin1.Write(GpioPinValue.Low);
                _pin2.Write(GpioPinValue.Low);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Taget Device Doesnt have a GPIO pin");
            }
        }
    }

    public class Contact
    {
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}
