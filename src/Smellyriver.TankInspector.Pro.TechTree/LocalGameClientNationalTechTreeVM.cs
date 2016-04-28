using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Pro.Data;
using Smellyriver.TankInspector.Pro.Gameplay;
using Smellyriver.TankInspector.Pro.Repository;
using TankData = Smellyriver.TankInspector.Pro.Data.IXQueryable;
using TankEntity = Smellyriver.TankInspector.Pro.Data.Entities.Tank;

namespace Smellyriver.TankInspector.Pro.TechTree
{
    class LocalGameClientNationalTechTreeVM : NotificationObject
    {
        public string Name { get; private set; }

        public ImageSource SmallIcon { get; private set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            private set
            {
                _isLoading = value;
                this.RaisePropertyChanged(() => this.IsLoading);
            }
        }


        private bool _isLoaded;
        public bool IsLoaded
        {
            get { return _isLoaded; }
            private set
            {
                _isLoaded = value;
                this.RaisePropertyChanged(() => this.IsLoaded);
            }
        }

        private const int TankTiers = 10;

        private int _rows;
        public int Rows
        {
            get { return _rows; }
            private set
            {
                _rows = value;
                this.RaisePropertyChanged(() => this.Rows);
            }
        }

        private int _columns;
        public int Columns
        {
            get { return _columns; }
            private set
            {
                _columns = value;
                this.RaisePropertyChanged(() => this.Columns);
            }
        }


        private NationalTechTreeNodeVM[] _nodes;
        public NationalTechTreeNodeVM[] Nodes
        {
            get { return _nodes; }
            private set
            {
                _nodes = value;
                this.RaisePropertyChanged(() => this.Nodes);
            }
        }


        private readonly LocalGameClient _client;
        public string NationKey { get; }

        private readonly IXQueryable _layout;

        public event EventHandler LoadBegin;

        public LocalGameClientNationalTechTreeVM(LocalGameClient client, string nationKey)
        {
            _client = client;
            this.NationKey = nationKey;

            _layout = client.TechTreeLayoutDatabase.Query("layout[@nation='{0}']", nationKey);

            this.Name = _client.Localization.GetLocalizedNationName(NationKey);
            this.SmallIcon = _client.PackageImages.GetNationSmallIcon(NationKey);
        }

        private void Load()
        {
            this.IsLoading = true;

            const int columns = TankTiers;

            var nodes = new List<NationalTechTreeNodeVM>();

            var nodeTankMap = new Dictionary<TechTreeLayoutNode, TankEntity>();

            var tanksInTechTree = new HashSet<TankEntity>(TankHelper.KeyEqualityComparer);

            var rowMap = new Dictionary<int, int>();

            var definedRows = _layout == null ? 0 : _layout.QueryInt("grid/rows");

            // 1. build a node grid, solve situations where two tanks are occupying one cell

            var occupiedCells = new List<TechTreeLayoutNode[]>();
            for (int row = 0; row < definedRows + 1; ++row)
                occupiedCells.Add(new TechTreeLayoutNode[columns]);

            if (_layout != null)
            {

                var t1TankRow = 0;

                foreach (var node in _layout.QueryMany("nodes/node").Select(n => new TechTreeLayoutNode(n)))
                {
                    var tankData = _client.GetTank(this.NationKey, node.TankKey);
                    if (tankData != null)
                    {
                        var tank = new TankEntity(tankData);
                        var row = node.Row;
                        int mappedRow;
                        if (rowMap.TryGetValue(row, out mappedRow))
                            row = mappedRow;

                        var tankTier = tank.Tier;
                        var column = tankTier - 1;
                        while (occupiedCells[row][column] != null)
                        {
                            // two tanks are occupying one cell, insert a row
                            ++row;
                            if (row == occupiedCells.Count)
                                occupiedCells.Insert(row, new TechTreeLayoutNode[columns]);
                        }

                        rowMap[node.Row] = row;

                        occupiedCells[row][column] = node;
                        nodeTankMap[node] = tank;
                        tanksInTechTree.Add(tank);

                        if (tankTier == 1 && !tank.IsPremium && !tank.IsObsoleted)
                            t1TankRow = row;
                    }
                }

                // remove empty rows
                for (int row = occupiedCells.Count - 1; row >= 0; --row)
                    if (occupiedCells[row].All(node => node == null))
                        occupiedCells.RemoveAt(row);
                    else
                        break;

                // place T1 tank in vertical center 
                var newT1TankRow = (int)Math.Round(occupiedCells.Count / 2.0);
                var sign = newT1TankRow > t1TankRow ? -1 : 1;
                for (int row = newT1TankRow; row != t1TankRow; row += sign)
                {
                    if (occupiedCells[row][0] == null)
                    {
                        var t1Node = occupiedCells[t1TankRow][0];
                        var newT1Node = new TechTreeLayoutNode(t1Node.Tank, row, t1Node.Column, t1Node.UnlockTanks);
                        occupiedCells[row][0] = newT1Node;
                        occupiedCells[t1TankRow][0] = null;
                        nodeTankMap[newT1Node] = nodeTankMap[t1Node];
                        nodeTankMap.Remove(t1Node);
                        break;
                    }
                }
            }

            // 2. find out tanks which are existed and valid, but not appeared in the techtree
            var techTreeRowCount = occupiedCells.Count;

            foreach (var tankData in _client.TankDatabase.QueryMany("tank[nation/@key='{0}']", this.NationKey))
            {
                var tank = new TankEntity(tankData);
                if (!tanksInTechTree.Contains(tank) /*&& tank.IsValid*/)
                {
                    var row = techTreeRowCount;
                    var column = tank.QueryInt("level") - 1;
                    while (true)
                    {
                        if (occupiedCells.Count <= row)
                            occupiedCells.Insert(row, new TechTreeLayoutNode[columns]);
                        else if (occupiedCells[row][column] != null)
                            ++row;
                        else
                        {
                            var node = new TechTreeLayoutNode(tank, row, column, null);
                            occupiedCells[row][column] = node;
                            nodeTankMap[node] = tank;
                            break;
                        }
                    }
                }
            }

            // 3. create node viewmodels
            for (int row = 1; row < occupiedCells.Count; ++row)
                for (int column = 0; column < columns; ++column)
                {
                    var node = occupiedCells[row][column];
                    if (node != null)
                    {
                        var tank = nodeTankMap[node];
                        nodes.Add(new NationalTechTreeNodeVM(_client, tank, row, column, node.UnlockTanks, this.GetTankIcon(tank)));
                    }
                }


            this.Rows = occupiedCells.Count;
            this.Columns = columns;
            this.Nodes = nodes.ToArray();
            // IsLoading will be set by the view, after all controls are populated
            this.IsLoading = false;

            this.IsLoaded = true;
        }

        private ImageSource GetTankIcon(TankEntity tank)
        {
            return _client.PackageImages.GetTankSmallIcon(tank.IconKey);
        }

        public void BeginLoad()
        {
            if (this.IsLoading)
                return;

            if (this.LoadBegin != null)
                this.LoadBegin(this, EventArgs.Empty);

            Task.Factory.StartNew(this.Load);
        }

    }
}
