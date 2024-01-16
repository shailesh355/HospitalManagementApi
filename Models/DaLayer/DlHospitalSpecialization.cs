using BaseClass;
using HospitalManagementApi.Models.BLayer;
using MySql.Data.MySqlClient;
using System.Transactions;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlHospitalSpecialization
    {
        readonly DBConnection db = new();
        ReturnClass.ReturnDataTable dt = new();

        public async Task<ReturnClass.ReturnBool> CUDOperation(BlHospitalSpecialization bl)
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
                    query = @"SELECT hs.hospitalSpecializationId
		                        FROM hospitalspecialization AS hs
		                    WHERE hs.hospitalRegNo=@hospitalRegNo ";
                    dt = await db.ExecuteSelectQueryAsync(query, pmInner);
                    if (dt.table.Rows.Count > 0)
                    {
                        message = "Data updated successfully.";
                        query = @"INSERT INTO hospitalspecializationlog
                                    SELECT * FROM hospitalspecialization
                                WHERE hospitalRegNo = @hospitalRegNo";
                        rb = await db.ExecuteQueryAsync(query, pmInner, "hospitalspecializationlog");
                        if (rb.status)
                        {
                            query = @"DELETE FROM hospitalspecialization 
                                WHERE hospitalRegNo = @hospitalRegNo";
                            rb = await db.ExecuteQueryAsync(query, pmInner, "hospitalspecialization");
                        }
                    }

                    query = @"INSERT INTO hospitalspecialization (hospitalRegNo,specializationTypeId,specializationTypeName,levelOfCareId,levelOfCareName,specializationId,specializationName,clientIp,userId,entryDateTime)
                                    VALUES ";
                    List<MySqlParameter> pm = new();
                    for (int i = 0; i < bl.Bl!.Count; i++)
                    {
                        pm.Add(new MySqlParameter("hospitalRegNo" + (i).ToString(), MySqlDbType.Int64) { Value = bl.hospitalRegNo });
                        pm.Add(new MySqlParameter("specializationTypeId" + (i).ToString(), MySqlDbType.Int16) { Value = bl.Bl[i].specializationTypeId });
                        pm.Add(new MySqlParameter("specializationTypeName" + (i).ToString(), MySqlDbType.VarChar) { Value = bl.Bl[i].specializationTypeName });
                        pm.Add(new MySqlParameter("levelOfCareId" + (i).ToString(), MySqlDbType.Int16) { Value = bl.Bl[i].levelOfCareId });
                        pm.Add(new MySqlParameter("levelOfCareName" + (i).ToString(), MySqlDbType.VarChar) { Value = bl.Bl[i].levelOfCareName });
                        pm.Add(new MySqlParameter("specializationId" + (i).ToString(), MySqlDbType.Int16) { Value = bl.Bl[i].specializationId });
                        pm.Add(new MySqlParameter("specializationName" + (i).ToString(), MySqlDbType.VarChar) { Value = bl.Bl[i].specializationName });
                        pm.Add(new MySqlParameter("clientIp" + (i).ToString(), MySqlDbType.VarChar) { Value = bl.clientIp });
                        pm.Add(new MySqlParameter("userId" + (i).ToString(), MySqlDbType.Int64) { Value = bl.userId });
                        pm.Add(new MySqlParameter("entryDateTime" + (i).ToString(), MySqlDbType.String) { Value = bl.entryDateTime });

                        query += "(@hospitalRegNo" + i.ToString() + ", @specializationTypeId" + i.ToString() + ", @specializationTypeName" + i.ToString() + ", @levelOfCareId" + i.ToString() +
                                 ", @levelOfCareName" + i.ToString() + ", @specializationId" + i.ToString() + ", @specializationName" + i.ToString() +
                                 ", @clientIp" + i.ToString() + ", @userId" + i.ToString() + ",@entryDateTime" + i.ToString() + ") ,";
                    }
                    query = query.TrimEnd(',');
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "CUDOperation(hospitalspecialization)");
                    if (rb.status)
                    {
                        transaction.Complete();
                        rb.message = message;
                    }
                }
            }
            return rb;
        }

        public async Task<ReturnClass.ReturnDataTable> GetHospitalSpecDetail(Int64 hospitalRegNo)
        {
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                         new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                   };
                string qr = @" SELECT hs.hospitalSpecializationId,hs.specializationTypeId,hs.specializationTypeName,
			                        hs.specializationId,hs.specializationName,hs.levelOfCareId,hs.levelOfCareName
		                        FROM hospitalspecialization AS hs
		                            WHERE hs.hospitalRegNo=@hospitalRegNo   
		                        ORDER BY hs.specializationName";
                dt = await db.ExecuteSelectQueryAsync(qr, pm);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

       
    }
}
