# Scenario Testing Engine

`NeverTest` is a library that enables scenario based testing.
It provides mechanism to define executable specifications
to primarily test .NET based solutions.

Take BDD example:
```
Feature: User Registration

  Scenario: User registers successfully

    Given the user is on the registration page
    When the user enters their valid registration details
      | field    | value               |
      | Username | newuser             |
      | Email    | newuser@example.com |
      | Password | password123         |
    And the user submits the registration form
    Then the user should see a registration success message

  Scenario: User registration fails due to username being taken

    Given the user is on the registration page
    And the username "existinguser" is already taken
    When the user enters their registration details with the taken username
      | field    | value               |
      | Username | existinguser        |
      | Email    | newemail@example.com|
      | Password | password123         |
    And the user submits the registration form
    Then the user should see an error message
      | message  | Username already taken. Please choose another one. |
```

Here is how this could be expressed using technically oriented scenario:

```yaml
scenarios:
- name: should register new user
  given:
    clock: 2003-01-01
  when:
    $registration:
      http:
        url: /api/register
        method: post
        body:
          username: newuser
          email: newuser@example.com
          password: password123
  then:
    $registration:
      matches:
        status: 200
    db:
      users:
        query: email == "newuser@example.com"
        matches:
          user_name: newuser
          reg_date: 2003-01-01

- name: should now allows users with existing usernames
  given:
    db:
      users:
        - username: existing
          email: newemail@example.com
  when:
    http:
      url: /api/register
      method: post
      body:
        username: existinguser
        email: other@example.com
        password: password123
  then:
    matches:
      status: 400
      body:
        type: https://example.com/errors/username-taken
        title: Username already taken
        status: 400
        detail: Username already taken. Please choose another one.
```
















