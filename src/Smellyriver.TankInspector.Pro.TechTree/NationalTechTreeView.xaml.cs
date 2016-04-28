using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Smellyriver.TankInspector.Common.Wpf.Utilities;

namespace Smellyriver.TankInspector.Pro.TechTree
{
    /// <summary>
    /// Interaction logic for NationalTechTreeView.xaml
    /// </summary>
    public partial class NationalTechTreeView : UserControl
    {
        private static BitmapImage s_arrowHeadImage;

        static NationalTechTreeView()
        {
            s_arrowHeadImage = BitmapImageEx.LoadAsFrozen("Resources/Images/ArrowHead.png");
        }

        internal LocalGameClientNationalTechTreeVM ViewModel
        {
            get { return this.DataContext as LocalGameClientNationalTechTreeVM; }
            set
            {
                this.DataContext = value;
            }
        }



        private bool _updateConnectorsPending = true;
        private Style _connectorStyle;
        private Thickness _nodeMargin;

        public NationalTechTreeView()
        {
            InitializeComponent();
            _connectorStyle = this.FindResource("ConnectionLine") as Style;
            this.CalculateNodeMargin();
            this.TanksContainer.ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
            this.TanksContainer.LayoutUpdated += TanksContainer_LayoutUpdated;
            this.TanksContainer.AddPropertyChangedHandler(ActualHeightProperty, TanksContainer_ActualHeightChanged);
        }

        private void TanksContainer_ActualHeightChanged(object sender, EventArgs e)
        {
            if (this.TanksContainer.ActualHeight > 0 && _updateConnectorsPending)
                this.UpdateConnectors();
        }


        private void CalculateNodeMargin()
        {
            var padding = (Thickness)this.FindResource("NodeButtonPadding");
            var margin = (Thickness)this.FindResource("NodeButtonMarginInTechTree");
            _nodeMargin = new Thickness(padding.Left + margin.Left,
                padding.Top + margin.Top,
                padding.Right + margin.Right,
                padding.Bottom + margin.Bottom);
        }

        void TanksContainer_LayoutUpdated(object sender, EventArgs e)
        {
            if (_updateConnectorsPending)
                this.UpdateConnectors();
        }

        void ItemContainerGenerator_ItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            _updateConnectorsPending = true;
        }


        private Point GetTopCenterPoint(ContentPresenter node)
        {
            return node.TranslatePoint(new Point(node.ActualWidth / 2 - (_nodeMargin.Right - _nodeMargin.Left) / 2, _nodeMargin.Top), this.LineCanvas);
        }

        private Point GetBottomCenterPoint(ContentPresenter node)
        {
            return node.TranslatePoint(new Point(node.ActualWidth / 2 - (_nodeMargin.Right - _nodeMargin.Left) / 2, node.ActualHeight - _nodeMargin.Bottom), this.LineCanvas);
        }

