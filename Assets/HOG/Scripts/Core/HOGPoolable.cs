namespace Core
{
    public class HOGPoolable : HOGMonoBehaviour
    {
        public PoolNames poolName;

        public virtual void OnReturnedToPool()
        {
            this.gameObject.SetActive(false);
        }

        public virtual void OnTakenFromPool()
        {
            this.gameObject.SetActive(true);
        }

        public virtual void PreDestroy()
        {
        }
    }
}