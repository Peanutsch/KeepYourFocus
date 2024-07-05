/*
 * Simon Says-like game with some level based challenges
 * Each level has 6 sequences. After 6 succesful sequences:
 * Level++; Add 1 challenge; Clear correctOrder and playerOrder and start with new sequence = 1
 * From Level >= 6: no Clear correctOrder; sequences++ untill game over
 * More challenges in progress
 * 
 * === Levels ===
 * Level 1 [EasyPeasy]: standard
 * level 2 [OkiDoki] and onward: some misleading text in pictureboxes, plus:
 *                               Shuffle Pictureboxes before start sequence with 55% chance; level 3 75% chance; level 6 100% chance
 * level 3 [Please No]: Shuffle Pictureboxes per player click with 55% chance; level 4 75% chance; level 7 100% chance
 * Level 4 [No Way!]: In each sequence, swap one tile order with 55% chance; level 5 75% chance; level 8 100% chance
 * level 5 [HELL NO]: There is a chance that 1 or more colors get swapped by other colors in the running order with 55% chance; level 6 75% chance; level 9 100% chance
 * level 6 [...]: 
 * level 7 [...]: 
 * level 8 [...]: 
 * level 9 [...]: 
 * level 666 [HELLMODE]: No Clear correctOrder; sequences++ untill game over
 */