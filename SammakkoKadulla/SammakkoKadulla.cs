using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Controls;
using Jypeli.Widgets;

/// @author: Ali Nadhum
/// @version: 0.1


/// <summary>
/// Sammakko kadulla.
/// </summary>
public class SammakkoKadulla : PhysicsGame
{
	#region Pelin muuttujat
	Image taustaKuva; //Taustakuvaa varten 
	Image sammakonKuva;
	Image[] pyorailijatKuva;
	Image[] sammakkoAnimKuvat;
	Image[] poliisiAutoAnim;
	Image pensasVaaleaKuva;
	Image pensasTummaKuva;
	private double LiikkumisNopeus = 800;
	private double loikkimisNopeus = 400; 
	PlatformCharacter sammakko; //Sammakon hahmo
	Vector paikka;
	PhysicsObject pensasVaalea;
	PhysicsObject pensasTumma;
	private bool flipped = false;
	Angle kulma;
	DoubleMeter	aikaMittari;
	Timer aikaLaskuri;
	#endregion

	#region Pelin alustaminen
	public override void Begin ()
	{
		Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
		PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
		Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
		paikka = new Vector (0, -350);
		LataaKuvat();
		LuoTaustaKuva(1000, 800);
		int x = -500;
		for (int i = 0; i <= 8; i++) {
			LuoPensaat (x, 100, 65, 65);
			LuoPensaat (x, -350, 75, 75);
			if (i == 3)
			{
				x = 0;
			}
			x += 120;
		}
		LuoSammakko(paikka,60,60);
		LuoAikaLaskuri ();
		PyoraAjastin ();
		Camera.ZoomFactor = 1.2;
		Camera.StayInLevel = true;
		Camera.Follow(sammakko);
		kulma = new Angle ();
		MessageDisplay.Add( "Nappaa kärppästä!!" );
		SetWindowSize (1000, 750);
	}
	#endregion

	#region Kuvan haku
	public void LataaKuvat()
	{
        taustaKuva = LoadImage ("Tausta");
		sammakonKuva = LoadImage ("Sammakko/sammakko_0");
		pyorailijatKuva = LoadImages (
			"Pyorailijat/Pyora1",
			"Pyorailijat/Pyora2",
			"Pyorailijat/Pyora3",
			"Pyorailijat/Pyora4",
			"Pyorailijat/Pyora5",
			"Pyorailijat/Pyora6",
			"Pyorailijat/Pyora7",
			"Pyorailijat/Pyora8",
			"Pyorailijat/Pyora9",
			"Pyorailijat/Pyora10",
			"Pyorailijat/Pyora11",
			"Pyorailijat/Pyora12",
			"Pyorailijat/Pyora13",
			"Pyorailijat/Pyora14",
			"Pyorailijat/Pyora15"
		);
		sammakkoAnimKuvat = LoadImages(
			"Sammakko/sammakko_0", 
			"Sammakko/sammakko_1", 
			"Sammakko/sammakko_2",
			"Sammakko/sammakko_3", 
			"Sammakko/sammakko_4", 
			"Sammakko/sammakko_5", 
			"Sammakko/sammakko_6"
		);
		poliisiAutoAnim = LoadImages (
			"Poliisi1.1",
			"Pollisi1.2",
			"Poliisi1.3",
			"Poliisi1.2"
		);
		pensasVaaleaKuva = LoadImage ("Pensaat/Pensas1");
		pensasTummaKuva = LoadImage ("Pensaat/Pensas2");
	}
	#endregion

	public void LuoTaustaKuva (int leveys, int korkeus){
	 	Level.Background.Image = taustaKuva;
		Level.Background.Width = leveys;
		Level.Background.Height = korkeus;
	 	Level.CreateBorders ();
	}

	#region Sammakon logiikka
	public void LuoSammakko(Vector paikka, double leveys, double korkeus)
	{
		sammakko = new PlatformCharacter(leveys, korkeus, Shape.FromImage(sammakonKuva));
		sammakko.Image = sammakonKuva;
		sammakko.Position = paikka;
		sammakko.SizingByLayout = true;
		Add(sammakko);
		LiikutaSammakko ();
	}
		

