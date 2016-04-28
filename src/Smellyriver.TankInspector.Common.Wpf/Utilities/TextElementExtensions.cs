using System.Collections.Generic;
using System.Windows.Documents;

namespace Smellyriver.TankInspector.Common.Wpf.Utilities
{
    public static class TextElementExtensions
    {

        public static TextElement GetNextSibling(this TextElement element)
        {
            var block = element as Block;
            if (block != null)
                return block.NextBlock;

            var inline = element as Inline;
            if (inline != null)
                return inline.NextInline;

            var listItem = element as ListItem;
            if (listItem != null)
                return listItem.NextListItem;

            var tableCell = element as TableCell;
            if (tableCell != null)
            {
                var parentRow = (TableRow)tableCell.Parent;
                if (parentRow == null)
                    return null;

                return GetNextSibling(parentRow.Cells, tableCell);
            }

            var tableRow = element as TableRow;
            if (tableRow != null)
            {
                var parentRowGroup = (TableRowGroup)tableRow.Parent;
                if (parentRowGroup == null)
                    return null;

                return GetNextSibling(parentRowGroup.Rows, tableRow);
            }

            var tableRowGroup = element as TableRowGroup;
            if (tableRowGroup != null)
            {
                var parentTable = (Table)tableRowGroup.Parent;
                if (parentTable == null)
                    return null;

                return GetNextSibling(parentTable.RowGroups, tableRowGroup);
            }

            return null;
        }

        private static TextElement GetNextSibling<T>(IList<T> parent, T element)
            where T : TextElement
        {
            var index = parent.IndexOf(element);
            if (index == -1)
                return null;

            if (index == parent.Count - 1)
                return null;

            return parent[index + 1];
        }

        public static TextElement GetPreviousSibling(this TextElement element)
        {
            var block = element as Block;
            if (block != null)
                return block.PreviousBlock;

            var inline = element as Inline;
            if (inline != null)
                return inline.PreviousInline;

            var listItem = element as ListItem;
            if (listItem != null)
                return listItem.PreviousListItem;

            var tableCell = element as TableCell;
            if (tableCell != null)
            {
                var parentRow = (TableRow)tableCell.Parent;
                if (parentRow == null)
                    return null;

                return GetPreviousSibling(parentRow.Cells, tableCell);
            }

            var tableRow = element as TableRow;
            if (tableRow != null)
            {
                var parentRowGroup = (TableRowGroup)tableRow.Parent;
                if (parentRowGroup == null)
                    return null;

                return GetPreviousSibling(parentRowGroup.Rows, tableRow);
            }

            var tableRowGroup = element as TableRowGroup;
            if (tableRowGroup != null)
            {
                var parentTable = (Table)tableRowGroup.Parent;
                if (parentTable == null)
                    return null;

                return GetPreviousSibling(parentTable.RowGroups, tableRowGroup);
            }

            return null;
        }

        private static TextElement GetPreviousSibling<T>(IList<T> parent, T element)
            where T : TextElement
        {
            var index = parent.IndexOf(element);
            if (index == -1)
                return null;

            if (index == 0)
                return null;

            return parent[index - 1];
        }

        public static bool RemoveChild(this TextElement parent, TextElement child)
        {
            var list = parent as List;
            if (list != null)
                return RemoveChild(list, child);

            var listItem = parent as ListItem;
            if (listItem != null)
                return RemoveChild(listItem, child);

            var paragraph = parent as Paragraph;
            if (paragraph != null)
                return RemoveChild(paragraph, child);

            var section = parent as Section;
            if (section != null)
                return RemoveChild(section, child);

            var table = parent as Table;
            if (table != null)
                return RemoveChild(table, child);

            var tableRowGroup = parent as TableRowGroup;
            if (tableRowGroup != null)
                return RemoveChild(tableRowGroup, child);

            var tableRow = parent as TableRow;
            if (tableRow != null)
                return RemoveChild(tableRow, child);

            var tableCell = parent as TableCell;
            if (tableCell != null)
                return RemoveChild(tableCell, child);

            var anchoredBlock = parent as AnchoredBlock;
            if (anchoredBlock != null)
                return RemoveChild(anchoredBlock, child);

            var span = parent as Span;
            if (span != null)
                return RemoveChild(span, child);

            return false;
        }

        public static bool RemoveChild(this List parent, TextElement child)
        {
            var listItem = child as ListItem;
            if (listItem == null)
                return false;

            return parent.ListItems.Remove(listItem);
        }

        public static bool RemoveChild(this ListItem parent, TextElement child)
        {
            var block = child as Block;
            if (block == null)
                return false;

            return parent.Blocks.Remove(block);
        }

