namespace Interview.UI.Models.JqueryDataTables
{
    
    public class DtColumn
    {

        /// <summary>
        /// Column's data source, as defined by columns.data.
        /// </summary>
        public string data { get; set; }

        /// <summary>
        /// Column's name, as defined by columns.name.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Flag to indicate if this column is searchable (true) or not (false). This is controlled by columns.searchable.
        /// </summary>
        public bool searchable { get; set; }

        /// <summary>
        /// Flag to indicate if this column is orderable (true) or not (false). This is controlled by columns.orderable.
        /// </summary>
        public bool orderable { get; set; }

        /// <summary>
        /// Search value to apply to this specific column.
        /// </summary>
        public DtSearch search { get; set; }

    }

}
