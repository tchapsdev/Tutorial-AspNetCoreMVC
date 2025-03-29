using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAppMVC.Data;
using WebAppMVC.Models;

namespace WebAppMVC.DAL
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly WebAppMVCContext _context;

        public CustomerRepository(WebAppMVCContext context)
        {
            _context = context;
        }


        // CRUD

        public async Task InsertCustomer(Customer customer)
        {
            await _context.Customer.AddAsync(customer);
        }

        public void UpdateCustomer(Customer customer)
        {
            _context.Entry<Customer>(customer).State = EntityState.Modified;
        }

        public async Task DeleteCustomer(int customerId)
        {
            Customer customer = await _context.Customer.FindAsync(customerId);
            _context.Customer.Remove(customer);
        }

        public async Task<Customer> GetCustomerById(int customerId)
        {
            return await _context.Customer.FindAsync(customerId);
        }

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            return await _context.Customer.ToListAsync();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }




        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


    }
}