	public void LiikutaSammakko (){
		Keyboard.Listen (Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
		Keyboard.Listen( Key.Up, ButtonState.Pressed, LoikiYlos, null, sammakko, loikkimisNopeus);
		Keyboard.Listen( Key.Down, ButtonState.Pressed, LoikiAlas, null, sammakko, -loikkimisNopeus );
		Keyboard.Listen( Key.Up, ButtonState.Released, sammakko.StopVertical, null );
		Keyboard.Listen( Key.Down, ButtonState.Released,sammakko.StopVertical , null );
		Keyboard.Listen( Key.Left, ButtonState.Pressed, LiikutaVasemmalle, null, sammakko, -LiikkumisNopeus );
		Keyboard.Listen( Key.Right, ButtonState.Pressed, LiikutaOikealle, null, sammakko, LiikkumisNopeus );
		Keyboard.Listen( Key.Left, ButtonState.Released, sammakko.StopHorizontal, null);
		Keyboard.Listen( Key.Right, ButtonState.Released, sammakko.StopHorizontal, null );
	}


	void LiikutaVasemmalle( PlatformCharacter hahmo, double nopeus)
	{
		if (kulma.Degrees == 0)
		{
			kulma.Degrees = 90;

		} 
		else if (kulma.Degrees > 0 && kulma.Degrees < 90)
		{
			kulma.Degrees = -90;
		}
		hahmo.Angle = kulma;
		hahmo.Walk (nopeus);
		AnimoiSammakko (sammakko.Animation = new Animation (sammakkoAnimKuvat), true);
	}


	void LiikutaOikealle( PlatformCharacter hahmo, double nopeus)
	{
		if (kulma.Degrees == 0) 
		{
			kulma.Degrees = -90;
		} 
		else if (kulma.Degrees > 0 && kulma.Degrees < 90)
		{
			kulma.Degrees = 90;
		}
		hahmo.Angle = kulma;
		hahmo.Walk (nopeus);
		AnimoiSammakko (sammakko.Animation = new Animation (sammakkoAnimKuvat), true);
	}


	public void LoikiYlos(PlatformCharacter hahmo, double nopeus)
	{
		kulma.Degrees = 0;
		hahmo.Angle = kulma;
		hahmo.ForceJump (nopeus);
		if (flipped) {
			hahmo.FlipImage ();
			flipped = false;
		}
		AnimoiSammakko (sammakko.Animation = new Animation (sammakkoAnimKuvat), true);
	}


	public void LoikiAlas(PlatformCharacter hahmo, double nopeus)
	{
		hahmo.ForceJump (nopeus);
		kulma.Degrees = 1;
		hahmo.Angle = kulma;
		if (!flipped) {
			hahmo.FlipImage ();
			flipped = true;
		}
		AnimoiSammakko (sammakko.Animation = new Animation (sammakkoAnimKuvat), true);
	}


	public void AnimoiSammakko(Animation animaatio, bool tilanne)
	{
		animaatio.FPS = 40;
		if (tilanne  == true) {
			animaatio.Start (1);
		} else {
			animaatio.Stop ();
		}
	}
	#endregion

	public void LuoPensaat(double x, double y,double leveys, double korkeus)
	{
		pensasVaalea = PhysicsObject.CreateStaticObject(leveys, korkeus, Shape.FromImage(pensasVaaleaKuva));
		pensasTumma = PhysicsObject.CreateStaticObject(leveys, korkeus, Shape.FromImage(pensasTummaKuva));
		pensasVaalea.Image = pensasVaaleaKuva; 
		pensasTumma.Image = pensasTummaKuva;
		pensasVaalea.MakeStatic ();
		pensasTumma.MakeStatic ();
		pensasVaalea.Tag = "pVaalea";
		pensasTumma.Tag = "pTumma";
		pensasTumma.X = x;
		pensasVaalea.X = x + 60;
		pensasTumma.Y = y;
		pensasVaalea.Y = y;
		Add (pensasTumma);
		Add (pensasVaalea);
	}


	public void Pyorailija(Image pyorailijanKuva, double x, double y,double leveys, double korkeus)
	{
		PhysicsObject pyrorailija = new PhysicsObject (leveys, korkeus, Shape.FromImage(pyorailijanKuva));
		pyrorailija.Image = pyorailijanKuva;
		pyrorailija.Tag = "pyorailija";
		pyrorailija.X = x;
		pyrorailija.Y = y;
		Vector pyoraNopeus = new Vector (100, 0);
		pyrorailija.Hit(pyoraNopeus);
		Add (pyrorailija);
	}


	public void LuoPyorailijat()
	{
		int rndPyorat = RandomGen.NextInt (1, 15);
		double rndY = RandomGen.NextDouble (-170, -250);
		Pyorailija (pyorailijatKuva[rndPyorat], -450, rndY, 40, 20);
	}


	public void PyoraAjastin()
	{
		Timer ajastin = new Timer ();
		ajastin.Interval = 0.4;   // tällä voit myös säätää nopeutta
		ajastin.Timeout += LuoPyorailijat;
		ajastin.Start();
	}


	public void LuoAikaLaskuri()
	{
		aikaMittari = new DoubleMeter(10);
		aikaMittari.MaxValue = 10;

		aikaLaskuri = new Timer();
		aikaLaskuri.Interval = 0.1;
		aikaLaskuri.Timeout += LaskeAlaspain;
		aikaLaskuri.Start();

		ProgressBar aikaPalkki = new ProgressBar(150, 20);
		aikaPalkki.X = Screen.Left + 250;
		aikaPalkki.Y = Screen.Top - 40;
		aikaPalkki.Color = Color.Transparent;
		aikaPalkki.BarColor = Color.Red;
		aikaPalkki.BorderColor = Color.Black;
		aikaPalkki.BindTo (aikaMittari);
		Add(aikaPalkki);
	}

	public void LaskeAlaspain()
	{
		aikaMittari.Value -= 0.1;

		if (aikaMittari.Value <= 0)
		{
			MessageDisplay.Add("Aika loppui...");
			aikaLaskuri.Stop();

		}
	}

}

