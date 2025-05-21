-- infra/postgres-init/init-dbs.sql

-- Cria o banco de dados para o serviço de Lançamentos
DO
$$
BEGIN
   IF NOT EXISTS (SELECT FROM pg_database WHERE datname = 'flowwise_lancamentos_db') THEN
      CREATE DATABASE flowwise_lancamentos_db;
      RAISE NOTICE 'Database flowwise_lancamentos_db created.';
   ELSE
      RAISE NOTICE 'Database flowwise_lancamentos_db already exists.';
   END IF;
END
$$;

-- Cria o banco de dados para o serviço de Consolidação
DO
$$
BEGIN
   IF NOT EXISTS (SELECT FROM pg_database WHERE datname = 'flowwise_consolidacao_db') THEN
      CREATE DATABASE flowwise_consolidacao_db;
      RAISE NOTICE 'Database flowwise_consolidacao_db created.';
   ELSE
      RAISE NOTICE 'Database flowwise_consolidacao_db already exists.';
   END IF;
END
$$;

-- Opcional: Criar um usuário específico para as aplicações se não for usar 'flowwise_user'
/*
DO
$$
BEGIN
   IF NOT EXISTS (SELECT FROM pg_user WHERE usename = 'flowwise_user') THEN
      CREATE USER flowwise_user WITH PASSWORD 'flowwise_password';
      GRANT ALL PRIVILEGES ON DATABASE flowwise_lancamentos_db TO flowwise_user;
      GRANT ALL PRIVILEGES ON DATABASE flowwise_consolidacao_db TO flowwise_user;
      RAISE NOTICE 'User flowwise_user created and granted privileges.';
   ELSE
      RAISE NOTICE 'User flowwise_user already exists.';
   END IF;
END
$$;
*/
