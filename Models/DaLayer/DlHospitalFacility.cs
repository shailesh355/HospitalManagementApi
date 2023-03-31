using BaseClass;
using MySql.Data.MySqlClient;
using System.Transactions;
using HospitalManagementApi.Models.BLayer;
using static HospitalManagementApi.Models.BLayer.BlCommon;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlHospitalFacility
    {
        readonly DBConnection db = new();
        ReturnClass.ReturnDataTable dt = new();

        public async Task<ReturnClass.ReturnBool> CUDOperation(BlHospitalFacility bl)
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
                pm = new MySqlParameter[]
             {
                    new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo },
              };
                query = @"DELETE FROM hospitalfacility 
                                WHERE hospitalRegNo = @hospitalRegNo";
                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "hospitalfacility");

                foreach (var item in bl.Bl)
                {
                    pm = new MySqlParameter[]
                    {
                        new MySqlParameter("facilityId", MySqlDbType.Int32) { Value = item.facilityId },
                        new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo },
                        new MySqlParameter("hospitalFacilityId", MySqlDbType.Int16) { Value = item.hospitalFacilityId },
                        new MySqlParameter("hospitalFacility", MySqlDbType.VarChar, 100) { Value = item.hospitalFacility },
                        new MySqlParameter("remarks", MySqlDbType.VarChar, 100) { Value = item.remarks },
                        new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                        new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime },
                };
                    if (bl.CRUD == (Int16)CRUD.Create)
                    {
                        query = @"INSERT INTO hospitalfacility (hospitalRegNo,hospitalFacilityId,hospitalFacility,remarks,entryDateTime,userId)
                                        VALUES (@hospitalRegNo,@hospitalFacilityId,@hospitalFacility,@remarks,@entryDateTime,@userId)";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "hospitalfacility");
                    }
                    else if (bl.CRUD == (Int16)CRUD.Update)
                    {
                        query = @"UPDATE hospitalfacility 
                                        SET hospitalFacilityId = @hospitalFacilityId,hospitalFacility = @hospitalFacility,
                                            remarks=@remarks
                                WHERE facilityId = @facilityId";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "hospitalfacility");
                    }
                }
            }
            return rb;
        }

        public async Task<ReturnClass.ReturnDataTable> GetHospitalFacilityDetail(Int64 hospitalRegNo)
        {
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                         new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                   };
                string qr = @"
                            SELECT infra.facilityId,infra.hospitalRegNo,infra.hospitalFacilityId,
                            infra.hospitalFacility,infra.remarks,cat.nameEnglish,cat.grouping,cat.category 
		                            FROM hospitalfacility AS infra
			                            INNER JOIN ddlcatlist AS cat ON infra.facilityId = cat.id
			                            AND cat.category IN ('hospitalFacility')
			                            WHERE infra.hospitalRegNo=@hospitalRegNo   
			                            ORDER BY cat.sortOrder";
                dt = await db.ExecuteSelectQueryAsync(qr, pm);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }
    }
}
