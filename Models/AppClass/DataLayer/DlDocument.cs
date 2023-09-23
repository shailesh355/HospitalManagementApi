using HospitalStoreAPI.Models.AppClass.BLayer;
using MySql.Data.MySqlClient;
using System.Transactions;
using static ReturnClass;

namespace HospitalStoreAPI.Models.AppClass.DataLayer
{
    public class DlDocument
    {
        ReturnBool rb = new();
        ReturnDataTable dt = new();
        DlCommon dl = new();
        DBConnection db = new();

        ///// <summary>
        ///// Save work document related documents
        ///// </summary>
        ///// <param name="bl"></param>
        ///// <returns>ReturnBool</returns>
        //public async Task<ReturnBool> SaveDocumentsAsync(BlDocument bl)
        //{
        //    //BlNoticeCircularDocuments docno = new BlNoticeCircularDocuments();
        //    BlDocumentImagesModel bdc = await dl.GetDocumentImagesPath_Async(bl.stateId, bl.documentType, bl.documentImageGroup);

        //    string year = bdc.addYear ? DateTime.Now.Year.ToString() + @"\" : "";
        //    string addFolder = bdc.createFolder ? bl.documentId + @"\" : "";
        //    string errorMsg = "";
        //    bool allowSave = true;

        //    string query = @"SELECT documentId
        //                        FROM documentstore AS ds
        //                    WHERE ds.documentId = @documentId AND ds.documentNumber = @documentNumber ";
        //    MySqlParameter[] pm1 = new MySqlParameter[]
        //    {
        //        new MySqlParameter("documentId", MySqlDbType.Int64) { Value = bl.documentId},
        //        new MySqlParameter("documentNumber", MySqlDbType.Int16) { Value = bl.documentNumber},
        //    };
        //    dt = await db.ExecuteSelectQueryAsync(query, pm1);
        //    if (dt.table.Rows.Count > 0)
        //    {
        //        query = @"DELETE
        //                        FROM documentstore AS ds
        //                    WHERE ds.documentId = @documentId AND ds.documentNumber = @documentNumber ";
        //        rb = await db.ExecuteDeleteQueryAsync(query, pm1);
        //    }

        //    query = @"INSERT INTO documentstore(documentId, documentNumber, dptTableId, amendmentNo,
        //                                 documentName, documentExtension, documentMimeType, userId, clientIp)
        //                     VALUES (@documentId, @documentNumber, @dptTableId, @amendmentNo, 
        //                                 @documentName, @documentExtension, @documentMimeType, @userId, @clientIp)";
        //    try
        //    {
        //        if (bdc.status)
        //        {
        //            //get category wise file count for file name generation
        //            BlDocumentImagesModel docno = await GetDocumentCount(bl);
        //            if (docno.status)
        //            {
        //                string storage_path = bdc.physcialPath.Replace("D:", "C:") + year + addFolder;
        //                bool exists = Directory.Exists(storage_path);
        //                if (!exists)
        //                    Directory.CreateDirectory(storage_path);

        //                bl.documentNumber = docno.fileCount;
        //                if (bl.files.Count > 0)
        //                {
        //                    foreach (var file in bl.files)
        //                    {
        //                        if (file.Length > bdc.maxFileSizeAllowed)
        //                        {
        //                            errorMsg = "File exceeds the allowed limit";
        //                            allowSave = false;
        //                            break;
        //                        }

        //                        if (bdc.fileType.Contains(bl.documentMimeType) == false)
        //                        {
        //                            errorMsg = "File has a different Mime type";
        //                            allowSave = false;
        //                            break;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    errorMsg = "No files provided";
        //                    allowSave = false;
        //                }

        //                if (allowSave)
        //                {
        //                    foreach (var file in bl.files)
        //                    {
        //                        if (file.Length > 0)
        //                        {
        //                            bl.documentMimeType = file.ContentType.ToLower();
        //                            bl.documentExtension = Path.GetExtension(file.FileName).ToString();
        //                            bl.documentName = bl.documentId + "_" + bl.documentNumber.ToString();

        //                            using (var stream = new FileStream(storage_path + @"\" + bl.documentId + "_" + bl.documentNumber.ToString()
        //                                + bl.documentExtension, FileMode.CreateNew))
        //                            {
        //                                try
        //                                {
        //                                    using (MemoryStream fs = new MemoryStream())
        //                                    {
        //                                        MySqlParameter[] pm = new MySqlParameter[]
        //                                        {
        //                                            new MySqlParameter("documentId", MySqlDbType.Int64) { Value = bl.documentId},
        //                                            new MySqlParameter("documentNumber", MySqlDbType.Int16) { Value = bl.documentNumber},
        //                                            new MySqlParameter("dptTableId", MySqlDbType.Int16) { Value = bdc.dptTableId},
        //                                            new MySqlParameter("amendmentNo", MySqlDbType.Int16) { Value = bl.amendmentNo},
        //                                            new MySqlParameter("documentName", MySqlDbType.String) { Value = bl.documentName},
        //                                            new MySqlParameter("documentExtension", MySqlDbType.String) { Value = bl.documentExtension},
        //                                            new MySqlParameter("documentMimeType", MySqlDbType.String) { Value = bl.documentMimeType},
        //                                            new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.loginId},
        //                                            new MySqlParameter("clientIp", MySqlDbType.String) { Value = bl.clientIp}
        //                                        };

