=firechief_dungeon_check
{dungeonVISITS > 0: 
The firechief's mustache frays at the edges, what little hair he had left is disheveled along with his uniform. Dark circles sit under his eyes.
*[Approach with caution.]
->firechief_post_dungeon
-else: 
The fire chief looks up from a book in his lap as he sits on a lawn chair in the fire engine garage.
* [Greet him.]
->fire_chief
}

=fire_chief
FIRE CHIEF: Listen <>
{pronoun:
-1: son.
-2: sweetheart.
-3: kid.
}
<>  I haven't had any emergencies other than the Mayor twisting her ankle in those ridiculous shoes of hers. Don't complicate my life, I won't complicate yours.

{moralityCHOICE > 0: 
*[I'll do my best to be good.]
    FIRE CHIEF: Glad to see strong morals in the youth.->firechiefloop
- else:
* [No promises, old man.]
    He sighs and returns to his book.
-> firechiefloop
}
= firechiefloop
 {firechiefTALKS < 1: 
+ [YOU: Quick question.]
{changefirechiefTALKS (1)}
->firechiefloop2
    - else:
+ [YOU: Hey.]
->firechiefloop2
}

=firechiefloop2
FIRE CHIEF: {~Already said everything I need to.|Don't make me raise my voice.| I got a lot of patience, but it ain't infinite.| Door's that way.}
    ++ [Talk to him again.] ->firechiefloop
    **[Leave]
    -->DONE
* [Leave.]
->DONE

=firechief_post_dungeon
* Ask if he's okay.
    FIRE CHIEF: What? I'm fine.
    ** Double-check.
        {moralityCHOICE > 0: 
        YOU: Are you sure? You don't look so good. ->firechief_post_dungeon_loop
        -else:
        YOU: You look like shit. -> firechief_post_dungeon_loop
        }
=firechief_post_dungeon_loop
{firechief_post_dungeon_loop < 1: 
FIRE CHIEF: I said I'm fine. Now get out.
- else:
FIRE CHIEF: {~Get out.|Scram.|Don't push it.}
}
+ Ask again. ->firechief_post_dungeon_loop
*Leave. ->DONE
