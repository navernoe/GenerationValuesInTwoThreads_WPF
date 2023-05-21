using System;
using System.Data;
using WpfApp.DataAccess.Providers;
using WpfApp.UI.Models;

namespace WpfApp.UI.ViewModels;

public class HistoryDataGridModel
{
    public HistoryDataGridModel()
    {
        DataTableCollection = GetDataTable();
    }

    public DataTable DataTableCollection { get; set; }

    private DataTable GetDataTable()
    {
        DataTable dataTable = new DataTable();
        dataTable.Columns.Add(nameof(DataGridGeneratedRow.CarName), typeof(string));
        dataTable.Columns.Add(nameof(DataGridGeneratedRow.DriverName), typeof(string));
        dataTable.Columns.Add(nameof(DataGridGeneratedRow.GeneratedDateTime), typeof(DateTimeOffset));

        return dataTable;
    }
}