        //                                        rb = await db.ExecuteInsertQueryAsync(query, pm);
        //                                        if (rb.status)
        //                                        {
        //                                            file.CopyTo(stream);
        //                                            errorMsg = "Successfully uploaded";
        //                                        }
        //                                        else
        //                                        {
        //                                            errorMsg = "Failed to save document";
        //                                            break;
        //                                        }
        //                                    }
        //                                }
        //                                catch (Exception ex)
        //                                {
        //                                    Gen_Error_Rpt.Write_Error("DlDocument:SaveDocumentsAsync(error)", ex);
        //                                    rb.status = false;
        //                                    break;
        //                                }
        //                            }
        //                        }
        //                        bl.documentNumber++;
        //                        rb.remark = bl.documentName;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                errorMsg = "Invalid document type supplied";
        //            }
        //        }
        //        else
        //            errorMsg = "Invalid document supplied for the method";
        //    }
        //    catch (Exception ex)
        //    {
        //        errorMsg = "Something went wrong";
        //        Gen_Error_Rpt.Write_Error("DlDocument:SaveDocumentsAsync : ", ex);
        //    }
        //    rb.message = errorMsg;
        //    return rb;
        //}

        /// <summary>
        /// Returns Document count based on document ID
        /// </summary>
        /// <param name="bl"></param>
        /// <returns></returns>
        private async Task<BlDocumentImagesModel> GetDocumentCount(BlDocument bl)
        {
            BlDocumentImagesModel bdc = new BlDocumentImagesModel();
            MySqlParameter[] pm = new MySqlParameter[]
            {
                new MySqlParameter("stateId", MySqlDbType.Int16) { Value = bl.stateId},
                new MySqlParameter("documentId", MySqlDbType.Int64) { Value = bl.documentId },
                new MySqlParameter("documentImageGroup", MySqlDbType.Int16){ Value = bl.documentImageGroup }
            };
            string query = @"SELECT COUNT(w.documentId)+1 Total
                             FROM documentstore w
                             INNER JOIN documentpathtbl t ON t.dptTableId = w.dptTableId 
                             WHERE w.documentId = @documentId AND t.stateId = @stateId AND
                                   t.documentImageGroup=@documentImageGroup";

            ReturnDataTable dt = await db.ExecuteSelectQueryAsync(query, pm);
            if (dt.table.Rows.Count > 0)
            {
                bdc.fileCount = Convert.ToInt32(dt.table.Rows[0]["Total"].ToString());
                bdc.status = true;
            }
            return bdc;
        }

        /// <summary>
        /// Retrive Document details like FilePath, Mime Type etc. based on input 
        /// </summary>
        /// <param name="documentName"></param>
        /// <param name="documentType"></param>
        /// <param name="documentImageGroup"></param>
        /// <returns></returns>
        public async Task<ReturnDocumentDetail> GetDocumentAsync(string documentName, DocumentType documentType, DocumentImageGroup documentImageGroup)
        {
            ReturnDocumentDetail rs = new ReturnDocumentDetail();
            string query = @" SELECT ds.documentMimeType, dp.documentImageGroup, 
                                    CONCAT(dp.physicalPath,
                                    (CASE WHEN dp.addYear = 1 then CONCAT(ds.uploadYear, '\\') ELSE '' END),
                                    (CASE WHEN dp.addFolder = 1 then CONCAT(ds.documentId, '\\') ELSE '' END),
                                    ds.documentName, ds.documentExtension) AS filepath,ds.documentId,ds.documentNumber, dp.dptTableId
                              FROM documentstore ds
                              INNER JOIN documentpathtbl dp ON dp.dptTableId = ds.dptTableId
                              WHERE ds.documentName = @documentName AND dp.documentType = @documentType AND ds.active = @active";
            MySqlParameter[] pm = new MySqlParameter[]
            {
                new MySqlParameter("documentName", MySqlDbType.String) { Value = documentName },
                new MySqlParameter("documentType", MySqlDbType.Int16) { Value = (int) documentType },
                new MySqlParameter("active", MySqlDbType.Int16) { Value = 1 }
            };
            dt = await db.ExecuteSelectQueryAsync(query, pm);
            if (dt.table.Rows.Count > 0)
            {
                _ = Enum.TryParse(dt.table.Rows[0]["documentImageGroup"].ToString(), out DocumentImageGroup dig);
                if (documentImageGroup == dig)
                {
                    rs.filePath = dt.table.Rows[0]["filepath"].ToString();
                    rs.mimeType = dt.table.Rows[0]["documentMimeType"].ToString();
                    rs.documentNumber = Convert.ToInt16(dt.table.Rows[0]["documentNumber"]);
                    rs.dptTableId = Convert.ToInt16(dt.table.Rows[0]["dptTableId"]);
                    rs.status = true;
                }
                else
                    rs.message = "Invalid method!!!";
            }
            else
                rs.message = "document not found";
            return rs;
        }

