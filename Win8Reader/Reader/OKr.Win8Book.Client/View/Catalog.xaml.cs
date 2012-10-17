﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OKr.Win8Book.Client.Common;
using OKr.Win8Book.Client.Controls;
using OKr.Win8Book.Client.Core.Context;
using OKr.Win8Book.Client.Core.Data;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using OKr.Win8Book.Client.ViewModel;

namespace OKr.Win8Book.Client.View
{
    public sealed partial class Catalog : OKrPageBase
    {
        #region Properties

        HomeViewModel viewModel
        {
            get
            {
                return App.HomeViewModel;
            }
        }

        #endregion

        #region Ctor

        public Catalog()
        {
            this.InitializeComponent();
            this.TopAppBar = new NavBar(this, true, false, true);
            this.DataContext = viewModel;
        }

        #endregion

        #region Lifecycle

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            LoadTheme();
        }

        private void OnChapterItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(Viewer), e.ClickedItem as Chapter);
        }

        #endregion

        #region App Bar

        private void AppBarThemeButton_Click(object sender, EventArgs e)
        {
            SwitchTheme();
            HideAppBars();
        }

        #endregion


    }
}
