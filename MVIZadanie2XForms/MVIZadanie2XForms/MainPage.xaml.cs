using System;
using System.Net.Http;
using Xamarin.Forms;

namespace MVIZadanie2XForms
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void PredictGradeButton_OnClicked(object sender, EventArgs e)
        {
            try
            {
                PredictedGradeLabel.TextColor = Color.Black;
                PredictedGradeLabel.Text = OnlineGradePredictor.GetPredictedGrade(NameEntry.Text, SubjectDifficultyEntry.Text, HoursStudiedEntry.Text) + "";
            }
            catch (Exception ex)
            {
                PredictedGradeLabel.TextColor = Color.Red;
                PredictedGradeLabel.Text = ex.Message;
            }
        }
    }
}
