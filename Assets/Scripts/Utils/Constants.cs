using UnityEngine;

namespace Utils {
    public static class Constants {
        //Hierarchy Names
        public const string HIERARCHY_PLAYER = "Adventurer";

        //Animator
        public const string ANIM_IS_RUNNING = "IsRunning";
        public const string ANIM_IS_JUMPING = "IsJumping";
        public const string ANIM_JUMP = "Jump";
        public const string ANIM_FALL = "Fall";
        public const string ANIM_IS_FALLING = "IsFalling";

        public const string IDLE_SKELETON = "Idle";
        public const string ATTACK_SKELETON = "Attack";
        public const string HIT_SKELETON = "Hit";
        public const string DEATH_SKELETON = "Death";

        public const string ATTACK_EYE = "Attack";
        public const string HIT_EYE = "Hit";
        public const string DEATH_EYE = "Death";

        //Tags
        public const string TAG_GROUND = "Ground";
        public const string TAG_PLAYER = "Player";
        public const string TAG_ENEMY = "Enemy";

        //Layers
        public const string LAYER_PLAYER = "Player";
        public const string LAYER_ENEMY = "Enemy";
    }
}