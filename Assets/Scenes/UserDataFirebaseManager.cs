using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using master;
using Firebase.Database;

public class UserDataFirebaseManager : SingletonDDOL<UserDataFirebaseManager>
{
    private FirebaseFirestore db;
    private const string COLLECTION_NAME = "UserData";

    public string CurrentUserId { get; private set; }
    public string CurrentUserName { get; private set; }

    private void Start()
    {
        // Khởi tạo db khi script bắt đầu
        db = FirebaseFirestore.DefaultInstance;

        CheckAndInitializeUser();
        //AddTestUsersSequential();
        // string myId = PlayerPrefs.GetString("PlayerID", "-1");
        // if (myId != "-1") AddFriend("10000011", "10000010");
    }


    /// <summary>
    /// Lắng nghe các lời mời kết bạn mới thông qua Realtime Database. Khi có lời mời mới, sẽ nhận được callback trong OnFriendRequestAdded để xử lý (ví dụ: hiển thị popup thông báo, load info người gửi từ Firestore, hiện nút Accept/Reject, v.v...). Lưu ý: đây chỉ là phần lắng nghe realtime để cập nhật UI ngay khi có lời mời mới. Logic lưu trữ và quản lý lời mời vẫn nên được thực hiện trong Firestore để đảm bảo tính nhất quán và dễ dàng truy vấn.
    /// </summary>
    private DatabaseReference friendRequestRef;

    public void StartListeningFriendRequest(string myUserId)
    {
        var db = FirebaseDatabase.GetInstance("https://magicwaterpuzzle-default-rtdb.asia-southeast1.firebasedatabase.app");

        friendRequestRef = db.GetReference("friend_requests").Child(myUserId);

        friendRequestRef.ChildAdded += OnFriendRequestAdded;

        Debug.Log("[Realtime] Listening friend requests...");
    }