        public static bool RemoveChild(this Paragraph parent, TextElement child)
        {
            var inline = child as Inline;
            if (inline == null)
                return false;

            return parent.Inlines.Remove(inline);
        }

        public static bool RemoveChild(this Section parent, TextElement child)
        {
            var block = child as Block;
            if (block == null)
                return false;

            return parent.Blocks.Remove(block);
        }

        public static bool RemoveChild(this Table parent, TextElement child)
        {
            var rowGroup = child as TableRowGroup;
            if (rowGroup == null)
                return false;

            return parent.RowGroups.Remove(rowGroup);
        }

        public static bool RemoveChild(this TableRowGroup parent, TextElement child)
        {
            var row = child as TableRow;
            if (row == null)
                return false;

            return parent.Rows.Remove(row);
        }

        public static bool RemoveChild(this TableRow parent, TextElement child)
        {
            var cell = child as TableCell;
            if (cell == null)
                return false;

            return parent.Cells.Remove(cell);
        }

        public static bool RemoveChild(this TableCell parent, TextElement child)
        {
            var block = child as Block;
            if (block == null)
                return false;

            return parent.Blocks.Remove(block);
        }

        public static bool RemoveChild(this AnchoredBlock parent, TextElement child)
        {
            var block = child as Block;
            if (block == null)
                return false;

            return parent.Blocks.Remove(block);
        }

        public static bool RemoveChild(this Span parent, TextElement child)
        {
            var inline = child as Inline;
            if (inline == null)
                return false;

            return parent.Inlines.Remove(inline);
        }


        public static void AddChild(this TextElement parent, TextElement child)
        {
            var list = parent as List;
            if (list != null)
                AddChild(list, child);

            var listItem = parent as ListItem;
            if (listItem != null)
                AddChild(listItem, child);

            var paragraph = parent as Paragraph;
            if (paragraph != null)
                AddChild(paragraph, child);

            var section = parent as Section;
            if (section != null)
                AddChild(section, child);

            var table = parent as Table;
            if (table != null)
                AddChild(table, child);

            var tableRowGroup = parent as TableRowGroup;
            if (tableRowGroup != null)
                AddChild(tableRowGroup, child);

            var tableRow = parent as TableRow;
            if (tableRow != null)
                AddChild(tableRow, child);

            var tableCell = parent as TableCell;
            if (tableCell != null)
                AddChild(tableCell, child);

            var anchoredBlock = parent as AnchoredBlock;
            if (anchoredBlock != null)
                AddChild(anchoredBlock, child);

            var span = parent as Span;
            if (span != null)
                AddChild(span, child);
        }

        public static void AddChild(this List parent, TextElement child)
        {
            var listItem = child as ListItem;
            if (listItem == null)
                return;

            parent.ListItems.Add(listItem);
        }

        public static void AddChild(this ListItem parent, TextElement child)
        {
            var block = child as Block;
            if (block == null)
                return;

            parent.Blocks.Add(block);
        }

        public static void AddChild(this Paragraph parent, TextElement child)
        {
            var inline = child as Inline;
            if (inline == null)
                return;

            parent.Inlines.Add(inline);
        }

        public static void AddChild(this Section parent, TextElement child)
        {
            var block = child as Block;
            if (block == null)
                return;

            parent.Blocks.Add(block);
        }

        public static void AddChild(this Table parent, TextElement child)
        {
            var rowGroup = child as TableRowGroup;
            if (rowGroup == null)
                return;

            parent.RowGroups.Add(rowGroup);
        }

        public static void AddChild(this TableRowGroup parent, TextElement child)
        {
            var row = child as TableRow;
            if (row == null)
                return;

            parent.Rows.Add(row);
        }

        public static void AddChild(this TableRow parent, TextElement child)
        {
            var cell = child as TableCell;
            if (cell == null)
                return;

            parent.Cells.Add(cell);
        }

        public static void AddChild(this TableCell parent, TextElement child)
        {
            var block = child as Block;
            if (block == null)
                return;

            parent.Blocks.Add(block);
        }

        public static void AddChild(this AnchoredBlock parent, TextElement child)
        {
            var block = child as Block;
            if (block == null)
                return;

            parent.Blocks.Add(block);
        }

        public static void AddChild(this Span parent, TextElement child)
        {
            var inline = child as Inline;
            if (inline == null)
                return;

            parent.Inlines.Add(inline);
        }


