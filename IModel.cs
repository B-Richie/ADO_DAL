using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AutomobileApp.Entities
{
    public interface IModel
    {
        void Delete(int id);
        Object Insert(Model model);
        Model MapModelObject(SqlDataReader dr);
        List<Model> GetModelsByMakerId(int Id);

    }
}