using BaseClass;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using HospitalManagementApi.Models.BLayer;
using static HospitalManagementApi.Models.BLayer.BlCommon;
using System.Security.Policy;
using System;
using System.Data.Common;
using System.Data;
using HospitalManagementApi.Models.Balayer;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlPatient
    {
        readonly DBConnection db = new();
        ReturnClass.ReturnDataTable dt = new();
        DlCommon dl = new();
        Utilities util = new Utilities();
        string key_id = "", key_secret = "", currency = "", checkoutURL = "";
        public async Task<ReturnClass.ReturnBool> RegisterNewPatient(BlPatient blPatient)
        {
            DlCommon dlcommon = new();
            string pass = "";
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            if (blPatient.patientRegNo == null || blPatient.patientRegNo == 0)
            {
                blPatient.patientRegNo = 0;
                blPatient.patientId = await GeneratePatientId((int)blPatient.pinCode, blPatient.patientNameEnglish, blPatient.genderId);
            }
            bool isExists = await CheckMobileExistAsync(blPatient.mobileNo, "INSERT", (Int64)blPatient.patientRegNo);
            //BlCity blcity = new();
            //blcity.cityId = (Int32)blPatient.cityId!;
            //blcity.cityNameEnglish = blPatient.cityName!;
            //blPatient.cityId = await dlcommon.ReturnCity(blcity);
            if (!isExists)
            {
                isExists = await CheckEmailExistAsync(blPatient.emailId, "INSERT", (Int64)blPatient.patientRegNo);
                if (!isExists)
                {
                    using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {

                        string query = @"INSERT INTO patientregistration (patientRegNo,patientNameEnglish,patientNameLocal,mobileNo,emailId,active,isVerified
                                                    ,otp,userId,entryDateTime,clientIp,registrationYear,genderId,pincode,patientId)
                                        VALUES (@patientRegNo,@patientNameEnglish,@patientNameLocal,@mobileNo,@emailId,@active,@isVerified
                                                    ,@otp,@userId,@entryDateTime,@clientIp,@registrationYear,@genderId,@pincode,@patientId)";
                        blPatient.patientRegNo = await GetPatientId((int)blPatient.registrationYear);

                        Utilities util = new Utilities();
                        Int64 smsotp = util.GenRendomNumber(4);
                        List<MySqlParameter> pm = new();
                        if (blPatient.emailId == null || blPatient.emailId.Length == 0)
                            blPatient.emailId = blPatient.mobileNo;


                        pm.Add(new MySqlParameter("patientRegNo", MySqlDbType.Int64) { Value = blPatient.patientRegNo });
                        pm.Add(new MySqlParameter("patientNameEnglish", MySqlDbType.String) { Value = blPatient.patientNameEnglish });
                        pm.Add(new MySqlParameter("patientNameLocal", MySqlDbType.String) { Value = blPatient.patientNameLocal });
                        // pm.Add(new MySqlParameter("stateId", MySqlDbType.Int16) { Value = blPatient.stateId });
                        // pm.Add(new MySqlParameter("districtId", MySqlDbType.Int16) { Value = blPatient.districtId });
                        // pm.Add(new MySqlParameter("address", MySqlDbType.String) { Value = blPatient.address });
                        pm.Add(new MySqlParameter("mobileNo", MySqlDbType.String) { Value = blPatient.mobileNo });
                        pm.Add(new MySqlParameter("emailId", MySqlDbType.String) { Value = blPatient.emailId });
                        pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = (int)Active.No });
                        pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = (int)Active.No });
                        pm.Add(new MySqlParameter("otp", MySqlDbType.Int32) { Value = smsotp });
                        // pm.Add(new MySqlParameter("registrationYear", MySqlDbType.Int32) { Value = blPatient.registrationYear });
                        pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blPatient.clientIp });
                        pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blPatient.userId });
                        pm.Add(new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = blPatient.entryDateTime });
                        pm.Add(new MySqlParameter("genderId", MySqlDbType.Int16) { Value = blPatient.genderId });
                        pm.Add(new MySqlParameter("pincode", MySqlDbType.Int32) { Value = blPatient.pinCode });
                        pm.Add(new MySqlParameter("patientId", MySqlDbType.String) { Value = blPatient.patientId });
                        pm.Add(new MySqlParameter("Password", MySqlDbType.String) { Value = blPatient.password });
                        pm.Add(new MySqlParameter("isDisabled", MySqlDbType.Int16) { Value = Active.No });
                        pm.Add(new MySqlParameter("userRole", MySqlDbType.Int16) { Value = UserRole.Patient });
                        pm.Add(new MySqlParameter("isSingleWindowUser", MySqlDbType.Int16) { Value = Active.No });
                        pm.Add(new MySqlParameter("modificationType", MySqlDbType.Int16) { Value = Active.No });
                        pm.Add(new MySqlParameter("userTypeCode", MySqlDbType.Int16) { Value = Active.No });
                        pm.Add(new MySqlParameter("changePassword", MySqlDbType.Int16) { Value = Active.No });
                        pm.Add(new MySqlParameter("registrationYear", MySqlDbType.Int32) { Value = Convert.ToInt32(DateTime.Now.Year.ToString()) });
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "patientregistration");
                        if (rb.status)
                        {
                            query = @"INSERT INTO userlogin
                                            (userName,userId,emailId,password,changePassword,active,isDisabled,
                                            clientIp,userRole,registrationYear,isSingleWindowUser,modificationType,userTypeCode)
                                    VALUES (@patientNameEnglish,@patientRegNo,@emailId,@password, @changePassword, @active, @isDisabled,
                                        @clientIp,@userRole, @registrationYear,@isSingleWindowUser,@modificationType,@userTypeCode)";
                            rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "InsertUserLogin");
                            if (rb.status)
                            {
                                ts.Complete();
                                rb.error = smsotp.ToString();
                                rb.message = "User Registered!! Please Complete OTP verification!!";
                                rb.value = blPatient.patientRegNo.ToString();
                            }

                        }
                        else
                            rb.message = "Please try after some time!!";

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
        public async Task<Int64> GetPatientId(int registrationYear)
        {
            string PatientId = "0";
            try
            {
                string qr = @"SELECT IFNULL(MAX(SUBSTRING(ur.PatientRegNo,6,12)),0) + 1 AS PatientId
        	                    FROM patientregistration ur 
                            WHERE ur.registrationYear = @registrationYear";

                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("registrationYear", MySqlDbType.Int32) { Value = registrationYear });

                ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(qr, pm.ToArray());
                if (dt.table.Rows.Count > 0)
                {
                    PatientId = dt.table.Rows[0]["PatientId"].ToString();
                    PatientId = ((int)idPrefix.PatientRegistrationId).ToString() + registrationYear.ToString() + PatientId.PadLeft(7, '0');
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return Convert.ToInt64(PatientId);
        }
        public async Task<bool> CheckEmailExistAsync(string emailId, string transType, Int64 PatientRegNo)
        {
            bool isAccountExists = false;
            string query = @"SELECT u.emailId
                             FROM patientregistration u
                             WHERE u.emailId = @emailId ";
            if (transType == "UPDATE")
            {
                query = query + " AND u.PatientRegNo!=@PatientRegNo ";
            }
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("emailId", MySqlDbType.VarString) { Value = emailId });
            pm.Add(new MySqlParameter("PatientRegNo", MySqlDbType.Int64) { Value = PatientRegNo });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
                isAccountExists = true;
            if (isAccountExists)
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
        public async Task<bool> CheckMobileExistAsync(string mobileNo, string transType, Int64 PatientRegNo)
        {
            bool isAccountExists = false;
            string query = @"SELECT u.mobileNo
                                    FROM patientregistration u
                               WHERE u.mobileNo = @mobileNo ";
            if (transType == "UPDATE")
            {
                query = query + " AND u.PatientRegNo!=@PatientRegNo ";
            }
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("mobileNo", MySqlDbType.VarString) { Value = mobileNo });
            pm.Add(new MySqlParameter("PatientRegNo", MySqlDbType.Int64) { Value = PatientRegNo });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dt.table.Rows.Count > 0)
                isAccountExists = true;
            return isAccountExists;
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

        public async Task<ReturnClass.ReturnBool> VerifyOTP(long patientRegNo, Int32 OTP, string mobileNo)
        {
            ReturnClass.ReturnBool rb = new();
            string query = "";
            MySqlParameter[] pm = new MySqlParameter[]
           {
                new MySqlParameter("patientRegNo", MySqlDbType.String) { Value = patientRegNo},
                new MySqlParameter("mobileNo", MySqlDbType.String) { Value = mobileNo},
                new MySqlParameter("otp", MySqlDbType.Int32) { Value = OTP},
                new MySqlParameter("active", MySqlDbType.Int16) { Value = (int)Active.Yes},
                new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = (int)Active.Yes},
                new MySqlParameter("userRole", MySqlDbType.Int16) { Value = UserRole.Patient }
        };
            query = @"SELECT e.patientRegNo
                             FROM patientregistration e
                             WHERE e.patientRegNo = @patientRegNo AND  e.mobileNo = @mobileNo  AND e.otp=@otp  ";
            dt = await db.ExecuteSelectQueryAsync(query, pm);
            if (dt.table.Rows.Count > 0)
            {
                rb.value = dt.table.Rows[0]["patientRegNo"].ToString();
                query = @"UPDATE patientregistration p
                              SET
                        p.active=@active,p.isVerified=@isVerified
                             WHERE p.patientRegNo = @patientRegNo  ";
                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "OTPVerify");
                if (rb.status)
                {
                    query = @"UPDATE userlogin p
                              SET
                        p.active=@active
                             WHERE p.userId = @patientRegNo AND p.userRole=@userRole ";
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "userloginVerify");
                    rb.status = true;
                }
            }
            return rb;
        }

        public async Task<ReturnClass.ReturnBool> SetPassword(BlPatientOtp verificationDetail)
        {
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();


            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("patientRegNo", MySqlDbType.Int64) { Value = verificationDetail.patientRegNo });
            pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = verificationDetail.clientIp });
            pm.Add(new MySqlParameter("verifiedByLoginId", MySqlDbType.Int64) { Value = verificationDetail.userId });
            pm.Add(new MySqlParameter("registrationYear", MySqlDbType.Int32) { Value = DateTime.Now.Date.Year });
            pm.Add(new MySqlParameter("changePassword", MySqlDbType.Int16) { Value = (int)Active.No });
            pm.Add(new MySqlParameter("active1", MySqlDbType.Int16) { Value = (int)Active.Yes });
            pm.Add(new MySqlParameter("isDisabled", MySqlDbType.Int16) { Value = (int)Active.No });
            pm.Add(new MySqlParameter("userRole", MySqlDbType.Int16) { Value = (int)UserRole.Patient });
            pm.Add(new MySqlParameter("isSingleWindowUser", MySqlDbType.Int16) { Value = (int)Active.No });
            pm.Add(new MySqlParameter("modificationType", MySqlDbType.Int16) { Value = (int)Active.No });
            pm.Add(new MySqlParameter("userTypeCode", MySqlDbType.Int16) { Value = (int)Active.No });
            pm.Add(new MySqlParameter("Password", MySqlDbType.String) { Value = verificationDetail.password });


            string query = @"INSERT INTO userlogin
                                            (userName,userId,emailId,password,changePassword,active,isDisabled,
                                            clientIp,userRole,registrationYear,isSingleWindowUser,modificationType,userTypeCode)
                                        SELECT p.patientNameEnglish,p.patientRegNo,p.emailId,@Password,@changePassword,
                                        @active1,@isDisabled,@clientIp,@userRole,p.registrationYear,@isSingleWindowUser,
                                        @modificationType,@userTypeCode
                                            FROM  patientregistration p 
                                          WHERE p.patientRegNo=@patientRegNo";
            rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "InsertUserLogin(Patient)");
            if (rb.status)
                rb.message = "Registration Completed!!";
            return rb;
        }


        public async Task<ReturnClass.ReturnDataTable> GetPatientSlotsHistory(Int64 patientRegNo)
        {
            string query = @"SELECT pb.appointmentNo,pb.doctorRegNo,pr.patientRegNo,
			                        pb.consultancyFee,pb.bookingFee,pb.videoCallFee,
			                        pr.patientNameEnglish,pr.patientId,
			                        DATE_FORMAT(dstd.scheduleDate,'%d/%m/%Y') AS scheduleDate,dstd.fromTime,
                                dstd.toTime,pb.timeslot,
			                        dr.doctorNameEnglish,dr.doctorNameLocal,dr.address
		                        FROM patienttimeslotbooking AS pb 
	                            INNER JOIN doctorscheduletimedatewise AS dstd ON dstd.scheduleTimeId = pb.scheduleTimeId
	                            INNER JOIN patientregistration AS pr ON pr.patientRegNo = pb.patientRegNo
	                            INNER JOIN doctorregistration dr ON dr.doctorRegNo=pb.doctorRegNo
	                            WHERE pb.patientRegNo=@patientRegNo
                         ORDER BY dstd.scheduleDate DESC";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("patientRegNo", MySqlDbType.Int64) { Value = patientRegNo });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }

        public async Task<ReturnClass.ReturnDataTable> GetAppointmentCalender(Int64 doctorRegNo)
        {
            string query = @"WITH RECURSIVE Date_Ranges AS (
                                SELECT ELT(DAYOFWEEK(NOW()), 'SUNDAY', 'MONDAY', 'TUESDAY', 'WEDNESDAY','THURSDAY', 'FRIDAY', 'SATURDAY')
	 	                            AS daysOfWeek,CURDATE() as Date
                               UNION ALL 
                               SELECT ELT(DAYOFWEEK(Date + INTERVAL 1 day), 'SUNDAY', 'MONDAY', 'TUESDAY', 'WEDNESDAY','THURSDAY', 'FRIDAY', 'SATURDAY')
		                            AS daysOfWeek,Date + INTERVAL 1 DAY
                               from Date_Ranges
                               where Date <  DATE_ADD(CURDATE(), INTERVAL 14 DAY))
	                            SELECT dr.daysOfWeek,dr.Date,dsd.scheduleDateId,dst.scheduleTimeId,dsd.doctorRegNo,dsd.dayId,dsd.`day`,dst.fromTime,dst.toTime,dst.patientLimit
			                            ,CASE WHEN IFNULL(dstd.scheduleTimeId,0)=0 AND IFNULL(dst.scheduleDateId,0)!=0 THEN 'Avail' 
				                            WHEN IFNULL(dstd.scheduleTimeId,0)!=0 AND IFNULL(dst.scheduleDateId,0)!=0 THEN 'Not Available' ELSE
				                            'Not Scheduled' END AS Availability,
                                CASE WHEN dwa.venueTypeId = 1 THEN 'Clinic' WHEN dwa.venueTypeId = 2 THEN 'Hospital' END
                                AS venueTypeName,dwa.venueTypeId,dwa.doctorWorkAreaId, dwa.hospitalRegNo AS venueRegNo, dwa.hospitalNameEnglish AS venueName,
                                dwa.hospitalAddress AS venueAddress
                                    FROM doctorscheduledate AS dsd 
                                    INNER JOIN doctorworkarea AS dwa ON dsd.doctorWorkAreaId = dwa.doctorWorkAreaId
                                    INNER JOIN doctorscheduletime AS dst ON dsd.scheduleDateId = dst.scheduleDateId 
                                    RIGHT JOIN Date_Ranges AS dr ON dr.daysOfWeek=dsd.`day` AND dsd.doctorRegNo=@doctorRegNo
                                AND dsd.isActive=@isActive AND dst.isActive=@isActive
                                     LEFT JOIN doctorscheduletimedatewise AS dstd 
				                            ON dstd.scheduleTimeId=dst.scheduleTimeId AND dstd.scheduleDate = dr.date AND dsd.doctorRegNo=@doctorRegNo
		                            WHERE IFNULL(dstd.scheduleTimeId,0)=0 AND IFNULL(dst.scheduleDateId,0)!=0 
                                ORDER BY dr.Date ";
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo });
            pm.Add(new MySqlParameter("isActive", MySqlDbType.Int16) { Value = YesNo.Yes });
            ReturnClass.ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return dt;
        }

        public async Task<string> GeneratePatientId(Int32 pincode, string name, string genderId)
        {
            string query = string.Empty;
            string patientId = pincode.ToString() + name.Substring(0, 3).ToString().ToUpper() + genderId;
            // ID Format = P(5) SS NNNN NNNN NN
            Utilities util = new Utilities();
        ReExecute:
            long rno = util.GenRendomNumber(3);
            string id = patientId + rno;
            bool isExist = await VerifyPatientId(patientId);
            if (isExist)
                goto ReExecute;
            else
                return id;

        }

        private async Task<bool> VerifyPatientId(string patientId)
        {
            string query = "";
            query = @"SELECT patientId
								FROM patientregistration AS pr 
                    WHERE pr.patientId = @patientId;";

            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("patientId", MySqlDbType.String) { Value = patientId });
            ReturnClass.ReturnDataTable dataTable = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dataTable.table.Rows.Count > 0)
                return true;
            else
                return false;
        }
        private async Task<bool> ISPaymentTransactionExists(Int64 transactionNo)
        {
            string query = "";
            query = @"SELECT e.patientRegNo
								FROM ewalletrequest AS e 
                    WHERE e.transactionNo = @transactionNo;";

            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("transactionNo", MySqlDbType.Int64) { Value = transactionNo });
            ReturnClass.ReturnDataTable dataTable = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dataTable.table.Rows.Count > 0)
                return true;
            else
                return false;

        }
        private async Task<bool> ISPaymentTransactionPending(Int64 transactionNo)
        {
            string query = "";
            query = @"SELECT e.paymentStatus
								FROM ewalletrequest AS e 
                    WHERE e.transactionNo = @transactionNo;";

            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("transactionNo", MySqlDbType.Int64) { Value = transactionNo });
            ReturnClass.ReturnDataTable dataTable = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dataTable.table.Rows.Count > 0)
            {
                if (Convert.ToInt16(dataTable.table.Rows[0]["paymentStatus"].ToString()) == (Int16)PaymentStatus.TransactionPending)
                    return true;
                else
                    return false;
            }
            else
                return false;

        }
        public async Task<Int64> GeneratePaymentTransaction()
        {
            Int32 counter = 0;
        ReExecute:
            counter = counter + 1;
            Int64 transactionNo = Convert.ToInt64(DateTime.Now.ToString("yyMMddHHmmss") + counter.ToString().PadLeft(2, '0'));
            bool isExist = await ISPaymentTransactionExists(transactionNo);
            if (isExist)
                goto ReExecute;
            else
                return transactionNo;

        }

        private async Task<CreatePaymentOrder> createPaymentRequest(BlAddWallet blAddWallet)
        {

            ReturnClass.ReturnBool rb = util.GetAppSettings("Build", "Version");
            if (rb.status)
            {
                string buildType = rb.message.ToLower();
                if (buildType == "production")
                {
                    key_id = util.GetAppSettings("RazorPay", "Production", "key_id").message!;
                    key_secret = util.GetAppSettings("RazorPay", "Production", "key_secret").message!;
                    currency = util.GetAppSettings("RazorPay", "Production", "currency").message!;
                    checkoutURL = util.GetAppSettings("RazorPay", "Production", "checkoutURL").message!;
                }
                if (buildType == "localdevelopment")
                {
                    key_id = util.GetAppSettings("RazorPay", "LocalDevelopment", "key_id").message!;
                    key_secret = util.GetAppSettings("RazorPay", "LocalDevelopment", "key_secret").message!;
                    currency = util.GetAppSettings("RazorPay", "LocalDevelopment", "currency").message!;
                    checkoutURL = util.GetAppSettings("RazorPay", "LocalDevelopment", "checkoutURL").message!;
                }
            }

            Razorpay.Api.RazorpayClient razorpayClient = new Razorpay.Api.RazorpayClient(key_id, key_secret);
            Dictionary<string, object> option = new Dictionary<string, object>();
            option.Add("amount", blAddWallet.walletAmount! * 100);
            option.Add("receipt", blAddWallet.transactionNo.ToString()!);
            option.Add("currency", currency);
            option.Add("payment_capture", 0);//1=Automatic , 0=manual   
            Razorpay.Api.Order orderRespense = razorpayClient.Order.Create(option);
            blAddWallet.razorPaytransactionNo = orderRespense["id"].ToString();
            ReturnClass.ReturnDataTable dt1 = await GetPatientProfile((long)blAddWallet.userId!);

            CreatePaymentOrder createPaymentOrder = new CreatePaymentOrder
            {
                OrderId = orderRespense["id"],
                RazorpayKey = key_id,
                amount = Convert.ToInt32(blAddWallet.walletAmount!),
                Currency = currency,
                Name = dt1.table.Rows[0]["patientNameEnglish"].ToString(),
                Email = dt1.table.Rows[0]["emailId"].ToString(),
                PhoneNumber = dt1.table.Rows[0]["mobileNo"].ToString(),
                Address = " ",
                Description = "Merchant Order",
                checkOutURL = checkoutURL,
                transactionNo = blAddWallet.transactionNo

            };
            return createPaymentOrder;

        }
        public async Task<CreatePaymentOrder> AddWallet(BlAddWallet blAppointment)
        {
            DlCommon dlcommon = new();
            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            CreatePaymentOrder createPaymentOrder = new CreatePaymentOrder();
            if (blAppointment.patientRegNo == null || blAppointment.patientRegNo == 0)
            {
                createPaymentOrder.status = false;
                createPaymentOrder.message = "Invalid Patient";
                return createPaymentOrder;
            }
            blAppointment.transactionNo = await GeneratePaymentTransaction();
            createPaymentOrder = await createPaymentRequest(blAppointment);
            if (createPaymentOrder.OrderId != null || createPaymentOrder.OrderId != string.Empty)
            {
                List<MySqlParameter> pm = new();
                pm.Add(new MySqlParameter("patientRegNo", MySqlDbType.Int64) { Value = blAppointment.patientRegNo });
                pm.Add(new MySqlParameter("walletAmount", MySqlDbType.Decimal) { Value = blAppointment.walletAmount });
                pm.Add(new MySqlParameter("Remark", MySqlDbType.VarChar) { Value = blAppointment.Remark });
                pm.Add(new MySqlParameter("transactionNo", MySqlDbType.VarChar) { Value = blAppointment.transactionNo });
                pm.Add(new MySqlParameter("razorPayTransactionNo", MySqlDbType.VarChar) { Value = blAppointment.razorPaytransactionNo });
                pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blAppointment.clientIp });
                pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blAppointment.userId });
                pm.Add(new MySqlParameter("paymentStatus", MySqlDbType.Int32) { Value = (Int16)PaymentStatus.TransactionPending });
                pm.Add(new MySqlParameter("paymentStatusName", MySqlDbType.VarChar) { Value = "Transaction Pending" });
                string query = @" INSERT INTO ewalletrequest (patientRegNo,walletAmount,Remark,entryDateTime,userId,
                                            clientIp,transactionNo,paymentStatus,paymentStatusName,razorPayTransactionNo)
 									VALUES
 									              (@patientRegNo,@walletAmount,@Remark,NOW(),@userId,@clientIp,
                                                @transactionNo,@paymentStatus,@paymentStatusName,@razorPayTransactionNo);";
                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "SubmitWallet");
                if (rb.status)
                {
                    createPaymentOrder.message = "Pending For Payment.";
                    createPaymentOrder.status = true;
                }
                else
                    createPaymentOrder.message = "Please try after some time!";
            }
            return createPaymentOrder;
        }
        private async Task<Int32> GetewalletActionId(long patientId)
        {
            string query = @"SELECT IFNULL(MAX(p.actionId),0) + 1 AS actionId
								FROM ewallettransaction AS p 
                    WHERE p.patientRegNo = @patientRegNo;";

            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("patientRegNo", MySqlDbType.Int64) { Value = patientId });
            ReturnClass.ReturnDataTable dataTable = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            return Convert.ToInt32(dataTable.table.Rows[0]["actionId"]);
        }

        public async Task<ReturnClass.ReturnDataTable> GetwalletlistByUser(long userId)
        {
            //e.walletAmount,
            string query = @"SELECT e.patientRegNo,
                            e.walletBalanceAmount,e.Remark
                                    FROM ewalletmaster e
                                    WHERE e.userId=@userId ;";

            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = userId });
            ReturnClass.ReturnDataTable dataTable = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dataTable == null || dataTable.table.Rows.Count == 0)
            {
                dataTable = new ReturnClass.ReturnDataTable();
                dataTable.table.Columns.Add("patientRegNo");
                //dataTable.table.Columns.Add("walletAmount");
                dataTable.table.Columns.Add("walletBalanceAmount");
                dataTable.table.Columns.Add("Remark");
                DataRow dr = dataTable.table.NewRow();
                dr["patientRegNo"] = userId.ToString();
                // dr["walletAmount"] = "0";
                dr["walletBalanceAmount"] = "0";
                dr["Remark"] = "Wallet Empty";
                dataTable.table.Rows.Add(dr);
                dataTable.message = "Wallet Empty";
                dataTable.status = false;
            }

            return dataTable;
        }
        public async Task<ReturnClass.ReturnDataTable> GetwalletlistHistoryByUser(long userId)
        {
            string query = @"SELECT e.patientRegNo,e.walletAmount,e.walletReleasedAmount,e.walletBalanceAmount,
                                    e.transactionNo,e.Remark,DATE_FORMAT(e.entryDateTime,'%d/%m/%Y') AS transactionDate
                                    FROM  ewallettransaction e
                                    WHERE e.userId=@userId ORDER BY e.actionId DESC ;";

            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = userId });
            ReturnClass.ReturnDataTable dataTable = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dataTable == null || dataTable.table.Rows.Count == 0)
            {
                dataTable.message = "Wallet History Not available.";
                dataTable.status = false;
            }

            return dataTable;
        }
        public async Task<ReturnClass.ReturnBool> UpdateWalletPaymentStatus(BlAddWallet blAppointment)
        {

            ReturnClass.ReturnBool rb = new ReturnClass.ReturnBool();
            string query = "";
            bool isfirstRecharge = false;
            if (blAppointment.patientRegNo == null || blAppointment.patientRegNo == 0)
            {
                rb.status = false;
                rb.message = "Invalid Patient";
                return rb;
            }
            if (blAppointment.transactionNo == null || blAppointment.transactionNo == 0)
            {
                rb.status = false;
                rb.message = "Invalid Transaction Details";
                return rb;
            }
            if (blAppointment.paymentStatus == null || blAppointment.paymentStatus == 0)
            {
                rb.status = false;
                rb.message = "Invalid Payment Status";
                return rb;
            }
            bool transactionValid = await ISPaymentTransactionPending((long)blAppointment.transactionNo);
            if (!transactionValid)
            {
                rb.status = false;
                rb.message = "Duplicate Transaction Details";
                return rb;
            }
            if (blAppointment.paymentStatus == null || blAppointment.paymentStatus == 0)
            {
                rb.status = false;
                rb.message = "Invalid Payment Status";
                return rb;
            }
            List<MySqlParameter> pm1 = new();
            pm1.Add(new MySqlParameter("patientRegNo", MySqlDbType.Int64) { Value = blAppointment.patientRegNo });
            query = @" SELECT IFNULL(e.walletAmount,0) AS walletAmount,IFNULL(e.walletBalanceAmount ,0) AS walletBalanceAmount
                                    FROM ewalletmaster e
                                    WHERE e.patientRegNo=@patientRegNo;";
            dt = await db.ExecuteSelectQueryAsync(query, pm1.ToArray());
            if (dt != null)
            {
                if (dt.table.Rows.Count > 0)
                {
                    //walletAmount = Convert.ToDecimal(dt.table.Rows[0]["walletAmount"]);
                    //walletBalanceAmount = Convert.ToDecimal(dt.table.Rows[0]["walletBalanceAmount"]);
                }
                else
                    isfirstRecharge = true;
            }
            else
                isfirstRecharge = true;
            if (blAppointment.paymentStatus == (Int16)PaymentStatus.TransactionSuccessful)
                blAppointment.paymentStatusName = "Transaction Successful";
            else if (blAppointment.paymentStatus == (Int16)PaymentStatus.TransactionFailed)
                blAppointment.paymentStatusName = "Transaction Failed";
            else
            {
                rb.status = false;
                rb.message = "Invalid Payment Status";
                return rb;
            }
            blAppointment.actionId = await GetewalletActionId((long)blAppointment.patientRegNo);
            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("patientRegNo", MySqlDbType.Int64) { Value = blAppointment.patientRegNo });
            pm.Add(new MySqlParameter("walletAmount", MySqlDbType.Decimal) { Value = blAppointment.walletAmount });
            pm.Add(new MySqlParameter("actionId", MySqlDbType.Int32) { Value = blAppointment.actionId });
            pm.Add(new MySqlParameter("Remark", MySqlDbType.VarChar) { Value = blAppointment.Remark });
            pm.Add(new MySqlParameter("transactionNo", MySqlDbType.VarChar) { Value = blAppointment.transactionNo });
            pm.Add(new MySqlParameter("clientIp", MySqlDbType.VarString) { Value = blAppointment.clientIp });
            pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = blAppointment.userId });
            pm.Add(new MySqlParameter("entryDateTime", MySqlDbType.DateTime) { Value = DateTime.Now });
            pm.Add(new MySqlParameter("paymentStatus", MySqlDbType.Int32) { Value = blAppointment.paymentStatus });
            pm.Add(new MySqlParameter("pendingStatus", MySqlDbType.Int32) { Value = (Int16)PaymentStatus.TransactionPending });
            pm.Add(new MySqlParameter("paymentStatusName", MySqlDbType.VarChar) { Value = blAppointment.paymentStatusName });
            pm.Add(new MySqlParameter("razorpay_order_id", MySqlDbType.VarChar) { Value = blAppointment.razorPaytransactionNo });
            pm.Add(new MySqlParameter("razorpay_payment_id", MySqlDbType.VarChar) { Value = blAppointment.razorpay_payment_id });
            pm.Add(new MySqlParameter("razorpay_signature", MySqlDbType.VarChar) { Value = blAppointment.razorpay_signature });
            
            using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                query = @" UPDATE ewalletrequest 
                                    SET paymentStatus=@paymentStatus,paymentStatusName=@paymentStatusName,
                                        bankRemark=@Remark,razorpay_payment_id=@razorpay_payment_id,
                                        razorpay_signature=@razorpay_signature 
                                        WHERE patientRegNo=patientRegNo AND	transactionNo=@transactionNo 
                                AND razorPayTransactionNo=@razorpay_order_id AND paymentStatus=@pendingStatus;";
                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "Updateewalletrequest");
                if (rb.status && blAppointment.paymentStatus == (Int16)PaymentStatus.TransactionSuccessful)
                {
                    if (isfirstRecharge)
                        query = @" INSERT INTO ewalletmaster (patientRegNo,walletAmount,walletBalanceAmount,actionId,
									remark,entryDateTime,userId,clientIp)
 									VALUES
 									(@patientRegNo,@walletAmount,@walletAmount,@actionId,
					@Remark,NOW(),@userId,@clientIp);";
                    else
                        query = @" UPDATE ewalletmaster SET  walletAmount=walletAmount+@walletAmount,
                                    walletBalanceAmount=walletBalanceAmount+@walletAmount,
                                    actionId=@actionId,remark=@Remark,entryDateTime=NOW(),userId=@userId,clientIp=@clientIp
 									 WHERE patientRegNo=@patientRegNo;";
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "Insertewalletmaster");
                    if (rb.status)
                    {
                        query = @" INSERT INTO ewallettransaction(patientRegNo,actionId,walletAmount,Remark,
										transactionNo,entryDateTime,userId,clientIp) 
											VALUES
											(@patientRegNo,@actionId,@walletAmount,@Remark,
										@transactionNo,@entryDateTime,@userId,@clientIp);";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "Insertewallettransaction");
                    }
                }
                if (rb.status)
                {
                    ts.Complete();
                    rb.message = blAppointment.paymentStatusName;

                }
                else
                    rb.message = "Please try after some time!";

            }
            return rb;
        }

        public async Task<ReturnClass.ReturnDataTable> GetPatientProfile(long userId)
        {
            string query = @"SELECT  p.patientRegNo,p.patientNameEnglish,p.patientNameLocal,p.mobileNo,p.emailId,
                                 p.genderId,d.nameEnglish AS genderName,p.pincode,p.patientId 
                                FROM patientregistration p 
                                JOIN ddlcatlist d ON d.id=p.genderId AND d.category=@gender
                                WHERE p.patientRegNo=@userId 
                                AND p.active=@active AND p.isVerified=@isVerified;";

            List<MySqlParameter> pm = new();
            pm.Add(new MySqlParameter("userId", MySqlDbType.Int64) { Value = userId });
            pm.Add(new MySqlParameter("active", MySqlDbType.Int16) { Value = (Int16)Active.Yes });
            pm.Add(new MySqlParameter("isVerified", MySqlDbType.Int16) { Value = (Int16)Active.Yes });
            pm.Add(new MySqlParameter("gender", MySqlDbType.VarChar) { Value = "gender" });
            ReturnClass.ReturnDataTable dataTable = await db.ExecuteSelectQueryAsync(query, pm.ToArray());
            if (dataTable == null || dataTable.table.Rows.Count == 0)
            {
                dataTable.message = "Invalid Patient Details";
                dataTable.status = false;
            }

            return dataTable;
        }

    }
}
