﻿using UnityEngine;

public class ItemTime
{
	public short idIcon;

	public int second;

	public int minute;

	internal long curr;

	internal long last;

	internal bool isText;

	internal bool dontClear;

	internal string text;

	internal bool isPaint_coolDownBar;

	public int time;

	public int coutTime;

	internal int per = 100;

    internal bool isEquivalence;

    internal bool isInfinity;

    internal int tenthSecond;

    public ItemTime()
	{
	}

    internal ItemTime(short idIcon, int time, bool isEquivalence) : this(idIcon, time)
    {
        this.isEquivalence = isEquivalence;
    }

    internal ItemTime(short idIcon, bool isInfinity)
    {
        this.idIcon = idIcon;
        this.isInfinity = isInfinity;
    }


    public ItemTime(short idIcon, int s)
	{
		this.idIcon = idIcon;
		minute = s / 60;
		second = s % 60;
		time = s;
		coutTime = s;
		curr = (last = mSystem.currentTimeMillis());
		isPaint_coolDownBar = idIcon == 14;
	}

	public void initTimeText(sbyte id, string text, int time)
	{
		if (time == -1)
			dontClear = true;
		else
			dontClear = false;
		isText = true;
		minute = time / 60;
		second = time % 60;
		idIcon = id;
		this.time = time;
		coutTime = time;
		this.text = text;
		curr = (last = mSystem.currentTimeMillis());
		isPaint_coolDownBar = idIcon == 14;
	}

	public void initTime(int time, bool isText)
	{
		minute = time / 60;
		second = time % 60;
		this.time = time;
		coutTime = time;
		this.isText = isText;
		curr = (last = mSystem.currentTimeMillis());
	}

	public static bool isExistItem(int id)
	{
		for (int i = 0; i < Char.vItemTime.size(); i++)
		{
			if (((ItemTime)Char.vItemTime.elementAt(i)).idIcon == id)
				return true;
		}
		return false;
	}

	public static ItemTime getMessageById(int id)
	{
		for (int i = 0; i < GameScr.textTime.size(); i++)
		{
			ItemTime itemTime = (ItemTime)GameScr.textTime.elementAt(i);
			if (itemTime.idIcon == id)
				return itemTime;
		}
		return null;
	}

	public static bool isExistMessage(int id)
	{
		for (int i = 0; i < GameScr.textTime.size(); i++)
		{
			if (((ItemTime)GameScr.textTime.elementAt(i)).idIcon == id)
				return true;
		}
		return false;
	}

	public static ItemTime getItemById(int id)
	{
		for (int i = 0; i < Char.vItemTime.size(); i++)
		{
			ItemTime itemTime = (ItemTime)Char.vItemTime.elementAt(i);
			if (itemTime.idIcon == id)
				return itemTime;
		}
		return null;
	}

	public void initTime(int time)
	{
		minute = time / 60;
		second = time % 60;
		coutTime = time;
		curr = (last = mSystem.currentTimeMillis());
	}

	public void paint(mGraphics g, int x, int y)
	{
        SmallImage.drawSmallImage(g, idIcon, x, y, 0, 3);
        string str;
        if (!isInfinity)
        {
            str = minute + "'" + second + "s";
            if (minute == 0)
            {
                str = second + "s";
                if (second < 10)
                    str = second + "." + tenthSecond + "s";
            }
            if (isEquivalence)
                str = "~" + str;
        }
        else
			str = "∞";
        mFont.tahoma_7b_white.drawString(g, str, x, y + 15, 2, mFont.tahoma_7b_dark);
    }

	public void paintText(mGraphics g, int x, int y)
	{
		if (isPaint_coolDownBar)
		{
			if (Char.myCharz() != null)
			{
				int num = 80;
				int x2 = GameCanvas.w / 2 - num / 2;
				int y2 = GameCanvas.h - 80;
				g.setColor(8421504);
				g.fillRect(x2, y2, num, 2);
				g.setColor(16777215);
				if (per > 0)
					g.fillRect(x2, y2, num * per / 100, 2);
			}
			return;
		}
        string str = minute + "'" + second + "s";
        if (minute < 1)
            str = second + "s";
        if (minute < 0)
            str = string.Empty;
        if (dontClear)
            str = string.Empty;
        mFont.tahoma_7b_white.drawString(g, text + " " + str, x, y, 0, mFont.tahoma_7b_dark);
	}

	public void update()
	{
        if (isInfinity)
            return;
        curr = mSystem.currentTimeMillis();
        tenthSecond = Mathf.Clamp((int)(9 - (curr - last) / 100), 0, 9);
        if (curr - last >= 1000)
        {
            last = mSystem.currentTimeMillis();
            second--;
            coutTime--;
            if (second == -1)
            {
                second = 59;
                minute--;
            }
            if (time > 0)
                per = coutTime * 100 / time;
        }
        if (minute < 0 && !isText)
            Char.vItemTime.removeElement(this);
        if (minute < 0 && isText && !dontClear)
            GameScr.textTime.removeElement(this);
    }
}
