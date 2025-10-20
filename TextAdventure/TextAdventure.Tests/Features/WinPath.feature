Feature: Win path

  Scenario: Speler pakt sleutel en opent deur
    Given I am at Start
    When I go east
    And I take key
    And I go west
    And I go north
    Then I should win
