=Town_Hall_Interior
Percival unlocks the main doors, blowing dust and swiping cobwebs off the handles as he takes off the lock.
PERCIVAL: Here it is, city <>
{pronoun:
-1: boy.
-2: girl.
-3: kid.
}
* [YOU: Wow.] <>
{moralityCHOICE < 0: 
 What a shit hole.
 -else:
 It's not...awful...
 }
Percival flips several switches near the door. The lights come on and it becomes clear the Town Hall is a repurposed dance hall. No offices, a sole scaffold of lights across the middle of the ceiling holds ancient spill lights. 
YOU: When was the last time anything was turned on here?
PERCIVAL: Not since you were sucking on your--
MAYOR: It has been quite some time. However, whatever you need to make us ready for a streaming festival, please let me know. 
-
* [Clean the hall.]
    YOU:<> {moralityCHOICE > 0: 
    First thing's first. Gotta make sure this place looks its best. So I'll get to work on cleaning up. ->DONE
    -else:
    This place is disgusting. So I guess we should start with that.->DONE
    }
* [Set up commercial wifi hotspot.]
    YOU: <>{moralityCHOICE > 0:
    Can't have a streaming festival without a hotspot, so I should get started there.
    -else:
    You luddites probably don't have a wifi hotspot set up. Time for you all to crawl out of the nineteenth century.
    }
-
MAYOR: Whatever you need, you just let Percival know and he'll do his best to get you the materials. Won't he?
PERCIVAL: Uh huh. I'll get right on that.
YOU: <>{moralityCHOICE > 0:
Great! Really appreciate your help, Percival.
Percival growls at you.
-else:
YOU: Great. Gonna be such a joy working with you.
Percival wheezes a crackling laugh.
}
-
MAYOR: Now if there's nothing else, I'll be on my way. Thank you for coming. 
The mayor's clacking footsteps echo in the large open room. //Were we able to add these footsteps to an animated cutscene then I would cut this out. You'll have to let me know what we can do.
->DONE