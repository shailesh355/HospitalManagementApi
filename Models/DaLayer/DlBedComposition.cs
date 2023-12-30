using BaseClass;
using HospitalManagementApi.Models.BLayer;
using MySql.Data.MySqlClient;
using System.Transactions;
using static HospitalManagementApi.Models.BLayer.BlCommon;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlBedComposition
    {
        readonly DBConnection db = new();
        ReturnClass.ReturnDataTable dt = new();
        public async Task<ReturnClass.ReturnBool> CUDOperation(BlBedComposition bl)
        {
            MySqlParameter[] pm;
            MySqlParameter[] pmd;
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            if (bl.hospitalRegNo == 0)
            {
                rb.status = false;
                rb.message = "Invalid Hospital Registration No !";
                return rb;
            }
            string query = "";
            bool isValidated = true;
            if (isValidated)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    pmd = new MySqlParameter[]
                    {
                            new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo },
                    };
                    query = @"INSERT INTO bedcompositionlog
                                    SELECT * FROM bedcomposition
                                WHERE hospitalRegNo = @hospitalRegNo ";
                    rb = await db.ExecuteQueryAsync(query, pmd, "bedcomposition");
                    query = @"DELETE FROM bedcomposition 
                                    WHERE hospitalRegNo = @hospitalRegNo";
                    rb = await db.ExecuteQueryAsync(query, pmd.ToArray(), "bedcomposition");
                    if (bl.CRUD == (Int16)CRUD.Create)
                    {
                        Int16 counter = 0;
                        foreach (var item in bl.Bl!)
                        {
                            pm = new MySqlParameter[]
                            {
                                new MySqlParameter("bedCompositionId", MySqlDbType.Int32) { Value = item.bedCompositionId },
                                new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo },
                                new MySqlParameter("noOfBeds", MySqlDbType.Int16) { Value = item.noOfBeds },
                                new MySqlParameter("rentPerDay", MySqlDbType.Int16) { Value = item.rentPerDay },
                                new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                                new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime }
                            };
                            query = @"INSERT INTO bedcomposition (bedCompositionId,hospitalRegNo,noOfBeds,rentPerDay,entryDateTime,userId)
                                        VALUES (@bedCompositionId,@hospitalRegNo,@noOfBeds,@rentPerDay,@entryDateTime,@userId)";
                            rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "bedcomposition");
                            if (rb.status)
                                counter++;
                            //else if (bl.CRUD == (Int16)CRUD.Update)
                            //{
                            //    query = @"UPDATE bedcomposition 
                            //                    SET noOfBeds = @noOfBeds,rentPerDay = @rentPerDay
                            //            WHERE bedCompositionId = @bedCompositionId";
                            //    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "bedcomposition");
                            //}
                        }
                        if (bl.Bl.Count == counter)
                        {
                            ts.Complete();
                        }
                    }
                }
            }
            return rb;
        }

        public async Task<ReturnClass.ReturnDataTable> GetBedCompositionDetail(Int64 hospitalRegNo)
        {
            string query = string.Empty;
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                     new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                   };
                query = @"SELECT bc.bedCompositionId,bc.hospitalRegNo,bc.noOfBeds,bc.rentPerDay,cat.nameEnglish 
		                        FROM bedcomposition AS bc
			                        INNER JOIN ddlcatlist AS cat ON bc.bedCompositionId = cat.id
			                        AND cat.category='bedComposition' 
		                        WHERE bc.hospitalRegNo=@hospitalRegNo   
			                        ORDER BY cat.sortOrder";
                dt = await db.ExecuteSelectQueryAsync(query, pm);
            }
            catch (Exception ex)
            {
                WriteLog.Error(" Query: " + query + "\n   error - ", ex);
                dt.status = false;
                dt.message = ex.Message;
            }
            return dt;
        }
    }
}
