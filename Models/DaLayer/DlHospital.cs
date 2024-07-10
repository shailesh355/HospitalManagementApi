using BaseClass;
using HospitalManagementApi.Models.BLayer;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using static BaseClass.ReturnClass;
using static HospitalManagementApi.Models.BLayer.BlCommon;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlHospital
    {
        readonly DBConnection db = new();
        ReturnClass.ReturnDataTable dt = new();
        ReturnClass.ReturnDataSet ds = new();
        DlCommon dlcommon = new();
        DlMasters dlmaster = new();
        public async Task<ReturnClass.ReturnBool> RegisterNewHospital(BlHospital blHospital)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            if (blHospital.hospitalRegNo == null)
                blHospital.hospitalRegNo = 0;
            bool isExists = await CheckMobileExistAsync(blHospital.mobileNo!, "INSERT", (Int64)blHospital.hospitalRegNo);

            bool isExistsMail = await dlcommon.CheckMailExistOnUserAsync(blHospital.emailId!);
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

            string query = "";
            if (!isExists)
            {
                isExists = await CheckEmailExistAsync(blHospital.emailId!, "INSERT", (Int64)blHospital.hospitalRegNo);
                if (!isExists)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        BlCity blcity = new();
                        blcity.districtId = (Int16)blHospital.districtId!;
                        blcity.stateId = (Int16)blHospital.stateId!;
                        blcity.cityId = (Int32)blHospital.cityId!;
                        blcity.cityNameEnglish = blHospital.cityName!;
                        blcity.clientIp = blHospital.clientIp;
                        blcity.userId = blHospital.userId;
                        blcity.entryDateTime = blHospital.entryDateTime;
                        blHospital.cityId = await dlcommon.ReturnCity(blcity);

                        //string qr = @"SELECT c.cityId,c.cityNameEnglish 
                        //              FROM city AS c
                        //        WHERE c.cityNameEnglish=@cityNameEnglish AND stateId=@stateId";
                        //MySqlParameter[] pmc = new MySqlParameter[]
                        //  {
                        //     new MySqlParameter("cityNameEnglish", MySqlDbType.VarChar,99) { Value = blHospital.cityName },
                        //     new MySqlParameter("stateId", MySqlDbType.Int16) { Value = blHospital.stateId },
                        //  };
                        //dt = await db.ExecuteSelectQueryAsync(qr, pmc);
                        //if (dt.table.Rows.Count > 0)
                        //{
                        //    blHospital.cityId = Convert.ToInt32(dt.table.Rows[0]["cityId"].ToString());
                        //}
                        //else
                        //{
                        //    blHospital.cityId = await dlmaster.GetCityId();

                        //    List<MySqlParameter> pm1 = new();
                        //    pm1.Add(new MySqlParameter("districtId", MySqlDbType.Int16) { Value = blHospital.districtId });
                        //    pm1.Add(new MySqlParameter("stateId", MySqlDbType.Int16) { Value = blHospital.stateId });
                        //    pm1.Add(new MySqlParameter("cityId", MySqlDbType.Int32) { Value = blHospital.cityId });
                        //    pm1.Add(new MySqlParameter("cityName", MySqlDbType.String) { Value = blHospital.cityName });
                        //    pm1.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blHospital.clientIp });
                        //    pm1.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blHospital.userId });
                        //    pm1.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = blHospital.entryDateTime });

                        //    query = @"INSERT INTO city (stateId,districtId,cityId,cityNameEnglish,clientIp,entryDateTime,userId)
                        //                VALUES (@stateId,@districtId,@cityId,@cityName,@clientIp,@entryDateTime,@userId)";
                        //    rb = await db.ExecuteQueryAsync(query, pm1.ToArray(), "city");
                        //}

                        blHospital.hospitalRegNo = await GetHospitalId((int)blHospital.registrationYear!);
                        List<MySqlParameter> pm = new();
                        pm.Add(new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = blHospital.hospitalRegNo });
                        pm.Add(new MySqlParameter("hospitalNameEnglish", MySqlDbType.String) { Value = blHospital.hospitalNameEnglish });
                        pm.Add(new MySqlParameter("hospitalNameLocal", MySqlDbType.String) { Value = blHospital.hospitalNameLocal });
                        pm.Add(new MySqlParameter("stateId", MySqlDbType.Int16) { Value = blHospital.stateId });
                        pm.Add(new MySqlParameter("districtId", MySqlDbType.Int16) { Value = blHospital.districtId });
                        pm.Add(new MySqlParameter("cityId", MySqlDbType.Int32) { Value = blHospital.cityId });
                        pm.Add(new MySqlParameter("cityName", MySqlDbType.String) { Value = blHospital.cityName });
                        pm.Add(new MySqlParameter("address", MySqlDbType.String) { Value = blHospital.address });
                        pm.Add(new MySqlParameter("mobileNo", MySqlDbType.String) { Value = blHospital.mobileNo });
                        pm.Add(new MySqlParameter("emailId", MySqlDbType.String) { Value = blHospital.emailId });
                        pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = blHospital.active == null ? 0 : (Int16)blHospital.active });
                        pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = blHospital.isVerified == null ? 0 : (Int16)blHospital.isVerified });
                        pm.Add(new MySqlParameter("verifiedByLoginId", MySqlDbType.Int64) { Value = blHospital.userId });
                        pm.Add(new MySqlParameter("registrationStatus", MySqlDbType.Int16) { Value = blHospital.isVerified == null ? 0 : (Int16)blHospital.isVerified });
                        pm.Add(new MySqlParameter("registrationYear", MySqlDbType.Int32) { Value = blHospital.registrationYear });
                        pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blHospital.clientIp });
                        pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blHospital.userId });
                        pm.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = blHospital.entryDateTime });
                        query = @"INSERT INTO hospitalregistration (hospitalRegNo,hospitalNameEnglish,hospitalNameLocal,stateId,districtId,address,mobileNo,
                                                 emailId,active,isVerified,verificationDate,verifiedByLoginId,registrationStatus,userId, 
                                                entryDateTime, clientIp,registrationYear,cityId,cityName)
                                        VALUES (@hospitalRegNo,@hospitalNameEnglish,@hospitalNameLocal,@stateId, @districtId, @address, @mobileNo,
                                                 @emailId,@active, @isVerified,@verificationDate,@verifiedByLoginId,@registrationStatus,@userId, 
                                                @entryDateTime,@clientIp,@registrationYear,@cityId,@cityName)";
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
            string query = string.Empty;
            string hospitalId = "0";
            try
            {
                query = @"SELECT IFNULL(MAX(SUBSTRING(ur.hospitalRegNo,6,12)),0) + 1 AS hospitalId
								FROM hospitalregistration ur 
							WHERE ur.registrationYear = @registrationYear";

                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("registrationYear", MySqlDbType.Int32) { Value = registrationYear });

                ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
                if (dt.table.Rows.Count > 0)
                {
                    hospitalId = dt.table.Rows[0]["hospitalId"].ToString()!;
                    hospitalId = ((int)idPrefix.officeId).ToString() + registrationYear.ToString() + hospitalId.PadLeft(7, '0');
                }
            }
            catch (Exception ex)
            {
                WriteLog.Error(" Query: " + query + "\n   error - ", ex);
                dt.status = false;
                dt.message = ex.Message;
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
            if (isAccountExists == false)
            {
                query = @"SELECT u.emailId
                            FROM userlogin u
                            WHERE u.emailId = @emailId ";
                dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
                if (dt.table.Rows.Count > 0)
                    isAccountExists = true;
            }
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
        public async Task<ReturnClass.ReturnDataTable> GetAllHospitalList(Int16 registrationStatus)
        {
            string query = @"SELECT  h.hospitalRegNo,h.hospitalNameEnglish,h.hospitalNameLocal,h.stateId,h.cityId,h.address,h.mobileNo,
                                     h.emailId,h.active,s.stateNameEnglish AS stateName,c.cityNameEnglish AS cityName,d.districtNameEnglish
                                FROM hospitalregistration h
                                 JOIN state s ON s.stateId=h.stateId
				                 JOIN city c ON c.cityId=h.cityId
                                 LEFT JOIN district d ON d.districtId=h.districtId
                            WHERE h.registrationStatus=@registrationStatus";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("registrationStatus", MySqlDbType.Int16) { Value = registrationStatus });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }
        public async Task<ReturnClass.ReturnDataTable> GetHospitalById(Int64 hospitalRegNo)
        {
            string query = @"SELECT  h.hospitalRegNo,h.hospitalNameEnglish,h.hospitalNameLocal,h.stateId,h.districtId,h.address,h.mobileNo,
                                     h.emailId,h.active,s.stateNameEnglish AS stateName,d.districtNameEnglish AS districtName,c.cityId,c.cityNameEnglish AS cityName
                                FROM hospitalregistration h
                                 JOIN state s ON s.stateId=h.stateId
                                 JOIN city c ON c.cityId=h.cityId
                                 LEFT JOIN district d ON d.districtId=h.districtId
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
            string pass = "", hash_Pass = "";

            if (verificationDetail.VerificationHospital.Count != 0)
            {
                foreach (var item in verificationDetail.VerificationHospital)
                {
                    item.isVerified = YesNo.Yes;
                    bool isofficeExists = await CheckVerifyHospital(item.hospitalRegNo, (Int16)item.isVerified);
                    if (!isofficeExists)
                    {
                        if (item.registrationStatus == RegistrationStatus.Approved)
                        {
                            pass = "HRP" + getrandom();
                            hash_Pass = sha256_hash(pass);
                        }

                        string query = @"UPDATE hospitalregistration 
                             SET isVerified=@isVerified,clientIp=@clientIp,verificationDate=NOW(),
                                verifiedByLoginId=@verifiedByLoginId,registrationStatus=@registrationStatus,active=@active 
                              WHERE hospitalRegNo=@hospitalRegNo";

                        List<MySqlParameter> pm = new();
                        pm.Add(new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = item.hospitalRegNo });
                        pm.Add(new MySqlParameter("registrationStatus", MySqlDbType.Int16) { Value = (int)item.registrationStatus });
                        pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = (Int16)item.isVerified });
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
                        rb.message = "This Hospital is Already Verified..";
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

                rb.message = "Hospital Data Empty..";
                rb.error = string.Empty;
            }
            return rb;
        }

        public async Task<bool> CheckVerifyHospital(Int64 hospitalRegNo, Int16 isVerified)
        {
            bool isHospitalExists = false;
            string query = @"SELECT h.hospitalRegNo
                            FROM hospitalregistration h
                            WHERE h.hospitalRegNo = @hospitalRegNo AND h.isVerified=@isVerified AND h.registrationStatus=@registrationStatus";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo });
            pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = isVerified });
            pm.Add(new MySqlParameter("registrationStatus", MySqlDbType.Int16) { Value = isVerified });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
            {
                isHospitalExists = true;
            }
            return isHospitalExists;
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

        public async Task<ReturnClass.ReturnDataSet> SearchHS(Filter filter)
        {
            string query = string.Empty;
            ReturnClass.ReturnDataSet dataSet = new();
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                    new MySqlParameter("hospitalSpec", MySqlDbType.VarChar,500) { Value = "%" +filter.hospitalSpec +"%" },
                    new MySqlParameter("districtId", MySqlDbType.Int16) { Value = filter.districtId },
                   };
                query = @" SELECT hr.hospitalRegNo,hr.hospitalNameEnglish,hr.districtId,hr.address,hr.mobileNo,hr.emailId,hr.registrationYear
		                                ,hr.cityId,hr.pinCode,hr.phoneNumber,hr.landMark,hr.fax,hr.isCovid,hr.latitude,hr.longitude,hr.typeOfProviderId,hr.website,hr.natureOfEntityId
		                                ,hs.specializationTypeName,hs.specializationName,hs.levelOfCareName , GROUP_CONCAT(hs.specializationName SEPARATOR ', ') AS specializationName
	                                FROM hospitalregistration AS hr 
		                            LEFT JOIN hospitalspecialization AS hs ON hr.hospitalRegNo = hs.hospitalRegNo
		                            WHERE ( hr.hospitalNameEnglish LIKE @hospitalSpec OR hs.specializationName LIKE @hospitalSpec ) AND hr.districtId=@districtId 
                                          AND hr.active=" + (Int16)YesNo.Yes + @" AND hr.isVerified = " + (Int16)YesNo.Yes + " GROUP BY hr.hospitalRegNo ;";
                ReturnClass.ReturnDataTable dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "HospitalSearch";
                dataSet.dataset.Tables.Add(dtt.table);
                query = " SELECT dr.doctorRegNo,dr.doctorNameEnglish,dr.doctorNameLocal,dr.stateId,dr.districtId,dr.address AS DoctorAddress,dr.mobileNo," +
                    " dr.emailId,dr.active,s.stateNameEnglish AS stateName,d.districtNameEnglish AS DoctorDistrictName,dr.cityId,dr.cityName AS DoctorCityname," +
                    " dwa.price,dwa.consultancyTypeName,dwa.address1 as workAreaAddress,dwa.phoneNumber AS workAreaPhoneNumber ,GROUP_CONCAT(dws.specializationName SEPARATOR ', ') AS specializationName " +
                     " FROM doctorregistration AS dr JOIN state AS s ON s.stateId=dr.stateId " +
                     "  INNER JOIN district AS d ON d.districtId=dr.districtId " +
                     " LEFT JOIN doctorworkarea AS dwa ON dwa.doctorRegNo = dr.doctorRegNo " +
                     "  LEFT JOIN doctorspecialization AS dws ON dws.doctorRegNo = dr.doctorRegNo " +
                     "  WHERE dr.registrationStatus=" + (Int16)YesNo.Yes + " AND ( dr.doctorNameEnglish LIKE @hospitalSpec OR dws.specializationName LIKE @hospitalSpec ) AND dwa.districtId=@districtId " +
                        " GROUP BY dr.doctorRegNo ;";
                  dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "DoctorSearch";
                dataSet.dataset.Tables.Add(dtt.table);
            }
            catch (Exception ex)
            {
                WriteLog.Error(" Query: " + query + "\n   error - ", ex);
                dt.status = false;
                dt.message = ex.Message;
            }
            return dataSet;
        }

        public async Task<ReturnClass.ReturnDataTable> GetHospitalDoc(Int64 hospitalRegNo)
        {
            string query = string.Empty;
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                     new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                   };
                query = @" SELECT CONCAT(REPLACE(dp.physicalPath, 'D:', ''),ds.documentId,'/',ds.documentName,ds.documentExtension) AS preview,
                                        dp.documentType,ds.documentName,ds.documentExtension,dp.documentId,dp.documentImageGroup,dp.dptTableId 
                                   FROM documentstore AS ds
                               INNER JOIN documentpathtbl AS dp ON dp.dptTableId = ds.dptTableId 
		                            AND dp.documentType IN ('" + (Int16)DocumentType.ProfilePic + @"','" + (Int16)DocumentType.ProfileLogo + @"','" +
                                    (Int16)DocumentType.ProfileDocument + @"','" + (Int16)DocumentType.NACH + @"','" + (Int16)DocumentType.License +
                                    @"','" + (Int16)DocumentType.OtherDocument + @"') 
                                    AND dp.documentImageGroup=@" + (Int16)DocumentImageGroup.Hospital + @"'
	                               AND ds.documentId=@hospitalRegNo ORDER BY dp.documentImageGroup";
                dt = await db.ExecuteSelectQueryAsync(query, pm);
            }
            catch (Exception ex)
            {
                WriteLog.Error(" Query: " + query + "\n   error - ", ex);
                dt.status = false;
                dt.message = ex.Message;
            }
            return dt;
        }

        public async Task<ReturnClass.ReturnBool> UpdateHospitalInfoMI(BlHospitalMI bl)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            MySqlParameter[] pm; string query = "";
            try
            {
                if (!string.IsNullOrEmpty(bl.CRUD.ToString()))
                {
                    if (bl.CRUD == (Int16)CRUD.Update)
                        if (bl.hospitalRegNo == 0)
                        {
                            rb.message = " Invalid Hospital Registration Id";
                            rb.status = false;
                            return rb;
                        }
                    using (TransactionScope ts = new TransactionScope())
                    {

                        pm = new MySqlParameter[]
                        {
                            new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo },
                         };
                        query = @"DELETE FROM maincontact 
                                    WHERE hospitalRegNo = @hospitalRegNo";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "maincontact");

                        //query = @"DELETE FROM hospitaldocuments 
                        //            WHERE hospitalRegNo = @hospitalRegNo";
                        //rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "hospitaldocuments");
                        DateTime licenseExpiryDate = DateTime.ParseExact(bl.licenseExpiryDate.Replace('-', '/'), "dd/MM/yyyy", null);
                        bl.licenseExpiryDate = licenseExpiryDate.ToString("yyyy/MM/dd");
                        foreach (var item in bl.BLMC!)
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
                                    licenseExpiryDate=@licenseExpiryDate,nabhCertificationLevelId=@nabhCertificationLevel,registeredWith=@registeredWith,
                                    anyOtherCertification=@anyOtherCertification
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
                        pm2.Add(new MySqlParameter("licenseExpiryDate", MySqlDbType.VarChar, 20) { Value = bl.licenseExpiryDate });
                        pm2.Add(new MySqlParameter("nabhCertificationLevel", MySqlDbType.VarChar, 50) { Value = bl.nabhCertificationLevel });
                        pm2.Add(new MySqlParameter("registeredWith", MySqlDbType.VarChar, 200) { Value = bl.registeredWith });
                        pm2.Add(new MySqlParameter("anyOtherCertification", MySqlDbType.VarChar, 200) { Value = bl.anyOtherCertification });

                        rb = await db.ExecuteQueryAsync(query, pm2.ToArray(), "hospitalregistration");
                        //if (rb.status)
                        //{
                        //    DateTime licenseExpiryDate = DateTime.ParseExact(bl.licenseExpiryDate.Replace('-', '/'), "dd/MM/yyyy", null);
                        //    bl.licenseExpiryDate = licenseExpiryDate.ToString("yyyy/MM/dd");

                        //    List<MySqlParameter> pm3 = new();
                        //    pm3.Add(new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo });
                        //    //pm3.Add(new MySqlParameter("hospitalRegistrationNo", MySqlDbType.VarChar, 20) { Value = bl.hospitalRegistrationNo });
                        //    pm3.Add(new MySqlParameter("licenseExpiryDate", MySqlDbType.VarChar, 20) { Value = bl.licenseExpiryDate });
                        //    pm3.Add(new MySqlParameter("isNABH", MySqlDbType.Int16) { Value = bl.isNABH });
                        //    pm3.Add(new MySqlParameter("isNABL", MySqlDbType.Int16) { Value = bl.isNABL });
                        //    pm3.Add(new MySqlParameter("isISO", MySqlDbType.Int16) { Value = bl.isISO });
                        //    pm3.Add(new MySqlParameter("registeredWith", MySqlDbType.VarChar, 100) { Value = bl.registeredWith });
                        //    pm3.Add(new MySqlParameter("anyOtherCertification", MySqlDbType.VarChar, 100) { Value = bl.anyOtherCertification });
                        //    pm3.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId });
                        //    pm3.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime });
                        //    query = @"INSERT INTO hospitaldocuments (hospitalRegNo,hospitalRegistrationNo,licenseExpiryDate,isNABH,isNABL,
                        //                isISO,registeredWith,anyOtherCertification,entryDateTime,userId)
                        //        VALUES (@hospitalRegNo,@hospitalRegistrationNo,@licenseExpiryDate,@isNABH,@isNABL,
                        //                @isISO,@registeredWith,@anyOtherCertification,@entryDateTime,@userId)";
                        //    rb = await db.ExecuteQueryAsync(query, pm3.ToArray(), "hospitaldocuments");
                        //}
                        if (rb.status)
                            ts.Complete();
                    }
                }
                else
                {
                    rb.message = "something went wrong !";
                    rb.status = false;
                }
            }
            catch (Exception ex)
            {
                WriteLog.Error(" Query: " + query + "\n   error - ", ex);
                dt.status = false;
                rb.message = "Could not save , " + ex.Message;

            }
            return rb;
        }

        public async Task<ReturnClass.ReturnDataSet> GetHospitalInfoMI(Int64 hospitalRegNo)
        {
            string query = string.Empty;
            ReturnClass.ReturnDataSet dataSet = new();
            ReturnClass.ReturnDataTable dtt = new();
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                     new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                   };

                // maincontact
                query = @"SELECT designationId,designationName,contactPersonName,mobileNo,emailId 
			                FROM maincontact WHERE hospitalRegNo=@hospitalRegNo ";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "maincontact";
                dataSet.dataset.Tables.Add(dtt.table);

                // hospitalregistration
                query = @"SELECT hospitalNameEnglish,st.stateId,st.stateNameEnglish,dt.districtId,dt.districtNameEnglish,address,mobileNo,emailId,ct.cityId,ct.cityNameEnglish AS cityName,pinCode,
                                phoneNumber,landMark,fax,isCovid,
			                    case when isCovid = 1 then 'Yes' when isCovid = 0 then 'No' ELSE '' END AS isCovidYesNo ,latitude,longitude,typeOfProviderId,typeOfProviderName,website,
                                natureOfEntityId,natureOfEntityName,rohiniId,DATE_FORMAT(licenseExpiryDate,'%d/%m/%Y')AS licenseExpiryDate,
                                nabhCertificationLevelId AS nabhCertificationLevel,registeredWith,anyOtherCertification,hospitalTypeId,hospitalTypeName
                            FROM hospitalregistration AS hr
				                INNER JOIN state AS st ON hr.stateId=st.stateId
				                INNER JOIN city AS ct ON hr.cityId=ct.cityId
                                LEFT JOIN district dt ON dt.districtId=hr.districtId
			                WHERE hospitalRegNo=@hospitalRegNo";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "hospitalregistration";
                dataSet.dataset.Tables.Add(dtt.table);

                //string qr = @" SELECT designationId,designationName,contactPersonName,mobileNo,emailId 
                //               FROM maincontact WHERE hospitalRegNo=@hospitalRegNo;
                //                SELECT hospitalNameEnglish,st.stateId,st.stateNameEnglish,dt.districtId,dt.districtNameEnglish,address,mobileNo,emailId,ct.cityId,ct.cityNameEnglish AS cityName,pinCode,
                //                      phoneNumber,landMark,fax,isCovid,
                //             case when isCovid = 1 then 'Yes' ELSE 'No' END AS isCovidYesNo ,latitude,longitude,typeOfProviderId,typeOfProviderName,website,
                //                      natureOfEntityId,natureOfEntityName,rohiniId,DATE_FORMAT(licenseExpiryDate,'%d/%m/%Y')AS licenseExpiryDate,
                //                      nabhCertificationLevelId AS nabhCertificationLevel,registeredWith,anyOtherCertification,hospitalTypeId,hospitalTypeName
                //                    FROM hospitalregistration AS hr
                //            INNER JOIN state AS st ON hr.stateId=st.stateId
                //            INNER JOIN city AS ct ON hr.cityId=ct.cityId
                //                        LEFT JOIN district dt ON dt.districtId=hr.districtId
                //           WHERE hospitalRegNo=@hospitalRegNo;
                //                SELECT DATE_FORMAT(licenseExpiryDate,'%d/%m/%Y')AS licenseExpiryDate ,isNABH,isNABL,isISO,registeredWith,anyOtherCertification
                //                        ,CASE WHEN isNABH= 1 THEN 'Yes' ELSE 'No' END isNABHYesNo
                //                        ,CASE WHEN isNABL= 1 THEN 'Yes' ELSE 'No' END isNABLYesNo
                //                        ,CASE WHEN isISO= 1 THEN 'Yes' ELSE 'No' END isISOYesNo
                //              FROM hospitaldocuments 
                //              WHERE hospitalRegNo=@hospitalRegNo;";
            }
            catch (Exception ex)
            {
                WriteLog.Error(" Query: " + query + "\n   error - ", ex);
                dt.status = false;
                dt.message = ex.Message;
            }
            return dataSet;
        }

        public async Task<ReturnClass.ReturnDataTable> GetHospitalFinancialInfo(Int64 hospitalRegNo)
        {
            string query = string.Empty;
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                     new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                   };
                query = @"SELECT accountNumber,beneficiaryName,accountTypeId,accountTypeName,
		                            bankName,bankAddress,IFSCCode,PANNo,nameOnPAN
		                        FROM financialinformation 
		                    WHERE hospitalRegNo=@hospitalRegNo;";
                dt = await db.ExecuteSelectQueryAsync(query, pm);
            }
            catch (Exception ex)
            {
                WriteLog.Error(" Query: " + query + "\n   error - ", ex);
                dt.status = false;
                dt.message = ex.Message;
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
                        url = "WorkDocs/uploaddocsmi";
                        Int16 i = 0;
                        foreach (var item in bl.BlDocument!)
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
                        foreach (var item in bl.BlDocument!)
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
                WriteLog.Error(" Error - ", ex);
                succeded.message = "Could not upload image , " + ex.Message;
                succeded.status = false;
            }
            return succeded;
        }

        public async Task<ReturnClass.ReturnDataSet> GetHospitalInfoMIDoc(Int64 hospitalRegNo)
        {
            string query = string.Empty;
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                     new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                   };
                query = @" SELECT hr.hospitalRegNo,dsdpt1.documentIdProfilePic,dsdpt1.documentNameProfilePic,dsdpt1.documentExtensionProfilePic
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
                                                INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND ds.active=1 AND dpt.documentType=" + (Int16)DocumentType.ProfilePic + @" AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Hospital + @"
                                            ) AS dsdpt1 ON dsdpt1.documentIdProfilePic=hr.hospitalRegNo
                                            LEFT JOIN (
   			                                    SELECT CONCAT(REPLACE(dpt.physicalPath, 'D:', ''),ds.documentId,'/',ds.documentName,ds.documentExtension) AS previewProfileLogo,ds.documentId AS documentIdProfileLogo,ds.documentName AS documentNameProfileLogo,ds.documentExtension AS documentExtensionProfileLogo
				 	                                FROM documentstore AS ds 
                                                INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND ds.active=1 AND dpt.documentType=" + (Int16)DocumentType.ProfileLogo + @" AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Hospital + @"
                                            ) AS dsdpt2 ON dsdpt2.documentIdProfileLogo=hr.hospitalRegNo
                                            LEFT JOIN (
   			                                    SELECT CONCAT(REPLACE(dpt.physicalPath, 'D:', ''),ds.documentId,'/',ds.documentName,ds.documentExtension) AS previewProfileDoc,ds.documentId AS documentIdProfileDoc,ds.documentName AS documentNameProfileDoc,ds.documentExtension AS documentExtensionProfileDoc
				 	                                FROM documentstore AS ds 
                                                INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND ds.active=1 AND dpt.documentType=" + (Int16)DocumentType.ProfileDocument + @" AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Hospital + @"
                                            ) AS dsdpt3 ON dsdpt3.documentIdProfileDoc=hr.hospitalRegNo
                                            LEFT JOIN (
   			                                    SELECT CONCAT(REPLACE(dpt.physicalPath, 'D:', ''),ds.documentId,'/',ds.documentName,ds.documentExtension) AS previewNACH,ds.documentId AS documentIdNACH,ds.documentName AS documentNameNACH,ds.documentExtension AS documentExtensionNACH
				 	                                FROM documentstore AS ds 
                                                INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND ds.active=1  AND dpt.documentType=" + (Int16)DocumentType.NACH + @"  AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Hospital + @"
                                            ) AS dsdpt4 ON dsdpt4.documentIdNACH=hr.hospitalRegNo
                                            LEFT JOIN (
   			                                    SELECT CONCAT(REPLACE(dpt.physicalPath, 'D:', ''),ds.documentId,'/',ds.documentName,ds.documentExtension) AS previewLicense,ds.documentId AS documentIdLicense,ds.documentName AS documentNameLicense,ds.documentExtension AS documentExtensionLicense
				 	                                FROM documentstore AS ds 
                                                INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND ds.active=1  AND dpt.documentType=" + (Int16)DocumentType.License + @"  AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Hospital + @"
                                            ) AS dsdpt5 ON dsdpt5.documentIdLicense=hr.hospitalRegNo
                                            LEFT JOIN (
   			                                    SELECT CONCAT(REPLACE(dpt.physicalPath, 'D:', ''),ds.documentId,'/',ds.documentName,ds.documentExtension) AS previewOther,ds.documentId AS documentIdOther,ds.documentName AS documentNameOther,ds.documentExtension AS documentExtensionOther
				 	                                FROM documentstore AS ds 
                                                INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND ds.active=1  AND dpt.documentType=" + (Int16)DocumentType.OtherDocument + @"  AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Hospital + @"
                                            ) AS dsdpt6 ON dsdpt6.documentIdOther=hr.hospitalRegNo
                                              LEFT JOIN (
   			                                    SELECT CONCAT(REPLACE(dpt.physicalPath, 'D:', ''),ds.documentId,'/',ds.documentName,ds.documentExtension) AS previewPAN,ds.documentId AS documentIdPAN,ds.documentName AS documentNamePAN,ds.documentExtension AS documentExtensionPAN
				 	                                FROM documentstore AS ds 
                                                INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND ds.active=1  AND dpt.documentType=" + (Int16)DocumentType.HospitalPAN + @"  AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Hospital + @"
                                            ) AS dsdpt7 ON dsdpt7.documentIdPAN=hr.hospitalRegNo 
                                WHERE hr.hospitalRegNo=@hospitalRegNo ;";
                ds = await db.executeSelectQueryForDataset_async(query, pm);
            }
            catch (Exception ex)
            {
                WriteLog.Error(" Query: " + query + "\n   error - ", ex);
                dt.status = false;
                dt.message = ex.Message;
            }
            return ds;
        }

        public async Task<ReturnClass.ReturnDataSet> GetHospitalProfileLogo(Int64 hospitalRegNo)
        {
            string query = string.Empty;
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                     new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                   };
                query = @" SELECT hr.hospitalRegNo,dsdpt1.documentIdProfilePic,dsdpt1.documentNameProfilePic,dsdpt1.documentExtensionProfilePic
								        ,dsdpt2.previewProfileLogo,dsdpt2.documentIdProfileLogo,dsdpt2.documentNameProfileLogo,dsdpt2.documentExtensionProfileLogo
		                                    FROM hospitalregistration AS hr
			                                LEFT JOIN (
   			                                    SELECT CONCAT(REPLACE(dpt.physicalPath, 'D:', ''),ds.documentId,'/',ds.documentName,ds.documentExtension) AS previewProfilePic,ds.documentId AS documentIdProfilePic,ds.documentName AS documentNameProfilePic,ds.documentExtension AS documentExtensionProfilePic
				 	                                FROM documentstore AS ds 
                                                INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND dpt.documentType=" + (Int16)DocumentType.ProfilePic + @" AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Hospital + @"
                                            ) AS dsdpt1 ON dsdpt1.documentIdProfilePic=hr.hospitalRegNo
                                            LEFT JOIN (
   			                                    SELECT CONCAT(REPLACE(dpt.physicalPath, 'D:', ''),ds.documentId,'/',ds.documentName,ds.documentExtension) AS previewProfileLogo,ds.documentId AS documentIdProfileLogo,ds.documentName AS documentNameProfileLogo,ds.documentExtension AS documentExtensionProfileLogo
				 	                                FROM documentstore AS ds 
                                                INNER JOIN documentpathtbl AS dpt ON dpt.dptTableId = ds.dptTableId AND dpt.documentType=" + (Int16)DocumentType.ProfileLogo + @" AND dpt.documentImageGroup=" + (Int16)DocumentImageGroup.Hospital + @"
                                            ) AS dsdpt2 ON dsdpt2.documentIdProfileLogo=hr.hospitalRegNo
                                WHERE hr.hospitalRegNo=@hospitalRegNo ;";
                ds = await db.executeSelectQueryForDataset_async(query, pm);
            }
            catch (Exception ex)
            {
                WriteLog.Error(" Query: " + query + "\n   error - ", ex);
                dt.status = false;
                dt.message = ex.Message;
            }
            return ds;
        }
        /// <summary>
        /// Returns 12 digit hospitalId id Outer Hospitals
        /// </summary>
        /// <returns></returns>
        public async Task<Int64> GetHospitalId()
        {
            string query = string.Empty;
            string hospitalId = "0"; string registrationYear = "0000";
            string prefix = "2";
            try
            {
                query = @"SELECT IFNULL(MAX(SUBSTRING(ur.hospitalRegNo,6,12)),0) + 1 AS hospitalId
								FROM hospitalregistration ur 
							WHERE ur.registrationYear = @registrationYear";

                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("registrationYear", MySqlDbType.Int32) { Value = registrationYear });
                ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
                if (dt.table.Rows.Count > 0)
                {
                    hospitalId = dt.table.Rows[0]["hospitalId"].ToString()!;
                    hospitalId = prefix + registrationYear + hospitalId.PadLeft(7, '0');
                }
            }
            catch (Exception ex)
            {
                WriteLog.Error(" Query: " + query + "\n   error - ", ex);
                dt.status = false;
                dt.message = ex.Message;
            }
            return Convert.ToInt64(hospitalId);
        }


        public async Task<ReturnClass.ReturnDataSet> GetHospitalPreview(Int64 hospitalRegNo)
        {
            string query = string.Empty;
            ReturnClass.ReturnDataSet dataSet = new();
            ReturnClass.ReturnDataTable dtt = new();
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                     new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                   };

                // maincontact
                query = @"SELECT designationId,designationName,contactPersonName,mobileNo,emailId 
			                FROM maincontact WHERE hospitalRegNo=@hospitalRegNo ";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "maincontact";
                dataSet.dataset.Tables.Add(dtt.table);

                // hospitalregistration
                query = @"SELECT hospitalNameEnglish,st.stateId,st.stateNameEnglish,dt.districtId,dt.districtNameEnglish,address,mobileNo,emailId,ct.cityId,ct.cityNameEnglish AS cityName,pinCode,
                                phoneNumber,landMark,fax,isCovid,
			                    case when isCovid = 1 then 'Yes' ELSE 'No' END AS isCovidYesNo ,latitude,longitude,typeOfProviderId,typeOfProviderName,website,
                                natureOfEntityId,natureOfEntityName,rohiniId,DATE_FORMAT(licenseExpiryDate,'%d/%m/%Y')AS licenseExpiryDate,
                                nabhCertificationLevelId AS nabhCertificationLevel,registeredWith,anyOtherCertification,hospitalTypeId,hospitalTypeName
                            FROM hospitalregistration AS hr
				                INNER JOIN state AS st ON hr.stateId=st.stateId
				                INNER JOIN city AS ct ON hr.cityId=ct.cityId
                                LEFT JOIN district dt ON dt.districtId=hr.districtId
			                WHERE hospitalRegNo=@hospitalRegNo";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "hospitalregistration";
                dataSet.dataset.Tables.Add(dtt.table);

                // financial Information
                query = @"SELECT fi.financialInformationId,fi.hospitalRegNo,fi.accountNumber,fi.beneficiaryName,fi.accountTypeId,fi.accountTypeName,
                               fi.bankName,fi.bankAddress,fi.IFSCCode,fi.PANNo,fi.nameOnPAN,fi.TDSExemptionPercent,fi.TDSExemptionLimit,fi.TDSExemptionPeriod	
                            FROM financialinformation AS fi
			                WHERE hospitalRegNo=@hospitalRegNo";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "financialinformation";
                dataSet.dataset.Tables.Add(dtt.table);

                // Bed Composition
                query = @"SELECT bc.bedCompositionId,bc.hospitalRegNo,bc.noOfBeds,bc.rentPerDay,cat.nameEnglish 
                            FROM bedcomposition AS bc
                                INNER JOIN ddlcatlist AS cat ON bc.bedCompositionId = cat.id
                                AND cat.category='bedComposition'
			                WHERE hospitalRegNo=@hospitalRegNo ORDER BY cat.sortOrder";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "bedcomposition";
                dataSet.dataset.Tables.Add(dtt.table);

                // infrastructure
                query = @"SELECT infra.infrastructureId,infra.hospitalRegNo,infra.medicalInfrastructureId,infra.medicalInfrastructure,infra.infrastructureFacilitiesId,
		                        infra.infrastructureFacilities,infra.remarks,cat.nameEnglish,cat.grouping,cat.category 
		                        FROM infrastructure AS infra
	                        INNER JOIN ddlcatlist AS cat ON infra.medicalInfrastructureId = cat.id
	                        AND cat.category IN ('medicalInfrastructure')
			                WHERE hospitalRegNo=@hospitalRegNo ORDER BY cat.grouping,cat.sortOrder";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "infrastructure";
                dataSet.dataset.Tables.Add(dtt.table);

                // cashlessbenefits
                query = @"SELECT clb.cashlessBenefitsId, clb.hospitalRegNo, clb.discountPercent, cat.nameEnglish, cat.grouping, cat.category,
		                     clb.isWaiver, CASE WHEN clb.isWaiver = 1 AND cat.category='waiverOffered' THEN 'Yes' ELSE 'NA' END AS isWaiverYesNo
                          FROM cashlessbenefits AS clb
                              INNER JOIN ddlcatlist AS cat ON clb.cashlessBenefitsFacilityId = cat.id
                              AND cat.category IN ('ipdServices','opdServices','waiverOffered')
			                WHERE hospitalRegNo=@hospitalRegNo ORDER BY cat.grouping,cat.sortOrder";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "cashlessbenefits";
                dataSet.dataset.Tables.Add(dtt.table);

                // hospitalspecialization
                query = @"SELECT hs.hospitalSpecializationId,hs.specializationTypeId,hs.specializationTypeName,
                                hs.specializationId,hs.specializationName,hs.levelOfCareId,hs.levelOfCareName
                            FROM hospitalspecialization AS hs
			                WHERE hospitalRegNo=@hospitalRegNo ORDER BY hs.specializationName";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "hospitalspecialization";
                dataSet.dataset.Tables.Add(dtt.table);

                // documentstore
                query = @"SELECT CONCAT(REPLACE(dp.physicalPath, 'D:', ''),ds.documentId,'/',ds.documentName,ds.documentExtension) AS preview,
                                  dp.documentType,ds.documentName,ds.documentExtension,ds.documentId,dp.documentImageGroup,dp.dptTableId 
                             FROM documentstore AS ds
 		                        INNER JOIN documentpathtbl AS dp ON dp.dptTableId = ds.dptTableId 
	                            AND dp.documentType IN ('1','2','3','4','5','6') 
	                              AND dp.documentImageGroup=1
			                WHERE hospitalRegNo=@hospitalRegNo ORDER BY dp.documentImageGroup ";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "documentstore";
                dataSet.dataset.Tables.Add(dtt.table);

                // empaneled
                query = @"SELECT em.rowId,em.empaneledId,em.hospitalRegNo,em.empaneledTypeId,em.empaneledTypeName,em.headName,
  		                        ep.providerName,DATE_FORMAT(ep.fromDate,'%d/%m/%Y') AS fromDate,DATE_FORMAT(ep.toDate,'%d/%m/%Y') AS toDate
                               FROM empaneled AS em 
                              INNER JOIN empaneledprovider AS ep ON em.empaneledId = ep.empaneledId AND em.empaneledTypeId=ep.empaneledTypeId
      		                        AND em.hospitalRegNo=ep.hospitalRegNo
                              INNER JOIN ddlcatlist AS cat ON cat.category='empaneled'
                               WHERE em.empaneledTypeId=cat.id AND em.hospitalRegNo=@hospitalRegNo
                            ORDER BY hs.specializationName";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "empaneled";
                dataSet.dataset.Tables.Add(dtt.table);
            }
            catch (Exception ex)
            {
                WriteLog.Error(" Query: " + query + "\n   error - ", ex);
                dt.status = false;
                dt.message = ex.Message;
            }
            return dataSet;
        }

        public async Task<ReturnClass.ReturnBool> RollbackHospitalRegistration(VerificationDetail verificationDetail)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            Int32 countData = 0;
            if (verificationDetail.VerificationHospital.Count != 0)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    foreach (var item in verificationDetail.VerificationHospital)
                    {
                        item.isVerified = YesNo.Yes;
                        bool isofficeExists = await CheckRollbackHospital(item.hospitalRegNo, (Int16)item.registrationStatus);
                        if (isofficeExists)
                        {
                            string query = @"UPDATE hospitalregistration 
                             SET isVerified=@isVerified,verificationDate=NOW(),registrationStatus=@registrationStatus 
                              WHERE hospitalRegNo=@hospitalRegNo";

                            List<MySqlParameter> pm = new();
                            pm.Add(new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = item.hospitalRegNo });
                            pm.Add(new MySqlParameter("registrationStatus", MySqlDbType.Int16) { Value = YesNo.No });
                            pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = YesNo.No });

                            rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "RollbackHospitalRegistration");
                            if (rb.status)
                            {
                                countData = countData + 1;
                            }
                        }
                    }
                    if (verificationDetail.VerificationHospital.Count == countData)
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
                rb.message = "Hospital Data Empty..";
                rb.error = string.Empty;
            }
            return rb;
        }
        public async Task<bool> CheckRollbackHospital(Int64 hospitalRegNo, Int16 registrationStatus)
        {
            bool isHospitalExists = false;
            string query = @"SELECT h.hospitalRegNo
                            FROM hospitalregistration h
                            WHERE h.hospitalRegNo = @hospitalRegNo AND h.registrationStatus=@registrationStatus";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo });
            pm.Add(new MySqlParameter("registrationStatus", MySqlDbType.Int16) { Value = registrationStatus });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
            {
                isHospitalExists = true;
            }
            return isHospitalExists;
        }

    
    }
}
