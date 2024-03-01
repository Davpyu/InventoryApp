INSERT INTO
    users (name, email, password, created_at)
VALUES
    (
        'Super Admin',
        'admin@pama.dot.co.id',
        '$2a$11$dGJAHAdnLSgjDxqg68W4leL2birufffmtgz3oeOtVBgdFKZPl2D.G',
        GETDATE()
    ),
    (
        'Counter',
        'counter@pama.dot.co.id',
        '$2a$11$dGJAHAdnLSgjDxqg68W4leL2birufffmtgz3oeOtVBgdFKZPl2D.G',
        GETDATE()
    ),
    (
        'Head Counter',
        'head.counter@pama.dot.co.id',
        '$2a$11$dGJAHAdnLSgjDxqg68W4leL2birufffmtgz3oeOtVBgdFKZPl2D.G',
        GETDATE()
    );