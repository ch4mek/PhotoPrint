import { CreateConversationFileDto, DocumentResponse } from '@/types/document';

export const convertIdsToConversationFiles = (
	ids: DocumentResponse[],
	documents: File[]
): CreateConversationFileDto[] =>
	ids.map(({ fileId }, index: number) => {
		const { name, size } = documents[index];

		return {
			fileId: String(fileId),
			fileName: name,
			fileSize: size,
		};
	});
