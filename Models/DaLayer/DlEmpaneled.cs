using BaseClass;
using HospitalManagementApi.Models.BLayer;
using MySql.Data.MySqlClient;
using System.Transactions;
using static HospitalManagementApi.Models.BLayer.BlCommon;

namespace HospitalManagementApi.Models.DaLayer
{
    public class DlEmpaneled
    {
        readonly DBConnection db = new();
        ReturnClass.ReturnDataTable dt = new();
        public async Task<ReturnClass.ReturnBool> CUDOperation(BlEmpaneled bl)
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
                    query = @"SELECT hs.empaneledId
		                        FROM empaneled AS hs
		                    WHERE hs.hospitalRegNo=@hospitalRegNo ";
                    dt = await db.ExecuteSelectQueryAsync(query, pmInner);
                    if (dt.table.Rows.Count > 0)
                    {
                        message = "Data updated successfully.";
                        query = @"INSERT INTO empaneledlog
                                    SELECT * FROM empaneled
                                WHERE hospitalRegNo = @hospitalRegNo";
                        rb = await db.ExecuteQueryAsync(query, pmInner, "empaneledlog");
                        if (rb.status)
                        {
                            query = @"DELETE FROM empaneled 
                                WHERE hospitalRegNo = @hospitalRegNo";
                            rb = await db.ExecuteQueryAsync(query, pmInner, "empaneled");
                        }
                    }
                    query = @"INSERT INTO empaneled (hospitalRegNo,empaneledId,headName,empaneledTypeId,empaneledTypeName,clientIp,userId,entryDateTime)
                                    VALUES ";
                    List<MySqlParameter> pm = new();
                    for (int i = 0; i < bl.Bl!.Count; i++)
                    {
                        pm.Add(new MySqlParameter("hospitalRegNo" + (i).ToString(), MySqlDbType.Int64) { Value = bl.hospitalRegNo });
                        pm.Add(new MySqlParameter("empaneledId" + (i).ToString(), MySqlDbType.Int32) { Value = bl.Bl[i].empaneledId });
                        pm.Add(new MySqlParameter("headName" + (i).ToString(), MySqlDbType.VarChar) { Value = bl.Bl[i].headName });
                        pm.Add(new MySqlParameter("empaneledTypeId" + (i).ToString(), MySqlDbType.Int16) { Value = bl.Bl[i].empaneledTypeId });
                        pm.Add(new MySqlParameter("empaneledTypeName" + (i).ToString(), MySqlDbType.VarChar) { Value = bl.Bl[i].empaneledTypeName });
                        pm.Add(new MySqlParameter("clientIp" + (i).ToString(), MySqlDbType.VarChar) { Value = bl.clientIp });
                        pm.Add(new MySqlParameter("userId" + (i).ToString(), MySqlDbType.Int64) { Value = bl.userId });
                        pm.Add(new MySqlParameter("entryDateTime" + (i).ToString(), MySqlDbType.String) { Value = bl.entryDateTime });

                        query += "(@hospitalRegNo" + i.ToString() + ", @empaneledId" + i.ToString() + ", @headName" + i.ToString() + ", @empaneledTypeId" + i.ToString() + ", @empaneledTypeName" + i.ToString() +
                                  ", @clientIp" + i.ToString() + ", @userId" + i.ToString() + ",@entryDateTime" + i.ToString() + ") ,";
                    }
                    query = query.TrimEnd(',');
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "CUDOperation(empaneled)");
                    if (rb.status)
                    {
                        transaction.Complete();
                        rb.message = message;
                    }
                }
            }
            return rb;
        }

        public async Task<ReturnClass.ReturnDataTable> GetEmpaneledDetail(Int64 hospitalRegNo, Int16 empaneledTypeId)
        {
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                         new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                         new MySqlParameter("empaneledTypeId", MySqlDbType.Int16) { Value = empaneledTypeId },
                   };
                string qr = @"SELECT em.hospitalRegNo,em.empaneledTypeId,em.empaneledTypeName,em.empaneledId,em.headName
	                                  FROM empaneled AS em 
	                                  WHERE em.empaneledTypeId=@empaneledTypeId AND em.hospitalRegNo=@hospitalRegNo   
			                        ORDER BY em.headName";
                dt = await db.ExecuteSelectQueryAsync(qr, pm);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public async Task<ReturnClass.ReturnBool> CUDDoctorOperation(BlDoctorEmpaneled bl)
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
                query = @"DELETE FROM doctorempaneled 
                                WHERE doctorRegNo = @doctorRegNo";
                pm = new MySqlParameter[]
                   {
                        new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                   };
                rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "empaneled");

                foreach (var item in bl.Bl)
                {
                    pm = new MySqlParameter[]
                    {
                        new MySqlParameter("rowId", MySqlDbType.Int64) { Value = item.rowId },
                        new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = bl.doctorRegNo },
                        new MySqlParameter("empaneledId", MySqlDbType.Int32) { Value = item.empaneledId },
                        new MySqlParameter("empaneledTypeId", MySqlDbType.Int16) { Value = item.empaneledTypeId },
                        new MySqlParameter("empaneledTypeName", MySqlDbType.VarChar,50) { Value = item.empaneledTypeName},
                        new MySqlParameter("headName", MySqlDbType.VarChar,1000) { Value = item.headName},
                        new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                        new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime }
                    };
                    if (bl.CRUD == (Int16)CRUD.Create)
                    {
                        query = @"INSERT INTO doctorempaneled (doctorRegNo,empaneledId,empaneledTypeId,empaneledTypeName,headName,entryDateTime,userId)
                                        VALUES (@doctorRegNo,@empaneledId,@empaneledTypeId,@empaneledTypeName,@headName,@entryDateTime,@userId)";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "empaneled");
                    }
                    else if (bl.CRUD == (Int16)CRUD.Update)
                    {
                        query = @"DELETE FROM doctorempaneled 
                                WHERE doctorRegNo = @doctorRegNo";
                        rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "empaneled");
                        if (rb.status)
                        {
                            query = @"INSERT INTO doctorempaneled (doctorRegNo,empaneledId,empaneledTypeId,empaneledTypeName,headName,entryDateTime,userId)
                                        VALUES (@doctorRegNo,@empaneledId,@empaneledTypeId,@empaneledTypeName,@headName,@entryDateTime,@userId)";
                            rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "empaneled");
                        }
                    }
                }
            }
            return rb;
        }

        public async Task<ReturnClass.ReturnDataTable> GetDoctorEmpaneledDetail(Int64 doctorRegNo, Int16 empaneledTypeId)
        {
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                         new MySqlParameter("doctorRegNo", MySqlDbType.Int64) { Value = doctorRegNo },
                         new MySqlParameter("empaneledTypeId", MySqlDbType.Int16) { Value = empaneledTypeId },
                   };
                string qr = @"SELECT em.rowId,em.empaneledId,em.doctorRegNo,em.empaneledTypeId,em.empaneledTypeName,em.headName
	                                  FROM doctorempaneled AS em 
	                                  WHERE em.empaneledTypeId=@empaneledTypeId AND em.doctorRegNo=@doctorRegNo   
			                        ORDER BY em.headName";
                dt = await db.ExecuteSelectQueryAsync(qr, pm);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }
        public async Task<ReturnClass.ReturnDataTable> GetProviderEmpaneledDetail(Int64 hospitalRegNo, Int16 empaneledTypeId, Int32 empaneledId)
        {
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                         new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                         new MySqlParameter("empaneledTypeId", MySqlDbType.Int16) { Value = empaneledTypeId },
                         new MySqlParameter("empaneledId", MySqlDbType.Int32) { Value = empaneledId },
                   };
                string qr = @"SELECT ep.providerName,DATE_FORMAT(ep.fromDate,'%d/%m/%Y') AS fromDate,DATE_FORMAT(ep.toDate,'%d/%m/%Y') AS toDate
	                                 FROM empaneledprovider AS ep
	                                WHERE ep.empaneledTypeId=@empaneledTypeId AND ep.empaneledId=@empaneledId
		                                AND ep.hospitalRegNo=@hospitalRegNo";
                dt = await db.ExecuteSelectQueryAsync(qr, pm);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public async Task<ReturnClass.ReturnBool> CUDProviderEmpaneled(BlProviderEmpaneled bl)
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
                    DateTime fromDate = DateTime.ParseExact(bl.fromDate.Replace('-', '/'), "dd/MM/yyyy", null);
                    bl.fromDate = fromDate.ToString("yyyy/MM/dd");

                    DateTime toDate = DateTime.ParseExact(bl.toDate.Replace('-', '/'), "dd/MM/yyyy", null);
                    bl.toDate = toDate.ToString("yyyy/MM/dd");

                    pmInner = new MySqlParameter[]
                    {
                        new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = bl.hospitalRegNo },
                        new MySqlParameter("empaneledTypeId", MySqlDbType.Int16) { Value = bl.empaneledTypeId },
                        new MySqlParameter("empaneledTypeName", MySqlDbType.VarChar,100) { Value = bl.empaneledTypeName},
                        new MySqlParameter("empaneledId", MySqlDbType.Int32) { Value = bl.empaneledId },
                        new MySqlParameter("empaneledName", MySqlDbType.VarChar,100) { Value = bl.empaneledName},
                        new MySqlParameter("providerName", MySqlDbType.VarChar,100) { Value = bl.providerName},
                        new MySqlParameter("fromDate", MySqlDbType.VarChar,100) { Value = bl.fromDate},
                        new MySqlParameter("toDate", MySqlDbType.VarChar,100) { Value = bl.toDate},
                        new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId },
                        new MySqlParameter("entryDateTime", MySqlDbType.String) { Value = bl.entryDateTime }
                    };
                    query = @"SELECT ep.empaneledProviderId
		                        FROM empaneledprovider AS ep
		                    WHERE ep.hospitalRegNo=@hospitalRegNo AND ep.empaneledTypeId=@empaneledTypeId AND ep.empaneledId=@empaneledId ";
                    dt = await db.ExecuteSelectQueryAsync(query, pmInner);
                    if (dt.table.Rows.Count > 0)
                    {
                        message = "Data updated successfully.";
                        query = @"INSERT INTO empaneledproviderlog
                                    SELECT * FROM empaneledprovider
                                WHERE hospitalRegNo = @hospitalRegNo AND empaneledTypeId=@empaneledTypeId AND empaneledId=@empaneledId";
                        rb = await db.ExecuteQueryAsync(query, pmInner, "hempaneledproviderlog");
                        if (rb.status)
                        {
                            query = @"DELETE FROM empaneledprovider 
                                WHERE hospitalRegNo = @hospitalRegNo AND empaneledTypeId=@empaneledTypeId AND empaneledId=@empaneledId ";
                            rb = await db.ExecuteQueryAsync(query, pmInner, "empaneledprovider");
                        }
                    }

                    query = @"INSERT INTO empaneledprovider (hospitalRegNo,empaneledTypeId,empaneledTypeName,empaneledId,empaneledName,providerName,fromDate,toDate,entryDateTime,userId,clientIp)
                                        VALUES (@hospitalRegNo,@empaneledTypeId,@empaneledTypeName,@empaneledId,@empaneledName,@providerName,@fromDate,@toDate,@entryDateTime,@userId,@clientIp)";
                    rb = await db.ExecuteQueryAsync(query, pmInner, "empaneledprovider");
                    if (rb.status)
                    {
                        transaction.Complete();
                        rb.message = message;
                    }

                }
            }
            return rb;
        }

        public async Task<ReturnClass.ReturnBool> CUDEmpInsOperation(BlEmpaneledIns bl)
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
                    query = @"SELECT hs.empaneledId
		                        FROM empaneled AS hs
		                    WHERE hs.hospitalRegNo=@hospitalRegNo ";
                    dt = await db.ExecuteSelectQueryAsync(query, pmInner);
                    if (dt.table.Rows.Count > 0)
                    {
                        message = "Data updated successfully.";
                        query = @"INSERT INTO empaneledlog
                                    SELECT * FROM empaneled
                                WHERE hospitalRegNo = @hospitalRegNo";
                        rb = await db.ExecuteQueryAsync(query, pmInner, "empaneledlog");
                        if (rb.status)
                        {
                            query = @"DELETE FROM empaneled 
                                WHERE hospitalRegNo = @hospitalRegNo";
                            rb = await db.ExecuteQueryAsync(query, pmInner, "empaneled");
                        }
                    }
                    query = @"INSERT INTO empaneled (hospitalRegNo,empaneledId,headName,empaneledTypeId,empaneledTypeName,clientIp,userId,entryDateTime)
                                    VALUES ";
                    List<MySqlParameter> pm = new();
                    for (int i = 0; i < bl.blEmpaneledItemsIns!.Count; i++)
                    {
                        pm.Add(new MySqlParameter("hospitalRegNo" + (i).ToString(), MySqlDbType.Int64) { Value = bl.hospitalRegNo });
                        pm.Add(new MySqlParameter("empaneledId" + (i).ToString(), MySqlDbType.Int32) { Value = bl.blEmpaneledItemsIns[i].empaneledId });
                        pm.Add(new MySqlParameter("headName" + (i).ToString(), MySqlDbType.VarChar) { Value = bl.blEmpaneledItemsIns[i].headName });
                        pm.Add(new MySqlParameter("empaneledTypeId" + (i).ToString(), MySqlDbType.Int16) { Value = bl.blEmpaneledItemsIns[i].empaneledTypeId });
                        pm.Add(new MySqlParameter("empaneledTypeName" + (i).ToString(), MySqlDbType.VarChar) { Value = bl.blEmpaneledItemsIns[i].empaneledTypeName });
                        pm.Add(new MySqlParameter("clientIp" + (i).ToString(), MySqlDbType.VarChar) { Value = bl.clientIp });
                        pm.Add(new MySqlParameter("userId" + (i).ToString(), MySqlDbType.Int64) { Value = bl.userId });
                        pm.Add(new MySqlParameter("entryDateTime" + (i).ToString(), MySqlDbType.String) { Value = bl.entryDateTime });

                        query += "(@hospitalRegNo" + i.ToString() + ", @empaneledId" + i.ToString() + ", @headName" + i.ToString() + ", @empaneledTypeId" + i.ToString() + ", @empaneledTypeName" + i.ToString() +
                                  ", @clientIp" + i.ToString() + ", @userId" + i.ToString() + ",@entryDateTime" + i.ToString() + ") ,";
                    }
                    query = query.TrimEnd(',');
                    rb = await db.ExecuteQueryAsync(query, pm.ToArray(), "CUDOperation(empaneled)");
                    if (rb.status)
                    {
                        query = @"INSERT INTO empaneledproviderlog
                                    SELECT * FROM empaneledprovider
                                WHERE hospitalRegNo = @hospitalRegNo ";
                        rb = await db.ExecuteQueryAsync(query, pmInner, "hempaneledproviderlog");
                        if (rb.status)
                        {
                            query = @"DELETE FROM empaneledprovider 
                                    WHERE hospitalRegNo = @hospitalRegNo ";
                            rb = await db.ExecuteQueryAsync(query, pmInner, "empaneledprovider");
                        }
                        if (rb.status)
                        {
                            BlEmpaneledItemsIns blItemIns = new();
                            List<MySqlParameter> pmInnerIns = new();
                            query = @"INSERT INTO empaneledprovider(hospitalRegNo,empaneledTypeId,empaneledTypeName,empaneledId,empaneledName,providerName,fromDate,toDate,clientIp,userId,entryDateTime)
                                        VALUES ";

                            for (int i = 0; i < bl.blProviderItemsIns!.Count; i++)
                            {
                                DateTime fromDate = DateTime.ParseExact(bl.blProviderItemsIns![i].fromDate!.Replace('-', '/'), "dd/MM/yyyy", null);
                                bl.blProviderItemsIns![i].fromDate = fromDate.ToString("yyyy/MM/dd");

                                DateTime toDate = DateTime.ParseExact(bl.blProviderItemsIns![i].toDate!.Replace('-', '/'), "dd/MM/yyyy", null);
                                bl.blProviderItemsIns![i].toDate = toDate.ToString("yyyy/MM/dd");

                                pmInnerIns.Add(new MySqlParameter("hospitalRegNo" + (i).ToString(), MySqlDbType.Int64) { Value = bl.hospitalRegNo });
                                pmInnerIns.Add(new MySqlParameter("empaneledTypeId" + (i).ToString(), MySqlDbType.Int16) { Value = bl.blProviderItemsIns![i].empaneledTypeId });
                                pmInnerIns.Add(new MySqlParameter("empaneledTypeName" + (i).ToString(), MySqlDbType.VarChar) { Value = bl.blProviderItemsIns![i].empaneledTypeName });
                                pmInnerIns.Add(new MySqlParameter("empaneledId" + (i).ToString(), MySqlDbType.Int32) { Value = bl.blProviderItemsIns![i].empaneledId });
                                pmInnerIns.Add(new MySqlParameter("empaneledName" + (i).ToString(), MySqlDbType.VarChar) { Value = bl.blProviderItemsIns![i].empaneledName });
                                pmInnerIns.Add(new MySqlParameter("providerName" + (i).ToString(), MySqlDbType.VarChar) { Value = bl.blProviderItemsIns![i].providerName });
                                pmInnerIns.Add(new MySqlParameter("fromDate" + (i).ToString(), MySqlDbType.VarChar) { Value = bl.blProviderItemsIns![i].fromDate });
                                pmInnerIns.Add(new MySqlParameter("toDate" + (i).ToString(), MySqlDbType.VarChar) { Value = bl.blProviderItemsIns![i].toDate });
                                pmInnerIns.Add(new MySqlParameter("clientIp" + (i).ToString(), MySqlDbType.VarChar) { Value = bl.clientIp });
                                pmInnerIns.Add(new MySqlParameter("userId" + (i).ToString(), MySqlDbType.Int64) { Value = bl.userId });
                                pmInnerIns.Add(new MySqlParameter("entryDateTime" + (i).ToString(), MySqlDbType.String) { Value = bl.entryDateTime });

                                query += "(@hospitalRegNo" + i.ToString() + ", @empaneledTypeId" + i.ToString() + ", @empaneledTypeName" + i.ToString() + ", @empaneledId" + i.ToString() +
                                            ", @empaneledName" + i.ToString() + ", @providerName" + i.ToString() + ", @fromDate" + i.ToString() + ", @toDate" + i.ToString() +
                                          ", @clientIp" + i.ToString() + ", @userId" + i.ToString() + ",@entryDateTime" + i.ToString() + ") ,";
                            }
                            query = query.TrimEnd(',');
                            rb = await db.ExecuteQueryAsync(query, pmInnerIns.ToArray(), "CUDOperation(empaneledprovider)");
                        }
                        if (rb.status)
                        {
                            transaction.Complete();
                            rb.message = message;
                        }
                    }
                }
            }
            return rb;
        }

        public async Task<ReturnClass.ReturnDataSet> GetEmpaneledProviderDetail(Int64 hospitalRegNo, Int16 empaneledTypeId)
        {
            string query = string.Empty;
            ReturnClass.ReturnDataSet dataSet = new();
            ReturnClass.ReturnDataTable dtt = new();
            try
            {
                MySqlParameter[] pm = new MySqlParameter[]
                   {
                         new MySqlParameter("hospitalRegNo", MySqlDbType.Int64) { Value = hospitalRegNo },
                         new MySqlParameter("empaneledTypeId", MySqlDbType.Int16) { Value = empaneledTypeId },
                   };
                query = @"SELECT em.empaneledId,em.hospitalRegNo,em.empaneledTypeId,em.empaneledTypeName,em.headName
	                                  FROM empaneled AS em 
	                                  WHERE em.empaneledTypeId=@empaneledTypeId AND em.hospitalRegNo=@hospitalRegNo   
			                        ORDER BY em.headName";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "empaneled";
                dataSet.dataset.Tables.Add(dtt.table);

                // empaneled provider
                query = @"SELECT ep.empaneledTypeId,ep.empaneledTypeName,ep.empaneledId,ep.empaneledName,ep.providerName,DATE_FORMAT(ep.fromDate,'%d/%m/%Y') AS fromDate,DATE_FORMAT(ep.toDate,'%d/%m/%Y') AS toDate
	                                 FROM empaneledprovider AS ep
	                                WHERE ep.empaneledTypeId=@empaneledTypeId
		                                AND ep.hospitalRegNo=@hospitalRegNo";
                dtt = await db.ExecuteSelectQueryAsync(query, pm);
                dtt.table.TableName = "empaneledprovider";
                dataSet.dataset.Tables.Add(dtt.table);

            }
            catch (Exception ex)
            {
            }
            return dataSet;
        }
    }
}
