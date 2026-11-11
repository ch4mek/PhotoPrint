import { ConversationFormDocument } from '@/types/document';

export const getPreviousConversationFiles = (
	documents: ConversationFormDocument[]
) => documents.filter((document) => !document.file);
