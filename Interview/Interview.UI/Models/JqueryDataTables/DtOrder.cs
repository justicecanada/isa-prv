namespace Interview.UI.Models.JqueryDataTables
{
    
    public class DtOrder
    {

        /// <summary>
        /// Column to which ordering should be applied.
        /// This is an index reference to the columns array of information that is also submitted to the server.
        /// </summary>
        public int column { get; set; }

        /// <summary>
        /// Ordering direction for this column.
        /// It will be dt-string asc or dt-string desc to indicate ascending ordering or descending ordering, respectively.
        /// </summary>
        public string dir { get; set; }

    }

    public enum DtOrderDir
    {
        asc,
        desc
    }

}
