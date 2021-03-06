﻿using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;

namespace FootballTournament2014
{
    public class GroupMatchView:BaseView
    {
        private GroupMatchesViewModel ViewModel
        {
            get{ return BindingContext as GroupMatchesViewModel; }
        }

        public ObservableCollection<GroupHelper> Groups{ get; set; }
        public GroupMatchView()
        {
            BindingContext = new GroupMatchesViewModel();

            var activity = new ActivityIndicator
            {
                Color = Helpers.Color.Greenish.ToFormsColor(),
                IsEnabled = true
            };
            activity.SetBinding(ActivityIndicator.IsVisibleProperty, "IsBusy");
            activity.SetBinding(ActivityIndicator.IsRunningProperty, "IsBusy");

            this.Groups = new ObservableCollection<GroupHelper>();
            var refresh = new ToolbarItem
            {
                Command = ViewModel.LoadItemsCommand,
                Icon = "refresh.png",
                Name = "refresh",
                Priority = 0
            };
            ToolbarItems.Add(refresh);

            ViewModel.ItemsLoaded += new EventHandler((sender, e) =>
            {
                this.Groups.Clear();
                ViewModel.Result.Select(r => r.MatchDate).Distinct().ToList()
                    .ForEach(r => Groups.Add(new GroupHelper(r)));
                foreach (var g in Groups)
                {
                    foreach (var match in ViewModel.Result.Where(m=> m.MatchDate == g.Date))
                    {
                        g.Add(match);
                    }
                }
            });

            Title = "Group Match Schedule";
            var stack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(0, 0, 0, 8)
            };

            var listView = new ListView
            {
                IsGroupingEnabled = true,
                GroupDisplayBinding = new Binding("Date"),
            };
               

            var viewTemplate = new DataTemplate(typeof(ScoreCell));

            listView.ItemTemplate = viewTemplate;

            listView.ItemsSource = Groups;
            stack.Children.Add(activity);

            stack.Children.Add(listView);

            Content = stack;
        }
            

        protected override void OnAppearing ()
        {
            base.OnAppearing ();
            if (ViewModel == null || !ViewModel.CanLoadMore || ViewModel.IsBusy || ViewModel.Result.Count > 0)
                return;

            ViewModel.LoadItemsCommand.Execute (null);
        }
    }
}