        public static void InsertAfter(this TextElement parent, TextElement previousSibling, TextElement child)
        {
            var list = parent as List;
            if (list != null)
                InsertAfter(list, previousSibling, child);

            var listItem = parent as ListItem;
            if (listItem != null)
                InsertAfter(listItem, previousSibling, child);

            var paragraph = parent as Paragraph;
            if (paragraph != null)
                InsertAfter(paragraph, previousSibling, child);

            var section = parent as Section;
            if (section != null)
                InsertAfter(section, previousSibling, child);

            var table = parent as Table;
            if (table != null)
                InsertAfter(table, previousSibling, child);

            var tableRowGroup = parent as TableRowGroup;
            if (tableRowGroup != null)
                InsertAfter(tableRowGroup, previousSibling, child);

            var tableRow = parent as TableRow;
            if (tableRow != null)
                InsertAfter(tableRow, previousSibling, child);

            var tableCell = parent as TableCell;
            if (tableCell != null)
                InsertAfter(tableCell, previousSibling, child);

            var anchoredBlock = parent as AnchoredBlock;
            if (anchoredBlock != null)
                InsertAfter(anchoredBlock, previousSibling, child);

            var span = parent as Span;
            if (span != null)
                InsertAfter(span, previousSibling, child);
        }

        public static void InsertAfter(this List parent, TextElement previousSibling, TextElement child)
        {
            var listItem = child as ListItem;
            if (listItem == null)
                return;

            var previousListItem = previousSibling as ListItem;
            if (previousListItem == null || !parent.ListItems.Contains(previousListItem))
                parent.ListItems.Add(listItem);

            parent.ListItems.InsertAfter(previousListItem, listItem);
        }

        public static void InsertAfter(this ListItem parent, TextElement previousSibling, TextElement child)
        {
            var block = child as Block;
            if (block == null)
                return;

            var previousBlock = previousSibling as Block;
            if (previousBlock == null || !parent.Blocks.Contains(previousBlock))
                parent.Blocks.Add(block);

            parent.Blocks.InsertAfter(previousBlock, block);
        }

        public static void InsertAfter(this Paragraph parent, TextElement previousSibling, TextElement child)
        {
            var inline = child as Inline;
            if (inline == null)
                return;

            var previousInline = previousSibling as Inline;
            if (previousInline == null || !parent.Inlines.Contains(previousInline))
                parent.Inlines.Add(inline);

            parent.Inlines.InsertAfter(previousInline, inline);
        }

        public static void InsertAfter(this Section parent, TextElement previousSibling, TextElement child)
        {
            var block = child as Block;
            if (block == null)
                return;

            var previousBlock = previousSibling as Block;
            if (previousBlock == null || !parent.Blocks.Contains(previousBlock))
                parent.Blocks.Add(block);

            parent.Blocks.InsertAfter(previousBlock, block);
        }

        public static void InsertAfter(this Table parent, TextElement previousSibling, TextElement child)
        {
            var rowGroup = child as TableRowGroup;
            if (rowGroup == null)
                return;

            var previousRowGroup = previousSibling as TableRowGroup;
            if (previousRowGroup == null)
            {
                parent.RowGroups.Add(rowGroup);
                return;
            }

            var previousRowGroupIndex = parent.RowGroups.IndexOf(previousRowGroup);
            if (previousRowGroupIndex == -1)
            {
                parent.RowGroups.Add(rowGroup);
                return;
            }

            parent.RowGroups.Insert(previousRowGroupIndex + 1, rowGroup);
        }

        public static void InsertAfter(this TableRowGroup parent, TextElement previousSibling, TextElement child)
        {
            var row = child as TableRow;
            if (row == null)
                return;

            var previousRow = previousSibling as TableRow;
            if (previousRow == null)
            {
                parent.Rows.Add(row);
                return;
            }

            var previousRowGroupIndex = parent.Rows.IndexOf(previousRow);
            if (previousRowGroupIndex == -1)
            {
                parent.Rows.Add(row);
                return;
            }

            parent.Rows.Insert(previousRowGroupIndex + 1, row);
        }

        public static void InsertAfter(this TableRow parent, TextElement previousSibling, TextElement child)
        {
            var cell = child as TableCell;
            if (cell == null)
                return;

            var previousCell = previousSibling as TableCell;
            if (previousCell == null)
            {
                parent.Cells.Add(cell);
                return;
            }

            var previousRowGroupIndex = parent.Cells.IndexOf(previousCell);
            if (previousRowGroupIndex == -1)
            {
                parent.Cells.Add(cell);
                return;
            }

            parent.Cells.Insert(previousRowGroupIndex + 1, cell);
        }

