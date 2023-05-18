=demmon_dungeon_check
{dungeonVISITS > 0:
DEMON: Hello again! 
YOU: Same deal?
DEMON: That's right! Get out there and bash some brains in. Or shoot them. Or...I'll stop talking. 
*Leave. ->DONE
-else:
-> demon_intro
}

=demon_intro
Your eyes feel heavy as you lay down in bed. You feel the familiar weightlessness of falling into a deep sleep. Your vision dims, but just as you're about to fall asleep you feel a heavy pressure on your chest. You open your eyes to see a demon leaning on you.
DEMON: Good evening!
{moralityCHOICE > 0: 
*Be polite.->demon_intro_good
-else:
*Curse.->demon_intro_bad
}
*Scream!
    DEMON: I'm so terribly sorry to startle you. Truly. With a face like mine there just isn't a good way to introduce myself.
*Stay silent.
    DEMON: Strong silent type. I like it. 

-YOU: Am I dreaming?
DEMON: Well ... demons like me certainly aren't real. Right?
* Agree.
    YOU:Right. 
    DEMON: Then the only reasonable answer is that you're dreaming! Since we're on the topic of you dreaming. I have an idea.
* Disagree
    YOU: No I think demons are real.
    DEMON: Oh, you do? Well, then you will be glad to know that I am a Somnus Daemonium Parasomnis.
    YOU: I don't know latin.
    DEMON: Colloquially, I'm a sleep paralysis demon. In answer to your original question. Yes! You're asleep and dreaming. Speaking of which, I have an idea.
-YOU: What's that?
DEMON: What do you say about living through your own little version of a dark video game? Go out there, and kill some demons for a little while before you wake up, sound good?
* Yes.
    YOU: Ye—
* No.
    YOU: N—
-DEMON: Great! Let's go.
->DONE

=demon_intro_good
YOU: H-Hello.
DEMON: So polite! You know. I bet everyone likes you.
YOU: Am ... am I dreaming?
DEMON: Well, demons like me certainly aren't real. Right?
* Agree.
    YOU:Right. 
    DEMON: Then the only reasonable answer is that you're dreaming! Since we're on the topic of you dreaming. I have an idea.
* Disagree
    YOU: I think demons are real.
    DEMON: Oh, you do? Well, then you will be glad to know that I am a Somnus Daemonium Parasomnis.
    YOU: I don't know latin.
    DEMON: Colloquially, I'm a sleep paralysis demon. In answer to your original question: Yes! You're asleep and dreaming. Speaking of which, I have an idea.
-YOU: What's that?
DEMON: What do you say about living through your own little version of a dark video game? Go out there, and kill some demons for a little while before you wake up, sound good?
* Yes.
    YOU: Ye—
* No.
    YOU: N—
-DEMON: Great! Let's go.
->DONE

=demon_intro_bad
YOU: What the actual fuck?!
DEMON: Quite the vibrant vocabulary. Your mother must be very proud of you. 
YOU: Is this some sort of shitty dream?
DEMON: Well, demons like me certainly aren't real. Right?
* Agree.
    YOU:Right. 
    DEMON: Then the only reasonable answer is that you're dreaming! Since we're on the topic of you dreaming. I have an idea.
* Disagree
    YOU: No I think demons are real.
    DEMON: Oh, you do? Well, then you will be glad to know that I am a Somnus Daemonium Parasomnis.
    YOU: I don't know latin.
    DEMON: Colloquially, I'm a sleep paralysis demon. In answer to your original question. Yes! You're asleep and dreaming. Speaking of which, I have an idea.
-YOU: What's that?
DEMON: What do you say about living through your own little version of a dark video game? Go out there, and kill some demons for a little while before you wake up, sound good?
* Yes.
    YOU: Ye—
* No.
    YOU: N—
-DEMON: Great! Let's go.
->DONE