using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Exercise3
{
    
    public class BrickGame : MonoBehaviour
    {
        [GamePropertyAttribute]
        public int number= 4;
    }
    
    public class GamePropertyAttribute : PropertyAttribute { }

    [CustomPropertyDrawer(typeof(GamePropertyAttribute))]
    public class BrickGamePropertyDrawer : PropertyDrawer
    {
        public List<Break> breaks;
        public Ball ball;
        public Player player;

        public int Xmove;
        public int Ymove;

        public Vector2Int ballMovement;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Texture colorTexture = EditorGUIUtility.whiteTexture;
 
        
            if (breaks == null)
            {
                NewGameStart(position, property);

            }
            for (var i = 0; i < property.intValue; i++)
            {
                breaks[i].DrawBreak( colorTexture);
            }
            ball.DrawBall( colorTexture);

            ball.BallMovement();
            
            var e = Event.current;

            if (e.type == EventType.KeyDown)
            {


                if (e.keyCode ==KeyCode.RightArrow && player.rect.x < position.width-80)
                {
                    player.rect.x += 5;
                }

                if (e.keyCode == KeyCode.LeftArrow && player.rect.x >0)
                {
                    player.rect.x -= 5;
                }

                if ( ball.rect.y < 0)
                {
                    ball.ChangeDirectiony();
                }
                if (ball.rect.x > position.width || ball.rect.x < 0)
                {
                    ball.ChangeDirectionX();
                }
                if (ball.rect.Overlaps(player.rect))
                {
                    Debug.Log("collision");
                    ball.ChangeDirectiony();
                    ball.rect.y -= 10;
                }
           
                foreach (var t in breaks.Where(t => ball.rect.Overlaps(t.rect)&&t.color == Color.red))
                {
                    Debug.Log("collision"); 
                    ball.ChangeDirectiony();
                    t.color = Color.black;
                    ball.rect.y += 10;
                }
                if (ball.rect.y > position.height )
                {
                    NewGameStart(position, property);
                }
            
                e.Use();
            }

            player.DrawPlayer( colorTexture);
        
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight*20;
        }
        public class Break
        {
            public Rect rect;
            public Color color;
            public Break(Rect _rect,Color _color)
            {
                rect =  _rect;
                color = _color;
            }

        
            public void DrawBreak( Texture colorTexture)
            {
                GUI.color = color;
                GUI.DrawTexture(rect, colorTexture);
            }
        }

        public class Ball
        {
            public Rect rect;


            public int MoveSizeX;
            public int MoveSizeY;
            public Ball(Rect _rect)
            {
                rect = _rect;


                MoveSizeX = 5;
                MoveSizeY = 5;
            }
            public void DrawBall(Texture colorTexture)
            {
                rect.x = rect.x;
                rect.y = rect.y;
                GUI.color = Color.blue;
                GUI.DrawTexture(rect, colorTexture);
            }
            public void BallMovement()
            {
                rect.x += MoveSizeX;
                rect.y += MoveSizeY;
            }

            public void ChangeDirectionX()
            {
                MoveSizeX *= -1;
            }
            public void ChangeDirectiony()
            {
                MoveSizeY *= -1;
            }

        }

        public class Player
        {
            public Rect rect;
            public Player(Rect _rect)
            {
                rect = _rect;
            }
            public void DrawPlayer( Texture colorTexture)
            {
                GUI.color = Color.black;
                GUI.DrawTexture(rect, colorTexture);
            }


        }

        private void NewGameStart(Rect position, SerializedProperty property)
        {
            Xmove = 5;
            Ymove = 5;
            ballMovement = new Vector2Int((int)position.x + 50, (int)position.y + 50);
            player = new Player(new Rect(position.x, position.y + position.height - 20, 80, 20));
            ball = new Ball(new Rect(ballMovement.x, ballMovement.y, 20, 20));
            property.intValue = 8;
            breaks = new List<Break>();
            for (var i = 0; i < property.intValue; i++)
            {
                breaks.Add(new Break(new Rect(position.x + i * 80, position.y + 20, 70, 20), Color.red));
            }

        }

    
    }
}