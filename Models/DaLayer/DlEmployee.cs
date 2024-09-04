using BaseClass;
using MySql.Data.MySqlClient;
using System.Transactions;
using HospitalManagementApi.Models.BLayer;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlEmployee
    {
        readonly DBConnection db = new();
        public async Task<ReturnClass.ReturnBool> SaveEmployeeDetail(BlEmployee blEmployee)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            bool isofficeExists = await CheckEmployeeMobileExists(blEmployee.mobileNo, "INSERT", (Int64)blEmployee.employeeId);

            if (!isofficeExists)
            {
                isofficeExists = await CheckEmployeeMobileExists(blEmployee.emailId, "INSERT", (Int64)blEmployee.employeeId);
                if (!isofficeExists)
                {
                    string query = @"INSERT INTO employeemaster (hodOfficeId,stateId,employeeId,employeeName,mobileNo,emailId,workingStatus,
                                                recruitmentType,userId,clientIp,entryDateTime, registrationYear)
                                        VALUES (@hodOfficeId,@stateId,@employeeId,@employeeName,@mobileNo,@emailId,@workingStatus,
                                                @recruitmentType,@userId,@clientIp,@entryDateTime,@registrationYear)";

                    blEmployee.employeeId = await GetEmployeeId((int)blEmployee.registrationYear);

                    List<MySqlParameter> pm = new();
                    pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value = blEmployee.hodOfficeId });
                    pm.Add(new MySqlParameter("employeeId", MySqlDbType.Int64) { Value = blEmployee.employeeId });
                    pm.Add(new MySqlParameter("stateId", MySqlDbType.Int16) { Value = blEmployee.stateId });
                    pm.Add(new MySqlParameter("employeeName", MySqlDbType.String) { Value = blEmployee.employeeName });
                    pm.Add(new MySqlParameter("mobileNo", MySqlDbType.String) { Value = blEmployee.mobileNo });
                    pm.Add(new MySqlParameter("emailId", MySqlDbType.String) { Value = blEmployee.emailId });
                    pm.Add(new MySqlParameter("workingStatus", MySqlDbType.Int16) { Value = blEmployee.workingStatus });
                    pm.Add(new MySqlParameter("recruitmentType", MySqlDbType.Int16) { Value = blEmployee.recruitmentType });
                    pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blEmployee.userId });
                    pm.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = blEmployee.entryDateTime });
                    pm.Add(new MySqlParameter("registrationYear", MySqlDbType.Int32) { Value = (int)blEmployee.registrationYear });
                    pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blEmployee.clientIp });

                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "employeeDetail");
                }
                else
                {
                    rb.message = "Employee Email-Id has Already Used!!";
                }
            }
            else
            {
                rb.message = "Employee Mobile number has Already Used!!";
            }
            return rb;
        }

        public async Task<ReturnClass.ReturnBool> UpdateEmployeeDetail(BlEmployee blEmployee)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            bool isofficeExists = await CheckEmployeeMobileExists(blEmployee.mobileNo, "UPDATE", (Int64)blEmployee.employeeId);

            if (!isofficeExists)
            {
                isofficeExists = await CheckEmployeeMobileExists(blEmployee.emailId, "UPDATE", (Int64)blEmployee.employeeId);
                if (!isofficeExists)
                {
                    string query = @"INSERT INTO employeemasterlog
                                    SELECT * FROM employeemaster e
                                        WHERE e.employeeId=@employeeId";
                    List<MySqlParameter> pm = new();
                    pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value = blEmployee.hodOfficeId });
                    pm.Add(new MySqlParameter("employeeId", MySqlDbType.Int64) { Value = blEmployee.employeeId });
                    pm.Add(new MySqlParameter("stateId", MySqlDbType.Int16) { Value = blEmployee.stateId });
                    pm.Add(new MySqlParameter("employeeName", MySqlDbType.String) { Value = blEmployee.employeeName });
                    pm.Add(new MySqlParameter("mobileNo", MySqlDbType.String) { Value = blEmployee.mobileNo });
                    pm.Add(new MySqlParameter("emailId", MySqlDbType.String) { Value = blEmployee.emailId });
                    pm.Add(new MySqlParameter("workingStatus", MySqlDbType.Int16) { Value = blEmployee.workingStatus });
                    pm.Add(new MySqlParameter("recruitmentType", MySqlDbType.Int16) { Value = blEmployee.recruitmentType });
                    pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blEmployee.userId });
                    pm.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = blEmployee.entryDateTime });
                    pm.Add(new MySqlParameter("registrationYear", MySqlDbType.Int32) { Value = (int)blEmployee.registrationYear });
                    pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blEmployee.clientIp });

                    using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "employeelog");
                        if (rb.status == true)
                        {
                            query = @"UPDATE employeemaster 
                               SET stateId=@stateId,employeeName=@employeeName,mobileNo=@mobileNo,emailId=@emailId,
                               workingStatus=@workingStatus,recruitmentType=@recruitmentType,userId=@userId,clientIp=@clientIp,
                                entryDateTime=@entryDateTime 
                                WHERE employeeId=@employeeId AND hodOfficeId=@hodOfficeId";
                            rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "employeeUpdate");
                            if (rb.status == true)
                            {
                                ts.Complete();
                            }
                        }
                        else
                        {
                            rb.message = "Record Not Updated!!";
                        }
                    }
                }
                else
                {
                    rb.message = "Employee Email-Id has Already Used!!";
                }
            }
            else
            {
                rb.message = "Employee Mobile number has Already Used!!";
            }
            return rb;
        }

        /// <summary>
        /// for EmployeeId generation 12 digit
        /// </summary>
        /// <returns></returns>
        public async Task<Int64> GetEmployeeId(int registrationYear)
        {
            string employeeId = "0";
            try
            {
                string qr = @"SELECT IFNULL(MAX(SUBSTRING(ur.employeeId,6,12)),0) + 1 AS employeeId
								FROM employeemaster ur 
							WHERE ur.registrationYear = @registrationYear";

                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("registrationYear", MySqlDbType.Int32) { Value = registrationYear });

                ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(qr, pm.ToArray());
                if (dt.table.Rows.Count > 0)
                {
                    employeeId = dt.table.Rows[0]["employeeId"].ToString();
                    employeeId = ((int)idPrefix.employeeId).ToString() + registrationYear.ToString() + employeeId.PadLeft(7, '0');
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return Convert.ToInt64(employeeId);
        }

        public async Task<ReturnClass.ReturnDataTable> GetAllEmployeeList(Int64 hodOfficeId)
        {
            string query = @"SELECT e.stateId,e.employeeId,e.employeeName,e.mobileNo,e.emailId,e.workingStatus,
                                        e.recruitmentType,e.registrationYear,
									workingStatus.nameEnglish AS workingStatusName,recruitmentType.nameEnglish AS recruitmentTypeName
                            FROM employeemaster e
                            JOIN ddlcatlist AS workingStatus ON  workingStatus.category='workingStatus' AND workingStatus.id=e.workingStatus
							JOIN ddlcatlist AS recruitmentType ON  recruitmentType.category='recruitmentType' 
                            AND recruitmentType.id=e.recruitmentType 
                            WHERE e.hodOfficeId=@hodOfficeId";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value = hodOfficeId });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }
        public async Task<ReturnClass.ReturnDataTable> GetEmployeeById(Int64 employeeId)
        {
            string query = @"SELECT e.stateId,e.employeeId,e.employeeName,e.mobileNo,e.emailId,e.workingStatus,
                                        e.recruitmentType,e.userId,e.clientIp,e.entryDateTime,e.registrationYear,
									workingStatus.nameEnglish AS workingStatusName,recruitmentType.nameEnglish AS recruitmentTypeName
                            FROM employeemaster e
                            JOIN ddlcatlist AS workingStatus ON  workingStatus.category='workingStatus' AND workingStatus.id=e.workingStatus
							JOIN ddlcatlist AS recruitmentType ON  recruitmentType.category='recruitmentType' AND recruitmentType.id=e.recruitmentType
                    WHERE e.employeeId=@employeeId ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("employeeId", MySqlDbType.Int64) { Value = employeeId });

            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }
        public async Task<bool> CheckEmployeeMobileExists(string mobileNo, string transType, Int64 employeeId)
        {
            bool isHodOfficeExists = false;
            string query = @"SELECT e.employeeId
                            FROM employeemaster e
                            WHERE e.mobileNo=@mobileNo ";
            if (transType == "UPDATE")
            {
                query = query + " AND e.employeeId!=@employeeId ";
            }
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("mobileNo", MySqlDbType.VarChar) { Value = mobileNo });
            pm.Add(new MySqlParameter("employeeId", MySqlDbType.Int64) { Value = employeeId });

            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
            {
                isHodOfficeExists = true;
            }
            return isHodOfficeExists;
        }
        public async Task<bool> CheckEmployeeEmailExists(string emailId, string transType, Int64 employeeId)
        {
            bool isHodOfficeExists = false;
            string query = @"SELECT e.employeeId
                            FROM employeemaster e
                            WHERE e.emailId=@emailId";
            if (transType == "UPDATE")
            {
                query = query + " AND e.employeeId!=@employeeId ";
            }
            List<MySqlParameter> pm = new();

            pm.Add(new MySqlParameter("emailId", MySqlDbType.VarChar) { Value = emailId });
            pm.Add(new MySqlParameter("employeeId", MySqlDbType.Int64) { Value = employeeId });

            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
            {
                isHodOfficeExists = true;
            }
            return isHodOfficeExists;
        }

        public async Task<ReturnClass.ReturnBool> SaveOfficeDetail(BlOffice blOffice)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            bool isofficeExists = await CheckOfficeMobileExists(blOffice.officePhoneNumber, "INSERT", (Int64)blOffice.hodOfficeId, (Int64)blOffice.OfficeId);

            if (!isofficeExists)
            {
                isofficeExists = await CheckOfficeMobileExists(blOffice.officeEmailId, "INSERT", (Int64)blOffice.hodOfficeId, (Int64)blOffice.OfficeId);
                if (!isofficeExists)
                {
                    string query = @"INSERT INTO office (hodOfficeId,officeId,officeName,baseDeptId,officeLevel,districtId,
                                                districtname,urbanRural,pinCode,officeAddress, officeEmailId,officePhoneNumber,userId,
                                            entryDateTime,clientIp)
                                        VALUES (@hodOfficeId,@officeId,@officeName,@baseDeptId,@officeLevel,@districtId,
                                                @districtname,@urbanRural,@pinCode,@officeAddress,@officeEmailId,@officePhoneNumber,@userId,
                                            @entryDateTime,@clientIp)";

                    blOffice.OfficeId = await GetOfficeId((Int64)blOffice.hodOfficeId);

                    List<MySqlParameter> pm = new();
                    pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value = blOffice.hodOfficeId });
                    pm.Add(new MySqlParameter("officeId", MySqlDbType.Int64) { Value = blOffice.OfficeId });
                    pm.Add(new MySqlParameter("officeName", MySqlDbType.String) { Value = blOffice.OfficeName });
                    pm.Add(new MySqlParameter("baseDeptId", MySqlDbType.Int16) { Value = blOffice.baseDeptId });
                    pm.Add(new MySqlParameter("officeLevel", MySqlDbType.Int16) { Value = blOffice.officeLevel });
                    pm.Add(new MySqlParameter("districtId", MySqlDbType.Int16) { Value = blOffice.districtId });
                    pm.Add(new MySqlParameter("districtname", MySqlDbType.String) { Value = blOffice.districtname });
                    pm.Add(new MySqlParameter("urbanRural", MySqlDbType.Int16) { Value = blOffice.urbanRural });
                    pm.Add(new MySqlParameter("officeAddress", MySqlDbType.String) { Value = blOffice.officeAddress });
                    pm.Add(new MySqlParameter("officeEmailId", MySqlDbType.String) { Value = blOffice.officeEmailId });
                    pm.Add(new MySqlParameter("officePhoneNumber", MySqlDbType.String) { Value = blOffice.officePhoneNumber });
                    pm.Add(new MySqlParameter("pinCode", MySqlDbType.String) { Value = blOffice.pinCode });
                    pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blOffice.userId });
                    pm.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = blOffice.entryDateTime });
                    pm.Add(new MySqlParameter("clientIp", MySqlDbType.String) { Value = blOffice.clientIp });

                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "OfficeEntry");
                }
                else
                {
                    rb.message = "Office Email-Id has Already Used!!";
                }
            }
            else
            {
                rb.message = "Office Mobile number has Already Used!!";
            }
            return rb;
        }
        public async Task<ReturnClass.ReturnBool> UpdateOfficeDetail(BlOffice blOffice)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            bool isofficeExists = await CheckOfficeMobileExists(blOffice.officePhoneNumber, "UPDATE", (Int64)blOffice.hodOfficeId, (Int64)blOffice.OfficeId);

            if (!isofficeExists)
            {
                isofficeExists = await CheckOfficeMobileExists(blOffice.officeEmailId, "UPDATE", (Int64)blOffice.hodOfficeId, (Int64)blOffice.OfficeId);
                if (!isofficeExists)
                {

                    string query = @"INSERT INTO officelog
                                       SELECT *
                                        FROM office                                             
                                        WHERE officeId=@officeId;";
                    List<MySqlParameter> pm = new();
                    pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value = blOffice.hodOfficeId });
                    pm.Add(new MySqlParameter("officeId", MySqlDbType.Int64) { Value = blOffice.OfficeId });
                    pm.Add(new MySqlParameter("officeName", MySqlDbType.String) { Value = blOffice.OfficeName });
                    pm.Add(new MySqlParameter("baseDeptId", MySqlDbType.Int16) { Value = blOffice.baseDeptId });
                    pm.Add(new MySqlParameter("officeLevel", MySqlDbType.Int16) { Value = blOffice.officeLevel });
                    pm.Add(new MySqlParameter("districtId", MySqlDbType.Int16) { Value = blOffice.districtId });
                    pm.Add(new MySqlParameter("districtname", MySqlDbType.String) { Value = blOffice.districtname });
                    pm.Add(new MySqlParameter("urbanRural", MySqlDbType.Int16) { Value = blOffice.urbanRural });
                    pm.Add(new MySqlParameter("officeAddress", MySqlDbType.String) { Value = blOffice.officeAddress });
                    pm.Add(new MySqlParameter("officeEmailId", MySqlDbType.String) { Value = blOffice.officeEmailId });
                    pm.Add(new MySqlParameter("officePhoneNumber", MySqlDbType.String) { Value = blOffice.officePhoneNumber });
                    pm.Add(new MySqlParameter("pinCode", MySqlDbType.String) { Value = blOffice.pinCode });
                    pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blOffice.userId });
                    pm.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = blOffice.entryDateTime });
                    pm.Add(new MySqlParameter("clientIp", MySqlDbType.String) { Value = blOffice.clientIp });

                    using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "Officelog");
                        if (rb.status == true)
                        {
                            query = @"UPDATE office 
                                            set hodOfficeId=@hodOfficeId,officeName=@officeName,baseDeptId=@baseDeptId,
                                            officeLevel=@officeLevel,districtId=@districtId,districtname=@districtname,urbanRural=@urbanRural,
                                            pinCode=@pinCode,officeAddress=@officeAddress,officeEmailId=@officeEmailId,
                                            officePhoneNumber=@officePhoneNumber,userId=@userId,entryDateTime=@entryDateTime,
                                            clientIp=@clientIp 
                                            WHERE officeId=@officeId;";



                            rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "OfficeUpdate");
                            if (rb.status == true)
                            {
                                ts.Complete();
                            }
                        }
                        else
                        {
                            rb.message = "Office data Failed To Update!";
                        }
                    }
                }
                else
                {
                    rb.message = "Office Email-Id has Already Used!!";
                }
            }
            else
            {
                rb.message = "Office Mobile number has Already Used!!";
            }
            return rb;
        }
        public async Task<Int64> GetOfficeId(Int64 hodOfficeId)
        {
            string OfficeId = "0";
            try
            {
                string qr = @"SELECT IFNULL(MAX(SUBSTRING(o.officeId,12,13)),0) + 1 AS OfficeId
								FROM office o 
							WHERE o.hodOfficeId = @hodOfficeId";

                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value = hodOfficeId });

                ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(qr, pm.ToArray());
                if (dt.table.Rows.Count > 0)
                {
                    OfficeId = dt.table.Rows[0]["OfficeId"].ToString();
                    OfficeId = hodOfficeId.ToString() + OfficeId.PadLeft(2, '0');
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return Convert.ToInt64(OfficeId);
        }
        public async Task<bool> CheckOfficeMobileExists(string mobileNo, string transType, Int64 hodOfficeId, Int64 officeId)
        {
            bool isHodOfficeExists = false;
            string query = @"SELECT o.officeId
                            FROM office o
                            WHERE o.hodOfficeId=@hodOfficeId AND o.officePhoneNumber=@mobileNo ";
            if (transType == "UPDATE")
            {
                query = query + " AND o.officeId!=@officeId ";
            }
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("mobileNo", MySqlDbType.VarChar) { Value = mobileNo });
            pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value = hodOfficeId });
            pm.Add(new MySqlParameter("officeId", MySqlDbType.Int64) { Value = officeId });

            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
            {
                isHodOfficeExists = true;
            }
            return isHodOfficeExists;
        }
        public async Task<bool> CheckOfficeEmailExists(string emailId, string transType, Int64 hodOfficeId, Int64 officeId)
        {
            bool isHodOfficeExists = false;
            string query = @"SELECT o.officeId
                            FROM office o
                            WHERE o.hodOfficeId=@hodOfficeId AND o.emailId=@emailId ";
            if (transType == "UPDATE")
            {
                query = query + " AND o.officeId!=@officeId ";
            }
            List<MySqlParameter> pm = new();

            pm.Add(new MySqlParameter("emailId", MySqlDbType.VarChar) { Value = emailId });
            pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value = hodOfficeId });
            pm.Add(new MySqlParameter("officeId", MySqlDbType.Int64) { Value = officeId });

            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
            {
                isHodOfficeExists = true;
            }
            return isHodOfficeExists;
        }

        public async Task<ReturnClass.ReturnDataTable> GetAllOfficeList(Int64 hodOfficeId)
        {
            string query = @"SELECT h.hodOfficeId ,h.hodOfficeName,h.baseDeptId,b.deptNameEnglish,h.orgType,orgeType.nameEnglish AS orgTypeName,h.hodOfficeLevel,
                                     OfficeLevel.nameEnglish AS OfficeLevelName,h.hodOfficeStateId,s.stateNameEnglish AS StateName,o.officeId,o.officeName,o.officeLevel,districtId,
                                                districtname,urbanRural,pinCode,officeAddress, officeEmailId,officePhoneNumber
                            FROM office AS o 
                                      JOIN hodofficeregistration AS  h ON o.hodOfficeId=h.hodOfficeId
									 JOIN  basedepartment AS b ON b.deptId=h.baseDeptId
									 JOIN ddlcatlist AS orgeType ON  orgeType.category='organizationType' AND orgeType.id=h.orgType
									 JOIN ddlcatlist AS OfficeLevel ON  OfficeLevel.category='officeLevel' AND OfficeLevel.id=o.officeLevel
									 JOIN designation AS ds ON  ds.designationId=h.applicantDesignationCode 
									 JOIN  state AS s ON s.stateId=h.hodOfficeStateId WHERE o.hodOfficeId=@hodOfficeId ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value = hodOfficeId });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }
        public async Task<ReturnClass.ReturnDataTable> GetOfficeById(Int64 officeId)
        {
            string query = @"SELECT h.hodOfficeId ,h.hodOfficeName,h.baseDeptId,b.deptNameEnglish,h.orgType,orgeType.nameEnglish AS orgTypeName,h.hodOfficeLevel,
                                     OfficeLevel.nameEnglish AS OfficeLevelName,h.hodOfficeStateId,s.stateNameEnglish AS StateName,o.officeId,o.officeName,o.officeLevel,districtId,
                                                districtname,urbanRural,pinCode,officeAddress, officeEmailId,officePhoneNumber
                            FROM office AS o 
                                      JOIN hodofficeregistration AS  h ON o.hodOfficeId=h.hodOfficeId
									 JOIN  basedepartment AS b ON b.deptId=h.baseDeptId 
									 JOIN ddlcatlist AS orgeType ON  orgeType.category='organizationType' AND orgeType.id=h.orgType
									 JOIN ddlcatlist AS OfficeLevel ON  OfficeLevel.category='officeLevel' AND OfficeLevel.id=o.officeLevel
									 JOIN designation AS ds ON  ds.designationId=h.applicantDesignationCode 
									 JOIN  state AS s ON s.stateId=h.hodOfficeStateId WHERE o.officeId=@officeId  ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("officeId", MySqlDbType.Int64) { Value = officeId });

            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }

        public async Task<ReturnClass.ReturnBool> SaveEmpOfficeMapping(BlEmpOffice blEmpOffice)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            bool empOfficeExists = await CheckEmpOfficeExists((Int32)blEmpOffice.empOfficeId, (Int64)blEmpOffice.employeeId, (Int64)blEmpOffice.OfficeId, (Int16)blEmpOffice.userType, "INSERT");

            if (!empOfficeExists)
            {

                string query = @"INSERT INTO employeeofficemapping (empOfficeMappingId,chargeMappingKey,stateId,districtId,employeeId,
                                                officeId,designationId,chargeType,userType,startDate, endDate,active,
                                                userId,entryDateTime,clientIp)
                                        VALUES (@empOfficeMappingId,@chargeMappingKey,@stateId,@districtId,@employeeId,
                                                @officeId,@designationId,@chargeType,@userType,@startDate,@endDate,@active,
                                                @userId,@entryDateTime,@clientIp)";

                blEmpOffice.empOfficeId = await GenerateEmpOfficemappingId();

                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("empOfficeMappingId", MySqlDbType.Int32) { Value = blEmpOffice.empOfficeId });
                pm.Add(new MySqlParameter("chargeMappingKey", MySqlDbType.Int32) { Value = 0 });
                pm.Add(new MySqlParameter("stateId", MySqlDbType.Int16) { Value = blEmpOffice.stateId }); ;
                pm.Add(new MySqlParameter("districtId", MySqlDbType.Int16) { Value = blEmpOffice.districtId });
                pm.Add(new MySqlParameter("employeeId", MySqlDbType.Int64) { Value = blEmpOffice.employeeId });
                pm.Add(new MySqlParameter("officeId", MySqlDbType.Int64) { Value = blEmpOffice.OfficeId });
                pm.Add(new MySqlParameter("designationId", MySqlDbType.Int16) { Value = blEmpOffice.designationId });
                pm.Add(new MySqlParameter("chargeType", MySqlDbType.Int16) { Value = blEmpOffice.chargeType });
                pm.Add(new MySqlParameter("userType", MySqlDbType.Int16) { Value = blEmpOffice.userType });
                pm.Add(new MySqlParameter("startDate", MySqlDbType.String) { Value = blEmpOffice.startDate });
                pm.Add(new MySqlParameter("endDate", MySqlDbType.String) { Value = blEmpOffice.endDate });
                pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = (Int16)Active.Yes });
                pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blEmpOffice.userId });
                pm.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = blEmpOffice.entryDateTime });
                pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blEmpOffice.clientIp });

                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "SaveEmployeeOfficeMapping");

            }
            else
            {
                rb.message = "employee Office Mapping has Already Used!!";
            }
            return rb;
        }
        public async Task<ReturnClass.ReturnBool> UpdateEmpOfficeMapping(BlEmpOffice blEmpOffice)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            bool empOfficeExists = await CheckEmpOfficeExists((Int32)blEmpOffice.empOfficeId, (Int64)blEmpOffice.employeeId, (Int64)blEmpOffice.OfficeId, (Int16)blEmpOffice.userType, "UPDATE");

            if (!empOfficeExists)
            {

                string query = @"INSERT INTO employeeofficemappinglog
                                    SELECT * FROM employeeofficemapping eom
                                WHERE eom.empOfficeMappingId=@empOfficeMappingId";
                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("empOfficeMappingId", MySqlDbType.Int32) { Value = blEmpOffice.empOfficeId });
                pm.Add(new MySqlParameter("chargeMappingKey", MySqlDbType.Int32) { Value = 0 });
                pm.Add(new MySqlParameter("stateId", MySqlDbType.Int16) { Value = blEmpOffice.stateId }); ;
                pm.Add(new MySqlParameter("districtId", MySqlDbType.Int16) { Value = blEmpOffice.districtId });
                pm.Add(new MySqlParameter("employeeId", MySqlDbType.Int64) { Value = blEmpOffice.employeeId });
                pm.Add(new MySqlParameter("officeId", MySqlDbType.Int64) { Value = blEmpOffice.OfficeId });
                pm.Add(new MySqlParameter("designationId", MySqlDbType.Int16) { Value = blEmpOffice.designationId });
                pm.Add(new MySqlParameter("chargeType", MySqlDbType.Int16) { Value = blEmpOffice.chargeType });
                pm.Add(new MySqlParameter("userType", MySqlDbType.Int16) { Value = blEmpOffice.userType });
                pm.Add(new MySqlParameter("startDate", MySqlDbType.String) { Value = blEmpOffice.startDate });
                pm.Add(new MySqlParameter("endDate", MySqlDbType.String) { Value = blEmpOffice.endDate });
                pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = blEmpOffice.active });
                pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blEmpOffice.userId });
                pm.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = blEmpOffice.entryDateTime });
                pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blEmpOffice.clientIp });

                using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "EmployeeOfficeMappingLog");
                    query = @"UPDATE employeeofficemapping 
                               SET chargeMappingKey=@chargeMappingKey,stateId=@stateId,
                                    districtId=@districtId,employeeId=@employeeId,officeId=@officeId,designationId=@designationId,
                                   chargeType=@chargeType,userType=@userType,startDate=@startDate,endDate=@endDate,active=@active,
                                    userId=@userId,entryDateTime=@entryDateTime,clientIp=@clientIp 
                                    WHERE empOfficeMappingId=@empOfficeMappingId;";

                    blEmpOffice.empOfficeId = await GenerateEmpOfficemappingId();


                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "UpdateEmployeeOfficeMapping");
                    if (rb.status == true)
                    {
                        ts.Complete();
                    }
                }


            }
            else
            {
                rb.message = "employee Office Mapping has Already Used!!";
            }
            return rb;
        }

        /// <summary>
        /// generates EmpOfficemappingId of 6 digits in format 2 NNNNN where 2 is prefix for EmpOfficemappingId.
        /// </summary>
        /// <returns></returns>
        public async Task<int> GenerateEmpOfficemappingId()
        {
            string EmpOfficemappingId = "0";
            ReturnClass.ReturnDataTable dt = new ReturnClass.ReturnDataTable();
            dt = await GetEmpOfficemappingId();
            if (dt.table.Rows.Count > 0)
            {
                EmpOfficemappingId = dt.table.Rows[0]["empOfficeMappingId"].ToString();
                EmpOfficemappingId = (int)idPrefix.empOfficeMappingId + EmpOfficemappingId.PadLeft(5, '0');// for type of id, example: 2 for empOfficeMappingId.
            }
            return Convert.ToInt32(EmpOfficemappingId);
        }
        /// <summary>
        /// Get MAX EmpOfficemappingId.
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnClass.ReturnDataTable> GetEmpOfficemappingId()
        {
            ReturnClass.ReturnDataTable dt = new ReturnClass.ReturnDataTable();
            try
            {
                string query = @"SELECT IFNULL(MAX(SUBSTRING(em.empOfficeMappingId,2,6)),0) + 1 AS empOfficeMappingId
								    FROM employeeofficemapping em";

                dt = await db.ExecuteSelectQueryAsync(query);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return dt;
        }

        public async Task<bool> CheckEmpOfficeExists(int empOfficeMappingId, Int64 employeeId, Int64 officeId, Int16 userType, string transType)
        {
            bool empOfficeMappingIdExists = false;
            string query = @"SELECT eom.empOfficeMappingId
                            FROM employeeofficemapping eom
                            WHERE eom.employeeId=@employeeId AND eom.officeId=@officeId AND eom.userType=@userType ";
            if (transType == "UPDATE")
            {
                query = query + " AND eom.empOfficeMappingId!=@empOfficeMappingId ";
            }
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("employeeId", MySqlDbType.Int64) { Value = employeeId });
            pm.Add(new MySqlParameter("userType", MySqlDbType.Int16) { Value = userType });
            pm.Add(new MySqlParameter("officeId", MySqlDbType.Int64) { Value = officeId });
            pm.Add(new MySqlParameter("empOfficeMappingId", MySqlDbType.Int32) { Value = empOfficeMappingId });

            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
            {
                empOfficeMappingIdExists = true;
            }
            return empOfficeMappingIdExists;
        }

        public async Task<ReturnClass.ReturnDataTable> GetAllEmployeeOfficeList(Int64 hodOfficeId)
        {
            string query = @"SELECT eom.empOfficeMappingId,eom.stateId,s.stateNameEnglish AS stateName,
			                        eom.districtId,d.districtNameEnglish AS districtName,eom.employeeId,e.employeeName,
			                        eom.officeId,o.officeName,eom.designationId,ds.designationNameEnglish AS designationName,
			                        eom.chargeType,chargeType.nameEnglish AS chargeTypeName,eom.userType,userType.nameEnglish AS userTypeName,
			                        eom.startDate,eom.endDate,eom.active			
				                         FROM employeeofficemapping eom 
				                        JOIN employeemaster e ON e.employeeId=eom.employeeId
				                        JOIN office o ON o.officeId=eom.officeId AND o.hodOfficeId =e.hodOfficeId
				                        JOIN state s ON s.stateId=eom.stateId
				                        JOIN district d ON d.districtId=eom.districtId
				                        JOIN ddlcatlist chargeType ON chargeType.id=eom.chargeType AND chargeType.category='chargeType'
					                        JOIN ddlcatlist userType ON userType.id=eom.userType AND chargeType.category='userType'
					                        JOIN designation ds ON ds.designationId=eom.designationId WHERE o.hodOfficeId=@hodOfficeId ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value = hodOfficeId });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }
        public async Task<ReturnClass.ReturnDataTable> GetEmployeeOfficeById(Int32 empOfficeMappingId)
        {
            string query = @"SELECT eom.empOfficeMappingId,eom.stateId,s.stateNameEnglish AS stateName,
			                        eom.districtId,d.districtNameEnglish AS districtName,eom.employeeId,e.employeeName,
			                        eom.officeId,o.officeName,eom.designationId,ds.designationNameEnglish AS designationName,
			                        eom.chargeType,chargeType.nameEnglish AS chargeTypeName,eom.userType,userType.nameEnglish AS userTypeName,
			                        eom.startDate,eom.endDate,eom.active			
				                         FROM employeeofficemapping eom 
				                        JOIN employeemaster e ON e.employeeId=eom.employeeId
				                        JOIN office o ON o.officeId=eom.officeId 
				                        JOIN state s ON s.stateId=eom.stateId
				                        JOIN district d ON d.districtId=eom.districtId
				                        JOIN ddlcatlist chargeType ON chargeType.id=eom.chargeType AND chargeType.category='chargeType'
					                        JOIN ddlcatlist userType ON userType.id=eom.userType AND chargeType.category='userType'
					                        JOIN designation ds ON ds.designationId=eom.designationId WHERE 
                                    eom.empOfficeMappingId=@empOfficeMappingId  ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("empOfficeMappingId", MySqlDbType.Int64) { Value = empOfficeMappingId });

            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }

        public async Task<List<ListValue>> GetEmployeeByDistrict(Int64 hodOfficeId)
        {

            string query = @"SELECT e.employeeId AS id,e.employeeName AS name
                            FROM employeemaster e                            
                            WHERE e.hodOfficeId=@hodOfficeId";
            MySqlParameter[] pm = new MySqlParameter[]
            {
                new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value= hodOfficeId }
            };
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm);
            List<ListValue> lv = Helper.GetGenericDropdownList(dt.table);
            return lv;
        }
        public async Task<List<ListValue>> GetOfficeByDistrict(Int64 hodOfficeId, Int16 districtId)
        {

            string query = @"SELECT o.officeId  AS id,o.officeName AS name
                            FROM office AS o                                       								 
									 WHERE o.hodOfficeId=@hodOfficeId AND o.districtId=@districtId ";
            MySqlParameter[] pm = new MySqlParameter[]
            {
                new MySqlParameter("districtId", MySqlDbType.Int16) { Value= districtId },
                new MySqlParameter("hodOfficeId", MySqlDbType.Int64) { Value= hodOfficeId }
            };
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm);
            List<ListValue> lv = Helper.GetGenericDropdownList(dt.table);
            return lv;
        }
    }
}
