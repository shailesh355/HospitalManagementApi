using BaseClass;
using MySql.Data.MySqlClient;
using System.Transactions;
using HospitalManagementApi.Models.BLayer;
using static HospitalManagementApi.Models.BLayer.BlCommon;
using Microsoft.VisualBasic;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlEmpaneled
    {
        readonly DBConnection db = new();
        ReturnClass.ReturnDataTable dt = new();
        public async Task<ReturnClass.ReturnBool> CUDOperation(BlEmpaneled bl)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            MySqlParameter[] pm;
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
                query = @"DELETE FROM empaneled 
                                WHERE hospitalRegNo = @hospitalRegNo";
                pm = new MySqlParameter[]
                   {
                        new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo },
                   };
                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "empaneled");

                foreach (var item in bl.Bl)
                {
                    pm = new MySqlParameter[]
                    {
                        new MySqlParameter("rowId", MySqlDbType.Int64) { Value = item.rowId },
                        new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo },
                        new MySqlParameter("empaneledId", MySqlDbType.Int32) { Value = item.empaneledId },
                        new MySqlParameter("empaneledTypeId", MySqlDbType.Int16) { Value = item.empaneledTypeId },
                        new MySqlParameter("empaneledTypeName", MySqlDbType.VarChar,50) { Value = item.empaneledTypeName},
                        new MySqlParameter("headName", MySqlDbType.VarChar,1000) { Value = item.headName},
                        new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                        new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime }
                    };
                    if (bl.CRUD == (Int16)CRUD.Create)
                    {
                        query = @"INSERT INTO empaneled (hospitalRegNo,empaneledId,empaneledTypeId,empaneledTypeName,headName,entryDateTime,userId)
                                        VALUES (@hospitalRegNo,@empaneledId,@empaneledTypeId,@empaneledTypeName,@headName,@entryDateTime,@userId)";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "empaneled");
                    }
                    else if (bl.CRUD == (Int16)CRUD.Update)
                    {
                        query = @"DELETE FROM empaneled 
                                WHERE hospitalRegNo = @hospitalRegNo";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "empaneled");
                        if (rb.status)
                        {
                            query = @"INSERT INTO empaneled (hospitalRegNo,empaneledId,empaneledTypeId,empaneledTypeName,headName,entryDateTime,userId)
                                        VALUES (@hospitalRegNo,@empaneledId,@empaneledTypeId,@empaneledTypeName,@headName,@entryDateTime,@userId)";
                            rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "empaneled");
                        }
                    }
                }
            }
            return rb;
        }

        public async Task<ReturnClass.ReturnDataTable> GetEmpaneledDetail(Int64 hospitalRegNo, Int16 empaneledTypeId)
        {
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                         new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                         new MySqlParameter("empaneledTypeId", MySqlDbType.Int16) { Value = empaneledTypeId },
                   };
                string qr = @"SELECT em.rowId,em.empaneledId,em.hospitalRegNo,em.empaneledTypeId,em.empaneledTypeName,em.headName
	                                  FROM empaneled AS em 
	                                  WHERE em.empaneledTypeId=@empaneledTypeId AND em.hospitalRegNo=@hospitalRegNo   
			                        ORDER BY em.headName";
                dt = await db.ExecuteSelectQueryAsync(qr, pm);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public async Task<ReturnClass.ReturnBool> CUDDoctorOperation(BlDoctorEmpaneled bl)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            MySqlParameter[] pm;
            if (bl.doctorRegNo == 0)
            {
                rb.status = false;
                rb.message = "Invalid Doctor Registration No !";
                return rb;
            }
            string query = "";
            bool isValidated = true;
            if (isValidated)
            {
                query = @"DELETE FROM doctorempaneled 
                                WHERE doctorRegNo = @doctorRegNo";
                pm = new MySqlParameter[]
                   {
                        new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                   };
                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "empaneled");

                foreach (var item in bl.Bl)
                {
                    pm = new MySqlParameter[]
                    {
                        new MySqlParameter("rowId", MySqlDbType.Int64) { Value = item.rowId },
                        new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                        new MySqlParameter("empaneledId", MySqlDbType.Int32) { Value = item.empaneledId },
                        new MySqlParameter("empaneledTypeId", MySqlDbType.Int16) { Value = item.empaneledTypeId },
                        new MySqlParameter("empaneledTypeName", MySqlDbType.VarChar,50) { Value = item.empaneledTypeName},
                        new MySqlParameter("headName", MySqlDbType.VarChar,1000) { Value = item.headName},
                        new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                        new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime }
                    };
                    if (bl.CRUD == (Int16)CRUD.Create)
                    {
                        query = @"INSERT INTO doctorempaneled (doctorRegNo,empaneledId,empaneledTypeId,empaneledTypeName,headName,entryDateTime,userId)
                                        VALUES (@doctorRegNo,@empaneledId,@empaneledTypeId,@empaneledTypeName,@headName,@entryDateTime,@userId)";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "empaneled");
                    }
                    else if (bl.CRUD == (Int16)CRUD.Update)
                    {
                        query = @"DELETE FROM doctorempaneled 
                                WHERE doctorRegNo = @doctorRegNo";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "empaneled");
                        if (rb.status)
                        {
                            query = @"INSERT INTO doctorempaneled (doctorRegNo,empaneledId,empaneledTypeId,empaneledTypeName,headName,entryDateTime,userId)
                                        VALUES (@doctorRegNo,@empaneledId,@empaneledTypeId,@empaneledTypeName,@headName,@entryDateTime,@userId)";
                            rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "empaneled");
                        }
                    }
                }
            }
            return rb;
        }

        public async Task<ReturnClass.ReturnDataTable> GetDoctorEmpaneledDetail(Int64 doctorRegNo, Int16 empaneledTypeId)
        {
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                         new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo },
                         new MySqlParameter("empaneledTypeId", MySqlDbType.Int16) { Value = empaneledTypeId },
                   };
                string qr = @"SELECT em.rowId,em.empaneledId,em.doctorRegNo,em.empaneledTypeId,em.empaneledTypeName,em.headName
	                                  FROM doctorempaneled AS em 
	                                  WHERE em.empaneledTypeId=@empaneledTypeId AND em.doctorRegNo=@doctorRegNo   
			                        ORDER BY em.headName";
                dt = await db.ExecuteSelectQueryAsync(qr, pm);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }
        public async Task<ReturnClass.ReturnDataTable> GetProviderEmpaneledDetail(Int64 hospitalRegNo, Int16 empaneledTypeId, Int32 empaneledId)
        {
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                         new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                         new MySqlParameter("empaneledTypeId", MySqlDbType.Int16) { Value = empaneledTypeId },
                         new MySqlParameter("empaneledId", MySqlDbType.Int32) { Value = empaneledId },
                   };
                string qr = @"SELECT ep.providerName,DATE_FORMAT(ep.fromDate,'%d/%m/%Y') AS fromDate,DATE_FORMAT(ep.toDate,'%d/%m/%Y') AS toDate
	                                 FROM empaneledprovider AS ep
	                                WHERE ep.empaneledTypeId=@empaneledTypeId AND ep.empaneledId=@empaneledId
		                                AND ep.hospitalRegNo=@hospitalRegNo";
                dt = await db.ExecuteSelectQueryAsync(qr, pm);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }
         
        public async Task<ReturnClass.ReturnBool> CUDProviderEmpaneled(BlProviderEmpaneled bl)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            MySqlParameter[] pm;
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
                    query = @"DELETE FROM empaneledprovider 
                                WHERE hospitalRegNo = @hospitalRegNo AND empaneledTypeId=@empaneledTypeId AND empaneledId=@empaneledId";

                    DateTime fromDate = DateTime.ParseExact(bl.fromDate.Replace('-', '/'), "dd/MM/yyyy", null);
                    bl.fromDate = fromDate.ToString("yyyy/MM/dd");

                    DateTime toDate = DateTime.ParseExact(bl.toDate.Replace('-', '/'), "dd/MM/yyyy", null);
                    bl.toDate = toDate.ToString("yyyy/MM/dd");

                    pm = new MySqlParameter[]
                       {
                        new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo },
                        new MySqlParameter("empaneledTypeId", MySqlDbType.Int16) { Value = bl.empaneledTypeId },
                        new MySqlParameter("empaneledTypeName", MySqlDbType.VarChar,100) { Value = bl.empaneledTypeName},
                        new MySqlParameter("empaneledId", MySqlDbType.Int32) { Value = bl.empaneledId },
                        new MySqlParameter("empaneledName", MySqlDbType.VarChar,100) { Value = bl.empaneledName},
                        new MySqlParameter("providerName", MySqlDbType.VarChar,100) { Value = bl.providerName},
                        new MySqlParameter("fromDate", MySqlDbType.VarChar,100) { Value = bl.fromDate},
                        new MySqlParameter("toDate", MySqlDbType.VarChar,100) { Value = bl.toDate},
                        new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                        new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime }
                       };
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "empaneledprovider");
                    if (rb.status)
                    {
                        query = @"INSERT INTO empaneledprovider (hospitalRegNo,empaneledTypeId,empaneledTypeName,empaneledId,empaneledName,providerName,fromDate,toDate,entryDateTime,userId,clientIp)
                                        VALUES (@hospitalRegNo,@empaneledTypeId,@empaneledTypeName,@empaneledId,@empaneledName,@providerName,@fromDate,@toDate,@entryDateTime,@userId,@clientIp)";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "empaneledprovider");
                    }
                    if(rb.status)
                        ts.Complete();

                }
            }
            return rb;
        }
    }
}
