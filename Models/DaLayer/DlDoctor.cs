using BaseClass;
using DmfPortalApi.Models.AppClass;
using HospitalManagementApi.Models.BLayer;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using System.Xml.Linq;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlDoctor
    {
        readonly DBConnection db = new();
        ReturnClass.ReturnDataTable dt = new();
        DlCommon dl = new();

        public async Task<ReturnClass.ReturnBool> RegisterNewDoctor(BlDoctor blDoctor)
        {
            DlCommon dlcommon = new();
            string pass = "";
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            try
            {
                if (blDoctor.doctorRegNo == null)
                    blDoctor.doctorRegNo = 0;
                bool isExists = await CheckMobileExistAsync(blDoctor.mobileNo!, "INSERT", (Int64)blDoctor.doctorRegNo);
                bool isExistsMail = await dl.CheckMailExistOnUserAsync(blDoctor.emailId!);
                if (isExists)
                {
                    rb.message = " Mobile no. has Already Used For Registration!!";
                    rb.status = false;
                    return rb;
                }
                else if (isExistsMail)
                {
                    rb.message = " EmailId already registered !";
                    rb.status = false;
                    return rb;
                }
                isExists = await CheckEmailExistAsync(blDoctor.emailId!, "INSERT", (Int64)blDoctor.doctorRegNo);
                if (!isExists)
                {
                    using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        BlCity blcity = new();
                        blcity.districtId = (Int16)(string.IsNullOrEmpty(blDoctor.districtId.ToString()) ? 0 : blDoctor.districtId);
                        blcity.stateId = (Int16)blDoctor.stateId;
                        blcity.cityId = (Int32)blDoctor.cityId;
                        blcity.cityNameEnglish = blDoctor.cityName;
                        blcity.clientIp = blDoctor.clientIp;
                        //blcity.userId = blDoctor.userId;
                        blcity.entryDateTime = blDoctor.entryDateTime;
                        blDoctor.cityId = await dlcommon.ReturnCity(blcity);

                        string query = @"INSERT INTO doctorregistration (doctorRegNo,doctorNameEnglish,doctorNameLocal,stateId,districtId,address,mobileNo,
                                                 emailId,active,isVerified,verificationDate,verifiedByLoginId,registrationStatus,userId, 
                                                entryDateTime, clientIp,registrationYear,cityName,cityId,firstName,middleName,lastName)
                                        VALUES (@doctorRegNo,@doctorNameEnglish,@doctorNameLocal,@stateId, @districtId, @address, @mobileNo,
                                                 @emailId,@active, @isVerified,@verificationDate,@verifiedByLoginId,@registrationStatus,@userId, 
                                                @entryDateTime,@clientIp,@registrationYear,@cityName,@cityId,@firstName,@middleName,@lastName)";
                        blDoctor.doctorRegNo = await GetDoctorId((int)blDoctor.registrationYear!);


                        List<MySqlParameter> pm = new();
                        pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = blDoctor.doctorRegNo });
                        pm.Add(new MySqlParameter("doctorNameEnglish", MySqlDbType.String) { Value = blDoctor.doctorNameEnglish });
                        pm.Add(new MySqlParameter("doctorNameLocal", MySqlDbType.String) { Value = blDoctor.doctorNameLocal });
                        pm.Add(new MySqlParameter("stateId", MySqlDbType.Int16) { Value = blDoctor.stateId });
                        pm.Add(new MySqlParameter("districtId", MySqlDbType.Int16) { Value = blcity.districtId });
                        pm.Add(new MySqlParameter("cityId", MySqlDbType.Int32) { Value = blDoctor.cityId });
                        pm.Add(new MySqlParameter("address", MySqlDbType.String) { Value = blDoctor.address });
                        pm.Add(new MySqlParameter("mobileNo", MySqlDbType.String) { Value = blDoctor.mobileNo });
                        pm.Add(new MySqlParameter("emailId", MySqlDbType.String) { Value = blDoctor.emailId });
                        pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = Active.Yes });
                        pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = blDoctor.isVerified });
                        pm.Add(new MySqlParameter("verifiedByLoginId", MySqlDbType.Int64) { Value = blDoctor.userId });
                        pm.Add(new MySqlParameter("registrationStatus", MySqlDbType.Int16) { Value = RegistrationStatus.Pending });
                        pm.Add(new MySqlParameter("registrationYear", MySqlDbType.Int32) { Value = blDoctor.registrationYear });
                        pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blDoctor.clientIp });
                        pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blDoctor.userId });
                        pm.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = blDoctor.entryDateTime });
                        pm.Add(new MySqlParameter("registrationDate", MySqlDbType.String) { Value = blDoctor.entryDateTime });
                        pm.Add(new MySqlParameter("isDisabled", MySqlDbType.Int16) { Value = Active.No });
                        pm.Add(new MySqlParameter("userRole", MySqlDbType.Int16) { Value = UserRole.Doctor });
                        pm.Add(new MySqlParameter("isSingleWindowUser", MySqlDbType.Int16) { Value = Active.No });
                        pm.Add(new MySqlParameter("modificationType", MySqlDbType.Int16) { Value = Active.No });
                        pm.Add(new MySqlParameter("userTypeCode", MySqlDbType.Int16) { Value = Active.No });
                        //pm.Add(new MySqlParameter("Password", MySqlDbType.String) { Value = hash_Pass });
                        pm.Add(new MySqlParameter("changePassword", MySqlDbType.Int16) { Value = Active.No });
                        pm.Add(new MySqlParameter("cityName", MySqlDbType.String) { Value = blDoctor.cityName });
                        pm.Add(new MySqlParameter("firstName", MySqlDbType.VarChar, 100) { Value = blDoctor.firstName });
                        pm.Add(new MySqlParameter("middleName", MySqlDbType.VarChar, 100) { Value = blDoctor.middleName });
                        pm.Add(new MySqlParameter("lastName", MySqlDbType.VarChar, 100) { Value = blDoctor.lastName });

                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorregistration");
                        //if (rb.status)
                        //{
                        //    query = @"INSERT INTO userlogin
                        //                    (userName,userId,emailId,password,changePassword,active,isDisabled,
                        //                    clientIp,userRole,registrationYear,isSingleWindowUser,modificationType,userTypeCode)
                        //            VALUES (@doctorNameEnglish,@doctorRegNo,@emailId,@password, @changePassword, @active, @isDisabled,
                        //                @clientIp,@userRole, @registrationYear,@isSingleWindowUser,@modificationType,@userTypeCode)";
                        //    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "InsertUserLogin");
                        //}
                        if (rb.status)
                        {
                            ts.Complete();
                            rb.error = pass;
                        }

                    }
                }
                else
                {
                    rb.message = " Email-Id has Already Used For Registration!!";
                }
            }
            catch (Exception ex)
            {
                WriteLog.CustomLog("Doctore Registration", ex.Message.ToString());
            }
            return rb;
        }

        // public async Task<ReturnClass.ReturnBool> UpdateNewDoctor(BlDoctor blDoctor)
        // {
        //     ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
        //     bool isExists = await CheckMobileExistAsync(blDoctor.mobileNo, "UPDATE", (Int64)blDoctor.doctorRegNo);

        //     if (!isExists)
        //     {
        //         isExists = await CheckEmailExistAsync(blDoctor.emailId, "UPDATE", (Int64)blDoctor.doctorRegNo);
        //         if (!isExists)
        //         {
        //             List<MySqlParameter> pm = new();
        //             pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = blDoctor.doctorRegNo });
        //             pm.Add(new MySqlParameter("doctorNameEnglish", MySqlDbType.String) { Value = blDoctor.doctorNameEnglish });
        //             pm.Add(new MySqlParameter("doctorNameLocal", MySqlDbType.String) { Value = blDoctor.doctorNameLocal });
        //             pm.Add(new MySqlParameter("stateId", MySqlDbType.Int16) { Value = blDoctor.stateId });
        //             pm.Add(new MySqlParameter("districtId", MySqlDbType.Int16) { Value = blDoctor.districtId });
        //             pm.Add(new MySqlParameter("address", MySqlDbType.String) { Value = blDoctor.address });
        //             pm.Add(new MySqlParameter("mobileNo", MySqlDbType.String) { Value = blDoctor.mobileNo });
        //             pm.Add(new MySqlParameter("emailId", MySqlDbType.String) { Value = blDoctor.emailId });
        //             pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = blDoctor.active });
        //             pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = (int)blDoctor.isVerified });
        //             pm.Add(new MySqlParameter("verifiedByLoginId", MySqlDbType.Int64) { Value = blDoctor.userId });
        //             pm.Add(new MySqlParameter("registrationStatus", MySqlDbType.Int16) { Value = (Int16)RegistrationStatus.Approved });
        //             pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blDoctor.clientIp });
        //             pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blDoctor.userId });
        //             pm.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = blDoctor.entryDateTime });
        //             string query = @"INSERT INTO Doctorregistrationlog
        //                              SELECT * FROM  doctorregistration h
        //                                WHERE h.doctorRegNo=@doctorRegNo";
        //             using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //             {
        //                 rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "Doctorlog");
        //                 query = @"UPDATE doctorregistration 
        //                                          SET doctorNameEnglish=@doctorNameEnglish,doctorNameLocal=@doctorNameLocal,stateId=@stateId,
        //                                         districtId=@districtId,address=@address,mobileNo=@mobileNo,emailId=@emailId,active=@active,isVerified=@isVerified,
        //                                         verifiedByLoginId=@verifiedByLoginId,registrationStatus=@registrationStatus,userId=@userId, 
        //                                         entryDateTime=@entryDateTime,clientIp=@clientIp WHERE  doctorRegNo=@doctorRegNo";
        //                 rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "DoctorUpdate");
        //                 if (rb.status == true)
        //                 {
        //                     ts.Complete();
        //                 }
        //             }
        //         }
        //         else
        //         {
        //             rb.message = " Email-Id has Already Used For Registration!!";
        //         }
        //     }
        //     else
        //     {
        //         rb.message = " Mobile no. has Already Used For Registration!!";
        //     }
        //     return rb;
        // }
        // /// <summary>
        // /// Returns 12 digit DoctorId id based on year
        // /// </summary>
        // /// <param name="registrationYear"></param>
        // /// <returns></returns>
        public async Task<Int64> GetDoctorId(int registrationYear)
        {
            string DoctorId = "0";
            try
            {
                string qr = @"SELECT IFNULL(MAX(SUBSTRING(ur.doctorRegNo,6,12)),0) + 1 AS DoctorId
        	                    FROM doctorregistration ur 
                            WHERE ur.registrationYear = @registrationYear";

                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("registrationYear", MySqlDbType.Int32) { Value = registrationYear });

                ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(qr, pm.ToArray());
                if (dt.table.Rows.Count > 0)
                {
                    DoctorId = dt.table.Rows[0]["DoctorId"].ToString();
                    DoctorId = ((int)idPrefix.doctorRegistrationId).ToString() + registrationYear.ToString() + DoctorId.PadLeft(7, '0');
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return Convert.ToInt64(DoctorId);
        }
        public async Task<bool> CheckEmailExistAsync(string emailId, string transType, Int64 doctorRegNo)
        {
            bool isAccountExists = false;
            string query = @"SELECT u.emailId
                             FROM doctorregistration u
                             WHERE u.emailId = @emailId ";
            if (transType == "UPDATE")
            {
                query = query + " AND u.doctorRegNo!=@doctorRegNo ";
            }
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("emailId", MySqlDbType.VarString) { Value = emailId });
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
                isAccountExists = true;
            return isAccountExists;
        }
        public async Task<bool> CheckMobileExistAsync(string mobileNo, string transType, Int64 doctorRegNo)
        {
            bool isAccountExists = false;
            string query = @"SELECT u.mobileNo
                                    FROM doctorregistration u
                               WHERE u.mobileNo = @mobileNo ";
            if (transType == "UPDATE")
            {
                query = query + " AND u.doctorRegNo!=@doctorRegNo ";
            }
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("mobileNo", MySqlDbType.VarString) { Value = mobileNo });
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
                isAccountExists = true;
            return isAccountExists;
        }

        // public async Task<ReturnClass.ReturnDataTable> GetAllDoctorList(Int16 vId)
        // {
        //     string query = @"SELECT  h.doctorRegNo,h.doctorNameEnglish,h.doctorNameLocal,h.stateId,h.districtId,h.address,h.mobileNo,
        //                              h.emailId,h.active,s.stateNameEnglish AS stateName,d.districtNameEnglish AS districtName
        //                         FROM doctorregistration h
        //                          JOIN state s ON s.stateId=h.stateId
        //              JOIN district d ON d.districtId=h.districtId
        //                     WHERE h.isVerified=@isVerified";
        //     List<MySqlParameter> pm = new();
        //     pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = vId });

        //     ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
        //     return dt;
        // }
        // public async Task<ReturnClass.ReturnDataTable> GetDoctorById(Int64 doctorRegNo)
        // {
        //     string query = @"SELECT  h.doctorRegNo,h.doctorNameEnglish,h.doctorNameLocal,h.stateId,h.districtId,h.address,h.mobileNo,
        //                              h.emailId,h.active,s.stateNameEnglish AS stateName,d.districtNameEnglish AS districtName
        //                         FROM doctorregistration h
        //                          JOIN state s ON s.stateId=h.stateId
        //              JOIN district d ON d.districtId=h.districtId
        //                     WHERE h.doctorRegNo=@doctorRegNo ";
        //     List<MySqlParameter> pm = new();
        //     pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });

        //     ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
        //     return dt;
        // }

        public static String sha256_hash(String value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        // public async Task<ReturnClass.ReturnBool> VerifyDoctor(VerificationDetail verificationDetail)
        // {
        //     ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
        //     Int32 countData = 0;
        //     string pass = "";
        //     if (verificationDetail.VerificationDoctor.Count != 0)
        //     {
        //         foreach (var item in verificationDetail.VerificationDoctor)
        //         {
        //             if (item.registrationStatus == RegistrationStatus.Approved)
        //             {
        //                 item.isVerified = YesNo.Yes;
        //             }
        //             else
        //             {
        //                 item.isVerified = YesNo.No;
        //             }
        //             bool isofficeExists = await CheckVerifyDoctor(item.doctorRegNo, (Int16)item.isVerified);
        //             if (!isofficeExists)
        //             {
        //                 string query = @"UPDATE doctorregistration 
        //                      SET isVerified=@isVerified,clientIp=@clientIp,verificationDate=@verificationDate,
        //                         verifiedByLoginId=@verifiedByLoginId,registrationStatus=@registrationStatus,active=@active 
        //                       WHERE doctorRegNo=@doctorRegNo";


        //                 pass = "HRP" + getrandom();
        //                 string hash_Pass = sha256_hash(pass);
        //                 List<MySqlParameter> pm = new();
        //                 pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = item.doctorRegNo });
        //                 pm.Add(new MySqlParameter("registrationStatus", MySqlDbType.Int16) { Value = (int)item.registrationStatus });
        //                 pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = (int)item.isVerified });
        //                 pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = verificationDetail.clientIp });
        //                 pm.Add(new MySqlParameter("verifiedByLoginId", MySqlDbType.Int64) { Value = verificationDetail.userId });
        //                 pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = (int)item.isVerified });
        //                 pm.Add(new MySqlParameter("officeMappingKey", MySqlDbType.Int32) { Value = 0 });
        //                 pm.Add(new MySqlParameter("registrationDate", MySqlDbType.String) { Value = verificationDetail.date });
        //                 pm.Add(new MySqlParameter("registrationYear", MySqlDbType.Int32) { Value = DateTime.Now.Date.Year });
        //                 pm.Add(new MySqlParameter("changePassword", MySqlDbType.Int16) { Value = (int)Active.No });
        //                 pm.Add(new MySqlParameter("active1", MySqlDbType.Int16) { Value = (int)Active.Yes });
        //                 pm.Add(new MySqlParameter("isDisabled", MySqlDbType.Int16) { Value = (int)Active.No });
        //                 pm.Add(new MySqlParameter("userRole", MySqlDbType.Int16) { Value = (int)UserRole.Nodal });
        //                 pm.Add(new MySqlParameter("isSingleWindowUser", MySqlDbType.Int16) { Value = (int)Active.No });
        //                 pm.Add(new MySqlParameter("modificationType", MySqlDbType.Int16) { Value = (int)Active.No });
        //                 pm.Add(new MySqlParameter("userTypeCode", MySqlDbType.Int16) { Value = (int)Active.No });
        //                 pm.Add(new MySqlParameter("Password", MySqlDbType.String) { Value = hash_Pass });
        //                 using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //                 {
        //                     rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "VerifyDoctor");
        //                     if (rb.status == true && item.registrationStatus == RegistrationStatus.Approved && item.isVerified == YesNo.Yes)
        //                     {
        //                         query = @"INSERT INTO userlogin
        //                                     (userName,userId,emailId,password,changePassword,active,isDisabled,
        //                                     clientIp,userRole,registrationYear,isSingleWindowUser,modificationType,userTypeCode)
        //                                 SELECT h.doctorNameEnglish,h.doctorRegNo,h.emailId,@Password,@changePassword,
        //                                 @active1,@isDisabled,@clientIp,@userRole,@registrationYear,@isSingleWindowUser,
        //                                 @modificationType,@userTypeCode
        //                                     FROM  doctorregistration h 
        //                                   WHERE h.doctorRegNo=@doctorRegNo";
        //                         rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "InsertUserLogin");
        //                         if (rb.status)
        //                         {
        //                             ts.Complete();
        //                             countData = countData + 1;
        //                             rb.error = pass;
        //                         }
        //                     }
        //                     else if (rb.status == true && item.registrationStatus == RegistrationStatus.Reject)
        //                     {
        //                         ts.Complete();
        //                         countData = countData + 1;
        //                         rb.error = string.Empty;
        //                     }


        //                 }
        //             }
        //             else
        //             {
        //                 countData = 0;
        //                 rb.message = "This Doctor has Already Verified!!";
        //                 rb.error = string.Empty;
        //             }
        //         }

        //         if (verificationDetail.VerificationDoctor.Count == countData)
        //         {
        //             rb.status = true;
        //             rb.error = pass;
        //         }
        //         else
        //         {
        //             rb.status = false;
        //             rb.error = string.Empty;
        //         }
        //     }
        //     else
        //     {

        //         rb.message = "Doctor Data Empty!!";
        //         rb.error = string.Empty;
        //     }
        //     return rb;
        // }

        // public async Task<bool> CheckVerifyDoctor(Int64 doctorRegNo, Int16 isVerified)
        // {
        //     bool isDoctorExists = false;
        //     string query = @"SELECT h.doctorRegNo
        //                     FROM doctorregistration h
        //                     WHERE h.doctorRegNo = @doctorRegNo AND h.isVerified=@isVerified ";
        //     List<MySqlParameter> pm = new();
        //     pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });
        //     pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = isVerified });
        //     ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
        //     if (dt.table.Rows.Count > 0)
        //     {
        //         isDoctorExists = true;
        //     }
        //     return isDoctorExists;
        // }
        // public async Task<bool> checkOldPassword(ResetPassword resetPassword)
        // {
        //     bool isExists = false;
        //     string query = @"SELECT  l.password
        //                           FROM userlogin l
        //                           WHERE l.userId=@doctorRegNo AND l.active = @active ";
        //     List<MySqlParameter> pm = new();
        //     pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = resetPassword.userId });
        //     pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = (Int16)Active.Yes });
        //     ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
        //     if (dt.table.Rows.Count > 0)
        //     {
        //         if (dt.table.Rows[0]["password"].ToString().Equals(resetPassword.oldPasssword, StringComparison.InvariantCultureIgnoreCase))
        //         {
        //             isExists = true;
        //         }
        //     }
        //     return isExists;
        // }


        public string getrandom()
        {
            string rno;
            Random randomclass = new Random();
            Int32 rno1 = randomclass.Next(10000, 99999);
            string random = Convert.ToString(rno1);
            rno = random.ToString();
            return rno;
        }

        // public async Task<ReturnClass.ReturnBool> ResetPassword(ResetPassword resetPassword)
        // {
        //     ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
        //     bool isExists = await checkOldPassword(resetPassword);
        //     if (isExists)
        //     {
        //         string query = @"Update userlogin  SET changePassword=@changePassword,password=@password
        //                       WHERE userId=@doctorRegNo";
        //         List<MySqlParameter> pm = new();
        //         pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = resetPassword.userId });
        //         pm.Add(new MySqlParameter("changePassword", MySqlDbType.Int16) { Value = (int)Active.Yes });
        //         pm.Add(new MySqlParameter("Password", MySqlDbType.String) { Value = resetPassword.Passsword });

        //         rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "InsertUserLogin");
        //         if (rb.status)
        //         {


        //             rb.message = "  Successfully Password Reset";
        //         }

        //         else
        //         {
        //             rb.message = " Somthing Went Wrong Please Try Again!!!";
        //         }
        //     }
        //     else
        //     {
        //         rb.message = "Old Password Not Matched!!";
        //         rb.error = string.Empty;
        //     }



        //     return rb;
        // }

        // public async Task<ReturnClass.ReturnBool> UpdateDoctorInfo(BlDoctor blDoctor)
        // {
        //     ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
        //     if (blDoctor.doctorRegNo == 0)
        //     {
        //         rb.message = " Invalid Doctor Registration Id";
        //         rb.status = false;
        //         return rb;
        //     }
        //     using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //     {
        //         string query = @"UPDATE doctorregistration 
        //                                   SET cityId=@cityId,pinCode=@pinCode,phoneNumber=@phoneNumber,landMark=@landMark,fax=@fax,isCovid=@isCovid,
        //                                     latitude=@latitude,longitude=@longitude,typeOfProviderId=@typeOfProviderId,website=@website,natureOfEntityId=@natureOfEntityId,
        //                                     clientIp=@clientIp,userId=@userId,lastUpdate=@lastUpdate
        //                                   WHERE doctorRegNo=@doctorRegNo";
        //         List<MySqlParameter> pm = new();
        //         pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = blDoctor.doctorRegNo });
        //         pm.Add(new MySqlParameter("cityId", MySqlDbType.Int32) { Value = blDoctor.cityId });
        //         pm.Add(new MySqlParameter("pinCode", MySqlDbType.VarChar, 6) { Value = blDoctor.pinCode });
        //         pm.Add(new MySqlParameter("phoneNumber", MySqlDbType.VarChar, 15) { Value = blDoctor.phoneNumber });
        //         pm.Add(new MySqlParameter("landMark", MySqlDbType.VarChar, 50) { Value = blDoctor.landMark });
        //         pm.Add(new MySqlParameter("fax", MySqlDbType.VarChar, 15) { Value = blDoctor.fax });
        //         pm.Add(new MySqlParameter("isCovid", MySqlDbType.Int16) { Value = blDoctor.isCovid });
        //         pm.Add(new MySqlParameter("latitude", MySqlDbType.Decimal) { Value = blDoctor.latitude });
        //         pm.Add(new MySqlParameter("longitude", MySqlDbType.Decimal) { Value = blDoctor.longitude });
        //         pm.Add(new MySqlParameter("typeOfProviderId", MySqlDbType.Int16) { Value = blDoctor.typeOfProviderId });
        //         pm.Add(new MySqlParameter("website", MySqlDbType.VarChar, 50) { Value = blDoctor.website });
        //         pm.Add(new MySqlParameter("natureOfEntityId", MySqlDbType.Int16) { Value = blDoctor.natureOfEntityId });
        //         pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blDoctor.clientIp });
        //         pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blDoctor.userId });
        //         pm.Add(new MySqlParameter("lastUpdate", MySqlDbType.String) { Value = blDoctor.entryDateTime });
        //         rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorregistration");
        //         ts.Complete();
        //     }
        //     return rb;
        // }

        // public async Task<ReturnClass.ReturnDataTable> SearchHS(Filter filter)
        // {
        //     try
        //     {
        //         MySqlParameter[] pm = new MySqlParameter[]
        //            {
        //             new MySqlParameter("DoctorSpec", MySqlDbType.VarChar,500) { Value = "%" +filter.DoctorSpec +"%" },
        //             new MySqlParameter("districtId", MySqlDbType.Int16) { Value = filter.districtId },
        //            };
        //         string qr = @"  SELECT  hr.doctorRegNo,hr.doctorNameEnglish,hr.districtId,hr.address,hr.mobileNo,hr.emailId,hr.registrationYear
        //                           ,hr.cityId,hr.pinCode,hr.phoneNumber,hr.landMark,hr.fax,hr.isCovid,hr.latitude,hr.longitude,hr.typeOfProviderId,hr.website,hr.natureOfEntityId
        //                           ,hs.specializationTypeName,hs.specializationName,hs.levelOfCareName 
        //                          FROM doctorregistration AS hr 
        //                       LEFT JOIN Doctorspecialization AS hs ON hr.doctorRegNo = hs.doctorRegNo
        //                       WHERE ( hr.doctorNameEnglish LIKE @DoctorSpec OR hs.specializationName LIKE @DoctorSpec ) AND hr.districtId=@districtId 
        //                                   AND hr.active=" + (Int16)YesNo.Yes + @" AND hr.isVerified = " + (Int16)YesNo.Yes;
        //         dt = await db.ExecuteSelectQueryAsync(qr, pm);
        //     }
        //     catch (Exception ex)
        //     {
        //     }
        //     return dt;
        // }

        // public async Task<ReturnClass.ReturnDataTable> GetDoctorDoc(Int64 doctorRegNo)
        // {
        //     try
        //     {
        //         MySqlParameter[] pm = new MySqlParameter[]
        //            {
        //              new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo },
        //            };
        //         string qr = @" SELECT ds.documentName,ds.documentExtension,dp.documentId,dp.documentImageGroup,dp.dptTableId 
        //                            FROM documentstore AS ds
        //                        INNER JOIN documentpathtbl AS dp ON dp.dptTableId = ds.dptTableId 
        //                       AND dp.documentType IN ('1','2','3','4') AND dp.documentImageGroup IN ('53','54','55','56')
        //                         AND ds.documentId=@doctorRegNo ORDER BY dp.documentImageGroup";
        //         dt = await db.ExecuteSelectQueryAsync(qr, pm);
        //     }
        //     catch (Exception ex)
        //     {
        //     }
        //     return dt;
        // }

        public async Task<ReturnClass.ReturnBool> SaveUpdateDoctorAcademic(BlDoctorAcademic bl)
        {
            MySqlParameter[] pm;
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            if (bl.doctorRegNo == 0)
            {
                rb.status = false;
                rb.message = "Invalid Doctor Registration No !";
                return rb;
            }
            Int32 countData = 0;
            string query = "";
            bool isValidated = true;
            if (isValidated)
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    pm = new MySqlParameter[]
                           {
                        new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo }, };
                    query = @"DELETE FROM doctoracademic 
                                WHERE doctorRegNo = @doctorRegNo";
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctoracademic");
                    if (rb.status)
                    {
                        foreach (var item in bl.Bl)
                        {
                            pm = new MySqlParameter[]
                            {
                                new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                                new MySqlParameter("degreePgId", MySqlDbType.Int16) { Value = item.degreePgId },
                                new MySqlParameter("degreePgName", MySqlDbType.VarChar,50) { Value = item.degreePgName },
                                new MySqlParameter("specialityId", MySqlDbType.Int16) { Value = item.specialityId },
                                new MySqlParameter("specialityName", MySqlDbType.VarChar,50) { Value = item.specialityName },
                                new MySqlParameter("collegeName", MySqlDbType.VarChar,50) { Value = item.collegeName },
                                new MySqlParameter("passingYear", MySqlDbType.Int32) { Value = item.passingYear },
                                new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                                new MySqlParameter("clientIp", MySqlDbType.String) { Value = bl.clientIp },
                                new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime },

                            };
                            query = @"INSERT INTO doctoracademic (doctorRegNo,degreePgId,degreePgName,specialityId,specialityName,collegeName,passingYear,entryDateTime,userId,clientIp)
                                        VALUES (@doctorRegNo,@degreePgId,@degreePgName,@specialityId,@specialityName,@collegeName,@passingYear,@entryDateTime,@userId,@clientIp)";
                            rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctoracademic");
                            if (rb.status)
                            {
                                countData = countData + 1;
                            }
                        }
                    }
                    if (bl.Bl.Count == countData)
                    {
                        ts.Complete();
                        rb.status = true;
                        rb.message = "Saved Successfully.";
                    }
                    else
                    {
                        rb.status = false;
                        rb.error = "Could not save academic, " + rb.message;
                    }
                }
            }
            return rb;
        }
        public async Task<ReturnClass.ReturnBool> SaveUpdateDoctorExperience(BlDoctorExperience bl)
        {
            MySqlParameter[] pm;
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            if (bl.doctorRegNo == 0)
            {
                rb.status = false;
                rb.message = "Invalid Doctor Registration No !";
                return rb;
            }
            Int32 countData = 0;
            string query = "";
            bool isValidated = true;
            if (isValidated)
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    pm = new MySqlParameter[]
                        {
                        new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                        };
                    query = @"DELETE FROM doctorworkexperience 
                                    WHERE doctorRegNo = @doctorRegNo";
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorworkexperience");
                    if (rb.status)
                    {
                        foreach (var item in bl.Bl)
                        {
                            //DateTime startDate = DateTime.ParseExact(item.startDate.Replace('-', '/'), "dd/MM/yyyy", null);
                            //item.startDate = startDate.ToString("yyyy/MM/dd");

                            //DateTime endDate = DateTime.ParseExact(item.endDate.Replace('-', '/'), "dd/MM/yyyy", null);
                            //item.endDate = endDate.ToString("yyyy/MM/dd");

                            pm = new MySqlParameter[]
                            {
                            new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                            new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = item.hospitalRegNo },
                            new MySqlParameter("hospitalNameEnglish", MySqlDbType.VarChar,200) { Value = item.hospitalNameEnglish },
                            new MySqlParameter("hospitalNameLocal", MySqlDbType.VarChar,200) { Value = item.hospitalNameLocal },
                            new MySqlParameter("yearFrom", MySqlDbType.Int32) { Value =item.yearFrom},
                            new MySqlParameter("yearTo", MySqlDbType.Int32) { Value = item.yearTo},
                            new MySqlParameter("designationName", MySqlDbType.VarChar,100) { Value = item.designationName },
                            new MySqlParameter("designationId", MySqlDbType.Int16) { Value = item.designationId },
                            new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                            new MySqlParameter("clientIp", MySqlDbType.String) { Value = bl.clientIp },
                            new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime },
                            new MySqlParameter("hospitalNameOther", MySqlDbType.VarChar,200) { Value = item.hospitalNameOther },
                            };
                            query = @"INSERT INTO doctorworkexperience (doctorRegNo,hospitalRegNo,hospitalNameEnglish,hospitalNameLocal,yearFrom,yearTo,designationId,designationName,entryDateTime,userId,clientIp,hospitalNameOther)
                                        VALUES (@doctorRegNo,@hospitalRegNo,@hospitalNameEnglish,@hospitalNameLocal,@yearFrom,@yearTo,@designationId,@designationName,@entryDateTime,@userId,@clientIp,@hospitalNameOther)";
                            rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorworkexperience");
                            if (rb.status)
                            {
                                countData = countData + 1;
                            }
                        }
                    }
                    if (bl.Bl.Count == countData)
                    {
                        ts.Complete();
                        rb.status = true;
                        rb.message = "Saved Successfully.";
                    }
                    else
                    {
                        rb.status = false;
                        rb.error = "Could not save experience, " + rb.message;
                    }
                }
            }
            return rb;
        }
        public async Task<ReturnClass.ReturnBool> SaveUpdateDoctorWorkArea(BlDoctorWorkArea bl)
        {
            DlCommon dlcommon = new();
            MySqlParameter[] pm;
            string query = "";
            Int32 uploadDataCount = 0;
            Int32 totalDataCount = 0;
            string url = "";
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            DlHospital dlHos = new();
            if (bl.doctorRegNo == null)
                bl.doctorRegNo = 0;
            using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                pm = new MySqlParameter[]
                  {
                    new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                  };
                query = @"DELETE FROM doctorworkarea 
                                    WHERE doctorRegNo = @doctorRegNo";
                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorworkarea");
                if (rb.status)
                {
                    foreach (var item in bl.Bl!)
                    {
                        if (item.hospitalRegNo == null || item.hospitalRegNo == 0)
                            item.hospitalRegNo = await dlHos.GetHospitalId();
                        pm = new MySqlParameter[]
                        {
                            new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                            new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = item.hospitalRegNo },
                            new MySqlParameter("hospitalNameEnglish", MySqlDbType.String) { Value = item.hospitalNameEnglish },
                            new MySqlParameter("consultancyTypeId", MySqlDbType.Int16) { Value = item.consultancyTypeId },
                            new MySqlParameter("consultancyTypeName", MySqlDbType.VarChar, 50) { Value = item.consultancyTypeName },
                            new MySqlParameter("price", MySqlDbType.Decimal) { Value =  item.price },
                            new MySqlParameter("hospitalAddress", MySqlDbType.String) { Value = item.hospitalAddress },
                            new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                            new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime },
                            new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = item.clientIp },
                        };
                        query = @"INSERT INTO doctorworkarea (doctorRegNo,hospitalRegNo,hospitalNameEnglish,consultancyTypeId,consultancyTypeName,price,
                                                               hospitalAddress,userId,entryDateTime, clientIp)
                                                     VALUES (@doctorRegNo,@hospitalRegNo,@hospitalNameEnglish,@consultancyTypeId,@consultancyTypeName,@price,
                                                              @hospitalAddress,@userId,@entryDateTime,@clientIp)";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorworkarea");
                        if (rb.status)
                        {
                            //countData = countData + 1;
                            //query = @"UPDATE documentstore AS ds SET ds.active=0 
                            //WHERE ds.documentId = @hospitalRegNo AND ds.userId = @userId AND ds.dptTableId = 12 ";
                            //rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "documentstore");

                            url = "WorkDocs/uploaddocs";
                            Int16 i = 0;
                            foreach (var itemDoc in item.BlDocument!)
                            {
                                totalDataCount++;
                                i++;
                                itemDoc.userId = bl.userId;
                                itemDoc.documentId = (Int64)itemDoc.documentId;
                                rb = await dlcommon.callStoreAPI(itemDoc, url);
                                if (rb.status)
                                {
                                    uploadDataCount++;
                                }
                            }
                        }
                    }
                }
                if (totalDataCount == uploadDataCount)
                {
                    rb.message = "Updated Successfully.";
                    rb.status = true;
                    ts.Complete();
                }
                return rb;
            }

        }
        public async Task<ReturnClass.ReturnDataTable> GetAllDoctorList(Int16 vId)
        {
            string query = @"SELECT dr.doctorRegNo,dr.doctorNameEnglish,dr.doctorNameLocal,dr.stateId,dr.districtId,dr.address,dr.mobileNo,
                                     dr.emailId,dr.active,s.stateNameEnglish AS stateName,d.districtNameEnglish AS districtName,dr.cityId,dr.cityName
                                FROM doctorregistration AS dr
                                 JOIN state AS s ON s.stateId=dr.stateId
				                 LEFT JOIN district AS d ON d.districtId=dr.districtId
                            WHERE dr.registrationStatus=@isVerified";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = vId });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());


            return dt;
        }
        public async Task<ReturnClass.ReturnBool> VerifyDoctor(VerificationDoctorDetail verificationDetail)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            Int32 countData = 0;
            string pass = "", hash_Pass = "";
            if (verificationDetail.VerificationDoctor.Count != 0)
            {
                foreach (var item in verificationDetail.VerificationDoctor)
                {
                    item.isVerified = YesNo.Yes;
                    bool isofficeExists = await CheckVerifyDoctor(item.doctorRegNo, (Int16)item.isVerified);
                    if (!isofficeExists)
                    {
                        if (item.registrationStatus == RegistrationStatus.Approved)
                        {
                            pass = "HRP27487";// "DR" + getrandom();
                            hash_Pass = sha256_hash(pass);
                        }
                        string query = @"UPDATE doctorregistration 
                             SET isVerified=@isVerified,clientIp=@clientIp,verificationDate=@verificationDate,
                                verifiedByLoginId=@verifiedByLoginId,registrationStatus=@registrationStatus,active=@active 
                              WHERE doctorRegNo=@doctorRegNo";

                        List<MySqlParameter> pm = new();
                        pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = item.doctorRegNo });
                        pm.Add(new MySqlParameter("registrationStatus", MySqlDbType.Int16) { Value = (int)item.registrationStatus });
                        pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = (int)item.isVerified });
                        pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = verificationDetail.clientIp });
                        pm.Add(new MySqlParameter("verifiedByLoginId", MySqlDbType.Int64) { Value = verificationDetail.userId });
                        pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = (int)item.isVerified });
                        pm.Add(new MySqlParameter("officeMappingKey", MySqlDbType.Int32) { Value = 0 });
                        pm.Add(new MySqlParameter("registrationDate", MySqlDbType.String) { Value = verificationDetail.date });
                        pm.Add(new MySqlParameter("registrationYear", MySqlDbType.Int32) { Value = DateTime.Now.Date.Year });
                        pm.Add(new MySqlParameter("changePassword", MySqlDbType.Int16) { Value = (int)Active.No });
                        pm.Add(new MySqlParameter("active1", MySqlDbType.Int16) { Value = (int)Active.Yes });
                        pm.Add(new MySqlParameter("isDisabled", MySqlDbType.Int16) { Value = (int)Active.No });
                        pm.Add(new MySqlParameter("userRole", MySqlDbType.Int16) { Value = (int)UserRole.Doctor });
                        pm.Add(new MySqlParameter("isSingleWindowUser", MySqlDbType.Int16) { Value = (int)Active.No });
                        pm.Add(new MySqlParameter("modificationType", MySqlDbType.Int16) { Value = (int)Active.No });
                        pm.Add(new MySqlParameter("userTypeCode", MySqlDbType.Int16) { Value = (int)Active.No });
                        pm.Add(new MySqlParameter("Password", MySqlDbType.String) { Value = hash_Pass });
                        using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "VerifyDoctor");
                            if (rb.status == true && item.registrationStatus == RegistrationStatus.Approved && item.isVerified == YesNo.Yes)
                            {
                                query = @"INSERT INTO userlogin
                                            (userName,userId,emailId,password,changePassword,active,isDisabled,
                                            clientIp,userRole,registrationYear,isSingleWindowUser,modificationType,userTypeCode)
                                        SELECT dr.doctorNameEnglish,dr.doctorRegNo,dr.emailId,@Password,@changePassword,
                                        @active1,@isDisabled,@clientIp,@userRole,@registrationYear,@isSingleWindowUser,
                                        @modificationType,@userTypeCode
                                            FROM  doctorregistration AS dr 
                                          WHERE dr.doctorRegNo=@doctorRegNo";
                                //query = "UPDATE userlogin SET isVerified=isVerified,verifiedByLoginId=@verifiedByLoginId," +
                                //        " active=@active" +
                                //    "  WHERE userId=@doctorRegNo";
                                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "VerifyDoctor");
                                if (rb.status)
                                {
                                    ts.Complete();
                                    countData = countData + 1;
                                    rb.message = "Doctor has been Verified Successfully";
                                    rb.error = pass;
                                }
                            }
                            else if (rb.status == true && item.registrationStatus == RegistrationStatus.Reject)
                            {
                                ts.Complete();
                                countData = countData + 1;
                                rb.message = "Doctor has been Rejected Successfully";
                                rb.error = string.Empty;
                            }
                            else if (rb.status == true && item.registrationStatus == RegistrationStatus.Pending)
                            {
                                ts.Complete();
                                countData = countData + 1;
                                rb.message = "Doctor has been Roll backed Successfully";
                                rb.error = string.Empty;
                            }

                        }
                    }
                    else
                    {
                        countData = 0;
                        rb.message = "This Doctor has Already Verified!!";
                        rb.error = string.Empty;
                    }
                }

                if (verificationDetail.VerificationDoctor.Count == countData)
                {
                    rb.status = true;
                    rb.error = rb.error;
                }
                else
                {
                    rb.status = false;
                    rb.error = string.Empty;
                }
            }
            else
            {

                rb.message = "Doctor Data Empty!!";
                rb.error = string.Empty;
            }
            return rb;
        }
        public async Task<bool> CheckVerifyDoctor(Int64 doctorRegNo, Int16 isVerified)
        {
            bool isDoctorExists = false;
            string query = @"SELECT dr.doctorRegNo
                            FROM doctorregistration AS dr
                            WHERE dr.doctorRegNo = @doctorRegNo AND dr.isVerified=@isVerified ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });
            pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = isVerified });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
            {
                isDoctorExists = true;
            }
            return isDoctorExists;
        }
        public async Task<ReturnClass.ReturnBool> SaveDoctorProfilePartOne(DoctorProfilePart1 bl)
        {
            DlCommon dlcommon = new();
            MySqlParameter[] pm;
            bool allFilesUploaded = true;
            string url = "";
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
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
                using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    DateTime dateOfBirth = DateTime.ParseExact(bl.dateOfBirth.Replace('-', '/'), "dd/MM/yyyy", null);
                    bl.dateOfBirth = dateOfBirth.ToString("yyyy/MM/dd");
                    pm = new MySqlParameter[]
                    {
                        new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                        new MySqlParameter("firstName", MySqlDbType.VarChar,99) { Value = bl.firstName},
                        new MySqlParameter("middleName", MySqlDbType.VarChar,99) { Value = bl.middleName},
                        new MySqlParameter("lastName", MySqlDbType.VarChar,99) { Value = bl.lastName},
                        new MySqlParameter("phoneNumber", MySqlDbType.VarChar,15) { Value = bl.phoneNumber},
                        new MySqlParameter("genderId", MySqlDbType.Int16) { Value = bl.genderId},
                        new MySqlParameter("genderName", MySqlDbType.VarChar,50) { Value = bl.genderName},
                        new MySqlParameter("dateOfBirth", MySqlDbType.VarChar,50) { Value = bl.dateOfBirth},
                        new MySqlParameter("aboutMe", MySqlDbType.VarChar,500) { Value = bl.aboutMe},
                        new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                        new MySqlParameter("clientIp", MySqlDbType.String) { Value = bl.clientIp },
                        new MySqlParameter("address1", MySqlDbType.VarChar,200) { Value = bl.address1},
                        new MySqlParameter("address2", MySqlDbType.VarChar,200) { Value = bl.address2},
                        new MySqlParameter("cityId", MySqlDbType.Int16) { Value = bl.cityId},
                        new MySqlParameter("cityName", MySqlDbType.VarChar,99) { Value = bl.cityName},
                        new MySqlParameter("stateId", MySqlDbType.Int16) { Value = bl.stateId},
                        new MySqlParameter("stateName", MySqlDbType.VarChar,99) { Value = bl.stateName},
                         new MySqlParameter("countryId", MySqlDbType.Int16) { Value = bl.countryId},
                        new MySqlParameter("countryName", MySqlDbType.VarChar,99) { Value = bl.countryName},
                         new MySqlParameter("pinCode", MySqlDbType.VarChar,6) { Value = bl.pinCode},
                        new MySqlParameter("specialization", MySqlDbType.VarChar,200) { Value = bl.specialization},
                        new MySqlParameter("specializationId", MySqlDbType.Int16) { Value = bl.specializationId},
                        new MySqlParameter("subSpecialization", MySqlDbType.MediumText) { Value = bl.subSpecialization },

                    };
                    query = @"SELECT dp.doctorProfileId
                            FROM doctorprofile AS dp
                            WHERE dp.doctorRegNo = @doctorRegNo";
                    ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
                    if (dt.table.Rows.Count > 0)
                    {
                        query = @"UPDATE doctorprofile 
                             SET firstName=@firstName,middleName=@middleName,lastName=@lastName,phoneNumber=@phoneNumber,
                                genderId=@genderId,genderName=@genderName,dateOfBirth=@dateOfBirth,aboutMe=@aboutMe ,clientIp=@clientIp,userId=@userId,address1=@address1,address2=@address2,
                                cityId=@cityId,cityName=@cityName,stateId=@stateId,stateName=@stateName,countryId=@countryId,countryName=@countryName,pinCode=@pinCode,specialization=@specialization,
                                specializationId=@specializationId,subSpecialization=@subSpecialization
                              WHERE doctorRegNo=@doctorRegNo";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorprofile");
                    }
                    else if (dt.table.Rows.Count == 0)
                    {
                        query = @"INSERT INTO doctorprofile (doctorRegNo,firstName,middleName,lastName,phoneNumber,genderId,genderName,dateOfBirth,aboutMe,clientIp,userId,address1,
                                                            address2,cityId,cityName,stateId,stateName,countryId,countryName,pinCode,specialization,specializationId,subSpecialization) 
                                                     VALUES (@doctorRegNo,@firstName,@middleName,@lastName,@phoneNumber,@genderId,@genderName,@dateOfBirth,@aboutMe,@clientIp,@userId,@address1,
                                                            @address2,@cityId,@cityName,@stateId,@stateName,@countryId,@countryName,@pinCode,@specialization,@specializationId,@subSpecialization) ;";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorprofile");
                    }
                    if (rb.status == true && bl.BlDocument != null)
                    {
                        url = "WorkDocs/uploadprofdoctor";
                        Int16 i = 0;
                        foreach (var item in bl.BlDocument)
                        {
                            i++;
                            item.userId = bl.userId;
                            item.documentId = (Int64)bl.doctorRegNo!;
                            rb = await dlcommon.callStoreAPI(item, url);
                            bl.BlDocument[i - 1].documentName = rb.message;
                            if (rb.status)
                                bl.BlDocument[i - 1].uploaded = 1;
                        }
                        i = 0;
                        foreach (var item in bl.BlDocument)
                        {
                            if (item.uploaded == 0)
                            { allFilesUploaded = false; break; }
                            else if (item.uploaded == 1)
                                allFilesUploaded = true;
                        }
                    }
                    if (allFilesUploaded && rb.status)
                    {
                        rb.message = "Updated Successfully.";
                        rb.status = true;
                        ts.Complete();
                    }
                    else
                    {
                        rb.message = "Could not save , " + rb.message;
                        rb.status = false;
                        Int16 i = 0;
                        url = "WorkDocs/deleteanydoc";
                        foreach (var item in bl.BlDocument!)
                        {
                            if (item.uploaded == 1)
                            {
                                i++;
                                bl.userId = bl.userId;
                                item.documentId = (Int64)item.documentId;
                                rb = await dlcommon.callStoreAPI(item, url);
                            }
                        }
                    }
                }
            }
            return rb;
        }
        public async Task<ReturnClass.ReturnBool> SaveUpdateDoctorAward(DoctorAward bl)
        {
            Int32 countData = 0;
            string pass = "";
            MySqlParameter[] pm;
            string query = "";
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            if (bl.doctorRegNo == null)
                bl.doctorRegNo = 0;
            using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                pm = new MySqlParameter[]
                      {
                        new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                      };
                query = @"DELETE FROM doctoraward 
                                    WHERE doctorRegNo = @doctorRegNo";
                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctoraward");
                if (rb.status)
                {
                    foreach (var item in bl.items!)
                    {
                        pm = new MySqlParameter[]
                        {
                        new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                        new MySqlParameter("awardName", MySqlDbType.VarChar,100) { Value = item.awardName },
                        new MySqlParameter("awardYear", MySqlDbType.VarChar,10) { Value = item.awardYear },
                        new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                        new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.date },
                        new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = bl.clientIp },
                         };
                        query = @"INSERT INTO doctoraward (doctorRegNo,awardName,awardYear,userId,entryDateTime,clientIp)
                                                   VALUES (@doctorRegNo,@awardName,@awardYear,@userId,@entryDateTime,@clientIp)";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctoraward");
                        if (rb.status)
                        {
                            countData = countData + 1;
                        }
                    }
                }
                if (bl.items.Count == countData)
                {
                    ts.Complete();
                    rb.status = true;
                    rb.error = pass;
                }
                else
                {
                    rb.status = false;
                    rb.error = "Could not save award, " + rb.message;
                }
                return rb;
            }

        }
        public async Task<ReturnClass.ReturnBool> SaveUpdateDoctorMCR(DoctorMCR bl)
        {
            Int32 countData = 0;
            string pass = "";
            MySqlParameter[] pm;
            string query = "";
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            if (bl.doctorRegNo == null)
                bl.doctorRegNo = 0;
            using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                pm = new MySqlParameter[]
                      {
                        new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                      };
                query = @"DELETE FROM doctormedicalcouncilregistration 
                                    WHERE doctorRegNo = @doctorRegNo";
                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctoraward");
                if (rb.status)
                {


                    foreach (var item in bl.items)
                    {
                        //DateTime fromDate = DateTime.ParseExact(item.fromDate.Replace('-', '/'), "dd/MM/yyyy", null);
                        //item.fromDate = fromDate.ToString("yyyy/MM/dd");

                        //DateTime toDate = DateTime.ParseExact(item.toDate.Replace('-', '/'), "dd/MM/yyyy", null);
                        //item.toDate = toDate.ToString("yyyy/MM/dd");

                        pm = new MySqlParameter[]
                        {
                            new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                            new MySqlParameter("remark", MySqlDbType.VarChar,200) { Value = item.remark },
                            new MySqlParameter("year", MySqlDbType.Int32) { Value = item.year },
                            new MySqlParameter("mcrName", MySqlDbType.VarChar,100) { Value = item.mcrName },
                            new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                            new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.date },
                            new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = bl.clientIp },
                         };
                        query = @"INSERT INTO doctormedicalcouncilregistration (doctorRegNo,remark,year,mcrName,userId,entryDateTime,clientIp)
                                                                        VALUES (@doctorRegNo,@remark,@year,@mcrName,@userId,@entryDateTime,@clientIp)";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctormedicalcouncilregistration");
                        if (rb.status)
                        {
                            countData = countData + 1;
                        }
                    }
                }
                if (bl.items.Count == countData)
                {
                    ts.Complete();
                    rb.status = true;
                    rb.error = pass;
                }
                else
                {
                    rb.status = false;
                    rb.error = "Could not save Medical Council Registration, " + rb.message;
                }
                return rb;
            }

        }
        public async Task<ReturnClass.ReturnBool> SaveUpdateMembership(DoctorMembership bl)
        {
            Int32 countData = 0;
            string pass = "";
            MySqlParameter[] pm;
            string query = "";
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            if (bl.doctorRegNo == null)
                bl.doctorRegNo = 0;
            using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                pm = new MySqlParameter[]
                      {
                        new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                      };
                query = @"DELETE FROM doctormembership 
                                    WHERE doctorRegNo = @doctorRegNo";
                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctormembership");
                if (rb.status)
                {
                    foreach (var item in bl.items)
                    {
                        pm = new MySqlParameter[]
                        {
                            new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                            new MySqlParameter("membershipName", MySqlDbType.VarChar,100) { Value = item.membershipName },
                            new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                            new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.date },
                            new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = bl.clientIp },
                         };
                        query = @"INSERT INTO doctormembership (doctorRegNo,membershipName,userId,entryDateTime,clientIp)
                                                        VALUES (@doctorRegNo,@membershipName,@userId,@entryDateTime,@clientIp)";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctormembership");
                        if (rb.status)
                        {
                            countData = countData + 1;
                        }
                    }
                }
                if (bl.items.Count == countData)
                {
                    ts.Complete();
                    rb.status = true;
                    rb.error = pass;
                }
                else
                {
                    rb.status = false;
                    rb.error = "Could not save Membership, " + rb.message;
                }
                return rb;
            }

        }
        public async Task<ReturnClass.ReturnBool> SaveUpdateAddOns(DoctorAddOns bl)
        {
            Int32 countData = 0;
            string pass = "";
            MySqlParameter[] pm;
            string query = "";
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            if (bl.doctorRegNo == null)
                bl.doctorRegNo = 0;
            using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                pm = new MySqlParameter[]
                      {
                        new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                      };
                query = @"DELETE FROM doctoraddonscertification 
                                    WHERE doctorRegNo = @doctorRegNo";
                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctoraddonscertification");
                if (rb.status)
                {
                    foreach (var item in bl.items)
                    {
                        pm = new MySqlParameter[]
                        {
                            new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                            new MySqlParameter("certificateName", MySqlDbType.VarChar,100) { Value = item.certificateName },
                            new MySqlParameter("year", MySqlDbType.Int32) { Value = item.year },
                            new MySqlParameter("reason", MySqlDbType.VarChar,200) { Value = item.reason },
                            new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                            new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.date },
                            new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = bl.clientIp },
                         };
                        query = @"INSERT INTO doctoraddonscertification (doctorRegNo,certificateName,year,reason,userId,entryDateTime,clientIp)
                                                        VALUES (@doctorRegNo,@certificateName,@year,@reason,@userId,@entryDateTime,@clientIp)";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctormembership");
                        if (rb.status)
                        {
                            countData = countData + 1;
                        }
                    }
                }
                if (bl.items.Count == countData)
                {
                    ts.Complete();
                    rb.status = true;
                    rb.error = pass;
                }
                else
                {
                    rb.status = false;
                    rb.error = "Could not save Add ons Certification, " + rb.message;
                }
                return rb;
            }

        }
        public async Task<ReturnClass.ReturnBool> SaveUpdateIndaminity(DoctorIndaminity bl)
        {
            string pass = "";
            MySqlParameter[] pm;
            string query = "";
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            if (bl.doctorRegNo == null)
                bl.doctorRegNo = 0;
            using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                pm = new MySqlParameter[]
                      {
                        new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                      };
                query = @"DELETE FROM doctorindaminity 
                                    WHERE doctorRegNo = @doctorRegNo";
                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorindaminity");
                if (rb.status)
                {
                    pm = new MySqlParameter[]
                    {
                            new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                            new MySqlParameter("isIndaminity", MySqlDbType.Int16) { Value = bl.isIndaminity },
                            new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                            new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.date },
                            new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = bl.clientIp },
                     };
                    query = @"INSERT INTO doctorindaminity (doctorRegNo,isIndaminity,userId,entryDateTime,clientIp)
                                                        VALUES (@doctorRegNo,@isIndaminity,@userId,@entryDateTime,@clientIp)";
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorindaminity");
                }
                if (rb.status)
                {
                    ts.Complete();
                    rb.status = true;
                    rb.error = pass;
                }
                else
                {
                    rb.status = false;
                    rb.error = "Could not save Indaminity, " + rb.message;
                }
                return rb;
            }

        }
        public async Task<ReturnClass.ReturnDataTable> GetDoctorInfo(Int64 doctorRegNo)
        {
            string query = @"SELECT dr.doctorRegNo,dr.doctorNameEnglish,dr.doctorNameLocal,dp.stateId,dp.districtId,dp.address1, dp.address2 ,dr.mobileNo,dp.countryId,dp.countryName,
                                     dr.emailId,dr.active,dp.stateName,dp.districtName,ul.userName,dp.firstName,dp.middleName,dp.lastName,dp.phoneNumber,dp.genderId,
                                     dp.genderName,DATE_FORMAT(dp.dateOfBirth,'%d/%m/%Y') AS dateOfBirth,dsdpt1.documentId, dsdpt1.documentName,dsdpt1.documentExtension,dp.pincode,
                                     dp.cityId,dp.cityName,dp.specialization,dp.aboutMe, GROUP_CONCAT(DISTINCT da.degreePgName) AS degrees,
                                    GROUP_CONCAT(DISTINCT ds.specializationName) AS doctorMultipleSpecialization , dp.subSpecialization
                               FROM doctorregistration AS dr 
                                INNER JOIN userlogin ul ON dr.doctorRegNo=ul.userId
                                LEFT JOIN doctoracademic da ON dr.doctorRegNo=da.doctorRegNo
								LEFT JOIN doctorprofile AS dp ON dr.doctorRegNo=dp.doctorRegNo
                                LEFT JOIN doctorspecialization ds ON ds.doctorRegNo=dr.doctorRegNo
                                LEFT JOIN (
   			                                SELECT ds.documentId,ds.documentName,ds.documentExtension
				 	                            FROM documentstore AS ds 
                                           INNER JOIN documentpathtbl AS dpt ON ds.active=@active AND dpt.dptTableId = ds.dptTableId AND dpt.documentType=" + (Int16)DocumentType.DoctorProfilePic + @" AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Doctor + @"
                                         ) AS dsdpt1 ON dsdpt1.documentId=dr.doctorRegNo
                                 WHERE dr.doctorRegNo=@doctorRegNo; 
                            ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });
            pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = YesNo.Yes });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }
        public async Task<List<BlDoctorWorkAreaItemsDoc>> GetDoctorClinicInfo(Int64 doctorRegNo)
        {
            string query = @"
                            SELECT dwa.hospitalRegNo,dwa.hospitalNameEnglish AS hospitalName,dwa.hospitalAddress,
			                            dwa.consultancyTypeId,dwa.consultancyTypeName,dwa.price                                   	
                                   FROM doctorworkarea AS dwa  
                               WHERE dwa.doctorRegNo=@doctorRegNo ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            BlDoctorWorkAreaItemsDoc bl = new BlDoctorWorkAreaItemsDoc();
            BlDocument blDoc = new();
            List<BlDoctorWorkAreaItemsDoc> blFin = new List<BlDoctorWorkAreaItemsDoc>();
            for (int i = 0; i < dt.table.Rows.Count; i++)
            {
                query = @" SELECT ds.documentId,ds.documentName,ds.documentExtension,ds.userId
				 	                FROM documentstore AS ds 
                                INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId 
                               WHERE ds.documentId= " + dt.table.Rows[i]["hospitalRegNo"].ToString() + @" AND active = 1 AND 
                            dpt.documentType = " + (Int16)DocumentType.DoctorWorkArea + @" AND dpt.documentImageGroup = " + (Int16)DocumentImageGroup.Doctor;
                ReturnClass.ReturnDataTable dtChild = await db.ExecuteSelectQueryAsync(query, pm.ToArray());

                bl = new BlDoctorWorkAreaItemsDoc
                {
                    hospitalRegNo = Convert.ToInt64(dt.table.Rows[i]["hospitalRegNo"]),
                    hospitalNameEnglish = dt.table.Rows[i]["hospitalName"].ToString(),
                    hospitalAddress = dt.table.Rows[i]["hospitalAddress"].ToString(),
                    consultancyTypeId = Convert.ToInt16(dt.table.Rows[i]["consultancyTypeId"]),
                    consultancyTypeName = dt.table.Rows[i]["consultancyTypeName"].ToString(),
                    price = Convert.ToDecimal(dt.table.Rows[i]["price"]),
                };
                bl.BlDocument = new();
                for (int j = 0; j < dtChild.table.Rows.Count; j++)
                {
                    blDoc = new BlDocument
                    {
                        documentId = Convert.ToInt64(dtChild.table.Rows[j]["documentId"]),
                        documentName = dtChild.table.Rows[j]["documentName"].ToString(),
                        documentExtension = dtChild.table.Rows[j]["documentExtension"].ToString()
                    };

                    bl.BlDocument!.Add(blDoc);
                }
                blFin.Add(bl);
            }

            return blFin;
        }
        public async Task<ReturnClass.ReturnDataTable> GetDoctorEducationInfo(Int64 doctorRegNo)
        {
            string query = @"
                            SELECT da.degreePgId,da.degreePgName,da.specialityId,da.specialityName,da.collegeName,da.passingYear,da.academicId                               	
                                    FROM doctoracademic AS da  
                                WHERE da.doctorRegNo=@doctorRegNo ;";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });
            ReturnClass.ReturnDataTable ds = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return ds;
        }
        public async Task<ReturnClass.ReturnDataTable> GetDoctorExperienceInfo(Int64 doctorRegNo)
        {
            string query = @" SELECT dwe.doctorWorkExpId,dwe.hospitalRegNo,dwe.hospitalNameEnglish,dwe.hospitalNameLocal,dwe.yearFrom ,
                                    dwe.yearTo,dwe.designationId,dwe.designationName,dwe.hospitalNameOther,dwe.designationName
                                   FROM doctorworkexperience AS dwe  
                               WHERE dwe.doctorRegNo=@doctorRegNo ;";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });
            ReturnClass.ReturnDataTable ds = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return ds;
        }
        public async Task<ReturnClass.ReturnDataTable> GetDoctorAwardInfo(Int64 doctorRegNo)
        {
            string query = @" SELECT da.awardId,da.awardName,da.awardYear                        	
                                   FROM doctoraward AS da  
                               WHERE da.doctorRegNo=@doctorRegNo ;";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });
            ReturnClass.ReturnDataTable ds = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return ds;
        }
        public async Task<ReturnClass.ReturnDataTable> GetDoctorMembershipInfo(Int64 doctorRegNo)
        {
            string query = @" SELECT dm.membershipId,dm.membershipName
                               FROM doctormembership AS dm  
                           WHERE dm.doctorRegNo=@doctorRegNo ;";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });
            ReturnClass.ReturnDataTable ds = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return ds;
        }
        public async Task<ReturnClass.ReturnDataTable> GetDoctorAddons(Int64 doctorRegNo)
        {
            string query = @" SELECT dac.addOnId,dac.certificateName,dac.year,dac.reason
                                   FROM doctoraddonscertification AS dac  
                               WHERE dac.doctorRegNo=@doctorRegNo ;";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });
            ReturnClass.ReturnDataTable ds = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return ds;
        }
        public async Task<ReturnClass.ReturnDataTable> GetDoctorIndaminity(Int64 doctorRegNo)
        {
            string query = @" SELECT di.indaminityId,di.isIndaminity,case when di.isIndaminity =1 then 'Yes' when di.isIndaminity = 0 then 
                                'No' END isIndaminityYesNo
                                   FROM doctorindaminity AS di  
                               WHERE di.doctorRegNo=@doctorRegNo ;";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });
            ReturnClass.ReturnDataTable ds = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return ds;
        }
        public async Task<ReturnClass.ReturnDataTable> GetDoctorRegistrationInfo(Int64 doctorRegNo)
        {
            string query = @"SELECT  dmcr.mcrId,dmcr.registrations,dmcr.`year`,dmcr.mcrName,remark
                                       FROM doctormedicalcouncilregistration AS dmcr  
                            WHERE dmcr.doctorRegNo=@doctorRegNo ;";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });
            ReturnClass.ReturnDataTable ds = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return ds;
        }
        public async Task<ReturnClass.ReturnDataTable> GetDoctorProfileLogo(Int64 doctorRegNo)
        {
            string query = @"SELECT dsdpt1.documentIdProfilePic, dsdpt1.documentNameProfilePic,dsdpt1.documentExtensionProfilePic
                                FROM  doctorprofile AS dp INNER JOIN doctorregistration AS dr ON dr.doctorRegNo=dp.doctorRegNo
                                	INNER JOIN userlogin ul ON dr.doctorRegNo=ul.userId
                                LEFT JOIN (
   			                                    SELECT ds.documentId AS documentIdProfilePic,ds.documentName AS documentNameProfilePic,ds.documentExtension AS documentExtensionProfilePic
				 	                            FROM documentstore AS ds 
                                           INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND active=1 AND dpt.documentType=" + (Int16)DocumentType.DoctorProfilePic + @" AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Doctor + @"
                                       ) AS dsdpt1 ON dsdpt1.documentIdProfilePic=dr.doctorRegNo
                                 WHERE dr.doctorRegNo=@doctorRegNo; 
                            ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }
        public async Task<ReturnClass.ReturnBool> SaveUpdateDoctorScheduleDateTime(DoctorScheduleDate bl)
        {
            Int32 scheduleDateId = 0;
            Int32 countData = 0;
            MySqlParameter[] pm;
            string query = "";
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            if (bl.doctorRegNo == null)
                bl.doctorRegNo = 0;
            using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                pm = new MySqlParameter[]
                {
                    new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                    new MySqlParameter("dayId", MySqlDbType.Int16) { Value = bl.dayId },
                };
                query = @"SELECT scheduleDateId
	                        FROM doctorscheduledate AS dsd
                          WHERE doctorRegNo = @doctorRegNo AND dsd.dayId=@dayId";
                dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
                if (dt.table.Rows.Count == 0)
                {
                    scheduleDateId = await dl.GetMaxDoctorScheduleDateId();
                    pm = new MySqlParameter[]
                    {
                        new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                        new MySqlParameter("scheduleDateId", MySqlDbType.Int32) { Value = scheduleDateId },
                        new MySqlParameter("dayId", MySqlDbType.Int16) { Value = bl.dayId },
                        new MySqlParameter("day", MySqlDbType.VarChar,50) { Value = bl.day },
                        new MySqlParameter("isActive", MySqlDbType.Int16) { Value = (Int16)YesNo.Yes },
                        new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                        new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.date },
                        new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = bl.clientIp },
                    };
                    query = @"INSERT INTO doctorscheduledate (scheduleDateId,doctorRegNo,dayId,day,userId,entryDateTime,clientIp,isActive)
                                                      VALUES (@scheduleDateId,@doctorRegNo,@dayId,@day,@userId,@entryDateTime,@clientIp,@isActive)";
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorscheduledate");
                }
                else
                {
                    scheduleDateId = Convert.ToInt32(dt.table.Rows[0]["scheduleDateId"]);
                    query = @"INSERT INTO doctorscheduletimelog
                                            SELECT *
                                            FROM doctorscheduletime
                                            WHERE scheduleDateId=@scheduleDateId";
                    pm = new MySqlParameter[]
                        {
                            new MySqlParameter("scheduleDateId", MySqlDbType.Int32) { Value = scheduleDateId },
                        };
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorscheduletime");
                    if (rb.status == true)
                    {
                        query = @"DELETE FROM doctorscheduletime 
                                    WHERE scheduleDateId = @scheduleDateId";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorscheduletime");
                    }
                }
                if (rb.status)
                {
                    foreach (var item in bl.items)
                    {
                        pm = new MySqlParameter[]
                        {
                            new MySqlParameter("scheduleDateId", MySqlDbType.Int32) { Value = scheduleDateId },
                            new MySqlParameter("fromTime", MySqlDbType.VarChar,10) { Value = item.fromTime },
                            new MySqlParameter("toTime", MySqlDbType.VarChar,10) { Value = item.toTime },
                            new MySqlParameter("isActive", MySqlDbType.Int16) { Value = (Int16)YesNo.Yes },
                            new MySqlParameter("patientLimit", MySqlDbType.Int16) { Value = item.patientLimit },

                        };
                        query = @"INSERT INTO doctorscheduletime (scheduleDateId,fromTime,toTime,isActive,patientLimit)
                                                      VALUES (@scheduleDateId,@fromTime,@toTime,@isActive,@patientLimit)";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorscheduletime");
                        if (rb.status)
                        {
                            countData = countData + 1;
                        }
                    }
                    if (bl.items.Count == countData)
                    {
                        ts.Complete();
                        rb.status = true;
                    }
                    else
                    {
                        rb.status = false;
                        rb.error = "Could not save Doctor Schedule, " + rb.message;
                    }
                }

            }
            return rb;
        }
        public async Task<ReturnClass.ReturnDataTable> GetDoctorScheduleTimings(Int64 doctorRegNo, Int16 dayId)
        {
            string query = @"SELECT dsd.scheduleDateId,dst.scheduleTimeId,dsd.doctorRegNo,dsd.dayId,dsd.`day`,dst.fromTime,dst.toTime,dst.patientLimit
	                             FROM doctorscheduledate AS dsd 
 	                             INNER JOIN doctorscheduletime AS dst ON dsd.scheduleDateId = dst.scheduleDateId
                            WHERE dsd.doctorRegNo=@doctorRegNo AND dsd.isActive=@isActive AND dst.isActive=@isActive ";
            string where = "";
            if (dayId != 0)
                where = " AND dsd.dayId = @dayId";
            query += where + " ORDER BY dsd.dayId,dst.scheduleTimeId";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });
            pm.Add(new MySqlParameter("dayId", MySqlDbType.Int16) { Value = dayId });
            pm.Add(new MySqlParameter("isActive", MySqlDbType.Int16) { Value = (Int16)YesNo.Yes });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }
        public async Task<ReturnClass.ReturnBool> DeleteScheduleTime(Int64 scheduleTimeId)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            string query = "";
            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    query = @"INSERT INTO doctorscheduletimelog
                                            SELECT *
                                            FROM doctorscheduletime
                                            WHERE scheduleTimeId=@scheduleTimeId";
                    MySqlParameter[] pm = new MySqlParameter[]
                        {
                            new MySqlParameter("scheduleTimeId", MySqlDbType.Int64) { Value = scheduleTimeId },
                        };
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorscheduletime");
                    if (rb.status == true)
                    {
                        query = @"DELETE FROM doctorscheduletime                                
                                            WHERE scheduleTimeId=@scheduleTimeId";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorscheduletime");
                        if (rb.status == true)
                        {
                            rb.message = "Schedule Deleted successfully";
                            rb.value = Convert.ToString(scheduleTimeId);
                            ts.Complete();
                        }
                        else
                        {
                            rb.value = "0";
                            rb.message = "Failed to Delete Scheduled Time," + rb.message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                rb.error = "Could not save Doctor Schedule Time, " + rb.message;
                rb.status = false;
            }
            return rb;
        }
        public async Task<ReturnClass.ReturnDataSet> GetVerifiedCounters()
        {
            string query = @"SELECT COUNT(1) AS verifiedDoctors 
		                            FROM doctorregistration AS dr 
	                            WHERE dr.isVerified=" + (Int16)YesNo.Yes + @" AND dr.active=" + (Int16)YesNo.Yes + @";
                            SELECT COUNT(1) AS verifiedPatients
		                            FROM patientregistration AS pr 
	                            WHERE pr.isVerified=" + (Int16)YesNo.Yes + @" AND pr.active=" + (Int16)YesNo.Yes + @";";
            ReturnClass.ReturnDataSet ds = await db.executeSelectQueryForDataset_async(query);
            return ds;
        }
        public async Task<ReturnClass.ReturnDataTable> GetDoctorPatientLimList(Int16 role)
        {
            string query = "";
            if (role == (Int16)UserRole.Doctor)
                query = @"SELECT dr.doctorRegNo,dr.doctorNameEnglish,dr.doctorNameLocal,dr.address,dr.mobileNo,
		                    dr.emailId,GROUP_CONCAT(DISTINCT ds.specializationTypeName) AS specializationTypeName
		                     FROM doctorregistration AS dr 
		                    INNER JOIN doctorspecialization AS ds ON dr.doctorRegNo=ds.doctorRegNo
                         WHERE dr.isVerified= " + (Int16)YesNo.Yes + @" AND dr.active= " + (Int16)YesNo.Yes + @" GROUP BY dr.doctorRegNo";
            else if (role == (Int16)UserRole.Patient)
                query = @"SELECT pr.patientRegNo, pr.patientNameEnglish, pr.patientNameLocal, pr.mobileNo, pr.emailId
                          FROM patientregistration AS pr
                        WHERE pr.isVerified= " + (Int16)YesNo.Yes + @" AND pr.active= " + (Int16)YesNo.Yes;
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query);
            return dt;
        }
        public async Task<ReturnClass.ReturnDataSet> GetAllDoctorInfo(Int64 doctorRegNo)
        {
            ReturnClass.ReturnDataSet dataSet = new();
            ReturnClass.ReturnDataTable dtt;
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });
            string query = @"SELECT dr.doctorRegNo,dr.doctorNameEnglish,dr.doctorNameLocal,dr.address,dr.mobileNo,
                         dr.emailId,GROUP_CONCAT(DISTINCT ds.specializationTypeName) AS specializationTypeName,
                         dr.firstName,dr.middleName,dr.lastName, dist.districtNameEnglish AS districtName,dr.cityName
		                                            FROM doctorregistration AS dr 
		                                        INNER JOIN doctorspecialization AS ds ON dr.doctorRegNo=ds.doctorRegNo
                INNER JOIN district AS dist ON dist.districtId=dr.districtId
                                            WHERE dr.doctorRegNo=@doctorRegNo ";
            dtt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            dtt.table.TableName = "DoctorSingleScpecialization";
            dataSet.dataset.Tables.Add(dtt.table);

            query = @"SELECT dr.doctorRegNo,dr.doctorNameEnglish,dr.doctorNameLocal,dp.stateId,dp.districtId,dp.address1, dp.address2 ,dr.mobileNo,dp.countryId,dp.countryName,
                                     dr.emailId,dr.active,dp.stateName,dp.districtName,ul.userName,dp.firstName,dp.middleName,dp.lastName,dp.phoneNumber,dp.genderId,
                                     dp.genderName,DATE_FORMAT(dp.dateOfBirth,'%d/%m/%Y') AS dateOfBirth,dp.pincode,
                                     dp.cityId,dp.cityName,dp.specialization
                               FROM doctorregistration AS dr 
									LEFT JOIN doctorprofile AS dp ON dr.doctorRegNo=dp.doctorRegNo
                                	INNER JOIN userlogin ul ON dr.doctorRegNo=ul.userId 
                                WHERE dr.doctorRegNo=@doctorRegNo; ";
            dtt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            dtt.table.TableName = "DoctorProfileDetails";
            dataSet.dataset.Tables.Add(dtt.table);

            query = @" SELECT dsdpt1.documentId AS doctorProfileDocumentId, dsdpt1.documentName AS doctorProfileDocumentName,dsdpt1.documentExtension AS doctorProfileDocumentExtension
								   FROM doctorregistration AS dr LEFT JOIN (
   			                             SELECT ds.documentId,ds.documentName,ds.documentExtension
				 	                            FROM documentstore AS ds 
                                           INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND dpt.documentType=" + (Int16)DocumentType.ProfilePic + @" AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Doctor + @"
                                       ) AS dsdpt1 ON dsdpt1.documentId=dr.doctorRegNo
                                 WHERE dr.doctorRegNo=@doctorRegNo; ";
            dtt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            dtt.table.TableName = "DoctorProfileImages";
            dataSet.dataset.Tables.Add(dtt.table);

            query = @" SELECT dwa.hospitalRegNo,dwa.hospitalNameEnglish AS hospitalName,dwa.hospitalAddress,
			                            dwa.consultancyTypeId,dwa.consultancyTypeName,dwa.price 
                                   FROM doctorworkarea AS dwa 
                                 WHERE dr.doctorRegNo=@doctorRegNo; ";
            dtt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            dtt.table.TableName = "DoctorWorkArea";
            dataSet.dataset.Tables.Add(dtt.table);

            query = @" SELECT dwa.hospitalRegNo,dsdpt1.documentId AS doctorWorkAreaDocumentId, dsdpt1.documentName AS doctorWorkAreaDocumentName,dsdpt1.documentExtension AS doctorWorkAreaDocumentExtension
                                   FROM doctorworkarea AS dwa
                                    LEFT JOIN (
   			                                 SELECT ds.documentId,ds.documentName,ds.documentExtension
				 	                                FROM documentstore AS ds 
                                               INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND dpt.documentType=" + (Int16)DocumentType.DoctorHospitalImages + @"  AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Hospital + @"
                                           ) AS dsdpt1 ON dsdpt1.documentId=dwa.doctorRegNo
                               WHERE dwa.doctorRegNo=@doctorRegNo ; ";
            dtt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            dtt.table.TableName = "DoctorWorkAreaImageHospitalBasis";
            dataSet.dataset.Tables.Add(dtt.table);

            query = @" SELECT da.degreePgId,da.degreePgName,da.specialityId,da.specialityName,da.collegeName,da.passingYear,da.academicId                               	
                                    FROM doctoracademic AS da  
                                WHERE da.doctorRegNo=@doctorRegNo ; ";
            dtt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            dtt.table.TableName = "DoctorAcademic";
            dataSet.dataset.Tables.Add(dtt.table);

            query = @" SELECT dwe.doctorWorkExpId,dwe.hospitalRegNo,dwe.hospitalNameEnglish,dwe.hospitalNameLocal,dwe.yearFrom ,
                                    dwe.yearTo,dwe.designationId,dwe.designationName,dwe.hospitalNameOther,dwe.designationName
                                   FROM doctorworkexperience AS dwe  
                               WHERE dwe.doctorRegNo=@doctorRegNo ;";
            dtt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            dtt.table.TableName = "DoctorWorkExperience";
            dataSet.dataset.Tables.Add(dtt.table);

            query = @" SELECT da.awardId,da.awardName,da.awardYear                        	
                                   FROM doctoraward AS da  
                               WHERE da.doctorRegNo=@doctorRegNo ; ";
            dtt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            dtt.table.TableName = "DoctorAwards";
            dataSet.dataset.Tables.Add(dtt.table);

            query = @" SELECT dm.membershipId,dm.membershipName
                               FROM doctormembership AS dm  
                                WHERE dm.doctorRegNo=@doctorRegNo ; ";
            dtt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            dtt.table.TableName = "DoctorMembership";
            dataSet.dataset.Tables.Add(dtt.table);

            query = @"SELECT dac.addOnId,dac.certificateName,dac.year,dac.reason
                                   FROM doctoraddonscertification AS dac  
                               WHERE dac.doctorRegNo=@doctorRegNo ; ";
            dtt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            dtt.table.TableName = "DoctorAddOnsCertificate";
            dataSet.dataset.Tables.Add(dtt.table);

            query = @"SELECT di.indaminityId,di.isIndaminity,case when di.isIndaminity =1 then 'Yes' when di.isIndaminity = 0 then 
                                'No' END isIndaminityYesNo
                                   FROM doctorindaminity AS di  
                               WHERE di.doctorRegNo=@doctorRegNo ; ";
            dtt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            dtt.table.TableName = "DoctorIndeminity";
            dataSet.dataset.Tables.Add(dtt.table);

            query = @" SELECT hs.doctorSpecializationId,hs.specializationTypeId,hs.specializationTypeName,
			                        hs.specializationId,hs.specializationName,hs.levelOfCareId,hs.levelOfCareName
		                        FROM doctorspecialization AS hs
		                            WHERE hs.doctorRegNo=@doctorRegNo ; ";
            dtt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            dtt.table.TableName = "DoctorMultipleSpecialization";
            dataSet.dataset.Tables.Add(dtt.table);

            return dataSet;
        }
        public async Task<Int32> CheckDoctorDatewiseSchedule(Int64 doctorRegNo, Int16 month, Int32 year)
        {
            Int32 records = 0;
            string qr = @"SELECT COUNT(1) AS records FROM doctorscheduletimedatewise AS dstd
	                            WHERE dstd.doctorRegNo=@doctorRegNo AND MONTH(dstd.scheduleDate)=@month AND YEAR(dstd.scheduleDate)=@year";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("year", MySqlDbType.Int32) { Value = year });
            pm.Add(new MySqlParameter("month", MySqlDbType.Int16) { Value = month });
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });

            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(qr, pm.ToArray());
            if (dt.table.Rows.Count > 0)
            {
                records = Convert.ToInt32(dt.table.Rows[0]["records"]);
            }
            return records;
        }
        public async Task<ReturnClass.ReturnDataTable> GetDoctorDatewiseScheduleTime(Int64 doctorRegNo, Int16 month, Int32 year)
        {
            List<MySqlParameter> pm = new();
            string query = "";
            query = @"SELECT dstd.scheduleTimeId,dstd.doctorRegNo,dstd.scheduleDate,dstd.fromTime,dstd.toTime
		                         FROM doctorscheduletimedatewise AS dstd
	                        WHERE dstd.doctorRegNo=@doctorRegNo AND MONTH(dstd.scheduleDate)=@month AND YEAR(dstd.scheduleDate)=@year AND dstd.isActive= " + (Int16)YesNo.Yes +
                       @" ORDER BY dstd.scheduleDate,dstd.fromTime";
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });
            pm.Add(new MySqlParameter("month", MySqlDbType.Int16) { Value = month });
            pm.Add(new MySqlParameter("year", MySqlDbType.Int32) { Value = year });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }
        public async Task<ReturnClass.ReturnBool> saveBulkSlotDatewise(DoctorScheduleDatewise bl)
        {
            string query = "";
            ReturnClass.ReturnBool returnBool = new();
            List<DoctorScheduleDatewiseTime> blItems = new();
            string scheduleDate;
            blItems = bl.items;
            query = @" INSERT INTO doctorscheduletimedatewise(doctorRegNo, scheduleDate, fromTime, toTime,userId,isActive,clientIp) 
                                   VALUES ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo });
            pm.Add(new MySqlParameter("isActive", MySqlDbType.Int16) { Value = (Int16)YesNo.Yes });
            pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId });
            pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarChar) { Value = bl.clientIp });
            for (int i = 0; i < blItems.Count; i++)
            {
                scheduleDate = bl.items[i].scheduleDate.ToString().Replace('-', '/');
                scheduleDate = Convert.ToDateTime(scheduleDate).ToString("yyyy/MM/dd");
                pm.Add(new MySqlParameter("scheduleDate" + i.ToString(), MySqlDbType.VarChar) { Value = scheduleDate });
                pm.Add(new MySqlParameter("fromTime" + i.ToString(), MySqlDbType.VarChar) { Value = bl.items[i].fromTime });
                pm.Add(new MySqlParameter("toTime" + i.ToString(), MySqlDbType.VarChar) { Value = bl.items[i].toTime });

                query += "(@doctorRegNo,@scheduleDate" + i.ToString() + ", @fromTime" + i.ToString() + ", @toTime" + i.ToString() + "," +
                            " @userId,@isActive,@clientIp ),";
            }
            query = query.TrimEnd(',');
            using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                returnBool = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorscheduletimedatewise");
                if (returnBool.status)
                {
                    ts.Complete();
                    returnBool.status = true;
                    returnBool.message = "Time slot saved Successfully.";
                }
                else
                {
                    returnBool.status = false;
                    returnBool.error = "Could not save Time slot, " + returnBool.message;
                }
            }
            return returnBool;
        }
        public async Task<ReturnClass.ReturnBool> saveSingleSlotDatewise(DoctorScheduleDatewise bl)
        {
            MySqlParameter[] pm;
            string scheduleDate;
            string query = "";
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            if (bl.doctorRegNo == null)
                bl.doctorRegNo = 0;

            pm = new MySqlParameter[]
            {
                    new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                    new MySqlParameter("scheduleDate", MySqlDbType.VarChar) { Value = bl.items[0].scheduleDate },
                    new MySqlParameter("fromTime", MySqlDbType.VarChar) { Value = bl.items[0].fromTime },
                    new MySqlParameter("toTime", MySqlDbType.VarChar) { Value = bl.items[0].toTime },
            };
            query = @"SELECT scheduleTimeId
	                        FROM doctorscheduletimedatewise AS dstd
                          WHERE doctorRegNo = @doctorRegNo AND dstd.fromTime=@fromTime AND dstd.toTime=@toTime AND scheduleDate=@scheduleDate";
            dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count == 0)
            {
                scheduleDate = bl.items[0].scheduleDate.ToString().Replace('-', '/');
                scheduleDate = Convert.ToDateTime(scheduleDate).ToString("yyyy/MM/dd");
                pm = new MySqlParameter[]
                {
                        new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                        new MySqlParameter("scheduleDate", MySqlDbType.VarChar) { Value = scheduleDate },
                        new MySqlParameter("fromTime", MySqlDbType.VarChar) { Value = bl.items[0].fromTime },
                        new MySqlParameter("toTime", MySqlDbType.VarChar) { Value = bl.items[0].toTime },
                        new MySqlParameter("isActive", MySqlDbType.Int16) { Value = (Int16)YesNo.Yes },
                        new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                        new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = bl.clientIp },
                };
                query = @"INSERT INTO doctorscheduletimedatewise (doctorRegNo,scheduleDate,fromTime,toTime,userId,clientIp,isActive)
                                                      VALUES (@doctorRegNo,@scheduleDate,@fromTime,@toTime,@userId,@clientIp,@isActive)";
                using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorscheduletimedatewise");
                    if (rb.status)
                    {
                        ts.Complete();
                        rb.status = true;
                        rb.message = "Time slot for the Date is saved successfully.";
                    }
                    else
                    {
                        rb.status = false;
                        rb.error = "Could not save Time slot, " + rb.message;
                    }
                }
            }
            else
            {
                rb.status = false;
                rb.message = "Time slot for the Date is already exist.";
            }
            return rb;
        }
        public async Task<ReturnClass.ReturnBool> delSingleSlotDatewise(Int64 scheduleTimeId)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            string query = "";
            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    query = @"INSERT INTO doctorscheduletimedatewiselog
                                SELECT *
                                    FROM doctorscheduletimedatewise
                                WHERE scheduleTimeId=@scheduleTimeId";
                    MySqlParameter[] pm = new MySqlParameter[]
                        {
                            new MySqlParameter("scheduleTimeId", MySqlDbType.Int64) { Value = scheduleTimeId },
                        };
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorscheduletimedatewise");
                    if (rb.status == true)
                    {
                        query = @"DELETE FROM doctorscheduletimedatewise                                
                                    WHERE scheduleTimeId=@scheduleTimeId";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorscheduletimedatewise");
                        if (rb.status == true)
                        {
                            rb.message = "Time slot for the Date is Deleted successfully";
                            rb.value = Convert.ToString(scheduleTimeId);
                            ts.Complete();
                        }
                        else
                        {
                            rb.value = "0";
                            rb.message = "Failed to Delete Time slot," + rb.message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                rb.error = "Could not save Time slot for the Date, " + rb.message;
                rb.status = false;
            }
            return rb;
        }
        public async Task<ReturnClass.ReturnDataTable> GetAllDoctorListHome()
        {
            string query = @"SELECT dr.doctorRegNo,dr.doctorNameEnglish,dr.doctorNameLocal,dr.stateId,dr.districtId,dr.address,dr.mobileNo,
                                   dr.emailId,dr.active,s.stateNameEnglish AS stateName,d.districtNameEnglish AS districtName,
                                   GROUP_CONCAT(DISTINCT ds.specializationTypeName) AS specializationTypeName,
                                   dp.countryId,dp.countryName,ul.userName,dp.firstName,dp.middleName,dp.lastName,dp.phoneNumber,
                                   dp.genderName,DATE_FORMAT(dp.dateOfBirth,'%d/%m/%Y') AS dateOfBirth,dsdpt1.documentId, dsdpt1.documentName,dsdpt1.documentExtension,dp.pincode,
                                   dp.cityId,dp.cityName,IFNULL(dwa.consultancyTypeId,0) AS consultancyTypeId,IFNULL(dwa.consultancyTypeName,'') AS consultancyTypeName,
						            IFNULL(dwa.price,0) AS fee
                              FROM doctorregistration AS dr
                              INNER JOIN state AS s ON s.stateId=dr.stateId
				              INNER JOIN city AS ct ON dr.cityId=ct.cityId
           	                  INNER JOIN userlogin ul ON dr.doctorRegNo=ul.userId
                              LEFT JOIN district AS d ON d.districtId=dr.districtId
                              LEFT JOIN doctorprofile AS dp ON dr.doctorRegNo=dp.doctorRegNo
                              LEFT JOIN doctorspecialization AS ds ON dr.doctorRegNo=ds.doctorRegNo AND dr.registrationStatus=@isVerified
                              LEFT JOIN (
                                SELECT ds.documentId,ds.documentName,ds.documentExtension
                                  FROM documentstore AS ds 
                                  INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId 
                            AND dpt.documentType=" + (Int16)DocumentType.ProfilePic + @" AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Doctor + @"
                              ) AS dsdpt1 ON dsdpt1.documentId=dr.doctorRegNo
                             LEFT JOIN doctorworkarea AS dwa ON dr.doctorRegNo=dwa.doctorRegNo
                            GROUP BY dr.doctorRegNo ORDER BY dr.doctorNameLocal,specializationTypeName";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = (Int16)YesNo.Yes });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }
        public async Task<ReturnClass.ReturnDataTable> GetDoctorSlots(Int64 doctorRegNo)
        {
            string query = @"SELECT WEEKDAY(dstd.scheduleDate) weekDayId, dstd.scheduleTimeId,dstd.doctorRegNo,dstd.scheduleDate,dstd.fromTime,dstd.toTime
		                         FROM doctorscheduletimedatewise AS dstd
	                        WHERE dstd.doctorRegNo=@doctorRegNo AND DATE(scheduleDate) BETWEEN CURDATE() AND DATE(CURDATE()+6)
									 AND dstd.isActive=@isActive
                            ORDER BY dstd.scheduleDate,dstd.fromTime ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });
            pm.Add(new MySqlParameter("isActive", MySqlDbType.Int16) { Value = (Int16)YesNo.Yes });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }
        public async Task<ReturnClass.ReturnBool> AppointDoctor(BlDoctorAppointment bl)
        {
            MySqlParameter[] pm;
            ReturnClass.ReturnBool rb = new();
            string query = "";
            Int64 appointmentNo = await GetAppointmentNo();
            pm = new MySqlParameter[]
                {

                    new MySqlParameter("appointmentNo", MySqlDbType.Int64) { Value = appointmentNo },
                    new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                    new MySqlParameter("patientRegNo", MySqlDbType.Int64) { Value = bl.patientRegNo },
                    new MySqlParameter("scheduleDate", MySqlDbType.Date) { Value = bl.scheduleDate },
                    new MySqlParameter("scheduleTimeId", MySqlDbType.Int64) { Value = bl.scheduleTimeId },
                    new MySqlParameter("timeslot", MySqlDbType.VarChar,20) { Value = bl.timeslot },
                   // new MySqlParameter("firstName", MySqlDbType.VarChar,50) { Value = bl.firstName },
                    //new MySqlParameter("lastName", MySqlDbType.VarChar,50) { Value = bl.lastName },
                   // new MySqlParameter("emailId", MySqlDbType.VarChar,50) { Value = bl.emailId },
                   // new MySqlParameter("phoneNo", MySqlDbType.VarChar,15) { Value = bl.phoneNo },
                    new MySqlParameter("consultancyFee", MySqlDbType.Decimal) { Value = bl.consultancyFee },
                    new MySqlParameter("bookingFee", MySqlDbType.Decimal) { Value = bl.bookingFee },
                    new MySqlParameter("videoCallFee", MySqlDbType.Decimal) { Value = bl.videoCallFee },                   
                    new MySqlParameter("isActive", MySqlDbType.Int16) { Value = (Int16)YesNo.Yes },
                    new MySqlParameter("appointmentStatusId", MySqlDbType.Int16) { Value = (Int16)AppointmentStatus.PendingConfirmation},
                    new MySqlParameter("appointmentStatusName", MySqlDbType.VarChar) { Value = "Pending for Dr. Confirmation" },
                    new MySqlParameter("Remark", MySqlDbType.VarChar) { Value =  bl.remark},
                    new MySqlParameter("clientIp", MySqlDbType.VarChar) { Value = bl.clientIp },
                 };
            query = @"INSERT INTO patienttimeslotbooking (appointmentNo,doctorRegNo,patientRegNo,scheduleDate,scheduleTimeId,
                                                        timeslot,clientIp,isActive,appointmentStatusId
                                                        ,appointmentStatusName,remark)
                                    VALUES (@appointmentNo,@doctorRegNo,@patientRegNo,@scheduleDate,@scheduleTimeId,
                                                @timeslot,@clientIp,@isActive,@appointmentStatusId
                                                        ,@appointmentStatusName,@Remark)";

            using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "patienttimeslotbooking");
                if (rb.status)
                {
                    ts.Complete();
                    rb.status = true;
                    rb.message = "Appointment booked successfully";
                }
                else
                {
                    rb.status = false;
                    rb.error = "Failed to appoint Doctor, Please try again later.";
                }
            }
            return rb;
        }
        public async Task<Int64> GetAppointmentNo()
        {
            string appointmentNo = "0";
            try
            {
                string qr = @"SELECT IFNULL(MAX(SUBSTRING(ur.appointmentNo,5,12)),0) + 1 AS appointmentNo
        	                    FROM patienttimeslotbooking ur ";

                ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(qr);
                if (dt.table.Rows.Count > 0)
                {
                    appointmentNo = dt.table.Rows[0]["appointmentNo"].ToString()!;
                    appointmentNo = DateTime.Now.Year.ToString().Substring(2) +
                                    DateTime.Now.Month.ToString().PadLeft(2, '0') +
                                    appointmentNo!.PadLeft(8, '0');
                }

            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return Convert.ToInt64(appointmentNo);
        }
        public async Task<ReturnClass.ReturnDataTable> GetAllDoctorListHomeSpecialization(Int16 specializationId)
        {
            string query = @"SELECT dr.doctorRegNo,dr.doctorNameEnglish,dr.doctorNameLocal,dr.stateId,dr.districtId,dr.address,dr.mobileNo,
                                   dr.emailId,dr.active,s.stateNameEnglish AS stateName,d.districtNameEnglish AS districtName,
                                   dp.countryId,dp.countryName,ul.userName,dp.firstName,dp.middleName,dp.lastName,dp.phoneNumber,
                                   dp.genderName,DATE_FORMAT(dp.dateOfBirth,'%d/%m/%Y') AS dateOfBirth,dsdpt1.documentId, dsdpt1.documentName,dsdpt1.documentExtension,dp.pincode,
                                   dp.cityId,dp.cityName,IFNULL(dwa.consultancyTypeId,0) AS consultancyTypeId,IFNULL(dwa.consultancyTypeName,'') AS consultancyTypeName,
						            IFNULL(dwa.price,0) AS fee,ds.specializationTypeName AS hospitalSpecializationTypeName ,ds.specializationTypeId AS hospitalSpecializationTypeId,
                                    ds.specializationId AS hospitalSpecializationId,ds.specializationName AS hospitalSpecialization,
						            ds.levelOfCareId ,ds.levelOfCareName,dp.specialization AS doctorSpecialization
                              FROM doctorregistration AS dr
                              INNER JOIN state AS s ON s.stateId=dr.stateId
           	                  INNER JOIN userlogin ul ON dr.doctorRegNo=ul.userId
                              LEFT JOIN district AS d ON d.districtId=dr.districtId
                              LEFT JOIN doctorprofile AS dp ON dr.doctorRegNo=dp.doctorRegNo
                              LEFT JOIN doctorspecialization AS ds ON dr.doctorRegNo=ds.doctorRegNo AND dr.registrationStatus=@isVerified
                              LEFT JOIN (
                                SELECT ds.documentId,ds.documentName,ds.documentExtension
                                  FROM documentstore AS ds 
                                  INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND dpt.documentType=" + (Int16)DocumentType.ProfilePic + @" AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Doctor + @"
                              ) AS dsdpt1 ON dsdpt1.documentId=dr.doctorRegNo
                             LEFT JOIN doctorworkarea AS dwa ON dr.doctorRegNo=dwa.doctorRegNo
                             WHERE dp.specializationId=@specializationId
                            ORDER BY dr.doctorNameLocal";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = (Int16)YesNo.Yes });
            pm.Add(new MySqlParameter("specializationId", MySqlDbType.Int16) { Value = specializationId });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }
        public async Task<ReturnClass.ReturnBool> RollbackDocterRegistration(VerificationDoctorDetail verificationDetail)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            Int32 countData = 0;
            if (verificationDetail.VerificationDoctor.Count != 0)
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var item in verificationDetail.VerificationDoctor)
                    {
                        item.isVerified = YesNo.Yes;
                        bool isofficeExists = await CheckRollbackDoctor(item.doctorRegNo, (Int16)item.registrationStatus);
                        if (isofficeExists)
                        {
                            string query = @"UPDATE doctorregistration 
                             SET isVerified=@isVerified,verificationDate=NOW(),registrationStatus=@registrationStatus 
                              WHERE doctorRegNo=@doctorRegNo";

                            List<MySqlParameter> pm = new();
                            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = item.doctorRegNo });
                            pm.Add(new MySqlParameter("registrationStatus", MySqlDbType.Int16) { Value = YesNo.No });
                            pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = YesNo.No });

                            rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "RollbackDocterRegistration");
                            if (rb.status)
                            {
                                countData = countData + 1;
                            }
                        }
                    }
                    if (verificationDetail.VerificationDoctor.Count == countData)
                    {
                        ts.Complete();
                        rb.status = true;
                    }
                    else
                    {
                        rb.status = false;
                        rb.error = string.Empty;
                    }
                }
            }
            else
            {
                rb.message = "Doctor Data Empty..";
                rb.error = string.Empty;
            }
            return rb;
        }
        public async Task<bool> CheckRollbackDoctor(Int64 doctorRegNo, Int16 registrationStatus)
        {
            bool isDoctorExists = false;
            string query = @"SELECT h.doctorRegNo
                            FROM doctorregistration h
                            WHERE h.doctorRegNo = @doctorRegNo AND h.registrationStatus=@registrationStatus";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });
            pm.Add(new MySqlParameter("registrationStatus", MySqlDbType.Int16) { Value = registrationStatus });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
            {
                isDoctorExists = true;
            }
            return isDoctorExists;
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
        public async Task<ReturnClass.ReturnDataTable> GetDoctorHomeList()
        {
            string query = @"SELECT dr.doctorRegNo,dr.doctorNameEnglish,dr.doctorNameLocal,dr.stateId,dr.districtId,dr.address,dr.mobileNo,
                   dr.emailId,dr.active,s.stateNameEnglish AS stateName,d.districtNameEnglish AS districtName,dr.cityId,dr.cityName,
                   da.degreePgName,
                   ds.specializationTypeName,ds.specializationName,ds.levelOfCareName,
                   dwa.hospitalNameEnglish AS doctorWorkAreaHospital,
						 dwa.hospitalAddress AS doctorWorkAreaHospitalAddress,
						 dwa.consultancyTypeName,
                   dwa.price,dwa.districtName AS doctorWorkAreaDistrictName,dsd.Timings
              FROM doctorregistration AS dr 
				  	LEFT JOIN doctorworkarea AS dwa ON dr.doctorRegNo = dwa.doctorRegNo
               JOIN state AS s ON s.stateId=dr.stateId
           LEFT JOIN district AS d ON d.districtId=dr.districtId
			 	LEFT JOIN 
				 (	SELECT da.doctorRegNo,GROUP_CONCAT(DISTINCT da.degreePgName) AS degreePgName
					 		FROM doctoracademic AS da 
         		 INNER JOIN ddlcatlist AS cat ON cat.id = da.degreePgId AND (cat.category='Degree' OR cat.category='PG')
         		 		 GROUP BY da.doctorRegNo
         		) AS da ON dr.doctorRegNo=da.doctorRegNo
         		LEFT JOIN 
         		(
         			SELECT ds.doctorRegNo,GROUP_CONCAT(DISTINCT ds.specializationTypeName) AS specializationTypeName,
         				GROUP_CONCAT(DISTINCT ds.specializationName) AS specializationName,
         				GROUP_CONCAT(DISTINCT ds.levelOfCareName) AS levelOfCareName
         				FROM doctorspecialization AS ds  
         			INNER JOIN ddlcatlist AS cat1 ON cat1.id = ds.specializationId AND (cat1.category='hospitalSpecialization')
         			INNER JOIN ddlcatlist AS cat2 ON cat2.id = ds.specializationTypeId AND (cat2.category='specializationType')
         			INNER JOIN ddlcatlist AS cat3 ON cat3.id = ds.levelOfCareId AND (cat3.category='levelOfCare')
         			GROUP BY ds.doctorRegNo
         		)  AS ds ON dr.doctorRegNo=ds.doctorRegNo
         		LEFT JOIN (
                    SELECT dsd.doctorRegNo,GROUP_CONCAT(' ',dsd.`day`,' ',dst.fromTime,' - ',dst.toTime) AS Timings,dst.patientLimit
	                             FROM doctorscheduledate AS dsd 
 	                             INNER JOIN doctorscheduletime AS dst ON dsd.scheduleDateId = dst.scheduleDateId
                            WHERE   dsd.isActive=@isActive AND dst.isActive=@isActive 
                            ) AS dsd ON dr.doctorRegNo=dsd.doctorRegNo
          WHERE dr.registrationStatus=@isActive  
			  ORDER BY dr.doctorNameEnglish
          ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("isActive", MySqlDbType.Int16) { Value = (Int16)YesNo.Yes });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }
        public async Task<ReturnClass.ReturnDataTable> GetDoctorScheduleTimingsCalender(Int64 doctorRegNo)
        {
            string query = @"WITH RECURSIVE Date_Ranges AS (
                                SELECT ELT(DAYOFWEEK(NOW()), 'SUNDAY', 'MONDAY', 'TUESDAY', 'WEDNESDAY','THURSDAY', 'FRIDAY', 'SATURDAY')
	 	                            AS daysOfWeek,CURDATE() as Date
                               UNION ALL 
                               SELECT ELT(DAYOFWEEK(Date + INTERVAL 1 day), 'SUNDAY', 'MONDAY', 'TUESDAY', 'WEDNESDAY','THURSDAY', 'FRIDAY', 'SATURDAY')
		                            AS daysOfWeek,Date + INTERVAL 1 DAY
                               from Date_Ranges
                               where Date <  DATE_ADD(CURDATE(), INTERVAL 14 DAY))
	                            SELECT dstd.scheduleTimeId,dr.daysOfWeek,dr.Date,dsd.scheduleDateId,dst.scheduleTimeId,dsd.doctorRegNo,dsd.dayId,dsd.`day`,dst.fromTime,dst.toTime,dst.patientLimit
			                            ,CASE WHEN IFNULL(dstd.scheduleTimeId,0)=0 AND IFNULL(dst.scheduleDateId,0)!=0 THEN 'Avail' 
				                            WHEN IFNULL(dstd.scheduleTimeId,0)!=0 AND IFNULL(dst.scheduleDateId,0)!=0 THEN 'Not Available' ELSE
				                            'Not Scheduled' END AS Availability
                                    FROM doctorscheduledate AS dsd 
                                    INNER JOIN doctorscheduletime AS dst ON dsd.scheduleDateId = dst.scheduleDateId 
                                    RIGHT JOIN Date_Ranges AS dr ON dr.daysOfWeek=dsd.`day` AND dsd.doctorRegNo=@doctorRegNo
                                AND dsd.isActive=@isActive AND dst.isActive=@isActive
                                     LEFT JOIN doctorscheduletimedatewise AS dstd 
				                            ON dstd.scheduleTimeId=dst.scheduleTimeId AND dstd.scheduleDate = dr.date AND dsd.doctorRegNo=@doctorRegNo
                                ORDER BY dr.Date
                             ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });
            pm.Add(new MySqlParameter("isActive", MySqlDbType.Int16) { Value = YesNo.Yes });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }
        public async Task<ReturnClass.ReturnBool> NotAvailableDoctor(BlDoctorAvailability bl)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            string query = "";
            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    query = @"INSERT INTO doctorscheduletimedatewise(scheduleTimeId,doctorRegNo,scheduleDate,isActive,userId,clientIp)
                                         VALUES(@scheduleTimeId,@doctorRegNo,@date,@isActive,@userId,@clientIp)";
                    MySqlParameter[] pm = new MySqlParameter[]
                        {
                            new MySqlParameter("scheduleTimeId", MySqlDbType.Int64) { Value = bl.scheduleTimeId },
                            new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                            new MySqlParameter("date", MySqlDbType.Date) { Value = bl.date },
                            new MySqlParameter("clientIp", MySqlDbType.VarChar,100) { Value = bl.clientIp },
                            new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                        };
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorscheduletime");
                    if (rb.status == true)
                    {
                        rb.message = "Schedule Updated with No Availability.";
                        rb.value = Convert.ToString(bl.scheduleTimeId);
                        ts.Complete();
                    }
                    else
                    {
                        rb.value = "0";
                        rb.message = "Failed to Update Availability," + rb.message;
                    }
                }
            }
            catch (Exception ex)
            {
                rb.error = "Could not save Doctor Availability, " + rb.message;
                rb.status = false;
            }
            return rb;
        }
        public async Task<ReturnClass.ReturnBool> AvailableDoctor(BlDoctorAvailability bl)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            string query = "";
            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    query = @"INSERT INTO doctorscheduletimedatewiselog
                                            SELECT *
                                            FROM doctorscheduletimedatewise
                                            WHERE scheduleTimeId=@scheduleTimeId AND doctorRegNo=@doctorRegNo";
                    MySqlParameter[] pm = new MySqlParameter[]
                        {
                            new MySqlParameter("scheduleTimeId", MySqlDbType.Int64) { Value = bl.scheduleTimeId },
                            new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                        };
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorscheduletimedatewiselog");
                    if (rb.status == true)
                    {
                        query = @"DELETE FROM doctorscheduletimedatewise                                
                                            WHERE scheduleTimeId=@scheduleTimeId AND doctorRegNo=@doctorRegNo ";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "doctorscheduletimedatewise");
                        if (rb.status == true)
                        {
                            rb.message = "Availability Updated successfully";
                            rb.value = Convert.ToString(bl.scheduleTimeId);
                            ts.Complete();
                        }
                        else
                        {
                            rb.value = "0";
                            rb.message = "Failed to Delete Not Availability," + rb.message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                rb.error = "Could not save Availability, " + rb.message;
                rb.status = false;
            }
            return rb;
        }
        public async Task<ReturnClass.ReturnBool> saveMedicineMaster(MedicineMaster bl)
        {
            MySqlParameter[] pm;
            string query = "";
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();

            pm = new MySqlParameter[]
            {

                    new MySqlParameter("medicineName", MySqlDbType.VarChar) { Value = bl.medicineName },
                      new MySqlParameter("isActive", MySqlDbType.Int16) { Value = (Int16)YesNo.Yes},
                      new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                    new MySqlParameter("clientIp", MySqlDbType.VarChar) { Value = bl.clientIp }
            };
            query = @"INSERT INTO medicinemaster (medicineName,isActive,entryDateTime,userId,clientIp) 
                                    VALUES
                                    (@medicineName,@isActive,NOW(),@userId,@clientIp) ";
            rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "Savemedicinemaster");
            if (rb.status)
            {
                rb.status = true;
                rb.message = "Medicine has been Saved.";
            }
            else
            {
                rb.status = false;
                rb.error = "Could not save Medicine Master, " + rb.message;
            }

            return rb;
        }
        public async Task<ReturnClass.ReturnDataTable> GetMedicineList(Int16 roleId, Int64 userId)
        {
            string whr = "";
            if (roleId == (Int16)UserRole.Doctor)
                whr = " AND m.userId=@userId ";
            string query = @"SELECT m.medicineId,m.medicineName
                                FROM medicinemaster m
                                WHERE  m.isActive=@isActive " + whr + @"
                                ORDER BY m.medicineName;";

            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("isActive", MySqlDbType.Int16) { Value = (Int16)YesNo.Yes });
            pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = userId });
            return await db.ExecuteSelectQueryAsync(query, pm.ToArray());
        }
        public async Task<ReturnClass.ReturnDataTable> SearchMedicineByName(string medicineName)
        {
            string query = @"SELECT m.medicineName
                            FREOM medicinemaster m 
                            WHERE m.medicineName LIKE CONCAT(@medicineName,'%')  AND m.isActive=@isActive
                            ORDER BY m.medicineName;";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("isActive", MySqlDbType.Int16) { Value = (Int16)YesNo.Yes });
            pm.Add(new MySqlParameter("medicineName", MySqlDbType.VarChar) { Value = medicineName });
            return await db.ExecuteSelectQueryAsync(query, pm.ToArray());
        }
    }
}
