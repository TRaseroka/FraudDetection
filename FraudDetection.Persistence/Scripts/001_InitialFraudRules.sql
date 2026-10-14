
INSERT INTO "FraudRuleConfigurations"
(
    "Id",
    "Code",
    "Name",
    "Description",
    "IsEnabled",
    "RiskScore",
    "ParametersJson",
    "CreatedAt",
    "UpdatedAt"
)
VALUES
(
    gen_random_uuid(),
    'HIGH_VALUE_TRANSACTION',
    'High Value Transaction',
    'Detects transactions exceeding the configured threshold.',
    TRUE,
    70,
    '{"threshold":10000}'::jsonb,
    NOW(),
    NOW()
),
(
    gen_random_uuid(),
    'LARGE_CASH_DEPOSIT',
    'Large Cash Deposit',
    'Detects unusually large cash deposits.',
    TRUE,
    40,
    '{"threshold":5000}'::jsonb,
    NOW(),
    NOW()
),
(
    gen_random_uuid(),
    'HIGH_TRANSACTION_VELOCITY',
    'High Transaction Velocity',
    'Detects multiple transactions within a short time window.',
    TRUE,
    60,
    '{"transactionCount":3,"windowMinutes":10}'::jsonb,
    NOW(),
    NOW()
)
ON CONFLICT ("Code") DO NOTHING;
