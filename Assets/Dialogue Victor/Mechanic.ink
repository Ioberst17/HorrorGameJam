=mechanic_dungeon_check
{dungeonVISITS > 0:
Anthony is hunched over the hood of a car. He sighs when he hears you step up to him.
ANTHONY: I gotta get out of this town, man.
*Ask Why.
    YOU: What do you mean? 
    ANTHONY: I dunno. Something just don't feel right.
    **Ask for details.
        ANTHONY: Don't wanna talk about it. Bad night is all. 
        *** Leave. ->DONE
    **Leave. ->DONE
*Leave. ->DONE
- else:
->mechanic
}

=mechanic
A young man bounds up to you. He gives you a big grin and offers to shake your hand.
MECHANIC: Nice to meet you! I'm Anthony. You the new <> 
{pronoun:
-1: guy
-2: gal
-3: person
}<> who's gonna set up the festival? Bring some business here?
{moralityCHOICE > 0:
YOU: I'll certainly do my best.
-else:
YOU: Yeah, but I wouldn't get my hopes up.
}
ANTHONY: Yeah, it'll be a challenge for sure. Not a lot of foot traffic but mayor's gotta do something, right?
ANTHONY: Anyway, what can I do for you?
*Chat.
    {moralityCHOICE > 0:
    YOU: Oh, I'm just trying to meet people in the town. I figure I'll be here for a while so would be nice to put some faces and names together.
    -else:
    YOU: Just exploring. Maybe someone out here doesn't automatically hate me for not being being born in this dump town.
    }
    ANTHONY: Well you've met at least one person now. Unfortunately I'm a little busy so I gotta get back to work. But stop by any time around closing time. Not a lot of people around here for me to talk to, y'know?
    **Chat more. 
    The sound of a socket wrench comes from underneath an old truck. -> mechanic_loop
    ** Leave. ->DONE
*Leave.
    ANTHONY: Okay, well I'll be here if you need any kind of repairs. 
->DONE

=mechanic_loop
 ANTHONY: {~A little busy.|Got a lot of work to do.|The bar's a good place to chat.}
+ Talk to Anthony -> mechanic_loop
   
*Leave. ->DONE