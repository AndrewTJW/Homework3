using FirebaseAdmin;
using Google.Cloud.Firestore;
using Homework2Library;

namespace OOP_Homework3
{
    internal class Database
    {
        private const string firebase_key = "homework3-bbd99";
        private FirestoreDb database;
        
        //temporary lists are created to store newly retrieved data from Firebase
        public TransactionList tmp_trans { get; set; } 
        public Inventory tmp_items { get; set; }

        public Database()
        {
            tmp_trans = new TransactionList();
            tmp_items = new Inventory();

        }

        public void initDatabase()
        {
            FirebaseApp.Create();
            database = FirestoreDb.Create(firebase_key);
            Console.WriteLine("Created Cloud Firestore client with project ID: {0}", firebase_key);
        }

        //to save transaction
        public async Task saveTransaction(UserTransaction a_user_transaction)
        {
            CollectionReference collectionRef = database.Collection("user_transactions");
            DocumentReference docRef = collectionRef.Document(DateTime.Now.Ticks.ToString()); //DateTime.Now.Ticks.ToString() is used because it has near perfect unique ID
            Dictionary<string, object> transaction_data = new Dictionary<string, object>
            {
                {"TransactionReason", a_user_transaction.getTransactionReason()},
                {"Amount", a_user_transaction.getAmount()},
                //need to save transaction date as string because firebase only allows UTC timestamp, if save as DateTime need to convert to UTC before saving
                {"TransactionDate", a_user_transaction.getTransactionDateTime().ToString()}
            };

            Console.WriteLine("Transaction with ID: {0} has been saved.\n", docRef.Id);
            Console.WriteLine("Data saved: \n");
            Console.WriteLine("Transaction amount: {0}", a_user_transaction.getAmount());
            Console.WriteLine("Transaction reason: {0}", a_user_transaction.getTransactionReason());
            Console.WriteLine("Transaction date: {0}", a_user_transaction.getTransactionDateTime());
            await docRef.SetAsync(transaction_data);
        }

        //to save user items
        public async Task saveAsset(Item a_user_asset)
        {
            CollectionReference collectionRef = database.Collection("user_items");
            DocumentReference docRef = collectionRef.Document(DateTime.Now.Ticks.ToString());
            Dictionary<string, object> items = new Dictionary<string, object>
            {
                {"Item ID", a_user_asset.getItemID()},
                {"Item Name", a_user_asset.getItemName()},
                {"Quantity", a_user_asset.getItemCount()},
                {"Price", a_user_asset.getPrice()}
            };
            Console.WriteLine("User asset with ID: {0} has been saved.\n", docRef.Id);
            Console.WriteLine("Data saved: \n");
            Console.WriteLine("Item Name: {0}", a_user_asset.getItemName());
            Console.WriteLine("Item ID: {0}", a_user_asset.getItemID());
            Console.WriteLine("Quantity: {0}", a_user_asset.getItemCount());
            Console.WriteLine("Price: {0}", a_user_asset.getPrice());
            await docRef.SetAsync(items);
        }

