using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Common.Wpf.Animation
{
    public static class ShowHideAnimation
    {
        private class EntryInfo
        {
            public Storyboard ShowStoryboard;
            public Storyboard HideStoryboard;
            public Image SnapshotHolder;
            public FrameworkElement ContentHolder;
            public EventHandler ShowStoryboardCompletedEventHander;
            public EventHandler HideStoryboardCompletedEventHander;
        }


        private static Dictionary<FrameworkElement, EntryInfo> s_entries;

        static ShowHideAnimation()
        {
            s_entries = new Dictionary<FrameworkElement, EntryInfo>();
        }

        public static Storyboard GetShowStoryboard(DependencyObject obj)
        {
            return (Storyboard)obj.GetValue(ShowStoryboardProperty);
        }

        public static void SetShowStoryboard(DependencyObject obj, Storyboard value)
        {
            obj.SetValue(ShowStoryboardProperty, value);
        }

        public static readonly DependencyProperty ShowStoryboardProperty =
            DependencyProperty.RegisterAttached("ShowStoryboard", typeof(Storyboard), typeof(ShowHideAnimation), new PropertyMetadata(null, ShowHideAnimation.OnShowStoryboardChanged));

        public static Storyboard GetHideStoryboard(DependencyObject obj)
        {
            return (Storyboard)obj.GetValue(HideStoryboardProperty);
        }

        public static void SetHideStoryboard(DependencyObject obj, Storyboard value)
        {
            obj.SetValue(HideStoryboardProperty, value);
        }

        public static readonly DependencyProperty HideStoryboardProperty =
            DependencyProperty.RegisterAttached("HideStoryboard", typeof(Storyboard), typeof(ShowHideAnimation), new PropertyMetadata(null, ShowHideAnimation.OnHideStoryboardChanged));

        public static Visibility GetVisibility(DependencyObject obj)
        {
            return (Visibility)obj.GetValue(VisibilityProperty);
        }

        public static void SetVisibility(DependencyObject obj, Visibility value)
        {
            obj.SetValue(VisibilityProperty, value);
        }

        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.RegisterAttached("Visibility", typeof(Visibility), typeof(ShowHideAnimation), new PropertyMetadata(Visibility.Visible, ShowHideAnimation.OnVisibilityChanged));



        public static Image GetSnapshotHolder(DependencyObject obj)
        {
            return (Image)obj.GetValue(SnapshotHolderProperty);
        }

        public static void SetSnapshotHolder(DependencyObject obj, Image value)
        {
            obj.SetValue(SnapshotHolderProperty, value);
        }

        public static readonly DependencyProperty SnapshotHolderProperty =
            DependencyProperty.RegisterAttached("SnapshotHolder", typeof(Image), typeof(ShowHideAnimation), new PropertyMetadata(null, ShowHideAnimation.OnSnapshotHolderChanged));

        private static void OnSnapshotHolderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;

            var element = d as FrameworkElement;
            if (element == null)
                throw new ArgumentException("d");

            var entry = s_entries.GetOrCreate(element, () => new EntryInfo());
            entry.SnapshotHolder = (Image)e.NewValue;
        }



        public static FrameworkElement GetContentHolder(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(ContentHolderProperty);
        }

        public static void SetContentHolder(DependencyObject obj, FrameworkElement value)
        {
            obj.SetValue(ContentHolderProperty, value);
        }

        // Using a DependencyProperty as the backing store for ContentHolder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentHolderProperty =
            DependencyProperty.RegisterAttached("ContentHolder", typeof(FrameworkElement), typeof(ShowHideAnimation), new PropertyMetadata(null, ShowHideAnimation.OnContentHolderChanged));

        private static void OnContentHolderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;

            var element = d as FrameworkElement;
            if (element == null)
                throw new ArgumentException("d");

            var entry = s_entries.GetOrCreate(element, () => new EntryInfo());
            entry.ContentHolder = (FrameworkElement)e.NewValue;
        }



        public static readonly RoutedEvent PrepareSnapshotEvent = EventManager.RegisterRoutedEvent("PrepareSnapshot", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ShowHideAnimation));

        public static void AddPrepareSnapshotHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement element = d as UIElement;
            if (element != null)
                element.AddHandler(ShowHideAnimation.PrepareSnapshotEvent, handler);
        }
        public static void RemovePrepareSnapshotHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement element = d as UIElement;
            if (element != null)
                element.RemoveHandler(ShowHideAnimation.PrepareSnapshotEvent, handler);
        }


        public static readonly RoutedEvent SnapshotCompleteEvent = EventManager.RegisterRoutedEvent("SnapshotComplete", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ShowHideAnimation));

        public static void AddSnapshotCompleteHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement element = d as UIElement;
            if (element != null)
                element.AddHandler(ShowHideAnimation.SnapshotCompleteEvent, handler);
        }
        public static void RemoveSnapshotCompleteHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement element = d as UIElement;
            if (element != null)
                element.RemoveHandler(ShowHideAnimation.SnapshotCompleteEvent, handler);
        }

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;

            var element = d as FrameworkElement;
            if (element == null)
                throw new ArgumentException("d");

            var entry = s_entries.GetOrCreate(element, () => new EntryInfo());
            var visibility = (Visibility)e.NewValue;

            if (visibility == Visibility.Visible)
            {
                if (entry.ShowStoryboard == null)
                    element.Visibility = Visibility.Visible;
                else
                {
                    if (entry.SnapshotHolder != null)
                        ShowHideAnimation.TakeSnapshot(element);

                    entry.ShowStoryboardCompletedEventHander = (_, __) => ShowHideAnimation.ShowStoryboard_Completed(element, _, __);
                    entry.ShowStoryboard.Completed += entry.ShowStoryboardCompletedEventHander;
                    element.Visibility = Visibility.Visible;

                    if (element.IsLoaded)
                        entry.ShowStoryboard.Begin(element);
                    else
                    {
                        entry.ShowStoryboard.Begin(element, true);
                        entry.ShowStoryboard.SkipToFill(element);
                    }
                }
            }
            else if (visibility != Visibility.Visible)
            {
                if (entry.HideStoryboard == null)
                    element.Visibility = visibility;
                else
                {
                    if (entry.SnapshotHolder != null)
                        ShowHideAnimation.TakeSnapshot(element);

                    entry.HideStoryboardCompletedEventHander = (_, __) => ShowHideAnimation.HideStoryboard_Completed(element, _, __);
                    entry.HideStoryboard.Completed += entry.HideStoryboardCompletedEventHander;
                    element.Visibility = Visibility.Visible;

                    if (element.IsLoaded)
                        entry.HideStoryboard.Begin(element);
                    else
                    {
                        entry.HideStoryboard.Begin(element, true);
                        entry.HideStoryboard.SkipToFill(element);
                    }
                }
            }
        }

        private static void TakeSnapshot(FrameworkElement element)
        {
            var entry = s_entries[element];

            var eventArgs = new RoutedEventArgs(PrepareSnapshotEvent);
            element.RaiseEvent(eventArgs);

            var renderTarget = new RenderTargetBitmap((int)Math.Ceiling(element.ActualWidth), (int)Math.Ceiling(element.ActualHeight), 96, 96, PixelFormats.Pbgra32);
            renderTarget.Render(element);

            eventArgs = new RoutedEventArgs(SnapshotCompleteEvent);
            element.RaiseEvent(eventArgs);

            entry.SnapshotHolder.Source = renderTarget;
            entry.SnapshotHolder.Visibility = Visibility.Visible;

            if (entry.ContentHolder != null)
                entry.ContentHolder.Visibility = Visibility.Hidden;
        }

        private static void OnHideStoryboardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;

            var element = d as FrameworkElement;
            if (element == null)
                throw new ArgumentException("d");

            var entry = s_entries.GetOrCreate(element, () => new EntryInfo());

            if (entry.HideStoryboard != null)
                ShowHideAnimation.UnhandleHideStoryboardCompletedEvent(element);

            if (e.NewValue == null)
                entry.HideStoryboard = null;
            else
                entry.HideStoryboard = (Storyboard)((Freezable)e.NewValue).Clone();
        }

        private static void OnShowStoryboardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;

            var element = d as FrameworkElement;
            if (element == null)
                throw new ArgumentException("d");

            var entry = s_entries.GetOrCreate(element, () => new EntryInfo());

            if (entry.ShowStoryboard != null)
                ShowHideAnimation.UnhandleShowStoryboardCompletedEvent(element);

            if (e.NewValue == null)
                entry.ShowStoryboard = null;
            else
                entry.ShowStoryboard = (Storyboard)((Freezable)e.NewValue).Clone();
        }

        static void UnhandleShowStoryboardCompletedEvent(FrameworkElement element)
        {
            var entry = s_entries[element];
            if (entry.ShowStoryboardCompletedEventHander != null)
            {
                entry.ShowStoryboard.Completed -= entry.ShowStoryboardCompletedEventHander;
                entry.ShowStoryboardCompletedEventHander = null;
            }
        }

        static void UnhandleHideStoryboardCompletedEvent(FrameworkElement element)
        {
            var entry = s_entries[element];
            if (entry.HideStoryboardCompletedEventHander != null)
            {
                entry.HideStoryboard.Completed -= entry.HideStoryboardCompletedEventHander;
                entry.HideStoryboardCompletedEventHander = null;
            }
        }

        static void ShowStoryboard_Completed(FrameworkElement element, object sender, EventArgs e)
        {
            EntryInfo entry;
            if (s_entries.TryGetValue(element, out entry))
            {
                ShowHideAnimation.UnhandleShowStoryboardCompletedEvent(element);
                if (entry.ContentHolder != null)
                    entry.ContentHolder.Visibility = Visibility.Visible;
                if (entry.SnapshotHolder != null)
                    entry.SnapshotHolder.Visibility = Visibility.Hidden;
            }
        }

        static void HideStoryboard_Completed(FrameworkElement element, object sender, EventArgs e)
        {
            EntryInfo entry;
            if (s_entries.TryGetValue(element, out entry))
            {
                element.Visibility = ShowHideAnimation.GetVisibility(element);
                ShowHideAnimation.UnhandleHideStoryboardCompletedEvent(element);
                if (entry.ContentHolder != null)
                    entry.ContentHolder.Visibility = Visibility.Visible;
                if (entry.SnapshotHolder != null)
                    entry.SnapshotHolder.Visibility = Visibility.Hidden;
            }
        }



    }
}

