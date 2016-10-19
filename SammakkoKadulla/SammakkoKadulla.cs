using System;
using System.Collections.Generic;
using Jypeli;

/// @author: Ali Nadhum
/// @version: 0.1


/// <summary>
/// Sammakko kadulla.
/// </summary>
public class SammakkoKadulla : PhysicsGame
{
	#region Muuttujat
	private Image taustaKuva; //Taustakuvaa varten 
	private Image[] pyorailjat;
	private Image[] liikenneAutot;
	private Image[] sammakkoAnimKuvat;
	private Image[] poliisiAutoAnim;
	private Image pensasVaaleaKuva;
	private Image pensasTummaKuva;
	private double LiikkumisNopeus = 800;
	private double loikkimisNopeus = 400; 
	private PlatformCharacter sammakko; //Sammakon hahmo
	private Vector paikka = new Vector (0, -350); 
	private bool flipped = false;
	private Angle kulma;
	#endregion


	#region Pelin alustaminen
	public override void Begin ()
	{
		Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
		PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
		Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
		LataaKuvat();
		LuoTaustaKuva(1000, 800);
		LuoPensaat (100,100,50,50);
		LuoSammakko(paikka,90,90);
		Camera.ZoomFactor = 1.2;
		Camera.StayInLevel = true;
		Camera.Follow(sammakko);
		kulma = new Angle ();
		MessageDisplay.Add( "Nappaa kärppästä!!" );
	}
	#endregion


	#region Kuvan haku
	public void LataaKuvat()
	{
        taustaKuva = LoadImage ("Tausta");
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
		liikenneAutot = LoadImages (
			"MusAuto1",
			"Rekka1",
			"PunAuto1",
			"SinAuto1",
			"Poliisi1.1"
		);
		pensasVaaleaKuva = LoadImage ("Pensaat/Pensas1");
		pensasTummaKuva = LoadImage ("Pensaat/Pensas2");
		 
	}
	#endregion


	public void LuoSammakko(Vector paikka, double leveys, double korkeus)
	{
		sammakko = new PlatformCharacter(leveys, korkeus);
		sammakko.Image = LoadImage ("Sammakko/sammakko_0");
		sammakko.Position = paikka;
		sammakko.Mass = 0.1;
		Add(sammakko);
		LiikutaSammakko ();
	}


	public void LuoTaustaKuva (int leveys, int korkeus){
	 	Level.Background.Image = taustaKuva;
		Level.Background.Width = leveys;
		Level.Background.Height = korkeus;
	 	Level.CreateBorders ();
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


	void LuoPensaat(double x, double y,double leveys, double korkeus)
	{
		PhysicsObject pensasVaalea = new PhysicsObject(leveys, korkeus -2);
		PhysicsObject pensasTumma = new PhysicsObject(leveys, korkeus);
		pensasVaalea.Image = pensasVaaleaKuva;
		pensasTumma.Image = pensasTummaKuva;
		pensasVaalea.Mass = 1200;
		pensasTumma.Mass = 1200;
		pensasTumma.Tag = "pTumma";
		pensasVaalea.Tag = "pVaalea";
		for (int i = 0; i <= 20; i++) 
		{
			pensasTumma.X = x;
			pensasVaalea.X = x + 50;
			pensasTumma.Y = y;
			pensasVaalea.Y = y;
			Add (pensasTumma, -1);
			Add (pensasVaalea, -1);
			x = i + 1;
		}
	}


}