        private void Connect(ContentPresenter node1, ContentPresenter node2)
        {
            var from = node1;
            var to = node2;

            var fromColumn = Grid.GetColumn(from);
            var toColumn = Grid.GetColumn(to);
            var fromRow = Grid.GetRow(from);
            var toRow = Grid.GetRow(to);
            var inSameColumn = fromColumn == toColumn;

            Point fromPoint, toPoint;
            if (inSameColumn)
            {
                if (fromRow > toRow)
                {
                    fromPoint = this.GetTopCenterPoint(from);
                    toPoint = this.GetBottomCenterPoint(to);
                }
                else
                {
                    fromPoint = this.GetBottomCenterPoint(from);
                    toPoint = this.GetTopCenterPoint(to);
                }
            }
            else
            {
                Debug.Assert(fromColumn < toColumn);
                var fromX = from.ActualWidth - _nodeMargin.Right;
                var toX = _nodeMargin.Left;

                if (fromRow == toRow)
                {
                    var verticalMargin = (_nodeMargin.Bottom - _nodeMargin.Top) / 2;
                    fromPoint = from.TranslatePoint(new Point(fromX, from.ActualHeight / 2 - verticalMargin), this.LineCanvas);
                    toPoint = to.TranslatePoint(new Point(toX, to.ActualHeight / 2 - verticalMargin), this.LineCanvas);
                }
                else if (fromRow > toRow)
                {
                    fromPoint = from.TranslatePoint(new Point(fromX, _nodeMargin.Top), this.LineCanvas);
                    toPoint = to.TranslatePoint(new Point(toX, to.ActualHeight - _nodeMargin.Bottom), this.LineCanvas);
                }
                else
                {
                    fromPoint = from.TranslatePoint(new Point(fromX, from.ActualHeight - _nodeMargin.Bottom), this.LineCanvas);
                    toPoint = to.TranslatePoint(new Point(toX, _nodeMargin.Top), this.LineCanvas);
                }
            }


            var line = new Line();
            line.X1 = fromPoint.X;
            line.Y1 = fromPoint.Y;
            line.X2 = toPoint.X;
            line.Y2 = toPoint.Y;
            line.Style = _connectorStyle;
            this.LineCanvas.Children.Add(line);

            var arrowHead = new Image();
            arrowHead.Source = s_arrowHeadImage;
            Canvas.SetLeft(arrowHead, toPoint.X - s_arrowHeadImage.Width);
            Canvas.SetTop(arrowHead, toPoint.Y - s_arrowHeadImage.Height / 2);
            this.LineCanvas.Children.Add(arrowHead);
            arrowHead.RenderTransformOrigin = new Point(1, 0.5);
            arrowHead.RenderTransform = new RotateTransform(180 * Math.Atan2(toPoint.Y - fromPoint.Y, toPoint.X - fromPoint.X) / Math.PI);

        }


        private void UpdateConnectors()
        {
            this.LineCanvas.Children.Clear();

            if (!this.ViewModel.IsLoaded)
                return;

            if (this.TanksContainer.Items.Count == 0)
            {
                if (this.ViewModel.Nodes.Length == 0)
                {
                    _updateConnectorsPending = false;
                }
                return;
            }
            else if (this.TanksContainer.ItemContainerGenerator.ContainerFromIndex(0) == null
                || ((FrameworkElement)this.TanksContainer.ItemContainerGenerator.ContainerFromIndex(0)).IsLoaded)
            {
                _updateConnectorsPending = true;
                return;
            }
            else if (this.TanksContainer.ActualHeight == 0)
            {
                _updateConnectorsPending = true;
                return;
            }

            this.LineCanvas.BeginInit();
            this.LineCanvas.Children.Clear();

            var nodes = this.TanksContainer.Items.OfType<NationalTechTreeNodeVM>()
                            .Select(item =>
                                    TanksContainer.ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter);

            // don't use linq method ToDictionary here because there might be duplicated key
            var tankNodeMap = new Dictionary<string, ContentPresenter>();
            foreach (var node in nodes)
            {
                tankNodeMap[((NationalTechTreeNodeVM)node.Content).Tank["@key"]] = node;
            }

            foreach (var fromNode in tankNodeMap.Values)
            {
                var fromNodeVm = (NationalTechTreeNodeVM)fromNode.Content;
                if (fromNodeVm.UnlockTanks.Any())
                {
                    foreach (var unlockedTankKey in fromNodeVm.UnlockTanks)
                    {
                        ContentPresenter toNode;
                        if (tankNodeMap.TryGetValue(unlockedTankKey, out toNode))
                            this.Connect(fromNode, toNode);
                    }
                }
            }
            this.LineCanvas.EndInit();

            //this.ViewModel.IsLoading = false;

            _updateConnectorsPending = false;
            //this.PendContentSnapshot();

        }

        private void TankItemButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            button.ContextMenu.PlacementTarget = button;
            button.ContextMenu.IsOpen = true;
        }
    }
}
