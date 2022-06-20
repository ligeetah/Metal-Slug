﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using FrameWork.Movement;
using FrameWork.ENUM;
using FrameWork.Fire;
using FrameWork.ProgressB;
using FrameWork.Collision;
namespace FrameWork.GameF
{
    public class Game:IGame
    {
        private static Point boundary;
        private List<GameObject> Gameobjects;
        private List<CollisionClass> collisions;
        public Game(Point boundary)
        {
            this.Boundary=boundary;
            Gameobjects = new List<GameObject>();
            collisions = new List<CollisionClass>();
        }
        //public static List<GameObject> Gameobjects { get => gameobjects; set => gameobjects = value; }x
        public Point Boundary { get => boundary; set => boundary = value; }

        public static event EventHandler onGameObjectAdded;
        public static event EventHandler onPlayerHit;
        public static event EventHandler onEnemyHit;
        public event EventHandler removeObject;//remove from controls
        public void RemoveGameObject(GameObject a)
        {
            removeObject?.Invoke(a, EventArgs.Empty);
        }

        public void AddGameObject(Image img,ObjectTypes otype, int top,int left,int width,int height,IMovement movement,Ifire ifire,IProgressBar ibar)
        {
            GameObject ob = new GameObject(img,otype,top,left,width,height,movement,ifire,ibar);
            Gameobjects.Add(ob);
            onGameObjectAdded?.Invoke(ob.Pb, EventArgs.Empty);
        }
        public void AddGameObject(GameObject a)
        {
            Gameobjects.Add(a);
            onGameObjectAdded?.Invoke(a.Pb, EventArgs.Empty);
        }
        public void update()
        {
            detectCollision();
            fire(this);
            update_progressbar();
            move(this);
        }
        public void move(IGame igame)
        {
            for (int i = 0; i < Gameobjects.Count; i++)
            {
                Gameobjects[i].move(Gameobjects,igame);
            }
        }
        public void scroll()
        {
            for (int i = 0; i < Gameobjects.Count; i++)
            {
                Gameobjects[i].scroll();
            }
        }
        public void fire(IGame igame)
        {
            for (int i = 0; i < Gameobjects.Count; i++)
            {
                Gameobjects[i].fire(Gameobjects[i].Pb,igame);
            }
        }
        public void update_progressbar()
        {
            for (int i = 0; i < Gameobjects.Count; i++)
            {
                Gameobjects[i].updateprogressbar();
            }
        }
        public void RaisePlayerHitEvent(GameObject obj)
        {
            onPlayerHit?.Invoke(obj, EventArgs.Empty);
        }
        public void RaiseEnemyHitEvent(GameObject obj)
        {
            onEnemyHit?.Invoke(obj, EventArgs.Empty);
        }
        public void RaisePlayerBulletRemove(GameObject obj)
        {
            removeObject?.Invoke(obj, EventArgs.Empty);
        }
       
        public void detectCollision()
        {
            for (int i = Gameobjects.Count-1; i >= 0; i--)
            {
                for (int m = Gameobjects.Count-1; m >=0; m--)
                {
                    if (Gameobjects[i].Pb.Bounds.IntersectsWith(Gameobjects[m].Pb.Bounds))
                    {
                        foreach(CollisionClass c in collisions)
                        {
                            if (Gameobjects[i].Otype == c.G1&& Gameobjects[m].Otype==c.G2)
                            {
                                c.Behavior.performAction(this, Gameobjects[i], Gameobjects[m]);
                            }
                        }
                    }
                }
            }
        }
        public void addCollision(CollisionClass c)
        {
            collisions.Add(c);
        }
        public int get_lowest_floor()
        { 
            int low = 1200;
            for (int i = 0; i < Gameobjects.Count; i++)
            {
                if (Gameobjects[i].Otype==ENUM.ObjectTypes.floor && Gameobjects[i].Pb.Top >= 0 && Gameobjects[i].Pb.Top < low)
                {
                    low = Gameobjects[i].Pb.Top;
                }
            }
            return low;
        }
        public void RemoveUnwanted()
        {
            for (int i = Gameobjects.Count-1; i >=0; i--)
            {
                if (Gameobjects[i].Pb.Top>boundary.X)
                {
                    removeObject?.Invoke(Gameobjects[i], EventArgs.Empty);
                    Gameobjects.RemoveAt(i);
                }
            }
        }
        public void removeGameObjectfromlist(GameObject a)
        {
            Gameobjects.Remove(a);
        }
    }

}
