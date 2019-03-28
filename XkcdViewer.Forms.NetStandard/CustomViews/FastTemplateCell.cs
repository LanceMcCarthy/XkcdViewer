using System;
using FFImageLoading.Forms;
using Telerik.XamarinForms.DataControls.ListView;
using Xamarin.Forms;
using XkcdViewer.Forms.NetStandard.Models;

namespace XkcdViewer.Forms.NetStandard.CustomViews
{
    public class FastTemplateCell : ListViewTemplateCell
    {
        public FastTemplateCell()
        {
        }

        protected override void OnBindingContextChanged()
        {
            try
            {
                if (BindingContext is Comic comic)
                {
                    var cachedImage = this.View.FindByName<CachedImage>("MyImage");
                    cachedImage.Source = null;

                    if (comic.Title == "Garden" || string.IsNullOrEmpty(comic.Img))
                    {
                        cachedImage.Source = ImageSource.FromFile("garden_256.png");
                    }
                    else
                    {
                        cachedImage.Source = ImageSource.FromUri(new Uri(comic.Img));
                    }
                }
            }
            finally
            {
                base.OnBindingContextChanged();
            }
        }
    }
}