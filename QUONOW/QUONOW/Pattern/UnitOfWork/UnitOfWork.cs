using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QUONOW.Pattern.Repository;
using QUONOW.Models;
using System.Data.Entity;



namespace QUONOW.Pattern.UnitOfWork
{
    public class UnitOfWork
    {
        Repository<User> userRepository;

        Repository<Booking> BookingRepository;

        Repository<Event> EventRepository;

        Repository<EventType> EventTypeRepository;

        Repository<Product> ProductRepository;

        Repository<ProductType> ProductTypeRepository;

        Repository<Refund> RefundRepository;

        Repository<UserLocation> UserLocationRepository;

        Repository<UserType> UserTypeRepository;

        Repository<Review> ReviewRepository;

        Repository<Cart> CartRepository;

        Repository<Email> EmailRepository;

        Repository<Pub> PubRepository;

        Repository<Payment> paymentReporsitory;

    //    Repository<TempBooking> _tempBooking;

        private QUONOWEntities context;
        public UnitOfWork(QUONOWEntities db)
        {
            this.context = db;
        }


        public Repository<User> _userRepository
        {
            get
            {
                userRepository = new Repository<User>(context);
                return userRepository;
            }
            //set {
            //}
        }

        public Repository<Booking> _bookingRepository
        {
            get
            {
                BookingRepository = new Repository<Booking>(context);
                return BookingRepository;
            }
        }


        public Repository<Event> _eventRepository
        {
            get
            {
                EventRepository = new Repository<Event>(context);
                return EventRepository;
            }
        }


        public Repository<EventType> _evnetTypeRepository
        {
            get
            {
                EventTypeRepository = new Repository<EventType>(context);
                return EventTypeRepository;
            }
        }


        public Repository<Product> _productRepository
        {
            get
            {
                ProductRepository = new Repository<Product>(context);
                return ProductRepository;
            }
        }


        public Repository<ProductType> _productTypeRepository
        {
            get
            {
                ProductTypeRepository = new Repository<ProductType>(context);
                return ProductTypeRepository;
            }
        }
        public Repository<Refund> _RefundRepository
        {
            get
            {
                RefundRepository = new Repository<Refund>(context);
                return RefundRepository;
            }
        }

        public Repository<UserLocation> _userLocationRepository
        {
            get
            {
                UserLocationRepository = new Repository<UserLocation>(context);
                return UserLocationRepository;
            }
        }

        public Repository<UserType> _userTypeRepository
        {
            get
            {
                UserTypeRepository = new Repository<UserType>(context);
                return UserTypeRepository;
            }
        }

        //public IRepository<T> Repository<T>() where T : class
        //{
        //    if (repositories.Keys.Contains(typeof(T)) == true)
        //    {
        //        return repositories[typeof(T)] as IRepository<T>
        //    }
        //    IRepository<T> repo = new GenericRepository<T>(entities);
        //    repositories.Add(typeof(T), repo);
        //    return repo;
        //}


        public Repository<Review> _reviewRepository
        {
            get
            {
                ReviewRepository = new Repository<Review>(context);
                return ReviewRepository;
            }
        }

        public Repository<Cart> _cartRepository
        {
            get
            {
                CartRepository = new Repository<Cart>(context);
                return CartRepository;
            }
        }

        public Repository<Email> _EmailRepository
        {
            get
            {
                EmailRepository = new Repository<Email>(context);
                return EmailRepository;
            }
        }

        public Repository<Pub> _PubRepository
        {
            get
            {
                PubRepository = new Repository<Pub>(context);
                return PubRepository;
            }
        }

        public Repository<Payment> _paymentRepository
        {
            get
            {
                paymentReporsitory = new Repository<Payment>(context);
                return paymentReporsitory;
            }
        }


        //public Repository<TempBooking> _tempBookingRepository
        //{
        //    get
        //    {
        //        this._tempBooking = new Repository<TempBooking>(context);
        //        return this._tempBooking;
        //    }
        //}



        public int Commit()
        {
            return context.SaveChanges();
        }


        public void Dispose()
        {
            context.Dispose();
        }
    }
}