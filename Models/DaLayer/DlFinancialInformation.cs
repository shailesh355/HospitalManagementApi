using BaseClass;
using HospitalManagementApi.Models.BLayer;
using MySql.Data.MySqlClient;
using static HospitalManagementApi.Models.BLayer.BlCommon;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlFinancialInformation
    {
        readonly DBConnection db = new();
        ReturnClass.ReturnDataTable dt = new();

        public async Task<ReturnClass.ReturnBool> CUDOperation(BlFinancialInformation bl)
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
                        new MySqlParameter("financialInformationId", MySqlDbType.Int32) { Value = item.financialInformationId },
                        new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo },
                        new MySqlParameter("accountNumber", MySqlDbType.VarChar,20) { Value = item.accountNumber },
                        new MySqlParameter("beneficiaryName", MySqlDbType.VarChar, 100) { Value = item.beneficiaryName },
                        new MySqlParameter("accountTypeId", MySqlDbType.Int16) { Value = item.accountTypeId },
                        new MySqlParameter("accountTypeName", MySqlDbType.VarChar, 50) { Value = item.accountTypeName },
                        new MySqlParameter("bankName", MySqlDbType.VarChar, 100) { Value = item.accountTypeName },
                        new MySqlParameter("bankAddress", MySqlDbType.VarChar, 200) { Value = item.bankAddress },
                        new MySqlParameter("IFSCCode", MySqlDbType.VarChar, 11) { Value = item.IFSCCode },
                        new MySqlParameter("PANNo", MySqlDbType.VarChar, 10) { Value = item.PANNo },
                        new MySqlParameter("nameOnPAN", MySqlDbType.VarChar, 100) { Value = item.nameOnPAN },
                        new MySqlParameter("TDSExemptionPercent", MySqlDbType.Int16) { Value = item.TDSExemptionPercent },
                        new MySqlParameter("TDSExemptionLimit", MySqlDbType.Int16) { Value = item.TDSExemptionLimit },
                        new MySqlParameter("TDSExemptionPeriod", MySqlDbType.Int16) { Value = item.TDSExemptionPeriod },
                        new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                        new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime },
                    };
                    if (bl.CRUD == (Int16)CRUD.Create)
                    {
                        query = @"INSERT INTO financialinformation (hospitalRegNo,accountNumber,beneficiaryName,accountTypeId,accountTypeName,bankName,bankAddress,IFSCCode,
                                                                PANNo,nameOnPAN,TDSExemptionPercent,TDSExemptionLimit,TDSExemptionPeriod,entryDateTime,userId)
                                        VALUES (@hospitalRegNo,@accountNumber,@beneficiaryName,@accountTypeId,@accountTypeName,@bankName,@bankAddress,@IFSCCode,
                                                                @PANNo,@nameOnPAN,@TDSExemptionPercent,@TDSExemptionLimit,@TDSExemptionPeriod,@entryDateTime,@userId)";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "financialinformation");
                    }
                    else if (bl.CRUD == (Int16)CRUD.Update)
                    {
                        query = @"UPDATE financialinformation 
                                        SET accountNumber = @accountNumber,beneficiaryName = @beneficiaryName,accountTypeId=@accountTypeId,accountTypeName=@accountTypeName,
                                            bankName=@bankName,bankAddress=@bankAddress,IFSCCode=@IFSCCode,PANNo=@PANNo,nameOnPAN=@nameOnPAN,TDSExemptionPercent=@TDSExemptionPercent,
                                            TDSExemptionLimit=@TDSExemptionLimit,TDSExemptionPeriod=@TDSExemptionPeriod
                                WHERE financialInformationId = @financialInformationId";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "financialinformation");
                    }
                }
            }
            return rb;
        }

        public async Task<ReturnClass.ReturnDataTable> GetFinancialDetail(Int64 hospitalRegNo)
        {
            string query = string.Empty;
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                         new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                   };
                query = @"SELECT fi.financialInformationId,fi.hospitalRegNo,fi.accountNumber,fi.beneficiaryName,fi.accountTypeId,fi.accountTypeName,
		                            fi.bankName,fi.bankAddress,fi.IFSCCode,fi.PANNo,fi.nameOnPAN,fi.TDSExemptionPercent,fi.TDSExemptionLimit,fi.TDSExemptionPeriod	
		                    FROM financialinformation AS fi
			                WHERE fi.hospitalRegNo=@hospitalRegNo";
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
    }
}
