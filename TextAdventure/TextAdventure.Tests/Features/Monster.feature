Feature: Monster regels

  Scenario: Vluchten uit monsterkamer zonder te vechten is dodelijk
    Given I am at Start
    When I go south
    And I go south
    And I go north
    Then I should be dead

  Scenario: Met zwaard vechten en veilig vertrekken
    Given I am at Start
    When I go south
    And I take sword
    And I go south
    And I fight
    And I go north
    Then I should be alive