        /// <summary>
        /// Method used for deleting documents.
        /// </summary>
        /// <param name="bl"></param>
        /// <returns></returns>
        public async Task<ReturnBool> DeleteApplicationDocumentsAsync(BlDocument bl)
        {
            bool allowDelete = false;
            rb.status = false;
            //====Check whether file exists or not
            ReturnDocumentDetail rd = await GetDocumentAsync(bl.documentName, bl.documentType, bl.documentImageGroup);
            if (rd.status)
            {
                rd.filePath = rd.filePath.Replace("D:", "C:");
                if (File.Exists(rd.filePath))
                    allowDelete = true;
            }
            //====Proceed for delete only if physical file exists====
            if (allowDelete)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    string query = @"INSERT INTO documentstorelog 
                                         SELECT *, CURRENT_TIMESTAMP(), @userId AS userId, @clientIp AS ClientIp 
                                            FROM documentstore d
                                         WHERE d.documentName = @documentName";
                    MySqlParameter[] pm1 = new MySqlParameter[]
                    {
                        new MySqlParameter("documentId",MySqlDbType.Int64) { Value = bl.documentId},
                        new MySqlParameter("dptTableId",MySqlDbType.String) { Value = rd.dptTableId},
                        new MySqlParameter("documentName",MySqlDbType.String) { Value = bl.documentName},
                        new MySqlParameter("userId",MySqlDbType.Int64) { Value = bl.loginId},
                        new MySqlParameter("clientIp",MySqlDbType.String) { Value = bl.clientIp},
                        new MySqlParameter("documentNumber",MySqlDbType.Int16) { Value = rd.documentNumber},
                    };
                    rb = await db.ExecuteInsertQueryAsync(query, pm1);
                    if (rb.status)
                    {
                        query = @" DELETE FROM documentstore 
                                     WHERE documentId = @documentId AND documentNumber = @documentNumber AND documentName = @documentName AND dptTableId = @dptTableId";
                        rb = await db.ExecuteDeleteQueryAsync(query, pm1);
                        if (rb.status)
                        {
                            File.Delete(rd.filePath);
                            ts.Complete();
                            rb.message = "File deleted successfully.";
                            rb.status = true;
                        }
                        else
                            rb.message = "Failed to delete.";

                    }
                    else
                        rb.message = "Failed to delete.";
                }
            }
            else
                rb.message = "Invalid details provided!!!";
            return rb;
        }

        ///// <summary>
        ///// Retrive Document details like FilePath, Mime Type etc. based on input 
        ///// </summary>
        ///// <param name="documentName"></param>
        ///// <param name="documentType"></param>
        ///// <param name="documentImageGroup"></param>
        ///// <returns></returns>
        //public async Task<ReturnString> GetDocumentAsyncNew(string documentName, DocumentType documentType, DocumentImageGroup documentImageGroup)
        //{
        //    ReturnString rs = new ReturnString();
        //    string query = @" SELECT ds.documentMimeType, dp.documentImageGroup, 
        //                            CONCAT(dp.physicalPath, 
        //                            (CASE WHEN dp.addYear = 1 then CONCAT(ds.uploadYear, '\\') ELSE '' END),
        //                            (CASE WHEN dp.addFolder = 1 then CONCAT(ds.documentId, '\\') ELSE '' END),
        //                                ds.documentName, ds.documentExtension) AS filepath,ds.documentId,ds.documentNumber
        //                      FROM documentstore ds
        //                      INNER JOIN documentpathtbl dp ON dp.dptTableId = ds.dptTableId
        //                      WHERE ds.documentName = @documentName AND dp.documentType = @documentType AND ds.active = @active";
        //    MySqlParameter[] pm = new MySqlParameter[]
        //    {
        //        new MySqlParameter("documentName", MySqlDbType.String) { Value = documentName },
        //        new MySqlParameter("documentType", MySqlDbType.Int16) { Value = (int) documentType },
        //        new MySqlParameter("active", MySqlDbType.Int16) { Value = 1 }
        //    };
        //    dt = await db.ExecuteSelectQueryAsync(query, pm);
        //    if (dt.table.Rows.Count > 0)
        //    {
        //        Enum.TryParse(dt.table.Rows[0]["documentImageGroup"].ToString(), out DocumentImageGroup dig);
        //        if (documentImageGroup == dig)
        //        {
        //            rs.value = dt.table.Rows[0]["filepath"].ToString();
        //            rs.msg_id = dt.table.Rows[0]["documentMimeType"].ToString();
        //            rs.email_msg = dt.table.Rows[0]["documentId"].ToString();//document id
        //            rs.message = dt.table.Rows[0]["documentNumber"].ToString();
        //            rs.status = true;
        //        }
        //        else
        //            rs.message = "Invalid method!!!";
        //    }
        //    else
        //        rs.message = "document not found";
        //    return rs;
        //}


        /// <summary>
        /// Save work document related documents Recevied in Bytes
        /// </summary>
        /// <param name="bl"></param>
        /// <returns>ReturnBool</returns>
        public async Task<ReturnBool> SaveDocumentsInByteAsync(BlDocument bl)
        {
            rb = new ReturnBool();
            BlDocumentImagesModel bdc = await dl.GetDocumentImagesPath_Async(bl.stateId, bl.documentType, bl.documentImageGroup);

            string year = bdc.addYear ? DateTime.Now.Year.ToString() + @"\" : "";
            string addFolder = bdc.createFolder ? bl.documentId + @"\" : "";
            string errorMsg = "";
            bool allowSave = true;
            string query = @"INSERT INTO documentstore(documentId, documentNumber, dptTableId, amendmentNo,
                                         documentName, documentExtension, documentMimeType, userId, clientIp)
                             VALUES (@documentId, @documentNumber, @dptTableId, @amendmentNo, 
                                         @documentName, @documentExtension, @documentMimeType, @userId, @clientIp)";
            try
            {
                if (bdc.status)
                {
                    //get category wise file count for file name generation
                    BlDocumentImagesModel docno = await GetDocumentCount(bl);
                    if (docno.status)
                    {
                        string storage_path = bdc.physcialPath.Replace("D:", "C:") + year + addFolder;
                        bool exists = System.IO.Directory.Exists(storage_path);
                        if (!exists)
                            System.IO.Directory.CreateDirectory(storage_path);

                        bl.documentNumber = docno.fileCount;
                        if (bl.documentInByte != null)
                        {
                            if (bdc.fileType.Contains(bl.documentMimeType) == false)
                            {
                                errorMsg = "File has a different Mime type";
                                allowSave = false;
                            }
                            else
                            {
                                bl.documentExtension = ".pdf";
                            }
                            if (bl.documentInByte[0].Length > bdc.maxFileSizeAllowed)
                            {
                                errorMsg = "File exceeds the allowed limit";
                                allowSave = false;
                            }
                        }
                        else
                        {
                            errorMsg = "No files provided";
                            allowSave = false;
                        }

                        if (allowSave)
                        {
                            bl.documentName = bl.documentId + "_" + bdc.dptTableId + "_" + bl.documentNumber.ToString();
                            string filePath = storage_path + @"\" + bl.documentName + bl.documentExtension;
                            using (var stream = new FileStream(storage_path + @"\" + bl.documentName + bl.documentExtension, FileMode.CreateNew))
                            {
                                try
                                {
                                    using (MemoryStream fs = new MemoryStream(bl.documentInByte[0]))
                                    {
                                        MySqlParameter[] pm = new MySqlParameter[]
                                        {
                                            new MySqlParameter("documentId", MySqlDbType.Int64) { Value = bl.documentId},
                                            new MySqlParameter("documentNumber", MySqlDbType.Int16) { Value = bl.documentNumber},
                                            new MySqlParameter("dptTableId", MySqlDbType.Int16) { Value = bdc.dptTableId},
                                            new MySqlParameter("amendmentNo", MySqlDbType.Int16) { Value = bl.amendmentNo},
                                            new MySqlParameter("documentName", MySqlDbType.String) { Value = bl.documentName},
                                            new MySqlParameter("documentExtension", MySqlDbType.String) { Value = bl.documentExtension},
                                            new MySqlParameter("documentMimeType", MySqlDbType.String) { Value = bl.documentMimeType},
                                            new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.userId},
                                            new MySqlParameter("clientIp", MySqlDbType.String) { Value = bl.clientIp}
                                        };

                                        rb = await db.ExecuteInsertQueryAsync(query, pm);
                                        if (rb.status)
                                        {
                                            fs.CopyTo(stream);
                                            errorMsg = "Successfully uploaded";
                                        }
                                        else
                                        {
                                            errorMsg = "Failed to save document";
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Gen_Error_Rpt.Write_Error("dl_work_document:SaveDocumentsInByteAsync(error)", ex);
                                    rb.status = false;
                                }
                            }
                        }
                        bl.documentNumber++;
                        rb.remark = bl.documentName;
                    }
                    else
                    {
                        errorMsg = "Invalid document type supplied";
                    }
                }
                else
                    errorMsg = "Invalid document supplied for the method";
            }
            catch (Exception ex)
            {
                errorMsg = "Something went wrong";
                Gen_Error_Rpt.Write_Error("SaveDocumentsInByteAsync : ", ex);
            }
            rb.message = errorMsg;
            return rb;
        }

        /// <summary>
        /// Save work document related documents
        /// </summary>
        /// <param name="bl"></param>
        /// <returns>ReturnBool</returns>
        public async Task<ReturnBool> DeleteWorkDocumentsAsync(BlDocument bl)
        {
            BlDocumentImagesModel bdc = await dl.GetDocumentImagesPath_Async(bl.stateId, bl.documentType, bl.documentImageGroup);

            string year = bdc.addYear ? DateTime.Now.Year.ToString() + @"\" : "";
            string addFolder = bdc.createFolder ? bl.documentId + @"\" : "";
            string errorMsg = "";
            bool allowSave = true;

            string query = @"SELECT documentId
                                FROM documentstore AS ds
                            WHERE ds.documentId = @documentId AND ds.documentNumber = @documentNumber AND ds.dptTableId = @dptTableId ";
            MySqlParameter[] pm1 = new MySqlParameter[]
            {
                new MySqlParameter("documentId", MySqlDbType.Int64) { Value = bl.documentId},
                new MySqlParameter("documentNumber", MySqlDbType.Int16) { Value = bl.documentNumber},
                new MySqlParameter("dptTableId", MySqlDbType.Int16) { Value = bdc.dptTableId},
            };
            dt = await db.ExecuteSelectQueryAsync(query, pm1);
            if (dt.table.Rows.Count > 0)
            {
                query = @"DELETE
                                FROM documentstore AS ds
                            WHERE ds.documentId = @documentId AND ds.documentNumber = @documentNumber  AND ds.dptTableId = @dptTableId ";
                rb = await db.ExecuteDeleteQueryAsync(query, pm1);
            }

            query = @"INSERT INTO documentstore(documentId, documentNumber, dptTableId, amendmentNo,
                                         documentName, documentExtension, documentMimeType, userId, clientIp)
                             VALUES (@documentId, @documentNumber, @dptTableId, @amendmentNo, 
                                         @documentName, @documentExtension, @documentMimeType, @userId, @clientIp)";
            try
            {
                if (bdc.status)
                {
                    //get category wise file count for file name generation
                    BlDocumentImagesModel docno = await GetDocumentCount(bl);
                    if (docno.status)
                    {
                        string storage_path = bdc.physcialPath.Replace("D:", "C:") + year + addFolder;
                        bool exists = Directory.Exists(storage_path);
                        if (!exists)
                            Directory.CreateDirectory(storage_path);

                        bl.documentNumber = docno.fileCount;
                        if (bl.files.Count > 0)
                        {
                            foreach (var file in bl.files)
                            {
                                if (file.Length > bdc.maxFileSizeAllowed)
                                {
                                    errorMsg = "File exceeds the allowed limit";
                                    allowSave = false;
                                    break;
                                }

                                if (bdc.fileType.Contains(bl.documentMimeType) == false)
                                {
                                    errorMsg = "File has a different Mime type";
                                    allowSave = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            errorMsg = "No files provided";
                            allowSave = false;
                        }

                        if (allowSave)
                        {
                            foreach (var file in bl.files)
                            {
                                if (file.Length > 0)
                                {
                                    bl.documentMimeType = file.ContentType.ToLower();
                                    bl.documentExtension = Path.GetExtension(file.FileName).ToString();
                                    bl.documentName = bl.documentId + "_" + bdc.dptTableId + "_" + bl.documentNumber.ToString();

                                    using (var stream = new FileStream(storage_path + @"\" + bl.documentName + bl.documentExtension, FileMode.CreateNew))
                                    {
                                        try
                                        {
                                            using (MemoryStream fs = new MemoryStream())
                                            {
                                                MySqlParameter[] pm = new MySqlParameter[]
                                                {
                                                    new MySqlParameter("documentId", MySqlDbType.Int64) { Value = bl.documentId},
                                                    new MySqlParameter("documentNumber", MySqlDbType.Int16) { Value = bl.documentNumber},
                                                    new MySqlParameter("dptTableId", MySqlDbType.Int16) { Value = bdc.dptTableId},
                                                    new MySqlParameter("amendmentNo", MySqlDbType.Int16) { Value = bl.amendmentNo},
                                                    new MySqlParameter("documentName", MySqlDbType.String) { Value = bl.documentName},
                                                    new MySqlParameter("documentExtension", MySqlDbType.String) { Value = bl.documentExtension},
                                                    new MySqlParameter("documentMimeType", MySqlDbType.String) { Value = bl.documentMimeType},
                                                    new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.loginId},
                                                    new MySqlParameter("clientIp", MySqlDbType.String) { Value = bl.clientIp}
                                                };

                                                rb = await db.ExecuteInsertQueryAsync(query, pm);
                                                if (rb.status)
                                                {
                                                    file.CopyTo(stream);
                                                    errorMsg = "Successfully uploaded";
                                                }
                                                else
                                                {
                                                    errorMsg = "Failed to save document";
                                                    break;
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Gen_Error_Rpt.Write_Error("DlDocument:SaveDocumentsAsync(error)", ex);
                                            rb.status = false;
                                            break;
                                        }
                                    }
                                }
                                bl.documentNumber++;
                                rb.remark = bl.documentName;
                            }
                        }
                    }
                    else
                    {
                        errorMsg = "Invalid document type supplied";
                    }
                }
                else
                    errorMsg = "Invalid document supplied for the method";
            }
            catch (Exception ex)
            {
                errorMsg = "Something went wrong";
                Gen_Error_Rpt.Write_Error("DlDocument:SaveDocumentsAsync : ", ex);
            }
            rb.message = errorMsg;
            return rb;
        }

        /// <summary>
        /// Save work document related documents
        /// </summary>
        /// <param name="bl"></param>
        /// <returns>ReturnBool</returns>
        public async Task<ReturnBool> SaveDocumentsAsync(BlDocument bl)
        {
            BlDocumentImagesModel bdc = await dl.GetDocumentImagesPath_Async(bl.stateId, bl.documentType, bl.documentImageGroup);

            string year = bdc.addYear ? DateTime.Now.Year.ToString() + @"\" : "";
            string addFolder = bdc.createFolder ? bl.documentId + @"\" : "";
            string errorMsg = "";
            bool allowSave = true;

            string query = @"SELECT documentId
                                FROM documentstore AS ds
                            WHERE ds.documentId = @documentId AND ds.documentNumber = @documentNumber  AND ds.dptTableId = @dptTableId ";
            MySqlParameter[] pm1 = new MySqlParameter[]
            {
                new MySqlParameter("documentId", MySqlDbType.Int64) { Value = bl.documentId},
                new MySqlParameter("documentNumber", MySqlDbType.Int16) { Value = bl.documentNumber},
                new MySqlParameter("dptTableId", MySqlDbType.Int16) { Value = bdc.dptTableId},
            };
            dt = await db.ExecuteSelectQueryAsync(query, pm1);
            if (dt.table.Rows.Count > 0)
            {
                query = @"DELETE
                                FROM documentstore AS ds
                            WHERE ds.documentId = @documentId AND ds.documentNumber = @documentNumber AND ds.dptTableId = @dptTableId ";
                rb = await db.ExecuteDeleteQueryAsync(query, pm1);
            }

            query = @"INSERT INTO documentstore(documentId, documentNumber, dptTableId, amendmentNo,
                                         documentName, documentExtension, documentMimeType, userId, clientIp)
                             VALUES (@documentId, @documentNumber, @dptTableId, @amendmentNo, 
                                         @documentName, @documentExtension, @documentMimeType, @userId, @clientIp)";
            try
            {
                if (bdc.status)
                {
                    //get category wise file count for file name generation
                    BlDocumentImagesModel docno = await GetDocumentCount(bl);
                    if (docno.status)
                    {
                        string storage_path = bdc.physcialPath.Replace("D:", "C:") + year + addFolder;
                        bool exists = Directory.Exists(storage_path);
                        if (!exists)
                            Directory.CreateDirectory(storage_path);

                        bl.documentNumber = docno.fileCount;
                        if (bl.documentInByte != null)
                            bl = await convertFileBack(bl);
                        else if (bl.documentInByteS != null)
                            bl = await convertFileBackFromBase64(bl);

                        if (bl.files.Count > 0)
                        {
                            foreach (var file in bl.files)
                            {
                                if (file.Length > bdc.maxFileSizeAllowed)
                                {
                                    errorMsg = "File exceeds the allowed limit";
                                    allowSave = false;
                                    break;
                                }

                                if (bdc.fileType.Contains(bl.documentMimeType) == false)
                                {
                                    errorMsg = "File has a different Mime type";
                                    allowSave = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            errorMsg = "No files provided";
                            allowSave = false;
                        }

                        if (allowSave)
                        {
                            foreach (var file in bl.files)
                            {
                                if (file.Length > 0)
                                {
                                    bl.documentMimeType = file.ContentType.ToLower();
                                    bl.documentExtension = Path.GetExtension(file.FileName).ToString();
                                    bl.documentName = bl.documentId + "_" + bdc.dptTableId + "_" + bl.documentNumber;

                                    using (var stream = new FileStream(storage_path + @"\" + bl.documentName + bl.documentExtension, FileMode.CreateNew))
                                    {
                                        try
                                        {
                                            using (MemoryStream fs = new MemoryStream())
                                            {
                                                MySqlParameter[] pm = new MySqlParameter[]
                                                {
                                                    new MySqlParameter("documentId", MySqlDbType.Int64) { Value = bl.documentId},
                                                    new MySqlParameter("documentNumber", MySqlDbType.Int16) { Value = bl.documentNumber},
                                                    new MySqlParameter("dptTableId", MySqlDbType.Int16) { Value = bdc.dptTableId},
                                                    new MySqlParameter("amendmentNo", MySqlDbType.Int16) { Value = bl.amendmentNo},
                                                    new MySqlParameter("documentName", MySqlDbType.String) { Value = bl.documentName},
                                                    new MySqlParameter("documentExtension", MySqlDbType.String) { Value = bl.documentExtension},
                                                    new MySqlParameter("documentMimeType", MySqlDbType.String) { Value = bl.documentMimeType},
                                                    new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.loginId},
                                                    new MySqlParameter("clientIp", MySqlDbType.String) { Value = bl.clientIp}
                                                };

                                                rb = await db.ExecuteInsertQueryAsync(query, pm);
                                                if (rb.status)
                                                {
                                                    file.CopyTo(stream);
                                                    errorMsg = "Successfully uploaded";
                                                }
                                                else
                                                {
                                                    errorMsg = "Failed to save document";
                                                    break;
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Gen_Error_Rpt.Write_Error("DlDocument:SaveDocumentsAsyncDirect(error)", ex);
                                            rb.status = false;
                                            break;
                                        }
                                    }
                                }
                                bl.documentNumber++;
                                //rb.remark = rb.remark;
                                errorMsg = bl.documentName;
                            }
                        }
                    }
                    else
                    {
                        errorMsg = "Invalid document type supplied";
                    }
                }
                else
                    errorMsg = "Invalid document supplied for the method";
            }
            catch (Exception ex)
            {
                //errorMsg = "Something went wrong";
                errorMsg = ex.Message;
                Gen_Error_Rpt.Write_Error("DlDocument:SaveDocumentsAsync : ", ex);
            }
            rb.message = errorMsg;
            return rb;
        }

        /// <summary>
        /// Save work document related documents
        /// </summary>
        /// <param name="bl"></param>
        /// <returns>ReturnBool</returns>
        public async Task<ReturnBool> SaveDocumentsAsyncNew(BlDocument bl)
        {
            BlDocumentImagesModel bdc = await dl.GetDocumentImagesPath_Async(bl.stateId, bl.documentType, bl.documentImageGroup);

            string year = bdc.addYear ? DateTime.Now.Year.ToString() + @"\" : "";
            string addFolder = bdc.createFolder ? bl.documentId + @"\" : "";
            string errorMsg = "";
            bool allowSave = true;

            string query = @"SELECT documentId
                                FROM documentstore AS ds
                            WHERE ds.documentId = @documentId AND ds.documentNumber = @documentNumber  AND ds.dptTableId = @dptTableId ";
            MySqlParameter[] pm1 = new MySqlParameter[]
            {
                new MySqlParameter("documentId", MySqlDbType.Int64) { Value = bl.documentId},
                new MySqlParameter("documentNumber", MySqlDbType.Int16) { Value = bl.documentNumber},
                new MySqlParameter("dptTableId", MySqlDbType.Int16) { Value = bdc.dptTableId},
            };
            dt = await db.ExecuteSelectQueryAsync(query, pm1);
            if (dt.table.Rows.Count > 0)
            {
                query = @"DELETE
                                FROM documentstore AS ds
                            WHERE ds.documentId = @documentId AND ds.documentNumber = @documentNumber AND ds.dptTableId = @dptTableId ";
                rb = await db.ExecuteDeleteQueryAsync(query, pm1);
            }

            query = @"INSERT INTO documentstore(documentId, documentNumber, dptTableId, amendmentNo,
                                         documentName, documentExtension, documentMimeType, userId, clientIp)
                             VALUES (@documentId, @documentNumber, @dptTableId, @amendmentNo, 
                                         @documentName, @documentExtension, @documentMimeType, @userId, @clientIp)";
            try
            {
                if (bdc.status)
                {
                    //get category wise file count for file name generation
                    BlDocumentImagesModel docno = await GetDocumentCount(bl);
                    if (docno.status)
                    {
                        string storage_path = bdc.physcialPath.Replace("D:", "C:") + year + addFolder;
                        bool exists = Directory.Exists(storage_path);
                        if (!exists)
                            Directory.CreateDirectory(storage_path);

                        bl.documentNumber = docno.fileCount;
                        if (bl.documentInByte != null)
                            bl = await convertFileBack(bl);
                        else if (bl.documentInByteS != null)
                            bl = await convertFileBackFromBase64(bl);

                        if (bl.files.Count > 0)
                        {
                            foreach (var file in bl.files)
                            {
                                if (file.Length > bdc.maxFileSizeAllowed)
                                {
                                    errorMsg = "File exceeds the allowed limit";
                                    allowSave = false;
                                    break;
                                }

                                if (bdc.fileType.Contains(bl.documentMimeType) == false)
                                {
                                    errorMsg = "File has a different Mime type";
                                    allowSave = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            errorMsg = "No files provided";
                            allowSave = false;
                        }

                        if (allowSave)
                        {
                            foreach (var file in bl.files)
                            {
                                if (file.Length > 0)
                                {
                                    bl.documentMimeType = file.ContentType.ToLower();
                                    bl.documentExtension = Path.GetExtension(file.FileName).ToString();
                                    bl.documentName = bl.documentId + "_" + bdc.dptTableId + "_" + bl.documentNumber;

                                    using (var stream = new FileStream(storage_path + @"\" + bl.documentName + bl.documentExtension, FileMode.CreateNew))
                                    {
                                        try
                                        {
                                            using (MemoryStream fs = new MemoryStream())
                                            {
                                                MySqlParameter[] pm = new MySqlParameter[]
                                                {
                                                    new MySqlParameter("documentId", MySqlDbType.Int64) { Value = bl.documentId},
                                                    new MySqlParameter("documentNumber", MySqlDbType.Int16) { Value = bl.documentNumber},
                                                    new MySqlParameter("dptTableId", MySqlDbType.Int16) { Value = bdc.dptTableId},
                                                    new MySqlParameter("amendmentNo", MySqlDbType.Int16) { Value = bl.amendmentNo},
                                                    new MySqlParameter("documentName", MySqlDbType.String) { Value = bl.documentName},
                                                    new MySqlParameter("documentExtension", MySqlDbType.String) { Value = bl.documentExtension},
                                                    new MySqlParameter("documentMimeType", MySqlDbType.String) { Value = bl.documentMimeType},
                                                    new MySqlParameter("userId", MySqlDbType.Int64) { Value = bl.loginId},
                                                    new MySqlParameter("clientIp", MySqlDbType.String) { Value = bl.clientIp}
                                                };

                                                rb = await db.ExecuteInsertQueryAsync(query, pm);
                                                if (rb.status)
                                                {
                                                    file.CopyTo(stream);
                                                    errorMsg = "Successfully uploaded";
                                                }
                                                else
                                                {
                                                    errorMsg = "Failed to save document";
                                                    break;
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Gen_Error_Rpt.Write_Error("DlDocument:SaveDocumentsAsyncDirect(error)", ex);
                                            rb.status = false;
                                            break;
                                        }
                                    }
                                }
                                bl.documentNumber++;
                                rb.remark = bl.documentName;
                            }
                        }
                    }
                    else
                    {
                        errorMsg = "Invalid document type supplied";
                    }
                }
                else
                    errorMsg = "Invalid document supplied for the method";
            }
            catch (Exception ex)
            {
                errorMsg = "Something went wrong";
                Gen_Error_Rpt.Write_Error("DlDocument:SaveDocumentsAsync : ", ex);
            }
            rb.message = errorMsg;
            return rb;
        }
        public async Task<BlDocument> convertFileBack(BlDocument bl)
        {
            bl.files = new();
            foreach (var item in bl.documentInByte)
            {
                var stream = new MemoryStream(item);
                var file = new FormFile(stream, 0, item.Length, "name", "fileName.pdf")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "application/pdf",
                };

                System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = file.FileName
                };
                file.ContentDisposition = cd.ToString();
                bl.files.Add(file);
            }
            return bl;
        }

        public async Task<BlDocument> convertFileBackFromBase64(BlDocument bl)
        {
            bl.files = new();
            List<IFormFile> formFiles = new List<IFormFile>();
            string extension = "";
            string[] documentInByteS;
            string[] contentType;
            foreach (var eqp in bl.documentInByteS)
            {
                documentInByteS = eqp.Split(",");
                byte[] bytes = Convert.FromBase64String(documentInByteS[1]);
                extension = await GetFileType(documentInByteS);
                var stream = new MemoryStream(bytes);
                contentType = documentInByteS[0].Split("data:");
                contentType = contentType[1].Split(";");
                var file = new FormFile(stream, 0, bytes.Length, "name", "fileName." + extension)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = Convert.ToString(contentType[0])
                };

                System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = file.FileName
                };
                file.ContentDisposition = cd.ToString();
                bl.files.Add(file);
            }
            return bl;
        }

        public async Task<String> GetFileType(string[] base64String)
        {
            String[] strings = base64String;
            String extension;
            switch (strings[0])
            {//check image's extension
                case "data:image/jpeg;base64":
                    extension = "jpeg";
                    break;
                case "data:image/png;base64":
                    extension = "png";
                    break;
                case "data:application/pdf;base64":
                    extension = "pdf";
                    break;
                default:
                    extension = "jpg";
                    break;
            }
            return extension;
        }
    }
}
