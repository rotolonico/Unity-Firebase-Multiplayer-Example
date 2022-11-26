using System.Collections;
using System.Threading.Tasks;
using Serializables;
using UnityEngine;

namespace Managers
{
    public class MainManager : MonoBehaviour
    {
        public static MainManager Instance;

        public MatchmakingManager matchmakingManager;
        public GameManager gameManager;

        public string currentLocalPlayerName;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            matchmakingManager = GetComponent<MatchmakingManager>();
            gameManager = GetComponent<GameManager>();
        }
        
        public IEnumerator WaitForTask(Task task)
        {
            yield return new WaitUntil(() => task.IsCompleted);
        }
        
        public async Task<Task> WaitForTaskAsync(Task task)
        {
            await Task.Run(async () =>
            {
                while (task.IsCompleted) await Task.Delay(100);
            });
            
            return task;
        }
    }
}