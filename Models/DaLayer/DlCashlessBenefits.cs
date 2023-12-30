using BaseClass;
using HospitalManagementApi.Models.BLayer;
using MySql.Data.MySqlClient;
using System.Transactions;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlCashlessBenefits
    {
        readonly DBConnection db = new();
        ReturnClass.ReturnDataTable dt = new();
        public async Task<ReturnClass.ReturnBool> CUDOperation(BlCashlessBenefits bl)
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
                    query = @"SELECT cl.cashlessBenefitsId
		                        FROM cashlessbenefits AS cl
		                    WHERE cl.hospitalRegNo=@hospitalRegNo ";
                    dt = await db.ExecuteSelectQueryAsync(query, pmInner);
                    if (dt.table.Rows.Count > 0)
                    {
                        message = "Data updated successfully.";
                        query = @"INSERT INTO cashlessbenefitslog
                                    SELECT * FROM cashlessbenefits
                                WHERE hospitalRegNo = @hospitalRegNo";
                        rb = await db.ExecuteQueryAsync(query, pmInner, "cashlessbenefitslog");
                        if (rb.status)
                        {
                            query = @"DELETE FROM cashlessbenefits 
                                        WHERE hospitalRegNo = @hospitalRegNo";
                            rb = await db.ExecuteQueryAsync(query, pmInner, "cashlessbenefits");
                        }
                    }
                    query = @"INSERT INTO cashlessbenefits (cashlessBenefitsFacilityId,hospitalRegNo,discountPercent,isWaiver,userId,entryDateTime)
                                    VALUES ";
                    List<MySqlParameter> pm = new();
                    for (int i = 0; i < bl.Bl!.Count; i++)
                    {
                        pm.Add(new MySqlParameter("cashlessBenefitsFacilityId" + (i).ToString(), MySqlDbType.Int64) { Value = bl.Bl[i].cashlessBenefitsFacilityId });
                        pm.Add(new MySqlParameter("hospitalRegNo" + (i).ToString(), MySqlDbType.Int64) { Value = bl.hospitalRegNo });
                        pm.Add(new MySqlParameter("discountPercent" + (i).ToString(), MySqlDbType.Int16) { Value = bl.Bl[i].discountPercent });
                        pm.Add(new MySqlParameter("isWaiver" + (i).ToString(), MySqlDbType.Int16) { Value = bl.Bl[i].isWaiver });
                        pm.Add(new MySqlParameter("userId" + (i).ToString(), MySqlDbType.Int64) { Value = bl.userId });
                        pm.Add(new MySqlParameter("entryDateTime" + (i).ToString(), MySqlDbType.String) { Value = bl.entryDateTime });

                        query += "(@cashlessBenefitsFacilityId" + i.ToString() + ", @hospitalRegNo" + i.ToString() + ", @discountPercent" + i.ToString() + ", @isWaiver" + i.ToString() +
                                 ", @userId" + i.ToString() + ",@entryDateTime" + i.ToString() + ") ,";
                    }
                    query = query.TrimEnd(',');
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "CUDOperation(cashlessbenefits)");
                    if (rb.status)
                    {
                        transaction.Complete();
                        rb.message = message;
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
                string qr = @"  SELECT cat.id,clb.cashlessBenefitsId, clb.hospitalRegNo, clb.discountPercent,clb.cashlessBenefitsFacilityId, cat.nameEnglish AS cashlessBenefitsFacility, cat.grouping, cat.category,
		                             clb.isWaiver, CASE WHEN clb.isWaiver = 1 AND cat.category='waiverOffered' THEN 'Yes' ELSE 'NA' END AS isWaiverYesNo
                                  FROM cashlessbenefits AS clb
                                      INNER JOIN ddlcatlist AS cat ON clb.cashlessBenefitsFacilityId = cat.id
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
