EXTERNAL UpdateQuest()
EXTERNAL SaveCurrent()
EXTERNAL SaveNew(int)

EXTERNAL SeeIfFileHasBeenSavedBefore()
EXTERNAL GetSpecificFilePlayTime(int)

EXTERNAL PlaySaveSound()
EXTERNAL PlaySaveVFX()
EXTERNAL PlaySaveVFX2()

->Start

== Start ==
Hey, kid - wanna save your game?
* [Yeah!] -> CheckSaveGame
* [Nevermind] -> END

== CheckSaveGame ==
{SeeIfFileHasBeenSavedBefore(): {SaveCurrent()} {PlaySaveSound()} {PlaySaveVFX()} {PlaySaveVFX2()} {UpdateQuest()}-> END |-> SaveGameOptions }


== SaveGameOptions ==
Looks like you haven't saved on this journey before. Pick a card, any card.
* [Save to File 1 (Total time: {GetSpecificFilePlayTime(1)}, Player location: City)] -> SaveFile1
* [Save to File 2 (Total time: {GetSpecificFilePlayTime(2)}, Player location: Forest)] -> SaveFile2
* [Don't Save] -> Done

== SaveFile1 ==
~SaveNew(1)
~PlaySaveSound()
~PlaySaveVFX()
~PlaySaveVFX2()
-> END

== SaveFile2 ==
~SaveNew(2)
~PlaySaveSound()
~PlaySaveVFX()
~PlaySaveVFX2()
-> END

== SaveCurrentGame ==
~SaveCurrent()
~PlaySaveSound()
~PlaySaveVFX()
~PlaySaveVFX2()
-> END

== ERROR_SaveGame ==
An error occurred while saving the game. Please try again.
-> SaveGameOptions

== DoneSaving ==
    -> Done

== Done ==
-> END

//fallback functions for Editor testing

=== function UpdateQuest() ===
~ return 0

=== function SaveCurrent() ===
~ return 0


=== function SaveNew(int) ===
~ return 1


=== function SeeIfFileHasBeenSavedBefore() ===
~ return false


=== function GetSpecificFilePlayTime(int) ===
~ return int

=== function PlaySaveSound() ===
~ return 1

=== function PlaySaveVFX() ===
~ return 1

=== function PlaySaveVFX2() ===
~ return 1