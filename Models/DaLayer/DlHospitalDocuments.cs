using BaseClass;
using MySql.Data.MySqlClient;
using System.Transactions;
using HospitalManagementApi.Models.BLayer;
using static HospitalManagementApi.Models.BLayer.BlCommon;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlHospitalDocuments
    {
        readonly DBConnection db = new();
        public async Task<ReturnClass.ReturnBool> CUDOperation(BlHospitalDocuments bl)
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
            if (isValidated)
            {
                DateTime licenseExpiryDate = DateTime.ParseExact(bl.licenseExpiryDate.Replace('-', '/'), "dd/MM/yyyy", null);
                bl.licenseExpiryDate = licenseExpiryDate.ToString("yyyy/MM/dd");

                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("hospitalDocumentsId", MySqlDbType.Int32) { Value = bl.hospitalDocumentsId });
                pm.Add(new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo });
                pm.Add(new MySqlParameter("hospitalRegistrationNo", MySqlDbType.VarChar, 20) { Value = bl.hospitalRegistrationNo });
                pm.Add(new MySqlParameter("licenseExpiryDate", MySqlDbType.VarChar, 20) { Value = bl.licenseExpiryDate });
                pm.Add(new MySqlParameter("NABHCertificationLevel", MySqlDbType.VarChar, 50) { Value = bl.NABHCertificationLevel });
                pm.Add(new MySqlParameter("registeredWith", MySqlDbType.VarChar, 100) { Value = bl.registeredWith });
                pm.Add(new MySqlParameter("anyOtherCertification", MySqlDbType.VarChar, 100) { Value = bl.anyOtherCertification });
                pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId });
                pm.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime });
                if (bl.CRUD == (Int16)CRUD.Create)
                {
                    query = @"INSERT INTO hospitaldocuments (hospitalRegNo,hospitalRegistrationNo,licenseExpiryDate,NABHCertificationLevel,registeredWith,anyOtherCertification,entryDateTime,userId)
                                        VALUES (@hospitalRegNo,@hospitalRegistrationNo,@licenseExpiryDate,@NABHCertificationLevel,@registeredWith,@anyOtherCertification,@entryDateTime,@userId)";
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "hospitaldocuments");
                }
                else if (bl.CRUD == (Int16)CRUD.Update)
                {
                    query = @"UPDATE hospitaldocuments 
                                        SET hospitalRegistrationNo = @hospitalRegistrationNo,hospitalRegistrationNo = @hospitalRegistrationNo,licenseExpiryDate=@licenseExpiryDate,
                                            NABHCertificationLevel=@NABHCertificationLevel,registeredWith=@registeredWith,anyOtherCertification=@anyOtherCertification
                                WHERE hospitalDocumentsId = @hospitalDocumentsId";
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "hospitaldocuments");
                }
            }
            return rb;
        }
    }
}
