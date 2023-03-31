using BaseClass;
using MySql.Data.MySqlClient;
using System.Transactions;
using HospitalManagementApi.Models.BLayer;
using static HospitalManagementApi.Models.BLayer.BlCommon;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlHospitalSpecialization
    {
        readonly DBConnection db = new();
        ReturnClass.ReturnDataTable dt = new();

        public async Task<ReturnClass.ReturnBool> CUDOperation(BlHospitalSpecialization bl)
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
                query = @"DELETE FROM hospitalspecialization 
                                WHERE hospitalRegNo = @hospitalRegNo";
                pm = new MySqlParameter[]
                   {
                        new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo },
                   };
                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "hospitalspecialization");

                foreach (var item in bl.Bl)
                {
                    pm = new MySqlParameter[]
                    {
                        new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo },
                        new MySqlParameter("specializationTypeId", MySqlDbType.Int16) { Value = item.specializationTypeId },
                        new MySqlParameter("specializationTypeName", MySqlDbType.VarChar,99) { Value = item.specializationTypeName },
                        new MySqlParameter("levelOfCareId", MySqlDbType.Int16) { Value = item.levelOfCareId },
                        new MySqlParameter("levelOfCareName", MySqlDbType.VarChar,99) { Value = item.levelOfCareName },
                        new MySqlParameter("specializationId", MySqlDbType.Int16) { Value = item.specializationId },
                        new MySqlParameter("specializationName", MySqlDbType.VarChar, 100) { Value = item.specializationName },
                        new MySqlParameter("clientIp", MySqlDbType.VarChar,100) { Value = bl.clientIp },
                        new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                        new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime },
                    };
                    //if (bl.CRUD == (Int16)CRUD.Create)
                    //{
                        //query = @"INSERT INTO hospitalspecialization (hospitalRegNo,levelOfCareId,levelOfCareName,specializationTypeId,specializationTypeName,specializationId,specializationName,clientIp,entryDateTime,userId)
                        //                VALUES (@hospitalRegNo,@levelOfCareId,@levelOfCareName,@specializationTypeId,@specializationTypeName,@specializationId,@specializationName,@clientIp,@entryDateTime,@userId)";
                        //rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "hospitalspecialization");
                    //}
                    //else if (bl.CRUD == (Int16)CRUD.Update)
                    //{
                       
                        if (rb.status)
                        {
                            query = @"INSERT INTO hospitalspecialization (hospitalRegNo,levelOfCareId,levelOfCareName,specializationTypeId,specializationTypeName,specializationId,specializationName,clientIp,entryDateTime,userId)
                                        VALUES (@hospitalRegNo,@levelOfCareId,@levelOfCareName,@specializationTypeId,@specializationTypeName,@specializationId,@specializationName,@clientIp,@entryDateTime,@userId)";
                            rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "hospitalspecialization");
                        }
                    //}
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

        public async Task<ReturnClass.ReturnBool> CUDDoctorOperation(BlDoctorSpecialization bl)
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
                query = @"DELETE FROM doctorspecialization 
                                WHERE doctorRegNo = @doctorRegNo";
                pm = new MySqlParameter[]
                   {
                        new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                   };
                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorspecialization");

                foreach (var item in bl.Bl)
                {
                    pm = new MySqlParameter[]
                    {
                        new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                        new MySqlParameter("specializationTypeId", MySqlDbType.Int16) { Value = item.specializationTypeId },
                        new MySqlParameter("specializationTypeName", MySqlDbType.VarChar,99) { Value = item.specializationTypeName },
                        new MySqlParameter("levelOfCareId", MySqlDbType.Int16) { Value = item.levelOfCareId },
                        new MySqlParameter("levelOfCareName", MySqlDbType.VarChar,99) { Value = item.levelOfCareName },
                        new MySqlParameter("specializationId", MySqlDbType.Int16) { Value = item.specializationId },
                        new MySqlParameter("specializationName", MySqlDbType.VarChar, 100) { Value = item.specializationName },
                        new MySqlParameter("clientIp", MySqlDbType.VarChar,100) { Value = bl.clientIp },
                        new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                        new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime },
                    };
                    if (rb.status)
                    {
                        query = @"INSERT INTO doctorspecialization (doctorRegNo,levelOfCareId,levelOfCareName,specializationTypeId,specializationTypeName,specializationId,specializationName,clientIp,entryDateTime,userId)
                                        VALUES (@doctorRegNo,@levelOfCareId,@levelOfCareName,@specializationTypeId,@specializationTypeName,@specializationId,@specializationName,@clientIp,@entryDateTime,@userId)";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorspecialization");
                    }
                    //}
                }
            }
            return rb;
        }

        public async Task<ReturnClass.ReturnDataTable> GetDoctorSpecDetail(Int64 doctorRegNo)
        {
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                         new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo },
                   };
                string qr = @" SELECT hs.doctorSpecializationId,hs.specializationTypeId,hs.specializationTypeName,
			                        hs.specializationId,hs.specializationName,hs.levelOfCareId,hs.levelOfCareName
		                        FROM doctorspecialization AS hs
		                            WHERE hs.doctorRegNo=@doctorRegNo   
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
