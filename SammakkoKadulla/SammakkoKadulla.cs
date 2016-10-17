using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class SammakkoKadulla : PhysicsGame
{

	private Image taustaKuva = LoadImage("Tausta");
	private PlatformCharacter sammakko;
	private Vector paikka = new Vector (0, -350);
	private Image[] sammakkoAnimKuvat = LoadImages("Sammakko/sammakko_0", "Sammakko/sammakko_1", "Sammakko/sammakko_2",
		"Sammakko/sammakko_3", "Sammakko/sammakko_4", "Sammakko/sammakko_5", "Sammakko/sammakko_6");
	
	public override void Begin (){
		Keyboard.Listen (Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
		PhoneBackButton.Listen (ConfirmExit, "Lopeta peli");
		Keyboard.Listen (Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

		LuoTaustaKuva ();
		LuoSammakko (paikka,70,70);
		LiikutaSammakko ();
		Camera.ZoomFactor = 1.2;
		Camera.StayInLevel = true;
		Camera.Follow (sammakko);

	}

	public void LuoSammakko(Vector paikka, double leveys, double korkeus){
		sammakko = new PlatformCharacter(leveys, korkeus);
		sammakko.Image = LoadImage ("Sammakko/sammakko_0");
		sammakko.Position = paikka;
		Add(sammakko);
	}

	public void AnimoiObjekti(Animation animaatio, bool startAnimation){
		animaatio.FPS = 10;
		if (startAnimation  == true) {
			animaatio.Start ();
		} else {
			animaatio.Stop ();
		}
	}

	public void LuoTaustaKuva (){
		Level.Background.Image = taustaKuva;
		Level.Background.Height = 800;
		Level.Background.Width = 1000;
		Level.CreateBorders ();
	}

	public void LiikutaSammakko (){

		Keyboard.Listen(Key.Left,  ButtonState.Down,
			LiikutaPelaajaa, null, new Vector( -4000, 0 ));
		Keyboard.Listen(Key.Right, ButtonState.Down, 
			LiikutaPelaajaa, null, new Vector( 4000, 0 ));
		Keyboard.Listen(Key.Up,    ButtonState.Down, 
			LiikutaPelaajaa, null, new Vector( 0, 500 ));
		Keyboard.Listen(Key.Down,  ButtonState.Down, 
			LiikutaPelaajaa, null, new Vector( 0, -500 ));

	}

	void LiikutaPelaajaa(Vector vektori)
	{
		sammakko.Push(vektori);
		AnimoiObjekti(sammakko.Animation = new Animation(sammakkoAnimKuvat), true);
	}

	public void TormaaTahteen (PhysicsObject hahmo, PhysicsObject tahti){
		MessageDisplay.Add ("Keräsit tähden!");
		tahti.Destroy ();
	}
}

