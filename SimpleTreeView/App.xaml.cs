﻿using System;
using System.Windows;

namespace SimpleTreeView
{
    public partial class App : Application
    {
        public App()
        {
            this.Activated += StartElmish;
        }

        private void StartElmish(object sender, EventArgs e)
        {
            this.Activated -= StartElmish;
            Core.main(MainWindow);
        }
    }
}
