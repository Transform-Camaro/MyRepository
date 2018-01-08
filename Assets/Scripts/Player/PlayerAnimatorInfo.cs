using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorInfo
{
    public static string bool_Idle = "isLeft";


    public static string Left_Idle = "leftIdle";
    public static string Right_Idle = "rightIdle";
    public static string Jump_Up = "up";
    public static string Jump_Down = "down";
    public static string Jump_Right = "right";
    public static string Jump_Left = "left";

    public static string posx = "x";
    public static string posy = "y";
    public static string facex = "facex";
    public static string hurt_type = "enemytype";

   // public static string state_type = "statetype";

	public static string state_Jump = "jump";
	public static string state_Hurt = "hurt";
	public static string state_Die = "die";

    public static string Die = "die";

    public static string Hurt_Lightning = "lightning";
    public static string Hurt_Wind = "wind";
    public static string Hurt_Rock = "rock";
    public static string Hurt_Fire = "fire";


    public static int Jump_Up_Length = 218 - 188;
    public static int Jump_Down_Length = 266 - 236;
    public static int Jump_Right_Length = 314 - 284;
    public static int Jump_Left_Length = 362 - 332;

    public static int Hurt_Fire_Length = 82 - 62;
    public static int Hurt_Lightning_Length = 59 - 49;
    public static int Hurt_Wind_Length = 127 - 85;
    public static int Hurt_Rock_Length = 430 - 380;
}
