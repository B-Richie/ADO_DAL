using AutomobileApp.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AutomobileApp.Entities
{
    public class Model : IModel
    {
        private int _Id { get; set; }
        private string _ModelName { get; set; }
        private int _MakeId { get; set; }

        public int Id { get { return _Id; } set { _Id = value; } }
        public string ModelName { get { return _ModelName; } set { _ModelName = value; } }
        public int MakeId { get { return _MakeId; } set { _MakeId = value; } }


        public void Delete(int id)
        {
            const string sql = "delete from [dbo].Model where Id = @Id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Id", id);
            SqlConnectionClass.ExecuteNonQuery(sql, parameters, System.Data.CommandType.Text);
        }

        public Object Insert(Model model)
        {
            const string sql = "Insert into [dbo].Model (ModelName, MakeId) values (@modelName, @makeId)SELECT SCOPE_IDENTITY()";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@modelName", model.ModelName);
            parameters.Add("@makeId", model.MakeId);
            var result = SqlConnectionClass.ExecuteScalar(sql, parameters);
            return result;
        }

        public Model MapModelObject(SqlDataReader dr)
        {
            return new Model
            {
                Id = dr.GetValue<int>("Id"),
                ModelName = dr.GetValue<string>("ModelName"),
                MakeId= dr.GetValue<int>("MakeId")
            };
        }

        public List<Model> GetModelsByMakerId(int Id)
        {
            const string sql = "select * from [dbo].Model where MakeId = @Id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Id", Id);
            var result = SqlConnectionClass.GetList<Model>(sql, parameters, dr => MapModelObject(dr));
            return result;
        }
    }
}