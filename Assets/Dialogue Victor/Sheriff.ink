=sheriff_dungeon_check
{dungeonVISITS > 0:
The sheriff snores at his desk. Drool has pooled on his shirt.
*Try to wake him up. ->sheriff_dungeon_loop
* Leave ->DONE
-else:
*Talk to the sheriff. ->sheriff
*Leave. ->DONE
}
=sheriff_dungeon_loop
The sheriff snorts, but otherwise stays asleep.
+Try to wake him again. ->sheriff_dungeon_loop
*Leave. -> DONE
=sheriff
SHERIFF: Listen up, I'm only gonna tell you this once.
SHERIFF: This town is quiet for a reason. Nobody thinks they're above the low. I know you city slickers don't have the same kind of respect for authority as those of us who were raised right.
{moralityCHOICE > 0:
YOU: Sir, I have no intention of causing any trouble.
SHERIFF: We'll see about that.
-else:
YOU: That's rich. 
SHERIFF: What'd you say?
YOU: Nothing. 
}
-SHERIFF: Now I better not catch a whiff of that Mary Jane, or my foot will find itself up your ass, and I don't like having to clean my shoes. Understand?
{moralityCHOICE > 0:
* Nod. {changesheriffTALKS (1)} ->sheriff_loop
-else:
* Give him the finger. {changesheriffTALKS (1)} ->sheriff_loop
}
=sheriff_loop
{sheriffTALKS < 3:
SHERIFF: Good. Now scram.
    - else:
SHERIFF: {~You deaf?| Didn't hear me the first time? Git.|I said, get out.}
}

+ Talk to Sheriff again. {changesheriffTALKS(1)} ->sheriff_loop
*Leave. ->DONE