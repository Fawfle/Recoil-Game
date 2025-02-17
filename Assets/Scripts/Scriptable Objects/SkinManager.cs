using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Skins", menuName = "ScriptableObjects/PlayerSkinList", order = 1)]
public class SkinManager : ScriptableObject
{
    private static float HIGH_SCORE_1_REQUIREMENT = 1000;

    private static float HIGH_SCORE_2_REQUIREMENT = 10000;

    public List<PlayerSkin> skins;

    public int GetSkinIndex(SkinKey skin)
    {
        for (int i = 0; i < skins.Count; i++)
        {
            if (skins[i].skinKey == skin) return i;
        }

        return -1;
    }

    public PlayerSkin GetSkin(SkinKey skin)
    {
        return skins[GetSkinIndex(skin)];
    }

    public static bool HasSkinUnlocked(SkinKey skin)
    {
		switch (skin)
        {
            case SkinKey.Default:
                return true;
            case SkinKey.HighScore1:
                return SaveManager.save.highScore >= HIGH_SCORE_1_REQUIREMENT;
			case SkinKey.HighScore2:
				return SaveManager.save.highScore >= HIGH_SCORE_2_REQUIREMENT;
            case SkinKey.Leaderboard:
                return SaveManager.save.highestLeaderboardRank != -1 && SaveManager.save.highestLeaderboardRank < 3;
            case SkinKey.Levels1:
                return SaveManager.HasCompletedLevels(0, 24);
            case SkinKey.Levels2:
                return SaveManager.HasCompletedLevels(24, 48);
            default:
                break;
		}


        return false;
    }
}

[System.Serializable]
public struct PlayerSkin
{
	public string name;
	public string description;

	public SkinKey skinKey;

	public Sprite bodySprite;
    public float bodyScale;// = 0.7f;
    public float outlineScale;// = 1.0f;

	public Sprite gunSprite;
    public float gunScale;// = 0.5f;
    public Vector2 gunSize;
}

public enum SkinKey
{
	Default,
	HighScore1, // get high score
	HighScore2,
	Leaderboard, // top 3 on leaderboard
	Levels1, // 1-24
	Levels2, // 25-48
	Levels3, // 49-72
}