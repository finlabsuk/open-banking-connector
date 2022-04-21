CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT pk___ef_migrations_history PRIMARY KEY (migration_id)
);

START TRANSACTION;

CREATE TABLE bank (
    id uuid NOT NULL,
    issuer_url text NOT NULL,
    financial_id text NOT NULL,
    reference text NULL,
    is_deleted boolean NOT NULL,
    is_deleted_modified timestamp with time zone NOT NULL,
    is_deleted_modified_by text NULL,
    created timestamp with time zone NOT NULL,
    created_by text NULL,
    CONSTRAINT pk_bank PRIMARY KEY (id)
);

CREATE TABLE account_and_transaction_api (
    id uuid NOT NULL,
    bank_id uuid NOT NULL,
    api_version text NOT NULL,
    base_url text NOT NULL,
    reference text NULL,
    is_deleted boolean NOT NULL,
    is_deleted_modified timestamp with time zone NOT NULL,
    is_deleted_modified_by text NULL,
    created timestamp with time zone NOT NULL,
    created_by text NULL,
    CONSTRAINT pk_account_and_transaction_api PRIMARY KEY (id),
    CONSTRAINT fk_account_and_transaction_api_bank_bank_id FOREIGN KEY (bank_id) REFERENCES bank (id) ON DELETE CASCADE
);

CREATE TABLE bank_registration (
    id uuid NOT NULL,
    bank_id uuid NOT NULL,
    software_statement_profile_id text NOT NULL,
    software_statement_and_certificate_profile_override_case text NULL,
    dynamic_client_registration_api_version text NOT NULL,
    registration_scope text NOT NULL,
    registration_endpoint text NOT NULL,
    token_endpoint text NOT NULL,
    authorization_endpoint text NOT NULL,
    token_endpoint_auth_method text NOT NULL,
    custom_behaviour text NULL,
    external_api_id text NOT NULL,
    external_api_secret text NULL,
    registration_access_token text NULL,
    reference text NULL,
    is_deleted boolean NOT NULL,
    is_deleted_modified timestamp with time zone NOT NULL,
    is_deleted_modified_by text NULL,
    created timestamp with time zone NOT NULL,
    created_by text NULL,
    CONSTRAINT pk_bank_registration PRIMARY KEY (id),
    CONSTRAINT fk_bank_registration_bank_bank_id FOREIGN KEY (bank_id) REFERENCES bank (id) ON DELETE CASCADE
);

CREATE TABLE payment_initiation_api (
    id uuid NOT NULL,
    bank_id uuid NOT NULL,
    api_version text NOT NULL,
    base_url text NOT NULL,
    reference text NULL,
    is_deleted boolean NOT NULL,
    is_deleted_modified timestamp with time zone NOT NULL,
    is_deleted_modified_by text NULL,
    created timestamp with time zone NOT NULL,
    created_by text NULL,
    CONSTRAINT pk_payment_initiation_api PRIMARY KEY (id),
    CONSTRAINT fk_payment_initiation_api_bank_bank_id FOREIGN KEY (bank_id) REFERENCES bank (id) ON DELETE CASCADE
);

CREATE TABLE variable_recurring_payments_api (
    id uuid NOT NULL,
    bank_id uuid NOT NULL,
    api_version text NOT NULL,
    base_url text NOT NULL,
    reference text NULL,
    is_deleted boolean NOT NULL,
    is_deleted_modified timestamp with time zone NOT NULL,
    is_deleted_modified_by text NULL,
    created timestamp with time zone NOT NULL,
    created_by text NULL,
    CONSTRAINT pk_variable_recurring_payments_api PRIMARY KEY (id),
    CONSTRAINT fk_variable_recurring_payments_api_bank_bank_id FOREIGN KEY (bank_id) REFERENCES bank (id) ON DELETE CASCADE
);

CREATE TABLE account_access_consent (
    id uuid NOT NULL,
    bank_registration_id uuid NOT NULL,
    account_and_transaction_api_id uuid NOT NULL,
    external_api_id text NOT NULL,
    access_token_access_token text NULL,
    access_token_expires_in integer NOT NULL,
    access_token_refresh_token text NULL,
    access_token_modified timestamp with time zone NOT NULL,
    access_token_modified_by text NULL,
    reference text NULL,
    is_deleted boolean NOT NULL,
    is_deleted_modified timestamp with time zone NOT NULL,
    is_deleted_modified_by text NULL,
    created timestamp with time zone NOT NULL,
    created_by text NULL,
    CONSTRAINT pk_account_access_consent PRIMARY KEY (id),
    CONSTRAINT fk_account_access_consent_account_and_transaction_api_account_a FOREIGN KEY (account_and_transaction_api_id) REFERENCES account_and_transaction_api (id) ON DELETE CASCADE,
    CONSTRAINT fk_account_access_consent_bank_registration_bank_registration_id FOREIGN KEY (bank_registration_id) REFERENCES bank_registration (id) ON DELETE CASCADE
);

