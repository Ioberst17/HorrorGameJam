EXTERNAL SaveCurrent()
EXTERNAL SaveNew(int)

EXTERNAL SeeIfFileHasBeenSavedBefore()
EXTERNAL GetSpecificFilePlayTime(int)

EXTERNAL PlaySaveSound()

->Start

== Start ==
Hey, kid - wanna save your game?
* [Yeah!] -> CheckSaveGame
* [Nevermind] -> Done

== CheckSaveGame ==
{SeeIfFileHasBeenSavedBefore(): -> SaveCurrentGame |-> SaveGameOptions }


== SaveGameOptions ==
Looks like you haven't saved on this journey before. Pick a card, any card.
* [Save to File 1 (Total time: {GetSpecificFilePlayTime(1)}, Player location: City)] -> SaveFile1
* [Save to File 2 (Total time: {GetSpecificFilePlayTime(2)}, Player location: Forest)] -> SaveFile2
* [Don't Save] -> Done

== SaveFile1 ==
~SaveNew(1)
~PlaySaveSound()
-> DoneSaving

== SaveFile2 ==
- { ~SaveNew(2) }
~PlaySaveSound()
-> DoneSaving

== SaveCurrentGame ==
~SaveCurrent()
~PlaySaveSound()
-> DoneSaving  

== ERROR_SaveGame ==
An error occurred while saving the game. Please try again.
-> SaveGameOptions

== DoneSaving ==
    -> Done

== Done ==
-> END

//fallback functions for Editor testing

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