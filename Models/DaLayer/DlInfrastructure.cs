using BaseClass;
using HospitalManagementApi.Models.BLayer;
using MySql.Data.MySqlClient;
using System.Transactions;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlInfrastructure
    {
        readonly DBConnection db = new();
        ReturnClass.ReturnDataTable dt = new();

        public async Task<ReturnClass.ReturnBool> CUDOperation(BlInfrastructure bl)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            if (bl.hospitalRegNo == 0)
            {
                rb.status = false;
                rb.message = "Invalid Hospital Registration No !";
                return rb;
            }
            string query = "";
            bool isValidated = true;
            string message = "Data saved successfully.";
            MySqlParameter[] pmInner;
            if (isValidated)
            {
                using (TransactionScope transaction = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    pmInner = new MySqlParameter[]
                    {
                        new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo },
                    };
                    query = @"SELECT infra.infrastructureId
		                        FROM infrastructure AS infra
		                    WHERE infra.hospitalRegNo=@hospitalRegNo ";
                    dt = await db.ExecuteSelectQueryAsync(query, pmInner);
                    if (dt.table.Rows.Count > 0)
                    {
                        message = "Data updated successfully.";
                        query = @"INSERT INTO infrastructurelog
                                    SELECT * FROM infrastructure
                                WHERE hospitalRegNo = @hospitalRegNo";
                        rb = await db.ExecuteQueryAsync(query, pmInner, "infrastructurelog");
                        if (rb.status)
                        {
                            query = @"DELETE FROM infrastructure 
                                WHERE hospitalRegNo = @hospitalRegNo";
                            rb = await db.ExecuteQueryAsync(query, pmInner, "infrastructure");
                        }
                    }
                    query = @" INSERT INTO infrastructure (hospitalRegNo,medicalInfrastructureId,medicalInfrastructure,infrastructureFacilitiesId,infrastructureFacilities,userId,entryDateTime)
                                VALUES ";
                    List<MySqlParameter> pm = new();
                    for (int i = 0; i < bl.Bl!.Count; i++)
                    {
                        pm.Add(new MySqlParameter("hospitalRegNo" + (i).ToString(), MySqlDbType.Int64) { Value = bl.hospitalRegNo });
                        pm.Add(new MySqlParameter("medicalInfrastructureId" + (i).ToString(), MySqlDbType.Int16) { Value = bl.Bl[i].medicalInfrastructureId });
                        pm.Add(new MySqlParameter("medicalInfrastructure" + (i).ToString(), MySqlDbType.VarChar, 50) { Value = bl.Bl[i].medicalInfrastructure });
                        pm.Add(new MySqlParameter("infrastructureFacilitiesId" + (i).ToString(), MySqlDbType.Int16) { Value = bl.Bl[i].infrastructureFacilitiesId });
                        pm.Add(new MySqlParameter("infrastructureFacilities" + (i).ToString(), MySqlDbType.VarChar, 50) { Value = bl.Bl[i].infrastructureFacilities });
                        pm.Add(new MySqlParameter("userId" + (i).ToString(), MySqlDbType.Int64) { Value = bl.userId });
                        pm.Add(new MySqlParameter("entryDateTime" + (i).ToString(), MySqlDbType.String) { Value = bl.entryDateTime });
                        query += "(@hospitalRegNo" + i.ToString() + ", @medicalInfrastructureId" + i.ToString() + ", @medicalInfrastructure" + i.ToString() + ", @infrastructureFacilitiesId" + i.ToString() + ", " +
                                 "@infrastructureFacilities" + i.ToString() + ", @userId" + i.ToString() + ",@entryDateTime" + i.ToString() + ") ,";
                    }
                    query = query.TrimEnd(',');
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "CUDOperation(infrastructure)");
                    if (rb.status)
                    {
                        transaction.Complete();
                        rb.message = message;
                    }
                }
            }
            return rb;
        }

        public async Task<ReturnClass.ReturnDataTable> GetInfrastructureDetail(Int64 hospitalRegNo)
        {
            string query = string.Empty;
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                         new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                   };
                query = @" SELECT infra.infrastructureId,infra.hospitalRegNo,infra.medicalInfrastructureId,infra.medicalInfrastructure,infra.infrastructureFacilitiesId,
		                        infra.infrastructureFacilities,infra.remarks,cat.grouping,cat.category 
		                            FROM infrastructure AS infra
			                            INNER JOIN ddlcatlist AS cat ON infra.medicalInfrastructureId = cat.id
			                            AND cat.category IN ('medicalInfrastructure')
			                    WHERE infra.hospitalRegNo=@hospitalRegNo   
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
