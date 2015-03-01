//==========================================================================
//
// Author:  Nick Landry
// Title:   Senior Technical Evangelist - Microsoft US DX - NY Metro
// Twitter: @ActiveNick
// Blog:    www.AgeofMobility.com
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Disclaimer: Portions of this code may been simplified to demonstrate
// useful application development techniques and enhance readability.
// As such they may not necessarily reflect best practices in enterprise 
// development, and/or may not include all required safeguards.
// 
// This code and information are provided "as is" without warranty of any
// kind, either expressed or implied, including but not limited to the
// implied warranties of merchantability and/or fitness for a particular
// purpose.
//
// To learn more about Universal Windows app development using Cortana
// and the Speech SDK, watch the full-day course for free on
// Microsoft Virtual Acdemy (MVA) at http://aka.ms/cortanamva
//
//==========================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace SpeechRecLoopDemoWP81
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Dictionary<string, string> imageFiles;

        // The object for controlling the speech synthesis engine (voice).
        SpeechSynthesizer synthesizer;
        SpeechRecognizer recognizer;

        // The media object for controlling and playing audio.
        MediaElement mediaplayer;

        bool isNew = true;
        bool recoEnabled = false;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // On first run, set up the dictionary of anomal names and matching photos 
            if (isNew)
            {
                imageFiles = new Dictionary<string, string>();
                imageFiles.Add("boa", "boa.jpg");
                imageFiles.Add("snake", "boa.jpg");
                imageFiles.Add("bunny", "bunny.jpg");
                imageFiles.Add("rabbit", "bunny.jpg");
                imageFiles.Add("canary", "canary.jpg");
                imageFiles.Add("bird", "canary.jpg");
                imageFiles.Add("cat", "cat.jpg");
                imageFiles.Add("dog", "dog.jpg");
                imageFiles.Add("ferret", "ferret.jpg");
                imageFiles.Add("guinea pig", "guineapig.jpg");
                imageFiles.Add("goldfish", "goldfish.jpg");
                imageFiles.Add("fish", "goldfish.jpg");
                imageFiles.Add("hamster", "hamster.jpg");
                imageFiles.Add("iguana", "iguana.jpg");
                imageFiles.Add("kitten", "kitten.jpg");
                imageFiles.Add("lizard", "lizard.jpg");
                imageFiles.Add("mouse", "mouse.jpg");
                imageFiles.Add("mice", "mouse.jpg");
                imageFiles.Add("parrot", "parrot.jpg");
                imageFiles.Add("puppy", "puppy.jpg");
                isNew = false;
            }

            try
            {
                // Create the speech recognizer and speech synthesizer objects. 
                if (this.synthesizer == null)
                {
                    synthesizer = new SpeechSynthesizer();

                    //Retrieve the first female voice
                    synthesizer.Voice = SpeechSynthesizer.AllVoices
                        .First(i => (i.Gender == VoiceGender.Female && i.Description.Contains("United States")));

                    mediaplayer = new MediaElement();
                }
                if (this.recognizer == null)
                {
                    recognizer = new SpeechRecognizer();
                }
                // Set up a list of pet animals to recognize.
                // Add a list constraint to the recognizer.
                string[] animals =  { "boa", "snake", "bunny", "rabbit", "canary", "bird","cat", "dog", "ferret", "guinea pig", "goldfish", "fish", "hamster", "iguana", "kitten", "lizard", "mouse", "mice", "parrot", "puppy" };
                var listConstraint = new Windows.Media.SpeechRecognition.SpeechRecognitionListConstraint(animals, "AnimalPick");
                
                recognizer.Constraints.Add(listConstraint);

                // Compile the constraint.
                await recognizer.CompileConstraintsAsync();
            }
            catch (Exception err)
            {
                txtResult.Text = err.ToString();
            }

        }

        private async void BtnContinuousRecognitionClick(object sender, RoutedEventArgs e)
        {
            // Change the button text. 
            if (this.recoEnabled)
            {
                this.recoEnabled = false;
                this.btnContinuousRecognition.Content = "Start speech recognition";
                txtResult.Text = String.Empty;
                return;
            }
            else
            {
                this.recoEnabled = true;
                this.btnContinuousRecognition.Content = "Cancel speech recognition";
            }

            while (this.recoEnabled)
            {
                try
                {
                    if ((mediaplayer.CurrentState == MediaElementState.Closed) ||
                        (mediaplayer.CurrentState == MediaElementState.Stopped) ||
                        (mediaplayer.CurrentState == MediaElementState.Paused))
                    {
                        // Perform speech recognition.  
                        SpeechRecognitionResult speechRecognitionResult = await recognizer.RecognizeAsync();

                        // Check the confidence level of the speech recognition attempt.
                        if ((speechRecognitionResult.Confidence == SpeechRecognitionConfidence.Low) || 
                            (speechRecognitionResult.Confidence == SpeechRecognitionConfidence.Rejected))
                        {
                            // If the confidence level of the speech recognition attempt is low, 
                            // ask the user to try again.
                            txtResult.Text = "Not sure what you said, please try again.";
                            //ReadText("Not sure what you said, please try again");
                        }
                        else
                        {
                            // Tell the user the photo is changing by updating
                            // the TextBox control and by using text-to-speech (TTS). 
                            string feedback = "";
                            if (speechRecognitionResult.Confidence == SpeechRecognitionConfidence.High)
                            {
                                feedback = "Showing the " + speechRecognitionResult.Text + " photo";
                            }
                            else
                            {
                                // We use different feedback if the confidence is only medium to
                                // inform the user there is a chance of error in accuracy
                                feedback = "I think you want the " + speechRecognitionResult.Text + " photo";
                            }
                            txtResult.Text = feedback;
                            //ReadText(feedback);

                            // Set the photo to the pet matching the user text 
                            var var_assets = await Package.Current.InstalledLocation.GetFolderAsync(@"Assets\Images");
                            var var_file = await var_assets.GetFileAsync(getImageFilePath(speechRecognitionResult.Text));
                            var var_stream = await var_file.OpenAsync(FileAccessMode.Read);
                            BitmapImage bmp = new BitmapImage();
                            bmp.SetSource(var_stream); 
                            photoResult.Source = bmp;
                        }
                    }
                }
                catch (System.Threading.Tasks.TaskCanceledException)
                {
                    // Ignore the cancellation exception of the recoOperation.
                }
                catch (Exception exception)
                {
                    // Handle the speech privacy policy error.
                    const uint HResultPrivacyStatementDeclined = 0x80045509;

                    if ((uint)exception.HResult == HResultPrivacyStatementDeclined)
                    {
                        var messageDialog = new Windows.UI.Popups.MessageDialog(
                            "You must accept the speech privacy policy to continue.", "Speech Exception");
                        messageDialog.ShowAsync().GetResults();
                        this.recoEnabled = false;
                        this.btnContinuousRecognition.Content = "Start speech recognition";
                    }
                    else
                    {
                        txtResult.Text = exception.Message;
                    }
                }
            }
        }

        /// <summary>
        /// Returns a file string that matches the recognized pet string.
        /// </summary>
        /// <param name="reco">The recognized pet string.</param>
        /// <returns>The matching pet image file and path.</returns>
        private string getImageFilePath(string recognizedPet)
        {
            if (imageFiles.ContainsKey(recognizedPet))
                return imageFiles[recognizedPet];

            return "";
        }

        private async void ReadText(string mytext)
        {
            //Reminder: You need to enable the Microphone capabilitiy in Windows Phone projects
            //Reminder: Add this namespace in your using statements
            //using Windows.Media.SpeechSynthesis;

            // Generate the audio stream from plain text.
            SpeechSynthesisStream stream = await synthesizer.SynthesizeTextToStreamAsync(mytext);

            // Send the stream to the media object.
            mediaplayer.SetSource(stream, stream.ContentType);
            mediaplayer.Play();
        }
    }
}
