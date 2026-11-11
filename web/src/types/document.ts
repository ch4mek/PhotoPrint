import { DocumentFileTypes } from './file';

export interface ConversationDocument {
	id: number;
	fileId: string;
	fileName: string;
	fileType: string;
	fileSize: number;
	conversationId: number | null;
	performanceReviewId: number | null;
}

export interface CreateConversationFileDto {
	fileId: string;
	fileName: string;
	fileSize: number;
}

export type ConversationFormDocument = CreateConversationFileDto & {
	id?: number;
	fileType?: string;
	file?: File;
	conversationId?: number | null;
	performanceReviewId?: number | null;
};

export interface Document {
	conversationId: number;
	fileId: string;
	fileName: string;
	fileSize: number;
	fileType: DocumentFileTypes;
	id: number;
	performanceReviewId: number | null;
}

export interface DocumentResponse {
	fileId: string | number;
}
