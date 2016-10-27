using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Controls;
using Jypeli.Widgets;
using Jypeli.Effects;
using Jypeli.Assets;
using Jypeli.GameObjects;

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
	Image elamaSydan;
	Image[] pyorailijatKuva;
	Image[] autotKuva;
	Image poliisiAutoKuva;
	Image[] sammakkoAnimKuvat;
	Image[] poliisiAutoAnimKuvat;
	Image pensasVaaleaKuva;
	Image pensasTummaKuva;
	private double loikkimisNopeus = 200; 
	PhysicsObject sammakko; //Sammakon hahmo
	Vector paikka;
	PhysicsObject pensasVaalea;
	PhysicsObject pensasTumma;
	PhysicsObject pyorailija;
	PhysicsObject auto;
	PhysicsObject poliisiAuto;
	PhysicsObject vasenReuna;
	PhysicsObject oikeaReuna;
	PhysicsObject jarvi;
	private bool flipped = false;
	Angle kulma;
	DoubleMeter	aikaMittari;
	Timer aikaLaskuri;
	List <Label> sydamet = new List<Label>(5);
	Label sydan;
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
			LuoPensaat (x, -365, 66, 75);
			LuoPensaat (x, -300, 66, 75);
			if (i == 3)
			{
				x = 0;
			}
			x += 120;
		}
		LuoJarvi (0, Screen.Top - 143, Screen.Width, 180);
		LuoAikaLaskuri ();
		LuoPelaajanElamaSydamet (Screen.Right - 150, Screen.Top - 40, 25, 25);
		PyoraAjastin ();
		AutotAjastin ();
		Camera.ZoomFactor = 1.2;
		Camera.StayInLevel = true;
		LuoSammakko(paikka, 60, 60);
		Camera.Follow(sammakko);
		kulma = new Angle ();
		SetWindowSize(1024, 768, false);
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
		autotKuva = LoadImages (
			"Autot/Bussi",
			"Autot/MusAuto",
			"Autot/PunAuto",
			"Autot/Rekka",
			"Autot/SinAuto",
			"Autot/Poliisi1.1"
		);
		poliisiAutoKuva = LoadImage ("Autot/Poliisi1.1");
		sammakkoAnimKuvat = LoadImages(
			"Sammakko/sammakko_0", 
			"Sammakko/sammakko_1", 
			"Sammakko/sammakko_2",
			"Sammakko/sammakko_3", 
			"Sammakko/sammakko_4", 
			"Sammakko/sammakko_5", 
			"Sammakko/sammakko_6"
		);
		poliisiAutoAnimKuvat = LoadImages (
			"Autot/Poliisi1.1",
			"Autot/Poliisi1.2",
			"Autot/Poliisi1.3",
			"Autot/Poliisi1.2"
		);
		pensasVaaleaKuva = LoadImage ("Pensaat/Pensas1");
		pensasTummaKuva = LoadImage ("Pensaat/Pensas2");
		elamaSydan = LoadImage ("Elama");
	}
	#endregion

	public void LuoTaustaKuva (int leveys, int korkeus){
	 	Level.Background.Image = taustaKuva;
		Level.Background.Width = leveys;
		Level.Background.Height = korkeus;
		vasenReuna = Level.CreateLeftBorder();
		oikeaReuna = Level.CreateRightBorder();
		Level.CreateBottomBorder();
		Level.CreateTopBorder();
	}

	#region Sammakon logiikka
	public void LuoSammakko(Vector paikka, double leveys, double korkeus)
	{
		sammakko = new PhysicsObject(leveys, korkeus, Shape.FromImage(sammakonKuva));
		sammakko.Image = sammakonKuva;
		sammakko.Position = paikka;
		sammakko.CanRotate = false;
		sammakko.Tag = "sammakko";
		Add(sammakko);
		LiikutaSammakko ();
	}
		

	public void LiikutaSammakko ()
	{
		Keyboard.Listen( Key.Up, ButtonState.Pressed, LoikiYlos, null, sammakko, loikkimisNopeus);
		Keyboard.Listen( Key.Down, ButtonState.Pressed, LoikiAlas, null, sammakko, loikkimisNopeus );
		Keyboard.Listen( Key.Up, ButtonState.Released, sammakko.StopVertical, null );
		Keyboard.Listen( Key.Down, ButtonState.Released,sammakko.StopVertical , null );
		Keyboard.Listen( Key.Left, ButtonState.Pressed, LiikutaVasemmalle, null, sammakko, loikkimisNopeus );
		Keyboard.Listen( Key.Right, ButtonState.Pressed, LiikutaOikealle, null, sammakko, loikkimisNopeus );
		Keyboard.Listen( Key.Left, ButtonState.Released, sammakko.StopHorizontal, null);
		Keyboard.Listen( Key.Right, ButtonState.Released, sammakko.StopHorizontal, null );
		AddCollisionHandler(sammakko, "pyorailija", SammakkoOsuu);
		AddCollisionHandler(jarvi, "sammakko", SammakkoOsuu);
	}


	void LiikutaVasemmalle( PhysicsObject hahmo, double nopeus)
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
		hahmo.Move(new Vector(-nopeus, 0)); 
		AnimoiSammakko (sammakko.Animation = new Animation (sammakkoAnimKuvat), true);
	}


	void LiikutaOikealle( PhysicsObject hahmo, double nopeus)
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
		hahmo.Move(new Vector(nopeus, 0));
		AnimoiSammakko (sammakko.Animation = new Animation (sammakkoAnimKuvat), true);
	}


	public void LoikiYlos(PhysicsObject hahmo, double nopeus)
	{
		kulma.Degrees = 0;
		hahmo.Angle = kulma;
		hahmo.Move(new Vector(0, nopeus));
		if (flipped) {
			hahmo.FlipImage ();
			flipped = false;
		}
		AnimoiSammakko (sammakko.Animation = new Animation (sammakkoAnimKuvat), true);
	}


	public void LoikiAlas(PhysicsObject hahmo, double nopeus)
	{
		hahmo.Move(new Vector(0, -nopeus));
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

	public void SammakkoOsuu(PhysicsObject sammakko, PhysicsObject kohde)
	{
		for(int i = 0; i < sydamet.Count; i++) 
		{
			sydamet.RemoveRange (i, 1);
		}
		kohde.Destroy ();
	}
	#endregion

	#region Pensaat
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
	#endregion 

	#region Pyorailijat funktiot
	public void Pyorailija(Image pyorailijanKuva, double x, double y,double leveys, double korkeus, Vector pyoranSuunta)
	{
		pyorailija = new PhysicsObject (leveys, korkeus, Shape.FromImage(pyorailijanKuva));
		pyorailija.Image = pyorailijanKuva;
		pyorailija.Tag = "pyorailija";
		pyorailija.Mass = 1;
		pyorailija.X = x;
		pyorailija.Y = y;
		pyorailija.Hit(pyoranSuunta);
		pyorailija.MakeStatic ();
		Add (pyorailija);
	}


	public void LuoPyorailijat()
	{
		int rndPyorat1 = RandomGen.NextInt (0, 15);
		int rndPyorat2 = RandomGen.NextInt (0, 15);
		double rndY1 = RandomGen.NextDouble (-160, -190);
		double rndY2 = RandomGen.NextDouble (-200, -250);
		Pyorailija (pyorailijatKuva[rndPyorat1], -450, rndY1, 40, 20, new Vector(100,0));
		Pyorailija (Image.Mirror(pyorailijatKuva[rndPyorat2]), 450, rndY2, 40, 20, new Vector(-100,0));
		AddCollisionHandler(pyorailija, ObjektiOsuuSeinaan);

	}

	public void PyoraAjastin()
	{
		Timer ajastin = new Timer ();
		ajastin.Interval = 2;   // tällä voit myös säätää nopeutta
		ajastin.Timeout += LuoPyorailijat;
		ajastin.Start();
	}
	#endregion


	#region Autot
	public void Auto(Image autoKuva, double x, double y,double leveys, double korkeus, Vector autonSuunta)
	{
		auto = new PhysicsObject (leveys, korkeus, Shape.FromImage(autoKuva));
		auto.Image = autoKuva;
		auto.Tag = "auto";
		auto.X = x;
		auto.Y = y;
		auto.Hit(autonSuunta);
		auto.MakeStatic ();
		Add (auto);
	}


	public void Luoautot()
	{
		int rndAuto1 = RandomGen.NextInt (0, 6);
		int rndAuto2 = RandomGen.NextInt (0, 6);
		Auto (autotKuva[rndAuto1], -450, 45, 50, 30, new Vector(100,0));
		Auto (Image.Mirror(autotKuva[rndAuto2]), 450, -50, 50, 30, new Vector(-100,0));
		AddCollisionHandler(auto, ObjektiOsuuSeinaan);
		MediaPlayer.Play ("Audio/CityTraffic");

	}
		
	public void PoliisiAuto(Image[] poliisiAutoAnimKuvat, double x, double y,double leveys, double korkeus, Vector autonSuunta)
	{
		poliisiAuto = new PhysicsObject (leveys, korkeus, Shape.FromImage(poliisiAutoKuva));
		poliisiAuto.Tag = "auto";
		poliisiAuto.X = x;
		poliisiAuto.Y = y;
		Animation poliisiAutoAnim = new Animation (poliisiAutoAnimKuvat);
		poliisiAutoAnim.FPS = 40;
		poliisiAuto.Animation = poliisiAutoAnim;
		poliisiAutoAnim.Start ();
		poliisiAuto.Hit(autonSuunta);
		poliisiAuto.MakeStatic ();
		MediaPlayer.Play("Audio/PoliisiSireeni");
		Add (poliisiAuto);
	}


	public void LuoPoliisiAuto(){
		PoliisiAuto (poliisiAutoAnimKuvat, -450, 0, 50, 30, new Vector (150, 0));
		PoliisiAuto (Image.Mirror(poliisiAutoAnimKuvat), 450, 0, 50, 30, new Vector (-150, 0));
		AddCollisionHandler(poliisiAuto, ObjektiOsuuSeinaan);
	}


	public void AutotAjastin()
	{
		Timer autoAjastin = new Timer ();
		autoAjastin.Interval = 3;   // tällä voit myös säätää nopeutta
		autoAjastin.Timeout += Luoautot;
		autoAjastin.Start();

		// Poliisi auto ajastin
		Timer poliisiAutoAjastin =  new Timer();
		poliisiAutoAjastin.Interval = 20;
		poliisiAutoAjastin.Timeout += LuoPoliisiAuto;
		poliisiAutoAjastin.Start ();
	}
		
	#endregion

	#region Jarvi
	public void LuoJarvi(double x, double y,double leveys, double korkeus)
	{
		jarvi = new PhysicsObject (leveys, korkeus);
		jarvi.Color = Color.Aqua;
		jarvi.Tag = "järvi";
		jarvi.X = x;
		jarvi.Y = y;
		jarvi.Width = leveys;
		jarvi.Height = korkeus;
		Add (jarvi);

	}
	#endregion

	#region Tukit
		
	#endregion


	public void ObjektiOsuuSeinaan(PhysicsObject objekti, PhysicsObject seina){
		if((seina == vasenReuna) || (seina == oikeaReuna))
		{
			objekti.Destroy ();
		}
	}

	public void LuoAikaLaskuri()
	{
		aikaMittari = new DoubleMeter(60);
		aikaMittari.MaxValue = 60;

		aikaLaskuri = new Timer();
		aikaLaskuri.Interval = 0.1;
		aikaLaskuri.Timeout += LaskeAlaspain;
		aikaLaskuri.Start();

		ProgressBar aikaPalkki = new ProgressBar(400, 10);
		aikaPalkki.X = Screen.Left + 230;
		aikaPalkki.Y = Screen.Top - 60;
		aikaPalkki.Color = Color.Transparent;
		aikaPalkki.BarColor = Color.Red;
		aikaPalkki.BorderColor = Color.Black;
		aikaPalkki.BindTo (aikaMittari);
		Add(aikaPalkki);

		Label aikaTeksti = new Label();
		aikaTeksti.TextColor = Color.Wheat;
		aikaTeksti.Color = Color.Transparent;
		aikaTeksti.Text = "Jäljellä oleva aika: ";
		aikaTeksti.X = Screen.Left + 120;
		aikaTeksti.Y = Screen.Top - 40;
		Add (aikaTeksti);
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
		
	public List<Label> LuoPelaajanElamaSydamet(double x, double y,double leveys, double korkeus)
	{
		int vali = 0;
		for (int i = 1; i <= sydamet.Capacity; i++) 
		{
			sydan = new Label ();
			sydan.Image = elamaSydan;
			sydan.X = x + vali;
			sydan.Y = y;
			sydan.Width = leveys;
			sydan.Height = korkeus;
			Add (sydan);
			sydamet.Add (sydan);
			vali += 30;
		}
			
		return sydamet;
	}
		

}

