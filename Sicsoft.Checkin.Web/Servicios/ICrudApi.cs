using Refit;
using Sicsoft.Checkin.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sicsoft.Checkin.Web.Servicios
{
    //public interface ICrudApi<T, in TKey, TQuery> where T : class
    //{
    //    [Post("")]
    //    Task Agregar([Body] T payload);

    //    [Get("")]
    //    Task<T[]> ObtenerLista(TQuery q);

    //    [Get("/{key}")]
    //    Task<T> ObtenerPorId(TKey key);

    //    [Put("/{key}")]
    //    Task Editar(TKey key, [Body]T payload);

    //    [Delete("/{key}")]
    //    Task Eliminar(TKey key);
    //}

    public interface ICrudApi<TEntity, TKey> where TEntity : class
    {
        //[Post("")]
        //Task<TEntity> Agregar<TCreate>([Body] TCreate payload) where TCreate : class;

        

        [Post("/Insertar")]
        Task<TEntity> Agregar([Body] TEntity payload);
        [Post("")]
        Task<TEntity> CambiarClave([Body] TEntity payload);

        [Get("")]
        Task<TEntity> Login(string email, string pw);


        [Get("")]
        Task<TEntity[]> ObtenerLista<TQuery>(TQuery q);

       

        


        [Get("/Consultar")]
        Task<TEntity> ObtenerPorId(int id);

        [Post("/{id}")]
        Task GenerarMovimientos(int id);

        [Post("/Debitar")]
        Task Debitar(int id, string certificado, decimal monto);

        [Put("/Actualizar")]
        Task Editar( [Body]TEntity payload);

        [Delete("/Eliminar")]
        Task EliminarInversion(string id);

        [Delete("/Eliminar")]
        Task Eliminar(int id);

        [Delete("/Eliminar")]
        Task EliminarEjecutivo (int idEjecutivo);


       
       // Task<TEntity> ObtenerPorId(TKey key);
    }
}
