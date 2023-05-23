=Tutorial_Intro
DEMON: You made it! See, falling down here didn't hurt at all did it?
DEMON: Now then. Try jumping around.
->DONE

=Jump_Success
DEMON: Great job! I bet you can't jump like that when you're awake right?
{moralityCHOICE >0:
YOU: No, but pretty close! I do like to exercise.
-else:
YOU: God no. That sounds like exercise.
}
->DONE

=Tutorial_DASH
DEMON: Not only can you jump, but you can dash too! Just think like a hummingbird. Zipping through the air.
->DONE

=Tutorial_DASH_SUCCESS
DEMON: You're a natural. Gifted. Exceptional. Prolific. Sup—
{moralityCHOICE > 0: 
YOU: Thank you.
DEMON: 
-else:
YOU: Shut up.  
}
->DONE

=tutorial_MELEE_pickup
//OBJ: Kill useing jump and melee.
DEMON: See that dog ... thing?
YOU: A Hellhound?
DEMON: You can't say that. It's offensive for non-demons to use that term.
{moralityCHOICE > 0:
YOU: Oh, I'm sorry. I didn't mean to off—
DENMON: I'm just kidding! Yes. A Hellhound. Well you're going to fight it. First you need something to hit it with. See if you can find a good bashing tool.
-else: 
YOU: I didn't know demons were SJWs.
DEMON: Hah! Good one. I was just trying to joke you see? We're not very funny. In any case, you're going to fight it. First, you need something to hit it with. See if there's anything lying around.
}
->DONE

=tutorial_MELEE_hit
DEMON: Okay, now that you have your weapon, run up to it and smash its face in!
->DONE

=tutorial_MELEE_hit_SUCCESS
DEMON: Great job! See how it's bleeding and dead? We like that. Now on to the next area!
->DONE

=tutorial_RANGED
//OBJ: Shoot the bats.
DEMON: You know what's better than hitting things?
{moralityCHOICE > 0:
YOU: N—no.
DEMON: Shooting them! 
YOU: Right..
DEMON: Oh look! A gun! You should go pick it up and try it out.
-else: 
YOU: Shooting them?
DEMON: Exactly!
YOU: Well hey, a revolver. That's convenient.
DEMON: What a dream!
}
DEMON: Anyway, those bats over there look pretty menacing don't they? You should do something about that.
->DONE

=tutorial_RANGED_SUCCESS
DEMON: They didn't stand a chance! Bullet beats demon. Onward!
->DONE

=tutorial_BLOCK_pickup
//OBJ: Pick up shield. 
DEMON: Oh no! The hellhoud respawned! It's going to attack you, but it's a good thing you can block.
YOU: With what?
DEMON: Um ... the manhole! Use it as a shield!
->DONE

=tutorial_BLOCK
//OBJ: Block ranged attack. 
DEMON: It's going to spit at you! Watch out!
->DONE

=tutorial_BLOCK_SUCCESS
DEMON: Phew. That was close, but I wasn't worried for you at all. Now comes the fun part.
DEMON: Let's move on. 
->DONE

=tutorial_env_EXPLODE
//OBJ: Open up the way to the next area.
DEMON: You've played videogames before right?
* Yes.
    DEMON: So you know what red barrels with an explosive sign on them do. Go ahead. Shoot it!
    ->DONE
*No.
    DEMON: Oh my. Okay, well when you see a big red barrel like that one over there? You shoot it and it explodes. ->DONE
=tutorial_complete
//Defeat the final boss.
DEMON: That's all I have for you! You're a master already, go out there and have fun! But remember. The sun will rise soon. See if you can kill the last boss before it does. I believe in you!
->DONE

=tutorial_GARGOYLE
DEMON: Be careful. This one is a bit angrier than the others. Just remember your training! You'll get through it. ... Hopefully. 
->DONE

=tutorial_GARGOYLE_SUCCESS
DEMON: Yesssss. One step closer to helpi— I mean, great job defeating that nasty demon.
*Thanks.
    DEMON: No, thank you. Just in time too, the sun is almost out.
*What were you going to say?
    DEMON: My, look at the time! The sun is coming up.
-DEMON: Time to wake up now.
->DONE