        public static void InsertAfter(this TableCell parent, TextElement previousSibling, TextElement child)
        {
            var block = child as Block;
            if (block == null)
                return;

            var previousBlock = previousSibling as Block;
            if (previousBlock == null || !parent.Blocks.Contains(previousBlock))
                parent.Blocks.Add(block);

            parent.Blocks.InsertAfter(previousBlock, block);
        }

        public static void InsertAfter(this AnchoredBlock parent, TextElement previousSibling, TextElement child)
        {
            var block = child as Block;
            if (block == null)
                return;

            var previousBlock = previousSibling as Block;
            if (previousBlock == null || !parent.Blocks.Contains(previousBlock))
                parent.Blocks.Add(block);

            parent.Blocks.InsertAfter(previousBlock, block);
        }

        public static void InsertAfter(this Span parent, TextElement previousSibling, TextElement child)
        {
            var inline = child as Inline;
            if (inline == null)
                return;

            var previousInline = previousSibling as Inline;
            if (previousInline == null || !parent.Inlines.Contains(previousInline))
                parent.Inlines.Add(inline);

            parent.Inlines.InsertAfter(previousInline, inline);
        }


        public static void InsertBefore(this TextElement parent, TextElement nextSibling, TextElement child)
        {
            var list = parent as List;
            if (list != null)
                InsertBefore(list, nextSibling, child);

            var listItem = parent as ListItem;
            if (listItem != null)
                InsertBefore(listItem, nextSibling, child);

            var paragraph = parent as Paragraph;
            if (paragraph != null)
                InsertBefore(paragraph, nextSibling, child);

            var section = parent as Section;
            if (section != null)
                InsertBefore(section, nextSibling, child);

            var table = parent as Table;
            if (table != null)
                InsertBefore(table, nextSibling, child);

            var tableRowGroup = parent as TableRowGroup;
            if (tableRowGroup != null)
                InsertBefore(tableRowGroup, nextSibling, child);

            var tableRow = parent as TableRow;
            if (tableRow != null)
                InsertBefore(tableRow, nextSibling, child);

            var tableCell = parent as TableCell;
            if (tableCell != null)
                InsertBefore(tableCell, nextSibling, child);

            var anchoredBlock = parent as AnchoredBlock;
            if (anchoredBlock != null)
                InsertBefore(anchoredBlock, nextSibling, child);

            var span = parent as Span;
            if (span != null)
                InsertBefore(span, nextSibling, child);
        }

        public static void InsertBefore(this List parent, TextElement nextSibling, TextElement child)
        {
            var listItem = child as ListItem;
            if (listItem == null)
                return;

            var nextListItem = nextSibling as ListItem;
            if (nextListItem == null || !parent.ListItems.Contains(nextListItem))
                parent.ListItems.Add(listItem);

            parent.ListItems.InsertBefore(nextListItem, listItem);
        }

        public static void InsertBefore(this ListItem parent, TextElement nextSibling, TextElement child)
        {
            var block = child as Block;
            if (block == null)
                return;

            var nextBlock = nextSibling as Block;
            if (nextBlock == null || !parent.Blocks.Contains(nextBlock))
                parent.Blocks.Add(block);

            parent.Blocks.InsertBefore(nextBlock, block);
        }

        public static void InsertBefore(this Paragraph parent, TextElement nextSibling, TextElement child)
        {
            var inline = child as Inline;
            if (inline == null)
                return;

            var nextInline = nextSibling as Inline;
            if (nextInline == null || !parent.Inlines.Contains(nextInline))
                parent.Inlines.Add(inline);

            parent.Inlines.InsertBefore(nextInline, inline);
        }

        public static void InsertBefore(this Section parent, TextElement nextSibling, TextElement child)
        {
            var block = child as Block;
            if (block == null)
                return;

            var nextBlock = nextSibling as Block;
            if (nextBlock == null || !parent.Blocks.Contains(nextBlock))
                parent.Blocks.Add(block);

            parent.Blocks.InsertBefore(nextBlock, block);
        }

        public static void InsertBefore(this Table parent, TextElement nextSibling, TextElement child)
        {
            var rowGroup = child as TableRowGroup;
            if (rowGroup == null)
                return;

            var nextRowGroup = nextSibling as TableRowGroup;
            if (nextRowGroup == null)
            {
                parent.RowGroups.Add(rowGroup);
                return;
            }

            var nextRowGroupIndex = parent.RowGroups.IndexOf(nextRowGroup);
            if (nextRowGroupIndex == -1)
            {
                parent.RowGroups.Add(rowGroup);
                return;
            }

            parent.RowGroups.Insert(nextRowGroupIndex, rowGroup);
        }

