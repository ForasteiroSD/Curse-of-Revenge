using UnityEngine;

namespace Utils {
    public static class Constants {
        //Hierarchy Names
        public const string HIERARCHY_PLAYER = "Adventurer";
        public const string HIERARCHY_CAMERA_CONTROLLER = "CameraController";
        public const string HIERARCHY_CINEMACHINE_CAMERA = "CinemachineCamera";

        //Animator
        public const string ANIM_IS_RUNNING = "IsRunning";
        public const string ANIM_IS_FALLING = "IsFalling";
        public const string ANIM_IS_WALL_SLIDING = "IsWallSliding";
        public const string ANIM_IS_SLIDING = "IsSliding";
        public const string ANIM_JUMP = "Jump";
        public const string ANIM_FALL = "Fall";
        public const string ANIM_GET_HIT = "Hurt";
        public const string ANIM_DIE = "Die";
        public const string ANIM_ATTACK = "Attack";
        public const string ANIM_ATTACK_COUNTER = "AttackNumber";

        public const string IDLE_SKELETON = "Idle";
        public const string ATTACK_SKELETON = "Attack";
        public const string HIT_SKELETON = "Hit";
        public const string DEATH_SKELETON = "Death";

        public const string ATTACK_EYE = "Attack";
        public const string HIT_EYE = "Hit";
        public const string DEATH_EYE = "Death";

        public const string IDLE_ENEMY = "Idle";
        public const string ATTACK_ENEMY = "Attack";
        public const string HIT_ENEMY = "Hit";
        public const string DEATH_ENEMY = "Death";

        //Tags
        public const string TAG_GROUND = "Ground";
        public const string TAG_PLAYER = "Player";
        public const string TAG_ENEMY = "Enemy";

        //Layers
        public const string LAYER_PLAYER = "Player";
        public const string LAYER_ENEMY = "Enemy";
    }
}