=demon_outside_check
{demon_outside > 0:
->demon_outside_loop
-else:
->demon_outside_loop
}
=demon_outside
DEMON: Hey you made it out! Look at you. Now, see that well over there? Just hop on down it!
YOU: Drop down a well? 
DEMON: Yep! You're dreaming, remember?
*Drop Down. ->DONE
*Not yet. -> DONE
//If we can make it work, would be cool to have the shades of townsfolk in the map and have the map look different in some way.
=demon_outside_loop
DEMON: Ready to go?
*Yes. ->DONE
*Not Yet. ->DONE
