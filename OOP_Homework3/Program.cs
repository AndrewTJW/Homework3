/*
After testing, everything works
Time tested: 11:51PM 22th October 2024
*/


using Google.Cloud.Firestore;
//using class library
using Homework2Library;

namespace OOP_Homework3
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Test code
            Database database = new Database();
            database.initDatabase();

            //create a object for demonstration
            UserTransaction transaction1 = new UserTransaction(500, "Transportation", DateTime.Now);

            // create a new task to carry out the saving function
            // need to do this way because app is responsive
            var task1 = Task.Run(async () => await database.saveTransaction(transaction1));
            task1.Wait(); //blocks until the task is complete

            UserTransaction transaction2 = new UserTransaction(100, "Food and Beverage", DateTime.Now);

            task1 = Task.Run(async () => await database.saveTransaction(transaction2));
            task1.Wait();

            UserTransaction transaction3 = new UserTransaction(300, "Advanced Salary", DateTime.Now);

            task1 = Task.Run(async () => await database.saveTransaction(transaction3));
            task1.Wait();

            Item item1 = new Item("Car", 1, 56000, "1");

            task1 = Task.Run(async () => await database.saveAsset(item1));
            task1.Wait();

            Item item2 = new Item("Table", 50, 25.50, "2");

            task1 = Task.Run(async () => await database.saveAsset(item2));
            task1.Wait();

            //retrive data test
            var task2 = Task.Run(async () => await database.retrieveTransaction());
            task2.Wait();

            task2 = Task.Run(async () => await database.retrieveItem());
            task2.Wait();


            //testing delete method => deleteAsset([docID])
            var task4 = Task.Run(async () => await database.deleteAsset("638652376210777632"));
            task4.Wait();

            task4 = Task.Run(async() => await database.deleteAsset("638652376211282668"));
            task4.Wait();



            //testing delete method => deleteTransaction([docID])
            var task3 = Task.Run(async () => await database.deleteTransaction("638652376205314712"));
            task3.Wait();

            task3 = Task.Run(async () => await database.deleteTransaction("638652376209496751"));
            task3.Wait();

            task3 = Task.Run(async () => await database.deleteTransaction("638652376210163143"));
            task3.Wait();

            //testing saving user detail

            UserDetail user1 = new UserDetail("evelynn@gmail.com", "12345pass", "Evelynn", 21);

            var task5 = Task.Run(async () => await database.saveUserData(user1));
            task5.Wait();
            //retrieving user data 
            task5 = Task.Run(async () => await database.retrieveUserData("789d18ac-203a-47c5-89ea-8e9693874d59"));
            task5.Wait();
            //testing delete method
            var task6 = Task.Run(async () => await database.deleteUserData("789d18ac-203a-47c5-89ea-8e9693874d59"));
            task6.Wait();
        }
    }
}