    private void OnFriendRequestAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.Snapshot.Exists)
        {
            string fromUserId = args.Snapshot.Key;

            Debug.Log($"[Realtime] New friend request from: {fromUserId}");

            // 👉 TODO:
            // - Hiện popup UI
            // - Load info user từ Firestore
            // - Hiện nút Accept / Reject
            UIManager.Instance.NotifyContent($"Bạn có một lời mời kết bạn mới từ {fromUserId}!");
            // 👉 XÓA NGAY để tránh trigger lại
            friendRequestRef.Child(fromUserId).RemoveValueAsync();
        }
    }

    public void AddTestUsersSequential()
    {
        CreateUserRecursive(0);
    }

    private void CreateUserRecursive(int count)
    {
        if (count >= 10) return;

        GenerateUniqueId(newId =>
        {
            Dictionary<string, object> data = new Dictionary<string, object>
            {
            { "Id", newId },
            { "Name", "Bot_" + newId },
            { "Coin", UnityEngine.Random.Range(0, 10000) },
            { "Level", UnityEngine.Random.Range(1, 50) },
            { "Heart", UnityEngine.Random.Range(1, 5) },
            { "Frame", 0 },
            { "CreatedAt", FieldValue.ServerTimestamp }
            };

            SaveUserData(newId, data, success =>
            {
                if (success)
                {
                    Debug.Log($"Created test user: {newId}");
                    CreateUserRecursive(count + 1); // gọi tiếp
                }
            });
        });
    }

    /// <summary>
    /// Kiểm tra người chơi ở local, nếu chưa có thì tạo mới ID (bắt đầu từ 10000000 không trùng lặp) và lưu vào Firebase.
    /// </summary>
    public void CheckAndInitializeUser()
    {
        if (PlayerPrefs.HasKey("PlayerID"))
        {
            CurrentUserId = PlayerPrefs.GetString("PlayerID");
            CurrentUserName = PlayerPrefs.GetString("PlayerName");
            Debug.Log($"[LocalUser] Welcome back, {CurrentUserName} (ID: {CurrentUserId})");
            StartListeningFriendRequest(CurrentUserId);
        }
        else
        {
            Debug.Log("[LocalUser] No local data found. Generating new user ID...");
            GenerateUniqueId(newId =>
            {
                CurrentUserId = newId;
                CurrentUserName = "Player" + newId;

                PlayerPrefs.SetString("PlayerID", CurrentUserId);
                PlayerPrefs.SetString("PlayerName", CurrentUserName);
                PlayerPrefs.Save();

                Debug.Log($"[LocalUser] Created new user locally: {CurrentUserName} (ID: {CurrentUserId})");

                // Lưu dữ liệu ban đầu của người chơi lên Firebase
                Dictionary<string, object> initialData = new Dictionary<string, object>
                {
                    { "Id", CurrentUserId },
                    { "Name", CurrentUserName },
                    { "Coin", 30 },
                    { "Level", 1 },
                    { "Heart", 5 },
                    { "Frame", 0 },
                    { "CreatedAt", FieldValue.ServerTimestamp }
                };
                SaveUserData(CurrentUserId, initialData);
                // lưu data heart
                PlayerPrefs.SetInt("Hearts", 5);
                PlayerPrefs.SetFloat("Timer", 0);
                PlayerPrefs.SetString("LastQuitTime", DateTime.Now.ToBinary().ToString());
                PlayerPrefs.Save();
                StartListeningFriendRequest(CurrentUserId);
            });
        }
    }

    private void GenerateUniqueId(Action<string> onComplete)
    {
        if (db == null) db = FirebaseFirestore.DefaultInstance;

        DocumentReference counterRef = db.Collection("ServerConfigs").Document("UserCounter");

        db.RunTransactionAsync(transaction =>
        {
            return transaction.GetSnapshotAsync(counterRef).ContinueWith(task =>
            {
                DocumentSnapshot snapshot = task.Result;
                long nextId = 10000000;

                if (snapshot.Exists && snapshot.TryGetValue("LastUserID", out long lastId))
                {
                    if (lastId >= 10000000)
                    {
                        nextId = lastId + 1;
                    }
                }

                Dictionary<string, object> updates = new Dictionary<string, object>
                {
                    { "LastUserID", nextId }
                };

                transaction.Set(counterRef, updates, SetOptions.MergeAll);
                return nextId;
            });
        }).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                onComplete?.Invoke(task.Result.ToString());
            }
            else
            {
                Debug.LogWarning($"[Firebase] Failed to generate sequential ID: {task.Exception}. Using fallback timestamp ID.");
                // Fallback nếu offline hoặc lỗi mạng: dùng timestamp để có ID duy nhất (>= 10000000)
                long fallbackId = 10000000 + (long)(DateTime.UtcNow - new DateTime(2024, 1, 1)).TotalSeconds;
                onComplete?.Invoke(fallbackId.ToString());
            }
        });
    }

    /// <summary>
    /// Lưu hoặc cập nhật data cho một người dùng (documentId thường là Id người dùng)
    /// </summary>
    public void SaveUserData(string documentId, Dictionary<string, object> data, Action<bool> onComplete = null)
    {
        if (db == null) db = FirebaseFirestore.DefaultInstance;
        if (db == null)
        {
            Debug.LogError("[Firebase] Firestore is not initialized.");
            onComplete?.Invoke(false);
            return;
        }

        DocumentReference docRef = db.Collection(COLLECTION_NAME).Document(documentId);

        // Dùng SetOptions.MergeAll để cập nhật các trường được truyền vào, giữ nguyên các trường khác
        docRef.SetAsync(data, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log($"[Firebase] UserData {documentId} saved successfully!");
                onComplete?.Invoke(true);
            }
            else
            {
                Debug.LogError($"[Firebase] Failed to save UserData {documentId}: {task.Exception}");
                onComplete?.Invoke(false);
            }
        });
    }

    /// <summary>
    /// Lấy data của một người dùng
    /// </summary>
    public void GetUserData(string documentId, Action<Dictionary<string, object>> onComplete)
    {
        if (db == null) db = FirebaseFirestore.DefaultInstance;
        if (db == null)
        {
            Debug.LogError("[Firebase] Firestore is not initialized.");
            onComplete?.Invoke(null);
            return;
        }

        DocumentReference docRef = db.Collection(COLLECTION_NAME).Document(documentId);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    Dictionary<string, object> data = snapshot.ToDictionary();
                    Debug.Log($"[Firebase] UserData {documentId} retrieved successfully!");
                    onComplete?.Invoke(data);
                }
                else
                {
                    Debug.LogWarning($"[Firebase] Document {documentId} does not exist!");
                    onComplete?.Invoke(null);
                }
            }
            else
            {
                Debug.LogError($"[Firebase] Failed to get UserData {documentId}: {task.Exception}");
                onComplete?.Invoke(null);
            }
        });
    }

    /// <summary>
    /// Xoá data của một người dùng
    /// </summary>
    public void DeleteUserData(string documentId, Action<bool> onComplete = null)
    {
        if (db == null) db = FirebaseFirestore.DefaultInstance;
        if (db == null)
        {
            Debug.LogError("[Firebase] Firestore is not initialized.");
            onComplete?.Invoke(false);
            return;
        }

        DocumentReference docRef = db.Collection(COLLECTION_NAME).Document(documentId);
        docRef.DeleteAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log($"[Firebase] UserData {documentId} deleted successfully!");
                onComplete?.Invoke(true);
            }
            else
            {
                Debug.LogError($"[Firebase] Failed to delete UserData {documentId}: {task.Exception}");
                onComplete?.Invoke(false);
            }
        });
    }

    public void GetAllUsers(Action<List<Dictionary<string, object>>> onComplete)
    {
        if (db == null) db = FirebaseFirestore.DefaultInstance;

        db.Collection(COLLECTION_NAME).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                QuerySnapshot snapshot = task.Result;

                List<Dictionary<string, object>> users = new List<Dictionary<string, object>>();

                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    if (doc.Exists)
                    {
                        users.Add(doc.ToDictionary());
                    }
                }

                Debug.Log($"[Firebase] Loaded {users.Count} users");
                onComplete?.Invoke(users);
            }
            else
            {
                Debug.LogError($"[Firebase] Failed to get users: {task.Exception}");
                onComplete?.Invoke(null);
            }
        });
    }

    /// <summary>
    /// Thêm bạn bè giữa 2 người dùng (tạo document trong subcollection "Friends" của mỗi người, chứa Id của người kia)
    /// </summary>
    /// <param name="userAId"></param>
    /// <param name="userBId"></param>
    /// <param name="onComplete"></param>
    public void AddFriend(string userAId, string userBId, Action<bool> onComplete = null)
    {
        if (db == null) db = FirebaseFirestore.DefaultInstance;

        WriteBatch batch = db.StartBatch();

        DocumentReference userARef = db.Collection(COLLECTION_NAME)
                                       .Document(userAId)
                                       .Collection("Friends")
                                       .Document(userBId);

        DocumentReference userBRef = db.Collection(COLLECTION_NAME)
                                       .Document(userBId)
                                       .Collection("Friends")
                                       .Document(userAId);

        Dictionary<string, object> dataA = new Dictionary<string, object>
    {
        { "Id", userBId },
        { "CreatedAt", FieldValue.ServerTimestamp }
    };

        Dictionary<string, object> dataB = new Dictionary<string, object>
    {
        { "Id", userAId },
        { "CreatedAt", FieldValue.ServerTimestamp }
    };

        batch.Set(userARef, dataA);
        batch.Set(userBRef, dataB);

        batch.CommitAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log($"[Friend] {userAId} and {userBId} are now friends!");
                onComplete?.Invoke(true);
            }
            else
            {
                Debug.LogError($"[Friend] AddFriend failed: {task.Exception}");
                onComplete?.Invoke(false);
            }
        });
    }
    /// <summary>
    /// Gửi lời mời kết bạn từ fromUserId đến toUserId. Lưu ý: để đơn giản, ở đây chúng ta chỉ lưu một document trong Firestore và một node trong Realtime Database để thông báo cho người nhận. Trong thực tế, bạn có thể muốn lưu thêm thông tin như trạng thái (đang chờ, đã chấp nhận, đã từ chối), thời gian gửi, v.v... và xử lý logic phức tạp hơn.
    /// </summary>
    /// <param name="fromUserId"></param>
    /// <param name="toUserId"></param>
    public void SendFriendRequest(string fromUserId, string toUserId, Action<bool> onComplete = null)
    {
        if (db == null) db = FirebaseFirestore.DefaultInstance;

        var requestData = new Dictionary<string, object>
    {
        { "FromUserId", fromUserId },
        { "ToUserId", toUserId },
        {"FromUserName", CurrentUserName},
        { "Status", "Pending" },
        { "CreatedAt", FieldValue.ServerTimestamp }
    };

        var firestoreTask = db.Collection("FriendRequests").AddAsync(requestData);

        var realtimeRef = FirebaseDatabase
    .GetInstance("https://magicwaterpuzzle-default-rtdb.asia-southeast1.firebasedatabase.app")
    .RootReference;
        var realtimeTask = realtimeRef.Child("friend_requests")
                                      .Child(toUserId)
                                      .Child(fromUserId)
                                      .SetValueAsync(true);

        // Đợi cả 2 hoàn thành
        Task.WhenAll(firestoreTask, realtimeTask)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                {
                    Debug.Log("[FriendRequest] Sent successfully");
                    onComplete?.Invoke(true);
                }
                else
                {
                    Debug.LogError($"[FriendRequest] Failed: {task.Exception}");
                    onComplete?.Invoke(false);
                }
            });
    }

    /// <summary>
    /// Chấp nhận lời mời kết bạn từ fromUserId đến toUserId. Logic đơn giản là gọi AddFriend để tạo mối quan hệ bạn bè, sau đó gửi thông báo realtime để cập nhật UI cho người gửi. Lưu ý: trong thực tế, bạn nên cập nhật trạng thái của lời mời trong Firestore (ví dụ: từ "Pending" sang "Accepted") và xử lý các trường hợp như từ chối, hủy lời mời, v.v...
    /// </summary>
    /// <param name="fromUserId"></param>
    /// <param name="toUserId"></param>
    public void AcceptFriend(string fromUserId, string toUserId)
    {
        AddFriend(fromUserId, toUserId);

        // notify lại
        var realtimeRef = FirebaseDatabase
    .GetInstance("https://magicwaterpuzzle-default-rtdb.asia-southeast1.firebasedatabase.app")
    .RootReference;
        realtimeRef.Child("friend_accept")
                   .Child(fromUserId)
                   .Child(toUserId)
                   .SetValueAsync(true);
    }
    /// <summary>
    /// Lấy danh sách bạn bè của một người dùng (truy vấn subcollection "Friends", sau đó lấy data user thật của từng friendId)
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="onComplete"></param>
    public void GetFriendsList(string userId, Action<List<Dictionary<string, object>>> onComplete)
    {
        if (db == null) db = FirebaseFirestore.DefaultInstance;

        CollectionReference friendsRef = db.Collection(COLLECTION_NAME)
                                           .Document(userId)
                                           .Collection("Friends");

        friendsRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("[Friend] Failed to get friends list");
                onComplete?.Invoke(null);
                return;
            }

            QuerySnapshot snapshot = task.Result;

            List<Dictionary<string, object>> friendsData = new List<Dictionary<string, object>>();

            if (snapshot.Count == 0)
            {
                onComplete?.Invoke(friendsData);
                return;
            }

            int remaining = snapshot.Count;

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                string friendId = doc.Id;

                // Lấy data user thật
                GetUserData(friendId, userData =>
                {
                    if (userData != null)
                    {
                        friendsData.Add(userData);
                    }

                    remaining--;

                    if (remaining == 0)
                    {
                        Debug.Log($"[Friend] Loaded {friendsData.Count} friends");
                        onComplete?.Invoke(friendsData);
                    }
                });
            }
        });
    }

    /// <summary>
    /// Tìm người dùng theo Id chính xác hoặc prefix (để add friend). Lưu ý: tìm theo prefix có thể trả về nhiều kết quả, nên UI cần hiển thị danh sách để người chơi chọn đúng người muốn kết bạn.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="onComplete"></param>

    public void SearchUserByExactId(string userId, Action<Dictionary<string, object>> onComplete)
    {
        if (db == null) db = FirebaseFirestore.DefaultInstance;

        db.Collection(COLLECTION_NAME)
          .Document(userId)
          .GetSnapshotAsync()
          .ContinueWithOnMainThread(task =>
          {
              if (task.IsCompleted && !task.IsFaulted)
              {
                  DocumentSnapshot snapshot = task.Result;
                  if (snapshot.Exists)
                  {
                      onComplete?.Invoke(snapshot.ToDictionary());
                  }
                  else
                  {
                      onComplete?.Invoke(null);
                  }
              }
              else
              {
                  Debug.LogError($"Search failed: {task.Exception}");
                  onComplete?.Invoke(null);
              }
          });
    }


    /// <summary>
    /// Tìm người dùng theo prefix của Id (ví dụ: nhập "1000001" sẽ trả về tất cả người dùng có Id bắt đầu bằng "1000001" như "10000010", "10000011",...). Lưu ý: tìm theo prefix có thể trả về nhiều kết quả, nên UI cần hiển thị danh sách để người chơi chọn đúng người muốn kết bạn.
    /// </summary>
    /// <param name="keyword"></param>
    /// <param name="onComplete"></param>
    public void SearchUsersByIdPrefix(string keyword, Action<List<Dictionary<string, object>>> onComplete)
    {
        if (db == null) db = FirebaseFirestore.DefaultInstance;

        string end = keyword + "\uf8ff"; // trick để query prefix

        db.Collection(COLLECTION_NAME)
          .WhereGreaterThanOrEqualTo("Id", keyword)
          .WhereLessThanOrEqualTo("Id", end)
          .GetSnapshotAsync()
          .ContinueWithOnMainThread(task =>
          {
              if (task.IsCompleted && !task.IsFaulted)
              {
                  List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

                  foreach (var doc in task.Result.Documents)
                  {
                      result.Add(doc.ToDictionary());
                  }

                  Debug.Log($"[Search] Found {result.Count} users");
                  onComplete?.Invoke(result);
              }
              else
              {
                  Debug.LogError($"Search failed: {task.Exception}");
                  onComplete?.Invoke(null);
              }
          });
    }

    /// <summary>
    /// Lấy danh sách lời mời kết bạn đến cho người dùng (truy vấn collection "FriendRequests" với điều kiện ToUserId = myUserId và Status = "Pending"). Lưu ý: bạn có thể muốn mở rộng logic này để phân biệt giữa các trạng thái khác nhau của lời mời (ví dụ: đã chấp nhận, đã từ chối) và xử lý tương ứng trong UI.
    /// </summary>
    /// <param name="myUserId"></param>
    /// <param name="onComplete"></param>

    public void GetMyFriendRequests(string myUserId, Action<List<Dictionary<string, object>>> onComplete)
    {
        if (db == null) db = FirebaseFirestore.DefaultInstance;

        db.Collection("FriendRequests")
          .WhereEqualTo("ToUserId", myUserId)
          .WhereEqualTo("Status", "Pending") // chỉ lấy lời mời đang chờ (tuỳ bạn)
          .GetSnapshotAsync()
          .ContinueWithOnMainThread(task =>
          {
              if (task.IsCompleted && !task.IsFaulted)
              {
                  List<Dictionary<string, object>> requests = new List<Dictionary<string, object>>();

                  foreach (var doc in task.Result.Documents)
                  {
                      if (doc.Exists)
                      {
                          requests.Add(doc.ToDictionary());
                      }
                  }

                  Debug.Log($"[FriendRequest] Loaded {requests.Count} requests");
                  onComplete?.Invoke(requests);
              }
              else
              {
                  Debug.LogError($"[FriendRequest] Load failed: {task.Exception}");
                  onComplete?.Invoke(null);
              }
          });
    }


    /// <summary>
    /// Từ chối lời mời kết bạn từ fromUserId đến toUserId. Logic đơn giản là xoá document lời mời trong Firestore và gửi thông báo realtime để cập nhật UI cho người gửi. Lưu ý: trong thực tế, bạn nên cập nhật trạng
    /// trạng của lời mời trong Firestore (ví dụ: từ "Pending" sang "Declined") thay vì xoá hẳn, để có thể lưu lại lịch sử và xử lý các trường hợp phức tạp hơn.
    /// </summary> <param name="fromUserId"></param>
    /// <param name="toUserId"></param>
    /// <param name="onComplete"></param>
    public void DeclineFriendRequest(string fromUserId, string toUserId, Action<bool> onComplete = null)
    {
        if (db == null) db = FirebaseFirestore.DefaultInstance;

        // Xoá document lời mời trong Firestore
        db.Collection("FriendRequests")
          .WhereEqualTo("FromUserId", fromUserId)
          .WhereEqualTo("ToUserId", toUserId)
          .GetSnapshotAsync()
          .ContinueWithOnMainThread(task =>
          {
              if (task.IsCompleted && !task.IsFaulted)
              {
                  foreach (var doc in task.Result.Documents)
                  {
                      doc.Reference.DeleteAsync();
                  }

                  Debug.Log("[FriendRequest] Declined successfully");
                  onComplete?.Invoke(true);
              }
              else
              {
                  Debug.LogError($"[FriendRequest] Decline failed: {task.Exception}");
                  onComplete?.Invoke(false);
              }
          });

        // notify lại
        var realtimeRef = FirebaseDatabase
    .GetInstance("https://magicwaterpuzzle-default-rtdb.asia-southeast1.firebasedatabase.app")
    .RootReference;
        realtimeRef.Child("friend_decline")
                   .Child(fromUserId)
                   .Child(toUserId)
                   .SetValueAsync(true);
    }


    ///accept friend request
    public void AcceptFriendRequest(string fromUserId, string toUserId, Action<bool> onComplete = null)
    {
        AddFriend(fromUserId, toUserId, success =>
        {
            if (success)
            {
                // Sau khi đã thêm bạn thành công, xoá lời mời trong Firestore
                db.Collection("FriendRequests")
                  .WhereEqualTo("FromUserId", fromUserId)
                  .WhereEqualTo("ToUserId", toUserId)
                  .GetSnapshotAsync()
                  .ContinueWithOnMainThread(task =>
                  {
                      if (task.IsCompleted && !task.IsFaulted)
                      {
                          foreach (var doc in task.Result.Documents)
                          {
                              doc.Reference.DeleteAsync();
                          }

                          Debug.Log("[FriendRequest] Accepted successfully");
                          onComplete?.Invoke(true);
                      }
                      else
                      {
                          Debug.LogError($"[FriendRequest] Accept failed: {task.Exception}");
                          onComplete?.Invoke(false);
                      }
                  });

                // notify lại
                var realtimeRef = FirebaseDatabase
            .GetInstance("https://magicwaterpuzzle-default-rtdb.asia-southeast1.firebasedatabase.app")
            .RootReference;
                realtimeRef.Child("friend_accept")
                           .Child(fromUserId)
                           .Child(toUserId)
                           .SetValueAsync(true);
            }
            else
            {
                onComplete?.Invoke(false);
            }
        });

        // notify lại
        var realtimeRef = FirebaseDatabase.GetInstance("https://magicwaterpuzzle-default-rtdb.asia-southeast1.firebasedatabase.app").RootReference;
        realtimeRef.Child("friend_decline")
                   .Child(fromUserId)
                   .Child(toUserId)
                   .SetValueAsync(true);
    }

}
