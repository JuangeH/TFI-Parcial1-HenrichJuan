using DAL.Contracts;
using DAL.Helper.Contract;
using Dapper;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repos
{
    public class GestorDocRepo : IGestorDocRepo
    {
        #region Statements
        private string InsertStatement
        {
            get => "INSERT INTO [dbo].[Operacion] (Origen, Destino, Fecha, Usuario_Id) OUTPUT INSERTED.Operacion_Id VALUES (@Origen, @Destino, @Fecha, @Usuario_Id)";
        }

        private string UpdateStockStatement
        {
            get => "UPDATE [dbo].[LocalProducto] SET Cantidad = @Cantidad WHERE Producto_Id = @Producto_Id AND Local_Id = @Local_Id";
        }

        private string SelectAllById
        {
            get => "SELECT * FROM [dbo].[Ciudad] WHERE Id_Ciudad = @Id_Ciudad";
        }
        private string SelectAllByNom
        {
            get => "SELECT * FROM [dbo].[DocumentosRegistrados] WHERE Nombre = @Nombre";
        }

        private string SelectAllByIdStatement
        {
            get => "SELECT Producto_Id, Cantidad FROM [dbo].[LocalProducto] WHERE Local_Id = @Local_Id";
        }
        private string SelectAllStatement
        {
            get => "SELECT * FROM [dbo].[Camion]";
        }
        private string InsertStockStatement
        {
            get => "INSERT INTO [dbo].[LocalProducto] (Producto_Id, Local_Id, Cantidad) VALUES (@Producto_Id, @Local_Id, @Cantidad)";
        }
        #endregion

        private readonly IDapper _dapperHelper;

        public GestorDocRepo(IDapper dapperHelper)
        {
            _dapperHelper = dapperHelper;
        }
        public FileDataModel ListarByNom(FileDataModel fileDataModel)
        {
            try
            {
                var dbPara = new DynamicParameters();
                dbPara.Add("Nombre", fileDataModel.Nombre);
                fileDataModel = _dapperHelper.Get<FileDataModel>(SelectAllByNom, dbPara, commandType: CommandType.Text);

                return fileDataModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}