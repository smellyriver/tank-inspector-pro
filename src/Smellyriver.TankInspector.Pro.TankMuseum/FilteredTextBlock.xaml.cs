using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Smellyriver.TankInspector.Pro.TankMuseum
{
    /// <summary>
    /// Interaction logic for FilteredTextBlock.xaml
    /// </summary>
    public partial class FilteredTextBlock : UserControl
    {

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(FilteredTextBlock), new PropertyMetadata(null, FilteredTextBlock.OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FilteredTextBlock)d).OnTextChanged(e.OldValue as string, e.NewValue as string);
        }

        public string Keyword
        {
            get { return (string)GetValue(KeywordProperty); }
            set { SetValue(KeywordProperty, value); }
        }

        public static readonly DependencyProperty KeywordProperty =
            DependencyProperty.Register("Keyword", typeof(string), typeof(FilteredTextBlock), new PropertyMetadata(null, FilteredTextBlock.OnKeywordChanged));

        private static void OnKeywordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FilteredTextBlock)d).OnKeywordChanged(e.OldValue as string, e.NewValue as string);
        }

        public Style HighlightStyle
        {
            get { return (Style)GetValue(HighlightStyleProperty); }
            set { SetValue(HighlightStyleProperty, value); }
        }

        public static readonly DependencyProperty HighlightStyleProperty =
            DependencyProperty.Register("HighlightStyle", typeof(Style), typeof(FilteredTextBlock), new PropertyMetadata(null, FilteredTextBlock.OnHighlightStyleChanged));

        private static void OnHighlightStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FilteredTextBlock)d).OnHighlightStyleChanged((Style)e.OldValue, (Style)e.NewValue);
        }

        private readonly List<TextBlock> _highlightTextBlocks;
        private readonly Style _defaultHighlightStyle;

        public FilteredTextBlock()
        {
            _highlightTextBlocks = new List<TextBlock>();
            InitializeComponent();
            _defaultHighlightStyle = (Style)this.FindResource("DefaultHighlightStyle");
        }


        private void OnTextChanged(string oldValue, string newValue)
        {
            this.RebuildTextBlocks();
        }

        private void OnKeywordChanged(string oldValue, string newValue)
        {
            this.RebuildTextBlocks();
        }

        private void OnHighlightStyleChanged(Style oldValue, Style newValue)
        {
            foreach (var textBlock in _highlightTextBlocks)
                textBlock.Style = newValue;
        }

        private void RebuildTextBlocks()
        {
            _highlightTextBlocks.Clear();
            this.Container.Children.Clear();

            if (string.IsNullOrEmpty(this.Text))
                return;

            if (string.IsNullOrEmpty(this.Keyword))
            {
                this.Container.Children.Add(new TextBlock { Text = this.Text });
                return;
            }

            var lowerText = this.Text.ToLowerInvariant();
            var lowerKeyword = this.Keyword.ToLowerInvariant();
            var startIndex = 0;

            while (true)
            {
                var searchIndex = lowerText.IndexOf(lowerKeyword, startIndex);
                if (searchIndex == -1)
                {
                    this.Container.Children.Add(new TextBlock { Text = this.Text.Substring(startIndex) });
                    break;
                }

                this.Container.Children.Add(new TextBlock { Text = this.Text.Substring(startIndex, searchIndex - startIndex) });
                this.CreateHighlightTextBlock(this.Text.Substring(searchIndex, this.Keyword.Length));
                startIndex = searchIndex + this.Keyword.Length;
            }
        }


        private void CreateHighlightTextBlock(string text)
        {
            var textBlock = new TextBlock { Text = text };
            textBlock.Style = this.HighlightStyle ?? _defaultHighlightStyle;
            _highlightTextBlocks.Add(textBlock);
            this.Container.Children.Add(textBlock);
        }
    }
}
