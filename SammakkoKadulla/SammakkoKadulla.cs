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
	private double LiikkumisNopeus = 200;
	private double loikkimisNopeus = 100; 
	private PlatformCharacter sammakko; //Sammakon hahmo
	private Vector paikka = new Vector (0, -350); 
	private Image[] sammakkoAnimKuvat;
	private Image[] poliisiAutoAnim;

	#endregion

	#region Pelin alustaminen
	public override void Begin ()
	{
		Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
		PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
		Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
		LataaKuvat();
		LuoTaustaKuva();
		LuoSammakko(paikka,90,90);
		Camera.ZoomFactor = 1.2;
		Camera.StayInLevel = true;
		Camera.Follow(sammakko);
		MessageDisplay.Add( "Etsi iso tähti!" );
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
		 
	}
	#endregion

	public void LuoSammakko(Vector paikka, double leveys, double korkeus)
	{
		sammakko = new PlatformCharacter(leveys, korkeus);
		sammakko.Image = LoadImage ("Sammakko/sammakko_0");
		sammakko.Position = paikka;
		Add(sammakko);
		LiikutaSammakko ();
	}


	public void LuoTaustaKuva (){
	 Level.Background.Image = taustaKuva;
	 Level.Background.Height = 800;
	 Level.Background.Width = 1000;
	 Level.CreateBorders ();
	}

	public void LiikutaSammakko (){
		Keyboard.Listen (Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
		Keyboard.Listen( Key.Up, ButtonState.Pressed, LoikiYlos, null, sammakko, loikkimisNopeus );
		Keyboard.Listen( Key.Down, ButtonState.Pressed, LoikiAlas, null, sammakko, -loikkimisNopeus );
		Keyboard.Listen( Key.Up, ButtonState.Released, sammakko.StopVertical, null );
		Keyboard.Listen( Key.Down, ButtonState.Released,sammakko.StopVertical , null );
		Keyboard.Listen( Key.Left, ButtonState.Pressed, LiikutaOikealleVasemmalle, null, sammakko, -LiikkumisNopeus );
		Keyboard.Listen( Key.Right, ButtonState.Pressed, LiikutaOikealleVasemmalle, null, sammakko, LiikkumisNopeus );
		Keyboard.Listen( Key.Left, ButtonState.Released, sammakko.StopHorizontal, null);
		Keyboard.Listen( Key.Right, ButtonState.Released, sammakko.StopHorizontal, null );
	}


	void LiikutaOikealleVasemmalle( PlatformCharacter hahmo, double nopeus)
	{
		hahmo.Walk (nopeus);
		AnimoiSammakko (sammakko.Animation = new Animation (sammakkoAnimKuvat), true);
	}

	public void LoikiYlos(PlatformCharacter hahmo, double nopeus)
	{
		hahmo.ForceJump (nopeus);
		MessageDisplay.Add(sammakko.TurnsWhenWalking.ToString());
		AnimoiSammakko (sammakko.Animation = new Animation (sammakkoAnimKuvat), true);
	}

	public void LoikiAlas(PlatformCharacter hahmo, double nopeus)
	{
		hahmo.ForceJump (nopeus);
		MessageDisplay.Add(sammakko.Top.ToString());
		if (sammakko.Angle.Degrees == 0) {
			sammakko.FlipImage ();
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

}