CREATE TABLE domestic_payment_consent (
    id uuid NOT NULL,
    bank_registration_id uuid NOT NULL,
    payment_initiation_api_id uuid NOT NULL,
    external_api_id text NOT NULL,
    access_token_access_token text NULL,
    access_token_expires_in integer NOT NULL,
    access_token_refresh_token text NULL,
    access_token_modified timestamp with time zone NOT NULL,
    access_token_modified_by text NULL,
    reference text NULL,
    is_deleted boolean NOT NULL,
    is_deleted_modified timestamp with time zone NOT NULL,
    is_deleted_modified_by text NULL,
    created timestamp with time zone NOT NULL,
    created_by text NULL,
    CONSTRAINT pk_domestic_payment_consent PRIMARY KEY (id),
    CONSTRAINT fk_domestic_payment_consent_bank_registration_bank_registration FOREIGN KEY (bank_registration_id) REFERENCES bank_registration (id) ON DELETE CASCADE,
    CONSTRAINT fk_domestic_payment_consent_payment_initiation_api_payment_init FOREIGN KEY (payment_initiation_api_id) REFERENCES payment_initiation_api (id) ON DELETE CASCADE
);

CREATE TABLE domestic_vrp_consent (
    id uuid NOT NULL,
    bank_registration_id uuid NOT NULL,
    variable_recurring_payments_api_id uuid NOT NULL,
    external_api_id text NOT NULL,
    access_token_access_token text NULL,
    access_token_expires_in integer NOT NULL,
    access_token_refresh_token text NULL,
    access_token_modified timestamp with time zone NOT NULL,
    access_token_modified_by text NULL,
    reference text NULL,
    is_deleted boolean NOT NULL,
    is_deleted_modified timestamp with time zone NOT NULL,
    is_deleted_modified_by text NULL,
    created timestamp with time zone NOT NULL,
    created_by text NULL,
    CONSTRAINT pk_domestic_vrp_consent PRIMARY KEY (id),
    CONSTRAINT fk_domestic_vrp_consent_bank_registration_bank_registration_id FOREIGN KEY (bank_registration_id) REFERENCES bank_registration (id) ON DELETE CASCADE,
    CONSTRAINT fk_domestic_vrp_consent_variable_recurring_payments_api_variab FOREIGN KEY (variable_recurring_payments_api_id) REFERENCES variable_recurring_payments_api (id) ON DELETE CASCADE
);

CREATE TABLE auth_context (
    id uuid NOT NULL,
    discriminator text NOT NULL,
    account_access_consent_id uuid NULL,
    domestic_payment_consent_id uuid NULL,
    domestic_vrp_consent_id uuid NULL,
    reference text NULL,
    is_deleted boolean NOT NULL,
    is_deleted_modified timestamp with time zone NOT NULL,
    is_deleted_modified_by text NULL,
    created timestamp with time zone NOT NULL,
    created_by text NULL,
    CONSTRAINT pk_auth_context PRIMARY KEY (id),
    CONSTRAINT fk_auth_context_account_access_consent_account_access_consent_id FOREIGN KEY (account_access_consent_id) REFERENCES account_access_consent (id) ON DELETE CASCADE,
    CONSTRAINT fk_auth_context_domestic_payment_consent_domestic_payment_consen FOREIGN KEY (domestic_payment_consent_id) REFERENCES domestic_payment_consent (id) ON DELETE CASCADE,
    CONSTRAINT fk_auth_context_domestic_vrp_consent_domestic_vrp_consent_id FOREIGN KEY (domestic_vrp_consent_id) REFERENCES domestic_vrp_consent (id) ON DELETE CASCADE
);

CREATE INDEX ix_account_access_consent_account_and_transaction_api_id ON account_access_consent (account_and_transaction_api_id);

CREATE INDEX ix_account_access_consent_bank_registration_id ON account_access_consent (bank_registration_id);

CREATE INDEX ix_account_and_transaction_api_bank_id ON account_and_transaction_api (bank_id);

CREATE INDEX ix_auth_context_account_access_consent_id ON auth_context (account_access_consent_id);

CREATE INDEX ix_auth_context_domestic_payment_consent_id ON auth_context (domestic_payment_consent_id);

CREATE INDEX ix_auth_context_domestic_vrp_consent_id ON auth_context (domestic_vrp_consent_id);

CREATE INDEX ix_bank_registration_bank_id ON bank_registration (bank_id);

CREATE INDEX ix_domestic_payment_consent_bank_registration_id ON domestic_payment_consent (bank_registration_id);

CREATE INDEX ix_domestic_payment_consent_payment_initiation_api_id ON domestic_payment_consent (payment_initiation_api_id);

CREATE INDEX ix_domestic_vrp_consent_bank_registration_id ON domestic_vrp_consent (bank_registration_id);

CREATE INDEX ix_domestic_vrp_consent_variable_recurring_payments_api_id ON domestic_vrp_consent (variable_recurring_payments_api_id);

CREATE INDEX ix_payment_initiation_api_bank_id ON payment_initiation_api (bank_id);

CREATE INDEX ix_variable_recurring_payments_api_bank_id ON variable_recurring_payments_api (bank_id);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20220420094645_InitialCreate', '6.0.4');

COMMIT;

