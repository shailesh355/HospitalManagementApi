using System.Data;
namespace BaseClass
{
    public class Helper
    {
        /// <summary>
        /// Get List Vale pair For Dropdowns and Radio
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<ListValue> GetGenericDropdownList(DataTable dt)
        {
            List<ListValue> lv = new();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lv.Add(new ListValue
                    {
                        value = dr["id"].ToString(),
                        name = dr["name"].ToString(),
                        label = dr["name"].ToString(),
                        type = dt.Columns.Contains("extraField") ? dr["extraField"].ToString() : ""
                    });
                }
            }
            return lv;
        }
    }
}