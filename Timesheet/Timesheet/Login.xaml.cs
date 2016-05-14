using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Timesheet
{
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();

            if (Application.Current.Properties.ContainsKey(Constants.SPOSITEURL))
            {
                LbSiteUrl.Text = Application.Current.Properties[Constants.SPOSITEURL].ToString();
                LbSiteUrl.IsVisible = true;
                BtModify.IsVisible = true;
                EntrySiteUrl.IsVisible = false;

            }
            else
            {
                EntrySiteUrl.IsVisible = true;
                LbSiteUrl.IsVisible = false;
                BtModify.IsVisible = false;

            }
        }

        protected async void BtLogin_Clicked(object sender, EventArgs e)
        {
			try 
			{
				if ((!string.IsNullOrWhiteSpace(LbSiteUrl.Text) || !string.IsNullOrWhiteSpace(EntrySiteUrl.Text)) && !string.IsNullOrWhiteSpace(EntryUserName.Text) && !string.IsNullOrWhiteSpace(EntryPassword.Text))
				{
					if (!string.IsNullOrWhiteSpace(EntrySiteUrl.Text))
					{
						Application.Current.Properties.Remove(Constants.SPOSITEURL);
						Application.Current.Properties.Add(Constants.SPOSITEURL, EntrySiteUrl.Text);
					}
					if (await Authentication.Authenticate(EntryUserName.Text, EntryPassword.Text))
					{
						Application.Current.MainPage = new Home();
					}
					else
					{
						LbMessage.Text = "login failed";
						LbMessage.IsVisible = true;
					}
				}
			}
			catch(Exception ex) 
			{
				LbMessage.Text = ex.Message;
				LbMessage.IsVisible = true;
			}
        }

        protected void BtModify_Clicked(object sender, EventArgs e)
        {
            EntrySiteUrl.Text = LbSiteUrl.Text;
            LbSiteUrl.IsVisible = false;
            BtModify.IsVisible = false;
        }
    }
}
