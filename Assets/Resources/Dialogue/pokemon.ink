-> main

=== main ===
Which pokemon do you choose?
    +[Charmander] 
        #positive
        -> chosen("Charmander")
    +[Bulbasaur]
        -> chosen("Bulbasaur")
    +[Squirtle] 
        #negative
        -> chosen("Squirtle")

=== chosen(pokemon) ===
You chose {pokemon}!
-> END