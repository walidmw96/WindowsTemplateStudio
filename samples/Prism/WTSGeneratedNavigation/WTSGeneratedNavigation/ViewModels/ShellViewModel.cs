﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using WTSGeneratedNavigation.Helpers;
using WTSGeneratedNavigation.Views;

namespace WTSGeneratedNavigation.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private const string PanoramicStateName = "PanoramicState";
        private const string WideStateName = "WideState";
        private const string NarrowStateName = "NarrowState";
        private const double WideStateMinWindowWidth = 640;
        private const double PanoramicStateMinWindowWidth = 1024;
        private readonly INavigationService navigationService;

        public ShellViewModel(INavigationService navigationServiceInstance)
        {
            navigationService = navigationServiceInstance;

            OpenPaneCommand = new DelegateCommand(() => IsPaneOpen = !isPaneOpen);
            ItemSelectedCommand = new DelegateCommand<ItemClickEventArgs>(ItemSelected);
            StateChangedCommand = new DelegateCommand<VisualStateChangedEventArgs>(args => GoToState(args.NewState.Name));
        }

        private bool isPaneOpen;

        public bool IsPaneOpen
        {
            get { return isPaneOpen; }
            set { SetProperty(ref isPaneOpen, value); }
        }

        private SplitViewDisplayMode displayMode = SplitViewDisplayMode.CompactInline;

        public SplitViewDisplayMode DisplayMode
        {
            get { return displayMode; }
            set { SetProperty(ref displayMode, value); }
        }

        private object _lastSelectedItem;

        private ObservableCollection<ShellNavigationItem> primaryItems = new ObservableCollection<ShellNavigationItem>();

        public ObservableCollection<ShellNavigationItem> PrimaryItems
        {
            get { return primaryItems; }
            set { SetProperty(ref primaryItems, value); }
        }

        private ObservableCollection<ShellNavigationItem> secondaryItems = new ObservableCollection<ShellNavigationItem>();

        public ObservableCollection<ShellNavigationItem> SecondaryItems
        {
            get { return secondaryItems; }
            set { SetProperty(ref secondaryItems, value); }
        }

        public ICommand OpenPaneCommand { get; }

        public ICommand ItemSelectedCommand { get; }

        public ICommand StateChangedCommand { get; }

        private void GoToState(string stateName)
        {
            switch (stateName)
            {
                case PanoramicStateName:
                    DisplayMode = SplitViewDisplayMode.CompactInline;
                    break;
                case WideStateName:
                    DisplayMode = SplitViewDisplayMode.CompactInline;
                    IsPaneOpen = false;
                    break;
                case NarrowStateName:
                    DisplayMode = SplitViewDisplayMode.Overlay;
                    IsPaneOpen = false;
                    break;
                default:
                    break;
            }
        }

        public void Initialize(Frame frame)
        {
            frame.Navigated += Frame_Navigated;
            PopulateNavItems();
            InitializeState(Window.Current.Bounds.Width);
        }

        private void InitializeState(double windowWith)
        {
            if (windowWith < WideStateMinWindowWidth)
            {
                GoToState(NarrowStateName);
            }
            else if (windowWith < PanoramicStateMinWindowWidth)
            {
                GoToState(WideStateName);
            }
            else
            {
                GoToState(PanoramicStateName);
            }
        }

        private void PopulateNavItems()
        {
            primaryItems.Clear();
            secondaryItems.Clear();

            // TODO WTS: Change the symbols for each item as appropriate for your app
            // More on Segoe UI Symbol icons: https://docs.microsoft.com/windows/uwp/style/segoe-ui-symbol-font
            // Or to use an IconElement instead of a Symbol see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/projectTypes/navigationpane.md
            // Edit String/en-US/Resources.resw: Add a menu item title for each page
            PrimaryItems.Add(new ShellNavigationItem("Shell_Main".GetLocalized(), Symbol.Document, "Main"));
            PrimaryItems.Add(new ShellNavigationItem("Shell_WebView".GetLocalized(), Symbol.Document, "WebView"));
            PrimaryItems.Add(new ShellNavigationItem("Shell_MediaPlayer".GetLocalized(), Symbol.Document, "MediaPlayer"));
            PrimaryItems.Add(new ShellNavigationItem("Shell_MasterDetail".GetLocalized(), Symbol.Document, "MasterDetail"));
            PrimaryItems.Add(new ShellNavigationItem("Shell_Grid".GetLocalized(), Symbol.Document, "Grid"));
            PrimaryItems.Add(new ShellNavigationItem("Shell_Chart".GetLocalized(), Symbol.Document, "Chart"));
            PrimaryItems.Add(new ShellNavigationItem("Shell_Tabbed".GetLocalized(), Symbol.Document, "Tabbed"));
            PrimaryItems.Add(new ShellNavigationItem("Shell_Map".GetLocalized(), Symbol.Document, "Map"));
            PrimaryItems.Add(new ShellNavigationItem("Shell_Camera".GetLocalized(), Symbol.Document, "Camera"));
            PrimaryItems.Add(new ShellNavigationItem("Shell_ImageGallery".GetLocalized(), Symbol.Document, "ImageGallery"));
            SecondaryItems.Add(new ShellNavigationItem("Shell_Settings".GetLocalized(), Symbol.Document, "Settings"));
        }

        private void ItemSelected(ItemClickEventArgs args)
        {
            if (DisplayMode == SplitViewDisplayMode.CompactOverlay || DisplayMode == SplitViewDisplayMode.Overlay)
            {
                IsPaneOpen = false;
            }

            Navigate(args.ClickedItem);
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e != null)
            {
                var vm = e.SourcePageType.ToString().Split('.').Last().Replace("Page", string.Empty);
                var navigationItem = PrimaryItems?.FirstOrDefault(i => i.PageIdentifier == vm);
                if (navigationItem == null)
                {
                    navigationItem = SecondaryItems?.FirstOrDefault(i => i.PageIdentifier == vm);
                }

                if (navigationItem != null)
                {
                    ChangeSelected(_lastSelectedItem, navigationItem);
                    _lastSelectedItem = navigationItem;
                }
            }
        }

        private void ChangeSelected(object oldValue, object newValue)
        {
            if (oldValue != null)
            {
                (oldValue as ShellNavigationItem).IsSelected = false;
            }

            if (newValue != null)
            {
                (newValue as ShellNavigationItem).IsSelected = true;
            }
        }

        private void Navigate(object item)
        {
            var navigationItem = item as ShellNavigationItem;
            if (navigationItem != null)
            {
                navigationService.Navigate(navigationItem.PageIdentifier, null);
            }
        }
    }
}