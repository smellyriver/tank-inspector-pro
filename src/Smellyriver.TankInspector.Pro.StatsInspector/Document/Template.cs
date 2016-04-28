using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Common.Utilities;
using Smellyriver.TankInspector.Common.Wpf.Utilities;
using Localization = Smellyriver.TankInspector.Pro.Globalization.Localization;

namespace Smellyriver.TankInspector.Pro.StatsInspector.Document
{
    public static class Template
    {
        public static string GetName(DependencyObject obj)
        {
            var name = (string)obj.GetValue(NameProperty);
            return name ?? Localization.Instance.L("stats_inspector", "untitled_template_name");
        }

        public static void SetName(DependencyObject obj, string value)
        {
            obj.SetValue(NameProperty, value);
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.RegisterAttached("Name", typeof(string), typeof(Template), new PropertyMetadata(null));

        public static string GetDescription(DependencyObject obj)
        {
            return (string)obj.GetValue(DescriptionProperty);
        }

        public static void SetDescription(DependencyObject obj, string value)
        {
            obj.SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.RegisterAttached("Description", typeof(string), typeof(Template), new PropertyMetadata(null));

        public static string GetAuthor(DependencyObject obj)
        {
            return (string)obj.GetValue(AuthorProperty) ?? Localization.Instance.L("stats_inspector", "anonymous_template_author");
        }

        public static void SetAuthor(DependencyObject obj, string value)
        {
            obj.SetValue(AuthorProperty, value);
        }

        public static readonly DependencyProperty AuthorProperty =
            DependencyProperty.RegisterAttached("Author", typeof(string), typeof(Template), new PropertyMetadata(null));


        private static readonly Dictionary<TextElement, TextElementPositionInfo> s_positionInfos
            = new Dictionary<TextElement, TextElementPositionInfo>();
        public static bool GetShouldShow(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShouldShowProperty);
        }

        public static void SetShouldShow(DependencyObject obj, bool value)
        {
            obj.SetValue(ShouldShowProperty, value);
        }


        public static readonly DependencyProperty ShouldShowProperty =
            DependencyProperty.RegisterAttached("ShouldShow", typeof(bool), typeof(Template), new PropertyMetadata(true, Template.OnShouldShowChanged));

        private static void OnShouldShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldValue = (bool)e.OldValue;
            var newValue = (bool)e.NewValue;

            var element = d as TextElement;
            if (element != null)
            {
                Template.OnShouldShowChanged(element, oldValue, newValue);
                return;
            }
        }


        private static void OnShouldShowChanged(TextElement element, bool oldValue, bool newValue)
        {
            if (oldValue == newValue)
                return;

            if (newValue == false)
            {

                var parent = element.Parent as TextElement;
                if (parent == null)
                    throw new ArgumentException("element");

                s_positionInfos[element] = Template.CreatePositionInfo(element, parent);

                parent.RemoveChild(element);
            }
            else
            {
                TextElementPositionInfo positionInfo;

                if (s_positionInfos.TryGetValue(element, out positionInfo))
                {
                    if (!positionInfo.Parent.IsAlive)
                    {
                        s_positionInfos.Remove(element);
                        return;
                    }

                    var parent = positionInfo.Parent.Target;
                    var currentSiblings = parent.GetChildren().ToArray();

                    TextElement previousSiblingCandidate = null;
                    var previousSiblingCandidateIndex = -1;

                    for (var i = positionInfo.PreceedingSiblingsSnapshot.Length - 1; i >= 0; --i)
                    {
                        if (!positionInfo.PreceedingSiblingsSnapshot[i].IsAlive)
                            continue;

                        var sibling = positionInfo.PreceedingSiblingsSnapshot[i].Target;

                        var currentIndex = currentSiblings.IndexOf(sibling);
                        if (currentIndex == -1)
                            continue;

                        if (currentIndex > previousSiblingCandidateIndex)
                        {
                            previousSiblingCandidateIndex = currentIndex;
                            previousSiblingCandidate = sibling;
                        }
                    }

                    TextElement nextSiblingCandidate = null;
                    var nextSiblingCandidateIndex = positionInfo.SucceedingSiblingsSnapshot.Length;

                    for (var i = 0; i < positionInfo.SucceedingSiblingsSnapshot.Length; ++i)
                    {
                        if (!positionInfo.SucceedingSiblingsSnapshot[i].IsAlive)
                            continue;

                        var sibling = positionInfo.SucceedingSiblingsSnapshot[i].Target;

                        var currentIndex = currentSiblings.IndexOf(sibling);
                        if (currentIndex == -1)
                            continue;

                        if (currentIndex < nextSiblingCandidateIndex)
                        {
                            nextSiblingCandidateIndex = currentIndex;
                            nextSiblingCandidate = sibling;
                        }
                    }

                    if (previousSiblingCandidate == null && nextSiblingCandidate == null)
                        parent.AddChild(element);
                    else if (previousSiblingCandidate != null && nextSiblingCandidate != null)
                    {
                        var previousSiblingCandidateDistance = positionInfo.PreceedingSiblingsSnapshot.Length - previousSiblingCandidateIndex;
                        var nextSiblingCandidateDistance = nextSiblingCandidateIndex + 1;
                        if (previousSiblingCandidateDistance >= nextSiblingCandidateDistance)
                            parent.InsertAfter(previousSiblingCandidate, element);
                        else
                            parent.InsertBefore(nextSiblingCandidate, element);
                    }
                    else if (previousSiblingCandidate != null)
                        parent.InsertAfter(previousSiblingCandidate, element);
                    else
                        parent.InsertBefore(nextSiblingCandidate, element);

                    s_positionInfos[element] = Template.CreatePositionInfo(element, parent, currentSiblings);
                }
            }
        }

        private static TextElementPositionInfo CreatePositionInfo(TextElement element, TextElement parent, TextElement[] siblings = null)
        {
            siblings = siblings ?? parent.GetChildren().Cast<TextElement>().ToArray();

            var elementIndex = siblings.IndexOf(element);
            var preceedingSiblings = siblings.SubArray(0, elementIndex);
            var succeedingSiblings = siblings.SubArray(elementIndex + 1);

            return new TextElementPositionInfo
            {
                Parent = new WeakReference<TextElement>(parent),
                PreceedingSiblingsSnapshot = preceedingSiblings.Select(s => new WeakReference<TextElement>(s)).ToArray(),
                SucceedingSiblingsSnapshot = succeedingSiblings.Select(s => new WeakReference<TextElement>(s)).ToArray(),
            };
        }

    }
}
