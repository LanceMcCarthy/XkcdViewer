﻿using System.Diagnostics;
using System.Threading.Tasks;
using CommonHelpers.Common;
using Newtonsoft.Json;
using Plugin.Share;
using Plugin.Share.Abstractions;
using Xamarin.Forms;
using XkcdViewer.Forms.Common;

namespace XkcdViewer.Forms.Models
{
    public class Comic : BindableBase
    {
        private string month;
        private int num;
        private string link;
        private string year;
        private string news;
        private string safeTitle;
        private string transcript;
        private string alt;
        private string img;
        private string title;
        private string day;
        private bool isFavorite;

        public Comic()
        {
            ShareCommand = new Command(async () => { await Share(); });
            ToggleFavoriteCommand = new Command(ToggleIsFavorite);
        }

        [JsonProperty("month")]
        public string Month
        {
            get => month;
            set => SetProperty(ref month, value);
        }

        [JsonProperty("num")]
        public int Num
        {
            get => num;
            set => SetProperty(ref num, value);
        }

        [JsonProperty("link")]
        public string Link
        {
            get => link;
            set => SetProperty(ref link, value);
        }

        [JsonProperty("year")]
        public string Year
        {
            get => year;
            set => SetProperty(ref year, value);
        }

        [JsonProperty("news")]
        public string News
        {
            get => news;
            set => SetProperty(ref news, value);
        }

        [JsonProperty("safe_title")]
        public string SafeTitle
        {
            get => safeTitle;
            set => SetProperty(ref safeTitle, value);
        }

        [JsonProperty("transcript")]
        public string Transcript
        {
            get => transcript;
            set => SetProperty(ref transcript, value);
        }

        [JsonProperty("alt")]
        public string Alt
        {
            get => alt;
            set => SetProperty(ref alt, value);
        }

        [JsonProperty("img")]
        public string Img
        {
            get => img;
            set => SetProperty(ref img, value);
        }

        [JsonProperty("title")]
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        [JsonProperty("day")]
        public string Day
        {
            get => day;
            set => SetProperty(ref day, value);
        }

        // Properties not in the json schema

        public bool IsFavorite
        {
            get => isFavorite;
            set => SetProperty(ref isFavorite, value);
        }

        [JsonIgnore]
        public Command ShareCommand { get; }

        [JsonIgnore]
        public Command ToggleFavoriteCommand { get; }

        public async Task Share()
        {
            Debug.WriteLine($"Share Comic: {Title}");

            if (string.IsNullOrEmpty(Img))
                return;

            await CrossShare.Current.Share(
                new ShareMessage
                {
                    Title = Title ?? "xkcd",
                    Text = Transcript ?? "",
                    Url = Img
                },
                new ShareOptions
                {
                    ChooserTitle = "Share the funny!"
                });
        }

        private void ToggleIsFavorite()
        {
            if (IsFavorite)
            {
                FavoritesManager.Current.RemoveFavorite(this);
            }
            else
            {
                FavoritesManager.Current.AddFavorite(this);
            }
        }
    }
}