Feature: Mini Text Adventure
  As a player
  I want to explore the rooms, take items, fight monsters, and reach the win condition
  So that I can fully test the game rules

  Background:
    Given the game is started

  Scenario: Player wins by collecting the key and opening the door
    Given the player is in the Start-Room
    When the player goes "right"
    And the player takes "key"
    And the player goes "up"
    Then the player should see "Congratulations, you win!"

  Scenario: Player dies by entering the deadly room
    Given the player is in the Start-Room
    When the player goes "left"
    Then the player should see "Game Over"

  Scenario: Player dies by trying to leave monster room alive
    Given the player is in the Sword-Room
    And the player takes "sword"
    And the player goes "down"
    When the player tries to go "up" without fighting
    Then the player should see "You died"

  Scenario: Player fights and defeats the monster safely
    Given the player is in the Sword-Room
    And the player takes "sword"
    And the player goes "down"
    When the player fights
    Then the monster should be dead
    And the player can go "up" safely

  Scenario: Player tries to go through locked door without key
    Given the player is in the Start-Room
    When the player goes "up"
    Then the player should see "The door is locked. You need a key!"