using BaseClass;
using MySql.Data.MySqlClient;
using System.Transactions;
using HospitalManagementApi.Models.BLayer;
using static HospitalManagementApi.Models.BLayer.BlCommon;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlMainContact
    {
        readonly DBConnection db = new();
        ReturnClass.ReturnDataTable dt = new();
        public async Task<ReturnClass.ReturnBool> CUDOperation(BlMainContact bl)
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
            //bool isExists = await CheckMobileExistAsync(item.mobileNo, "INSERT", (Int64)bl.hospitalRegNo);
            //if (!isExists)
            //{
            //    isExists = await CheckEmailExistAsync(item.emailId, "INSERT", (Int64)bl.hospitalRegNo);
            //    if (!isExists)
            //    { }
            //    else
            //    {
            //        rb.message = " Email-Id has Already Used For Main Contact!!";
            //    }
            //}
            //else
            //{
            //    rb.message = " Mobile no. has Already Used For  Main Contact!!";
            //}
            foreach (var item in bl.Bl)
            {
                pm = new MySqlParameter[]
                {
                    new MySqlParameter("mainContactId", MySqlDbType.Int32) { Value = item.mainContactId },
                    new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo },
                    new MySqlParameter("designationId", MySqlDbType.Int16) { Value = item.designationId },
                    new MySqlParameter("designationName", MySqlDbType.VarChar, 99) { Value = item.designationName },
                    new MySqlParameter("contactPersonName", MySqlDbType.VarChar, 99) { Value = item.contactPersonName },
                    new MySqlParameter("mobileNo", MySqlDbType.VarChar, 10) { Value = item.mobileNo },
                    new MySqlParameter("emailId", MySqlDbType.VarChar, 99) { Value = item.emailId },
                    new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                    new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime },
                };

                if (bl.CRUD == (Int16)CRUD.Create)
                {
                    query = @"INSERT INTO maincontact (hospitalRegNo,designationId,designationName,contactPersonName,mobileNo,emailId,entryDateTime,userId)
                                        VALUES (@hospitalRegNo,@designationId,@designationName,@contactPersonName, @mobileNo, @emailId,@entryDateTime,@userId)";
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "maincontact");
                }
                else if (bl.CRUD == (Int16)CRUD.Update)
                {
                    query = @"UPDATE maincontact 
                                        SET designationId = @designationId,designationName = @designationName,contactPersonName = @contactPersonName,
                                            mobileNo = @mobileNo, emailId = @emailId 
                                WHERE mainContactId = @mainContactId";
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "maincontact");
                }
            }

            return rb;
        }

        public async Task<bool> CheckEmailExistAsync(string emailId, string transType, Int64 hospitalRegNo)
        {
            bool isAccountExists = false;
            string query = @"SELECT u.emailId
                            FROM maincontact u
                            WHERE u.emailId = @emailId ";
            if (transType == "UPDATE")
            {
                query = query + " AND u.hospitalRegNo!=@hospitalRegNo ";
            }
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("emailId", MySqlDbType.VarString) { Value = emailId });
            pm.Add(new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
                isAccountExists = true;
            return isAccountExists;
        }
        public async Task<bool> CheckMobileExistAsync(string mobileNo, string transType, Int64 hospitalRegNo)
        {
            bool isAccountExists = false;
            string query = @"SELECT u.mobileNo
                            FROM maincontact u
                            WHERE u.mobileNo = @mobileNo ";
            if (transType == "UPDATE")
            {
                query = query + " AND u.hospitalRegNo!=@hospitalRegNo ";
            }
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("mobileNo", MySqlDbType.VarString) { Value = mobileNo });
            pm.Add(new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
                isAccountExists = true;
            return isAccountExists;
        }
        public async Task<ReturnClass.ReturnDataTable> GetMainContactDetail(Int64 hospitalRegNo)
        {
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                         new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                   };
                string qr = @"
SELECT mc.mainContactId,mc.hospitalRegNo,mc.designationId,mc.designationName,
		mc.contactPersonName,mc.mobileNo,mc.emailId,cat.nameEnglish,cat.grouping,cat.category 
		FROM maincontact AS mc
			INNER JOIN ddlcatlist AS cat ON mc.mainContactId = cat.id
			AND cat.category ='designation'
			WHERE clb.hospitalRegNo=@hospitalRegNo   
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
