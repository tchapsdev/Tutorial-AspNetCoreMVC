using WebAppMVC.Models;

namespace WebAppMVC.DAL
{
    public interface ICustomerRepository
    {
        Task DeleteCustomer(int customerId);
        void Dispose();
        Task<Customer> GetCustomerById(int customerId);
        Task<IEnumerable<Customer>> GetCustomers();
        Task InsertCustomer(Customer customer);
        Task Save();
        void UpdateCustomer(Customer customer);
    }
}