using BaseClass;
using HospitalManagementApi.Models.BLayer;
using MySql.Data.MySqlClient;
using static HospitalManagementApi.Models.BLayer.BlCommon;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlCategory
    {
        readonly DBConnection db = new();
        ReturnClass.ReturnDataTable dt = new();
        /// <summary>
        /// add labels in menu list
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnClass.ReturnBool> CUDCatOperation(BlMenuLabel bl)
        {
            MySqlParameter[] pm;
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            string query = "";
            string category, categoryname, description;
            if (bl.CRUD == (Int16)CRUD.Create)
            {
                string id, sortOrder;
                dt = await GetMaxIdSort((Int32)bl.groupingId);
                id = Convert.ToString(Convert.ToInt32(dt.table.Rows[0]["id"] == null ? 0 : dt.table.Rows[0]["id"]) + 1);
                sortOrder = Convert.ToString(Convert.ToInt32(dt.table.Rows[0]["sortOrder"] == null ? 0 : dt.table.Rows[0]["sortOrder"]) + 1);
                category = Convert.ToString(dt.table.Rows[0]["category"]);
                categoryname = Convert.ToString(dt.table.Rows[0]["categoryname"]);

                pm = new MySqlParameter[]
            {
                new MySqlParameter("id", MySqlDbType.Int32) { Value = id },
                new MySqlParameter("groupingId", MySqlDbType.Int32) { Value = bl.groupingId },
                new MySqlParameter("sortOrder", MySqlDbType.Int32) { Value = sortOrder },
                new MySqlParameter("nameEnglish", MySqlDbType.VarChar,300) { Value = bl.labelname },
                new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime },
                new MySqlParameter("category", MySqlDbType.String) { Value = category },
                new MySqlParameter("categoryname", MySqlDbType.String) { Value = categoryname },
                new MySqlParameter("description", MySqlDbType.String) { Value = bl.description },
            };

                query = @"INSERT INTO ddlcatlist(id,isStateSpecific,stateId,nameEnglish,nameLocal,category,categoryName
                        ,grouping,sortOrder,active,hideFromPublicAPI,verificationStatus,verifiedBy,entryDateTime,userId,description)
                                        VALUES (@id,0,22,@nameEnglish,@nameEnglish,@category,@categoryName,@groupingId,
                            @sortOrder,1,0,0,0,@entryDateTime,@userId,@description)";
                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "ddlcatlist");
            }
            else if (bl.CRUD == (Int16)CRUD.Update)
            {
                pm = new MySqlParameter[]
            {
                new MySqlParameter("id", MySqlDbType.Int32) { Value = bl.id },
                new MySqlParameter("groupingId", MySqlDbType.Int32) { Value = bl.groupingId },
                new MySqlParameter("nameEnglish", MySqlDbType.VarChar,300) { Value = bl.labelname },
                new MySqlParameter("nameLocal", MySqlDbType.VarChar,300) { Value = bl.labelname },
                new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime },
                new MySqlParameter("description", MySqlDbType.String) { Value = bl.description },
            };
                query = @"UPDATE ddlcatlist 
                                        SET nameLocal = @nameLocal,nameEnglish = @nameEnglish,lastUpdate=@entryDateTime
                                    ,description=@description
                                WHERE id = @id AND grouping=@groupingId";
                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "ddlcatlist");
            }
            return rb;
        }
        public async Task<ReturnClass.ReturnDataTable> GetMaxIdSort(Int32 groupingId)
        {
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                         new MySqlParameter("groupingId", MySqlDbType.Int32) { Value = groupingId },
                   };
                string qr = @"SELECT MAX(cast(id AS INT)) AS id, MAX(cast(sortorder AS INT)) AS sortorder
                                    ,category,categoryname
                                from ddlcatlist 
                                WHERE grouping = @groupingId";
                dt = await db.ExecuteSelectQueryAsync(qr, pm);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        /// <summary>
        /// Get List of Menu
        /// </summary>
        /// <returns></returns>
        public async Task<List<ListValue>> GetMenu()
        {
            string query = @"SELECT * FROM 
                                (SELECT DISTINCT grouping AS id,categoryname AS name,category AS extraField
                                    FROM ddlcatlist
                                    WHERE category IN ('bedComposition','corporate','designation','empaneled','hospitalSpecialization'
                                    ,'hospitalSpecialization','ipdServices','levelOfCare','medicalInfrastructure','medicalInfrastructure2','natureEntity',
                                    'opdServices','organizationType','providerType','specializationType','supportServices','waiverOffered')
                                     UNION 
                                SELECT DISTINCT id AS id,nameenglish  AS name,namelocal AS extraField
                                    FROM ddlcatlist
                                    WHERE category IN ('empaneled') 
                                ) A ORDER BY name";
            dt = await db.ExecuteSelectQueryAsync(query);
            List<ListValue> lv = Helper.GetGenericDropdownList(dt.table);
            return lv;
        }
    }

}
