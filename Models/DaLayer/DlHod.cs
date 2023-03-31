using BaseClass;
using MySql.Data.MySqlClient;
using System.Transactions;
using HospitalManagementApi.Models.BLayer;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlHod
    {
        readonly DBConnection db = new();
        public async Task<ReturnClass.ReturnBool> RegistorNewHodOffice(BlHod blhod)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            bool isofficeExists = await CheckHodOffice(blhod);

            if (!isofficeExists)
            {
                isofficeExists = await CheckEmailExistAsync(blhod.applicantEmailId);
                if (!isofficeExists)
                {
                    string query = @"INSERT INTO hodofficeregistration (hodOfficeId, hodOfficeName,officeCount, baseDeptId, orgType, hodOfficeLevel, hodOfficeStateId, hodOfficeDistrictId, 
                                                hodOfficeDistrictname, hodOfficeIsUrbanRural, hodOfficePinCode, hodOfficeAddress, hodOfficeEmailId, hodOfficePhoneNumber, 
                                                hodOfficeFaxNumber, hodOfficeWebsite, isRegistrationDocumentUploaded, registrationStatus, clientIp, 
                                                applicantName, applicantDesignationCode, applicantMobileNumber, applicantEmailId, isParichayLogin, applicantPassword)
                                        VALUES (@hodOfficeId, @hodOfficeName,@officeCount, @baseDeptId, @orgType, @hodOfficeLevel, @hodOfficeStateId, @hodOfficeDistrictId, 
                                                @hodOfficeDistrictname, @hodOfficeIsUrbanRural, @hodOfficePinCode, @hodOfficeAddress, @hodOfficeEmailId, @hodOfficePhoneNumber, 
                                                @hodOfficeFaxNumber, @hodOfficeWebsite, @isRegistrationDocumentUploaded, @registrationStatus, @clientIp, 
                                                @applicantName, @applicantDesignationCode, @applicantMobileNumber, @applicantEmailId, @isParichayLogin, @applicantPassword)";
                    ReturnClass.ReturnDataTable dt = await GetHodOfficeRegistrationIdAsync(blhod.hodOfficeStateId);
                    if (dt.table.Rows.Count > 0)
                    {
                        blhod.hodOfficeId = Convert.ToInt64(dt.table.Rows[0]["hodOfficeId"].ToString());
                        blhod.officeCount = Convert.ToInt32(dt.table.Rows[0]["officeCount"].ToString());
                    }
                    blhod.isParichayLogin = (int)YesNo.No;
                    List<MySqlParameter> pm = new();
                    pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value = blhod.hodOfficeId });
                    pm.Add(new MySqlParameter("hodOfficeName", MySqlDbType.String) { Value = blhod.hodOfficeName });
                    pm.Add(new MySqlParameter("baseDeptId", MySqlDbType.Int32) { Value = blhod.baseDeptId });
                    pm.Add(new MySqlParameter("orgType", MySqlDbType.Int16) { Value = blhod.orgType });
                    pm.Add(new MySqlParameter("hodOfficeLevel", MySqlDbType.Int16) { Value = blhod.hodOfficeLevel });
                    pm.Add(new MySqlParameter("hodOfficeStateId", MySqlDbType.Int16) { Value = blhod.hodOfficeStateId });
                    pm.Add(new MySqlParameter("officeCount", MySqlDbType.Int32) { Value = blhod.officeCount });
                    pm.Add(new MySqlParameter("hodOfficeDistrictId", MySqlDbType.Int32) { Value = blhod.hodOfficeDistrictId });
                    pm.Add(new MySqlParameter("hodOfficeDistrictname", MySqlDbType.VarString) { Value = blhod.hodOfficeDistrictname });
                    pm.Add(new MySqlParameter("hodOfficeIsUrbanRural", MySqlDbType.Int16) { Value = (int)blhod.hodOfficeIsUrbanRural });
                    pm.Add(new MySqlParameter("hodOfficePinCode", MySqlDbType.Int32) { Value = blhod.hodOfficePinCode });
                    pm.Add(new MySqlParameter("hodOfficeAddress", MySqlDbType.VarString) { Value = blhod.hodOfficeAddress });
                    pm.Add(new MySqlParameter("hodOfficeEmailId", MySqlDbType.VarString) { Value = blhod.hodOfficeEmailId });
                    pm.Add(new MySqlParameter("hodOfficePhoneNumber", MySqlDbType.VarString) { Value = blhod.hodOfficePhoneNumber });
                    pm.Add(new MySqlParameter("hodOfficeFaxNumber", MySqlDbType.VarString) { Value = blhod.hodOfficeFaxNumber });
                    pm.Add(new MySqlParameter("hodOfficeWebsite", MySqlDbType.VarString) { Value = blhod.hodOfficeWebsite });
                    pm.Add(new MySqlParameter("isRegistrationDocumentUploaded", MySqlDbType.Int16) { Value = (int)blhod.isRegistrationDocumentUploaded });
                    pm.Add(new MySqlParameter("registrationStatus", MySqlDbType.Int16) { Value = (Int16)RegistrationStatus.Pending });
                    pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blhod.clientIp });
                    pm.Add(new MySqlParameter("applicantName", MySqlDbType.VarString) { Value = blhod.applicantName });
                    pm.Add(new MySqlParameter("applicantDesignationCode", MySqlDbType.Int16) { Value = blhod.applicantDesignationCode });
                    pm.Add(new MySqlParameter("applicantMobileNumber", MySqlDbType.Int64) { Value = blhod.applicantMobileNumber });
                    pm.Add(new MySqlParameter("applicantEmailId", MySqlDbType.VarString) { Value = blhod.applicantEmailId });
                    pm.Add(new MySqlParameter("isParichayLogin", MySqlDbType.Int16) { Value = (int)blhod.isParichayLogin });
                    pm.Add(new MySqlParameter("applicantPassword", MySqlDbType.VarString) { Value = blhod.applicantPassword });
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "HodRegistration");
                }
                else
                {
                    rb.message = "Applicant Email-Id has Already Used For Registration!!";
                }
            }
            else
            {
                rb.message = "This Department has Already Applied For Registration!!";
            }
            return rb;
        }
        /// <summary>
        /// Returns 11 digit HodOfficeCode based on StateCode
        /// </summary>
        /// <param name="hodOfficeStateId"></param>
        /// <returns></returns>
        private async Task<ReturnClass.ReturnDataTable> GetHodOfficeRegistrationIdAsync(int hodOfficeStateId)
        {
            string query = @"SELECT LPAD(IFNULL(MAX(h.officeCount),0) + 1, 7, 0) AS SNO,(IFNULL(MAX(h.officeCount),0) + 1) AS  officeCount,
                                '0' AS hodOfficeId
                             FROM hodofficeregistration h
                             WHERE h.hodOfficeStateId = @hodOfficeStateId";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("hodOfficeStateId", MySqlDbType.Int16) { Value = hodOfficeStateId });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
            {
                string officeCount = dt.table.Rows[0]["SNO"].ToString();
                dt.table.Rows[0]["hodOfficeId"] = Convert.ToInt64(DefaultValues.HodOfficePrefix.ToString() + hodOfficeStateId.ToString() + officeCount).ToString();
                return dt;
            }
            else
                return dt;
        }
        public async Task<bool> CheckEmailExistAsync(string emailId)
        {
            bool isAccountExists = true;
            string query = @"SELECT u.emailId
                            FROM userlogin u
                            WHERE u.emailId = @emailId ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("emailId", MySqlDbType.VarString) { Value = emailId });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count < 1)
            {
                isAccountExists = false;

                query = @"SELECT h.applicantEmailId
                          FROM hodofficeregistration h
                          where h.applicantEmailId = @emailId AND h.registrationStatus != @registrationStatus ";
                pm.Add(new MySqlParameter("registrationStatus", MySqlDbType.Int16) { Value = (int)RegistrationStatus.Reject });
                dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());

                if (dt.table.Rows.Count > 0)
                    isAccountExists = true;
            }
            return isAccountExists;
        }
        public async Task<ReturnClass.ReturnDataTable> GetAllHODList(Int16 vid, Int16 rid)
        {
            string query = @"SELECT h.hodOfficeId ,h.officeCount,h.hodOfficeName,h.baseDeptId,h.orgType,h.hodOfficeLevel,
                                     h.hodOfficeStateId,h.hodOfficeDistrictId,h.hodOfficeDistrictname,h.hodOfficeIsUrbanRural,h.hodOfficeAddress,
                                     h.hodOfficePinCode,h.hodOfficeEmailId,h.hodOfficePhoneNumber,h.hodOfficeFaxNumber,h.hodOfficeWebsite,
                                     h.isRegistrationDocumentUploaded,h.isVerified,h.verificationDate,h.registrationStatus,h.registrationDate,
                                    h.applicantName,h.applicantDesignationCode,h.applicantMobileNumber,h.applicantEmailId,h.applicantEmailId,
                                    h.isParichayLogin
                            FROM hodofficeregistration h WHERE h.isVerified=@isVerified AND registrationStatus=@registrationStatus";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = vid });
            pm.Add(new MySqlParameter("registrationStatus", MySqlDbType.Int16) { Value = rid });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }
        public async Task<ReturnClass.ReturnDataTable> GetAllHODListById(Int64 hodOfficeId)
        {
            string query = @"SELECT h.hodOfficeId ,h.hodOfficeName,h.baseDeptId,b.deptNameEnglish,h.orgType,orgeType.nameEnglish AS orgTypeName,h.hodOfficeLevel,
                                     OfficeLevel.nameEnglish AS OfficeLevelName,h.hodOfficeStateId,s.stateNameEnglish AS StateName,h.hodOfficeDistrictId,
									h.hodOfficeDistrictname,h.hodOfficeIsUrbanRural,h.hodOfficeAddress,
                                     h.hodOfficePinCode,h.hodOfficeEmailId,h.hodOfficePhoneNumber,h.hodOfficeFaxNumber,h.hodOfficeWebsite,
                                     h.isRegistrationDocumentUploaded,h.isVerified,h.verificationDate,h.registrationStatus,h.registrationDate,
                                    h.applicantName,h.applicantDesignationCode,ds.designationNameEnglish AS designationName,h.applicantMobileNumber,
									h.applicantEmailId,h.isParichayLogin
                            FROM hodofficeregistration AS  h 
									 JOIN  basedepartment AS b ON b.deptId=h.baseDeptId AND h.hodOfficeStateId=b.stateId
									 JOIN ddlcatlist AS orgeType ON  orgeType.category='organizationType' AND orgeType.id=h.orgType
									 JOIN ddlcatlist AS OfficeLevel ON  OfficeLevel.category='officeLevel' AND OfficeLevel.id=h.hodOfficeLevel
									 JOIN designation AS ds ON  ds.designationId=h.applicantDesignationCode AND ds.stateId=h.hodOfficeStateId
									 JOIN  state AS s ON s.stateId=h.hodOfficeStateId WHERE h.hodOfficeId=@hodOfficeId ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value = hodOfficeId });

            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }
        public async Task<ReturnClass.ReturnBool> VerifyHodOffice(Verification blhod)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            Int32 countData = 0;
            if (blhod.verificationHods.Count != 0)
            {
                foreach (var item in blhod.verificationHods)
                {
                    bool isofficeExists = await CheckVerifyHodOffice(item.hodOfficeId);
                    if (!isofficeExists)
                    {
                        string query = @"UPDATE hodofficeregistration 
                             SET isVerified=@isVerified,clientIp=@clientIp,verificationDate=@verificationDate,
                                verifiedByLoginId=@verifiedByLoginId,registrationStatus=@registrationStatus 
                              WHERE hodOfficeId=@hodOfficeId";
                        if (item.registrationStatus == RegistrationStatus.Approved)
                        {
                            item.isVerified = YesNo.Yes;
                        }
                        else
                        {
                            item.isVerified = YesNo.No;
                        }

                        List<MySqlParameter> pm = new();
                        pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value = item.hodOfficeId });
                        pm.Add(new MySqlParameter("registrationStatus", MySqlDbType.Int16) { Value = (int)item.registrationStatus });
                        pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = (int)item.isVerified });
                        pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blhod.clientIp });
                        pm.Add(new MySqlParameter("verifiedByLoginId", MySqlDbType.Int64) { Value = blhod.userId });
                        pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = (int)item.isVerified });
                        pm.Add(new MySqlParameter("officeMappingKey", MySqlDbType.Int32) { Value = 0 });
                        pm.Add(new MySqlParameter("registrationDate", MySqlDbType.String) { Value = blhod.date });
                        pm.Add(new MySqlParameter("registrationYear", MySqlDbType.Int32) { Value = DateTime.Now.Date.Year });
                        pm.Add(new MySqlParameter("changePassword", MySqlDbType.Int16) { Value = (int)Active.No });
                        pm.Add(new MySqlParameter("active1", MySqlDbType.Int16) { Value = (int)Active.Yes });
                        pm.Add(new MySqlParameter("isDisabled", MySqlDbType.Int16) { Value = (int)Active.No });
                        //pm.Add(new MySqlParameter("userRole", MySqlDbType.Int16) { Value = (int)UserRole.Hospital });
                        pm.Add(new MySqlParameter("isSingleWindowUser", MySqlDbType.Int16) { Value = (int)Active.No });
                        pm.Add(new MySqlParameter("modificationType", MySqlDbType.Int16) { Value = (int)Active.No });
                        pm.Add(new MySqlParameter("userTypeCode", MySqlDbType.Int16) { Value = (int)Active.No });
                        using (TransactionScope ts = new TransactionScope())
                        {
                            rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "VerifyHodOffice");
                            if (rb.status == true && item.registrationStatus == RegistrationStatus.Approved && item.isVerified == YesNo.Yes)
                            {
                                query = @"INSERT INTO userlogin
                                            (userName,userId,emailId,password,changePassword,active,isDisabled,
                                            clientIp,userRole,registrationYear,isSingleWindowUser,modificationType,userTypeCode)
                                        SELECT h.applicantName,h.hodOfficeId,h.applicantEmailId,h.applicantPassword,@changePassword,
                                        @active1,@isDisabled,@clientIp,@userRole,@registrationYear,@isSingleWindowUser,
                                        @modificationType,@userTypeCode
                                            FROM  hodofficeregistration h 
                                          WHERE h.hodOfficeId=@hodOfficeId";
                                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "InsertUserLogin");
                                if (rb.status)
                                {
                                    query = @"INSERT INTO  hodoffice (hodOfficeId,hodOfficeName,baseDeptId,orgType,hodOfficeLevel,hodOfficeStateId,
                                                hodOfficeDistrictId,hodOfficeDistrictname,hodOfficePinCode,hodOfficeAddress,hodOfficeEmailId,
                                                hodOfficePhoneNumber,hodOfficeFaxNumber,hodOfficeWebsite,currentlyActiveHodMappingKey,
                                                loginId,active,clientIp,registrationDate) 
                              SELECT h.hodOfficeId,h.hodOfficeName,h.baseDeptId,h.orgType,h.hodOfficeLevel,h.hodOfficeStateId,
                                    h.hodOfficeDistrictId,h.hodOfficeDistrictname,h.hodOfficePinCode,h.hodOfficeAddress,h.hodOfficeEmailId,
                                   h.hodOfficePhoneNumber,h.hodOfficeFaxNumber,h.hodOfficeWebsite,@officeMappingKey,h.hodOfficeId,
                                   @active,@clientIp,@registrationDate                        
                               FROM  hodofficeregistration h 
                               WHERE h.hodOfficeId=@hodOfficeId";
                                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "InsertHodOffice");
                                    if (rb.status)
                                    {
                                        ts.Complete();
                                        countData = countData + 1;
                                    }
                                    else
                                    {
                                        rb.status = false;
                                    }
                                }

                            }
                            else if (rb.status == true && item.registrationStatus == RegistrationStatus.Reject)
                            {
                                ts.Complete();
                                countData = countData + 1;
                            }


                        }
                    }
                    else
                    {
                        countData = 0;
                        rb.message = "This Department has Already Verified!!";
                    }
                }

                if (blhod.verificationHods.Count == countData)
                {
                    rb.status = true;
                }
                else
                {
                    rb.status = false;
                }
            }
            else
            {

                rb.message = "Department Data Empty!!";
            }
            return rb;
        }

        public async Task<bool> CheckVerifyHodOffice(Int64 hodOfficeId)
        {
            bool isHodOfficeExists = false;
            string query = @"SELECT h.hodOfficeId
                            FROM hodoffice h
                            WHERE h.hodOfficeId = @hodOfficeId  ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value = hodOfficeId });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
            {
                isHodOfficeExists = true;
            }
            return isHodOfficeExists;
        }

        public async Task<bool> CheckHodOffice(BlHod blhod)
        {
            bool isHodOfficeExists = false;
            string query = @"SELECT h.hodOfficeId
                            FROM hodofficeregistration h
                            WHERE h.baseDeptId = @baseDeptId AND h.orgType=@orgType AND h.hodOfficeLevel=@hodOfficeLevel 
                            AND h.hodOfficeStateId=@hodOfficeStateId AND h.hodOfficeDistrictId=@hodOfficeDistrictId  
                            and h.registrationStatus!=registrationStatus ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("baseDeptId", MySqlDbType.Int32) { Value = blhod.baseDeptId });
            pm.Add(new MySqlParameter("orgType", MySqlDbType.Int16) { Value = blhod.orgType });
            pm.Add(new MySqlParameter("hodOfficeLevel", MySqlDbType.Int16) { Value = blhod.hodOfficeLevel });
            pm.Add(new MySqlParameter("hodOfficeStateId", MySqlDbType.Int16) { Value = blhod.hodOfficeStateId });
            pm.Add(new MySqlParameter("hodOfficeDistrictId", MySqlDbType.Int32) { Value = blhod.hodOfficeDistrictId });
            pm.Add(new MySqlParameter("registrationStatus", MySqlDbType.Int16) { Value = (int)RegistrationStatus.Reject });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
            {
                isHodOfficeExists = true;
            }
            return isHodOfficeExists;
        }

        public async Task<ReturnClass.ReturnBool> SaveTicketType(BlTicketType blTicketType)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            if (blTicketType.ticketTypeId == null)
                blTicketType.ticketTypeId = 0;
            bool isofficeExists = await CheckTicketType(blTicketType.ticketTypeName, "INSERT", (Int32)blTicketType.ticketTypeId);
            if (!isofficeExists)
            {
                string query = @"INSERT INTO tickettype (hodOfficeId,ticketTypeId,ticketTypeName,ticketTypeNameLocal,active,clientIp,
                                                entryDateTime,userId)
                                        VALUES (@hodOfficeId,@ticketTypeId,@ticketTypeName,@ticketTypeNameLocal,@active,@clientIp,
                                                @entryDateTime,@userId);";

                blTicketType.ticketTypeId = await GetTicketTypeId();

                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value = blTicketType.hodOfficeId });
                pm.Add(new MySqlParameter("ticketTypeId", MySqlDbType.Int64) { Value = blTicketType.ticketTypeId });
                pm.Add(new MySqlParameter("ticketTypeName", MySqlDbType.String) { Value = blTicketType.ticketTypeName });
                pm.Add(new MySqlParameter("ticketTypeNameLocal", MySqlDbType.String) { Value = blTicketType.ticketTypeNameLocal });
                pm.Add(new MySqlParameter("active", MySqlDbType.String) { Value = (Int16)Active.Yes });
                pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blTicketType.userId });
                pm.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = blTicketType.entryDateTime });
                pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blTicketType.clientIp });

                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "InsertTicketType");

            }
            else
            {
                rb.message = "Ticket Type has Already Exist!!";
            }

            return rb;
        }
        public async Task<ReturnClass.ReturnBool> UpdateTicketType(BlTicketType blTicketType)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            if (blTicketType.ticketTypeId == null)
                blTicketType.ticketTypeId = 0;
            bool isofficeExists = await CheckTicketType(blTicketType.ticketTypeName, "UPDATE", (Int32)blTicketType.ticketTypeId);
            if (!isofficeExists)
            {
                string query = @"INSERT INTO tickettypelog
                                 SELECT * FROM tickettype
                                   WHERE ticketTypeId=@ticketTypeId ;";                

                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value = blTicketType.hodOfficeId });
                pm.Add(new MySqlParameter("ticketTypeId", MySqlDbType.Int64) { Value = blTicketType.ticketTypeId });
                pm.Add(new MySqlParameter("ticketTypeName", MySqlDbType.String) { Value = blTicketType.ticketTypeName });
                pm.Add(new MySqlParameter("ticketTypeNameLocal", MySqlDbType.String) { Value = blTicketType.ticketTypeNameLocal });
                pm.Add(new MySqlParameter("active", MySqlDbType.String) { Value = (Int16)Active.Yes });
                pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blTicketType.userId });
                pm.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = blTicketType.entryDateTime });
                pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blTicketType.clientIp });
                using (TransactionScope ts = new TransactionScope())
                {
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "InsertTicketTypelog");
                    query = @"UPDATE tickettype
                                SET ticketTypeName=@ticketTypeName,ticketTypeNameLocal=@ticketTypeNameLocal,active=@active,clientIp=@clientIp,
                                    entryDateTime=@entryDateTime,userId=@userId WHERE hodOfficeId=@hodOfficeId AND ticketTypeId=@ticketTypeId;";
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "UpdateTicketType");
                    if (rb.status == true)
                    {
                        ts.Complete();
                    }
                }
            }
            else
            {
                rb.message = "Ticket Type has Already Exist!!";
            }

            return rb;
        }

        public async Task<bool> CheckTicketType(string ticketTypeName, string transType, Int32 ticketTypeId)
        {
            bool isHodOfficeExists = false;
            string query = @"SELECT t.ticketTypeId
                            FROM tickettype t
                            WHERE t.ticketTypeName = @ticketTypeName  ";
            if (transType == "UPDATE")
            {
                query = query + " AND t.ticketTypeId!=@ticketTypeId ";
            }
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("ticketTypeName", MySqlDbType.String) { Value = ticketTypeName });
            pm.Add(new MySqlParameter("ticketTypeId", MySqlDbType.Int32) { Value = ticketTypeId });

            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
            {
                isHodOfficeExists = true;
            }
            return isHodOfficeExists;
        }
        public async Task<Int32> GetTicketTypeId()
        {
            string ticketTypeId = "0";
            try
            {
                string qr = @"SELECT IFNULL(MAX(SUBSTRING(t.ticketTypeId,2,7)),0) + 1 AS ticketTypeId
								FROM tickettype t;";

                ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(qr);
                if (dt.table.Rows.Count > 0)
                {
                    ticketTypeId = dt.table.Rows[0]["ticketTypeId"].ToString();
                    ticketTypeId = ((int)idPrefix.empOfficeMappingId).ToString() + ticketTypeId.PadLeft(6, '0');
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return Convert.ToInt32(ticketTypeId);
        }

        public async Task<ReturnClass.ReturnDataTable> GetAllTicketType(Int64 hodOfficeId)
        {
            string query = @"SELECT t.hodOfficeId,t.ticketTypeId,t.ticketTypeName,t.ticketTypeNameLocal
                            FROM tickettype t WHERE t.active=@active AND t.hodOfficeId=@hodOfficeId";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = (Int16)Active.Yes });
            pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value = hodOfficeId });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }
        public async Task<ReturnClass.ReturnDataTable> GetTicketTypeById(Int32 ticketTypeId)
        {
            string query = @"SELECT t.hodOfficeId,t.ticketTypeId,t.ticketTypeName,t.ticketTypeNameLocal
                            FROM tickettype t WHERE t.active=@active AND t.ticketTypeId=@ticketTypeId";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = (Int16)Active.Yes });
            pm.Add(new MySqlParameter("ticketTypeId", MySqlDbType.Int64) { Value = ticketTypeId });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }

        public async Task<ReturnClass.ReturnBool> SaveTicketCategory(BlCategory blCategory)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            if (blCategory.ticketCatId == null)
                blCategory.ticketCatId = 0;
            bool isExists = await CheckCategory(blCategory.categoryName, "INSERT", (Int32)blCategory.ticketCatId);
            if (!isExists)
            {
                string query = @"INSERT INTO ticketcategory (hodOfficeId,ticketCatId,categoryName,categoryNameLocal,active,clientIp,
                                                entryDateTime,userId)
                                        VALUES (@hodOfficeId,@ticketCatId,@categoryName,@categoryNameLocal,@active,@clientIp,
                                                @entryDateTime,@userId);";

                blCategory.ticketCatId = await GetTicketCategoryId();

                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int32) { Value = blCategory.hodOfficeId });
                pm.Add(new MySqlParameter("ticketCatId", MySqlDbType.Int32) { Value = blCategory.ticketCatId });
                pm.Add(new MySqlParameter("categoryName", MySqlDbType.String) { Value = blCategory.categoryName });
                pm.Add(new MySqlParameter("categoryNameLocal", MySqlDbType.String) { Value = blCategory.categoryNameLocal });
                pm.Add(new MySqlParameter("active", MySqlDbType.String) { Value = (Int16)Active.Yes });
                pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blCategory.userId });
                pm.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = blCategory.entryDateTime });
                pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blCategory.clientIp });

                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "InsertTicketCategory");

            }
            else
            {
                rb.message = "Ticket Category has Already Exist!!";
            }

            return rb;
        }
        public async Task<ReturnClass.ReturnBool> UpdateTicketCategory(BlCategory blCategory)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
           
            bool isExists = await CheckCategory(blCategory.categoryName, "UPDATE", (Int32)blCategory.ticketCatId);
            if (!isExists)
            {
                string query = @"INSERT INTO ticketcategorylog
                                 SELECT * FROM ticketcategory
                                   WHERE ticketCatId=@ticketCatId ;";

                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int32) { Value = blCategory.hodOfficeId });
                pm.Add(new MySqlParameter("ticketCatId", MySqlDbType.Int32) { Value = blCategory.ticketCatId });
                pm.Add(new MySqlParameter("categoryName", MySqlDbType.String) { Value = blCategory.categoryName });
                pm.Add(new MySqlParameter("categoryNameLocal", MySqlDbType.String) { Value = blCategory.categoryNameLocal });
                pm.Add(new MySqlParameter("active", MySqlDbType.String) { Value = (Int16)Active.Yes });
                pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blCategory.userId });
                pm.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = blCategory.entryDateTime });
                pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blCategory.clientIp });
                using (TransactionScope ts = new TransactionScope())
                {
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "InsertTicketCategorylog");
                    query = @"UPDATE ticketcategory
                                SET categoryName=@categoryName,categoryNameLocal=@categoryNameLocal,active=@active,clientIp=@clientIp,
                                    entryDateTime=@entryDateTime,userId=@userId WHERE hodOfficeId=@hodOfficeId AND ticketCatId=@ticketCatId;";
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "UpdateTicketCategory");
                    if (rb.status == true)
                    {
                        ts.Complete();
                    }
                }
            }
            else
            {
                rb.message = "Ticket Category has Already Exist!!";
            }

            return rb;
        }

        public async Task<bool> CheckCategory(string categoryName, string transType, Int32 ticketCatId)
        {
            bool isHodOfficeExists = false;
            string query = @"SELECT t.ticketCatId
                            FROM ticketcategory t
                            WHERE t.categoryName = @categoryName  ";
            if (transType == "UPDATE")
            {
                query = query + " AND t.ticketCatId!=@ticketCatId ";
            }
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("categoryName", MySqlDbType.String) { Value = categoryName });
            pm.Add(new MySqlParameter("ticketCatId", MySqlDbType.Int32) { Value = ticketCatId });

            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
            {
                isHodOfficeExists = true;
            }
            return isHodOfficeExists;
        }
        public async Task<Int32> GetTicketCategoryId()
        {
            string ticketCatId = "0";
            try
            {
                string qr = @"SELECT IFNULL(MAX(SUBSTRING(t.ticketCatId,2,7)),0) + 1 AS ticketCatId
								FROM ticketcategory t;";

                ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(qr);
                if (dt.table.Rows.Count > 0)
                {
                    ticketCatId = dt.table.Rows[0]["ticketCatId"].ToString();
                    ticketCatId = ((int)idPrefix.employeeId).ToString() + ticketCatId.PadLeft(6, '0');
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return Convert.ToInt32(ticketCatId);
        }

        public async Task<ReturnClass.ReturnDataTable> GetAllTicketCategory(Int64 hodOfficeId)
        {
            string query = @"SELECT t.hodOfficeId,t.ticketCatId,t.categoryName,t.categoryNameLocal
                            FROM ticketcategory t WHERE t.active=@active AND t.hodOfficeId=@hodOfficeId";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = (Int16)Active.Yes });
            pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value = hodOfficeId });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }
        public async Task<ReturnClass.ReturnDataTable> GetTicketCategoryById(Int32 ticketCatId)
        {
            string query = @"SELECT t.hodOfficeId,t.ticketCatId,t.categoryName,t.categoryNameLocal
                            FROM ticketcategory t WHERE t.active=@active AND t.ticketCatId=@ticketCatId";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = (Int16)Active.Yes });
            pm.Add(new MySqlParameter("ticketCatId", MySqlDbType.Int64) { Value = ticketCatId });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }

        public async Task<ReturnClass.ReturnBool> SaveTicketSubCategory(BlSubCategory blSubCategory)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            if (blSubCategory.ticketSubCatId == null)
                blSubCategory.ticketSubCatId = 0;
            bool isExists = await CheckCategory(blSubCategory.subCategoryName, "INSERT", (Int32)blSubCategory.ticketCatId);
            if (!isExists)
            {
                string query = @"INSERT INTO ticketsubcategory (hodOfficeId,ticketCatId,ticketSubCatId,subCategoryName,subCategoryNameLocal,active,clientIp,
                                                entryDateTime,userId)
                                        VALUES (@hodOfficeId,@ticketCatId,@ticketSubCatId,@subCategoryName,@subCategoryNameLocal,@active,@clientIp,
                                                @entryDateTime,@userId);";

                blSubCategory.ticketSubCatId = await GetTicketSubCategoryId((Int64)blSubCategory.hodOfficeId);

                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int32) { Value = blSubCategory.hodOfficeId });
                pm.Add(new MySqlParameter("ticketCatId", MySqlDbType.Int32) { Value = blSubCategory.ticketCatId });
                pm.Add(new MySqlParameter("ticketSubCatId", MySqlDbType.Int32) { Value = blSubCategory.ticketSubCatId });
                pm.Add(new MySqlParameter("subCategoryName", MySqlDbType.String) { Value = blSubCategory.subCategoryName });
                pm.Add(new MySqlParameter("subCategoryNameLocal", MySqlDbType.String) { Value = blSubCategory.subCategoryNameLocal });
                pm.Add(new MySqlParameter("active", MySqlDbType.String) { Value = (Int16)Active.Yes });
                pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blSubCategory.userId });
                pm.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = blSubCategory.entryDateTime });
                pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blSubCategory.clientIp });

                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "InsertTicketCategory");

            }
            else
            {
                rb.message = "Ticket  Sub Category has Already Exist!!";
            }

            return rb;
        }
        public async Task<ReturnClass.ReturnBool> UpdateTicketSubCategory(BlSubCategory blSubCategory)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();

            bool isExists = await CheckCategory(blSubCategory.subCategoryName, "UPDATE", (Int32)blSubCategory.ticketSubCatId);
            if (!isExists)
            {
                string query = @"INSERT INTO ticketsubcategorylog
                                 SELECT * FROM ticketsubcategory
                                   WHERE ticketSubCatId=@ticketSubCatId ;";

                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int32) { Value = blSubCategory.hodOfficeId });
                pm.Add(new MySqlParameter("ticketCatId", MySqlDbType.Int32) { Value = blSubCategory.ticketCatId });
                pm.Add(new MySqlParameter("ticketSubCatId", MySqlDbType.Int32) { Value = blSubCategory.ticketSubCatId });
                pm.Add(new MySqlParameter("subCategoryName", MySqlDbType.String) { Value = blSubCategory.subCategoryName });
                pm.Add(new MySqlParameter("subCategoryNameLocal", MySqlDbType.String) { Value = blSubCategory.subCategoryNameLocal });
                pm.Add(new MySqlParameter("active", MySqlDbType.String) { Value = (Int16)Active.Yes });
                pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blSubCategory.userId });
                pm.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = blSubCategory.entryDateTime });
                pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blSubCategory.clientIp });
                using (TransactionScope ts = new TransactionScope())
                {
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "InsertTicketCategorylog");
                    query = @"UPDATE ticketsubcategory
                                SET subCategoryName=@subCategoryName,subCategoryNameLocal=@subCategoryNameLocal,active=@active,clientIp=@clientIp,
                                    entryDateTime=@entryDateTime,userId=@userId WHERE hodOfficeId=@hodOfficeId AND ticketSubCatId=@ticketSubCatId;";
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "UpdateTicketSubCategory");
                    if (rb.status == true)
                    {
                        ts.Complete();
                    }
                }
            }
            else
            {
                rb.message = "Ticket Sub Category has Already Exist!!";
            }

            return rb;
        }

        public async Task<bool> CheckSubCategory(string subCategoryName, string transType, Int32 ticketSubCatId)
        {
            bool isHodOfficeExists = false;
            string query = @"SELECT t.ticketCatId
                            FROM ticketsubcategory t
                            WHERE t.subCategoryName = @subCategoryName  ";
            if (transType == "UPDATE")
            {
                query = query + " AND t.ticketSubCatId!=@ticketSubCatId ";
            }
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("subCategoryName", MySqlDbType.String) { Value = subCategoryName });
            pm.Add(new MySqlParameter("ticketSubCatId", MySqlDbType.Int32) { Value = ticketSubCatId });

            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
            {
                isHodOfficeExists = true;
            }
            return isHodOfficeExists;
        }
        public async Task<Int32> GetTicketSubCategoryId(Int64 hodOfficeId)
        {
            string ticketCatId = "0";
            try
            {
                string qr = @"SELECT IFNULL(MAX(SUBSTRING(t.ticketSubCatId,2,7)),0) + 1 AS ticketSubCatId
								FROM ticketsubcategory t WHERE t.hodOfficeId=@hodOfficeId;";
                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value = hodOfficeId });
                ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(qr);
                if (dt.table.Rows.Count > 0)
                {
                    ticketCatId = dt.table.Rows[0]["ticketSubCatId"].ToString();
                    ticketCatId = ((int)idPrefix.employeeId).ToString() + ticketCatId.PadLeft(6, '0');
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return Convert.ToInt32(ticketCatId);
        }    

        public async Task<ReturnClass.ReturnDataTable> GetAllTicketSubCategory(Int64 hodOfficeId ,Int32 ticketCatId)
        {
            string query = @"SELECT t.hodOfficeId,t.ticketCatId,t.categoryName,t.categoryNameLocal,
                                    s.ticketSubCatId,s.subCategoryName,s.subCategoryNameLocal
                            FROM ticketsubcategory AS s
                            JOIN ticketcategory AS t ON t.ticketCatId=s.ticketCatId AND t.hodOfficeId=s.hodOfficeId
                            WHERE s.active=@active AND s.hodOfficeId=@hodOfficeId AND s.ticketCatId=@ticketCatId;";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = (Int16)Active.Yes });
            pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value = hodOfficeId });
            pm.Add(new MySqlParameter("ticketCatId", MySqlDbType.Int32) { Value = ticketCatId });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }
        public async Task<ReturnClass.ReturnDataTable> GetTicketSubCategoryById(Int32 ticketSubCatId)
        {
            string query = @"SELECT t.hodOfficeId,t.ticketCatId,t.categoryName,t.categoryNameLocal,
                                    s.ticketSubCatId,s.subCategoryName,s.subCategoryNameLocal
                            FROM ticketsubcategory AS s
                            JOIN ticketcategory AS t ON t.ticketCatId=s.ticketCatId AND t.hodOfficeId=s.hodOfficeId
                            WHERE s.active=@active AND s.ticketSubCatId=@ticketSubCatId";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = (Int16)Active.Yes });
            pm.Add(new MySqlParameter("ticketSubCatId", MySqlDbType.Int64) { Value = ticketSubCatId });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }
    }
}
