﻿using Jordans_Podman_Tool.Model;
using Jordans_Podman_Tool.Podman;
using Jordans_Podman_Tool.Settings;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace Jordans_Podman_Tool.ViewModel
{
    public class ImageViewModel : ViewModelBase
    {
        #region Private Properties
        private ObservableCollection<Image> images;
        private DispatcherTimer ImageTimer;
        private IAppSettings settings;
        private IPodman podman;
        #endregion
        #region Public Properties
        public ObservableCollection<Image> Images
        {
            get => images;
            set => images = value;
        }
        public IAppSettings Settings
        {
            get => settings;
            set => settings = value;
        }
        public IPodman Podman
        {
            get => podman;
            set => podman = value;
        }
        #endregion
        #region Public Methods
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ImageViewModel(IAppSettings settings, IPodman podman)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Settings = settings;
            Podman = podman;
            images = new ObservableCollection<Image>();

            PopulateImages();
            ImageTimer = new DispatcherTimer();
            ImageTimer.Interval = TimeSpan.FromSeconds(10);
            ImageTimer.Tick += OnImageEvent;
            ImageTimer.Start();
        }
        #endregion
        #region Private Methods
        private void OnImageEvent(object? send, EventArgs e)
        {
            PopulateImages();
        }
        private void PopulateImages()
        {
            string command = "podman image list";
            if (Podman.Run(command, out string output))
            {
                Images.Clear();
                output = output.Substring(output.IndexOf(command) + command.Length + 2);
                output = output.Substring(output.IndexOf("\n") + 1);
                if (output.IndexOf("\n\r\n") > -1)
                {
                    output = output.Substring(0, output.IndexOf("\n\r\n"));
                    string[] lines = output.Split("\n");
                    foreach (string line in lines)
                    {
                        string[] split = System.Text.RegularExpressions.Regex.Split(line, @"\s{2,}");
                        Images.Add(new Image(split[0], split[1], split[2], split[3], split[4]));
                    }
                }
            }
        }
        #endregion
    }
}
