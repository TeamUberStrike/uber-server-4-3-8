using System;
using System.Collections.Generic;
using Cmune.Realtime.Photon.Server;
using ExitGames.Concurrency.Fibers;
using UberStrike.Realtime.Common;

namespace UberStrike.Realtime.Photon.GameServer
{
    public class PowerupManager
    {
        private CmuneRoom _room;
        private FpsGameMode _game;

        private List<Powerup> _powerups;

        public PowerupManager(FpsGameMode game, CmuneRoom room)
        {
            _room = room;
            _game = game;

            _powerups = new List<Powerup>();
        }

        public void SetPowerupCount(List<byte> respawnDurations)
        {
            foreach (Powerup p in _powerups)
                p.Dispose();

            _powerups.Clear();

            for (int i = 0; i < respawnDurations.Count; i++)
            {
                _powerups.Add(new Powerup(i, respawnDurations[i]));
            }
        }

        public void PickPowerup(int id)
        {
            if (id >= 0 && id < _powerups.Count)
            {
                _powerups[id].Pick(_game, _room.ExecutionFiber);
            }
            else
            {
                _game.SendMethodToAll(FpsGameRPC.PowerUpPicked, id, (byte)1);
            }
        }

        public List<int> GetState()
        {
            List<int> inactivePickups = new List<int>(_powerups.Count);

            for (int i = 0; i < _powerups.Count; i++)
            {
                if (_powerups[i].IsPicked)
                    inactivePickups.Add(i);
            }

            return inactivePickups;
        }

        public void Reset()
        {
            foreach (Powerup p in _powerups)
            {
                if (p.IsPicked)
                {
                    p.Reset();
                    _game.SendMethodToAll(FpsGameRPC.PowerUpPicked, p.Id, (byte)0);
                }
            }
        }
    }

    public class Powerup : IDisposable
    {
        private int _id;
        private byte _respawnDuration;

        private bool _isPicked;
        private IDisposable _respawnRoutine;

        public int Id
        {
            get { return _id; }
        }

        public bool IsPicked
        {
            get { return _isPicked; }
        }

        public Powerup(int id, byte respawnDuration)
        {
            _id = id;
            _respawnDuration = respawnDuration;

            Reset();
        }

        public void Reset()
        {
            if (_isPicked)
                _isPicked = false;

            if (_respawnRoutine != null)
            {
                _respawnRoutine.Dispose();
                _respawnRoutine = null;
            }
        }

        public void Pick(FpsGameMode game, PoolFiber fiber)
        {
            lock (this)
            {
                if (_respawnRoutine == null && !_isPicked)
                {
                    _isPicked = true;

                    _respawnRoutine = fiber.Schedule(
                        () =>
                        {
                            if (_isPicked)
                                game.SendMethodToAll(FpsGameRPC.PowerUpPicked, _id, (byte)0);

                            _isPicked = false;
                            _respawnRoutine = null;
                        },
                        _respawnDuration * 1000);

                    game.SendMethodToAll(FpsGameRPC.PowerUpPicked, _id, (byte)1);
                }
            }
        }

        public void Dispose()
        {
            if (_respawnRoutine != null)
                _respawnRoutine.Dispose();
        }
    }
}
