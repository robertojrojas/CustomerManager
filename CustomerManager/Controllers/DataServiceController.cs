﻿using CustomerManager.Model;
using CustomerManager.Repository;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CustomerManager.Controllers
{
    public class DataServiceController : ApiController
    {
        CustomerManagerContext _Context;

        public DataServiceController()
        {
            _Context = new CustomerManagerContext();
        }

        [HttpGet]
        public IQueryable<Customer> Customers()
        {
            return _Context.Customers.Include("Orders");
        }

        [HttpGet]
        public IQueryable<CustomerSummary> CustomersSummary()
        {
            return _Context.Customers.Select(c => new CustomerSummary
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                City = c.City,
                State = c.State,
                OrderCount = c.Orders.Count(),
                Gender = c.Gender
            });
        }

        // GET api/<controller>/5
        [HttpGet]
        public Customer CustomerById(int id)
        {
            return _Context.Customers.Include("Orders").SingleOrDefault(c => c.Id == id);
        }

        // POST api/<controller>
        [HttpPost]
        public OperationStatus InsertCustomer([FromBody]Customer customer)
        {
            var opStatus = new OperationStatus();
            try
            {
                _Context.Customers.Add(customer);
                _Context.SaveChanges();
                opStatus.Status = true;
                opStatus.OperationID = customer.Id;
            }
            catch (Exception exp)
            {
                return OperationStatus.CreateFromException("Error updating customer", exp);
            }
            return opStatus;
        }

        // PUT api/<controller>/5
        [HttpPut]
        public OperationStatus UpdateCustomer(int id, [FromBody]Customer customer)
        {
            var opStatus = new OperationStatus();
            try
            {
                _Context.Customers.Attach(customer);
                _Context.Entry<Customer>(customer).State = System.Data.EntityState.Modified;
                _Context.SaveChanges();
                opStatus.Status = true;
            }
            catch (Exception exp)
            {
                return OperationStatus.CreateFromException("Error updating customer", exp);
            }
            return opStatus;
        }

        // DELETE api/<controller>/5
        [HttpDelete]
        public OperationStatus DeleteCustomer(int id)
        {
            var opStatus = new OperationStatus();
            try
            {
                var cust = _Context.Customers.SingleOrDefault(c => c.Id == id);
                if (cust != null)
                {
                    _Context.Customers.Remove(cust);
                    _Context.SaveChanges();
                    opStatus.Status = true;
                }
                else
                {
                    opStatus.Message = "Customer not found!";
                }
            }
            catch (Exception exp)
            {
                return OperationStatus.CreateFromException("Error deleting customer", exp);
            }
            return opStatus;
        }
    }
}