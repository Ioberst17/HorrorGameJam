INCLUDE Variables.ink
INCLUDE Functions.ink
INCLUDE Bartender.ink
INCLUDE Groundskeeper Dialogue.ink
INCLUDE Fire Chief.ink
INCLUDE Sheriff.ink
INCLUDE GKR townhall interior.ink
INCLUDE Mechanic.ink
INCLUDE Demon Intro.ink
INCLUDE Demon Outside.ink




VAR moralityCHOICE = 1
VAR moralitySCORE = 20
VAR pronoun = 1
VAR sheriffTALKS = 0
VAR bartenderTALKS = 0
VAR mechanicTALKS = 0
VAR mayorTALKS = 0
VAR groundskeeperTALKS = 0
VAR firechiefTALKS = 0
VAR LOCATION = 0
VAR dungeonVISITS = 0

->demmon_dungeon_check
===Start_Game===
CHOOSE YOUR PRONOUN:
* [Male]
~changepronoun(1)
-> player_Start
* [Female]
~changepronoun(2)
->player_Start
* [Non-binary]
~changepronoun(3)
-> player_Start

=== player_Start
The phone rings.
"Mom" flashes on the screen. 
* [Answer the phone.]
    ~changemoralityCHOICE (1)
   PLAYER: Hi mom.
   MOM: Found a job yet?
   PLAYER: No, not yet. 
   MOM:Well then I've got good news.
   PLAYER: Hit me.
   MOM: Remember that little town, Muldenberry we stopped at on our way to visit your grandfather?
    **[Yes]
   YOU: Yeah, I think we just stopped at a bar or something to eat. 
    MOM: That's right! I'm surprised you remember that, you were pretty young.
    YOU: What about it?
    ->mom_news
    **[No] 
    MOM: That's not surprising, there wasn't much there. We stopped for lunch at a bar, of all places.
    YOU: The news?
    -> mom_news
* Ignore.
~changemoralityCHOICE (-1)
    Your phone rings again.
    **[Ignore again.]
    ~changemoralitySCORE(-1)
    Your phone dings. It's a text message from her.
    It reads, "I know you're just sitting there watching Netflix. Answer the phone, or I'll stop paying for the subscription."
    She calls again.
        ***[Answer.]
     MOM: Would it kill you to answer the phone the first time?
     YOU: Hello, <>
     {pronoun:
     -1: son.
     -2: daughter.
     -3: spawn.
     }
     <> How are you? Oh I'm fine, just working on that job search. 
     MOM: *sighing* Hello, mother. Why yes I am happy to hear from you, the person who pushed for twelve hours because you refused to come out and now can't pick up the phone when she calls anymore.
     YOU: Because guilt trips are so effective.
     MOM: I'm just saying I can be snide too. Listen. Remember that town, Muldenberry we drove through to visit your grandfather?
      **** [Yes.]
        MOM: I'm surprised you remember that, you were pretty young.
        ***** [YOU: The news?]-> mom_news
    **** [No.] 
        MOM: That's not too surprising. You were pretty young.
        *****[YOU: The news?] -> mom_news
    **[Answer].
    MOM: Would it kill you to answer on the first call?
    YOU: It might. 
    MOM: Smart ass. Listen I have some news. Remember that town, Muldenberry we drove through to visit your grandfather?
    *** Yes.
        MOM: I'm surprised you remember that, you were pretty young.
        **** [YOU: The news?]-> mom_news
    *** No. 
        MOM: That's not too surprising. You were pretty young.
        ****[YOU: The news?] -> mom_news
    ->mom_news
    
    =mom_news
    {moralityCHOICE > 0:
    MOM: Right! Well they're putting on a festival of sorts to try and drum up some quick tourism. It's just a quick streaming wifi and maybe set up some speakers and lights. Interested? 
    YOU: I'm packing right now.
    -else:
    MOM: The mayor of that town contacted me to set up wifi streaming for a festival they're putting on. 
    YOU: Sounds boring.
    MOM: Sounds paid. Do you want the gig or not?
    YOU: Yeah. I guess. 
    MOM: Okay, well then I suggest you get packing. They want you to start setting up tomorrow.
    }
    
    ->DONE
    