        public static void InsertBefore(this TableRowGroup parent, TextElement nextSibling, TextElement child)
        {
            var row = child as TableRow;
            if (row == null)
                return;

            var nextRow = nextSibling as TableRow;
            if (nextRow == null)
            {
                parent.Rows.Add(row);
                return;
            }

            var nextRowGroupIndex = parent.Rows.IndexOf(nextRow);
            if (nextRowGroupIndex == -1)
            {
                parent.Rows.Add(row);
                return;
            }

            parent.Rows.Insert(nextRowGroupIndex, row);
        }

        public static void InsertBefore(this TableRow parent, TextElement nextSibling, TextElement child)
        {
            var cell = child as TableCell;
            if (cell == null)
                return;

            var nextCell = nextSibling as TableCell;
            if (nextCell == null)
            {
                parent.Cells.Add(cell);
                return;
            }

            var nextRowGroupIndex = parent.Cells.IndexOf(nextCell);
            if (nextRowGroupIndex == -1)
            {
                parent.Cells.Add(cell);
                return;
            }

            parent.Cells.Insert(nextRowGroupIndex, cell);
        }

        public static void InsertBefore(this TableCell parent, TextElement nextSibling, TextElement child)
        {
            var block = child as Block;
            if (block == null)
                return;

            var nextBlock = nextSibling as Block;
            if (nextBlock == null || !parent.Blocks.Contains(nextBlock))
                parent.Blocks.Add(block);

            parent.Blocks.InsertBefore(nextBlock, block);
        }

        public static void InsertBefore(this AnchoredBlock parent, TextElement nextSibling, TextElement child)
        {
            var block = child as Block;
            if (block == null)
                return;

            var nextBlock = nextSibling as Block;
            if (nextBlock == null || !parent.Blocks.Contains(nextBlock))
                parent.Blocks.Add(block);

            parent.Blocks.InsertBefore(nextBlock, block);
        }

        public static void InsertBefore(this Span parent, TextElement nextSibling, TextElement child)
        {
            var inline = child as Inline;
            if (inline == null)
                return;

            var nextInline = nextSibling as Inline;
            if (nextInline == null || !parent.Inlines.Contains(nextInline))
                parent.Inlines.Add(inline);

            parent.Inlines.InsertBefore(nextInline, inline);
        }


        public static IEnumerable<TextElement> GetChildren(this TextElement parent)
        {
            var list = parent as List;
            if (list != null)
                return GetChildren(list);

            var listItem = parent as ListItem;
            if (listItem != null)
                return GetChildren(listItem);

            var paragraph = parent as Paragraph;
            if (paragraph != null)
                return GetChildren(paragraph);

            var section = parent as Section;
            if (section != null)
                return GetChildren(section);

            var table = parent as Table;
            if (table != null)
                return GetChildren(table);

            var tableRowGroup = parent as TableRowGroup;
            if (tableRowGroup != null)
                return GetChildren(tableRowGroup);

            var tableRow = parent as TableRow;
            if (tableRow != null)
                return GetChildren(tableRow);

            var tableCell = parent as TableCell;
            if (tableCell != null)
                return GetChildren(tableCell);

            var anchoredBlock = parent as AnchoredBlock;
            if (anchoredBlock != null)
                return GetChildren(anchoredBlock);

            var span = parent as Span;
            if (span != null)
                return GetChildren(span);

            return null;
        }

        public static IEnumerable<TextElement> GetChildren(this List parent)
        {
            return parent.ListItems;
        }

        public static IEnumerable<TextElement> GetChildren(this ListItem parent)
        {
            return parent.Blocks;
        }

        public static IEnumerable<TextElement> GetChildren(this Paragraph parent)
        {
            return parent.Inlines;
        }

        public static IEnumerable<TextElement> GetChildren(this Section parent)
        {
            return parent.Blocks;
        }

        public static IEnumerable<TextElement> GetChildren(this Table parent)
        {
            return parent.RowGroups;
        }

        public static IEnumerable<TextElement> GetChildren(this TableRowGroup parent)
        {
            return parent.Rows;
        }

        public static IEnumerable<TextElement> GetChildren(this TableRow parent)
        {
            return parent.Cells;
        }

        public static IEnumerable<TextElement> GetChildren(this TableCell parent)
        {
            return parent.Blocks;
        }

        public static IEnumerable<TextElement> GetChildren(this AnchoredBlock parent)
        {
            return parent.Blocks;
        }

        public static IEnumerable<TextElement> GetChildren(this Span parent)
        {
            return parent.Inlines;
        }


    }
}
