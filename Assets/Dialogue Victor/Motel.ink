=motel_dungeon_check
{dungeonVISITS > 0: 
You wake from your dream. The sun shines through the curtains in the window. You roll over onto your side, rubbing the sleep out of them. You check the time on the clock and groan. Time to go to work. ->DONE
-else:
->motel_entrance
}

=motel_entrance
{moralityCHOICE > 0:
You sigh and droop your head before you head in.
YOU: Why did I expect this not to be disgusting.
-else:
You sigh.
YOU: At least I have somewhere to sleep.
}
- As you enter the room, the waft of must and dust comes over you. You cough and drag your bag over the dresser and put your clothes away.
* [Go to sleep.] ->demmon_dungeon_check
* [Explore.] ->
->DONE

=motel_lobby
When you enter the lobby, a tired young girl sighs.
MOLLY: Hey.
{moralityCHOICE > 0:
YOU: Not the greatest customer service.
She rolls her eyes at you, puts on a saccharine smile, and flashes the most insincere smile she can muster. 
MOLLY: Hi! I'm Molly! Welcome to Muldenberry Inn! The most prestigious rotting corpse of a building in Muldenberry. Here you'll find the best that this town has to offer like cockroaches, shit coffee, and exemplary customer service. How can I make your dreams come true today?
-else: 
YOU: Hey.
She nods and goes back to scrolling on her phone.
}
-
* [Chat.] -> motel_loop
* [Explore.] ->DONE

=motel_loop
{motelTALKS > 0:
{~What?| ... | Busy.| No hablo ingles.}
+ [Talk to her again.] ->motel_loop
}
MOLLY: What?
* [Ask about the town.]
    YOU: What is there to know about Muldenberry?
    MOLLY: Not a lot. Small town, shit education, dial-up speeds, and a good amount of racism. It's a great time.
    ** [It can't be that bad.]
        MOLLY: Do I look like I'm having a good time? 
        YOU: Not really.
        MOLLY: Well there you go. If you don't need anything like a towel or something, I got other shit to do.
        +++ [Talk to her again.]
            ~changemotelTALKS (1)
            ----->motel_loop
        *** [Leave.]
        --->DONE
    ** [Sounds about right.]
        MOLLY: Yup. Now if you don't need a towel or anything I got other shit to do.
        +++[ Talk to her again.]
        ~changemotelTALKS (1)
        ----> motel_loop
       *** [Leave.] --->DONE
*[Nevermind.] ->DONE