using BaseClass;
using MySql.Data.MySqlClient;
using System.Transactions;
using HospitalManagementApi.Models.BLayer;
using static HospitalManagementApi.Models.BLayer.BlCommon;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlCashlessBenefits
    {
        readonly DBConnection db = new();
        ReturnClass.ReturnDataTable dt = new();
        public async Task<ReturnClass.ReturnBool> CUDOperation(BlCashlessBenefits bl)
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
                        new MySqlParameter("cashlessBenefitsId", MySqlDbType.Int32) { Value = item.cashlessBenefitsId },
                        new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo },
                         new MySqlParameter("discountPercent", MySqlDbType.Int16) { Value = item.discountPercent },
                        new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                        new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime }
                    };
                    if (bl.CRUD == (Int16)CRUD.Create)
                    {
                        query = @"INSERT INTO cashlessbenefits (hospitalRegNo,discountPercent,entryDateTime,userId)
                                        VALUES (@hospitalRegNo,@discountPercent,@entryDateTime,@userId)";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "cashlessbenefits");
                    }
                    else if (bl.CRUD == (Int16)CRUD.Update)
                    {
                        query = @"UPDATE cashlessbenefits 
                                        SET discountPercent = @discountPercent
                                WHERE cashlessBenefitsId = @cashlessBenefitsId";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "cashlessbenefits"); 
                    }
                }
            }
                return rb;
        }

            public async Task<ReturnClass.ReturnDataTable> GetCashlessDetail(Int64 hospitalRegNo)
            {
                try
                {
                    MySqlParameter[] pm = new MySqlParameter[]
                       {
                         new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                       };
                    string qr = @"SELECT clb.cashlessBenefitsId, clb.hospitalRegNo, clb.discountPercent, cat.nameEnglish, cat.grouping, cat.category
                                        FROM cashlessbenefits AS clb
                                            INNER JOIN ddlcatlist AS cat ON clb.cashlessBenefitsId = cat.id
                                            AND cat.category IN ('ipdServices','opdServices','waiverOffered')
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
