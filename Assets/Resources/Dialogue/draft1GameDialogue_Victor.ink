-> Phase_1_Groundskeeper

=== Phase_1_Groundskeeper

GROUNDSKEEPER: So you're the hotshot techy who's gonna save our town, eh?
* PLAYER: That's me.
    GROUNDSKEEPER: Feh.
* PLAYER: Don't know about save, but I'll do my job.
    GROUNDSKEEPER: Uh huh.
* PLAYER: There's no saving this shithole. I'm just here for a paycheck.
    GROUNDSKEEPER: That's some mouth on you.
- PLAYER: Sorry. Long trip. 
GROUNDSKEEPER: Whatever you say. Well let's get this over with. This here is the town hall. You'll be working inside.
GROUNDSKEEPER: Shall we?
PLAYER: Might as well.
->interior_intro_townhall

=interior_intro_townhall
GROUNDSKEEPER: Well short tour, but this is the town hall. 
PLAYER: This'll be your office.
The Groundskeeper waves at a collection of dusty fold up chairs lined up in two colums with several rows of six with a narrow path between each set. A small podium, warped by age and coverd in dust and cobwebs stands alone on a slightly raised stage with tattered curtains and 
->intro_house_entry

=intro_house_entry
The entryway of the house opens to a dining room with a small table and two chair. To your right is a fireplace. Opposite the fireplace to your left is a wall with a void where something ought to be. The stairs that lead to the second floor await across the room, past the kitchen.
PLAYER: Man what a day.
    * Examine a strange empty space on the wall.
        PLAYER: Huh. Hooks? Looks like it was for a sword. -> intro_house_entry
    * The mantle of the fireplace has an empty wooden stand. 
        PLAYER: I wonder what this is for... -> intro_house_entry
    * A dusty coatrack looms next to the staircase leading to the bedroom.
        PLAYER: Seems as good a place as any to put my coat on. -> intro_house_entry
        * ->
- I should go to bed. Long day tomorrow. 
* Go to the bedroom.
    ~ ChangeLocation(bedroom)
-> intro_bedroom

=intro_bedroom
{ 
    - Location has bedroom:
    A twin bed sits against the wall in the middle of the room. A dresser sits beside it and a singular window looks out into the street.
PLAYER: Can't say I was expecting much. 
    *Climb into bed. 
-> intro_demon_start
}


= intro_demon_start
Your eyes flutter closed, and just as you're about to drift into sleep, you hear a creak and a grumble. 
You open your eyes and a demon is laying on top of you, crushing your chest and stifling your breath. 
    *What the f--
        DEMON: Oh, my! So sorry to startle you like this. 
    *Stay absolutely still.
        DEMON: Hello!
        You remain still and quiet.
        DEMON: Oh dear. I fear I may have frightened you, but please don't worry. 
        DEMON: I'm really quite harmless.
- PLAYER: What are you?
The demon flashes a grin.
    DEMON: Well, I would have thought my, admittedly unnerving, appearance would have given it away. 
    DEMON: But, what I am really isn't terribly important. We are, afterall, in your dream aren't we?
    * PLAYER: This is a dream?
        DEMON: Why yes! I'll prove it to you. Look at the clock.
        You do as instructed. The numbers are an illegible jumble.
        PLAYER: I...I can't read them.
        DEMON: Well there you have it! 
    * PLAYER: You're a demon.
        DEMON: That's a bit harsh, but I can see why you would label me like that. So if it's easier to think of me that way then, yes. I suppose I am.
- DEMON: Now, before we get too off-track I have...something of a proposition for you.
PLAYER: O...kay...
DEMON: Since I am your subconcious, what do you say we go on a little bit of an adventure. You've always wanted to be a hero, yes?
* PLAYER: Yes...
* PLAYER: Not really.
    DEMON: Well, I think you do. I think since you're dreaming about me telling you that you want to be a hero means you want to be a hero. Right?
    PLAYER: I..guess...
- DEMON: Great! Then this will be super easy. How about we go kill some monsters? I mean there's no harm in killing a bunch of monsters in a dream right?
PLAYER: I suppose not.
-> choose_weapon

=choose_weapon
LIST Inventory = (sword), manholeCover, huntersRifle, hammer, spear, bloodBubbles, flamethrower, fireworks, fireAxe, (flashlight)
LIST Location = dungeon, (townHall), bedroom, fireStation, policeStation, mechanicShop, mayorsHouse

DEMON: Superb! Then here, every hero needs a weapon. Why don't we start with this sword?
A sword has been added to your Inventory.
DEMON: You know what's better than one weapon?
PLAYER: 
    * PLAYER: Two weapons?
      DEMON: Winner Winner Person Dinner!
      PLAYER: Don't you mean chicken?
      DEMON: Yes, of course, you must have gotten that confused. Moving along.
    * PLAYER: What?
      DEMON: TWO weapons!
