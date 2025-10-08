
namespace UberStrike.Realtime.Photon.GameServer
{
    public class SpawnPointManager
    {
        private int _currentIndex = 0;
        public int[] _spawnPoints;
        private int count;

        public SpawnPointManager(int count)
        {
            this.count = count;
            Scramble();
        }

        public void Scramble()
        {
            int random = new System.Random().Next(0, int.MaxValue);
            this._spawnPoints = CreatePermutation(random, this.count);
        }

        public int GetSpawnPoint()
        {
            if (this._spawnPoints.Length > 1)
            {
                int idx = ++_currentIndex % this._spawnPoints.Length;
                return this._spawnPoints[idx];
            }
            else
            {
                return 0;
            }
        }

        private static int[] CreatePermutation(int k, int n)
        {
            int[] points = new int[n];

            if (n > 0)
            {
                // Step #1 - Find factoradic of k
                int[] factoradic = new int[n];

                for (int j = 1; j <= n; ++j)
                {
                    factoradic[n - j] = k % j;
                    k /= j;
                }

                // Step #2 - Convert factoradic to permuatation
                int[] temp = new int[n];

                for (int i = 0; i < n; ++i)
                {
                    temp[i] = ++factoradic[i];
                }

                points[n - 1] = 1;  // right-most element is set to 1.

                for (int i = n - 2; i >= 0; --i)
                {
                    points[i] = temp[i];
                    for (int j = i + 1; j < n; ++j)
                    {
                        if (points[j] >= points[i])
                            ++points[j];
                    }
                }

                for (int i = 0; i < n; ++i)  // put in 0-based form
                {
                    --points[i];
                }
            }

            return points;
        }
    }
}