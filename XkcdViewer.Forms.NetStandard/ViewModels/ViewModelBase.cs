using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using XkcdViewer.Forms.NetStandard.Common;

namespace XkcdViewer.Forms.NetStandard.ViewModels
{
    public class ViewModelBase : ObservableObject
    {
        private bool isBusy;
        private string title;
        
        public bool IsBusy
        {
            get => isBusy;
            set => Set(ref isBusy, value);
        }
        
        public string Title
        {
            get => title;
            set => Set(ref title, value);
        }
    }
}