- DEMON: And now you get to pick your second one!
    *Manhole Cover
        DEMON: Excellent choice! The best offense is the best defense, right? Or was it the other way around?
        ~Inventory += manholeCover
    *Hunter's rifle
        DEMON: Perfect choice! Best to kill effectively from all ranges. Best defense is the best offence, right? Or...
        ~Inventory += huntersRifle
    *Hammer
        DEMON: Yes, yes! Smash things! Make them squishy, splashy, and-- I mean, yes. Hammers have all sorts of uses.
        ~Inventory += hammer
- DEMON: Ready?
PLAYER: I guess so...
DEMON: Then away we go!
-> demon_tutorial

= demon_tutorial
>>> Player lands in the first level of the dreamscape dungeon. It's a weird warped version of the town and a hellhound is patrolling just outside the door of the house. 
PLAYER: What the...
DEMON: Don't worry. This is just a dream remember? That Shambling Horror can't hurt you. But it's super scary right?
PLAYER: Gross...I don't know about scary.
DEMON: Come now, he's not that ugly is he? Nevermind. The point is, you should go and slash him with your sword!
>>> Player successfully lands a hit.
DEMON: {
-Inventory has manholeCover:
DEMON: Okay, now try blocking its attack.
>>> Player successfully blocks attack.
DEMON: Great work! Now you can also bash it. Give a try! I bet it makes a satisfying crunch.
-Inventory has huntersRifle:
DEMON: Now let's try shooting it in the face!
-Inventory has hammer:
DEMON: Now do the same thing, but switch to your hammer.
}
DEMON: Great job! Now see that shiny, spinny thing? Go pick it up. Make sure you keep it in your pocket. 
PLAYER: What is it?
DEMON: It's...uh...what do you think it is?
PLAYER: Looks kind of like a candy.
DEMON: Yes! That's candy! Absolutely. No need to ask anything more. Now go be a hero! Oh, and remember: You'll have to wake up eventually so try to the make the most of it!
->demon_return

= demon_return
After a long day's work, you head back to the apartment. About to head straight upstairs, something catches your eye.
    * [Sword Mount]
        The sword from your dreams is resting within the mounts.
                {
        - Inventory has manholeCover:
            PLAYER: Weird. I thought this was empty, and I definitely think I would have remembered the shield and sword being put on there. Maybe the landlord put it up while I was working.
        - Inventory hasn't manholeCover:
            PLAYER: Weird. I thought that was empty before. Must have just missed it.
            }
    * [Wooden Mount]
                {
                - Inventory has huntersRifle: 
                Wait. That was definitely empty before. Huh. Must have been added after. I'll have to ask the Groundskeeper if he's redecorating while I'm out.
                - Inventory hasn't huntersRifle:
                PLAYER: Still empty.
                }
    * [Coat Rack]
        PLAYER: Still empty. 
    
- You head upstairs and lay down in bed. Before you finish yawning you drift into sleep.
DEMON (OS): Hello, again! 
PLAYER: Jesus, you scared the shit out of me.
DEMON: I'm so sorry about that. Nothing I can do about my appearance, sadly. I mean, considering we're in your mind you must just want me to look this way. 
PLAYER: That...makes sense? I think? 
DEMON: Sure it does! Anyway, because you did so well the last time, do you have that candy?
PLAYER: I think so.
DEMON: Wanna trade me some of them for cool new things?
PLAYER: Can't I just give myself the cool new thing?
DEMON: Hahaha! You're so funny! Sure, you could, but apparently you don't want to since you're dreaming me asking you to give it to me.
PLAYER: My head hurts.
DEMON: Hey, don't worry about it. Anyway, let me show you what I have! Then you can go back to being a hero again!
>>> After player makes purchases.
DEMON: All set? Ok well have fun down there!
-> DONE

= demon_phase_1_regular_shop_open
DEMON: Hi, again! Want a new toy?
-> demon_phase_1_regular_shop_close

= demon_phase_1_regular_shop_close
DEMON: Have fun! Remember, be the hero!
-> DONE

-> Phase_2_Groundskeeper

=== Townsfolk_Dialogue ===
= fire_chief
CHIEF: I don't care what you do here, city slicker, just don't make me have to work harder than I have to. 
->DONE

= mayor
MAYOR: How did you get in here? Leave before I call my driver to throw you out.
-> DONE

= sheriff
Sheriff: Not gonna have a problem with you am I? I know how you young millenial types like to party. If I catch one whiff of that mary jane, you will feel my wrath.
-> DONE

= mechanic
PLAYER: Leroy, I presume?
TOBY: Leroy's my dad. What can I do for you?
-> DONE
=== function ChangeLocation(newLocation)
~ Location += bedroom

=== Phase_2_Groundskeeper
GROUNDSKEEPER: Seems you're making some progress.
PLAYER: Well, all I've done is cleaned up a bit. Still have a lot to go before this place is ready.
GROUNDSKEEPER: Well gotta at least give you credit. Didn't expect someone as soft-handed as you to work this hard.
* PLAYER: Thanks?
* PLAYER: That's supposed to be a compliment?
* PLAYER: Don't know any other way to be.
- -> DONE
