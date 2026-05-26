-- ============================================================
-- FIX_GSRS_POLICIES.sql
-- Soluciona el error 42710 del proyecto GSRS-UNSCH
-- (NO afecta al chatbot 3D, solo arregla las políticas duplicadas)
-- ============================================================
-- USO: Ejecutar solo si necesitas RE-aplicar el SQL del GSRS
-- ============================================================

-- Eliminar políticas existentes (para poder recrearlas)
DROP POLICY IF EXISTS "Insertar estudiante público" ON public.estudiantes;
DROP POLICY IF EXISTS "Insertar sesión público"    ON public.gsrs_sesiones;
DROP POLICY IF EXISTS "Insertar respuesta público" ON public.gsrs_respuestas;
DROP POLICY IF EXISTS "Insertar contexto público"  ON public.contexto_estudiante;

DROP POLICY IF EXISTS "Leer solo autenticados"          ON public.estudiantes;
DROP POLICY IF EXISTS "Leer sesiones autenticados"      ON public.gsrs_sesiones;
DROP POLICY IF EXISTS "Leer respuestas autenticados"    ON public.gsrs_respuestas;
DROP POLICY IF EXISTS "Leer contexto autenticados"      ON public.contexto_estudiante;

-- Recrear políticas
CREATE POLICY "Insertar estudiante público"
    ON public.estudiantes FOR INSERT WITH CHECK (true);
CREATE POLICY "Insertar sesión público"
    ON public.gsrs_sesiones FOR INSERT WITH CHECK (true);
CREATE POLICY "Insertar respuesta público"
    ON public.gsrs_respuestas FOR INSERT WITH CHECK (true);
CREATE POLICY "Insertar contexto público"
    ON public.contexto_estudiante FOR INSERT WITH CHECK (true);

CREATE POLICY "Leer solo autenticados"
    ON public.estudiantes FOR SELECT USING (auth.role() = 'authenticated');
CREATE POLICY "Leer sesiones autenticados"
    ON public.gsrs_sesiones FOR SELECT USING (auth.role() = 'authenticated');
CREATE POLICY "Leer respuestas autenticados"
    ON public.gsrs_respuestas FOR SELECT USING (auth.role() = 'authenticated');
CREATE POLICY "Leer contexto autenticados"
    ON public.contexto_estudiante FOR SELECT USING (auth.role() = 'authenticated');

-- Verificar
SELECT schemaname, tablename, policyname
FROM pg_policies
WHERE schemaname = 'public'
  AND tablename IN ('estudiantes', 'gsrs_sesiones', 'gsrs_respuestas', 'contexto_estudiante')
ORDER BY tablename, policyname;
