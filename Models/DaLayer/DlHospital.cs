using BaseClass;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using HospitalManagementApi.Models.BLayer;
using static HospitalManagementApi.Models.BLayer.BlCommon;
using Org.BouncyCastle.Crypto.Tls;
using static BaseClass.ReturnClass;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlHospital
    {
        readonly DBConnection db = new();
        ReturnClass.ReturnDataTable dt = new();
        ReturnClass.ReturnDataSet ds = new();
        DlCommon dlcommon = new();
        public async Task<ReturnClass.ReturnBool> RegisterNewHospital(BlHospital blHospital)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            if (blHospital.hospitalRegNo == null)
                blHospital.hospitalRegNo = 0;
            bool isExists = await CheckMobileExistAsync(blHospital.mobileNo, "INSERT", (Int64)blHospital.hospitalRegNo);

            if (!isExists)
            {
                isExists = await CheckEmailExistAsync(blHospital.emailId, "INSERT", (Int64)blHospital.hospitalRegNo);
                if (!isExists)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        string query = @"INSERT INTO hospitalregistration (hospitalRegNo,hospitalNameEnglish,hospitalNameLocal,stateId,districtId,address,mobileNo,
                                                 emailId,active,isVerified,verificationDate,verifiedByLoginId,registrationStatus,userId, 
                                                entryDateTime, clientIp,registrationYear)
                                        VALUES (@hospitalRegNo,@hospitalNameEnglish,@hospitalNameLocal,@stateId, @districtId, @address, @mobileNo,
                                                 @emailId,@active, @isVerified,@verificationDate,@verifiedByLoginId,@registrationStatus,@userId, 
                                                @entryDateTime,@clientIp,@registrationYear)";
                        blHospital.hospitalRegNo = await GetHospitalId((int)blHospital.registrationYear);


                        List<MySqlParameter> pm = new();
                        pm.Add(new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = blHospital.hospitalRegNo });
                        pm.Add(new MySqlParameter("hospitalNameEnglish", MySqlDbType.String) { Value = blHospital.hospitalNameEnglish });
                        pm.Add(new MySqlParameter("hospitalNameLocal", MySqlDbType.String) { Value = blHospital.hospitalNameLocal });
                        pm.Add(new MySqlParameter("stateId", MySqlDbType.Int16) { Value = blHospital.stateId });
                        pm.Add(new MySqlParameter("districtId", MySqlDbType.Int16) { Value = blHospital.districtId });
                        pm.Add(new MySqlParameter("address", MySqlDbType.String) { Value = blHospital.address });
                        pm.Add(new MySqlParameter("mobileNo", MySqlDbType.String) { Value = blHospital.mobileNo });
                        pm.Add(new MySqlParameter("emailId", MySqlDbType.String) { Value = blHospital.emailId });
                        pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = blHospital.active });
                        pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = blHospital.isVerified == null ? 0 : (Int16)blHospital.isVerified });
                        pm.Add(new MySqlParameter("verifiedByLoginId", MySqlDbType.Int64) { Value = blHospital.userId });
                        pm.Add(new MySqlParameter("registrationStatus", MySqlDbType.Int16) { Value = (Int16)RegistrationStatus.Approved });
                        pm.Add(new MySqlParameter("registrationYear", MySqlDbType.Int32) { Value = blHospital.registrationYear });
                        pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blHospital.clientIp });
                        pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blHospital.userId });
                        pm.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = blHospital.entryDateTime });

                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "HospitalRegistration");

                        //query = @"INSERT INTO userlogin (userName,userId,emailId,password,active,isDisabled,userTypeCode,userRole,entryDateTime)
                        //                VALUES (@hospitalNameEnglish,@hospitalRegNo,@emailId,@password, 1, 0, 1,1,@entryDateTime)";
                        //rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "userlogin");
                        if (rb.status)
                            ts.Complete();

                    }
                }
                else
                {
                    rb.message = " Email-Id has Already Used For Registration!!";
                }
            }
            else
            {
                rb.message = " Mobile no. has Already Used For Registration!!";
            }
            return rb;
        }

        public async Task<ReturnClass.ReturnBool> UpdateNewHospital(BlHospital blHospital)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            bool isExists = await CheckMobileExistAsync(blHospital.mobileNo, "UPDATE", (Int64)blHospital.hospitalRegNo);

            if (!isExists)
            {
                isExists = await CheckEmailExistAsync(blHospital.emailId, "UPDATE", (Int64)blHospital.hospitalRegNo);
                if (!isExists)
                {
                    List<MySqlParameter> pm = new();
                    pm.Add(new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = blHospital.hospitalRegNo });
                    pm.Add(new MySqlParameter("hospitalNameEnglish", MySqlDbType.String) { Value = blHospital.hospitalNameEnglish });
                    pm.Add(new MySqlParameter("hospitalNameLocal", MySqlDbType.String) { Value = blHospital.hospitalNameLocal });
                    pm.Add(new MySqlParameter("stateId", MySqlDbType.Int16) { Value = blHospital.stateId });
                    pm.Add(new MySqlParameter("districtId", MySqlDbType.Int16) { Value = blHospital.districtId });
                    pm.Add(new MySqlParameter("address", MySqlDbType.String) { Value = blHospital.address });
                    pm.Add(new MySqlParameter("mobileNo", MySqlDbType.String) { Value = blHospital.mobileNo });
                    pm.Add(new MySqlParameter("emailId", MySqlDbType.String) { Value = blHospital.emailId });
                    pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = blHospital.active });
                    pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = (int)blHospital.isVerified });
                    pm.Add(new MySqlParameter("verifiedByLoginId", MySqlDbType.Int64) { Value = blHospital.userId });
                    pm.Add(new MySqlParameter("registrationStatus", MySqlDbType.Int16) { Value = (Int16)RegistrationStatus.Approved });
                    pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blHospital.clientIp });
                    pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blHospital.userId });
                    pm.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = blHospital.entryDateTime });
                    string query = @"INSERT INTO hospitalregistrationlog
                                     SELECT * FROM  hospitalregistration h
                                       WHERE h.hospitalRegNo=@hospitalRegNo";
                    using (TransactionScope ts = new TransactionScope())
                    {
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "Hospitallog");
                        query = @"UPDATE hospitalregistration 
                                                 SET hospitalNameEnglish=@hospitalNameEnglish,hospitalNameLocal=@hospitalNameLocal,stateId=@stateId,
                                                districtId=@districtId,address=@address,mobileNo=@mobileNo,emailId=@emailId,active=@active,isVerified=@isVerified,
                                                verifiedByLoginId=@verifiedByLoginId,registrationStatus=@registrationStatus,userId=@userId, 
                                                entryDateTime=@entryDateTime,clientIp=@clientIp WHERE  hospitalRegNo=@hospitalRegNo";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "HospitalUpdate");
                        if (rb.status == true)
                        {
                            ts.Complete();
                        }
                    }
                }
                else
                {
                    rb.message = " Email-Id has Already Used For Registration!!";
                }
            }
            else
            {
                rb.message = " Mobile no. has Already Used For Registration!!";
            }
            return rb;
        }
        /// <summary>
        /// Returns 12 digit hospitalId id based on year
        /// </summary>
        /// <param name="registrationYear"></param>
        /// <returns></returns>
        public async Task<Int64> GetHospitalId(int registrationYear)
        {
            string hospitalId = "0";
            try
            {
                string qr = @"SELECT IFNULL(MAX(SUBSTRING(ur.hospitalRegNo,6,12)),0) + 1 AS hospitalId
								FROM hospitalregistration ur 
							WHERE ur.registrationYear = @registrationYear";

                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("registrationYear", MySqlDbType.Int32) { Value = registrationYear });

                ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(qr, pm.ToArray());
                if (dt.table.Rows.Count > 0)
                {
                    hospitalId = dt.table.Rows[0]["hospitalId"].ToString();
                    hospitalId = ((int)idPrefix.officeId).ToString() + registrationYear.ToString() + hospitalId.PadLeft(7, '0');
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return Convert.ToInt64(hospitalId);
        }
        public async Task<bool> CheckEmailExistAsync(string emailId, string transType, Int64 hospitalRegNo)
        {
            bool isAccountExists = false;
            string query = @"SELECT u.emailId
                            FROM hospitalregistration u
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
                            FROM hospitalregistration u
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
        public async Task<ReturnClass.ReturnDataTable> GetAllHospitalList(Int16 vId)
        {
            string query = @"SELECT  h.hospitalRegNo,h.hospitalNameEnglish,h.hospitalNameLocal,h.stateId,h.districtId,h.address,h.mobileNo,
                                     h.emailId,h.active,s.stateNameEnglish AS stateName,d.districtNameEnglish AS districtName
                                FROM hospitalregistration h
                                 JOIN state s ON s.stateId=h.stateId
				                 JOIN district d ON d.districtId=h.districtId
                            WHERE h.isVerified=@isVerified";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = vId });

            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }
        public async Task<ReturnClass.ReturnDataTable> GetHospitalById(Int64 hospitalRegNo)
        {
            string query = @"SELECT  h.hospitalRegNo,h.hospitalNameEnglish,h.hospitalNameLocal,h.stateId,h.districtId,h.address,h.mobileNo,
                                     h.emailId,h.active,s.stateNameEnglish AS stateName,d.districtNameEnglish AS districtName
                                FROM hospitalregistration h
                                 JOIN state s ON s.stateId=h.stateId
				                 JOIN district d ON d.districtId=h.districtId
                            WHERE h.hospitalRegNo=@hospitalRegNo ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo });

            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }

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

        public async Task<ReturnClass.ReturnBool> VerifyHospital(VerificationDetail verificationDetail)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            Int32 countData = 0;
            string pass = "";
            if (verificationDetail.VerificationHospital.Count != 0)
            {
                foreach (var item in verificationDetail.VerificationHospital)
                {
                    if (item.registrationStatus == RegistrationStatus.Approved)
                    {
                        item.isVerified = YesNo.Yes;
                    }
                    else
                    {
                        item.isVerified = YesNo.No;
                    }
                    bool isofficeExists = await CheckVerifyHospital(item.hospitalRegNo, (Int16)item.isVerified);
                    if (!isofficeExists)
                    {
                        string query = @"UPDATE hospitalregistration 
                             SET isVerified=@isVerified,clientIp=@clientIp,verificationDate=@verificationDate,
                                verifiedByLoginId=@verifiedByLoginId,registrationStatus=@registrationStatus,active=@active 
                              WHERE hospitalRegNo=@hospitalRegNo";


                        pass = "HRP" + getrandom();
                        string hash_Pass = sha256_hash(pass);
                        List<MySqlParameter> pm = new();
                        pm.Add(new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = item.hospitalRegNo });
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
                        pm.Add(new MySqlParameter("userRole", MySqlDbType.Int16) { Value = (int)UserRole.Hospital });
                        pm.Add(new MySqlParameter("isSingleWindowUser", MySqlDbType.Int16) { Value = (int)Active.No });
                        pm.Add(new MySqlParameter("modificationType", MySqlDbType.Int16) { Value = (int)Active.No });
                        pm.Add(new MySqlParameter("userTypeCode", MySqlDbType.Int16) { Value = (int)Active.No });
                        pm.Add(new MySqlParameter("Password", MySqlDbType.String) { Value = hash_Pass });
                        using (TransactionScope ts = new TransactionScope())
                        {
                            rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "VerifyHospital");
                            if (rb.status == true && item.registrationStatus == RegistrationStatus.Approved && item.isVerified == YesNo.Yes)
                            {
                                query = @"INSERT INTO userlogin
                                            (userName,userId,emailId,password,changePassword,active,isDisabled,
                                            clientIp,userRole,registrationYear,isSingleWindowUser,modificationType,userTypeCode)
                                        SELECT h.hospitalNameEnglish,h.hospitalRegNo,h.emailId,@Password,@changePassword,
                                        @active1,@isDisabled,@clientIp,@userRole,@registrationYear,@isSingleWindowUser,
                                        @modificationType,@userTypeCode
                                            FROM  hospitalregistration h 
                                          WHERE h.hospitalRegNo=@hospitalRegNo";
                                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "InsertUserLogin");
                                if (rb.status)
                                {
                                    ts.Complete();
                                    countData = countData + 1;
                                    rb.error = pass;
                                }
                            }
                            else if (rb.status == true && item.registrationStatus == RegistrationStatus.Reject)
                            {
                                ts.Complete();
                                countData = countData + 1;
                                rb.error = string.Empty;
                            }


                        }
                    }
                    else
                    {
                        countData = 0;
                        rb.message = "This Hospital has Already Verified!!";
                        rb.error = string.Empty;
                    }
                }

                if (verificationDetail.VerificationHospital.Count == countData)
                {
                    rb.status = true;
                    rb.error = pass;
                }
                else
                {
                    rb.status = false;
                    rb.error = string.Empty;
                }
            }
            else
            {

                rb.message = "Hospital Data Empty!!";
                rb.error = string.Empty;
            }
            return rb;
        }

        public async Task<bool> CheckVerifyHospital(Int64 hospitalRegNo, Int16 isVerified)
        {
            bool isHospitalExists = false;
            string query = @"SELECT h.hospitalRegNo
                            FROM hospitalregistration h
                            WHERE h.hospitalRegNo = @hospitalRegNo AND h.isVerified=@isVerified ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo });
            pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = isVerified });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
            {
                isHospitalExists = true;
            }
            return isHospitalExists;
        }
        public async Task<bool> checkOldPassword(ResetPassword resetPassword)
        {
            bool isExists = false;
            string query = @"SELECT  l.password
                                  FROM userlogin l
                                  WHERE l.userId=@hospitalRegNo AND l.active = @active ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = resetPassword.userId });
            pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = (Int16)Active.Yes });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
            {
                if (dt.table.Rows[0]["password"].ToString().Equals(resetPassword.oldPasssword, StringComparison.InvariantCultureIgnoreCase))
                {
                    isExists = true;
                }
            }
            return isExists;
        }


        public string getrandom()
        {
            string rno;
            Random randomclass = new Random();
            Int32 rno1 = randomclass.Next(10000, 99999);
            string random = Convert.ToString(rno1);
            rno = random.ToString();
            return rno;
        }

        public async Task<ReturnClass.ReturnBool> ResetPassword(ResetPassword resetPassword)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            bool isExists = await checkOldPassword(resetPassword);
            if (isExists)
            {
                string query = @"Update userlogin  SET changePassword=@changePassword,password=@password
                              WHERE userId=@hospitalRegNo";
                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = resetPassword.userId });
                pm.Add(new MySqlParameter("changePassword", MySqlDbType.Int16) { Value = (int)Active.Yes });
                pm.Add(new MySqlParameter("Password", MySqlDbType.String) { Value = resetPassword.Passsword });

                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "InsertUserLogin");
                if (rb.status)
                {


                    rb.message = "  Successfully Password Reset";
                }

                else
                {
                    rb.message = " Somthing Went Wrong Please Try Again!!!";
                }
            }
            else
            {
                rb.message = "Old Password Not Matched!!";
                rb.error = string.Empty;
            }



            return rb;
        }

        public async Task<ReturnClass.ReturnBool> UpdateHospitalInfo(BlHospital blHospital)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            if (blHospital.hospitalRegNo == 0)
            {
                rb.message = " Invalid Hospital Registration Id";
                rb.status = false;
                return rb;
            }
            using (TransactionScope ts = new TransactionScope())
            {
                string query = @"UPDATE hospitalregistration 
                                          SET cityId=@cityId,pinCode=@pinCode,phoneNumber=@phoneNumber,landMark=@landMark,fax=@fax,isCovid=@isCovid,
                                            latitude=@latitude,longitude=@longitude,typeOfProviderId=@typeOfProviderId,website=@website,natureOfEntityId=@natureOfEntityId,
                                            clientIp=@clientIp,userId=@userId,lastUpdate=@lastUpdate,rohiniId=@rohiniId
                                          WHERE hospitalRegNo=@hospitalRegNo";
                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = blHospital.hospitalRegNo });
                pm.Add(new MySqlParameter("cityId", MySqlDbType.Int32) { Value = blHospital.cityId });
                pm.Add(new MySqlParameter("pinCode", MySqlDbType.VarChar, 6) { Value = blHospital.pinCode });
                pm.Add(new MySqlParameter("phoneNumber", MySqlDbType.VarChar, 15) { Value = blHospital.phoneNumber });
                pm.Add(new MySqlParameter("landMark", MySqlDbType.VarChar, 50) { Value = blHospital.landMark });
                pm.Add(new MySqlParameter("fax", MySqlDbType.VarChar, 15) { Value = blHospital.fax });
                pm.Add(new MySqlParameter("isCovid", MySqlDbType.Int16) { Value = blHospital.isCovid });
                pm.Add(new MySqlParameter("latitude", MySqlDbType.Decimal) { Value = blHospital.latitude });
                pm.Add(new MySqlParameter("longitude", MySqlDbType.Decimal) { Value = blHospital.longitude });
                pm.Add(new MySqlParameter("typeOfProviderId", MySqlDbType.Int16) { Value = blHospital.typeOfProviderId });
                pm.Add(new MySqlParameter("website", MySqlDbType.VarChar, 50) { Value = blHospital.website });
                pm.Add(new MySqlParameter("natureOfEntityId", MySqlDbType.Int16) { Value = blHospital.natureOfEntityId });
                pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blHospital.clientIp });
                pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blHospital.userId });
                pm.Add(new MySqlParameter("lastUpdate", MySqlDbType.String) { Value = blHospital.entryDateTime });
                pm.Add(new MySqlParameter("rohiniId", MySqlDbType.VarChar, 20) { Value = blHospital.rohiniId });
                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "HospitalRegistration");
                ts.Complete();
            }
            return rb;
        }

        public async Task<ReturnClass.ReturnDataTable> SearchHS(Filter filter)
        {
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                    new MySqlParameter("hospitalSpec", MySqlDbType.VarChar,500) { Value = "%" +filter.hospitalSpec +"%" },
                    new MySqlParameter("districtId", MySqlDbType.Int16) { Value = filter.districtId },
                   };
                string qr = @"  SELECT  hr.hospitalRegNo,hr.hospitalNameEnglish,hr.districtId,hr.address,hr.mobileNo,hr.emailId,hr.registrationYear
		                                ,hr.cityId,hr.pinCode,hr.phoneNumber,hr.landMark,hr.fax,hr.isCovid,hr.latitude,hr.longitude,hr.typeOfProviderId,hr.website,hr.natureOfEntityId
		                                ,hs.specializationTypeName,hs.specializationName,hs.levelOfCareName 
	                                FROM hospitalregistration AS hr 
		                            LEFT JOIN hospitalspecialization AS hs ON hr.hospitalRegNo = hs.hospitalRegNo
		                            WHERE ( hr.hospitalNameEnglish LIKE @hospitalSpec OR hs.specializationName LIKE @hospitalSpec ) AND hr.districtId=@districtId 
                                          AND hr.active=" + (Int16)YesNo.Yes + @" AND hr.isVerified = " + (Int16)YesNo.Yes;
                dt = await db.ExecuteSelectQueryAsync(qr, pm);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public async Task<ReturnClass.ReturnDataTable> GetHospitalDoc(Int64 hospitalRegNo)
        {
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                     new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                   };
                string qr = @" SELECT CONCAT(REPLACE(dp.physicalPath, 'D:', ''),ds.documentId,'/',ds.documentName,ds.documentExtension) AS preview,
                                        dp.documentType,ds.documentName,ds.documentExtension,dp.documentId,dp.documentImageGroup,dp.dptTableId 
                                   FROM documentstore AS ds
                               INNER JOIN documentpathtbl AS dp ON dp.dptTableId = ds.dptTableId 
		                            AND dp.documentType IN ('" + (Int16)DocumentType.ProfilePic + @"','" + (Int16)DocumentType.ProfileLogo + @"','" +
                                    (Int16)DocumentType.ProfileDocument + @"','" + (Int16)DocumentType.NACH + @"','" + (Int16)DocumentType.License +
                                    @"','" + (Int16)DocumentType.OtherDocument + @"') 
                                    AND dp.documentImageGroup=@" + (Int16)DocumentImageGroup.Hospitlal + @"'
	                               AND ds.documentId=@hospitalRegNo ORDER BY dp.documentImageGroup";
                dt = await db.ExecuteSelectQueryAsync(qr, pm);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public async Task<ReturnClass.ReturnBool> UpdateHospitalInfoMI(BlHospitalMI bl)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            MySqlParameter[] pm; string query = "";
            if (bl.hospitalRegNo == 0)
            {
                rb.message = " Invalid Hospital Registration Id";
                rb.status = false;
                return rb;
            }
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    pm = new MySqlParameter[]
                    {
                        new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo },
                     };
                    query = @"DELETE FROM maincontact 
                                WHERE hospitalRegNo = @hospitalRegNo";
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "maincontact");

                    query = @"DELETE FROM hospitaldocuments 
                                WHERE hospitalRegNo = @hospitalRegNo";
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "hospitaldocuments");

                    foreach (var item in bl.BlMC)
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
                        query = @"INSERT INTO maincontact (hospitalRegNo,designationId,designationName,contactPersonName,mobileNo,emailId,entryDateTime,userId)
                                        VALUES (@hospitalRegNo,@designationId,@designationName,@contactPersonName, @mobileNo, @emailId,@entryDateTime,@userId)";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "maincontact");
                    }
                    query = @"UPDATE hospitalregistration 
                                    SET cityId=@cityId,pinCode=@pinCode,phoneNumber=@phoneNumber,landMark=@landMark,fax=@fax,isCovid=@isCovid,
                                    latitude=@latitude,longitude=@longitude,typeOfProviderId=@typeOfProviderId,website=@website,natureOfEntityId=@natureOfEntityId,
                                    clientIp=@clientIp,userId=@userId,lastUpdate=@lastUpdate,rohiniId=@rohiniId,typeOfProviderName=@typeOfProviderName,natureOfEntityName=@natureOfEntityName,cityName=@cityName,
                                    hospitalTypeId=@hospitalTypeId,hospitalTypeName=@hospitalTypeName
                                WHERE hospitalRegNo=@hospitalRegNo";
                    List<MySqlParameter> pm2 = new();
                    pm2.Add(new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo });
                    pm2.Add(new MySqlParameter("cityId", MySqlDbType.Int32) { Value = bl.cityId });
                    pm2.Add(new MySqlParameter("pinCode", MySqlDbType.VarChar, 6) { Value = bl.pinCode });
                    pm2.Add(new MySqlParameter("phoneNumber", MySqlDbType.VarChar, 15) { Value = bl.phoneNumber });
                    pm2.Add(new MySqlParameter("landMark", MySqlDbType.VarChar, 50) { Value = bl.landMark });
                    pm2.Add(new MySqlParameter("fax", MySqlDbType.VarChar, 15) { Value = bl.fax });
                    pm2.Add(new MySqlParameter("isCovid", MySqlDbType.Int16) { Value = bl.isCovid });
                    pm2.Add(new MySqlParameter("latitude", MySqlDbType.Decimal) { Value = bl.latitude });
                    pm2.Add(new MySqlParameter("longitude", MySqlDbType.Decimal) { Value = bl.longitude });
                    pm2.Add(new MySqlParameter("typeOfProviderId", MySqlDbType.Int16) { Value = bl.typeOfProviderId });
                    pm2.Add(new MySqlParameter("website", MySqlDbType.VarChar, 50) { Value = bl.website });
                    pm2.Add(new MySqlParameter("natureOfEntityId", MySqlDbType.Int16) { Value = bl.natureOfEntityId });
                    pm2.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = bl.clientIp });
                    pm2.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId });
                    pm2.Add(new MySqlParameter("lastUpdate", MySqlDbType.String) { Value = bl.entryDateTime });
                    pm2.Add(new MySqlParameter("rohiniId", MySqlDbType.VarChar, 20) { Value = bl.rohiniId });
                    pm2.Add(new MySqlParameter("typeOfProviderName", MySqlDbType.VarChar, 200) { Value = bl.typeOfProviderName });
                    pm2.Add(new MySqlParameter("natureOfEntityName", MySqlDbType.VarChar, 200) { Value = bl.natureOfEntityName });
                    pm2.Add(new MySqlParameter("cityName", MySqlDbType.VarChar, 200) { Value = bl.cityName });
                    pm2.Add(new MySqlParameter("hospitalTypeId", MySqlDbType.Int16) { Value = bl.hospitalTypeId });
                    pm2.Add(new MySqlParameter("hospitalTypeName", MySqlDbType.VarChar, 100) { Value = bl.hospitalTypeName });

                    rb = await db.ExecuteQueryAsync(query, pm2.ToArray(), "hospitalregistration");

                    DateTime licenseExpiryDate = DateTime.ParseExact(bl.licenseExpiryDate.Replace('-', '/'), "dd/MM/yyyy", null);
                    bl.licenseExpiryDate = licenseExpiryDate.ToString("yyyy/MM/dd");

                    List<MySqlParameter> pm3 = new();
                    pm3.Add(new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo });
                    //pm3.Add(new MySqlParameter("hospitalRegistrationNo", MySqlDbType.VarChar, 20) { Value = bl.hospitalRegistrationNo });
                    pm3.Add(new MySqlParameter("licenseExpiryDate", MySqlDbType.VarChar, 20) { Value = bl.licenseExpiryDate });
                    pm3.Add(new MySqlParameter("NABHCertificationLevel", MySqlDbType.VarChar, 50) { Value = bl.NABHCertificationLevel });
                    pm3.Add(new MySqlParameter("registeredWith", MySqlDbType.VarChar, 100) { Value = bl.registeredWith });
                    pm3.Add(new MySqlParameter("anyOtherCertification", MySqlDbType.VarChar, 100) { Value = bl.anyOtherCertification });
                    pm3.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId });
                    pm3.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime });
                    query = @"INSERT INTO hospitaldocuments (hospitalRegNo,hospitalRegistrationNo,licenseExpiryDate,NABHCertificationLevel,registeredWith,anyOtherCertification,entryDateTime,userId)
                                                     VALUES (@hospitalRegNo,@hospitalRegistrationNo,@licenseExpiryDate,@NABHCertificationLevel,@registeredWith,@anyOtherCertification,@entryDateTime,@userId)";
                    rb = await db.ExecuteQueryAsync(query, pm3.ToArray(), "hospitaldocuments");
                }
                catch (Exception ex)
                {
                    rb.message = "Could not save , " + ex.Message;
                    rb.status = false;
                }
                if (rb.status)
                    ts.Complete();
            }
            return rb;
        }

        public async Task<ReturnClass.ReturnDataSet> GetHospitalInfoMI(Int64 hospitalRegNo)
        {
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                     new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                   };
                string qr = @" SELECT designationId,designationName,contactPersonName,mobileNo,emailId 
			                            FROM maincontact WHERE hospitalRegNo=@hospitalRegNo;
                            SELECT hospitalNameEnglish,stateId,districtId,address,mobileNo,emailId,cityId,pinCode,
		                             phoneNumber,landMark,fax,isCovid,latitude,longitude,typeOfProviderId,website,
		                             natureOfEntityId,rohiniId,hospitalTypeName,hospitalTypeId
		 	                            FROM hospitalregistration 
			                            WHERE hospitalRegNo=@hospitalRegNo;
                            SELECT DATE_FORMAT(licenseExpiryDate,'%d/%m/%Y')AS licenseExpiryDate ,NABHCertificationLevel,registeredWith,anyOtherCertification 
		                            FROM hospitaldocuments 
		                                WHERE hospitalRegNo=@hospitalRegNo;";
                ds = await db.executeSelectQueryForDataset_async(qr, pm);
            }
            catch (Exception ex)
            {
            }
            return ds;
        }

        public async Task<ReturnClass.ReturnDataTable> GetHospitalFinancialInfo(Int64 hospitalRegNo)
        {
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                     new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                   };
                string qr = @" 
                            SELECT accountNumber,beneficiaryName,accountTypeId,accountTypeName,
		                            bankName,bankAddress,IFSCCode,PANNo,nameOnPAN
		                        FROM financialinformation 
		                    WHERE hospitalRegNo=@hospitalRegNo;";
                dt = await db.ExecuteSelectQueryAsync(qr, pm);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }
        public async Task<ReturnBool> UploadDocument(HospitalDocs bl)
        {
            bool allFilesUploaded = false;
            string url = "";
            ReturnDataTable dt = new ReturnDataTable();
            ReturnBool succeded = new ReturnBool();
            succeded.status = true;
            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (succeded.status == true)
                    {
                        url = "WorkDocs/uploaddocs";
                        Int16 i = 0;
                        foreach (var item in bl.BlDocument)
                        {
                            i++;
                            item.userId = bl.userId;
                            item.documentId = bl.hospitalRegNo;
                            succeded = await dlcommon.callStoreAPI(item, url);
                            bl.BlDocument[i - 1].documentName = succeded.message;
                            if (succeded.status == false)
                            {
                                bl.BlDocument[i - 1].uploaded = 0;
                                allFilesUploaded = false;
                            }
                        }
                        foreach (var item in bl.BlDocument)
                        {
                            if (bl.BlDocument[i - 1].uploaded == 0)
                                allFilesUploaded = false;
                            else
                                allFilesUploaded = true;
                        }
                    }
                    if (allFilesUploaded == true)
                    {
                        succeded.message = "Uploaded Successfully.";
                        succeded.status = true;
                        ts.Complete();
                    }
                    else
                    {
                        Int16 i = 0;
                        url = "WorkDocs/deleteanydoc";
                        foreach (var item in bl.BlDocument)
                        {
                            if (item.uploaded == 1)
                            {
                                i++;
                                bl.userId = bl.userId;
                                item.documentId = (Int64)item.documentId;
                                succeded = await dlcommon.callStoreAPI(item, url);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                succeded.message = "Could not upload image , " + ex.Message;
                succeded.status = false;
            }
            return succeded;
        }

        public async Task<ReturnClass.ReturnDataSet> GetHospitalInfoMIDoc(Int64 hospitalRegNo)
        {
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                     new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                   };
                string qr = @" SELECT hr.hospitalRegNo,dsdpt1.documentIdProfilePic,dsdpt1.documentNameProfilePic,dsdpt1.documentExtensionProfilePic
								        ,dsdpt2.previewProfileLogo,dsdpt2.documentIdProfileLogo,dsdpt2.documentNameProfileLogo,dsdpt2.documentExtensionProfileLogo
								        ,dsdpt3.previewProfileDoc,dsdpt3.documentIdProfileDoc,dsdpt3.documentNameProfileDoc,dsdpt3.documentExtensionProfileDoc
								        ,dsdpt4.previewNACH,dsdpt4.documentIdNACH,dsdpt4.documentNameNACH,dsdpt4.documentExtensionNACH
								        ,dsdpt5.previewLicense,dsdpt5.documentIdLicense,dsdpt5.documentNameLicense,dsdpt5.documentExtensionLicense
								        ,dsdpt6.previewOther,dsdpt6.documentIdOther,dsdpt6.documentNameOther,dsdpt6.documentExtensionOther
								        ,dsdpt7.previewPAN,dsdpt7.documentIdPAN,dsdpt7.documentNamePAN,dsdpt7.documentExtensionPAN
		                                    FROM hospitalregistration AS hr
			                                LEFT JOIN (
   			                                    SELECT CONCAT(REPLACE(dpt.physicalPath, 'D:', ''),ds.documentId,'/',ds.documentName,ds.documentExtension) AS previewProfilePic,ds.documentId AS documentIdProfilePic,ds.documentName AS documentNameProfilePic,ds.documentExtension AS documentExtensionProfilePic
				 	                                FROM documentstore AS ds 
                                                INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND dpt.documentType=" + (Int16)DocumentType.ProfilePic + @" AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Hospitlal + @"
                                            ) AS dsdpt1 ON dsdpt1.documentIdProfilePic=hr.hospitalRegNo
                                            LEFT JOIN (
   			                                    SELECT CONCAT(REPLACE(dpt.physicalPath, 'D:', ''),ds.documentId,'/',ds.documentName,ds.documentExtension) AS previewProfileLogo,ds.documentId AS documentIdProfileLogo,ds.documentName AS documentNameProfileLogo,ds.documentExtension AS documentExtensionProfileLogo
				 	                                FROM documentstore AS ds 
                                                INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND dpt.documentType=" + (Int16)DocumentType.ProfileLogo + @" AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Hospitlal + @"
                                            ) AS dsdpt2 ON dsdpt2.documentIdProfileLogo=hr.hospitalRegNo
                                            LEFT JOIN (
   			                                    SELECT CONCAT(REPLACE(dpt.physicalPath, 'D:', ''),ds.documentId,'/',ds.documentName,ds.documentExtension) AS previewProfileDoc,ds.documentId AS documentIdProfileDoc,ds.documentName AS documentNameProfileDoc,ds.documentExtension AS documentExtensionProfileDoc
				 	                                FROM documentstore AS ds 
                                                INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND dpt.documentType=" + (Int16)DocumentType.ProfileDocument + @" AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Hospitlal + @"
                                            ) AS dsdpt3 ON dsdpt3.documentIdProfileDoc=hr.hospitalRegNo
                                            LEFT JOIN (
   			                                    SELECT CONCAT(REPLACE(dpt.physicalPath, 'D:', ''),ds.documentId,'/',ds.documentName,ds.documentExtension) AS previewNACH,ds.documentId AS documentIdNACH,ds.documentName AS documentNameNACH,ds.documentExtension AS documentExtensionNACH
				 	                                FROM documentstore AS ds 
                                                INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND dpt.documentType=" + (Int16)DocumentType.NACH + @"  AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Hospitlal + @"
                                            ) AS dsdpt4 ON dsdpt4.documentIdNACH=hr.hospitalRegNo
                                            LEFT JOIN (
   			                                    SELECT CONCAT(REPLACE(dpt.physicalPath, 'D:', ''),ds.documentId,'/',ds.documentName,ds.documentExtension) AS previewLicense,ds.documentId AS documentIdLicense,ds.documentName AS documentNameLicense,ds.documentExtension AS documentExtensionLicense
				 	                                FROM documentstore AS ds 
                                                INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND dpt.documentType=" + (Int16)DocumentType.License + @"  AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Hospitlal + @"
                                            ) AS dsdpt5 ON dsdpt5.documentIdLicense=hr.hospitalRegNo
                                            LEFT JOIN (
   			                                    SELECT CONCAT(REPLACE(dpt.physicalPath, 'D:', ''),ds.documentId,'/',ds.documentName,ds.documentExtension) AS previewOther,ds.documentId AS documentIdOther,ds.documentName AS documentNameOther,ds.documentExtension AS documentExtensionOther
				 	                                FROM documentstore AS ds 
                                                INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND dpt.documentType=" + (Int16)DocumentType.OtherDocument + @"  AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Hospitlal + @"
                                            ) AS dsdpt6 ON dsdpt6.documentIdOther=hr.hospitalRegNo
                                              LEFT JOIN (
   			                                    SELECT CONCAT(REPLACE(dpt.physicalPath, 'D:', ''),ds.documentId,'/',ds.documentName,ds.documentExtension) AS previewPAN,ds.documentId AS documentIdPAN,ds.documentName AS documentNamePAN,ds.documentExtension AS documentExtensionPAN
				 	                                FROM documentstore AS ds 
                                                INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND dpt.documentType=" + (Int16)DocumentType.HospitalPAN + @"  AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Hospitlal + @"
                                            ) AS dsdpt7 ON dsdpt7.documentIdPAN=hr.hospitalRegNo
                                WHERE hr.hospitalRegNo=@hospitalRegNo ;";
                ds = await db.executeSelectQueryForDataset_async(qr, pm);
            }
            catch (Exception ex)
            {
            }
            return ds;
        }

        public async Task<ReturnClass.ReturnDataSet> GetHospitalProfileLogo(Int64 hospitalRegNo)
        {
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                     new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                   };
                string qr = @" SELECT hr.hospitalRegNo,dsdpt1.documentIdProfilePic,dsdpt1.documentNameProfilePic,dsdpt1.documentExtensionProfilePic
								        ,dsdpt2.previewProfileLogo,dsdpt2.documentIdProfileLogo,dsdpt2.documentNameProfileLogo,dsdpt2.documentExtensionProfileLogo
		                                    FROM hospitalregistration AS hr
			                                LEFT JOIN (
   			                                    SELECT CONCAT(REPLACE(dpt.physicalPath, 'D:', ''),ds.documentId,'/',ds.documentName,ds.documentExtension) AS previewProfilePic,ds.documentId AS documentIdProfilePic,ds.documentName AS documentNameProfilePic,ds.documentExtension AS documentExtensionProfilePic
				 	                                FROM documentstore AS ds 
                                                INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND dpt.documentType=" + (Int16)DocumentType.ProfilePic + @" AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Hospitlal + @"
                                            ) AS dsdpt1 ON dsdpt1.documentIdProfilePic=hr.hospitalRegNo
                                            LEFT JOIN (
   			                                    SELECT CONCAT(REPLACE(dpt.physicalPath, 'D:', ''),ds.documentId,'/',ds.documentName,ds.documentExtension) AS previewProfileLogo,ds.documentId AS documentIdProfileLogo,ds.documentName AS documentNameProfileLogo,ds.documentExtension AS documentExtensionProfileLogo
				 	                                FROM documentstore AS ds 
                                                INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND dpt.documentType=" + (Int16)DocumentType.ProfileLogo + @" AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Hospitlal + @"
                                            ) AS dsdpt2 ON dsdpt2.documentIdProfileLogo=hr.hospitalRegNo
                                WHERE hr.hospitalRegNo=@hospitalRegNo ;";
                ds = await db.executeSelectQueryForDataset_async(qr, pm);
            }
            catch (Exception ex)
            {
            }
            return ds;
        }

        /// <summary>
        /// Returns 12 digit hospitalId id Outer Hospitals
        /// </summary>
        /// <returns></returns>
        public async Task<Int64> GetHospitalId()
        {
            string hospitalId = "0";string registrationYear = "0000";
            string prefix= "2";
            try
            {
                string qr = @"SELECT IFNULL(MAX(SUBSTRING(ur.hospitalRegNo,6,12)),0) + 1 AS hospitalId
								FROM hospitalregistration ur 
							WHERE ur.registrationYear = @registrationYear";

                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("registrationYear", MySqlDbType.Int32) { Value = registrationYear });
                ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(qr, pm.ToArray());
                if (dt.table.Rows.Count > 0)
                {
                    hospitalId = dt.table.Rows[0]["hospitalId"].ToString();
                    hospitalId = prefix + registrationYear.ToString() + hospitalId.PadLeft(7, '0');
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return Convert.ToInt64(hospitalId);
        }
    }
}
