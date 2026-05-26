-- ============================================================
-- SUPABASE_SETUP.sql — CHATBOT 3D
-- Script IDEMPOTENTE (ejecutable múltiples veces sin error)
-- ============================================================
-- Crea SOLO las tablas que necesita el chatbot 3D.
-- NO toca tus tablas del proyecto GSRS-UNSCH.
--
-- Cómo ejecutarlo:
--   1. https://supabase.com/dashboard → tu proyecto
--   2. SQL Editor → New Query
--   3. Pegar TODO este script → Run
-- ============================================================

-- ─── PASO 1: Eliminar tablas viejas con esquema incorrecto ──
-- (Si existen con columnas erróneas como "user_message", "ai_response")
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_name = 'conversations'
          AND column_name = 'user_message'
    ) THEN
        DROP TABLE IF EXISTS public.chat_messages CASCADE;
        DROP TABLE IF EXISTS public.conversations CASCADE;
        DROP TABLE IF EXISTS public.sessions CASCADE;
        RAISE NOTICE 'Tablas viejas con esquema incorrecto fueron eliminadas';
    END IF;
END $$;

-- ─── PASO 2: Crear tabla conversations (esquema correcto) ───
CREATE TABLE IF NOT EXISTS public.conversations (
    id          BIGSERIAL PRIMARY KEY,
    user_id     TEXT NOT NULL DEFAULT 'anonymous',
    title       TEXT NOT NULL DEFAULT 'Nueva conversación',
    created_at  TIMESTAMPTZ DEFAULT NOW(),
    updated_at  TIMESTAMPTZ DEFAULT NOW()
);

-- ─── PASO 3: Crear tabla messages ────────────────────────────
CREATE TABLE IF NOT EXISTS public.messages (
    id                BIGSERIAL PRIMARY KEY,
    conversation_id   BIGINT NOT NULL
                      REFERENCES public.conversations(id) ON DELETE CASCADE,
    role              TEXT NOT NULL
                      CHECK (role IN ('user', 'assistant', 'system')),
    content           TEXT NOT NULL,
    animation         TEXT,
    facial_expression TEXT,
    created_at        TIMESTAMPTZ DEFAULT NOW()
);

-- ─── PASO 4: Índices ─────────────────────────────────────────
CREATE INDEX IF NOT EXISTS idx_conversations_user_id
    ON public.conversations(user_id);

CREATE INDEX IF NOT EXISTS idx_conversations_updated_at
    ON public.conversations(updated_at DESC);

CREATE INDEX IF NOT EXISTS idx_messages_conversation_id
    ON public.messages(conversation_id);

CREATE INDEX IF NOT EXISTS idx_messages_created_at
    ON public.messages(created_at);

-- ─── PASO 5: RLS + Políticas IDEMPOTENTES ────────────────────
ALTER TABLE public.conversations ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.messages      ENABLE ROW LEVEL SECURITY;

-- Eliminar políticas previas (si existen) para evitar error 42710
DROP POLICY IF EXISTS "chatbot_conv_all_public" ON public.conversations;
DROP POLICY IF EXISTS "chatbot_msg_all_public"  ON public.messages;

-- Recrear políticas (permitir todo desde service_role del backend)
CREATE POLICY "chatbot_conv_all_public"
    ON public.conversations
    FOR ALL
    USING (true)
    WITH CHECK (true);

CREATE POLICY "chatbot_msg_all_public"
    ON public.messages
    FOR ALL
    USING (true)
    WITH CHECK (true);

-- ─── PASO 6: Verificación final ──────────────────────────────
SELECT
    'conversations' AS tabla,
    COUNT(*) AS columnas,
    array_agg(column_name ORDER BY ordinal_position) AS columnas_lista
FROM information_schema.columns
WHERE table_schema = 'public' AND table_name = 'conversations'
GROUP BY table_name

UNION ALL

SELECT
    'messages' AS tabla,
    COUNT(*) AS columnas,
    array_agg(column_name ORDER BY ordinal_position) AS columnas_lista
FROM information_schema.columns
WHERE table_schema = 'public' AND table_name = 'messages'
GROUP BY table_name;
