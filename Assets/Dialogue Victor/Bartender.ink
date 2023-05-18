
=bartender_dungeon_check
{dungeonVISITS < 0:
*Head to the bar. ->bartender
-else:
The bartender is dozing at the end of the bar.
*Approach.
    YOU: Long night?
    BARTENDER: What? No. I don't know why I'm so tired. I'm sorry, hon. I think I'm gonna close for the day. I don't feel so good.
    ** Try to get a drink. -> bartender_dungeon_loop
    }

=bartender_dungeon_loop
BARTENDER: {~Sorry, sugar. Not open today.| No can do.| Try a grocery store.|Closed.}
    + Try again. ->bartender_dungeon_loop
    *Leave. ->DONE
=bartender
BARTENDER: What can I get you?
{moralityCHOICE > 0:
*A Shirley Temple. -> good_drink
- else:
* Whiskey, with a beer back. -> bad_drink
}
=good_drink
The bartender raises an eyebrow. 
BARTENDER: Sure thing, sugar. 
She pours the drink and adds a flag to the cherry. She winks at you.
YOU: Thank you. 
* Drink it all at once.
    BARTENDER: Mighty thirsty. 
* Take a sip.
- BARTENDER: You that kid the mayor hired?
YOU: Yeah. I'm hoping I can do a good job. Percival doesn't like me though.
BARTENDER: He don't like anybody.
YOU: Thank you for saying so.
BARTENDER: You're welcome. Need anything else, just holler.
*YOU: Thank you. -> bartender_loop_good


=bad_drink
BARTENDER: Bold choice. 
She sets a shot of whiskey down and slides a beer over from the tap.
* Shoot both and pound it. 
    BARTENDER: 'nother beer, sugar?
    **YOU: Hit me.
    **YOU: No thanks.
* Shoot the shot, sip the beer.
- BARTENDER: You that kid the mayor hired?
YOU: Unfortunately.
BARTENDER: Percival give you a hard time?
YOU: He tried. 
BARTENDER: Well you seem to have a thick enough skin. If you need anything else, just holler.
*YOU: Will do. ->bartender_loop_bad

=bartender_loop_good
+ Order another Shirley Temple.{changebartenderTALKS (1)}
    {bartenderTALKS < 7: 
    BARTENDER: Comin' up. ->bartender_loop_good
    -else:
    BARTENDER: {~Quite a lot of sugar there, sugar.|Sure your teeth won't rot out?|If it's the cherry you want, how 'bout I just give you a hand} ->bartender_loop_good
    }
*Leave
->DONE

= bartender_loop_bad
+  Order another beer.{changebartenderTALKS(1)}
    {bartenderTALKS < 7:
    BARTENDER: Comin up. ->bartender_loop_bad
    -else:
    BARTENDER: {~I think you've had enough.| That's one too many.| You're done, sugar.} -> bartender_loop_bad
    }
* Leave.
->DONE