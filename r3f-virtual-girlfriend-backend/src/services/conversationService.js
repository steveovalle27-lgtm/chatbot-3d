// src/services/conversationService.js
// Repository Pattern para operaciones de conversaciones en Supabase

export class ConversationService {
  constructor(supabaseClient) {
    if (!supabaseClient) throw new Error("supabaseClient is required");
    this.supabase = supabaseClient;
  }

  /**
   * Lista todas las conversaciones de un usuario
   */
  async listByUser(userId) {
    const { data, error } = await this.supabase
      .from("conversations")
      .select("*")
      .eq("user_id", userId)
      .order("updated_at", { ascending: false });

    if (error) throw new Error(error.message);
    return data || [];
  }

  /**
   * Crea una nueva conversación
   */
  async create(userId, title = "Nueva conversación") {
    const { data, error } = await this.supabase
      .from("conversations")
      .insert({ user_id: userId, title })
      .select()
      .single();

    if (error) throw new Error(error.message);
    return data;
  }

  /**
   * Obtiene los mensajes de una conversación
   */
  async getMessages(conversationId) {
    const { data, error } = await this.supabase
      .from("messages")
      .select("*")
      .eq("conversation_id", conversationId)
      .order("created_at", { ascending: true });

    if (error) throw new Error(error.message);
    return data || [];
  }

  /**
   * Borra una conversación
   */
  async delete(conversationId) {
    const { error } = await this.supabase
      .from("conversations")
      .delete()
      .eq("id", conversationId);

    if (error) throw new Error(error.message);
    return { success: true };
  }

  /**
   * Guarda un mensaje en una conversación
   */
  async saveMessage(conversationId, role, content, extras = {}) {
    if (!conversationId) throw new Error("conversationId is required");
    if (!["user", "assistant", "system"].includes(role)) {
      throw new Error(`Invalid role: ${role}`);
    }

    const { error } = await this.supabase.from("messages").insert({
      conversation_id: conversationId,
      role,
      content,
      ...extras,
    });

    if (error) throw new Error(error.message);
    return { success: true };
  }

  /**
   * Actualiza el título de una conversación (solo si era el placeholder)
   */
  async updateTitleIfDefault(conversationId, newTitle) {
    const { error } = await this.supabase
      .from("conversations")
      .update({
        title: newTitle.slice(0, 50),
        updated_at: new Date().toISOString(),
      })
      .eq("id", conversationId)
      .eq("title", "Nueva conversación");

    if (error) throw new Error(error.message);
    return { success: true };
  }
}
