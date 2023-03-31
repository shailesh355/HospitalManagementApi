using BaseClass;
using MySql.Data.MySqlClient;
using System.Transactions;
using HospitalManagementApi.Models.BLayer;
using static HospitalManagementApi.Models.BLayer.BlCommon;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlInfrastructure
    {
        readonly DBConnection db = new();
        ReturnClass.ReturnDataTable dt = new();

        public async Task<ReturnClass.ReturnBool> CUDOperation(BlInfrastructure bl)
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
                foreach (var item in bl.Bl)
                {
                    pm = new MySqlParameter[]
                    {
                        new MySqlParameter("infrastructureId", MySqlDbType.Int32) { Value = item.infrastructureId },
                    new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo },
                     new MySqlParameter("medicalSupportId", MySqlDbType.Int16) { Value = item.medicalSupportId },
                    new MySqlParameter("medicalSupportName", MySqlDbType.VarChar, 50) { Value = item.medicalSupportName },
                    new MySqlParameter("infrastructureFacilitiesId", MySqlDbType.Int16) { Value = item.infrastructureFacilitiesId },
                    new MySqlParameter("infrastructureFacilities", MySqlDbType.VarChar, 50) { Value = item.infrastructureFacilities },
                    new MySqlParameter("remarks", MySqlDbType.VarChar, 100) { Value = item.remarks },
                    new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                    new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime },
                };
                    if (bl.CRUD == (Int16)CRUD.Create)
                    {
                        query = @"INSERT INTO infrastructure (hospitalRegNo,medicalSupportId,medicalSupportName,infrastructureFacilitiesId,infrastructureFacilities,remarks,entryDateTime,userId)
                                        VALUES (@hospitalRegNo,@medicalSupportId,@medicalSupportName,@infrastructureFacilitiesId,@infrastructureFacilities,@remarks,@entryDateTime,@userId)";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "infrastructure");
                    }
                    else if (bl.CRUD == (Int16)CRUD.Update)
                    {
                        query = @"UPDATE infrastructure 
                                        SET medicalSupportId=@medicalSupportId,medicalSupportName=@medicalSupportName,infrastructureFacilitiesId = @infrastructureFacilitiesId,infrastructureFacilities = @infrastructureFacilities,
                                            remarks=@remarks
                                WHERE infrastructureId = @infrastructureId";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "infrastructure");
                    }
                }
            }
            return rb;
        }

        public async Task<ReturnClass.ReturnDataTable> GetInfrastructureDetail(Int64 hospitalRegNo)
        {
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                         new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                   };
                string qr = @"
SELECT infra.infrastructureId,infra.hospitalRegNo,infra.medicalSupportId,infra.medicalSupportName,infra.infrastructureFacilitiesId,
infra.infrastructureFacilities,infra.remarks,cat.nameEnglish,cat.grouping,cat.category 
		FROM infrastructure AS infra
			INNER JOIN ddlcatlist AS cat ON infra.infrastructureId = cat.id
			AND cat.category IN ('medicalInfrastructure','supportServices')
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
