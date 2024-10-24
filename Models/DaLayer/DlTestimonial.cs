using BaseClass;
using HospitalManagementApi.Models.BLayer;
using MySql.Data.MySqlClient;
using System.Transactions;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlTestimonial
    {
        readonly DBConnection db = new();
        DlCommon dlcommon = new();
        public async Task<ReturnClass.ReturnBool> SubmitTestimonial(BlTestimonial blTest)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            try
            {
                string query = @"INSERT INTO testimonials (testimonialId,roleType,content,fullName,userId,clientIp)
                                                            VALUES (@testimonialId,@roleType,@content,@fullName,@userId,@clientIp)";
                blTest.testimonialId = await GetTestimonialId();
                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("testimonialId", MySqlDbType.Int64) { Value = blTest.testimonialId });
                pm.Add(new MySqlParameter("roleType", MySqlDbType.Int16) { Value = blTest.roleType });
                pm.Add(new MySqlParameter("content", MySqlDbType.VarChar) { Value = blTest.content });
                pm.Add(new MySqlParameter("fullName", MySqlDbType.String) { Value = blTest.fullName });
                pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blTest.clientIp });
                pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blTest.userId });

                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "testimonials");
                if (rb.status)
                {
                    rb.message = "Testimonial submitted successfully and sent for Approval.";
                    rb.status = true;
                }
                else
                {
                    rb.message = "Failed to submit Testimonial, " + rb.message;
                }
            }
            catch (Exception ex)
            {
                WriteLog.CustomLog("Testimonial Submission", ex.Message.ToString());
            }
            return rb;
        }

        public async Task<ReturnClass.ReturnDataTable> GetTestimonial(Int16 actionStatus)
        {
            string query = @"SELECT tm.testimonialId, tm.roleType, rt.name AS userRole, tm.content, tm.fullName, 
                                    DATE_FORMAT(tm.entryDateTime,'%d/%m/%Y') AS entryDate
                                 FROM testimonials AS tm
                                 LEFT JOIN roletype AS rt ON tm.roleType = rt.roleId
                             WHERE 1 = 1 ";

            if (actionStatus != 0)
                query += " AND tm.actionStatus=@actionStatus ";
            query += " ORDER BY tm.entryDateTime DESC";

            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("actionStatus", MySqlDbType.Int16) { Value = actionStatus });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }

        public async Task<ReturnClass.ReturnDataTable> GetTestimonialById(Int64 testimonialId)
        {
            string query = @"SELECT tm.roleType, rt.name AS userRole, tm.content, tm.fullName,
                                     DATE_FORMAT(tm.entryDateTime,'%d/%m/%Y') AS entryDate
                                 FROM testimonials AS tm
                                 LEFT JOIN roletype AS rt ON tm.roleType = rt.roleId
                             WHERE testimonialId = @testimonialId";

            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("testimonialId", MySqlDbType.Int64) { Value = testimonialId });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }

        public async Task<ReturnClass.ReturnBool> VerifyTestimonial(BlTestimonialVerification verificationDetail)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            Int32 countData = 0; string query = "";
            using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (verificationDetail.collectionOfTestimonialsIds != null)
                {
                    if (verificationDetail.collectionOfTestimonialsIds.Count > 0)
                    {
                        foreach (var item in verificationDetail.collectionOfTestimonialsIds)
                        {
                            query = @"UPDATE testimonials 
                                    SET actionStatus=@actionStatus,actionDate=@actionDate,actionTakerUserId=@userId
                              WHERE testimonialId = @testimonialId";

                            List<MySqlParameter> pm = new();
                            pm.Add(new MySqlParameter("testimonialId", MySqlDbType.Int64) { Value = item.testimonialsId });
                            pm.Add(new MySqlParameter("actionStatus", MySqlDbType.Int16) { Value = verificationDetail.actionStatus });
                            pm.Add(new MySqlParameter("actionTakerUserId", MySqlDbType.Int64) { Value = verificationDetail.userId });
                            pm.Add(new MySqlParameter("actionDate", MySqlDbType.String) { Value = verificationDetail.actionDate });
                            rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "verifytestimonial");
                            if (rb.status)
                                countData = countData + 1;
                        }
                        if (verificationDetail.collectionOfTestimonialsIds.Count == countData)
                        {
                            ts.Complete();
                            rb.status = true;
                            rb.message = "Action taken Successfully.";
                        }
                        else
                        {
                            rb.message = "Unable to take Action.";
                        }
                    }
                }
                else
                {
                    rb.message = "Unable to take Action.";
                }
                return rb;
            }
        }

        public async Task<Int64> GetTestimonialId()
        {
            Int64 testimonialId = 0;
            string query = @"SELECT MAX(IFNULL(tm.testimonialId,0)) AS testimonialId
                                 FROM testimonials AS tm ";

            List<MySqlParameter> pm = new();
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
            {
                testimonialId = Convert.ToInt64(dt.table.Rows[0]["testimonialId"].ToString());
            }
            return testimonialId;
        }
    }
}