        public async Task saveUserData(UserDetail a_user_detail)
        {
            CollectionReference collectionRef = database.Collection("user_detail");
            DocumentReference docRef = collectionRef.Document(a_user_detail.getUserID());
            Dictionary<string, object> userdata = new Dictionary<string, object>
            {
                {"user_ID", a_user_detail.getUserID()},
                {"email", a_user_detail.getEmail()},
                {"password", a_user_detail.getPassword()},
                {"name", a_user_detail.getName()},
                {"age", a_user_detail.getAge()}
            };
            Console.WriteLine("User detail with email {0} saved successfully", a_user_detail.getEmail());
            await docRef.SetAsync(userdata);
        }
        //to retrieve data from Firebase according to collection and it gets every document inside the collection and temporarily stores in in a dictionary
        //it is then distributed to temporary variables to store the value 
        //then it is transferred to a temporary object
        public async Task retrieveTransaction()
        {
            Query collectionQuery = database.Collection("user_transactions");
            QuerySnapshot allQuerySnapshot = await collectionQuery.GetSnapshotAsync();

            tmp_trans.Clear();
            foreach (DocumentSnapshot documentSnapshot in allQuerySnapshot.Documents)
            {
                Dictionary<string, object> data = documentSnapshot.ToDictionary();
                double tmp_amount = double.Parse(data["Amount"].ToString());
                //since firebase can only accept timestamps in UTC format, we need to have these additional operations to convert the UTC time back to Local Time
                /*
                string tmp_timestamp = data["TransactionDate"].ToString();
                string tmp_cleanedtimestamp = tmp_timestamp.Replace("Timestamp: ", "");
                DateTime tmp_datetime_utc = DateTime.Parse(tmp_cleanedtimestamp);
                DateTime tmp_datetime_local = tmp_datetime_utc.ToLocalTime();
                */ //either this solution or just save it in string then convert back to DateTime
                DateTime tmp_date = DateTime.Parse(data["TransactionDate"].ToString());

                string tmp_transreason = data["TransactionReason"].ToString();
                                                                                                  //if use alternative solution replace with tmp_datetime_local
                UserTransaction tmp_transaction = new UserTransaction(tmp_amount, tmp_transreason, tmp_date);
                tmp_trans.AddTransaction(tmp_transaction);
            }
            Console.WriteLine("Data retrieved: \n");
            tmp_trans.DisplayTransactionHistory();
        }

        //method to retrive user items from database
        public async Task retrieveItem()
        {
            Query collectionQuery = database.Collection("user_items");
            QuerySnapshot allQuerySnapshot = await collectionQuery.GetSnapshotAsync();

            tmp_items.Clear();
            foreach (DocumentSnapshot documentSnapshot in allQuerySnapshot.Documents)
            {
                Dictionary<string, object> data = documentSnapshot.ToDictionary();

                string tmp_itemID = data["Item ID"].ToString();
                string tmp_itemName = data["Item Name"].ToString();
                int tmp_quantity = int.Parse(data["Quantity"].ToString());
                double tmp_price = double.Parse(data["Price"].ToString());

                Item tmp_item = new Item(tmp_itemName, tmp_quantity, tmp_price, tmp_itemID);
                tmp_items.AddItem(tmp_item);
            }
            Console.WriteLine("Data retrived: \n");
            tmp_items.DisplayInventory();
        }

        //retrieve single document == user detail
        public async Task retrieveUserData(string a_user_ID)
        {
            //shorthand way of writing, got it from documentation.
            DocumentReference docRef = database.Collection("user_detail").Document(a_user_ID);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            Dictionary<string, object> data = snapshot.ToDictionary();
                string tmp_userID = data["user_ID"].ToString();
                string tmp_email = data["email"].ToString();
                string tmp_password = data["password"].ToString();
                string tmp_name = data["name"].ToString();
                int tmp_age = int.Parse(data["age"].ToString());
            UserDetail tmp_userdetail = new UserDetail(tmp_email, tmp_password, tmp_userID, tmp_name, tmp_age);

            Console.WriteLine("Data of user ID: {0} has been successfully retrieved.", tmp_userdetail.getUserID());
            tmp_userdetail.DisplayUserData();
        }

        //delete transaction with reference to tick time AKA documentID
        public async Task deleteTransaction(string docID)
        {
            CollectionReference collectionRef = database.Collection("user_transactions");
            DocumentReference docRef = collectionRef.Document(docID);

            Console.WriteLine("Transaction with ID: {0} has been deleted", docRef.Id);
            await docRef.DeleteAsync();
        }
        //delete user data => usually used for updating settings
        public async Task deleteUserData(string a_user_ID)
        {
            DocumentReference docRef = database.Collection("user_detail").Document(a_user_ID);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            Console.WriteLine("User data with ID: {0} has been deleted", a_user_ID);
            await docRef.DeleteAsync();
        }
        //delete user item
        public async Task deleteAsset(string docID)
        {
            CollectionReference collectionRef = database.Collection("user_items");
            DocumentReference docRef = collectionRef.Document(docID);

            //getting the key value pairs of the item stored in database    
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            Item tmp_item = snapshot.ConvertTo<Item>();

            Console.WriteLine("Item with ID: {0} has been deleted", tmp_item.getItemID());
            await docRef.DeleteAsync();
        }


    }
}
