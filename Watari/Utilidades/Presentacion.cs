using System;

namespace Watari.Utilidades
{
  public static class Presentacion
  {
    const string ApertureCake = @"            ,:/+/-
            /M/              .,-=;//;-
       .:/= ;MH/,    ,=/+%$XH@MM#@:
      -$##@+$###@H@MMM#######H:.    -/H#
 .,H@H@ X######@ -H#####@+-     -+H###@X
  .,@##H;      +XM##M/,     =%@###@X;-
X%-  :M##########$.    .:%M###@%:
M##H,   +H@@@$/-.  ,;$M###@%,          -
M####M=,,---,.-%%H####M$:          ,+@##
@##################@/.         :%H##@$-
M###############H,         ;HM##M$=
#################.    .=$M##M$=
################H..;XM##M$=          .:+
M###################@%=           =+@MH%
@#################M/.         =+H#X%=
=+M###############M,      ,/X#H+:,
  .;XM###########H=   ,/X#H+:;
     .=+HM#######M+/+HM@+=.
         ,:/%XM####H/.
              ,.:=-.";

    /// <summary>
    /// Podria explicarlo, o podrias verlo tu mismo.
    /// </summary>
    public static void PortalStillAlive()
    {
      ReproductorAudio stillAlive = new ReproductorAudio($@"{AppDomain.CurrentDomain.BaseDirectory}FormaDeOnda\stillalive.mp3");
      Console.WriteLine(ApertureCake);
      stillAlive.Reproducir();
    }
  }
}
