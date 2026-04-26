using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using master;

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
}
