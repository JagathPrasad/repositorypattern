using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QUONOW.Pattern.Repository;
using QUONOW.Models;

namespace QUONOW.Pattern.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {

        Repository<User> StudentRepository { get; }

        Repository<Booking> BookingRepository { get; }
        Repository<Event> EventRepository { get; }

        Repository<EventType> EventTypeRepository { get; }

        Repository<Product> ProductRepository { get; }

        Repository<ProductType> ProductTypeRepository { get; }

        Repository<Refund> RefundRepository { get; }

        Repository<UserLocation> UserLocationRepository { get; }

        Repository<UserType> UserTypeRepository { get; }

        int Commit();

    }
}