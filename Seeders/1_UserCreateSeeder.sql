INSERT INTO
    users (name, email, password, created_at)
VALUES
    (
        'Super Admin',
        'admin@dot.co.id',
        '$2a$11$dGJAHAdnLSgjDxqg68W4leL2birufffmtgz3oeOtVBgdFKZPl2D.G',
        GETDATE()
    ),
    (
        'User',
        'user@dot.co.id',
        '$2a$11$dGJAHAdnLSgjDxqg68W4leL2birufffmtgz3oeOtVBgdFKZPl2D.G',
        GETDATE()
    )