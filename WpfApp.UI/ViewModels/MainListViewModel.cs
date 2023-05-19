using System;
using System.Data;

namespace WpfApp.UI.ViewModels;

public class MainListViewModel
{
    public MainListViewModel()
    {
        DataTableCollection = GetDataTable();
    }

    public DataTable DataTableCollection { get; set; }

    private DataTable GetDataTable()
    {
        DataTable dataTable = new DataTable();
        dataTable.Columns.Add("Name", typeof(string));
        dataTable.Columns.Add("GeneratedDate", typeof(DateTimeOffset));

        return dataTable;
    }
}