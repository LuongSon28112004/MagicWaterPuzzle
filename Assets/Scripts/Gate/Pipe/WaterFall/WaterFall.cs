using UnityEngine;

public enum DirectionWaterSplash
{
    UP,
    LEFT,
    RIGHT,
    DOWN,
}

public class WaterFall : MonoBehaviour, IControlParticle
{
    [SerializeField] private DirectionWaterSplash directionWaterSplash;
    [SerializeField] GameObject SideSplashUp;
    [SerializeField] WaterSplashUp splashUp;
    [SerializeField] GameObject SideSplashLeft;
    [SerializeField] WaterSplashLeft splashLeft;
    [SerializeField] GameObject SideSplashRight;
    [SerializeField] WaterSplashRight splashRight;
    [SerializeField] private bool isPlayParticle = false;

    public DirectionWaterSplash DirectionWaterSplash { get => directionWaterSplash; set => directionWaterSplash = value; }

    public void SetColorSplash(Color color)
    {
        if (directionWaterSplash == DirectionWaterSplash.DOWN)
        {
            splashUp.SetColor(color);
        }
        else if (directionWaterSplash == DirectionWaterSplash.LEFT)
        {
            splashLeft.SetColor(color);
        }
        else if (directionWaterSplash == DirectionWaterSplash.RIGHT)
        {
            splashRight.SetColor(color);
        }
    }
    public void SetHeight(float Height)
    {
        if (directionWaterSplash == DirectionWaterSplash.DOWN)
        {
            SideSplashUp.transform.localScale = new Vector3(1f, Height, 1f);
        }
        else if (directionWaterSplash == DirectionWaterSplash.LEFT)
        {
            SideSplashLeft.transform.localScale = new Vector3(1f, Height, 1f);
        }
        else if (directionWaterSplash == DirectionWaterSplash.RIGHT)
        {
            SideSplashRight.transform.localScale = new Vector3(1f, Height, 1f);
        }
    }

    public void PlayParticle()
    {
        if (!isPlayParticle)
        {
            isPlayParticle = true;
            if (directionWaterSplash == DirectionWaterSplash.DOWN)
            {
                SideSplashUp.SetActive(true);
            }
            else if (directionWaterSplash == DirectionWaterSplash.LEFT)
            {
                SideSplashLeft.SetActive(true);
            }
            else if (directionWaterSplash == DirectionWaterSplash.RIGHT)
            {
                SideSplashRight.SetActive(true);
            }
        }
    }

    public void StopParticle()
    {
        if (isPlayParticle)
        {
            isPlayParticle = false;
            if (directionWaterSplash == DirectionWaterSplash.DOWN)
            {
                SideSplashUp.SetActive(false);
            }
            else if (directionWaterSplash == DirectionWaterSplash.LEFT)
            {
                SideSplashLeft.SetActive(false);
            }
            else if (directionWaterSplash == DirectionWaterSplash.RIGHT)
            {
                SideSplashRight.SetActive(false);
            }
        }
    }
    public void RestartParticle()
    {

    }

}
